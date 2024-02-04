namespace BookShop
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            //Age Restriction
            string ageRestriction = Console.ReadLine().ToLower();
            ageRestriction = char.ToUpper(ageRestriction[0]) + ageRestriction.Substring(1);

            Console.WriteLine(GetBooksByAgeRestriction(db, ageRestriction));

            //Golden Books
            Console.WriteLine(GetGoldenBooks(db));

            //Books by Price
            Console.WriteLine(GetBooksByPrice(db));

            //Not Released In
            int bookYear = int.Parse(Console.ReadLine());
            Console.WriteLine(GetBooksNotReleasedIn(db, bookYear));

            //Book Titles by Category
            string categories = Console.ReadLine();
            Console.WriteLine(GetBooksByCategory(db, categories));

            //Released Before Date
            string date = Console.ReadLine();
            Console.WriteLine(GetBooksReleasedBefore(db, date));

            //Author Search
            string endingNameString = Console.ReadLine();
            Console.WriteLine(GetAuthorNamesEndingIn(db, endingNameString));

            //Book Search
            string titleString = Console.ReadLine();
            Console.WriteLine(GetBookTitlesContaining(db, titleString));

            //Book Search by Author
            string authorLastNameString = Console.ReadLine().ToLower();
            Console.WriteLine(GetBooksByAuthor(db, authorLastNameString));

            //Count Books
            int lengthCheck = int.Parse(Console.ReadLine());
            Console.WriteLine(CountBooks(db, lengthCheck));

            //Total Book Copies
            Console.WriteLine(CountCopiesByAuthor(db));

            //Profit by Category
            Console.WriteLine(GetTotalProfitByCategory(db));

            //Most Recent Books
            Console.WriteLine(GetMostRecentBooks(db));

            //Increase Prices
            IncreasePrices(db);

            //Remove Books
            Console.WriteLine(RemoveBooks(db));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            if (Enum.TryParse(command, out AgeRestriction ageRestriction))
            {
                var result = context.Books
                    .Where(x => x.AgeRestriction == ageRestriction)
                    .OrderBy(b => b.Title)
                    .Select(b => b.Title)
                    .ToList();

                return string.Join(Environment.NewLine, result);
            }
            else
            {
                return "Invalid age restriction.";
            }
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var result = context.Books
            .Where(b => b.EditionType == (EditionType)Enum.Parse(typeof(EditionType), "Gold")
            && b.Copies < 5000)
            .OrderBy(b => b.BookId)
            .Select(b => b.Title)
            .ToList();

            return string.Join(Environment.NewLine, result);
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var result = context.Books
                .Where(b => b.Price > 40.00M)
                .OrderByDescending(b => b.Price)
                .Select(b => b.Title + " - $" + b.Price)
                .ToList();

            return string.Join(Environment.NewLine, result);
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var result = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, result);
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.Split().Select(x => x.Substring(0, 1).ToUpper() + x.Substring(1).ToLower());

            var result = context.Books
                .Include(b => b.BookCategories)
                .ToList()
                .Where(b => b.BookCategories.Any(b => categories.Contains(b.Category.Name.ToString())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, result);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var result = context.Books
                .Where(b => b.ReleaseDate.Value < DateTime.Parse(date))
                .OrderByDescending(b => b.ReleaseDate)
                .Select(x => $"{x.Title} - {x.EditionType.ToString()} - ${x.Price:F2}")
                .ToList();

            return string.Join(Environment.NewLine, result);
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var reuslt = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => a.FirstName + " " + a.LastName)
                .ToList();

            return string.Join(Environment.NewLine, reuslt);
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var result = context.Books
                .Where(b => b.Title.ToLower().Contains(input))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, result);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var result = context.Books
                .Include(b => b.Author)
                .Where(b => b.Author.LastName.ToLower().StartsWith(input))
                .OrderBy(b => b.BookId)
                .Select(x => $"{x.Title} ({x.Author.FirstName} {x.Author.LastName})")
                .ToList();

            return string.Join(Environment.NewLine, result);
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books
                    .Count(b => b.Title.Length > lengthCheck);
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Books
                .Include(b => b.Author)
                .GroupBy(b => new { b.Author.FirstName, b.Author.LastName })
                .Select(x => new
                {
                    AuthorName = x.Key.FirstName + " " + x.Key.LastName,
                    TotalCopiesCount = x.Sum(c => c.Copies)
                })
                .OrderByDescending(x => x.TotalCopiesCount)
                .ToList();

            var sb = new StringBuilder();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.AuthorName} - {author.TotalCopiesCount}");
            }

            return sb.ToString();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var result = context.BooksCategories
                .Include(c => c.Category)
                .ToList()
                .GroupBy(c => c.Category)
                .Select(x => new
                {
                    Category = x.Key.Name,
                    Profit = x.Sum(b => b.Book.Copies * b.Book.Price)
                })
                .OrderByDescending(p => p.Profit)
                .ThenBy(b => b.Category)
                .Select(x => $"{x.Category} ${x.Profit}")
                .ToList();

            return string.Join(Environment.NewLine, result);
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.BooksCategories
                .Include(c => c.Category)
                .ToList()
                .GroupBy(c => c.Category)
                .Select(x => new
                {
                    Category = x.Key.Name,
                    LatestThreeBooks = x.OrderByDescending(b => b.Book.ReleaseDate)
                                        .Take(3)
                                        .Select(b => $"{b.Book.Title} ({b.Book.ReleaseDate?.Year})")

                })
                .OrderBy(b => b.Category)
                .ToList();

            var sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.Category}");

                foreach (var book in category.LatestThreeBooks)
                {
                    sb.AppendLine($"{book}");
                }
            }

            return sb.ToString();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var booksToUpdate = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToList();

            booksToUpdate.ForEach(b => b.Price += 5M);

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var booksToRemove = context.Books
                .Where(b => b.Copies < 4200)
                .ToList();
            context.Books.RemoveRange(booksToRemove);

            int deletedBookCount = booksToRemove.Count();

            context.SaveChanges();

            return deletedBookCount;
        }
    }
}
