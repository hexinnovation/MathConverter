using System.Collections.Generic;

namespace HexInnovation
{
    internal class Token(TokenType tokenType)
    {
        public TokenType TokenType => tokenType;

        public override string ToString() => $"{TokenType} token";
    }
    internal class LexicalToken(TokenType tokenType, string lex) : Token(tokenType)
    {
        public string Lex => lex;

        public override string ToString()
        {
            return $"Lexical ({TokenType}) Token (\"{Lex.Replace("\"", "\\\"")}\")";
        }
    }
    internal class InterpolatedStringToken(string lex, List<AbstractSyntaxTree> arguments) : LexicalToken(TokenType.InterpolatedString, lex)
    {
        public List<AbstractSyntaxTree> Arguments => arguments;
    }
    internal enum TokenType
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
