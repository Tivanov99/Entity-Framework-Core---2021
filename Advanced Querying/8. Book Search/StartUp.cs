namespace BookShop
{
    using BookShop.Initializer;
    using BookShop.Models.Enums;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()

        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            string SubString = Console.ReadLine();
            Console.WriteLine(GetBookTitlesContaining(db, SubString));
        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            AgeRestriction WantedRestriction = CommandConfigurator(command);

            var result = context.Books
                .Where(x => x.AgeRestriction == WantedRestriction)
                .ToList()
                .Select(x => new { x.Title })
                .OrderBy(x => x.Title);

            StringBuilder sb = new StringBuilder();
            foreach (var BookTitle in result)
            {
                sb.AppendLine(BookTitle.Title);
            }
            return sb.ToString().TrimEnd();
        }
        public static AgeRestriction CommandConfigurator(string command)
        {
            command = command.ToLower();
            if (command == "minor")
            {
                return AgeRestriction.Minor;
            }
            else if (command == "teen")
            {
                return AgeRestriction.Teen;
            }
            return AgeRestriction.Adult;
        }
        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var books = context.Books
                .Where(x => x.Copies < 5000 && x.EditionType == EditionType.Gold)
                .Select(x => new { x.Title, x.BookId })
                .ToList()
                .OrderBy(x => x.BookId);

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                 .Where(x => x.Price > 40)
                 .Select(b => new { b.Title, b.Price })
                 .ToList()
                 .OrderByDescending(b => b.Price);
            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .Select(b => new { b.BookId, b.Title })
                .ToList()
                .OrderBy(b => b.BookId);

            StringBuilder sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            HashSet<string> Categories = new HashSet<string>(input.ToLower().Split(" "));
            var Titles = context.BooksCategories
                 .Where(x => Categories.Contains(x.Category.Name.ToLower()))
                 .Select(x => x.Book.Title)
                 .OrderBy(x => x)
                 .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var Title in Titles)
            {
                sb.AppendLine(Title);
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime WantedDate = DateTime.Parse(date);

            var ReleasedBooks = context.Books
                .Where(x => x.ReleaseDate.HasValue
                &&
                x.ReleaseDate < WantedDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new { b.Title, b.EditionType, b.Price })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in ReleasedBooks)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var Arthors = context.Authors
                 .Where(a => a.FirstName.EndsWith(input))
                 .Select(a => $"{a.FirstName} {a.LastName}")
                 .ToArray()
                 .OrderBy(x => x);

            StringBuilder sb = new StringBuilder();
            foreach (var author in Arthors)
            {
                //sb.AppendLine($"{author.FirstName} {author.LastName}");
                sb.AppendLine(author);
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            input = input.ToLower();
            var BooksTitles = context.
                Books.Where(x => x.Title.ToLower().Contains(input))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();
            StringBuilder sb = new StringBuilder();

            foreach (var Title in BooksTitles)
            {
                sb.AppendLine(Title);
            }
            return sb.ToString().TrimEnd();
        }
        //public static string GetBooksByAuthor(BookShopContext context, string input)
        //{

        //}
        //public static int CountBooks(BookShopContext context, int lengthCheck)
        //{

        //}

        //public static string CountCopiesByAuthor(BookShopContext context)
        //{

        //}
        //public static string GetTotalProfitByCategory(BookShopContext context)
        //{

        //}
        //public static string GetMostRecentBooks(BookShopContext context)
        //{

        //}
        //public static void IncreasePrices(BookShopContext context)
        //{

        //}
        //public static int RemoveBooks(BookShopContext context)
        //{

        //}
    }
}
