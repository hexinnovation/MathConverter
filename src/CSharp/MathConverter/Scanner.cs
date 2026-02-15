using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HexInnovation;

internal class Scanner : IDisposable
{
    public Scanner(Parser parser, string expression)
        : this(parser, new StringReader(expression), expression)
    {
    }
    private Scanner(Parser parser, StringReader reader, string expression)
    {
        _parser = parser;
        _reader = reader;
        Expression = expression;
    }

    private readonly Parser _parser;
    private readonly StringReader _reader;

    private Token? _lastToken;
    private bool _needsToken = true;
    internal string Expression { get; }
    public int Position { get; private set; } = -1;


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
            return _lastToken!;
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
                            return new(TokenType.EOF);

                        case '+':
                            if (_reader.Peek() == '+')
                                throw new ParsingException(this, "The ++ operator is not supported.");
                            return new(TokenType.Plus);

                        case '-':
                            if (_reader.Peek() == '-')
                                throw new ParsingException(this, "The -- operator is not supported.");
                            return new(TokenType.Minus);

                        case '*':
                            return new(TokenType.Times);

                        case '%':
                            return new(TokenType.Modulo);

                        case '/':
                            return new(TokenType.Divide);

                        case '(':
                            return new(TokenType.LParen);

                        case '^':
                            return new(TokenType.Caret);

                        case '[':
                            return new(TokenType.LBracket);

                        case ']':
                            return new(TokenType.RBracket);

                        case ')':
                            return new(TokenType.RParen);

                        case '}':
                            return new(TokenType.RCurlyBracket);

                        case ';' or ',':
                            return new(TokenType.Semicolon);

                        case 'x':
                            return new(TokenType.X);

                        case 'y':
                            return new(TokenType.Y);

                        case 'z':
                            return new(TokenType.Z);

                        case '?':
                            if (_reader.Peek() == '?')
                            {
                                Position++;
                                _reader.Read();
                                return new(TokenType.DoubleQuestionMark);
                            }

                            return new(TokenType.QuestionMark);

                        case ':':
                            return new(TokenType.Colon);

                        case '.':
                            state = ScannerState.NumberAfterDecimal;
                            sb.Append('.');
                            continue;

                        case '`':
                            state = ScannerState.CaretString;
                            continue;

                        case '"':
                            state = ScannerState.DoubleQuoteString;
                            continue;

                        case '\'':
                            state = ScannerState.SingleQuoteString;
                            continue;

                        case '$':
                            Position++;
                            state = _reader.Read() switch
                            {
                                '`' => ScannerState.InterpolatedString | ScannerState.CaretString,
                                '"' => ScannerState.InterpolatedString | ScannerState.DoubleQuoteString,
                                '\'' => ScannerState.InterpolatedString | ScannerState.SingleQuoteString,
                                _ => throw new ParsingException(this, "A '$' character must be followed by a caret (`), double-quote (\"), or single-quote (') character.")
                            };
                            continue;

