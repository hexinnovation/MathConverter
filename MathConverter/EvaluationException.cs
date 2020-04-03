using System;
using System.Linq;

namespace HexInnovation
{
    public class EvaluationException : Exception
    {
        private static string ComputeMessage(Exception innerException, string converterParameter, object[] bindingValues)
        {
            return $"MathConverter threw an exception while performing a conversion.{Environment.NewLine}{Environment.NewLine}{nameof(ConverterParameter)}:{Environment.NewLine}{converterParameter}{Environment.NewLine}{Environment.NewLine}{nameof(BindingValues)}:{string.Concat(bindingValues.Select((p, i) => $"{Environment.NewLine}[{i}]: {(p == null ? "null" : $"({p.GetType().FullName}):  {p}")}").MyToArray())}";
        }
        public EvaluationException(string converterParameter, object[] bindingValues, NodeEvaluationException inner) : base(ComputeMessage(inner, converterParameter, bindingValues), inner)
        {
            ConverterParameter = converterParameter;
            BindingValues = bindingValues;
        }
        public string ConverterParameter { get; }
        public object[] BindingValues { get; }
    }
    public class NodeEvaluationException : Exception
    {
        private static string ComputeMessage(Exception innerException, AbstractSyntaxTree node)
        {
            return $"A {innerException.GetType().FullName} was thrown while evaluating the {node.GetType().Name}:{Environment.NewLine}{node}";
        }

        internal NodeEvaluationException(AbstractSyntaxTree node, Exception inner) : base(ComputeMessage(inner, node), inner)
        {
            Node = node;
        }

        /// <summary>
        /// The abstract syntax tree that threw an exception while being evaluated.
        /// </summary>
        internal AbstractSyntaxTree Node { get; }
    }
}
