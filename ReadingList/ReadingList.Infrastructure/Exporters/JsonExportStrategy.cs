using ReadingList.Application.Interfaces;
using ReadingList.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReadingList.Infrastructure.Exporters
{
    public class JsonExportStrategy : IExportStrategy
    {
        public async Task<Result> ExportAsync<T>(IEnumerable<T> items, string path, CancellationToken ct = default)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
                await JsonSerializer.SerializeAsync(fs, items, options, ct).ConfigureAwait(false);
                await fs.FlushAsync(ct).ConfigureAwait(false);
                return Result.Success();
            }
            catch (OperationCanceledException)
            {
                return Result.Failure("Export canceled.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"JSON export failed: {ex.Message}");
            }
        }
    }
}
