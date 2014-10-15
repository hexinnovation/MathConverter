using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexInnovation
{
    public abstract class AbstractSyntaxTree
    {
        public abstract double Evaluate(object[] Parameters);
    }
    abstract class BinaryNode : AbstractSyntaxTree
    {
        public BinaryNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
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
        public override double Evaluate(object[] Parameters)
        {
            return Math.Pow(left.Evaluate(Parameters), right.Evaluate(Parameters));
        }
    }
    class AddNode : BinaryNode
    {
        public AddNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override double Evaluate(object[] Parameters)
        {
            return left.Evaluate(Parameters) + right.Evaluate(Parameters);
        }
    }
    class SubNode : BinaryNode
    {
        public SubNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override double Evaluate(object[] Parameters)
        {
            return left.Evaluate(Parameters) - right.Evaluate(Parameters);
        }
    }
    class TimesNode : BinaryNode
    {
        public TimesNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override double Evaluate(object[] Parameters)
        {
            return left.Evaluate(Parameters) * right.Evaluate(Parameters);
        }
    }
    class DivideNode : BinaryNode
    {
        public DivideNode(AbstractSyntaxTree left, AbstractSyntaxTree right)
            : base(left, right)
        {

        }
        public override double Evaluate(object[] Parameters)
        {
            return left.Evaluate(Parameters) / right.Evaluate(Parameters);
        }
    }
    class NegativeNode : AbstractSyntaxTree
    {
        public NegativeNode(AbstractSyntaxTree right)
        {
            this.right = right;
        }
        private AbstractSyntaxTree right;
        public override double Evaluate(object[] Parameters)
        {
            return -right.Evaluate(Parameters);
        }
    }
    class NumNode : AbstractSyntaxTree
    {
        public double Value { get; set; }
        public NumNode(double Value)
        {
            this.Value = Value;
        }
        public override double Evaluate(object[] Parameters)
        {
            return Value;
        }
    }
    /// <summary>
    /// A constant, like e or pi
    /// </summary>
    class ConstantNode : NumNode
    {
        public ConstantNode(double Value)
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
        public override double Evaluate(object[] Parameters)
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

                throw new IndexOutOfRangeException(error.Append(" variable").Append(Parameters.Length == 1 ? " " : "s ")
                    .Append(Parameters.Length == 1 ? "was" : "were").Append(" specified.").ToString());
            }

            return MathConverter.ConvertToDouble(Parameters[Index]);
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

        public override double Evaluate(object[] Parameters)
        {
            return formula(input.Evaluate(Parameters));
        }
    }
    /// <summary>
    /// A formula that takes two inputs.
    /// </summary>
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
        public override double Evaluate(object[] Parameters)
        {
            return formula(arg1.Evaluate(Parameters), arg2.Evaluate(Parameters));
        }
    }
    /// <summary>
    /// A formula that takes one to infinity arguments.
    /// </summary>
    class FormulaNodeN : AbstractSyntaxTree
    {
        public static double Max(IEnumerable<double> args)
        {
            var max = double.NaN;
            foreach (var arg in args)
            {
                if (double.IsNaN(max) || arg < max)
                {
                    max = arg;
                }
            }
            return max;
        }
        public static double Min(IEnumerable<double> args)
        {
            var min = double.NaN;
            foreach (var arg in args)
            {
                if (double.IsNaN(min) || arg < min)
                {
                    min = arg;
                }
            }
            return min;
        }
        public static double Average(IEnumerable<double> args)
        {
            var sum = 0.0;
            var count = 0;
            foreach (var arg in args)
            {
                count++;
                sum += arg;
            }
            if (count == 0)
                return double.NaN;

            return sum / count;
        }
        public FormulaNodeN(Func<IEnumerable<double>, double> formula, IEnumerable<AbstractSyntaxTree> args)
        {
            this.formula = formula;
            this.args = args;
        }
        private Func<IEnumerable<double>, double> formula;
        private IEnumerable<AbstractSyntaxTree> args;
        public override double Evaluate(object[] Parameters)
        {
            return formula(args.Select(p => p.Evaluate(Parameters)));
        }
    }
}
