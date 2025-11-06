using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Application.Interfaces
{
    public interface IImporter<T>
    {
        public Task<IEnumerable<T>> ImportFromFileAsync(params string[] filePaths);
    }
}
