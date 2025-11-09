using ReadingList.Application.Interfaces;
using ReadingList.Domain;
using ReadingList.Domain.Extensions;
using ReadingList.Domain.Models;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ReadingList.Infrastructure.Repositories;

public class CSVImporter : IImporter<Book>
{
    public async Task<Result<IEnumerable<Book>>> ImportFromFileAsync(params string[] filePaths)
    {
        try
        {
            var readTasks = filePaths.Select(f => ProcessFileAsync(f));
            var results = await Task.WhenAll(readTasks);

            var failures = results.Where(r => !r.IsSuccess).ToArray();
            if (failures.Any())
            {
                var message = string.Join("; ", failures.Select(f => f.Error));
                return Result<IEnumerable<Book>>.Failure($"CSV malformed: {message}");
            }

            var books = results.SelectMany(r => r.Value ?? Enumerable.Empty<Book>());
            return Result<IEnumerable<Book>>.Success(books);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Book>>.Failure($"Import failed: {ex.Message}");
        }
    }

    private async Task<Result<IEnumerable<Book>>> ProcessFileAsync(string filePath)
    {
        var books = new List<Book>();
        var lines = await File.ReadAllLinesAsync(filePath);

        for (int i = 1; i < lines.Length; i++) 
        {
            var lineNumber = i + 1;
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parse = TryParseLine(line);
            if (!parse.IsSuccess)
            {
                var error = $"File '{Path.GetFileName(filePath)}' line {lineNumber}: {parse.Error}";
                return Result<IEnumerable<Book>>.Failure(error);
            }

            books.Add(parse.Value!);
        }

        return Result<IEnumerable<Book>>.Success(books);
    }

    private Result<Book> TryParseLine(string line)
    {
        var fields = line.Split(',').Select(f => f.Trim()).ToArray();
        if (fields.Length < 8)
            return Result<Book>.Failure($"expected at least 8 fields but found {fields.Length}");

        if (!int.TryParse(fields[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            return Result<Book>.Failure("invalid id");

        var title = fields[1].ToTitleCaseSafe();
        var author = fields[2].ToTitleCaseSafe();

        if (!int.TryParse(fields[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out var yearPublished))
            return Result<Book>.Failure("invalid yearPublished");

        if (!uint.TryParse(fields[4], NumberStyles.Integer, CultureInfo.InvariantCulture, out var pages))
            return Result<Book>.Failure("invalid pages");

        var genre = fields[5].ToTitleCaseSafe();

        var finishedStr = fields[6].ToLowerInvariant();
        var finished = finishedStr is "yes" or "y" or "true";

        if (!double.TryParse(fields[7], NumberStyles.Float | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var rating))
            return Result<Book>.Failure("invalid rating");

        var book = new Book(id, title, author, yearPublished, pages, genre, rating, finished);
        return Result<Book>.Success(book);
    }
}