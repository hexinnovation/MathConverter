using System;
using System.Collections;
using System.Globalization;
using System.Linq;

#if WPF
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using BindableProperty = System.Windows.DependencyProperty;
#else
using Microsoft.Maui.Controls;
#endif

namespace HexInnovation;

internal sealed class NowFunction : ZeroArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo) => DateTime.Now;
}
internal sealed class UnsetValueFunction : ZeroArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo) => BindableProperty.UnsetValue;
}
internal sealed class DoNothingFunction : ZeroArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo) => Binding.DoNothing;
}
internal sealed class CosFunction : OneDoubleFunction
{
    public override double? Evaluate(CultureInfo cultureInfo, double x) => Math.Cos(x);
}
internal sealed class SinFunction : OneDoubleFunction
{
    public override double? Evaluate(CultureInfo cultureInfo, double x) => Math.Sin(x);
}
internal sealed class TanFunction : OneDoubleFunction
{
    public override double? Evaluate(CultureInfo cultureInfo, double x) => Math.Tan(x);
}
internal sealed class AbsFunction : OneDoubleFunction
{
    public override double? Evaluate(CultureInfo cultureInfo, double x) => Math.Abs(x);
}
internal sealed class AcosFunction : OneDoubleFunction
{
    public override double? Evaluate(CultureInfo cultureInfo, double x) => Math.Acos(x);
}
internal sealed class AsinFunction : OneDoubleFunction
{
    public override double? Evaluate(CultureInfo cultureInfo, double x) => Math.Asin(x);
}
internal sealed class AtanFunction : OneDoubleFunction
{
    public override double? Evaluate(CultureInfo cultureInfo, double x) => Math.Atan(x);
}
internal sealed class CeilingFunction : OneDoubleFunction
{
    public override double? Evaluate(CultureInfo cultureInfo, double x) => Math.Ceiling(x);
}
internal sealed class FloorFunction : OneDoubleFunction
{
    public override double? Evaluate(CultureInfo cultureInfo, double x) => Math.Floor(x);
}
internal sealed class SqrtFunction : OneDoubleFunction
{
    public override double? Evaluate(CultureInfo cultureInfo, double x) => Math.Sqrt(x);
}
internal sealed class DegreesFunction : OneDoubleFunction
{
    public override double? Evaluate(CultureInfo cultureInfo, double x) => x / Math.PI * 180;
}
internal sealed class RadiansFunction : OneDoubleFunction
{
    public override double? Evaluate(CultureInfo cultureInfo, double x) => x / 180 * Math.PI;
}
internal sealed class ToLowerFunction : OneArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, object? argument) => $"{argument}".ToLower(cultureInfo);
}
internal sealed class ToUpperFunction : OneArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, object? argument) => $"{argument}".ToUpper(cultureInfo);
}
#if WPF
internal sealed class VisibleOrCollapsedFunction : OneArgFunction
{
    public VisibleOrCollapsedFunction() => Debug.WriteLine($"{nameof(VisibleOrCollapsedFunction)} is deprecated. Use 'x ? `Visible` : `Collapsed` instead.'");
    public override object? Evaluate(CultureInfo cultureInfo, object? argument) => TryConvert<bool>(argument, out var value) && value ? Visibility.Visible : Visibility.Collapsed;
}
internal sealed class VisibleOrHiddenFunction : OneArgFunction
{
    public VisibleOrHiddenFunction() => Debug.WriteLine($"{nameof(VisibleOrCollapsedFunction)} is deprecated. Use 'x ? `Visible` : `Hidden` instead.'");

