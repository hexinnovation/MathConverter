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
        private string PositionOrdinal
        {
            get
            {
                if (Position % 100 < 11 || Position % 100 > 13)
                {
                    switch (Position % 10)
                    {
                        case 1:
                            return "st";
                        case 2:
                            return "nd";
                        case 3:
                            return "rd";
                    }
                }
                return "th";
            }
        }
        public override string Message => $"The parser threw an exception at the {Position}{PositionOrdinal} character:\r\n{base.Message}";
    }
}
