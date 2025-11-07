using ReadingList.Application.Services;
using ReadingList.Domain.Models;
using ReadingList.Infrastructure.Repositories;
namespace ReadingList.Cli.Menus;

internal class BookMenu
{
    public static void PrintOptions()
    {
        Console.WriteLine("""
                1. Import books from CSV
                2. View all books
                3. View finished books
                8. Exit
            """);
        Console.Write("Enter an option: ");
    }

    public static void MenuLoop()
    {
        var repository = new InMemoryRepository<Book, int>(b => b.Id);
        var importer = new CSVImporter();
        var bookService = new BookService(importer, repository); // DI

        int option = 0;
        while (option != 8)
        {
            try
            {
                PrintOptions();

                if (!int.TryParse(Console.ReadLine(), out option))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                if (option == 8)
                {
                    Console.WriteLine("You chose exiting the program!");
                    break;
                }

                switch (option)
                {
                    case 1:
                        ImportBooks(bookService);
                        break;
                    case 2:
                        ViewAllBooks(bookService);
                        break;
                    case 3:
                        ViewFinishedBooks(bookService);
                        break;
                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Press to continue");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }

    private static void ViewFinishedBooks(BookService bookService)
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

    private static void ImportBooks(BookService bookService)
    {
        Console.Write("Enter the CSV file paths separated by a space: ");
        string input = Console.ReadLine() ?? "";
        string[] fileNames = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string dataFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Data"));
        string[] filePaths = fileNames.Select(name => Path.Combine(dataFolder, name)).ToArray();

        var importRes = bookService.ImportBooksAsync(filePaths).GetAwaiter().GetResult();
        if (!importRes.IsSuccess)
        {
            Console.WriteLine($"Import failed: {importRes.Error}");
            return;
        }
        Console.WriteLine($"Imported {importRes.Value!.Count()} books.");
    }

    private static void ViewAllBooks(BookService bookService)
    {
        var res = bookService.GetAllBooks();
        if (!res.IsSuccess)
        {
            Console.WriteLine($"Error: {res.Error}");
            return;
        }
        res.Value!.ToList().ForEach(DisplayBook);
    }

    private static void DisplayBook(Book book)
    {
        Console.WriteLine($"ID: {book.Id}\n Title: {book.Title}\n Author: {book.Author}\n Year Published: {book.YearPublished}\n Pages: {book.Pages}\n Genre: {book.Genre}\n Finished: {book.Finished}\n Rating: {book.Rating}\n");
    }
}
