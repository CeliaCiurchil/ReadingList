using ReadingList.Application.Interfaces;
using ReadingList.Domain;
using ReadingList.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Infrastructure.Exporters
{
    public class CsvExportStrategy : IExportStrategy
    {
        public async Task<Result> ExportAsync<T>(IEnumerable<T> items, string path, CancellationToken ct = default)
        {
            try
            {
                if (items is not IEnumerable<Book> books)
                    return Result.Failure("CSV exporter expects Book items.");

                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                await using var sw = new StreamWriter(path, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

                await sw.WriteLineAsync("Id,Title,Author,Year,Pages,Genre,Finished,Rating").ConfigureAwait(false);

                foreach (var b in books)
                {
                    ct.ThrowIfCancellationRequested();
                    var line = string.Join(",",
                        b.Id.ToString(),
                        Escape(b.Title),
                        Escape(b.Author),
                        b.YearPublished.ToString(),
                        b.Pages.ToString(),
                        Escape(b.Genre),
                        b.Finished ? "yes" : "no",
                        b.Rating.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    );
                    await sw.WriteLineAsync(line).ConfigureAwait(false);
                }

                await sw.FlushAsync().ConfigureAwait(false);
                return Result.Success();
            }
            catch (OperationCanceledException)
            {
                return Result.Failure("Export canceled.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"CSV export failed: {ex.Message}");
            }
        }

        private static string Escape(string? s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var needsQuotes = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
            var escaped = s.Replace("\"", "\"\"");
            return needsQuotes ? $"\"{escaped}\"" : escaped;
        }
    }
}
