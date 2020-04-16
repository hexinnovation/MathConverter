using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HexInnovation
{
    abstract class AbstractSyntaxTree
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
    sealed class FormulaNode0 : AbstractSyntaxTree
    {
        public FormulaNode0(string formulaName, Func<object> formula)
        {
            _formulaName = formulaName;
            _formula = formula;
        }
        private readonly string _formulaName;
        private readonly Func<object> _formula;
        public override object DoEvaluate(CultureInfo cultureInfo, object[] parameters)
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
    sealed class FormulaNode1 : AbstractSyntaxTree
    {
        public FormulaNode1(string formulaName, Func<object, object> formula, AbstractSyntaxTree input)
        {
            _formulaName = formulaName;
            _formula = formula;
            _input = input;

            switch (formulaName.ToLower())
            {
                case "visibleorhidden":
                case "visibleorcollapsed":
                    Debug.WriteLine($"Warning: Function {formulaName} is deprecated and will be removed in a later release of MathConverter. You should instead use code like \"{input}\" ? `Visible` : `{(formulaName.ToLower().EndsWith("hidden") ? "Hidden" : "Collapsed")}`");
                    break;
            }

        }
        private readonly string _formulaName;
        private readonly Func<object, object> _formula;
        private readonly AbstractSyntaxTree _input;
        public override object DoEvaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return _formula(_input.Evaluate(cultureInfo, parameters));
        }
        public override string ToString()
        {
            return $"{_formulaName}({_input})";
        }
    }
    /// <summary>
    /// A formula that takes two arguments
    /// </summary>
    sealed class FormulaNode2 : AbstractSyntaxTree
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
        public override object DoEvaluate(CultureInfo cultureInfo, object[] parameters)
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
    /// A formula that takes anywhere from one to infinity arguments.
    /// </summary>
    sealed class FormulaNodeN : AbstractSyntaxTree
    {
        public static object And(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            var currentValueIsDefined = false;
            object currentValue = null;

            foreach (var arg in args)
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
        public static object Or(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            var currentValueIsDefined = false;
            object currentValue = null;

            foreach (var arg in args)
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
        public static object Nor(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            return Operator.LogicalNot.Evaluate(Or(cultureInfo, args));
        }
        public static object Max(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            var currentValueIsDefined = false;
            object max = null;

            foreach (var arg in args)
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
        public static object Min(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            var currentValueIsDefined = false;
            object min = null;

            foreach (var arg in args)
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
        public static object Format(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            // Make sure we don't evaluate any of the arguments twice.
            var argsList = args.ToList();

            if (argsList.Count > 0 && argsList[0] is string format)
            {
                return string.Format(cultureInfo, format, argsList.Skip(1).ToArray());
            }
            else
            {
                throw new ArgumentException("format() function must be called with a string as the first argument.");
            }
        }
        public static string Concat(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            var argsList = args.ToList();

            return string.Concat((argsList.Count == 1 && argsList[0] is IEnumerable enumerable ? enumerable.Cast<object>() : argsList).MyToArray());
        }
        public static string Join(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            var argsList = args.ToList();

            if (argsList[0] is string separator)
            {
                var argVals = argsList.Skip(1).ToArray();

                return string.Join(separator, (argVals.Length == 1 && argVals[0] is IEnumerable enumerable ? enumerable.Cast<object>() : argVals).MyToArray());

            }
            else
            {
                throw new ArgumentException("join() function must be called with a string as the first argument.");
            }
        }
        public static object Average(CultureInfo cultureInfo, IEnumerable<object> args)
        {
            var arguments = args.Select(p => (double?)Operator.DoImplicitConversion(p, typeof(double?)))
                .Where(p => p.HasValue).Select(p => p.Value).ToList();

            switch (arguments.Count)
            {
                case 0:
                    return null;
                default:
                    return arguments.Average();
            }
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
        public override object DoEvaluate(CultureInfo cultureInfo, object[] parameters)
        {
            return _formula(cultureInfo, _args.Select(p => p.Evaluate(cultureInfo, parameters)));
        }
        public override string ToString()
        {
            return $"{_formulaName}({string.Join(", ", _args.MyToArray())})";
        }
    }
}
