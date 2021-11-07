namespace BookShop
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using BookShop.Initializer;
    using BookShop.MapperProfiles;
    using BookShop.Models;
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
            using var DBcontext = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile(new BookInfoDTO())
            );
            ;
            var mapper = config.CreateMapper();

            MapBooks(DBcontext, config);
        }
        public static void MapBooks(BookShopContext context, MapperConfiguration mapper)
        {
            BookInfoDTO[] books = context
                .Books
                .ProjectTo<BookInfoDTO>(mapper)
                .ToArray();
            foreach (var book in books)
            {
                Console.WriteLine($"{book.Title} {book.AuthorFullName}");
            }
        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var config = new MapperConfiguration(cfg =>
             cfg.CreateMap<Book, BookDTO>()
             );

            var mapper = config.CreateMapper();

            AgeRestriction WantedRestriction = CommandConfigurator(command);

            var Books = context.Books
                .Where(x => x.AgeRestriction == WantedRestriction)
                .ProjectTo<BookDTO>(config)
                .ToList()
                .OrderBy(x => x.Title);

            StringBuilder sb = new StringBuilder();
            foreach (var BookTitle in Books)
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
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<Book, BookDTO>()
            );

            var Mapper = config.CreateMapper();

            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(x => x.Copies < 5000 && x.EditionType == EditionType.Gold)
                .OrderBy(x => x.BookId)
                .ProjectTo<BookDTO>(config)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetBooksByPrice(BookShopContext context)
        {
            var config = new MapperConfiguration(cfg =>
            cfg.CreateMap<Book, BookDTO>()
            );

            var Mapper = config.CreateMapper();

            var books = context.Books
                 .Where(x => x.Price > 40)
                .ProjectTo<BookDTO>(config)
                 .OrderByDescending(b => b.Price)
                 .ToList();
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
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            input = input.ToLower();
            var Books = context.
                Books
                .AsNoTracking()
                .Where(b => b.Author.LastName.ToLower().StartsWith(input))
                .OrderBy(b => b.BookId)
                .Select(b => new { b.Title, AuthorFullName = $"{b.Author.FirstName} {b.Author.LastName}" })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var books in Books)
            {
                sb.AppendLine($"{books.Title} ({books.AuthorFullName})");
            }

            return sb.ToString().TrimEnd();
        }
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int CountOfBooks = context
                .Books
                .AsNoTracking()
                .Where(b => b.Title.Length > lengthCheck)
                .Count();
            return CountOfBooks;
        }
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var result = context.Authors
                .Select(author => new
                {
                    FullName = $"{author.FirstName} {author.LastName}",
                    NumberOfCopies = author.Books.Select(book => book.Copies).Sum()
                })
                .OrderByDescending(a => a.NumberOfCopies)
                .ToArray();
            StringBuilder sb = new StringBuilder();

            foreach (var authors in result)
            {
                sb.AppendLine($"{authors.FullName} - {authors.NumberOfCopies}");
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var Categories = context
                 .Categories
                 .AsNoTracking()
                 .Select(c => new
                 {
                     c.Name,
                     CategoryProfit = c.CategoryBooks
                                        .Select(
                                                b => new { b.Book.Price, b.Book.Copies }
                                               )
                                        .Sum(x => x.Copies * x.Price)
                 })
                 .OrderByDescending(c => c.CategoryProfit)
                 .ThenBy(c => c.Name)
                 .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var Category in Categories)
            {
                sb.AppendLine($"{Category.Name} ${Category.CategoryProfit:f2}");
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var result = context
                .Categories
                .AsNoTracking()
                .Select(C => new
                {
                    C.Name,
                    Books = C.CategoryBooks
                    .Select(
                             b => new
                             { b.Book.Title, b.Book.ReleaseDate }
                            )
                    .OrderByDescending(b => b.ReleaseDate)
                    .Take(3)
                })
                .OrderBy(c => c.Name)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var Category in result)
            {
                sb.AppendLine($"--{Category.Name}");
                foreach (var Books in Category.Books)
                {
                    sb.AppendLine($"{Books.Title} ({Books.ReleaseDate.Value.Year})");
                }
            }
            return sb.ToString().TrimEnd();
        }
        public static void IncreasePrices(BookShopContext context)
        {
            Book[] books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year < 2010)
                .Select(b => b)
                .ToArray();

            foreach (var book in books)
            {
                book.Price = 0;
            }
            context.SaveChanges();
        }
        public static int RemoveBooks(BookShopContext context)
        {
            ICollection<Book> books = context.BooksCategories
                .Where(bc => bc.Book.Copies < 4200)
                .Select(x => x.Book)
                .ToList();

            context.Books
                .RemoveRange(books);

            context.SaveChanges();

            return books.Count();
        }
        public static void AddNewBooks(BookShopContext context)
        {
            Author auth = new Author();
            auth.FirstName = "Milen";
            auth.LastName = "Milenov";

            context.Authors.Add(auth);

            context.SaveChanges();

            Book newBook = new Book();
            newBook.Title = "C#";
            newBook.Description = "programming basics";
            newBook.EditionType = EditionType.Gold;
            newBook.Price = 99;
            newBook.Copies = 9999;
            newBook.AgeRestriction = AgeRestriction.Adult;
            newBook.Author = auth;

            context.Books.Add(newBook);
            context.SaveChanges();
        }
        public static void GetAllBooks(BookShopContext context)
        {
            ICollection<Book> AllBooks =
                context
                .Books
                .Select(x => x)
                .ToList();

            foreach (var book in AllBooks)
            {
                Console.WriteLine($"{book.Title} {book.Price}");
            }
        }
    }
}