    public override object? Evaluate(CultureInfo cultureInfo, object? argument) => TryConvert<bool>(argument, out var value) && value ? Visibility.Visible : Visibility.Hidden;
}
#endif
internal sealed class TryParseDoubleFunction : OneArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, object? argument) =>
        TryConvert<double>(argument, out var @double) ?
        @double :
        TryConvert<string>(argument, out var @string) && double.TryParse(@string, NumberStyles.Number, cultureInfo, out @double) ? @double : null;
}
internal sealed class GetTypeFunction : OneArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, object? argument) => argument?.GetType();
}
internal sealed class StartsWithFunction : TwoArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, object? x, object? y) => TryConvert<string>(x, out var a) && ConvertToString(y, out var b) ? a.StartsWith(b, false, cultureInfo) : default(bool?);
}
internal sealed class EndsWithFunction : TwoArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, object? x, object? y) => TryConvert<string>(x, out var a) && ConvertToString(y, out var b) ? a.EndsWith(b, false, cultureInfo) : default(bool?);
}
internal sealed class Atan2Function : TwoArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, object? x, object? y) => TryConvert<double>(x, out var a) && TryConvert<double>(y, out var b) ? Math.Atan2(a, b) : null;
}
internal sealed class LogFunction : TwoArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, object? x, object? y) => TryConvert<double>(x, out var a) && TryConvert<double>(y, out var b) ? Math.Log(a, b) : null;
}
internal sealed class ContainsFunction : TwoArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, object? x, object? y) =>
        x switch
        {
            string str1 when ConvertToString(y, out var str2) => str1.Contains(str2),
            IEnumerable @enum => @enum.OfType<object?>().Contains(y),
            _ => false
        };
}
internal sealed class ConvertTypeFunction : TwoArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, object? x, object? y) => y is Type type ? MathConverter.ConvertType(x, type) : x;
}
internal sealed class EnumEqualsFunction : TwoArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, object? x, object? y)
    {
        if (x is null || y is null)
            return x is null && y is null;

        var xType = x.GetType();
        var yType = y.GetType();

        var xIsEnum = xType.IsEnum;
        var yIsEnum = yType.IsEnum;

        // Enums cannot be inherited, so two enums will only be equal if they are the same type.
        // Technically, this is different from the default behavior. By default, enums of disparate types
        // will be equal as long as their integer value is the same (typically this means it's declared in the same order).
        if (xIsEnum && yIsEnum)
            return xType.Equals(yType) && TryConvert<bool>(Operator.Equality.Evaluate(x, y), out var result) && result;

        if (!xIsEnum && !yIsEnum)
            return false;

        var @enum = xIsEnum ? x : y;
        var other = xIsEnum ? y : x;
        var enumType = xIsEnum ? xType : yType;

        try
        {
            return MathConverter.ConvertType(other, enumType) is { } converted
                && converted.GetType() == enumType
                && TryConvert<bool>(Operator.Equality.Evaluate(@enum, converted), out var result2)
                && result2;
        }
        catch (FormatException) // We failed to convert the non-enum to the enum type.
        {
            return false;
        }
    }
}
internal sealed class IsNullFunction : ArbitraryArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, Func<object?>[] arguments)
    {
        for (int i = 0; i < arguments.Length - 1; i++)
        {
            if (arguments[i]() is { } v)
                return v;
        }

        return arguments[^1]();
    }
    public override bool IsValidNumberOfParameters(int numParams) => numParams >= 2;
}
internal sealed class RoundFunction : ArbitraryArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, Func<object?>[] arguments) =>
        arguments.Length
        switch
        {
            1 when TryConvert<double>(arguments[0](), out var value) => Math.Round(value),
            2 when TryConvert<double>(arguments[0](), out var a) && TryConvert<double>(arguments[1](), out var b) => b == (int)b ? Math.Round(a, (int)b) : throw new ArgumentException($"The second argument for {FunctionName} (if specified) must be an integer."),
            _ => null, // Other numbers of arguments will be thrown by the parser, since IsValidNumberOfParameters will return false.
        };
    public override bool IsValidNumberOfParameters(int numParams) => numParams is 1 or 2;
}

