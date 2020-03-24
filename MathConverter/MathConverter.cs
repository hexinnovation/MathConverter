using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HexInnovation
{
    /// <summary>
    /// MathConverter is a WPF Converter class that does it all.
    /// </summary>
    public class MathConverter : IValueConverter, IMultiValueConverter
    {
        /// <summary>
        /// Creates a new MathConverter object.
        /// </summary>
        public MathConverter()
        {

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

        private Dictionary<string, AbstractSyntaxTree[]> _cachedResults = new Dictionary<string, AbstractSyntaxTree[]>();

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
            List<object> evaluatedValues;

            if (parameter is string param)
            {
                // We start by evaluating the parameter passed in. For certain types (e.g. Rect), we allow multiple values to be specified in the parameter, separated by either commas or semicolons.
                // So the parameter "x,2x,x+y,2y" has four parts, which each parse to their own AbstractSyntaxTree: "x", "2x", "x+y", and "2y".
                var parameterParts = ParseParameter(param);

                // We now compute the evaluated values. In the above example, the parts would evaluate to four doubles: values[0], 2*values[0], values[0]+values[1], and 2*values[1].
                evaluatedValues = parameterParts.Select(p => p.Evaluate(culture, values)).ToList();
            }
            else if (parameter == null)
            {
                // If there is no parameter, we'll just use the value(s) specified by the (Multi)Binding.
                // In this case, MathConverter is merely used for type conversion (e.g. turning 4 doubles into a Rect).
                evaluatedValues = values.ToList();
            }
            else
            {
                throw new ArgumentException("The Converter Parameter must be a string.", nameof(parameter));
            }

            // Now if there are more than one value, we will simply merge the values with commas, and use TypeConverter to handle the conversion to the appropriate type.
            // We do this in invariant culture to ensure that any type conversion (which must happen in InvariantCulture) succeeds.
            var stringJoinCulture = targetType == typeof(string) ? culture : CultureInfo.InvariantCulture;
            var finalAnswerToConvert = evaluatedValues.Count == 1 ? evaluatedValues[0] : string.Join(",", evaluatedValues.Select(p => string.Format(stringJoinCulture, "{0}", p)));

            // At this point, we have now computed our final answer.
            // However, we might need to do standard type conversion to convert it to a different type.

            // We don't need to convert null, and we can't convert if there's no type specified that we need to convert to.
            if (finalAnswerToConvert == null || targetType == null)
                return finalAnswerToConvert;

            // We might not need to convert.
            if (targetType.IsInstanceOfType(finalAnswerToConvert))
            {
                return finalAnswerToConvert;
            }

            // We need to convert. Let's try the default TypeConverter.
            var converter = TypeDescriptor.GetConverter(targetType); 

            if (converter.CanConvertFrom(finalAnswerToConvert.GetType()))
            {
                // We don't want to use the CultureInfo here when converting, because Rect conversion is broken in some cultures.
                // We'll keep this conversion working in InvariantCulture.
                return converter.ConvertFrom(null, CultureInfo.InvariantCulture, finalAnswerToConvert);
            }

            // We know we're not returning null... If we're trying to convert to a Nullable<SomeStruct>, let's just convert to SomeStruct instead.
            var newTarget = Nullable.GetUnderlyingType(targetType);
            if (newTarget != null)
            {
                targetType = newTarget;
            }

            try
            {
                if (targetType == typeof(char))
                {
                    // We'll add a special cast for conversions to char, where we'll convert to int first.
                    return System.Convert.ToChar((int)System.Convert.ChangeType(finalAnswerToConvert, typeof(int)));
                }
                else
                {
                    // The default TypeConverter doesn't support this conversion. Let's try Convert.ChangeType
                    return System.Convert.ChangeType(finalAnswerToConvert, targetType);
                }
            }
            catch (InvalidCastException)
            {
                // Welp, we can't convert this value... O well.
                return finalAnswerToConvert;
            }
        }

        /// <summary>
        /// Parses an expression into a syntax tree that can be evaluated later.
        /// This method keeps a cache of parsed results, so it doesn't have to parse the same expression twice.
        /// </summary>
        /// <param name="parameter">The parameter that we're parsing</param>
        /// <returns>A syntax tree that can be evaluated later.</returns>
        internal AbstractSyntaxTree[] ParseParameter(string parameter)
        {
            if (_cachedResults == null)
                return Parser.Parse(parameter);

            return _cachedResults.ContainsKey(parameter) ? _cachedResults[parameter] : (_cachedResults[parameter] = Parser.Parse(parameter));
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
        /// Converts a number to an object.
        /// </summary>
        /// <param name="parameter">The value we're converting to an object.</param>
        /// <returns>The number, converted to an object.</returns>
        public static object ConvertToObject(object parameter)
        {
            if (parameter == null)
                return null;
            var paramType = parameter.GetType().FullName;
            switch (paramType)
            {
                case "System.TimeSpan":
                case "System.DateTime":
                case "System.String":
                case "System.Boolean":
                    return parameter;
                case "System.Char":
                    return System.Convert.ToDouble((int)(char)parameter);
                case "System.Byte":
                    return System.Convert.ToDouble((byte)parameter);
                case "System.SByte":
                    return System.Convert.ToDouble((sbyte)parameter);
                case "System.Decimal":
                    return System.Convert.ToDouble((decimal)parameter);
                case "System.Int16":
                    return System.Convert.ToDouble((short)parameter);
                case "System.UInt16":
                    return System.Convert.ToDouble((ushort)parameter);
                case "System.Int32":
                    return System.Convert.ToDouble((int)parameter);
                case "System.UInt32":
                    return System.Convert.ToDouble((uint)parameter);
                case "System.Int64":
                    return System.Convert.ToDouble((long)parameter);
                case "System.UInt64":
                    return System.Convert.ToDouble((ulong)parameter);
                case "System.Single":
                    return System.Convert.ToDouble((float)parameter);
                case "System.Double":
                    return (double)parameter;
                case "System.Windows.GridLength":
                    return ((GridLength)parameter).Value;
                default:
                    if (parameter == DependencyProperty.UnsetValue)
                    {
                        return null;
                    }

                    return parameter;
            }
        }
        /// <summary>
        /// Converts a number to an object.
        /// </summary>
        /// <param name="parameter">The value we're converting to an object.</param>
        /// <returns>The number, converted to an object.</returns>
        public static double? ConvertToDouble(object parameter)
        {
            if (parameter is double dbl)
                return dbl;

            if (parameter == null)
                return null;

            if (parameter is string str && double.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
            {
                return v;
            }

            try
            {
                return System.Convert.ChangeType(parameter, typeof(double)) as double?;
            }
            catch (InvalidCastException ex)
            {
                throw new Exception($"Failed to convert object of type {parameter.GetType().FullName} to double", ex);
            }
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
    }
}
