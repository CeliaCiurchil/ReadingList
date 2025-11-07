using ReadingList.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Application.Interfaces
{
    public interface IExportStrategy
    {
        Task<Result> ExportAsync<T>(IEnumerable<T> items, string path, CancellationToken ct = default);
    }
}
