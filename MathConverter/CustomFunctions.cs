using System;
using System.Collections;
using System.Globalization;
using System.Linq;

#if !XAMARIN
using System.Windows;
#endif

namespace HexInnovation
{
    sealed class NowFunction : ZeroArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo) => DateTime.Now;
    }
    sealed class CosFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Cos(x);
        }
    }
    sealed class SinFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Sin(x);
        }
    }
    sealed class TanFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Tan(x);
        }
    }
    sealed class AbsFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Abs(x);
        }
    }
    sealed class AcosFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Acos(x);
        }
    }
    sealed class AsinFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Asin(x);
        }
    }
    sealed class AtanFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Atan(x);
        }
    }
    sealed class CeilingFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Ceiling(x);
        }
    }
    sealed class FloorFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Floor(x);
        }
    }
    sealed class SqrtFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Sqrt(x);
        }
    }
    sealed class DegreesFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return x / Math.PI * 180;
        }
    }
    sealed class RadiansFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return x / 180 * Math.PI;
        }
    }
    sealed class ToLowerFunction : OneArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object parameter)
        {
            return $"{parameter}".ToLower();
        }
    }
    sealed class ToUpperFunction : OneArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object parameter)
        {
            return $"{parameter}".ToUpper();
        }
    }
#if !XAMARIN
    sealed class VisibleOrCollapsedFunction : OneArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object parameter)
        {
            return TryConvertStruct<bool>(parameter, out var value) && value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
    sealed class VisibleOrHiddenFunction : OneArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object parameter)
        {
            return TryConvertStruct<bool>(parameter, out var value) && value ? Visibility.Visible : Visibility.Hidden;
        }
    }
