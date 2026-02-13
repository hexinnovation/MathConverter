using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HexInnovation;

public abstract class AbstractSyntaxTree
{
    public object Evaluate(CultureInfo cultureInfo, object[] bindingValues)
    {
        try
        {
            return DoEvaluate(cultureInfo, bindingValues);
        }
        catch (Exception ex) when (ex is not NodeEvaluationException)
        {
            throw new NodeEvaluationException(this, ex);
        }
    }
    public abstract object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues);
    public abstract override string ToString();
}
internal abstract class BinaryNode(BinaryOperator @operator, AbstractSyntaxTree left, AbstractSyntaxTree right) : AbstractSyntaxTree
{
    public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues) => @operator.Evaluate(left, right, cultureInfo, bindingValues);
    public sealed override string ToString() => $"({left} {@operator} {right})";
}
internal sealed class ExponentNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.Exponentiation, left, right) { }
internal sealed class AddNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.Addition, left, right) { }
internal sealed class SubtractNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.Subtraction, left, right) { }
internal sealed class MultiplyNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.Multiply, left, right) { }
internal sealed class ModuloNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.Remainder, left, right) { }
internal sealed class AndNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.And, left, right) { }
internal sealed class NullCoalescingNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.NullCoalescing, left, right) { }
internal sealed class OrNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.Or, left, right) { }
internal sealed class DivideNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.Division, left, right) { }
internal sealed class NotEqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.Inequality, left, right) { }
internal sealed class EqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.Equality, left, right) { }
internal sealed class LessThanNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.LessThan, left, right) { }
internal sealed class LessThanEqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.LessThanOrEqual, left, right) { }
internal sealed class GreaterThanNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.GreaterThan, left, right) { }
internal sealed class GreaterThanEqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right) : BinaryNode(Operator.GreaterThanOrEqual, left, right) { }
internal sealed class TernaryNode(AbstractSyntaxTree condition, AbstractSyntaxTree positive, AbstractSyntaxTree negative) : AbstractSyntaxTree
{
    public override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues) => TernaryOperator.Evaluate(condition, positive, negative, cultureInfo, bindingValues);
    public override string ToString() => $"({condition} ? {positive} : {negative})";
}
internal abstract class UnaryNode(UnaryOperator @operator, AbstractSyntaxTree node) : AbstractSyntaxTree
{
    public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues) => @operator.Evaluate(node.Evaluate(cultureInfo, bindingValues));
    public sealed override string ToString() => $"{@operator}({node})";
}
internal sealed class NotNode(AbstractSyntaxTree node) : UnaryNode(Operator.LogicalNot, node) { }
internal sealed class NegativeNode(AbstractSyntaxTree node) : UnaryNode(Operator.UnaryNegation, node) { }
internal class ValueNode(object value) : AbstractSyntaxTree
{
    protected object Value { get; } = value;
    public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues) => Value;
    public override string ToString() => $"{Value}";
}
internal sealed class NullNode() : ValueNode(null)
{
    public override string ToString() => "null";
}
internal sealed class StringNode(string value) : ValueNode(value)
{
    public override string ToString() => $"\"{Value}\"";
}
internal sealed class VariableNode(int index) : AbstractSyntaxTree
{
    public override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues) =>
        bindingValues.Length <= index ?
#if NET5_0_OR_GREATER
#pragma warning disable CA2201 // Do not raise reserved exception types
#endif
            throw new IndexOutOfRangeException($"Error accessing binding value {this}. {bindingValues.Length switch { 0 => "No values were", 1 => "Only one value was", { } n => $"Only {n} values were" }} specified.") :
#if NET5_0_OR_GREATER
#pragma warning restore CA2201 // Do not raise reserved exception types
#endif
            bindingValues[index];
    public override string ToString() =>
        index switch
        {
            0 => "x",
            1 => "y",
            2 => "z",
            _ => $"[{index}]"
        };
}

/// <summary>
/// A custom function used by MathConverter.
/// Register the function with the <see cref="MathConverter" /> to use it.
/// </summary>
/// <seealso cref="ZeroArgFunction"/>
/// <seealso cref="OneArgFunction"/>
/// <seealso cref="OneDoubleFunction"/>
/// <seealso cref="TwoArgFunction"/>
/// <seealso cref="ArbitraryArgFunction"/>
public abstract class CustomFunction : AbstractSyntaxTree
{
    /// <summary>
    /// The name of the function.
    /// There could potentially be multiple names for same function.
    /// </summary>
    public string FunctionName { get; internal set; }
    /// <summary>
    /// Converts an object to a specified type. Returns true if the conversion was successful; otherwise false.
    /// </summary>
    /// <typeparam name="T">The type to convert the specified value to.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="convertedValue">The value, casted to the specified type, or the default value, if the conversion was unsuccessful.</param>
    /// <returns>True if the conversion was successful; otherwise false.</returns>
    protected static bool TryConvert<T>(object value, out T convertedValue)
    {
        var convertToType = typeof(T);

        if (Operator.DoesImplicitConversionExist(value?.GetType(), convertToType, true) && Operator.DoImplicitConversion(value, convertToType.IsValueType ? typeof(Nullable<>).MakeGenericType(convertToType) : convertToType) is T a)
        {
            convertedValue = a;
            return true;
        }
        else
        {
            convertedValue = default;
            return false;
        }
    }
    /// <summary>
    /// Similar to TryConvert&lt;string>, except that if the value is not a string but can be converted to a non-empty string using ToString(), then that string will be returned as the converted value.
    /// </summary>
    /// <param name="value">The value to try to convert to string</param>
    /// <param name="convertedValue">The converted string, or null if <c><paramref name="value"/>?.ToString()</c> is null or empty</param>
    /// <returns>True if <paramref name="convertedValue"/> is not null; Otherwise false.</returns>
    protected static bool ConvertToString(object value, out string convertedValue) =>
        (convertedValue = value switch
        {
            string s => s,
            { } when TryConvert<string>(value, out var s) => s,
            { } when $"{value}" is { Length: > 0 } toString => toString,
            _ => null
        }) is not null;

