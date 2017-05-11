using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexInnovation
{
    public abstract class AbstractSyntaxTree
    {
        public abstract object Evaluate(object[] Parameters);
        public abstract override string ToString();
    }
    abstract class BinaryNode : AbstractSyntaxTree
    {
        protected BinaryNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
        {
            this.left = left;
            this.right = right;
        }
        protected AbstractSyntaxTree left, right;
    }
    class ExponentNode : BinaryNode
    {
        public ExponentNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            var l = left.Evaluate(Parameters);
            var r = right.Evaluate(Parameters);
            if (l is double && r is double)
                return Math.Pow((double)l, (double)r);
            else
                return null;
        }
        public override string ToString()
        {
            return "(" + left + " ^ " + right + ")";
        }
    }
    class AddNode : BinaryNode
    {
        public AddNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            var l = (dynamic)left.Evaluate(Parameters);
            var r = (dynamic)right.Evaluate(Parameters);

            if (l == null && r == null)
                return null;

            return l + r;
        }
        public override string ToString()
        {
            return "(" + left + " + " + right + ")";
        }
    }
    class SubtractNode : BinaryNode
    {
        public SubtractNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            dynamic l = left.Evaluate(Parameters);
            dynamic r = right.Evaluate(Parameters);

            if (l == null || r == null)
                return null;

            return l - r;
        }
        public override string ToString()
        {
            return "(" + left + " - " + right + ")";
        }
    }
    class MultiplyNode : BinaryNode
    {
        public MultiplyNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            return (dynamic)left.Evaluate(Parameters) * (dynamic)right.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "(" + left + " * " + right + ")";
        }
    }
    class ModuloNode : BinaryNode
    {
        public ModuloNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            return (dynamic)left.Evaluate(Parameters) % (dynamic)right.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "(" + left + " % " + right + ")";
        }
    }
    class AndNode : BinaryNode
    {
        public AndNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            return (dynamic)left.Evaluate(Parameters) && (dynamic)right.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "(" + left + " && " + right + ")";
        }
    }

    class NullCoalescingNode : BinaryNode
    {
        public NullCoalescingNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            return (dynamic)left.Evaluate(Parameters) ?? (dynamic)right.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "(" + left + " ?? " + right + ")";
        }
    }
    class OrNode : BinaryNode
    {
        public OrNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            return (dynamic)left.Evaluate(Parameters) || (dynamic)right.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "(" + left + " || " + right + ")";
        }
    }
    class DivideNode : BinaryNode
    {
        public DivideNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            return (dynamic)left.Evaluate(Parameters) / (dynamic)right.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "(" + left + " / " + right + ")";
        }
    }
    class TernaryNode : AbstractSyntaxTree
    {
        public TernaryNode(AbstractSyntaxTree Condition, AbstractSyntaxTree Positive, AbstractSyntaxTree Negative)
        {
            this.Condition = Condition;
            this.Positive = Positive;
            this.Negative = Negative;
        }
        private AbstractSyntaxTree Condition;
        private AbstractSyntaxTree Positive;
        private AbstractSyntaxTree Negative;

        public override object Evaluate(object[] Parameters)
        {
            if ((dynamic)Condition.Evaluate(Parameters) == true)
            {
                return Positive.Evaluate(Parameters);
            }
            else
            {
                return Negative.Evaluate(Parameters);
            }
        }
        public override string ToString()
        {
            return Condition + " ? " + Positive + " : " + Negative;
        }
    }
    class NotEqualNode : BinaryNode
    {
        public NotEqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            return (dynamic)left.Evaluate(Parameters) != (dynamic)right.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "(" + left + " != " + right + ")";
        }
    }
    class EqualNode : BinaryNode
    {
        public EqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            return (dynamic)left.Evaluate(Parameters) == (dynamic)right.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "(" + left + " == " + right + ")";
        }
    }
    class LessThanNode : BinaryNode
    {
        public LessThanNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            return (dynamic)left.Evaluate(Parameters) < (dynamic)right.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "(" + left + " < " + right + ")";
        }
    }
    class LessThanEqualNode : BinaryNode
    {
        public LessThanEqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            return (dynamic)left.Evaluate(Parameters) <= (dynamic)right.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "(" + left + " <= " + right + ")";
        }
    }
    class GreaterThanNode : BinaryNode
    {
        public GreaterThanNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            return (dynamic)left.Evaluate(Parameters) > (dynamic)right.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "(" + left + " > " + right + ")";
        }
    }
    class GreaterThanEqualNode : BinaryNode
    {
        public GreaterThanEqualNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            return (dynamic)left.Evaluate(Parameters) >= (dynamic)right.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "(" + left + " >= " + right + ")";
        }
    }
    class NullNode : AbstractSyntaxTree
    {
        public override object Evaluate(object[] Parameters)
        {
            return null;
        }
        public override string ToString()
        {
            return "null";
        }
    }
    class BooleanNode : AbstractSyntaxTree
    {
        public BooleanNode(bool Value)
        {
            this.Value = Value;
        }
        private bool Value;
        public override object Evaluate(object[] Parameters)
        {
            return Value;
        }
        public override string ToString()
        {
            return Value.ToString().ToLower();
        }
    }
    class NotNode : AbstractSyntaxTree
    {
        public NotNode(AbstractSyntaxTree node)
        {
            this.node = node;
        }
        private AbstractSyntaxTree node;
        public override object Evaluate(object[] Parameters)
        {
            return !(dynamic)node.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "!" + node;
        }
    }
    class NegativeNode : AbstractSyntaxTree
    {
        public NegativeNode(AbstractSyntaxTree node)
        {
            this.node = node;
        }
        private AbstractSyntaxTree node;
        public override object Evaluate(object[] Parameters)
        {
            return -(dynamic)node.Evaluate(Parameters);
        }
        public override string ToString()
        {
            return "-" + node;
        }
    }
    class ValueNode : AbstractSyntaxTree
    {
        public object Value { get; set; }
        public ValueNode(object Value)
        {
            this.Value = Value;
        }
        public override object Evaluate(object[] Parameters)
        {
            return Value;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
    /// <summary>
    /// A constant, like e or pi
    /// </summary>
    class ConstantNumberNode : ValueNode
    {
        public ConstantNumberNode(double Value)
            : base(Value) { }
    }
    class StringNode : AbstractSyntaxTree
    {
        public StringNode(string Value)
        {
            this.Value = Value;
        }
        private string Value;
        public override string ToString()
        {
            return '"' + Value + '"';
        }
        public override object Evaluate(object[] Parameters)
        {
            return Value;
        }
    }
    class VariableNode : AbstractSyntaxTree
    {
        public VariableNode(int Index)
        {
            this.Index = Index;
        }
        /// <summary>
        /// The index of the variable we want to get.
        /// </summary>
        private int Index;
        public override object Evaluate(object[] Parameters)
        {
            if (Parameters.Length <= Index)
            {
                string variableName;
                switch (Index)
                {
                    case 0:
                        variableName = "x";
                        break;
                    case 1:
                        variableName = "y";
                        break;
                    case 2:
                        variableName = "z";
                        break;
                    default:
                        variableName = "[" + Index + "]";
                        break;
                }

                var error = new StringBuilder("Error accessing variable ").Append(variableName).Append(". ");

                if (Parameters.Length == 0)
                    error.Append("No");
                else
                    error.Append("Only ").Append(Parameters.Length);

                throw new IndexOutOfRangeException(error.Append(" variable").Append(Parameters.Length == 1 ? " was" : "s were")
                    .Append(" specified.").ToString());
            }

            return MathConverter.ConvertToObject(Parameters[Index]);
        }

        public override string ToString()
        {
            switch (Index)
            {
                case 0:
                    return "x";
                case 1:
                    return "y";
                case 2:
                    return "z";
                default:
                    return "[" + Index + "]";
            }
        }
    }
    class FormulaNode0 : AbstractSyntaxTree
    {
        public FormulaNode0(string FormulaName, Func<object> formula)
        {
            this.FormulaName = FormulaName;
            this.formula = formula;
        }
        private string FormulaName;
        private Func<object> formula;
        public override object Evaluate(object[] Parameters)
        {
            return formula();
        }
        public override string ToString()
        {
            return FormulaName + "()";
        }
    }
    /// <summary>
    /// A formula that takes one input
    /// </summary>
    class FormulaNode1 : AbstractSyntaxTree
    {
        public FormulaNode1(string FormulaName, Func<double, double> formula, AbstractSyntaxTree input)
        {
            this.FormulaName = FormulaName;
            this.formula = formula;
            this.input = input;
        }
        private string FormulaName;
        private Func<double, double> formula;
        private AbstractSyntaxTree input;

        public override object Evaluate(object[] Parameters)
        {
            var value = MathConverter.ConvertToDouble(input.Evaluate(Parameters));
            if (value == null)
                return null;
            else
                return formula(value.Value);
        }
        public override string ToString()
        {
            return FormulaName + "(" + input + ")";
        }
    }
    
    class FormulaNode2 : AbstractSyntaxTree
    {
        public FormulaNode2(string FormulaName, Func<object, object, object> formula, AbstractSyntaxTree arg1, AbstractSyntaxTree arg2)
        {
            this.FormulaName = FormulaName;
            this.formula = formula;
            this.arg1 = arg1;
            this.arg2 = arg2;
        }
        private string FormulaName;
        private Func<object, object, object> formula;
        private AbstractSyntaxTree arg1, arg2;
        public override object Evaluate(object[] Parameters)
        {
            var val1 = arg1.Evaluate(Parameters);
            var val2 = arg2.Evaluate(Parameters);
            return formula(val1, val2);
        }
        public override string ToString()
        {
            return FormulaName + "(" + arg1 + ", " + arg2 + ")";
        }
    }
    /// <summary>
    /// A formula that takes one to infinity arguments.
    /// </summary>
    class FormulaNodeN : AbstractSyntaxTree
    {
        public static object And(IEnumerable<object> args)
        {
            foreach (bool arg in args)
            {
                if (!arg)
                    return false;
            }

            return true;
        }
        public static object Or(IEnumerable<object> args)
        {
            foreach (bool arg in args)
            {
                if (arg)
                    return true;
            }

            return false;
        }
        public static object Nor(IEnumerable<object> args)
        {
            foreach (bool arg in args)
            {
                if (arg)
                    return false;
            }

            return true;
        }
        public static object Max(IEnumerable<object> args)
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
        public static object Min(IEnumerable<object> args)
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
        public static object Format(IEnumerable<object> args)
        {
            dynamic format = args.First();

            return string.Format(format, args.Skip(1).ToArray());
        }
        public static string Concat(IEnumerable<object> args)
        {
            List<object> argVals = args.ToList();
            if (argVals.Count == 1 && argVals[0] is IEnumerable)
                return string.Concat(argVals[0] as dynamic);
            else
                return string.Concat(argVals);
        }
        public static string Join(IEnumerable<object> args)
        {
            dynamic separator = args.First();

            var argVals = args.Skip(1).ToArray();
            if (argVals.Length == 1 && argVals[0] is IEnumerable)
                return string.Join(separator, argVals[0] as dynamic);
            else
                return string.Concat(argVals);
        }
        public static object Average(IEnumerable<object> args)
        {
            dynamic sum = 0.0;
            var count = 0;
            foreach (var arg in args)
            {
                if (arg != null)
                {
                    count++;
                    sum += arg;
                }
            }
            if (count == 0)
                return null;

            return sum / count;
        }
        public FormulaNodeN(string FormulaName, Func<IEnumerable<object>, object> formula, IEnumerable<AbstractSyntaxTree> args)
        {
            this.FormulaName = FormulaName;
            this.formula = formula;
            this.args = args;
        }
        private string FormulaName;
        private Func<IEnumerable<object>, object> formula;
        private IEnumerable<AbstractSyntaxTree> args;
        public override object Evaluate(object[] Parameters)
        {
            return formula(args.Select(p => p.Evaluate(Parameters)));
        }

        public override string ToString()
        {
            return FormulaName + "(" + string.Join(", ", args) + ")";
        }
    }
}
