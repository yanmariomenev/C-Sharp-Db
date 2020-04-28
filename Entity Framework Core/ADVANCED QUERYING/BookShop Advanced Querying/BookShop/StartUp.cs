using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using BookShop.Models;
using BookShop.Models.Enums;

namespace BookShop
{
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //DbInitializer.ResetDatabase(db);
                //var result = GetBooksByAgeRestriction(db, "teEN");
                //var result = GetGoldenBooks(db);
                //var result = GetBooksByPrice(db);
                //var result = GetBooksNotReleasedIn(db, 2000);
                //var result = GetBooksByCategory(db, input);
                //var input = Console.ReadLine();
                //var result = GetBooksReleasedBefore(db, input);
                //var result = GetBookTitlesContaining(db, input);
                //IncreasePrices(db);
                RemoveBooks(db);
                //Console.WriteLine(removedBooksCount);

            }


        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var sb = new StringBuilder();

            var ageRestrict = Enum.Parse<AgeRestriction>(command, true);
            var books = context.Books.Where(a => a.AgeRestriction == ageRestrict)
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(x => x.BookId)
                .Select(b => b.Title)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.Price > 40.00m)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price
                })
                .OrderByDescending(x => x.Price)
                .ToArray();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var sb = new StringBuilder();

            var books = context.Books
                .Where(r => r.ReleaseDate.HasValue && r.ReleaseDate.Value.Year != year)
                .Select(b => b.Title)
                .ToArray();
            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var sb = new StringBuilder();

            var categories = input.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var books = context.Books
                .Where(b => b.BookCategories.Any(c => categories.Contains(c.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToList();
            foreach (var book in books)
            {
                sb.AppendLine(book);
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var sb = new StringBuilder();

            var stringToDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var releaseBefore = context.Books
                .Where(b => b.ReleaseDate.Value < stringToDate)
                .OrderByDescending(x => x.ReleaseDate.Value)
                .Select(x => new
                {
                    Title = x.Title,
                    EditionType = x.EditionType,
                    Price = x.Price,
                })
                .ToArray();

            foreach (var book in releaseBefore)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var sb = new StringBuilder();

            var author = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName
                })
                .OrderBy(a => a.FullName);

            foreach (var a in author)
            {
                sb.AppendLine(a.FullName);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(x => new
                {
                    Title = x.Title,
                    AuthorFullName = x.Author.FirstName + " " + x.Author.LastName
                })
                .ToArray();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.AuthorFullName})");
            }

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var bookLength = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Select(b => b.Title)
                .Count();
            return bookLength;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var sb = new StringBuilder();

            var bookCopies = context.Authors
                .Select(a => new
                {
                    AuthorFullName = a.FirstName + " " + a.LastName,
                    Count = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(x => x.Count)
                .ToArray();

            foreach (var bookCopy in bookCopies)
            {
                sb.AppendLine($"{bookCopy.AuthorFullName} - {bookCopy.Count}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var sb = new StringBuilder();

            var profits = context.Categories
                .Select(x => new
                {
                    Category = x.Name,
                    Profit = x.CategoryBooks.Sum(c => c.Book.Price * c.Book.Copies)
                })
                .OrderByDescending(x => x.Profit)
                .ThenBy(x => x.Category)
                .ToArray();

            foreach (var profit in profits)
            {
                sb.AppendLine($"{profit.Category} ${profit.Profit}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var sb = new StringBuilder();

            var recentBooks = context.Categories
                .Select(b => new
                {
                    CategoryName = b.Name,
                    Books = b.CategoryBooks.Select( e => new
                    {
                        e.Book.Title,
                        e.Book.ReleaseDate
                    })
                        .OrderByDescending(e => e.ReleaseDate)
                        .Take(3)
                        .ToArray()
                        
                })
                .OrderBy(a => a.CategoryName)
                .ToArray();

            foreach (var rb in recentBooks)
            {
                sb.AppendLine($"--{rb.CategoryName}");

                foreach (var r in rb.Books)
                {
                    sb.AppendLine($"{r.Title} ({r.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            //var books = context.Books
            //    .Where(x => x.ReleaseDate.Value.Year < 2010)
            //    .Update(x => new Book()
            //    {
            //        Price = x.Price + 5
            //    });

            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .ToList();
            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(c => c.Copies < 4200)
                .ToArray();
            context.Books.RemoveRange(books);
            int affectedRows = context.SaveChanges();
            //return affectedRows;
            return books.Length;
        }
    }
}
