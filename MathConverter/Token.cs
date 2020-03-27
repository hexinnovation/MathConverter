using System.Collections.Generic;

namespace HexInnovation
{
    class Token
    {
        public Token(TokenType tokenType)
        {
            TokenType = tokenType;
        }
        public TokenType TokenType { get; }

        public override string ToString() => $"{TokenType} token";
    }
    class LexicalToken : Token
    {
        public LexicalToken(TokenType tokenType, string lex)
            : base(tokenType)
        {
            Lex = lex;
        }
        public string Lex { get; }

        public override string ToString()
        {
            return $"Lexical ({TokenType}) Token (\"{Lex.Replace("\"", "\\\"")}\")";
        }
    }
    class InterpolatedStringToken : LexicalToken
    {
        public InterpolatedStringToken(string lex, List<AbstractSyntaxTree> arguments)
            : base(TokenType.InterpolatedString, lex)
        {
            Arguments = arguments;
        }
        public List<AbstractSyntaxTree> Arguments { get; }
    }
    enum TokenType
    {
        X,
        Y,
        Z,
        Number,
        Plus,
        Minus,
        Times,
        Divide,
        LBracket,
        RBracket,
        LParen,
        RParen,
        EOF,
        Semicolon,
        Caret,
        Lexical,
        Not,
        DoubleEqual,
        NotEqual,
        LessThan,
        GreaterThan,
        LessThanEqual,
        GreaterThanEqual,
        QuestionMark,
        DoubleQuestionMark,
        Colon,
        String,
        Or,
        And,
        Modulo,
        InterpolatedString,
        RCurlyBracket,
    }
}
