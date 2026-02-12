using System.Collections.Generic;
using System.Linq;

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
#else
        public static string Concat<T>(IEnumerable<T> objects)
        {
            return string.Concat(objects);
        }
#endif
    }
}
