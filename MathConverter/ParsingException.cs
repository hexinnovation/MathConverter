using System;
using System.Runtime.Serialization;

namespace HexInnovation
{
    [Serializable]
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
        protected ParsingException(int position, SerializationInfo info, StreamingContext context)
            : base(info, context)
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