                        case '!':
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                Position++;
                                return new(TokenType.NotEqual);
                            }

                            return new(TokenType.Not);

                        case '=':
                            Position++;
                            if (_reader.Read() != '=')
                                throw new ParsingException(this, "'=' signs are only valid after as part of one of the following two operators: '!=', '==', '<=', and '>='");
                            return new(TokenType.DoubleEqual);

                        case '<':
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                Position++;
                                return new(TokenType.LessThanEqual);
                            }
                            return new(TokenType.LessThan);

                        case '>':
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                Position++;
                                return new(TokenType.GreaterThanEqual);
                            }

                            return new(TokenType.GreaterThan);

                        case '|':
                            Position++;
                            if (_reader.Read() != '|')
                                throw new ParsingException(this, "'|' signs are only valid in pairs of two.");
                            return new(TokenType.Or);

                        case '&':
                            Position++;
                            if (_reader.Read() != '&')
                                throw new ParsingException(this, "'&' signs are only valid in pairs of two.");
                            return new(TokenType.And);

                        default:
                            if (char.IsDigit((char)ch))
                            {
                                state = ScannerState.Number;
                                sb.Append((char)ch);
                                continue;
                            }

                            if (char.IsLetter((char)ch))
                            {
                                state = ScannerState.Lexical;
                                sb.Append((char)ch);
                                continue;
                            }

                            if (char.IsWhiteSpace((char)ch))
                            {
                                // We simply ignore whitespace; skip this character.
                                ch = _reader.Read();
                                Position++;
                                continue;
                            }

                            throw new ParsingException(this, $"Found invalid token '{(char)ch}'");
                    }

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

                            acceptDot &= ch != '.';
                            continue;
                        }

                        var number = sb.ToString();

                        return ch == '.'
                            ? throw new ParsingException(this, $"Found second decimal in number {sb}")
                            : number.Last() == '.'
                            ? throw new ParsingException(this, $"A number cannot end in a decimal. The number was {sb}")
                            : (Token)new LexicalToken(TokenType.Number, number);
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
                case ScannerState.SingleQuoteString | ScannerState.InterpolatedString:
                case ScannerState.CaretString:
                case ScannerState.DoubleQuoteString:
                case ScannerState.SingleQuoteString:
                    var isInterpolatedString = (state & ScannerState.InterpolatedString) == ScannerState.InterpolatedString;
                    var arguments = new List<AbstractSyntaxTree>();

                    while (true)
                    {
                        ch = _reader.Read();
                        Position++;

                        switch (ch)
                        {
                            case '{':
                                sb.Append((char)ch);
                                if (!isInterpolatedString)
                                    continue;

                                /*
                                    {{ => {
                                    }} => }
                                    \  => backslash-escaped.
                                    `  => maybe throw
                                    "  => maybe throw
                                    '  => maybe throw
                                */

                                if (_reader.Peek() == '{')
                                {
                                    ch = _reader.Read();
                                    Position++;
                                    sb.Append((char)ch);
                                    continue;
                                }

                                sb.Append(arguments.Count);
                                try
                                {
                                    arguments.Add(_parser.ParseInterpolatedStringArg());

                                    switch (GetToken().TokenType)
                                    {
                                        case (TokenType.Semicolon or TokenType.Colon) and TokenType x:
                                            sb.Append(x is TokenType.Semicolon ? ',' : ':');

                                            while (ch != '}')
                                            {
                                                Position++;
                                                ch = _reader.Read();

                                                switch (ch)
                                                {
                                                    case -1:
                                                        throw new ParsingException(this, "Missing close delimiter '}' for interpolated expression started with '{'.");

                                                    case '}':
                                                        sb.Append((char)ch);
                                                        if (_reader.Peek() == '}')
                                                        {
                                                            Position++;
                                                            _reader.Read();
                                                            sb.Append((char)ch);
                                                        }
                                                        continue;

                                                    case '{':
                                                        sb.Append((char)ch);
                                                        sb.Append((char)ch);
                                                        Position++;
                                                        if (_reader.Read() != ch)
                                                        {
                                                            throw new ParsingException(this, "A '{' character must be escaped (by doubling) in an interpolated string's argument.");
                                                        }
                                                        continue;

                                                    case '\\':
                                                        ReadBackslashEscapedCharacter(sb);
                                                        continue;

                                                    case '`':
                                                    case '"':
                                                    case '\'':
                                                        var endOfStringState = ch switch
                                                        {
                                                            '`' => ScannerState.CaretString,
                                                            '"' => ScannerState.DoubleQuoteString,
                                                            '\'' => ScannerState.SingleQuoteString,
                                                            _ => throw new InvalidOperationException("Unreachable: bad string type.")
                                                        };

                                                        if ((state & ~ScannerState.InterpolatedString) == endOfStringState)
                                                            throw new ParsingException(this, "Missing close delimiter '}' for interpolated expression started with '{'.");
                                                        sb.Append((char)ch);
                                                        continue;

                                                    default:
                                                        sb.Append((char)ch);
                                                        continue;
                                                }
                                            }
                                            continue;

                                        case TokenType.RCurlyBracket:
                                            sb.Append('}');
                                            continue;

                                        default:
                                            throw new InvalidOperationException("Unreachable due to Parser.ParseInterpolatedStringArg() implementation");
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw new ParsingException(this, "Failed to parse the interpolated string to a call to String.Format. See the inner exception.", e);
                                }

                            case '\\':
                                ReadBackslashEscapedCharacter(sb);
                                continue;

                            case '"':
                                switch (state & ~ScannerState.InterpolatedString)
                                {
                                    case ScannerState.CaretString:
                                        sb.Append('"');
                                        continue;

                                    case ScannerState.DoubleQuoteString:
                                        return isInterpolatedString ?
                                            new InterpolatedStringToken(sb.ToString(), arguments) :
                                            (Token)new LexicalToken(TokenType.String, sb.ToString());

                                    case ScannerState.SingleQuoteString:
                                        sb.Append('"');
                                        continue;

                                    default:
                                        continue;
                                }

                            case '`':
                                switch (state & ~ScannerState.InterpolatedString)
                                {
                                    case ScannerState.CaretString:
                                        return isInterpolatedString ?
                                            new InterpolatedStringToken(sb.ToString(), arguments) :
                                            (Token)new LexicalToken(TokenType.String, sb.ToString());

                                    case ScannerState.DoubleQuoteString:
                                        sb.Append('`');
                                        continue;

                                    case ScannerState.SingleQuoteString:
                                        sb.Append('`');
                                        continue;

                                    default:
                                        continue;
                                }

                            case '\'':
                                switch (state & ~ScannerState.InterpolatedString)
                                {
                                    case ScannerState.CaretString:
                                        sb.Append('\'');
                                        continue;

                                    case ScannerState.DoubleQuoteString:
                                        sb.Append('\'');
                                        continue;

                                    case ScannerState.SingleQuoteString:
                                        return isInterpolatedString ?
                                            new InterpolatedStringToken(sb.ToString(), arguments) :
                                            (Token)new LexicalToken(TokenType.String, sb.ToString());
                                    default:
                                        continue;
                                }

                            case -1:
                                throw new ParsingException(this, $"Could not find the end of the {(isInterpolatedString ? "interpolated " : "")}string.");

                            default:
                                sb.Append((char)ch);
                                continue;
                        }
                    }

                default:
                    throw new InvalidOperationException($"Unreachable: Bad token state.");
            }
        }
    }

    private void ReadBackslashEscapedCharacter(StringBuilder sb)
    {
        Position++;
        sb.Append(_reader.Read() switch
        {
            'a' => '\a',
            'b' => '\b',
            'f' => '\f',
            'n' => '\n',
            'r' => '\r',
            't' => '\t',
            'v' => '\v',
            '\\' => '\\',
            '`' => '`',
            '"' => '"',
            '\'' => '\'',
            { } c => throw new ParsingException(this, $"The character \'\\{(char)c}\' is not a valid backslash-escaped character.")
        });
    }

    public void PutBackToken() => _needsToken = false;

    public void Dispose() => _reader.Dispose();

    [Flags]
    private enum ScannerState
    {
        NoToken = 0,
        Number = 1,
        NumberAfterDecimal = 2,
        Lexical = 4,
        DoubleQuoteString = 8,
        CaretString = 16,
        SingleQuoteString = 32,

        InterpolatedString = 0x8000,
    }
}
