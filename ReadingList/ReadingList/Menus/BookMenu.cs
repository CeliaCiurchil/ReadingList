using ReadingList.Application.Services;
using ReadingList.Domain;
using ReadingList.Domain.Models;
using ReadingList.Domain.Enums;
using ReadingList.Infrastructure.Repositories;
using ReadingList.Application.Interfaces;
using ReadingList.Infrastructure.Exporters;
namespace ReadingList.Cli.Menus;

public static class BookMenu
{
    public static void BooksByAuthor(BookService bookService)
    {
        Console.Write("Enter author name: ");
        Console.WriteLine("Author: ");
        string author = Console.ReadLine() ?? "";

        var res = bookService.BooksByAuthor(author);
        if (!res.IsSuccess)
        {
            Console.WriteLine($"Error: {res.Error}");
            return;
        }

        res.Value!.ToList().ForEach(DisplayBook);
    }

    public static void TopRatedN(BookService bookService)
    {
        Console.Write("N: ");
        if (!int.TryParse(Console.ReadLine(), out int n) || n <= 0)
        { 
            Console.WriteLine("Invalid number.");
            return; 
        }

        var res = bookService.TopN(n);
        if (!res.IsSuccess) { Console.WriteLine($"Error: {res.Error}"); return; }

        res.Value!.ToList().ForEach(DisplayBook);
    }

    public static void ViewFinishedBooks(BookService bookService)
    {
        Console.WriteLine("Finished Books:");

        var res = bookService.GetFinishedBooks();
        if (!res.IsSuccess)
        {
            Console.WriteLine($"Error: {res.Error}");
            return;
        }

        res.Value!.ToList().ForEach(DisplayBook);
    }

    public static async Task ImportBooksAsync(BookService bookService)
    {
        Console.Write("Enter the CSV file paths separated by a space: ");
        string input = Console.ReadLine() ?? "";
        string[] fileNames = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string dataFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Data"));
        string[] filePaths = fileNames.Select(name => Path.Combine(dataFolder, name)).ToArray();

        var importRes = await bookService.ImportBooksAsync(filePaths);
        if (!importRes.IsSuccess)
        {
            Console.WriteLine($"Import failed: {importRes.Error}");
            return;
        }
        Console.WriteLine($"Imported {importRes.Value!.Count()} books.");
    }

    public static void ViewAllBooks(BookService bookService)
    {
        var res = bookService.GetAllBooks();
        if (!res.IsSuccess)
        {
            Console.WriteLine($"Error: {res.Error}");
            return;
        }

        res.Value!.ToList().ForEach(DisplayBook);
    }

    public static void Statistics(BookService bookService)
    {
        var allRes = bookService.GetAllBooks();
        if (!allRes.IsSuccess) { Console.WriteLine($"Error: {allRes.Error}"); return; }
        var books = allRes.Value!.ToList();

        int total = books.Count;
        var finishedCountRes = bookService.FinishedBooksCount();
        var averageRatingRes = bookService.AverageRating();

        Console.WriteLine("== Stats ==");
        Console.WriteLine($"Total books: {total}");
        Console.WriteLine($"Finished books: {(finishedCountRes.IsSuccess ? finishedCountRes.Value : 0)}");
        Console.WriteLine($"Average rating: {(averageRatingRes.IsSuccess ? averageRatingRes.Value : 0.0):F2}");

        Console.WriteLine("Pages by genre:");
        var pagesByGenre = books
            .GroupBy(b => b.Genre ?? string.Empty)
            .OrderBy(g => g.Key)
            .Select(g => new { Genre = g.Key, Pages = g.Sum(b => (int)b.Pages) });

        foreach (var g in pagesByGenre)
            Console.WriteLine($"  {g.Genre}: {g.Pages}");

        Console.WriteLine("Top 3 authors by book count:");
        var topAuthorsRes = bookService.Top3AuthorsByBookCount();
        if (!topAuthorsRes.IsSuccess || !topAuthorsRes.Value!.Any())
            Console.WriteLine("  (none)");
        else
            topAuthorsRes.Value!.ToList().ForEach(a => Console.WriteLine($"  {a}"));
    }

    public static void MarkFinished(BookService bookService)
    {
        Console.Write("Book ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var res = bookService.MarkFinished(id);
        Console.WriteLine(res.IsSuccess ? "200 OK" : $"404 Not Found / Error: {res.Error}");
    }

    public static void RateBook(BookService bookService)
    {
        Console.Write("Book ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Rating (0-5): ");
        if (!double.TryParse(Console.ReadLine(), out double rating))
        { Console.WriteLine("Invalid rating."); return; }

        var res = bookService.Rate(id, rating);
        Console.WriteLine(res.IsSuccess ? "200 OK" : $"Error: {res.Error}");
    }

    private static void DisplayBook(Book book)
    {
        Console.WriteLine($"ID: {book.Id}\n Title: {book.Title}\n Author: {book.Author}\n Year Published: {book.YearPublished}\n Pages: {book.Pages}\n Genre: {book.Genre}\n Finished: {book.Finished}\n Rating: {book.Rating}\n");
    }

    internal static void ExportBooks(BookService bookService)
    {
        Console.Write("Enter the export file path: ");
        var path = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine("Invalid path.");
            return;
        }

        Console.Write("Enter the export type (json/csv): ");
        var input = Console.ReadLine() ?? "";

        if (!Enum.TryParse<ExportStrategy>(input, true, out var strategy))
        {
            Console.WriteLine("Invalid export strategy. Use 'json' or 'csv'.");
            return; 
        }

        if (File.Exists(path))
        {
            Console.Write($"File '{path}' exists. Overwrite? (y/n): ");
            var answer = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            if (answer is not ("y" or "yes"))
            {
                Console.WriteLine("Canceled.");
                return;
            }
        }

        IExportStrategy exportStrategy = strategy == ExportStrategy.Json
            ? new JsonExportStrategy()
            : new CsvExportStrategy();

        var result = bookService.ExportAsync(exportStrategy, path).GetAwaiter().GetResult();

        Console.WriteLine(result.IsSuccess
            ? $"Exported successfully to {path}"
            : $"Export failed: {result.Error}");
    }

}
