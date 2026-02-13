using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

#if MAUI
using Microsoft.Maui.Controls;
#elif WPF
using BindableProperty = System.Windows.DependencyProperty;
using System.Windows.Data;
using System.Windows.Markup;
#endif

namespace HexInnovation
{
    /// <summary>
    /// MathConverter is a WPF Converter class that does it all.
    /// </summary>
    [ContentProperty(nameof(CustomFunctions))]
    public class MathConverter : IValueConverter, IMultiValueConverter
    {
        /// <summary>
        /// Computes the ordinal number for an integer.
        /// For example, turns 12 to "12th" or 1 to "1st"
        /// </summary>
        /// <param name="number">The number for which we want to compute the ordinal value.</param>
        /// <returns>A string that indicates to a human what position a number is in (1st, 2nd, 3rd, etc.)</returns>
        internal static string ComputeOrdinal(int number) =>
            (number % 100 is < 11 or > 13, number % 10) switch
            {
                (true, 1) => $"{number}st",
                (true, 2) => $"{number}nd",
                (true, 3) => $"{number}rd",
                _ => $"{number}th",
            };

        /// <summary>
        /// Sanitizes an argument as specified by a Binding.
        /// Converts DependencyProperty.UnsetValue with a warning to identify to a developer which Binding might not be correctly configured.
        /// </summary>
        /// <param name="arg">The argument to sanitize</param>
        /// <param name="argIndex">Which argument (starting with zero) is this binding in the <see cref="MultiBinding"/>?</param>
        /// <param name="totalBinding">How many arguments are there total? If there is only one, we're assuming this converter is being used on a <see cref="Binding"/>, not a <see cref="MultiBinding"/></param>
        /// <param name="parameter">The ConverterParameter being used for this conversion. This helps identify the (possibly faulty) binding.</param>
        /// <param name="targetType">The type we're trying to convert to. This helps identify the (possibly faulty) binding.</param>
        /// <returns>The <paramref name="arg"/> passed in, or <code>null</code> if the <paramref name="arg"/> is equal to <see cref="BindableProperty.UnsetValue"/></returns>
        private object SanitizeBinding(object arg, int argIndex, int totalBinding, object parameter, Type targetType)
        {
            if (arg == BindableProperty.UnsetValue && !AllowUnsetValue)
            {
                Debug.WriteLine($"Encountered {nameof(BindableProperty.UnsetValue)} in the {(totalBinding > 1 ? $"{ComputeOrdinal(argIndex + 1)} " : "")}argument while trying to convert to type \"{targetType.FullName}\" using the ConverterParameter {(parameter == null ? "'null'" : $"\"{parameter}\"")}. Double-check that your binding is correct.");
                return null;
            }

            return arg;
        }

        /// <summary>
        /// The custom functions used by MathConverter.
        /// Allows you to extend MathConverter with custom functions.
        /// </summary>
        public CustomFunctionCollection CustomFunctions { get; }



        /// <summary>
        /// Creates a new MathConverter object.
        /// </summary>
        public MathConverter()
        {
            CustomFunctions = [];
            CustomFunctions.RegisterDefaultFunctions();
        }

        /// <summary>
        /// If <see cref="UseCache"/> is set to true, clears the cache of this MathConverter object; If <see cref="UseCache"/> is false, this method does nothing.
        /// </summary>
        public void ClearCache() => _cachedResults?.Clear();

        /// <summary>
        /// True to use a cache, false to parse every expression every time.
        /// </summary>
        [DefaultValue(true)]
        public bool UseCache
        {
            get => _cachedResults != null;
            set
            {
                if (value)
                    _cachedResults ??= [];
                else
                    _cachedResults = null;
            }
        }

        /// <summary>
        /// Defaults to <c>false</c>, which implicitly converts <see cref="BindableProperty.UnsetValue"/> to <c>null</c> with a debug warning.
        /// Set to <c>true</c> to actually allow UnsetValue to be used to convert.
        /// </summary>
        public bool AllowUnsetValue { get; set; }

        /// <summary>
        /// A dictionary which stores a cache of AbstractSyntaxTrees for given ConverterParameter strings.
        /// This eliminates the need to parse the same statement over and over.
        /// </summary>
        private Dictionary<string, AbstractSyntaxTree[]> _cachedResults = [];
#if !WPF
        private static readonly Dictionary<Type, TypeConverter> PlatformTypeConverters = [];
#endif

        /// <summary>
        /// The conversion for a single value.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Convert([value], targetType, parameter, culture);

        /// <summary>
        /// The actual convert method, for zero or more parameters.
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            object[] sanitizedValues = values?.Select((v, i) => SanitizeBinding(v, i, values.Length, parameter, targetType)).ToArray() ?? [];

            object[] evaluatedValues;

            switch (parameter)
            {
                case string param:
                    // We start by evaluating the parameter passed in. For certain types (e.g. Rect), we allow multiple values to be specified in the parameter, separated by either commas or semicolons.
                    // So the parameter "x,2x,x+y,2y" has four parts, which each parse to their own AbstractSyntaxTree: "x", "2x", "x+y", and "2y".
                    var parameterParts = ParseParameter(param);

                    // We now compute the evaluated values. In the above example, the parts would evaluate to four doubles: values[0], 2*values[0], values[0]+values[1], and 2*values[1].
                    try
                    {
                        evaluatedValues = [.. parameterParts.Select(p => p.Evaluate(culture, sanitizedValues))];
                    }
                    catch (NodeEvaluationException ex)
                    {
                        throw new EvaluationException(param, values, ex);
                    }
                    break;

                case null:
                    // If there is no parameter, we'll just use the value(s) specified by the (Multi)Binding.
                    // In this case, MathConverter is merely used for type conversion (e.g. turning 4 doubles into a Rect).
                    evaluatedValues = sanitizedValues ?? [];
                    break;
                default:
                    throw new ArgumentException("The Converter Parameter must be a string.", nameof(parameter));
            }

