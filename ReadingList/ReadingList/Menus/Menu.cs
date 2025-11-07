using ReadingList.Application.Services;
using ReadingList.Domain.Models;
using ReadingList.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Cli.Menus;

internal class Menu
{
    public static void PrintOptions()
    {
        Console.WriteLine("""
            1. Import books from CSV
            2. List all books
            3. Filter finished books
            4. Top rated N
            5. By author <text> (case-insensitive contains)
            6. Stats
            7. Mark finished <id>
            8. Rate <id> <0-5>
            9. Exit
        """);
        Console.Write("Enter an option: ");
    }

    public static void MenuLoop()
    {
        var repository = new InMemoryRepository<Book, int>(b => b.Id);
        var importer = new CSVImporter();
        var bookService = new BookService(importer, repository); // DI

        int option = 0;
        while (option != 9)
        {
            try
            {
                PrintOptions();

                if (!int.TryParse(Console.ReadLine(), out option))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                if (option == 9)
                {
                    Console.WriteLine("You chose exiting the program!");
                    break;
                }

                switch (option)
                {
                    case 1: BookMenu.ImportBooks(bookService); break;
                    case 2: BookMenu.ViewAllBooks(bookService); break;
                    case 3: BookMenu.ViewFinishedBooks(bookService); break;
                    case 4: BookMenu.TopRatedN(bookService); break;
                    case 5: BookMenu.BooksByAuthor(bookService); break;
                    case 6: BookMenu.Statistics(bookService); break;
                    case 7: BookMenu.MarkFinished(bookService); break;
                    case 8: BookMenu.RateBook(bookService); break;
                    default: Console.WriteLine("Invalid option"); break;
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
}
