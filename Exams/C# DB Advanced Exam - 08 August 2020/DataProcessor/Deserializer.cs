namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            HashSet<GameDeserializeDTO> gameDeserializeDTOs = JsonConvert
                .DeserializeObject<HashSet<GameDeserializeDTO>>(jsonString);

            HashSet<Game> games = new HashSet<Game>();

            StringBuilder sb = new StringBuilder();

            int currGameID = 1;
            foreach (GameDeserializeDTO gameDTO in gameDeserializeDTOs)
            {
                DateTime date;
                decimal price;

                Developer dev = null;
                Genre genre = null;

                if (string.IsNullOrEmpty(gameDTO.Name) || gameDTO.Name == " "
                    || !decimal.TryParse(gameDTO.Price, out price) || decimal.Parse(gameDTO.Price) < 0
                    || string.IsNullOrEmpty(gameDTO.Developer)
                    || gameDTO.Developer == " " || string.IsNullOrEmpty(gameDTO.ReleaseDate)
                    || gameDTO.ReleaseDate == " " || string.IsNullOrEmpty(gameDTO.Genre)
                    || gameDTO.Genre == " " || gameDTO.Tags.Length == 0
                    || gameDTO == null || !DateTime.TryParse(gameDTO.ReleaseDate, out date))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                dev = DeveloperService(gameDTO.Developer, context);

                genre = GenreService(gameDTO.Genre, context);

                AddTag(gameDTO.Tags, context);

                Game game = CreateNewGame(gameDTO, dev, genre);

                AddTagsToTheGame(game, gameDTO.Tags, context, currGameID++);

                games.Add(game);

                sb.AppendLine($"Added {game.Name} ({gameDTO.Genre}) with {gameDTO.Tags.Length} tags");
            }
            context.Games.AddRange(games);
            context.SaveChanges();
            string result = sb.ToString().TrimEnd();
            return result;
        }
        public static Game CreateNewGame(GameDeserializeDTO gameDTO, Developer dev, Genre genre)
        {
            return new Game()
            {
                Name = gameDTO.Name,
                Price = decimal.Parse(gameDTO.Price),
                ReleaseDate = DateTime.Parse(gameDTO.ReleaseDate, CultureInfo.InvariantCulture),
                Developer = dev,
                Genre = genre,
            };
        }
        public static Developer DeveloperService(string devName, VaporStoreDbContext context)
        {
            Developer dev = DeveloperIsExist(devName, context);

            if (dev == null)
            {
                dev = AddDeveloper(devName, context);
            }
            return dev;
        }
        public static Genre GenreService(string genreType, VaporStoreDbContext context)
        {
            Genre genre = GenerIsExist(genreType, context);

            if (genre == null)
            {
                genre = AddGenre(genreType, context);
            }

            return genre;
        }
        public static void AddTagsToTheGame(Game Game, string[] tags, VaporStoreDbContext context, int gameID)
        {
            List<int> DbTags = context
                .Tags
                .AsNoTracking()
                .Where(t => tags.Contains(t.Name))
                .Select(x => x.Id)
                .ToList();

            List<GameTag> GameTags = new List<GameTag>();

            foreach (int tagID in DbTags)
            {
                GameTags.Add(new GameTag()
                {
                    GameId = gameID,
                    TagId = tagID,
                });
            }
            Game.GameTags = GameTags;
            context.SaveChanges();
        }

        public static void AddTag(string[] tagNames, VaporStoreDbContext context)
        {
            HashSet<string> dbTags = context
                .Tags
                .AsNoTracking()
                .Select(x => x.Name)
                .ToHashSet();

            HashSet<Tag> newTags = new HashSet<Tag>();

            foreach (string tag in tagNames)
            {
                if (!dbTags.Where(t => t == tag).Any())
                {
                    Tag newTag = new Tag()
                    {
                        Name = tag
                    };
                    newTags.Add(newTag);
                }
            }
            context.Tags.AddRange(newTags);
            context.SaveChanges();
        }
        public static Genre GenerIsExist(string generName, VaporStoreDbContext context)
        {
            return context.Genres
                .Where(g => g.Name == generName)
                .FirstOrDefault();
        }

        public static Genre AddGenre(string generName, VaporStoreDbContext context)
        {
            Genre genre = new Genre()
            {
                Name = generName
            };
            context.Genres.Add(genre);
            context.SaveChanges();
            return genre;
        }
        public static Developer DeveloperIsExist(string devName, VaporStoreDbContext context)
        {
            return context.Developers
                      .Where(x => x.Name == devName)
                      .FirstOrDefault();
        }
        public static Developer AddDeveloper(string devName, VaporStoreDbContext context)
        {
            Developer developer = new Developer()
            {
                Name = devName,
            };
            context.Developers.Add(developer);
            return developer;
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            HashSet<UserImportDTO> userImportDTOs = JsonConvert.DeserializeObject<HashSet<UserImportDTO>>(jsonString);

            HashSet<User> users = new HashSet<User>();

            StringBuilder sb = new StringBuilder();

            string namePattern = @"^[A-Z][a-z]*\s[A-Z][a-z]*$";

            int userID = 1;
            foreach (UserImportDTO userImportDTO in userImportDTOs)
            {
                bool validUserName = Regex.IsMatch(userImportDTO.FullName, namePattern);
                if (!validUserName || userImportDTO.Username.Length < 3 ||
                    userImportDTO.Username.Length > 20 || string.IsNullOrWhiteSpace(userImportDTO.Email)
                    || userImportDTO.Age < 3 || userImportDTO.Age > 103
                    || userImportDTO.Cards.Length == 0
                   || !CheckUserCards(userImportDTO.Cards))

                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                User user = CreateNewUser(userImportDTO.Username, userImportDTO.FullName,
                                             userImportDTO.Email, userImportDTO.Age);

                HashSet<Card> cards = CreateCards(userImportDTO.Cards, userID++);

                user.Cards = cards;
                users.Add(user);
                sb.AppendLine($"Imported {userImportDTO.Username} with {cards.Count} cards");
            }
            context.Users.AddRange(users);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }
        public static HashSet<Card> CreateCards(CardImportDTO[] cards, int userID)
        {
            HashSet<Card> newCards = new HashSet<Card>();

            foreach (CardImportDTO card in cards)
            {
                newCards.Add(new Card()
                {
                    Number = card.Number,
                    Cvc = card.CVC,
                    Type = card.Type != "Debit" ? CardType.Credit : CardType.Debit,
                    UserId = 1
                });
            }
            return newCards;
        }
        public static User CreateNewUser(string username, string fullName, string email, int age)
        {
            return new User()
            {
                Username = username,
                FullName = fullName,
                Email = email,
                Age = age
            };
        }
        public static bool CheckUserCards(CardImportDTO[] cards)
        {
            string cardNumberPattern = @"^[0-9]{4}\s[0-9]{4}\s[0-9]{4}\s[0-9]{4}$";
            string cardCvcPattern = @"^[0-9]{3}$";
            foreach (CardImportDTO card in cards)
            {
                bool validCardNumber = Regex.IsMatch(card.Number, cardNumberPattern);
                bool validCvcNumber = Regex.IsMatch(card.CVC, cardCvcPattern);
                if (!validCardNumber || !validCvcNumber || card.Type != "Debit" && card.Type != "Credit")
                {
                    return false;
                }
            }
            return true;
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute rootAttribute = new XmlRootAttribute("Purchases");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PurchaseImportDTO[]), rootAttribute);

            using StringReader reader = new StringReader(xmlString);

            PurchaseImportDTO[] purchaseImportDTOs = (PurchaseImportDTO[])xmlSerializer.Deserialize(reader);

            HashSet<Purchase> purchases = new HashSet<Purchase>();

            foreach (PurchaseImportDTO purchaseDTO in purchaseImportDTOs)
            {
                if (!IsValid(purchaseDTO))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                if (purchaseDTO.Type != "Retail" && purchaseDTO.Type != "Digital")
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                bool isValidDate = DateTime.TryParseExact(purchaseDTO.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime ParsedDate);

                if (!isValidDate)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Card card = context.Cards
                    .Where(c => c.Number == purchaseDTO.Card)
                    .FirstOrDefault();

                if (card == null)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Game game = context.Games
                    .Where(g => g.Name == purchaseDTO.Title)
                    .FirstOrDefault();

                if (game == null)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Purchase purchase = new Purchase()
                {
                    Type = purchaseDTO.Type != "Retail" ? PurchaseType.Digital : PurchaseType.Retail,
                    ProductKey = purchaseDTO.Key,
                    Date = ParsedDate,
                    CardId = card.Id,
                    GameId = game.Id
                };
                purchases.Add(purchase);
                sb.AppendLine($"Imported {game.Name} for {card.User.Username}");
            }
            context.Purchases.AddRange(purchases);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();

            return sb.ToString().TrimEnd();
        }


        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}