internal abstract class AndOrFunction(BinaryOperator @operator, bool exitEarlyIf, object? defaultValue) : ArbitraryArgFunction
{
    public sealed override object? Evaluate(CultureInfo cultureInfo, Func<object?>[] arguments)
    {
        var currentValueIsDefined = false;
        object? currentValue = null;

        foreach (var arg in arguments.Select(x => x()))
        {
            if (currentValueIsDefined)
            {
                currentValue = @operator.Evaluate(currentValue, arg);
            }
            else
            {
                currentValue = arg;
                currentValueIsDefined = true;
            }

            if (TryConvert<bool>(currentValue, out var v) && v == exitEarlyIf)
            {
                return currentValue;
            }
        }

        return defaultValue ?? currentValue;
    }
    public sealed override bool IsValidNumberOfParameters(int numParams) => numParams > 0;
}

internal sealed class AndFunction() : AndOrFunction(Operator.And, false, null)
{
}
internal sealed class OrFunction() : AndOrFunction(Operator.Or, true, false)
{
}
internal sealed class NorFunction : ArbitraryArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, Func<object?>[] arguments) => Operator.LogicalNot.Evaluate(new OrFunction().Evaluate(cultureInfo, arguments));

    public override bool IsValidNumberOfParameters(int numParams) => numParams > 0;
}
internal abstract class CompareFunction(BinaryOperator @operator) : ArbitraryArgFunction
{
    public sealed override object? Evaluate(CultureInfo cultureInfo, Func<object?>[] arguments)
    {
        var currentValueIsDefined = false;
        object? most = null;

        foreach (var arg in arguments.Select(x => x()))
        {
            if (currentValueIsDefined)
            {
                if (TryConvert<bool>(@operator.Evaluate(arg, most), out var v) && v)
                {
                    most = arg;
                }
            }
            else
            {
                most = arg;
                currentValueIsDefined = arg != null;
            }
        }

        return most;
    }
}
internal sealed class MaxFunction() : CompareFunction(Operator.GreaterThan)
{
}
internal sealed class MinFunction() : CompareFunction(Operator.LessThan)
{
}

internal sealed class FormatFunction : ArbitraryArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, Func<object?>[] arguments) => arguments.Length > 0 && arguments[0]() is string format ? string.Format<object?>(cultureInfo, format, arguments.Skip(1).Select(x => x())) : throw new ArgumentException($"The {FunctionName} function must be called with a string as the first argument.");
    public override bool IsValidNumberOfParameters(int numParams) => numParams > 0;
}
internal sealed class ConcatFunction : ArbitraryArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, Func<object?>[] arguments) =>
        string.Concat
#if NET35
        <object?>
#endif
        (arguments switch
        {
            [{ } f] when f() is IEnumerable enumerable => enumerable.Cast<object?>(),
            [..] x => x.Select(x => x()),
            _ => []
        });
}
internal sealed class JoinFunction : ArbitraryArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, Func<object?>[] arguments) =>
        arguments[0]() is string separator ?

            string.Join(separator,
                arguments.Skip(1).Select(x => x()).ToArray()
                switch
                {
                    [IEnumerable @enum] => @enum.Cast<object?>(),
                    [..] x => x,
                    null => []
                }) :

            throw new ArgumentException($"{FunctionName}() function must be called with a string as the first argument.");
    public override bool IsValidNumberOfParameters(int numParams) => numParams > 0;
}
internal sealed class AverageFunction : ArbitraryArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, Func<object?>[] arguments) =>
        arguments.Select(x => TryConvert<double>(x(), out var d) ? d : default(double?)).Where(x => x.HasValue).Select(x => x!.Value).ToList() switch
        {
            [{ }, ..] x => x.Average(),
            _ => null
        };
}
internal sealed class ThrowFunction : ArbitraryArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, Func<object?>[] arguments) =>
        throw new InvalidOperationException($"The {FunctionName} function was called with {arguments.Length} argument{(arguments.Length == 1 ? "" : "s")}: {string.Join(", ", arguments.Select(x => x()))}");
}
internal sealed class TryCatchFunction : ArbitraryArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, Func<object?>[] getArgument)
    {
        for (int i = 0; i < getArgument.Length - 1; i++)
        {
            try
            {
                return getArgument[i]();
            }
            catch { }
        }

        // Do not catch any exception thrown by the last argument.
        return getArgument[^1]();
    }
    public override bool IsValidNumberOfParameters(int numParams) => numParams >= 2;
}
