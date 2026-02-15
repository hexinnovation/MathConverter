using System;
using System.Linq;

namespace HexInnovation
{
    public class EvaluationException(string converterParameter, object?[] bindingValues, NodeEvaluationException inner) : Exception($"MathConverter threw an exception while performing a conversion.{Environment.NewLine}{Environment.NewLine}{nameof(ConverterParameter)}:{Environment.NewLine}{converterParameter}{Environment.NewLine}{Environment.NewLine}{nameof(BindingValues)}:{string.Concat<string>(bindingValues.Select((p, i) => $"{Environment.NewLine}[{i}]: {(p == null ? "null" : $"({p.GetType().FullName}):  {p}")}"))}", inner)
    {
        public string ConverterParameter => converterParameter;
        public object?[] BindingValues => bindingValues;
    }
    public class NodeEvaluationException(AbstractSyntaxTree node, Exception inner) : Exception($"A {inner.GetType().FullName} was thrown while evaluating the {node.GetType().Name}:{Environment.NewLine}{node}", inner)
    {
        /// <summary>
        /// The abstract syntax tree that threw an exception while being evaluated.
        /// </summary>
        internal AbstractSyntaxTree Node => node;
    }
}
