using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexInnovation
{
    class Token
    {
        public Token(TokenType TokenType)
        {
            this.TokenType = TokenType;
        }
        public TokenType TokenType { get; set; }

        public override string ToString()
        {
            return $"{TokenType} token";
        }
    }
    class LexicalToken : Token
    {
        public LexicalToken(TokenType TokenType, string Lex)
            : base(TokenType)
        {
            this.Lex = Lex;
        }
        public string Lex { get; set; }

        public override string ToString()
        {
            return $"Lexical ({TokenType}) Token (\"{Lex.Replace("\"", "\\\"")}\")";
        }
    }
    class InterpolatedStringToken : LexicalToken
    {
        public InterpolatedStringToken(string Lex, List<AbstractSyntaxTree> Arguments)
            : base(TokenType.InterpolatedString, Lex)
        {
            this.Arguments = Arguments;
        }
        public List<AbstractSyntaxTree> Arguments { get; set; }
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