            // Now if there are more than one value, we will simply merge the values with commas, and use TypeConverter to handle the conversion to the appropriate type.
            // We do this in invariant culture to ensure that any type conversion (which must happen in InvariantCulture) succeeds.
            var stringJoinCulture = targetType == typeof(string) ? culture : CultureInfo.InvariantCulture;
            var finalAnswerToConvert = evaluatedValues switch { [null] => null, [{ } onlyItem] => onlyItem, _ => string.Join(",", evaluatedValues.Select(x => string.Format(stringJoinCulture, "{0}", x))) };

            return ConvertType(finalAnswerToConvert, targetType);
        }

        /// <summary>
        /// Converts a value to a given type. Returns the input value if the type conversion fails.
        /// This function tries the following things:
        /// - TypeConverters
        /// - Coersion (Implicit conversions: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/types/casting-and-type-conversions#implicit-conversions)
        /// - IConvertible (System.Convert.ChangeType)
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="targetType">The type to convert to</param>
        public static object ConvertType(object value, Type targetType)
        {
            // At this point, we have now computed our final answer.
            // However, we might need to do standard type conversion to convert it to a different type.

            // We don't need to convert null, and we can't convert if there's no type specified that we need to convert to.
            if (value == null || targetType == null || value == BindableProperty.UnsetValue || value == Binding.DoNothing)
                return value;

            // We might not need to convert.
            if (targetType.IsInstanceOfType(value))
                return value;

            // We need to convert the answer to the appropriate type. Let's start with the default TypeConverter.
            var converter = TypeDescriptor.GetConverter(targetType);

            if (converter.CanConvertFrom(value.GetType()))
                // We don't want to use the CultureInfo here when converting, because Rect conversion is broken in some cultures.
                // We'll keep these conversions working in InvariantCulture.
                return converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);

            // We know we're not returning null... If we're trying to convert to a Nullable<SomeStruct>, let's just convert to SomeStruct instead.
            if (Nullable.GetUnderlyingType(targetType) is { } newTarget)
                targetType = newTarget;

#if !WPF
            // Let's try PlatformTypeConverter
            var typeConverter = GetPlatformTypeConverter(targetType);

            if (typeConverter != null)
            {
                // TypeConverters only convert from Invariant Strings. All other conversions are deprecated.
                string convertFrom = value as string ?? $"{value}";
                return typeConverter.ConvertFromInvariantString(convertFrom);
            }
#endif

            try
            {
                if (Operator.DoesImplicitConversionExist(value.GetType(), targetType, true))
                    // The default TypeConverter doesn't support this conversion. Let's try an implicit conversion.
                    return Operator.DoImplicitConversion(value, targetType);

                if (value is IConvertible)
                {
                    if (targetType == typeof(char))
                        // We'll add a special cast for conversions to char, where we'll convert to int first.
                        return System.Convert.ToChar((int)System.Convert.ChangeType(value, typeof(int), CultureInfo.InvariantCulture));

                    // Let's try System.Convert. This might throw an exception.
                    return System.Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                }
            }
            catch (InvalidCastException) { }

            try
            {
                if (targetType.IsEnum)
                    return Enum.ToObject(targetType, value);
            }
            catch (ArgumentException) { }

            // Welp, we can't convert this value... Oh well.
            return value;
        }

        /// <summary>
        /// Don't call this method, as it is not supported.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // WE CAN'T CONVERT BACK
            throw new NotSupportedException();
        }
        /// <summary>
        /// Don't call this method, as it is not supported.
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // WE CAN'T CONVERT BACK
            throw new NotSupportedException();
        }

#if !WPF
        private static TypeConverter GetPlatformTypeConverter(Type targetType)
        {
            if (PlatformTypeConverters.TryGetValue(targetType, out var x))
                return x;

            foreach (var attribute in Attribute.GetCustomAttributes(targetType).OfType<TypeConverterAttribute>())
            {
                if (Type.GetType(attribute.ConverterTypeName, false) is { } converterType)
                    return PlatformTypeConverters[targetType] = (TypeConverter)Activator.CreateInstance(converterType);
            }

            return PlatformTypeConverters[targetType] = null;
        }
#endif


        /// <summary>
        /// Parses an expression into a syntax tree that can be evaluated later.
        /// This method keeps a cache of parsed results, so it doesn't have to parse the same expression twice.
        /// </summary>
        /// <param name="parameter">The parameter that we're parsing</param>
        /// <returns>A syntax tree that can be evaluated later.</returns>
        internal AbstractSyntaxTree[] ParseParameter(string parameter) =>
            _cachedResults?.TryGetValue(parameter, out var x) is true ? x : Parser.Parse(CustomFunctions, parameter) is { } y ? _cachedResults is null ? y : _cachedResults[parameter] = y : (_cachedResults?[parameter] = null);
    }
}
