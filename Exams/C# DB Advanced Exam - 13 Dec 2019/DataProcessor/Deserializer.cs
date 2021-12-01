namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            XmlRootAttribute root = new XmlRootAttribute("Books");
            XmlSerializer serializer = new XmlSerializer(typeof(BookImportDTO[]), root);

            StringBuilder sb = new StringBuilder();

            StringReader sw = new StringReader(xmlString);

            BookImportDTO[] booksDTOS = (BookImportDTO[])serializer.Deserialize(sw);

            HashSet<Book> books = new HashSet<Book>();

            foreach (BookImportDTO bookDTO in booksDTOS)
            {
                if (!IsValid(bookDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isValidPublishedDate = DateTime.TryParseExact(bookDTO.PublishedOn,
                    "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime PublishedDate);

                if (!isValidPublishedDate)
                {
                    sb.AppendLine("ErrorMessage");
                    continue;
                }
                Book book = new Book()
                {
                    Name = bookDTO.Name,
                    Genre = (Genre)bookDTO.Genre,
                    Price = bookDTO.Price,
                    Pages = bookDTO.Pages,
                    PublishedOn = PublishedDate,
                };
                books.Add(book);
                sb.AppendLine(String.Format(SuccessfullyImportedBook, book.Name, book.Price));
            }
            context.Books.AddRange(books);
            context.SaveChanges();
            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            AuthorImportDTO[] authorImports = JsonConvert
                .DeserializeObject<AuthorImportDTO[]>(jsonString);

            HashSet<Author> authors = new HashSet<Author>();

            foreach (AuthorImportDTO authorDTO in authorImports)
            {
                if (!IsValid(authorDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (ExistEmail(authorDTO.Email, context))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                HashSet<AuthorBook> authorBooks = new HashSet<AuthorBook>();

                Author author = new Author()
                {
                    FirstName = authorDTO.FirstName,
                    LastName = authorDTO.LastName,
                    Email = authorDTO.Email,
                    Phone = authorDTO.Phone,
                    AuthorsBooks = authorBooks
                };

                foreach (AuthorBookImportDTO item in authorDTO.Books)
                {
                    bool isValiBookID = int.TryParse(item.Id, out int BookId);
                    if (!isValiBookID)
                    {
                        continue;
                    }
                    Book book = ExistBook(BookId, context);
                    if (book == null)
                    {
                        continue;
                    }
                    authorBooks.Add(new AuthorBook()
                    {
                        Author = author,
                        Book = book
                    });
                }

                if (authorBooks.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                sb.AppendLine(String.Format(SuccessfullyImportedAuthor, $"{author.FirstName} {author.LastName}", author.AuthorsBooks.Count));
                authors.Add(author);
            }
            context.Authors.AddRange(authors);
            context.SaveChanges();
            string result = sb.ToString().TrimEnd();
            return result;
        }
        public static bool ExistEmail(string email, BookShopContext context)
        {
            return context.Authors.Where(a => a.Email == email).Any();
        }
        public static Book ExistBook(int bookI, BookShopContext context)
        {
            return context.Books.Where(b => b.Id == bookI).FirstOrDefault();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}