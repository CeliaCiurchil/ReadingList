using ReadingList.Application.Interfaces;
using ReadingList.Domain;
using ReadingList.Domain.Models;
using System.Globalization;

namespace ReadingList.Infrastructure.Repositories;

public class CSVImporter : IImporter<Book>
{
    public async Task<Result<IEnumerable<Book>>> ImportFromFileAsync(params string[] filePaths)
    {
        try
        {
            var readTasks = filePaths.Select(async filePath =>
            {
                var books = new List<Book>();
                var lines = await File.ReadAllLinesAsync(filePath).ConfigureAwait(false);

                foreach (var line in lines.Skip(1)) // skip header
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var fields = line.Split(',');

                    string title = fields[1];
                    string author = fields[2];
                    int yearPublished = int.Parse(fields[3], CultureInfo.InvariantCulture);
                    uint pages = uint.Parse(fields[4], CultureInfo.InvariantCulture);
                    string genre = fields[5];
                    double rating = double.Parse(fields[7], CultureInfo.InvariantCulture);
                    string finishedString = fields[6];
                    bool finished = finishedString is "yes" or "y" or "true";

                    var book = new Book(title, author, yearPublished, pages, genre, rating, finished);
                    books.Add(book);
                }

                return books;
            });

            var results = await Task.WhenAll(readTasks).ConfigureAwait(false);
            return Result<IEnumerable<Book>>.Success(results.SelectMany(b => b));
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Book>>.Failure($"Import failed: {ex.Message}");
        }
    }
}

