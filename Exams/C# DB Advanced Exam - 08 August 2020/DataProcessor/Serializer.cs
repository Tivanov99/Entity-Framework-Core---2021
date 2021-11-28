namespace VaporStore.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
            };


            GenerExportDTO[] exportDTOs = context
                .Genres
                .ToArray()
                .Where(g => genreNames.Contains(g.Name))
                .Select(g => new GenerExportDTO()
                {
                    Id = g.Id,
                    Genre = g.Name,
                    Games = g.Games
                    .Where(game => game.Purchases.Any())
                    .Select(game => new GamesExportDTO()
                    {
                        Id = game.Id,
                        Title = game.Name,
                        Developer = game.Developer.Name,
                        Tags = string.Join(", ", game.GameTags.Select(gt => gt.Tag.Name)),
                        Players = game.Purchases.Count()
                    }).ToArray()
                    .OrderByDescending(x => x.Players)
                    .ThenBy(g => g.Id)
                    .ToArray(),
                    TotalPlayers = g.Games.Sum(x => x.Purchases.Count)
                })
                .OrderByDescending(genre => genre.TotalPlayers)
                .ThenBy(genre => genre.Id)
                .ToArray();

            string result = JsonConvert.SerializeObject(exportDTOs, settings);

            return result;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            XmlRootAttribute xmlRootAttribute = new XmlRootAttribute("Users");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserExportDTO[]), xmlRootAttribute);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

            ns.Add(string.Empty, string.Empty);

            UserExportDTO[] users = context
                .Users
                .ToArray()
                .Select(u => new UserExportDTO()
                {
                    Username = u.Username,
                    Purchases = u.Cards.SelectMany(x => x.Purchases.Where(p => p.Type.ToString() == storeType))
                    .OrderBy(x=>x.Date)
                    .Select(x => new PurchaseExportDTO()
                    {

                        Card = x.Card.Number,
                        Cvc = x.Card.Cvc,
                        Date = x.Date.ToString("yyyy-MM-dd HH:mm"),
                        Game = new GameExportDTO()
                        {
                            Title=x.Game.Name,
                            Genre = x.Game.Genre.Name,
                            Price = x.Game.Price
                        },
                    })
                    .ToArray(),
                    TotalSpent = u.Cards.SelectMany(x => x.Purchases.Where(p => p.Type.ToString() == storeType)).Sum(g => g.Game.Price).ToString()
                })
                .ToArray()
                .OrderByDescending(u => u.Purchases.Sum(p => p.Game.Price))
                .ThenBy(u => u.Username)
                .Where(x => x.Purchases.Any())
                .ToArray();

            StringBuilder sb = new StringBuilder();

            using StringWriter sw = new StringWriter(sb);
            xmlSerializer.Serialize(sw, users,ns);
            string da = sb.ToString().TrimEnd();
            return sb.ToString().TrimEnd();
        }
        public static DateTime ParseDate(DateTime dateTime)
        {
            DateTime.TryParseExact(dateTime.ToString(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime ParsedDate);

            return ParsedDate;
        }
    }
}