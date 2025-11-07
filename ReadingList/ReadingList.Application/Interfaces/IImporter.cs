using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReadingList.Domain;
namespace ReadingList.Application.Interfaces
{
    public interface IImporter<T>
    {
        Task<Result<IEnumerable<T>>> ImportFromFileAsync(params string[] filePaths);
    }
}
