using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Domain.Extensions
{
    public static class StringExtensions
    {
        public static string ToTitleCaseSafe(this string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input.ToLower());
        }
    }
}
