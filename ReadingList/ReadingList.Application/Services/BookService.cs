using ReadingList.Application.Interfaces;
using ReadingList.Domain;
using ReadingList.Domain.Extensions;
using ReadingList.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ReadingList.Application.Services;

public class BookService
{
    private readonly IImporter<Book> Importer;
    private readonly IRepository<Book> Repository;

    public BookService(IImporter<Book> importer, IRepository<Book> repository)
    {
        Importer = importer;
        Repository = repository;
    }

    public async Task<Result<IEnumerable<Book>>> ImportBooksAsync(params string[] filePaths)
    {
        var importResult = await Importer.ImportFromFileAsync(filePaths);
        if (!importResult.IsSuccess)
            return Result<IEnumerable<Book>>.Failure(importResult.Error!);

        var imported = new List<Book>();
        foreach (var book in importResult.Value!)
        {
            var addResult = Repository.Add(book);
            if (!addResult.IsSuccess)
            {
                Console.WriteLine($"Failed to add book: {addResult.Error}");
                continue;
            }
            imported.Add(addResult.Value!);
        }
        return Result<IEnumerable<Book>>.Success(imported);
    }

    public async Task<Result> ExportAsync(IExportStrategy strategy, string path, IEnumerable<Book>? selection = null, CancellationToken ct = default)
    {
        IEnumerable<Book> items;
        if (selection is not null)
        {
            items = selection;
        }
        else
        {
            var all = Repository.GetAll();
            if (!all.IsSuccess) return Result.Failure(all.Error!);
            items = all.Value!;
        }

        return await strategy.ExportAsync(items, path, ct).ConfigureAwait(false);
    }

    public Result<IEnumerable<Book>> GetAllBooks() => Repository.GetAll();

    public Result<IEnumerable<Book>> GetFinishedBooks()
    {
        var all = Repository.GetAll();
        if (!all.IsSuccess) return Result<IEnumerable<Book>>.Failure(all.Error!);
        return Result<IEnumerable<Book>>.Success(all.Value!.Where(b => b.Finished).ToList());
    }

    public Result<IEnumerable<Book>> TopN(int n)
    {
        var all = Repository.GetAll();
        if (!all.IsSuccess) return Result<IEnumerable<Book>>.Failure(all.Error!);
        return Result<IEnumerable<Book>>.Success(all.Value!.OrderByDescending(b => b.Rating).Take(n).ToList());
    }

    public Result<IEnumerable<Book>> BooksByAuthor(string text)
    {
        var all = Repository.GetAll();
        if (!all.IsSuccess) return Result<IEnumerable<Book>>.Failure(all.Error!);

        if (string.IsNullOrWhiteSpace(text))
            return Result<IEnumerable<Book>>.Success(Enumerable.Empty<Book>());

        return Result<IEnumerable<Book>>.Success(
            all.Value!.Where(b =>
                !string.IsNullOrEmpty(b.Author) &&
                b.Author.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0).ToList()
        );
    }

    public Result<int> TotalPagesRead()
    {
        var all = Repository.GetAll();
        if (!all.IsSuccess) return Result<int>.Failure(all.Error!);
        return Result<int>.Success(all.Value!.Where(b => b.Finished).Sum(b => (int)b.Pages));
    }

    public Result<int> FinishedBooksCount()
    {
        var all = Repository.GetAll();
        if (!all.IsSuccess) return Result<int>.Failure(all.Error!);
        return Result<int>.Success(all.Value!.Count(b => b.Finished));
    }

    public Result<double> AverageRating()
    {
        var all = Repository.GetAll();
        if (!all.IsSuccess) return Result<double>.Failure(all.Error!);

        var avg = all.Value!.Where(b => b.Finished).AverageRating();
        return Result<double>.Success(avg);
    }

    public Result<int> PagesInGenre(string genre)
    {
        var all = Repository.GetAll();
        if (!all.IsSuccess) return Result<int>.Failure(all.Error!);
        return Result<int>.Success(all.Value!.Where(b => b.Genre == genre).Sum(b => (int)b.Pages));
    }

    public Result<IEnumerable<string>> Top3AuthorsByBookCount()
    {
        var all = Repository.GetAll();
        if (!all.IsSuccess) return Result<IEnumerable<string>>.Failure(all.Error!);

        var top = all.Value!
            .GroupBy(b => b.Author)
            .OrderByDescending(g => g.Count())
            .Take(3)
            .Select(g => g.Key)
            .ToList();

        return Result<IEnumerable<string>>.Success(top);
    }

    public Result MarkFinished(int id)
    {
        var got = Repository.GetById(id);
        if (!got.IsSuccess) return Result.Failure(got.Error!);

        var book = got.Value!;
        if (!book.Finished)
        {
            book.Finished = true;
            var up = Repository.Update(book);
            if (!up.IsSuccess) return Result.Failure(up.Error!);
        }
        return Result.Success();
    }

    public Result Rate(int id, double rating)
    {
        if (rating < 0 || rating > 5)
            return Result.Failure("Rating must be between 0 and 5.");

        var got = Repository.GetById(id);
        if (!got.IsSuccess) return Result.Failure(got.Error!);

        var book = got.Value!;
        book.Rating = rating;

        var up = Repository.Update(book);
        return up.IsSuccess ? Result.Success() : Result.Failure(up.Error!);
    }

    public int GetTotalBookCount()
    {
        var all = Repository.GetAll();
        if (!all.IsSuccess) return 0;
        return all.Value!.Count();
    }
}
