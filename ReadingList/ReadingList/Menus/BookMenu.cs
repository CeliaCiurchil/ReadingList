using ReadingList.Application.Services;
using ReadingList.Domain.Models;
using ReadingList.Infrastructure.Repositories;
namespace ReadingList.Cli.Menus;

internal class BookMenu
{
    public void Show()
    {
        Console.WriteLine("Book Menu - Under Construction");
    }
    public static void PrintOptions()
    {
        Console.WriteLine("""
            1. Import books from CSV
            2. View all books
            
            8. Exit
        """);
        Console.Write("Enter an option: ");
    }
    public static void MenuLoop()
    {
        InMemoryRepository<Book,int> repository = new InMemoryRepository<Book,int>(b=>b.Id);
        CSVImporter importer = new CSVImporter();

        BookService bookService = new BookService(importer,repository);//DI

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
                        {
                            ImportBooks(bookService);
                            break;
                        }
                    case 2:
                        {
                            ViewAllBooks(bookService);
                            break;
                        }
                    case 3:
                        {
                            ViewFinishedBooks(bookService);
                            break;
                        }
                    case 4:
                        {
                            
                            break;
                        }
                    case 5:
                        {
                            
                            break;
                        }
                    case 6:
                        {
                            
                            break;
                        }
                    case 7:
                        {
                            
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid option");
                            break;
                        }
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
        bookService.GetFinishedBooks().ToList().ForEach(DisplayBook);
    }

    private static void ImportBooks(BookService bookService)
    {
        Console.Write("Enter the CSV file paths separated by a space: ");
        string input = Console.ReadLine() ?? "";
        string[] filePaths = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var books = bookService.ImportBooksAsync(filePaths).GetAwaiter().GetResult();

    }
    private static void ViewAllBooks(BookService bookService)
    {
        bookService.GetAllBooks().ToList().ForEach(b =>
        {
            DisplayBook(b);
        });
    }
    private static void DisplayBook(Book book)
    {
        Console.WriteLine($"ID: {book.Id}\n Title: {book.Title}\n Author: {book.Author}\n Year Published: {book.YearPublished}\n Pages: {book.Pages}\n Genre: {book.Genre}\n Finished: {book.Finished}\n Rating: {book.Rating}\n");
    }
}
