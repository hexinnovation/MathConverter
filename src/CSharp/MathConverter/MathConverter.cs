using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

#if XAMARIN
using Xamarin.Forms;
using TypeConverterAttribute = Xamarin.Forms.TypeConverterAttribute;
using PlatformTypeConverter = Xamarin.Forms.TypeConverter;
#elif MAUI
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using PlatformTypeConverter = System.ComponentModel.TypeConverter;
#elif WPF
using BindableProperty = System.Windows.DependencyProperty;
using System.Windows;
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
        internal static string ComputeOrdinal(int number)
        {
            if (number % 100 < 11 || number % 100 > 13)
            {
                switch (number % 10)
                {
                    case 1:
                        return $"{number}st";
                    case 2:
                        return $"{number}nd";
                    case 3:
                        return $"{number}rd";
                }
            }
            return $"{number}th";
        }
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
            CustomFunctions = new CustomFunctionCollection();
            CustomFunctions.RegisterDefaultFunctions();
        }

        /// <summary>
        /// If <see cref="UseCache"/> is set to true, clears the cache of this MathConverter object; If <see cref="UseCache"/> is false, this method does nothing.
        /// </summary>
        public void ClearCache()
        {
            if (UseCache)
            {
                _cachedResults.Clear();
            }
        }

        /// <summary>
        /// True to use a cache, false to parse every expression every time.
        /// </summary>
        [DefaultValue(true)]
        public bool UseCache
        {
            get => _cachedResults != null;
            set
            {
                if (value && _cachedResults == null)
                    _cachedResults = new Dictionary<string, AbstractSyntaxTree[]>();
                else if (!value)
                    _cachedResults = null;
            }
        }

        /// <summary>
        /// Defaults to <c>false</c>, which implicitly converts <see cref="BindableProperty.UnsetValue"/> to <c>null</c> with a debug warning.
        /// Set to <c>true</c> to actually allow UnsetValue to be used to convert.
        /// </summary>
        public bool AllowUnsetValue { get; set; } = false;

        /// <summary>
        /// A dictionary which stores a cache of AbstractSyntaxTrees for given ConverterParameter strings.
        /// This eliminates the need to parse the same statement over and over.
        /// </summary>
        private Dictionary<string, AbstractSyntaxTree[]> _cachedResults = new Dictionary<string, AbstractSyntaxTree[]>();
#if !WPF
        private static readonly Dictionary<Type, PlatformTypeConverter> PlatformTypeConverters = new()
#if MAUI
            { { typeof(GridLength), new GridLengthTypeConverter() } }
#endif
            ;
#endif

        /// <summary>
        /// The conversion for a single value.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(new[] { value }, targetType, parameter, culture);
        }
        /// <summary>
        /// The actual convert method, for zero or more parameters.
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var sanitizedValues = values?.Select((v, i) => SanitizeBinding(v, i, values.Length, parameter, targetType)).ToArray();

            List<object> evaluatedValues;

            if (parameter is string param)
            {
                // We start by evaluating the parameter passed in. For certain types (e.g. Rect), we allow multiple values to be specified in the parameter, separated by either commas or semicolons.
                // So the parameter "x,2x,x+y,2y" has four parts, which each parse to their own AbstractSyntaxTree: "x", "2x", "x+y", and "2y".
                var parameterParts = ParseParameter(param);

                // We now compute the evaluated values. In the above example, the parts would evaluate to four doubles: values[0], 2*values[0], values[0]+values[1], and 2*values[1].
                try
                {
                    evaluatedValues = parameterParts.Select(p => p.Evaluate(culture, sanitizedValues)).ToList();
                }
                catch (NodeEvaluationException ex)
                {
                    throw new EvaluationException(param, values, ex);
                }
            }
            else if (parameter == null)
            {
                // If there is no parameter, we'll just use the value(s) specified by the (Multi)Binding.
                // In this case, MathConverter is merely used for type conversion (e.g. turning 4 doubles into a Rect).
                evaluatedValues = sanitizedValues?.ToList() ?? new List<object>();
            }
            else
            {
                throw new ArgumentException("The Converter Parameter must be a string.", nameof(parameter));
            }

            // Now if there are more than one value, we will simply merge the values with commas, and use TypeConverter to handle the conversion to the appropriate type.
            // We do this in invariant culture to ensure that any type conversion (which must happen in InvariantCulture) succeeds.
            var stringJoinCulture = targetType == typeof(string) ? culture : CultureInfo.InvariantCulture;
            var finalAnswerToConvert = evaluatedValues?.Count == 1 ? evaluatedValues[0] : string.Join(",", evaluatedValues.Select(p => string.Format(stringJoinCulture, "{0}", p)).MyToArray());

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
            if (value == null || targetType == null)
                return value;

            // We might not need to convert.
            if (targetType.IsInstanceOfType(value))
            {
                return value;
            }

            // We need to convert the answer to the appropriate type. Let's start with the default TypeConverter.
            var converter = TypeDescriptor.GetConverter(targetType);

            if (converter.CanConvertFrom(value.GetType()))
            {
                // We don't want to use the CultureInfo here when converting, because Rect conversion is broken in some cultures.
                // We'll keep these conversions working in InvariantCulture.
                return converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
            }

            // We know we're not returning null... If we're trying to convert to a Nullable<SomeStruct>, let's just convert to SomeStruct instead.
            var newTarget = Nullable.GetUnderlyingType(targetType);
            if (newTarget != null)
            {
                targetType = newTarget;
            }

#if !WPF
            // Let's try PlatformTypeConverter
            var typeConverter = GetPlatformTypeConverter(targetType);

            if (typeConverter != null)
            {
                // PlatformTypeConverters only convert from Invariant Strings. All other conversions are deprecated.
                string convertFrom = value as string ?? $"{value}";
                return typeConverter.ConvertFromInvariantString(convertFrom);
            }
#endif

            try
            {
                if (Operator.DoesImplicitConversionExist(value.GetType(), targetType, true))
                {
                    // The default TypeConverter doesn't support this conversion. Let's try an implicit conversion.
                    return Operator.DoImplicitConversion(value, targetType);
                }
                else if (CompatibilityExtensions.IsIConvertible(value))
                {
                    if (targetType == typeof(char))
                    {
                        // We'll add a special cast for conversions to char, where we'll convert to int first.
                        return System.Convert.ToChar((int)System.Convert.ChangeType(value, typeof(int)));
                    }
                    else
                    {
                        // Let's try System.Convert. This might throw an exception.
                        return System.Convert.ChangeType(value, targetType);
                    }
                }
            }
            catch (InvalidCastException) { }

            try
            {
                if (targetType.GetTypeInfo().IsEnum)
                {
                    return Enum.ToObject(targetType, value);
                }
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
        private static PlatformTypeConverter GetPlatformTypeConverter(Type targetType)
        {
            if (PlatformTypeConverters.ContainsKey(targetType))
            {
                return PlatformTypeConverters[targetType];
            }

            foreach (var attribute in targetType.GetCustomAttributes<TypeConverterAttribute>())
            {
                if (Type.GetType(attribute.ConverterTypeName, false) is { } converterType)
                {
                    return PlatformTypeConverters[targetType] = (PlatformTypeConverter)Activator.CreateInstance(converterType);
                }
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
        internal AbstractSyntaxTree[] ParseParameter(string parameter)
        {
            if (_cachedResults == null)
                return Parser.Parse(CustomFunctions, parameter);

            return _cachedResults.ContainsKey(parameter) ? _cachedResults[parameter] : (_cachedResults[parameter] = Parser.Parse(CustomFunctions, parameter));
        }
    }
}
