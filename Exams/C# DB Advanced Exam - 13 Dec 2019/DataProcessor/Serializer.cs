namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            HashSet<AuthorExportDTO> authorExports = context
                .Authors
                .ToHashSet()
                .Select(a => new AuthorExportDTO()
                {
                    AuthorName = $"{a.FirstName} {a.LastName}",
                    Books = a.AuthorsBooks.Select(ab => ab.Book)
                    .OrderByDescending(b => b.Price)
                    .ToArray()
                    .Select(b => new AuthorBooksExportDTO()
                    {
                        BookName = b.Name,
                        BookPrice = b.Price.ToString("f2")
                    })
                    .ToArray()
                })
                .ToHashSet()
                .OrderByDescending(a => a.Books.Count())
                .ThenByDescending(a => a.AuthorName)
                .ToHashSet();

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
            };
            string result = JsonConvert.SerializeObject(authorExports, settings);
            return result;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            XmlRootAttribute root = new XmlRootAttribute("Books");

            XmlSerializer serializer =
                new XmlSerializer(typeof(BookExportDTO[]), root);

            StringBuilder sb = new StringBuilder();

            StringWriter sw = new StringWriter(sb);

            BookExportDTO[] bookExports = context
                .Books
                .ToArray()
                .Where(b => b.PublishedOn < date && b.Genre == Genre.Science)
                .OrderByDescending(b => b.Pages)
                .ThenByDescending(b => b.PublishedOn)
                .Select(b => new BookExportDTO()
                {
                    Pages = b.Pages,
                    Name = b.Name,
                    Date = b.PublishedOn.ToString("d",CultureInfo.InvariantCulture)
                })
                .Take(10)
                .ToArray();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

             serializer.Serialize(sw,bookExports, ns);

            string result = sb.ToString().TrimEnd();

            return result;
        }
    }
}