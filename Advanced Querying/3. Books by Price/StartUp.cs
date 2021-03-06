namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;

    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);
            //string command = Console.ReadLine();
            //GetBooksByAgeRestriction(db, command);
            Console.WriteLine(GetBooksByPrice(db));
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
        //public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        //{

        //}
        //public static string GetBooksByCategory(BookShopContext context, string input)
        //{

        //}
        //public static string GetBooksReleasedBefore(BookShopContext context, string date)
        //{

        //}
        //public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        //{

        //}
        //public static string GetBookTitlesContaining(BookShopContext context, string input)
        //{

        //}
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
