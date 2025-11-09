using ReadingList.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Domain.Extensions
{
    public static class BookEnumerableExtensions
    {
        public static double AverageRating(this IEnumerable<Book> books)
        {
            if (books == null || !books.Any())
                return 0.0;

            return books.Average(b => b.Rating);
        }
    }
}
