using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HexInnovation
{
    class Scanner : IDisposable
    {
        public Scanner(Parser parser, string Expression)
            : this(parser, new StringReader(Expression)) { }
        public Scanner(Parser parser, StringReader reader)
        {
            this._parser = parser;
            this._reader = reader;
            _needsToken = true;
            Position = -1;
        }
        private Parser _parser;
        private TextReader _reader;
        private Token _lastToken;
        private bool _needsToken;
        public int Position { get; private set; }


        public Token Peek()
        {
            try
            {
                return GetToken();
            }
            finally
            {
                PutBackToken();
            }
        }
        public Token GetToken()
        {
            if (_needsToken)
            {
                return _lastToken = NextToken();
            }
            else
            {
                _needsToken = true;
                return _lastToken;
            }
        }
        private Token NextToken()
        {
            var state = ScannerState.NoToken;
            var sb = new StringBuilder();

            // Get the next character.
            var ch = _reader.Read();
            Position++;

            while (true)
            {
                switch (state)
                {
                    case ScannerState.NoToken:
                        switch (ch)
                        {
                            case -1:
                                return new Token(TokenType.EOF);
                            case '+':
                                return new Token(TokenType.Plus);
                            case '-':
                                return new Token(TokenType.Minus);
                            case '*':
                                return new Token(TokenType.Times);
                            case '%':
                                return new Token(TokenType.Modulo);
                            case '/':
                                return new Token(TokenType.Divide);
                            case '(':
                                return new Token(TokenType.LParen);
                            case '^':
                                return new Token(TokenType.Caret);
                            case '[':
                                return new Token(TokenType.LBracket);
                            case ']':
                                return new Token(TokenType.RBracket);
                            case ')':
                                return new Token(TokenType.RParen);
                            case '}':
                                return new Token(TokenType.RCurlyBracket);
                            case ';':
                            case ',':
                                return new Token(TokenType.Semicolon);
                            case 'X':
                            case 'x':
                                return new Token(TokenType.X);
                            case 'Y':
                            case 'y':
                                return new Token(TokenType.Y);
                            case 'Z':
                            case 'z':
                                return new Token(TokenType.Z);
                            case '?':
                                switch (_reader.Peek())
                                {
                                    case '?':
                                        Position++;
                                        _reader.Read();
                                        return new Token(TokenType.DoubleQuestionMark);
                                    default:
                                        return new Token(TokenType.QuestionMark);
                                }
                            case ':':
                                return new Token(TokenType.Colon);
                            case '.':
                                state = ScannerState.NumberAfterDecimal;
                                sb.Append('.');
                                break;
                            case '"':
                                state = ScannerState.DoubleQuoteString;
                                break;
                            case '`':
                                state = ScannerState.CaretString;
                                break;
                            case '$':
                                Position++;
                                ch = _reader.Read();
                                switch (ch)
                                {
                                    case '`':
                                        state = ScannerState.InterpolatedString | ScannerState.CaretString;
                                        break;
                                    case '"':
                                        state = ScannerState.InterpolatedString | ScannerState.DoubleQuoteString;
                                        break;
                                    default:
                                        throw new ParsingException(Position, "A '$' character must be proceeded by a caret (`) or double-quote (\") character.");
                                }
                                break;
                            case '!':
                                switch (_reader.Peek())
                                {
                                    case '=':
                                        _reader.Read();
                                        Position++;
                                        return new Token(TokenType.NotEqual);
                                    default:
                                        return new Token(TokenType.Not);
                                }
                            case '=':
                                Position++;
                                if (_reader.Read() != '=')
                                    throw new ParsingException(Position, "'=' signs are only valid after as part of one of the following two operators: '!=', '==', '<=', and '>='");
                                return new Token(TokenType.DoubleEqual);
                            case '<':
                                switch (_reader.Peek())
                                {
                                    case '=':
                                        _reader.Read();
                                        Position++;
                                        return new Token(TokenType.LessThanEqual);
                                    default:
                                        return new Token(TokenType.LessThan);
                                }
                            case '>':
                                switch (_reader.Peek())
                                {
                                    case '=':
                                        _reader.Read();
                                        Position++;
                                        return new Token(TokenType.GreaterThanEqual);
                                    default:
                                        return new Token(TokenType.GreaterThan);
                                }
                            case '|':
                                Position++;
                                if (_reader.Read() != '|')
                                    throw new ParsingException(Position, "'|' signs are only valid in pairs of two.");
                                return new Token(TokenType.Or);
                            case '&':
                                Position++;
                                if (_reader.Read() != '&')
                                    throw new ParsingException(Position, "'&' signs are only valid in pairs of two.");
                                return new Token(TokenType.And);
                            default:
                                if (char.IsDigit((char)ch))
                                {
                                    state = ScannerState.Number;
                                    sb.Append((char)ch);
                                }
                                else if (char.IsLetter((char)ch))
                                {
                                    state = ScannerState.Lexical;
                                    sb.Append((char)ch);
                                }
                                else if (char.IsWhiteSpace((char)ch))
                                {
                                    // We simply ignore whitespace; skip this character.
                                    ch = _reader.Read();
                                    Position++;
                                }
                                else
                                {
                                    throw new ParsingException(Position, $"Found invalid token '{(char)ch}'");
                                }
                                break;
                        }
                        break;
                    case ScannerState.Number:
                    case ScannerState.NumberAfterDecimal:
                        var acceptDot = state == ScannerState.Number;

                        while (true)
                        {
                            ch = _reader.Peek();
                            if ((ch == '.' && acceptDot) || char.IsDigit((char)ch))
                            {
                                sb.Append((char)ch);
                                _reader.Read();
                                Position++;

                                acceptDot = acceptDot && ch != '.';
                            }
                            else
                            {
                                if (ch == '.')
                                    throw new ParsingException(Position, "Found second decimal in number " + sb.ToString());
                                else if (sb.ToString().Last() == '.')
                                    throw new ParsingException(Position, "A number cannot end in a decimal.  The number was: " + sb.ToString());

                                return new LexicalToken(TokenType.Number, sb.ToString());
                            }
                        }
                    case ScannerState.Lexical:
                        while (true)
                        {
                            ch = _reader.Peek();

                            switch (ch)
                            {
                                case -1:
                                    return new LexicalToken(TokenType.Lexical, sb.ToString());
                                default:
                                    if (char.IsLetterOrDigit((char)ch))
                                    {
                                        sb.Append((char)ch);
                                        _reader.Read();
                                        Position++;
                                        break;
                                    }

                                    return new LexicalToken(TokenType.Lexical, sb.ToString());
                            }
                        }

                    case ScannerState.CaretString | ScannerState.InterpolatedString:
                    case ScannerState.DoubleQuoteString | ScannerState.InterpolatedString:
                    case ScannerState.CaretString:
                    case ScannerState.DoubleQuoteString:
                        var isInterpolatedString = (state & ScannerState.InterpolatedString) == ScannerState.InterpolatedString;
                        var Arguments = new List<AbstractSyntaxTree>();

                        while (true)
                        {
                            ch = _reader.Read();
                            Position++;

                            switch (ch)
                            {
                                default:
                                    sb.Append((char)ch);
                                    break;
                                case '{':
                                    sb.Append((char)ch);
                                    if (isInterpolatedString)
                                    {
                                        if (_reader.Peek() == '{')
                                        {
                                            ch = _reader.Read();
                                            Position++;
                                            sb.Append((char)ch);
                                        }
                                        else
                                        {
                                            /*
                                             * {{ => {
                                             * }} => }
                                             *  \ => backslash-escaped.
                                             *  ` => maybe throw
                                             *  " => maybe throw
                                             */

                                            sb.Append(Arguments.Count);
                                            try
                                            {
                                                Arguments.Add(_parser.ParseInterpolatedStringArg());
                                                switch (GetToken().TokenType)
                                                {
                                                    case TokenType.Colon:
                                                        sb.Append(':');
                                                        while (ch != '}')
                                                        {
                                                            Position++;
                                                            ch = _reader.Read();

                                                            switch (ch)
                                                            {
                                                                case -1:
                                                                    throw new ParsingException(Position, "Missing close delimiter '}' for interpolated expression started with '{'.");
                                                                default:
                                                                    sb.Append((char)ch);
                                                                    break;
                                                                case '}':
                                                                    sb.Append((char)ch);
                                                                    if (_reader.Peek() == '}')
                                                                    {
                                                                        Position++;
                                                                        _reader.Read();
                                                                        sb.Append((char)ch);
                                                                        break;
                                                                    }
                                                                    break;
                                                                case '{':
                                                                    sb.Append((char)ch);
                                                                    sb.Append((char)ch);
                                                                    Position++;
                                                                    if (_reader.Read() != ch)
                                                                    {
                                                                        throw new ParsingException(Position, "A '{' character must be escaped (by doubling) in an interpolated string's argument.");
                                                                    }
                                                                    break;

                                                                case '\\':
                                                                    Position++;
                                                                    switch (ch = _reader.Read())
                                                                    {
                                                                        case 'a':
                                                                            sb.Append('\a');
                                                                            break;
                                                                        case 'b':
                                                                            sb.Append('\b');
                                                                            break;
                                                                        case 'f':
                                                                            sb.Append('\f');
                                                                            break;
                                                                        case 'n':
                                                                            sb.Append('\n');
                                                                            break;
                                                                        case 'r':
                                                                            sb.Append('\r');
                                                                            break;
                                                                        case 't':
                                                                            sb.Append('\t');
                                                                            break;
                                                                        case 'v':
                                                                            sb.Append('\v');
                                                                            break;
                                                                        case '\\':
                                                                            sb.Append('\\');
                                                                            break;
                                                                        case '`':
                                                                            sb.Append('`');
                                                                            break;
                                                                        case '"':
                                                                            sb.Append('"');
                                                                            break;
                                                                        default:
                                                                            throw new ParsingException(Position, $"The character \'\\{(char)ch}\' is not a valid backslash-escaped character.");
                                                                    }
                                                                    break;
                                                                case '`':
                                                                    if ((state & ~ScannerState.InterpolatedString) == ScannerState.CaretString)
                                                                        throw new ParsingException(Position, "Missing close delimiter '}' for interpolated expression started with '{'.");
                                                                    sb.Append('`');
                                                                    break;
                                                                case '"':
                                                                    if ((state & ~ScannerState.InterpolatedString) == ScannerState.DoubleQuoteString)
                                                                        throw new ParsingException(Position, "Missing close delimiter '}' for interpolated expression started with '{'.");
                                                                    sb.Append('"');
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    case TokenType.RCurlyBracket:
                                                        sb.Append('}');
                                                        break;
                                                    default:
                                                        throw new Exception(); // This should never ever happen because of the body of Parser.ParseInterpolatedStringArg().
                                                }
                                            }
                                            catch (Exception e)
#if DEBUG
#pragma warning disable CS7095 // Filter expression is a constant
                                            when (false)
#pragma warning restore CS7095 // Filter expression is a constant
#endif
                                            {
                                                throw new ParsingException(Position, "Failed to parse the interpolated string to a call to String.Format. See the inner exception.", e);
                                            }
                                        }
                                    }
                                    break;
                                case '\\':
                                    Position++;
                                    switch (ch = _reader.Read())
                                    {
                                        case 'a':
                                            sb.Append('\a');
                                            break;
                                        case 'b':
                                            sb.Append('\b');
                                            break;
                                        case 'f':
                                            sb.Append('\f');
                                            break;
                                        case 'n':
                                            sb.Append('\n');
                                            break;
                                        case 'r':
                                            sb.Append('\r');
                                            break;
                                        case 't':
                                            sb.Append('\t');
                                            break;
                                        case 'v':
                                            sb.Append('\v');
                                            break;
                                        case '\\':
                                            sb.Append('\\');
                                            break;
                                        case '`':
                                            sb.Append('`');
                                            break;
                                        case '"':
                                            sb.Append('"');
                                            break;
                                        default:
                                            throw new ParsingException(Position, $"The character \'\\{(char)ch}\' is not a valid backslash-escaped character.");
                                    }
                                    break;
                                case '"':
                                    switch (state & ~ScannerState.InterpolatedString)
                                    {
                                        case ScannerState.CaretString:
                                            sb.Append('"');
                                            break;
                                        case ScannerState.DoubleQuoteString:
                                            if (isInterpolatedString)
                                                return new InterpolatedStringToken(sb.ToString(), Arguments);
                                            else
                                                return new LexicalToken(TokenType.String, sb.ToString());
                                    }
                                    break;
                                case '`':
                                    switch (state & ~ScannerState.InterpolatedString)
                                    {
                                        case ScannerState.CaretString:
                                            if (isInterpolatedString)
                                                return new InterpolatedStringToken(sb.ToString(), Arguments);
                                            else
                                                return new LexicalToken(TokenType.String, sb.ToString());
                                        case ScannerState.DoubleQuoteString:
                                            sb.Append('`');
                                            break;
                                    }
                                    break;
                                case -1:
                                    throw new ParsingException(Position, $"Could not find the end of the {(isInterpolatedString ? "interpolated " : "")}string.");

                            }
                        }
                }
            }
        }

        public void PutBackToken()
        {
            _needsToken = false;
        }

        enum ScannerState
        {
            NoToken = 0,
            Number = 1,
            NumberAfterDecimal = 2,
            Lexical = 4,
            DoubleQuoteString = 8,
            CaretString = 16,

            InterpolatedString = 0x8000,
        }

        ~Scanner()
        {
            Dispose();
        }
        public void Dispose()
        {
            _reader.Dispose();
        }
    }

    [Serializable]
    public class ParsingException : Exception
    {
        public ParsingException(int position)
        {
            this.Position = position;
        }
        public ParsingException(int position, string message) : base(message)
        {
            this.Position = position;
        }
        public ParsingException(int position, string message, Exception inner) : base(message, inner)
        {
            this.Position = position;
        }
        protected ParsingException(int position, SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Position = position;
        }
        /// <summary>
        /// The position in the string at which an exception was thrown.
        /// </summary>
        public int Position { get; set; }
        private string PositionOrdinal
        {
            get
            {
                if (Position < 11 || Position > 13)
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
        public override string Message
        {
            get
            {
                return $"The parser threw an exception at the {Position}{PositionOrdinal} character:\r\n{base.Message}";
            }
        }
    }
}
