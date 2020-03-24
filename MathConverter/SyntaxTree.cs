using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HexInnovation
{
    public abstract class AbstractSyntaxTree
    {
        public abstract object Evaluate(CultureInfo cultureInfo, object[] parameters);
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
        public sealed override object Evaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return _operator.Evaluate(_left, _right, cultureInfo, parameters);
        }
        public sealed override string ToString()
        {
            return $"({_left} {_operator.OperatorSymbols} {_right})";
        }
    }
    class ExponentNode : BinaryNode
    {
        public ExponentNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Exponentiation, left, right) { }
    }
    class AddNode : BinaryNode
    {
        public AddNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Addition, left, right) { }
    }
    class SubtractNode : BinaryNode
    {
        public SubtractNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Subtraction, left, right) { }
    }
    class MultiplyNode : BinaryNode
    {
        public MultiplyNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Multiplication, left, right) { }
    }
    class ModuloNode : BinaryNode
    {
        public ModuloNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Remainder, left, right) { }
    }
    class AndNode : BinaryNode
    {
        public AndNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.And, left, right) { }
    }

    class NullCoalescingNode : BinaryNode
    {
        public NullCoalescingNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.NullCoalescing, left, right) { }
    }
    class OrNode : BinaryNode
    {
        public OrNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Or, left, right) { }
    }
    class DivideNode : BinaryNode
    {
        public DivideNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Division, left, right) { }
    }
    class NotEqualNode : BinaryNode
    {
        public NotEqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Inequality, left, right) { }
    }
    class EqualNode : BinaryNode
    {
        public EqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.Equality, left, right) { }
    }
    class LessThanNode : BinaryNode
    {
        public LessThanNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.LessThan, left, right) { }
    }
    class LessThanEqualNode : BinaryNode
    {
        public LessThanEqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.LessThanOrEqual, left, right) { }
    }
    class GreaterThanNode : BinaryNode
    {
        public GreaterThanNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.GreaterThan, left, right) { }
    }
    class GreaterThanEqualNode : BinaryNode
    {
        public GreaterThanEqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(Operator.GreaterThanOrEqual, left, right) { }
    }
    class TernaryNode : AbstractSyntaxTree
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

        public override object Evaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return TernaryOperator.Evaluate(_condition, _positive, _negative, cultureInfo, parameters);
        }
        public override string ToString()
        {
            return $"({_condition} ? {_positive} : {_negative})";
        }
    }
    class NullNode : AbstractSyntaxTree
    {
        public override object Evaluate(CultureInfo cultureInfo, object[] parameters)
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
        protected UnaryNode(AbstractSyntaxTree node)
        {
            _node = node;
        }
        private readonly AbstractSyntaxTree _node;
        public override object Evaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return Evaluate((dynamic)_node.Evaluate(cultureInfo, parameters));
        }
        protected abstract object Evaluate(dynamic value);
        public override string ToString()
        {
            return _node.ToString();
        }
    }
    class NotNode : UnaryNode
    {
        public NotNode(AbstractSyntaxTree node)
            : base(node)
        {
        }
        protected override object Evaluate(dynamic value)
        {
            return !value;
        }
        public override string ToString()
        {
            return $"!({base.ToString()})";
        }
    }
    class NegativeNode : UnaryNode
    {
        public NegativeNode(AbstractSyntaxTree node)
            : base(node)
        {
        }
        protected override object Evaluate(dynamic value)
        {
            return -value;
        }
        public override string ToString()
        {
            return $"-({base.ToString()})";
        }
    }
    class ValueNode : AbstractSyntaxTree
    {
        protected object Value { get; }
        public ValueNode(object value)
        {
            Value = value;
        }
        public override object Evaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return Value;
        }
        public override string ToString()
        {
            return $"{Value}";
        }
    }
    /// <summary>
    /// A constant, like e or pi
    /// </summary>
    class ConstantNumberNode : ValueNode
    {
        public ConstantNumberNode(double value)
            : base(value) { }
    }
    class StringNode : ValueNode
    {
        public StringNode(string value)
            : base(value) { }
        public override string ToString() => $"\"{Value}\"";
    }
    class VariableNode : AbstractSyntaxTree
    {
        public VariableNode(int index)
        {
            _index = index;
        }
        /// <summary>
        /// The index of the variable we want to get.
        /// </summary>
        private readonly int _index;
        public override object Evaluate(CultureInfo cultureInfo, object[] parameters)
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

            return MathConverter.ConvertToObject(parameters[_index]);
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
    class FormulaNode0 : AbstractSyntaxTree
    {
        public FormulaNode0(string formulaName, Func<object> formula)
        {
            _formulaName = formulaName;
            _formula = formula;
        }
        private readonly string _formulaName;
        private readonly Func<object> _formula;
        public override object Evaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return _formula();
        }
        public override string ToString()
        {
            return $"{_formulaName}()";
        }
    }
    /// <summary>
    /// A formula that takes one input
    /// </summary>
    class FormulaNode1 : AbstractSyntaxTree
    {
        public FormulaNode1(string formulaName, Func<object, object> formula, AbstractSyntaxTree input)
        {
            _formulaName = formulaName;
            _formula = formula;
            _input = input;
        }
        private readonly string _formulaName;
        private readonly Func<object, object> _formula;
        private readonly AbstractSyntaxTree _input;

        public override object Evaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return _formula(_input.Evaluate(cultureInfo, parameters));
        }
        public override string ToString()
        {
            return $"{_formulaName}({_input})";
        }
    }
    
    class FormulaNode2 : AbstractSyntaxTree
    {
        public FormulaNode2(string formulaName, Func<object, object, object> formula, AbstractSyntaxTree arg1, AbstractSyntaxTree arg2)
        {
            _formulaName = formulaName;
            _formula = formula;
            _arg1 = arg1;
            _arg2 = arg2;
        }
        private readonly string _formulaName;
        private readonly Func<object, object, object> _formula;
        private readonly AbstractSyntaxTree _arg1, _arg2;
        public override object Evaluate(CultureInfo cultureInfo, object[] parameters)
        {
            var val1 = _arg1.Evaluate(cultureInfo, parameters);
            var val2 = _arg2.Evaluate(cultureInfo, parameters);
            return _formula(val1, val2);
        }
        public override string ToString()
        {
            return $"{_formulaName}({_arg1}, {_arg2})";
        }
    }
    /// <summary>
    /// A formula that takes one to infinity arguments.
    /// </summary>
    class FormulaNodeN : AbstractSyntaxTree
    {
        public static object And(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            foreach (bool arg in args)
            {
                if (!arg)
                    return false;
            }

            return true;
        }
        public static object Or(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            foreach (bool arg in args)
            {
                if (arg)
                    return true;
            }

            return false;
        }
        public static object Nor(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            foreach (bool arg in args)
            {
                if (arg)
                    return false;
            }

            return true;
        }
        public static object Max(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            dynamic max = null;
            foreach (dynamic arg in args)
            {
                if (max == null || arg > max)
                {
                    max = arg;
                }
            }
            return max;
        }
        public static object Min(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            dynamic min = null;
            foreach (dynamic arg in args)
            {
                if (min == null || arg < min)
                {
                    min = arg;
                }
            }
            return min;
        }
        public static object Format(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            dynamic format = args.First();

            return string.Format(cultureInfo, format, args.Skip(1).ToArray());
        }
        public static string Concat(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            List<object> argVals = args.ToList();
            if (argVals.Count == 1 && argVals[0] is IEnumerable)
                return string.Concat(argVals[0] as dynamic);
            else
                return string.Concat(argVals);
        }
        public static string Join(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            var argsList = args.ToList();

            string separator = $"{argsList[0]}";

            var argVals = argsList.Skip(1).ToArray();
            if (argVals.Length == 1 && argVals[0] is IEnumerable enumerable)
                return string.Join(separator, enumerable.Cast<object>().ToArray());
            else
                return string.Join(separator, argVals);
        }
        public static object Average(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            double sum = 0.0;
            var count = 0;
            foreach (double? arg in args.Select(MathConverter.ConvertToDouble))
            {
                if (arg.HasValue)
                {
                    count++;
                    sum += arg.Value;
                }
            }
            if (count == 0)
                return null;

            return sum / count;
        }

        public FormulaNodeN(string formulaName, Func<CultureInfo, IEnumerable<object>, object> formula, IEnumerable<AbstractSyntaxTree> args)
        {
            _formulaName = formulaName;
            _formula = formula;
            _args = args;
        }
        private readonly string _formulaName;
        private readonly Func<CultureInfo, IEnumerable<object>, object> _formula;
        private readonly IEnumerable<AbstractSyntaxTree> _args;
        public override object Evaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return _formula(cultureInfo, _args.Select(p => p.Evaluate(cultureInfo, parameters)));
        }

        public override string ToString()
        {
            return $"{_formulaName}({string.Join(", ", _args)})";
        }
    }
}
