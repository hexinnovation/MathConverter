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
#if WINDOWS_UWP || NETSTANDARD1_0 || NETSTANDARD1_3
                self.GetTypeInfo().GetCustomAttributes()
#else
                Attribute.GetCustomAttributes(self)
#endif
                    .OfType<TAttribute>();
        }

        public static bool IsIConvertible(object self)
        {
#if NETSTANDARD1_0
            // I can't figure out how to see if an object is an IConvertible before .NET Standard 1.3, so we'll just let an
            // InvalidCastException occur if it's not an IConvertible. We'll swallow that exception and return an unconverted
            // value. This totally sucks, so it'd be nice if we could not do that, but I don't really know a workaround.
            return true;
#else
            return self is IConvertible;
#endif
        }

        public static IEnumerable<MethodInfo> GetPublicStaticMethods(this Type self)
        {
#if NETSTANDARD1_0 || NETSTANDARD1_3
            return self.GetRuntimeMethods().Where(method => method.IsPublic && method.IsStatic);
#else
            return self.GetMethods(BindingFlags.Public | BindingFlags.Static);
#endif
        }

#if NETSTANDARD1_0
        public static char Last(this string str)
        {
            return str.ToCharArray().Last();
        }
#endif

#if NETSTANDARD1_0 || NETSTANDARD1_3
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            Xamarin.Forms.Internals.EnumerableExtensions.ForEach(enumeration, action);
        }
        public static IEnumerable<Type> GetInterfaces(this Type self)
        {
            return self.GetTypeInfo().ImplementedInterfaces;
        }
        public static Type[] GetGenericArguments(this Type self)
        {
            return self.GenericTypeArguments;
        }
#endif

#if WINDOWS_UWP || NETSTANDARD1_0 || NETSTANDARD1_3
        public static bool IsInstanceOfType(this Type self, object o)
        {
            return Xamarin.Forms.Internals.ReflectionExtensions.IsInstanceOfType(self, o);
        }
        public static bool IsAssignableFrom(this Type self, Type o)
        {
            return Xamarin.Forms.Internals.ReflectionExtensions.IsAssignableFrom(self, o);
        }
#else
        public static Type GetTypeInfo(this Type self)
        {
            return self;
        }
#endif
    }
}