    /// <summary>
    /// The actual parameters passed to this function.
    /// </summary>
    internal List<AbstractSyntaxTree> Parameters { get; set; }
    /// <summary>
    /// Gets the number of parameters passed to the function.
    /// </summary>
    protected int NumParameters => Parameters.Count;
    /// <summary>
    /// Evaluates a specific parameter passed into the function.
    /// </summary>
    /// <param name="whichParameter">The zero-based index of the argument to evaluate.</param>
    /// <param name="cultureInfo">The CultureInfo to use when evaluating the parameter.</param>
    /// <param name="bindingValues">The values being converted by MathConverter.</param>
    /// <returns></returns>
    protected object EvaluateParameter(int whichParameter, CultureInfo cultureInfo, object[] bindingValues) => Parameters[whichParameter].Evaluate(cultureInfo, bindingValues);

    /// <summary>
    /// A method that can be overridden in base classes that specifies if a ParsingException should be thrown while parsing the parameters to this function.
    /// </summary>
    /// <param name="numParams">The number of parameters parsed.</param>
    /// <returns>True if the number of parameters is valid; otherwise false.</returns>
    public virtual bool IsValidNumberOfParameters(int numParams) => true;
    public sealed override string ToString() => $"{FunctionName}({string.Join(", ", Parameters)})";
}

/// <summary>
/// A function that takes no arguments and returns an object.
/// </summary>
public abstract class ZeroArgFunction : CustomFunction
{
    public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues) => Evaluate(cultureInfo);
    /// <summary>
    /// The actual function.
    /// </summary>
    /// <param name="cultureInfo">The culture to evaluate with.</param>
    public abstract object Evaluate(CultureInfo cultureInfo);

    /// <inheritdoc />
    public sealed override bool IsValidNumberOfParameters(int numParams) => numParams == 0;
}


/// <summary>
/// A function that takes a single parameter of type object that returns an object.
/// </summary>
public abstract class OneArgFunction : CustomFunction
{
    public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues) => Evaluate(cultureInfo, Parameters[0].Evaluate(cultureInfo, bindingValues));
    /// <summary>
    /// The actual function.
    /// </summary>
    /// <param name="cultureInfo">The culture to evaluate with.</param>
    /// <param name="argument">The argument passed to the function.</param>
    public abstract object Evaluate(CultureInfo cultureInfo, object argument);
    /// <inheritdoc />
    public sealed override bool IsValidNumberOfParameters(int numParams) => numParams == 1;
}
/// <summary>
/// A function that takes a single parameter of type double that returns a double.
/// </summary>
public abstract class OneDoubleFunction : OneArgFunction
{
    /// <inheritdoc />
    public sealed override object Evaluate(CultureInfo cultureInfo, object argument) => argument is null ? EvaluateNullArgument(cultureInfo) : TryConvert<double>(argument, out var x) ? Evaluate(cultureInfo, x) : throw new ArgumentException($"{FunctionName} accepts only a numeric input or null.");
    /// <summary>
    /// The actual function.
    /// </summary>
    /// <param name="cultureInfo">The culture to evaluate with.</param>
    /// <param name="argument">The argument passed to the function.</param>
    public abstract double? Evaluate(CultureInfo cultureInfo, double argument);
    /// <summary>
    /// What the function should return when the argument is null: defaults to null.
    /// </summary>
    /// <param name="cultureInfo">The culture to evaluate with.</param>
    public virtual double? EvaluateNullArgument(CultureInfo cultureInfo) => null;
}

/// <summary>
/// A function that takes two parameters.
/// </summary>
public abstract class TwoArgFunction : CustomFunction
{
    public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues)
    {
        return Evaluate(cultureInfo, Parameters[0].Evaluate(cultureInfo, bindingValues), Parameters[1].Evaluate(cultureInfo, bindingValues));
    }
    /// <summary>
    /// The actual function.
    /// </summary>
    /// <param name="cultureInfo">The culture to evaluate with.</param>
    /// <param name="x">The first argument passed to the function.</param>
    /// <param name="y">The second argument passed to the function.</param>
    public abstract object Evaluate(CultureInfo cultureInfo, object x, object y);
    /// <inheritdoc/>
    public sealed override bool IsValidNumberOfParameters(int numParams) => numParams == 2;
}
/// <summary>
/// A formula that takes anywhere from zero to infinity arguments.
/// </summary>
public abstract class ArbitraryArgFunction : CustomFunction
{
    public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues)
    {
        return Evaluate(cultureInfo, [.. Enumerable.Range(0, NumParameters).Select(i => new Func<object>(() => EvaluateParameter(i, cultureInfo, bindingValues)))]);
    }
    /// <summary>
    /// The actual function.
    /// </summary>
    /// <param name="cultureInfo">The culture to evaluate with.</param>
    /// <param name="getArgument">A function that can be used to get arbitrary arguments passed to the function.</param>
    public abstract object Evaluate(CultureInfo cultureInfo, Func<object>[] getArgument);
}
