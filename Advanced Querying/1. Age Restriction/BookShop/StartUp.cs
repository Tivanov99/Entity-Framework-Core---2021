namespace BookShop
{
    using Data;
    using Initializer;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);
            string Command = Console.ReadLine();
            Console.WriteLine(GetBooksByAgeRestriction(db, Command));
        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            string FormatedCommand = CommandConfigurator(command);
            var result = context.Books
                .Select(x => new { x.Title, Restriction = x.AgeRestriction.ToString() })
                .ToList()
                .Where(x => x.Restriction == FormatedCommand)
                .OrderBy(x => x.Title);

            StringBuilder sb = new StringBuilder();
            foreach (var BookTitle in result)
            {
                sb.AppendLine(BookTitle.Title);
            }
            return sb.ToString().TrimEnd();
        }
        public static string CommandConfigurator(string command)
        {
            command.ToLower();
            char FirstChar = command[0];
            command = command.Substring(1, command.Length - 1).ToLower();
            return FirstChar.ToString().ToUpper() + command;
        }
        //public static string GetGoldenBooks(BookShopContext context)
        //{

        //}
        //public static string GetBooksByPrice(BookShopContext context)
        //{

        //}
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
