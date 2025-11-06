using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReadingList.Application.Interfaces;
using ReadingList.Domain.Models;
namespace ReadingList.Application.Services;

public class BookService
{
    IImporter<Book> Importer;
    IRepository<Book> Repository;
    public BookService(IImporter<Book> importer, IRepository<Book> repository)
    {
        Importer = importer;
        Repository = repository;
    }
    public async Task<IEnumerable<Book>> ImportBooksAsync(params string[] filePaths)
    {
        var books = await Importer.ImportFromFileAsync(filePaths);
        foreach (var book in books)
        {
            Repository.Add(book);
        }
        return books;
    }
    public IEnumerable<Book> GetAllBooks()
    {
        return Repository.GetAll();
    }
    public IEnumerable<Book> GetFinishedBooks()
    {
        return Repository.GetAll().Where(b => b.Finished);
    }
}
