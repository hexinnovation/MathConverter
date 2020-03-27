using System.Collections.Generic;
#if NET35
using System.Linq;
#endif

namespace HexInnovation
{
    internal static class CompatibilityExtensions
    {
#if NET35
        public static string[] MyToArray(this IEnumerable<AbstractSyntaxTree> objects)
        {
            return objects.Cast<object>().MyToArray();
        }
        public static string[] MyToArray(this IEnumerable<object> objects)
        {
            return objects.Select(p => $"{p}").ToArray();
        }
        public static string[] MyToArray(this IEnumerable<string> strings)
        {
            return strings.ToArray();
        }
#else
        public static IEnumerable<object> MyToArray(this IEnumerable<object> objects)
        {
            return objects;
        }
        public static IEnumerable<string> MyToArray(this IEnumerable<string> strings)
        {
            return strings;
        }
#endif
    }
}
