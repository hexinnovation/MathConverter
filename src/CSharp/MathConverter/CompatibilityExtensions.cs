using System;
using System.Collections.Generic;

#if NET35
using System.Linq;
#endif

namespace HexInnovation;

internal static class CompatibilityExtensions
{
    extension(string)
    {
#if NET35
        public static string Join<T>(string separator, IEnumerable<T> objects)
        {
            return string.Join(separator, [.. objects.Select(x => $"{x}")]);
        }
        public static string Concat<T>(IEnumerable<T> objects)
        {
            return string.Concat([.. objects]);
        }
#endif

        public static string Format<T>(IFormatProvider provider, string format, IEnumerable<T> args)
        {
            return string.Format(provider, format, [.. args]);
        }
    }
}
