using System;

namespace HexInnovation
{
    public class ParsingException : Exception
    {
        internal ParsingException(Scanner scanner)
        {
            Position = scanner.Position;
            Expression = scanner.Expression;
        }
        internal ParsingException(Scanner scanner, string message) : base(message)
        {
            Position = scanner.Position;
            Expression = scanner.Expression;
        }
        internal ParsingException(Scanner scanner, string message, Exception inner) : base(message, inner)
        {
            Position = scanner.Position;
            Expression = scanner.Expression;
        }
        /// <summary>
        /// The position in the string at which an exception was thrown.
        /// </summary>
        public int Position { get; }
        public string Expression { get; }

        public override string Message => $"The parser threw an exception at the {MathConverter.ComputeOrdinal(Position)} character:\r\n{base.Message}\r\n\r\nExpression: \"{Expression}\"";
    }
}
