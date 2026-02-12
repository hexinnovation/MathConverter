using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this Type self) where TAttribute : Attribute
        {
            return
                Attribute.GetCustomAttributes(self)
                    .OfType<TAttribute>();
        }

        public static bool IsIConvertible(object self)
        {
            return self is IConvertible;
        }

        public static IEnumerable<MethodInfo> GetPublicStaticMethods(this Type self)
        {
            return self.GetMethods(BindingFlags.Public | BindingFlags.Static);
        }

        public static Type GetTypeInfo(this Type self)
        {
            return self;
        }
    }
}
