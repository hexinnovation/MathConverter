using System;

namespace HexInnovation
{
    public class ParsingException : Exception
    {
        public ParsingException(int position)
        {
            Position = position;
        }
        public ParsingException(int position, string message) : base(message)
        {
            Position = position;
        }
        public ParsingException(int position, string message, Exception inner) : base(message, inner)
        {
            Position = position;
        }
        /// <summary>
        /// The position in the string at which an exception was thrown.
        /// </summary>
        public int Position { get; set; }

        public override string Message => $"The parser threw an exception at the {MathConverter.ComputeOrdinal(Position)} character:\r\n{base.Message}";
    }
}