#endif
    sealed class TryParseDoubleFunction : OneArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object parameter)
        {
            if (TryConvertStruct<double>(parameter, out var @double))
                return @double;
            else if (TryConvert<string>(parameter, out var @string) && double.TryParse(@string, NumberStyles.Number, cultureInfo, out @double))
                return @double;
            else
                return null;
        }
    }
    sealed class StartsWithFunction : TwoArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object x, object y)
        {
            if (TryConvert<string>(x, out var string1))
            {
                if (!TryConvert<string>(y, out var string2))
                {
                    string2 = $"{y}";
                    if (string2.Length == 0)
                        return new bool?();
                }

                return string1.StartsWith(string2);
            }

            return new bool?();
        }
    }
    sealed class EndsWithFunction : TwoArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object x, object y)
        {
            if (TryConvert<string>(x, out var string1))
            {
                if (!TryConvert<string>(y, out var string2))
                {
                    string2 = $"{y}";
                    if (string2.Length == 0)
                        return new bool?();
                }

                return string1.EndsWith(string2);
            }

            return new bool?();
        }
    }
    sealed class Atan2Function : TwoArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object x, object y)
        {
            if (TryConvertStruct<double>(x, out var a) && TryConvertStruct<double>(y, out var b))
                return Math.Atan2(a, b);
            else
                return null;
        }
    }
    sealed class LogFunction : TwoArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object x, object y)
        {
            if (TryConvertStruct<double>(x, out var a) && TryConvertStruct<double>(y, out var b))
                return Math.Log(a, b);
            else
                return null;
        }
    }
    sealed class ContainsFunction : TwoArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object x, object y)
        {
            if (x is string str1 && (y is string || $"{y}".Length > 0))
            {
                return str1.Contains($"{y}");
            }
            else if (x is IEnumerable @enum)
            {
                return @enum.OfType<object>().Contains(y);
            }
            else
            {
                return null;
            }
        }
    }
    sealed class IsNullFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] parameters)
        {
            if (parameters.Length == 2)
            {
                return parameters[0]() ?? parameters[1]();
            }
            else
            {
                throw new Exception($"The function {FunctionName} only accepts two arguments. It should be called like \"{FunctionName}(3.45;1)\".");
            }
        }
    }
    sealed class RoundFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] parameters)
        {
            switch (parameters.Length)
            {
                case 1:
                    if (TryConvertStruct<double>(parameters[0](), out var value))
                        return Math.Round(value);
                    else
                        return null;
                case 2:
                    if (TryConvertStruct<double>(parameters[0](), out var a) && TryConvertStruct<double>(parameters[1](), out var b))
                    {
                        if (b == (int)b)
                            return Math.Round(a, (int)b);
                        else
                            throw new Exception($"The second argument for {FunctionName} (if specified) must be an integer.");
                    }
                    else
                        return null;
                default:
                    throw new Exception($"The function {FunctionName} only accepts one or two arguments. It should be called like \"{FunctionName}(3.4)\" or \"{FunctionName}(3.45;1)\".");
            }
        }
    }
    sealed class AndFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] parameters)
        {
            var currentValueIsDefined = false;
            object currentValue = null;

            foreach (var arg in parameters.Select(x => x()))
            {
                if (currentValueIsDefined)
                {
                    currentValue = Operator.And.Evaluate(currentValue, arg);
                }
                else
                {
                    currentValue = arg;
                    currentValueIsDefined = true;
                }

                if (Operator.TryConvertToBool(currentValue) == false)
                {
                    return currentValue;
                }
            }

            return currentValue;
        }
    }
    sealed class OrFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] parameters)
        {
            var currentValueIsDefined = false;
            object currentValue = null;

            foreach (var arg in parameters.Select(x => x()))
            {
                if (currentValueIsDefined)
                {
                    currentValue = Operator.Or.Evaluate(currentValue, arg);
                }
                else
                {
                    currentValue = arg;
                    currentValueIsDefined = true;
                }

                if (Operator.TryConvertToBool(currentValue) == true)
                {
                    return currentValue;
                }
            }

            return false;
        }
    }
    sealed class NorFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] parameters)
        {
            return Operator.LogicalNot.Evaluate(new OrFunction().Evaluate(cultureInfo, parameters));
        }
    }
    sealed class MaxFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] parameters)
        {
            var currentValueIsDefined = false;
            object max = null;

            foreach (var arg in parameters.Select(x => x()))
            {
                if (currentValueIsDefined)
                {
                    if (Operator.TryConvertToBool(Operator.GreaterThan.Evaluate(arg, max)) == true)
                    {
                        max = arg;
                    }
                }
                else
                {
                    max = arg;
                    currentValueIsDefined = arg != null;
                }
            }

            return max;
        }
    }
    sealed class MinFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] parameters)
        {
            var currentValueIsDefined = false;
            object min = null;

            foreach (var arg in parameters.Select(x => x()))
            {
                if (currentValueIsDefined)
                {
                    if (Operator.TryConvertToBool(Operator.LessThan.Evaluate(arg, min)) == true)
                    {
                        min = arg;
                    }
                }
                else
                {
                    min = arg;
                    currentValueIsDefined = arg != null;
                }
            }

            return min;
        }
    }
    sealed class FormatFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] parameters)
        {
            // Make sure we don't evaluate any of the arguments twice.
            if (parameters.Length > 0 && parameters[0]() is string format)
            {
                return string.Format(cultureInfo, format, parameters.Skip(1).Select(x => x()).ToArray());
            }
            else
            {
                throw new ArgumentException($"The {FunctionName} function must be called with a string as the first argument.");
            }
        }
    }
    sealed class ConcatFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] parameters)
        {
            return string.Concat((parameters.Length == 1 && parameters[0]() is IEnumerable enumerable ? enumerable.Cast<object>() : parameters.Select(x => x())).MyToArray());
        }
    }
    sealed class JoinFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] parameters)
        {
            if (parameters[0]() is string separator)
            {
                var argVals = parameters.Skip(1).Select(x => x()).ToArray();

                return string.Join(separator, (argVals.Length == 1 && argVals[0] is IEnumerable enumerable ? enumerable.Cast<object>() : argVals).MyToArray());
            }
            else
            {
                throw new ArgumentException($"{FunctionName}() function must be called with a string as the first argument.");
            }
        }
    }
    sealed class AverageFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] parameters)
        {
            var arguments = parameters.Select(x => TryConvertStruct<double>(x(), out var d) ? d : new double?())
                .Where(x => x.HasValue).Select(x => x.Value).ToList();

            return arguments.Count == 0 ? new double?() : arguments.Average();
        }
    }
    sealed class ThrowFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] parameters)
        {
            throw new Exception($"The {FunctionName} function was called with {parameters.Length} argument{(parameters.Length == 1 ? "" : "s")}: {string.Join(", ", parameters.Select(x => x()).MyToArray())}");
        }
    }
}
