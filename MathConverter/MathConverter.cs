using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

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
                CachedResults.Clear();
            }
        }

        private Dictionary<string, AbstractSyntaxTree[]> CachedResults = new Dictionary<string, AbstractSyntaxTree[]>();

        /// <summary>
        /// The conversion for a single value.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(new object[] { value }, targetType, parameter, culture);
        }
        /// <summary>
        /// The actual convert method, for zero or more parameters.
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string)
            {
                // Get the parameter
                var param = parameter is string ? parameter as string : parameter.ToString();
                
                // Get the syntax tree from the expression.  We will cache the results to save time for future parsing of the same expression
                // by the same MathConverter.
                var x = ParseParameter(param);

                switch (targetType.FullName)
                {
                    case "System.Object":
                        switch (x.Length)
                        {
                            case 1:
                                return x[0].Evaluate(values);
                            default:
                                throw new NotSupportedException(string.Format("The parameter specifies {0} values; Double supports only one", x.Length));
                        }
                    case "System.Double":
                        switch (x.Length)
                        {
                            case 1:
                                return MathConverter.ConvertToDouble(x[0].Evaluate(values));
                            default:
                                throw new NotSupportedException(string.Format("The parameter specifies {0} values; Double supports only one", x.Length));
                        }
                    case "System.Windows.CornerRadius":
                        switch (x.Length)
                        {
                            case 1:
                                return new CornerRadius(MathConverter.ConvertToDouble(x[0].Evaluate(values)).Value);
                            case 4:
                                return new CornerRadius(MathConverter.ConvertToDouble(x[0].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[1].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[2].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[3].Evaluate(values)).Value);
                            default:
                                throw new NotSupportedException(string.Format("The parameter specifies {0} values; CornerRadius supports only one or four", x.Length));
                        }
                    case "System.Windows.GridLength":
                        switch (x.Length)
                        {
                            case 1:
                                return new GridLength(MathConverter.ConvertToDouble(x[0].Evaluate(values)).Value);
                            default:
                                throw new NotSupportedException(string.Format("The parameter specifies {0} values; GridLength supports only one", x.Length));
                        }
                    case "System.Windows.Thickness":
                        switch (x.Length)
                        {
                            case 1:
                                return new Thickness(MathConverter.ConvertToDouble(x[0].Evaluate(values)).Value);
                            case 2:
                                return new Thickness(MathConverter.ConvertToDouble(x[0].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[1].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[0].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[1].Evaluate(values)).Value);
                            case 4:
                                return new Thickness(MathConverter.ConvertToDouble(x[0].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[1].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[2].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[3].Evaluate(values)).Value);
                            default:
                                throw new NotSupportedException(string.Format("The parameter specifies {0} values; Thickness supports only one, two, or four", x.Length));
                        }
                    case "System.Windows.Rect":
                        switch (x.Length)
                        {
                            case 4:
                                return new Rect(MathConverter.ConvertToDouble(x[0].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[1].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[2].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[3].Evaluate(values)).Value);
                            default:
                                throw new NotSupportedException(string.Format("The parameter specifies {0} values; Rect supports only four", x.Length));
                        }
                    case "System.Windows.Size":
                        switch (x.Length)
                        {
                            case 2:
                                return new Size(MathConverter.ConvertToDouble(x[0].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[1].Evaluate(values)).Value);
                            default:
                                throw new NotSupportedException(string.Format("You supplied {0} values; Size supports only two", x.Length));
                        }

                    case "System.Windows.Point":
                        switch (x.Length)
                        {
                            case 2:
                                return new Point(MathConverter.ConvertToDouble(x[0].Evaluate(values)).Value, MathConverter.ConvertToDouble(x[1].Evaluate(values)).Value);
                            default:
                                throw new NotSupportedException(string.Format("You supplied {0} values; Point supports only two", x.Length));
                        }
                    case "System.Boolean":
                        switch (x.Length)
                        {
                            case 1:
                                return (bool?)x[0].Evaluate(values);
                            default:
                                throw new NotSupportedException(string.Format("You supplied {0} values; Boolean supports only one", x.Length));
                        }
                    case "System.String":
                        switch (x.Length)
                        {
                            case 1:
                                var val = x[0].Evaluate(values);
                                if (val is string)
                                    return val;
                                else if (val == null)
                                    return null;
                                else
                                    return val.ToString();
                            default:
                                throw new NotSupportedException(string.Format("You supplied {0} values; string supports only one", x.Length));
                        }
                    default:
                        if (targetType == typeof(double?))
                        {
                            return MathConverter.ConvertToDouble(x[0].Evaluate(values));
                        }

                        // We don't know what to return, so let's just evaluate the parameter and return what it returns.
                        return x[0].Evaluate(values); //throw new NotSupportedException(string.Format("You cannot convert to a {0}", targetType.Name));
                }
            }
            else if (parameter == null)
            {
                switch (targetType.FullName)
                {
                    case "System.Object":
                        switch (values.Length)
                        {
                            case 1:
                                return ConvertToObject(values[0]);
                            default:
                                throw new NotSupportedException($"You supplied {values.Length} values; Object supports only one or one");
                        }
                    case "System.Double":
                        switch (values.Length)
                        {
                            case 1:
                                return ConvertToDouble(values[0]);
                            default:
                                throw new NotSupportedException($"You supplied {values.Length} values; Double supports only one or one");
                        }
                    case "System.Windows.CornerRadius":
                        switch (values.Length)
                        {
                            case 1:
                                return new CornerRadius(ConvertToDouble(values[0]).Value);
                            case 4:
                                return new CornerRadius(ConvertToDouble(values[0]).Value, ConvertToDouble(values[1]).Value, ConvertToDouble(values[2]).Value, ConvertToDouble(values[3]).Value);
                            default:
                                throw new NotSupportedException($"You supplied {values.Length} values; GridLength supports only one or four");
                        }
                    case "System.Windows.GridLength":
                        switch (values.Length)
                        {
                            case 1:
                                return new GridLength(ConvertToDouble(values[0]).Value);
                            default:
                                throw new NotSupportedException($"You supplied {values.Length} values; GridLength supports only one");
                        }
                    case "System.Windows.Thickness":
                        switch (values.Length)
                        {
                            case 1:
                                return new Thickness(ConvertToDouble(values[0]).Value);
                            case 2:
                                return new Thickness(ConvertToDouble(values[0]).Value, ConvertToDouble(values[1]).Value, ConvertToDouble(values[0]).Value, ConvertToDouble(values[1]).Value);
                            case 4:
                                return new Thickness(ConvertToDouble(values[0]).Value, ConvertToDouble(values[1]).Value, ConvertToDouble(values[2]).Value, ConvertToDouble(values[3]).Value);
                            default:
                                throw new NotSupportedException($"You supplied {values.Length} values; Thickness supports only one, two, or four");
                        }
                    case "System.Windows.Rect":
                        switch (values.Length)
                        {
                            case 4:
                                return new Rect(ConvertToDouble(values[0]).Value, ConvertToDouble(values[1]).Value, ConvertToDouble(values[2]).Value, ConvertToDouble(values[3]).Value);
                            default:
                                throw new NotSupportedException($"You supplied {values.Length} values; Rect supports only four");
                        }
                    case "System.Windows.Size":
                        switch (values.Length)
                        {
                            case 2:
                                return new Size(ConvertToDouble(values[0]).Value, ConvertToDouble(values[1]).Value);
                            default:
                                throw new NotSupportedException($"You supplied {values.Length} values; Size supports only two");
                        }
                    case "System.Windows.Point":
                        switch (values.Length)
                        {
                            case 2:
                                return new Point(ConvertToDouble(values[0]).Value, ConvertToDouble(values[1]).Value);
                            default:
                                throw new NotSupportedException($"You supplied {values.Length} values; Point supports only two");
                        }
                    case "System.Boolean":
                        switch (values.Length)
                        {
                            case 1:
                                if (values[0] is string)
                                {
                                    return bool.Parse(values[0] as string);
                                }
                                return (bool?)values[0];
                            default:
                                throw new NotSupportedException($"You supplied {values.Length} values; boolean supports only one");
                        }
                    case "System.String":
                        switch (values.Length)
                        {
                            case 1:
                                if (values[0] is string)
                                    return values[0];
                                else if (values[0] == null)
                                    return null;
                                else
                                    return values[0].ToString();
                            default:
                                throw new NotSupportedException($"You supplied {values.Length} values; string supports only one");
                        }
                    case "System.Uri":
                        switch (values.Length)
                        {
                            case 1:
                                if (values[0] is string)
                                    return new Uri(values[0] as string);
                                else if (values[0] == null)
                                    return null;
                                else
                                    return new Uri(values[0].ToString());
                            default:
                                throw new NotSupportedException($"You supplied {values.Length} values; Uri supports only one");
                        }
                    default:
                        if (targetType == typeof(double?))
                        {
                            return ConvertToDouble(values[0]);
                        }

                        throw new NotSupportedException(string.Format("You cannot convert to a {0}", targetType.Name));
                }
            }
            else
            {
                throw new ArgumentException("The Converter Parameter must be a string.", "parameter");
            }
        }
        /// <summary>
        /// Parses an expression into a syntax tree that can be evaluated later.
        /// This method keeps a cache of parsed results, so it doesn't have to parse the same expression twice.
        /// </summary>
        /// <param name="Parameter">The parameter that we're parsing</param>
        /// <returns>A syntax tree that can be evaluated later.</returns>
        internal AbstractSyntaxTree[] ParseParameter(string Parameter)
        {
            if (CachedResults == null)
                return Parser.Parse(Parameter);

            return CachedResults.ContainsKey(Parameter) ? CachedResults[Parameter] : (CachedResults[Parameter] = Parser.Parse(Parameter));
        }

        /// <summary>
        /// True to use a cache, false to parse every expression every time.
        /// </summary>
        [DefaultValue(true)]
        public bool UseCache
        {
            get
            {
                return CachedResults != null;
            }
            set
            {
                if (value && CachedResults == null)
                    CachedResults = new Dictionary<string, AbstractSyntaxTree[]>();
                else if (!value)
                    CachedResults = null;
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
                    //throw new NotSupportedException(paramType + " cannot be converted to singleton.");
            }
        }
        public static double? ConvertToDouble(object parameter)
        {
            if (parameter is double)
                return (double)parameter;

            if (parameter == null)
                return null;

            double v;
            if (parameter is string && double.TryParse(parameter as string, NumberStyles.Number, CultureInfo.InvariantCulture, out v))
            {
                return v;
            }

            throw new Exception("Could not convert the value to a double. The value was: " + parameter);
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
