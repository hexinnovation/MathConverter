using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HexInnovation
{
    public abstract class AbstractSyntaxTree
    {
        public object Evaluate(CultureInfo cultureInfo, object[] parameters)
        {
            try
            {
                return DoEvaluate(cultureInfo, parameters);
            }
            catch (Exception ex) when (!(ex is NodeEvaluationException))
            {
                throw new NodeEvaluationException(this, ex);
            }
        }
        public abstract object DoEvaluate(CultureInfo cultureInfo, object[] parameters);
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
        public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return _operator.Evaluate(_left, _right, cultureInfo, parameters);
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

        public override object DoEvaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return TernaryOperator.Evaluate(_condition, _positive, _negative, cultureInfo, parameters);
        }
        public override string ToString()
        {
            return $"({_condition} ? {_positive} : {_negative})";
        }
    }
    sealed class NullNode : AbstractSyntaxTree
    {
        public override object DoEvaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return null;
        }
        public override string ToString()
        {
            return "null";
        }
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
        public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return _operator.Evaluate(_node.Evaluate(cultureInfo, parameters));
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
        public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] parameters) => Value;
        public override string ToString() => $"{Value}";
    }
    /// <summary>
    /// A constant, like e or pi, or any arbitrary number specified in the ConverterParameter
    /// </summary>
    sealed class ConstantNumberNode : ValueNode
    {
        public ConstantNumberNode(double value)
            : base(value) { }
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
        public override object DoEvaluate(CultureInfo cultureInfo, object[] parameters)
        {
            if (parameters.Length <= _index)
            {
                var error = new StringBuilder("Error accessing variable ").Append(this).Append(". ");

                if (parameters.Length == 0)
                    error.Append("No");
                else
                    error.Append("Only ").Append(parameters.Length);

                throw new IndexOutOfRangeException(error.Append(" variable").Append(parameters.Length == 1 ? " was" : "s were")
                    .Append(" specified.").ToString());
            }

            return parameters[_index];
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

    public abstract class CustomFunction : AbstractSyntaxTree
    {
        public string FunctionName { get; internal set; }

        protected bool TryConvertStruct<T>(object argument, out T value)
            where T : struct
        {
            if (Operator.DoesImplicitConversionExist(argument?.GetType(), typeof(T), true) && Operator.DoImplicitConversion(argument, typeof(T?)) is T a)
            {
                value = a;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        protected bool TryConvert<T>(object argument, out T value)
            where T : class
        {
            if (Operator.DoesImplicitConversionExist(argument?.GetType(), typeof(T), true) && Operator.DoImplicitConversion(argument, typeof(T)) is T a)
            {
                value = a;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }

    public abstract class ZeroArgFunction : CustomFunction
    {
        public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return Evaluate(cultureInfo);
        }
        public abstract object Evaluate(CultureInfo cultureInfo);

        public override string ToString()
        {
            return $"{FunctionName}()";
        }
    }


    /// <summary>
    /// A formula that takes one input
    /// </summary>
    public abstract class OneArgFunction : CustomFunction
    {
        internal AbstractSyntaxTree Argument { get; set; }
        public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return Evaluate(cultureInfo, Argument.Evaluate(cultureInfo, parameters));
        }
        public abstract object Evaluate(CultureInfo cultureInfo, object parameter);
        public override string ToString()
        {
            return $"{FunctionName}({Argument})";
        }
    }
    public abstract class OneDoubleFunction : OneArgFunction
    {
        public sealed override object Evaluate(CultureInfo cultureInfo, object parameter)
        {
            if (TryConvertStruct<double>(parameter, out var x))
                return Evaluate(cultureInfo, x);
            else if (parameter == null)
                return EvaluateNullArgument(cultureInfo);
            else
                throw new ArgumentException($"{FunctionName} accepts only a numeric input or null.");
        }
        public abstract double? Evaluate(CultureInfo cultureInfo, double parameter);
        public virtual double? EvaluateNullArgument(CultureInfo cultureInfo)
        {
            return null;
        }
    }

    /// <summary>
    /// A function that takes two arguments
    /// </summary>
    public abstract class TwoArgFunction : CustomFunction
    {
        internal AbstractSyntaxTree Argument1 { get; set; }
        internal AbstractSyntaxTree Argument2 { get; set; }
        public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return Evaluate(cultureInfo, Argument1.Evaluate(cultureInfo, parameters), Argument2.Evaluate(cultureInfo, parameters));
        }
        public abstract object Evaluate(CultureInfo cultureInfo, object x, object y);
        public override string ToString()
        {
            return $"{FunctionName}({Argument1}, {Argument2})";
        }
    }
    /// <summary>
    /// A formula that takes anywhere from zero to infinity arguments.
    /// </summary>
    public abstract class ArbitraryArgFunction : CustomFunction
    {
        internal AbstractSyntaxTree[] Arguments { get; set; }
        public sealed override object DoEvaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return Evaluate(cultureInfo, Arguments.Select(x => new Func<object>(() => x.Evaluate(cultureInfo, parameters))).ToArray());
        }
        public abstract object Evaluate(CultureInfo cultureInfo, Func<object>[] parameters);
        public override string ToString()
        {
            return $"{FunctionName}({string.Join(", ", Arguments.OfType<object>().MyToArray())})";
        }
    }
}
