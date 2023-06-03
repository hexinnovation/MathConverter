using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HexInnovation
{
    public abstract class AbstractSyntaxTree
    {
        public object Evaluate(CultureInfo cultureInfo, object[] bindingValues)
        {
            try
            {
                return DoEvaluate(cultureInfo, bindingValues);
            }
            catch (Exception ex) when (!(ex is NodeEvaluationException))
            {
                throw new NodeEvaluationException(this, ex);
            }
        }
        public abstract object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues);
        public abstract override string ToString();
    }
    abstract class BinaryNode : AbstractSyntaxTree
    {
        protected BinaryNode(BinaryOperator @operator, AbstractSyntaxTree left, AbstractSyntaxTree right)
        {
            _operator = @operator;
            _left = left;
            _right = right;
        }
        private readonly AbstractSyntaxTree _left, _right;
        private readonly BinaryOperator _operator;
        public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues)
        {
            return _operator.Evaluate(_left, _right, cultureInfo, bindingValues);
        }
        public sealed override string ToString()
        {
            return $"({_left} {_operator} {_right})";
        }
    }
    sealed class ExponentNode : BinaryNode
    {
        public ExponentNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Exponentiation, left, right) { }
    }
    sealed class AddNode : BinaryNode
    {
        public AddNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Addition, left, right) { }
    }
    sealed class SubtractNode : BinaryNode
    {
        public SubtractNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Subtraction, left, right) { }
    }
    sealed class MultiplyNode : BinaryNode
    {
        public MultiplyNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Multiply, left, right) { }
    }
    sealed class ModuloNode : BinaryNode
    {
        public ModuloNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Remainder, left, right) { }
    }
    sealed class AndNode : BinaryNode
    {
        public AndNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.And, left, right) { }
    }
    sealed class NullCoalescingNode : BinaryNode
    {
        public NullCoalescingNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.NullCoalescing, left, right) { }
    }
    sealed class OrNode : BinaryNode
    {
        public OrNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Or, left, right) { }
    }
    sealed class DivideNode : BinaryNode
    {
        public DivideNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Division, left, right) { }
    }
    sealed class NotEqualNode : BinaryNode
    {
        public NotEqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Inequality, left, right) { }
    }
    sealed class EqualNode : BinaryNode
    {
        public EqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Equality, left, right) { }
    }
    sealed class LessThanNode : BinaryNode
    {
        public LessThanNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.LessThan, left, right) { }
    }
    sealed class LessThanEqualNode : BinaryNode
    {
        public LessThanEqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.LessThanOrEqual, left, right) { }
    }
    sealed class GreaterThanNode : BinaryNode
    {
        public GreaterThanNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.GreaterThan, left, right) { }
    }
    sealed class GreaterThanEqualNode : BinaryNode
    {
        public GreaterThanEqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.GreaterThanOrEqual, left, right) { }
    }
    sealed class TernaryNode : AbstractSyntaxTree
    {
        public TernaryNode(AbstractSyntaxTree condition, AbstractSyntaxTree positive, AbstractSyntaxTree negative)
        {
            _condition = condition;
            _positive = positive;
            _negative = negative;
        }
        private readonly AbstractSyntaxTree _condition;
        private readonly AbstractSyntaxTree _positive;
        private readonly AbstractSyntaxTree _negative;

        public override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues)
        {
            return TernaryOperator.Evaluate(_condition, _positive, _negative, cultureInfo, bindingValues);
        }
        public override string ToString()
        {
            return $"({_condition} ? {_positive} : {_negative})";
        }
    }
    sealed class NullNode : ValueNode
    {
        public NullNode() : base(null) { }
        public override string ToString() => "null";
    }
    abstract class UnaryNode : AbstractSyntaxTree
    {
        protected UnaryNode(UnaryOperator @operator, AbstractSyntaxTree node)
        {
            _operator = @operator;
            _node = node;
        }
        private readonly UnaryOperator _operator;
        private readonly AbstractSyntaxTree _node;
        public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues)
        {
            return _operator.Evaluate(_node.Evaluate(cultureInfo, bindingValues));
        }
        public sealed override string ToString() => $"{_operator}({_node})";
    }
    sealed class NotNode : UnaryNode
    {
        public NotNode(AbstractSyntaxTree node)
            : base(Operator.LogicalNot, node) { }
    }
    sealed class NegativeNode : UnaryNode
    {
        public NegativeNode(AbstractSyntaxTree node)
            : base(Operator.UnaryNegation, node) { }
    }
    class ValueNode : AbstractSyntaxTree
    {
        public ValueNode(object value)
        {
            Value = value;
        }
        protected object Value { get; }
        public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues) => Value;
        public override string ToString() => $"{Value}";
    }
    sealed class StringNode : ValueNode
    {
        public StringNode(string value)
            : base(value) { }
        public override string ToString() => $"\"{Value}\"";
    }
    sealed class VariableNode : AbstractSyntaxTree
    {
        public VariableNode(int index)
        {
            _index = index;
        }
        /// <summary>
        /// The index of the variable we want to get.
        /// </summary>
        private readonly int _index;
        public override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues)
        {
            if (bindingValues.Length <= _index)
            {
                var error = new StringBuilder("Error accessing binding value ").Append(this).Append(". ");

                if (bindingValues.Length == 0)
                    error.Append("No");
                else
                    error.Append("Only ").Append(bindingValues.Length);

                throw new IndexOutOfRangeException(error.Append(" value").Append(bindingValues.Length == 1 ? " was" : "s were")
                    .Append(" specified.").ToString());
            }

            return bindingValues[_index];
        }
        public override string ToString()
        {
            switch (_index)
            {
                case 0:
                    return "x";
                case 1:
                    return "y";
                case 2:
                    return "z";
                default:
                    return $"[{_index}]";
            }
        }
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
        protected bool TryConvert<T>(object value, out T convertedValue)
        {
            var convertToType = typeof(T);

            if (Operator.DoesImplicitConversionExist(value?.GetType(), convertToType, true) && Operator.DoImplicitConversion(value, convertToType.GetTypeInfo().IsValueType ? typeof(Nullable<>).MakeGenericType(convertToType) : convertToType) is T a)
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
        protected object EvaluateParameter(int whichParameter, CultureInfo cultureInfo, object[] bindingValues)
        {
            return Parameters[whichParameter].Evaluate(cultureInfo, bindingValues);
        }
        /// <summary>
        /// A method that can be overridden in base classes that specifies if a ParsingException should be thrown while parsing the parameters to this function.
        /// </summary>
        /// <param name="numParams">The number of parameters parsed.</param>
        /// <returns>True if the number of parameters is valid; otherwise false.</returns>
        public virtual bool IsValidNumberOfParameters(int numParams)
        {
            return true;
        }
        public override sealed string ToString()
        {
            return $"{FunctionName}({string.Join(", ", Parameters.MyToArray())})";
        }
    }

    /// <summary>
    /// A function that takes no arguments and returns an object.
    /// </summary>
    public abstract class ZeroArgFunction : CustomFunction
    {
        public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues)
        {
            return Evaluate(cultureInfo);
        }
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
        public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] bindingValues)
        {
            return Evaluate(cultureInfo, Parameters[0].Evaluate(cultureInfo, bindingValues));
        }
        /// <summary>
        /// The actual function.
        /// </summary>
        /// <param name="cultureInfo">The culture to evaluate with.</param>
        /// <param name="argument">The argument passed to the function.</param>
        public abstract object Evaluate(CultureInfo cultureInfo, object argument);
        /// <inheritdoc />
        public override bool IsValidNumberOfParameters(int numParams) => numParams == 1;
    }
    /// <summary>
    /// A function that takes a single parameter of type double that returns a double.
    /// </summary>
    public abstract class OneDoubleFunction : OneArgFunction
    {
        /// <inheritdoc />
        public sealed override object Evaluate(CultureInfo cultureInfo, object argument)
        {
            if (TryConvert<double>(argument, out var x))
                return Evaluate(cultureInfo, x);
            else if (argument == null)
                return EvaluateNullArgument(cultureInfo);
            else
                throw new ArgumentException($"{FunctionName} accepts only a numeric input or null.");
        }
        /// <summary>
        /// The actual function.
        /// </summary>
        /// <param name="cultureInfo">The culture to evaluate with.</param>
        /// <param name="argument">The argument passed to the function.</param>
        public abstract double? Evaluate(CultureInfo cultureInfo, double parameter);
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
            return Evaluate(cultureInfo, Enumerable.Range(0, NumParameters).Select(i => new Func<object>(() => EvaluateParameter(i, cultureInfo, bindingValues))).ToArray());
        }
        /// <summary>
        /// The actual function.
        /// </summary>
        /// <param name="cultureInfo">The culture to evaluate with.</param>
        /// <param name="getArgument">A function that can be used to get arbitrary arguments passed to the function.</param>
        public abstract object Evaluate(CultureInfo cultureInfo, Func<object>[] getArgument);
    }
}
