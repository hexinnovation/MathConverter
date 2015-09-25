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
            return TokenType.ToString() + " token";
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
            return "Lexical Token (\"" + Lex + "\")";
        }
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
    }
}
