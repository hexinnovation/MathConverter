using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace HexInnovation
{
    public class MathConverter : IValueConverter, IMultiValueConverter
    {
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
            double d;

            if (parameter == null && values[0] is string && !double.TryParse(values[0] as string, out d))
            {
                return Convert(values.Skip(1).ToArray(), targetType, values[0], culture);
            }

            // We handle string conversion entirely differently.
            if (targetType == typeof(string))
            {
                if (parameter is string)
                {
                    // Get the parameter
                    var param = parameter as string;

                    var ex = new Exception("Parameter must be in one of the following formats:\r\n\"format with special characters '\"' ; , {0}\";x\r\n - or - \r\nformat without the semicolon or comma (which can still contain quote (\") characters) {0};x");

                    string format, args;

                    if (param[0] == '"')
                    {
                        var endquote = param.LastIndexOf('\"');
                        if (endquote == 0 || (param[endquote + 1] != ';' && param[endquote + 1] != ','))
                        {
                            throw ex;
                        }

                        format = param.Substring(1, endquote - 1);
                        args = param.Substring(endquote + 2);
                    }
                    else if (param.Contains(',') || param.Contains(';'))
                    {
                        var end = param.IndexOfAny(new char[] { ',', ';' });
                        format = param.Substring(0, end);
                        args = param.Substring(end + 1);
                    }
                    else
                    {
                        // They don't have any arguments
                        return string.Format(param);
                    }

                    var argVals = ParseParameter(args);
                    return string.Format(format, argVals.Select(p => (object)p.Evaluate(values)).ToArray());
                }
                else if (parameter == null)
                {
                    throw new Exception("If you are converting to a string, you must either use a ConverterParameter or have the first binding be a string.");
                }
                else
                {
                    throw new NotSupportedException("The parameter must be a string.");
                }
            }

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
                    default:
                        throw new NotSupportedException(string.Format("You cannot convert to a {0}", targetType.Name));
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
                                throw new NotSupportedException(string.Format("The parameter specifies {0} values; Object supports only one", values.Length));
                        }
                    case "System.Double":
                        switch (values.Length)
                        {
                            case 1:
                                return ConvertToDouble(values[0]);
                            default:
                                throw new NotSupportedException(string.Format("The parameter specifies {0} values; Double supports only one", values.Length));
                        }
                    case "System.Windows.CornerRadius":
                        switch (values.Length)
                        {
                            case 1:
                                return new CornerRadius(ConvertToDouble(values[0]).Value);
                            case 4:
                                return new CornerRadius(ConvertToDouble(values[0]).Value, ConvertToDouble(values[1]).Value, ConvertToDouble(values[2]).Value, ConvertToDouble(values[3]).Value);
                            default:
                                throw new NotSupportedException(string.Format("You supplied {0} values; CornerRadius supports only one or four", values.Length));
                        }
                    case "System.Windows.GridLength":
                        switch (values.Length)
                        {
                            case 1:
                                return new GridLength(ConvertToDouble(values[0]).Value);
                            default:
                                throw new NotSupportedException(string.Format("You supplied {0} values; GridLength supports only one", values.Length));
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
                                throw new NotSupportedException(string.Format("You supplied {0} values; Thickness supports only one, two, or four", values.Length));
                        }
                    case "System.Windows.Rect":
                        switch (values.Length)
                        {
                            case 4:
                                return new Rect(ConvertToDouble(values[0]).Value, ConvertToDouble(values[1]).Value, ConvertToDouble(values[2]).Value, ConvertToDouble(values[3]).Value);
                            default:
                                throw new NotSupportedException(string.Format("You supplied {0} values; Rect supports only four", values.Length));
                        }
                    case "System.Windows.Size":
                        switch (values.Length)
                        {
                            case 2:
                                return new Size(ConvertToDouble(values[0]).Value, ConvertToDouble(values[1]).Value);
                            default:
                                throw new NotSupportedException(string.Format("You supplied {0} values; Size supports only two", values.Length));
                        }
                    case "System.Windows.Point":
                        switch (values.Length)
                        {
                            case 2:
                                return new Point(ConvertToDouble(values[0]).Value, ConvertToDouble(values[1]).Value);
                            default:
                                throw new NotSupportedException(string.Format("You supplied {0} values; Point supports only two", values.Length));
                        }
                    case "System.Boolean":
                        switch (values.Length)
                        {
                            case 1:
                                return (bool?)values[0];
                            default:
                                throw new NotSupportedException(string.Format("You supplied {0} values; Boolean supports only one", values.Length));
                        }
                    default:
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
        private AbstractSyntaxTree[] ParseParameter(string Parameter)
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
        /// Converts a number to a double.
        /// </summary>
        /// <param name="parameter">The value we're converting to a double.</param>
        /// <returns>The number, converted to a double.</returns>
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

                    throw new NotSupportedException(paramType + " cannot be converted to singleton.");
            }
        }
        public static double? ConvertToDouble(object parameter)
        {
            if (parameter is double)
                return (double)parameter;

            if (parameter == null)
                return null;

            double v;
            if (parameter is string && double.TryParse(parameter as string, out v))
            {
                return v;
            }

            throw new Exception("Could not convert the value to a double. The value was: \r\n" + parameter);
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
