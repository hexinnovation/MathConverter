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
        public Scanner(string Expression)
        {
            reader = new StringReader(Expression);
            needsToken = true;
            Position = -1;
        }

        private TextReader reader;
        private Token lastToken;
        private bool needsToken;
        public int Position { get; private set; }

        public Token GetToken()
        {
            if (needsToken)
            {
                return lastToken = NextToken();
            }
            else
            {
                needsToken = true;
                return lastToken;
            }
        }
        private Token NextToken()
        {
            var state = ScannerState.NoToken;
            var sb = new StringBuilder();

            // Get the next character.
            var ch = reader.Read();
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
                            case '.':
                                state = ScannerState.NumberAfterDecimal;
                                sb.Append('.');
                                break;
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
                                else
                                {
                                    throw new ParsingException(Position, "Found invalid token '" + (char)ch + '\'');
                                }
                                break;
                        }
                        break;
                    case ScannerState.Number:
                    case ScannerState.NumberAfterDecimal:
                        var acceptDot = state == ScannerState.Number;

                        while (true)
                        {
                            ch = reader.Peek();
                            if ((ch == '.' && acceptDot) || char.IsDigit((char)ch))
                            {
                                sb.Append((char)ch);
                                reader.Read();
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
                            ch = reader.Peek();

                            if (ch != '(')
                            {
                                sb.Append((char)ch);
                                reader.Read();
                            }
                            else
                            {
                                return new LexicalToken(TokenType.Lexical, sb.ToString().ToLower());
                            }
                        }
                }
            }
        }

        public void PutBackToken()
        {
            needsToken = false;
        }

        enum ScannerState
        {
            NoToken,
            Number,
            NumberAfterDecimal,
            Lexical,
        }

        ~Scanner()
        {
            Dispose();
        }
        public void Dispose()
        {
            reader.Dispose();
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
        public override string Message
        {
            get
            {
                return string.Format("The parser threw an exception at the {0}th character:\r\n{1}", Position, base.Message);
            }
        }
    }
}
