using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexInnovation
{
    public abstract class AbstractSyntaxTree
    {
        public abstract object Evaluate(object[] Parameters);
        //public double? EvaluateAsDouble(object[] Parameters)
        //{
        //    return MathConverter.ConvertToDouble(Evaluate(Parameters));
        //}
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
    }
    class AddNode : BinaryNode
    {
        public AddNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override object Evaluate(object[] Parameters)
        {
            return (dynamic)left.Evaluate(Parameters) + right.Evaluate(Parameters);
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
            return (dynamic)left.Evaluate(Parameters) - (dynamic)right.Evaluate(Parameters);
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
    }
    /// <summary>
    /// A constant, like e or pi
    /// </summary>
    class ConstantNumberNode : ValueNode
    {
        public ConstantNumberNode(double Value)
            : base(Value) { }
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

                var error = new StringBuilder("Error accessing variable ").Append(variableName);

                if (Parameters.Length == 0)
                    error.Append("No");
                else
                    error.Append("Only ").Append(Parameters.Length);

                throw new IndexOutOfRangeException(error.Append(" variable").Append(Parameters.Length == 1 ? " was" : "s were")
                    .Append(" specified.").ToString());
            }

            return MathConverter.ConvertToObject(Parameters[Index]);
        }
    }
    /// <summary>
    /// A formula that takes one input
    /// </summary>
    class FormulaNode1 : AbstractSyntaxTree
    {
        public FormulaNode1(Func<double, double> formula, AbstractSyntaxTree input)
        {
            this.formula = formula;
            this.input = input;
        }
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
    }
    
    class FormulaNode2 : AbstractSyntaxTree
    {
        public FormulaNode2(Func<double, double, double> formula, AbstractSyntaxTree arg1, AbstractSyntaxTree arg2)
        {
            this.formula = formula;
            this.arg1 = arg1;
            this.arg2 = arg2;
        }
        private Func<double, double, double> formula;
        private AbstractSyntaxTree arg1, arg2;
        public override object Evaluate(object[] Parameters)
        {
            var val1 = MathConverter.ConvertToDouble(arg1.Evaluate(Parameters));
            var val2 = MathConverter.ConvertToDouble(arg2.Evaluate(Parameters));
            if (val1.HasValue && val2.HasValue)
                return formula(val1.Value, val2.Value);
            else
                return null;
        }
    }
    /// <summary>
    /// A formula that takes one to infinity arguments.
    /// </summary>
    class FormulaNodeN : AbstractSyntaxTree
    {
        public static object Max(IEnumerable<object> args)
        {
            dynamic max = null;
            foreach (var arg in args)
            {
                if (arg < max)
                {
                    max = arg;
                }
            }
            return max;
        }
        public static object Min(IEnumerable<object> args)
        {
            dynamic min = null;
            foreach (var arg in args)
            {
                if (arg < min)
                {
                    min = arg;
                }
            }
            return min;
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
        public FormulaNodeN(Func<IEnumerable<object>, object> formula, IEnumerable<AbstractSyntaxTree> args)
        {
            this.formula = formula;
            this.args = args;
        }
        private Func<IEnumerable<object>, object> formula;
        private IEnumerable<AbstractSyntaxTree> args;
        public override object Evaluate(object[] Parameters)
        {
            return formula(args.Select(p => p.Evaluate(Parameters)));
        }
    }
}
