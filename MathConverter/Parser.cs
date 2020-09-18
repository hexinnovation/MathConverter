using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if !XAMARIN
using System.Windows;
#endif

namespace HexInnovation
{
    class Parser : IDisposable
    {
        public static AbstractSyntaxTree[] Parse(CustomFunctionCollection customFunctions, string expression)
        {
            using (var p = new Parser(customFunctions, expression))
            {
                try
                {
                    return p.ConverterParameter().ToArray();
                }
                catch (ParsingException e)
#if DEBUG
#pragma warning disable CS7095 // Filter expression is a constant
#pragma warning disable CS8360 // Filter expression is a constant 'false'. 
                when (false)
#pragma warning restore CS8360 // Filter expression is a constant 'false'. 
#pragma warning restore CS7095 // Filter expression is a constant
#endif

                {
                    throw new Exception($"Failed to parse the expression:{Environment.NewLine}{expression}{Environment.NewLine}See the inner exception for details.", e);
                }
            }
        }

        private static string ErrorWrongTokenType(TokenType type)
        {
            return $"MathConverter internal exception: if TokenType is {type}, the token must be a {(type == TokenType.InterpolatedString ? nameof(InterpolatedStringToken) : nameof(LexicalToken))}.";
        }

        private readonly Scanner _scanner;
        private readonly CustomFunctionCollection _customFunctions;
        private Parser(CustomFunctionCollection customFunctions, string expression)
        {
            _scanner = new Scanner(this, expression);
            _customFunctions = customFunctions;
        }
        internal AbstractSyntaxTree ParseInterpolatedStringArg()
        {
            var result = Conditional();

            var t = _scanner.Peek();

            switch (t.TokenType)
            {
                case TokenType.Semicolon:
                case TokenType.RCurlyBracket:
                case TokenType.Colon:
                    return result;
                default:
                    throw new ParsingException(_scanner.Position, "Error parsing interpolated string. Could not find closing curly bracket (or a colon, comma, or semicolon) after the argument.");
            }
        }
        private IEnumerable<AbstractSyntaxTree> ConverterParameter()
        {
            while (true)
            {
                var result = Conditional();
                var t = _scanner.GetToken();

                switch (t.TokenType)
                {
                    case TokenType.EOF:
                        yield return result;
                        yield break;
                    case TokenType.Semicolon:
                        yield return result;
                        break;
                    default:
                        throw new ParsingException(_scanner.Position, "The conversion parameter could not be parsed to a valid string.");
                }
            }
        }
        private AbstractSyntaxTree Conditional()
        {
            return Conditional(NullCoalescing());
        }
        private AbstractSyntaxTree Conditional(AbstractSyntaxTree e)
        {
            var t = _scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.QuestionMark:
                    var then = Conditional();
                    t = _scanner.GetToken();
                    switch (t.TokenType)
                    {
                        case TokenType.Colon:
                            return Conditional(new TernaryNode(e, then, Conditional()));
                        default:
                            throw new ParsingException(_scanner.Position, "Could not find the ':' to terminate the ternary ('?:') statement");
                    }
                default:
                    _scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree NullCoalescing()
        {
            return NullCoalescing(ConditionalOr());
        }
        private AbstractSyntaxTree NullCoalescing(AbstractSyntaxTree e)
        {
            var t = _scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.DoubleQuestionMark:
                    return NullCoalescing(new NullCoalescingNode(e, ConditionalOr()));
                default:
                    _scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree ConditionalOr()
        {
            return ConditionalOr(ConditionalAnd());
        }
        private AbstractSyntaxTree ConditionalOr(AbstractSyntaxTree e)
        {
            var t = _scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.Or:
                    return ConditionalOr(new OrNode(e, ConditionalAnd()));
                default:
                    _scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree ConditionalAnd()
        {
            return ConditionalAnd(Equality());
        }
        private AbstractSyntaxTree ConditionalAnd(AbstractSyntaxTree e)
        {
            var t = _scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.And:
                    return ConditionalAnd(new AndNode(e, Equality()));
                default:
                    _scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree Equality()
        {
            return Equality(Relational());
        }
        private AbstractSyntaxTree Equality(AbstractSyntaxTree e)
        {
            var t = _scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.DoubleEqual:
                    return Equality(new EqualNode(e, Relational()));
                case TokenType.NotEqual:
                    return Equality(new NotEqualNode(e, Relational()));
                default:
                    _scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree Relational()
        {
            return Relational(Additive());
        }
        private AbstractSyntaxTree Relational(AbstractSyntaxTree e)
        {
            var t = _scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.LessThan:
                    return Relational(new LessThanNode(e, Additive()));
                case TokenType.LessThanEqual:
                    return Relational(new LessThanEqualNode(e, Additive()));
                case TokenType.GreaterThan:
                    return Relational(new GreaterThanNode(e, Additive()));
                case TokenType.GreaterThanEqual:
                    return Relational(new GreaterThanEqualNode(e, Additive()));
                default:
                    _scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree Additive()
        {
            return Additive(Multiplicative());
        }
        private AbstractSyntaxTree Additive(AbstractSyntaxTree e)
        {
            var t = _scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.Plus:
                    return Additive(new AddNode(e, Multiplicative()));
                case TokenType.Minus:
                    return Additive(new SubtractNode(e, Multiplicative()));
                default:
                    _scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree Multiplicative()
        {
            return Multiplicative(Exponent());
        }
        private AbstractSyntaxTree Multiplicative(AbstractSyntaxTree e)
        {
            var t = _scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.Modulo:
                    return Multiplicative(new ModuloNode(e, Exponent()));
                case TokenType.Times:
                    return Multiplicative(new MultiplyNode(e, Exponent()));
                case TokenType.Divide:
                    return Multiplicative(new DivideNode(e, Exponent()));
                case TokenType.X:
                case TokenType.Y:
                case TokenType.Z:
                case TokenType.Lexical:
                case TokenType.LBracket:
                case TokenType.LParen:
                    _scanner.PutBackToken();
                    return Multiplicative(new MultiplyNode(e, Exponent()));
                default:
                    _scanner.PutBackToken();
                    return e;
            }
        }

        private AbstractSyntaxTree Exponent()
        {
            return Exponent(Primary());
        }
        private AbstractSyntaxTree Exponent(AbstractSyntaxTree e)
        {
            var t = _scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.Caret:
                    return Exponent(new ExponentNode(e, Primary()));
                case TokenType.Number:
                    if (e is VariableNode)
                    {
                        if (!(t is LexicalToken lex))
                            throw new ArgumentException(ErrorWrongTokenType(t.TokenType));
                        
                        return Exponent(new ExponentNode(e, new ConstantNumberNode(double.Parse(lex.Lex, NumberStyles.Number, CultureInfo.InvariantCulture))));
                    }
                    else
                    {
                        _scanner.PutBackToken();
                        return e;
                    }
                default:
                    _scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree Primary()
        {
            var t = _scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.Number:
                    if (!(t is LexicalToken numToken))
                        throw new ArgumentException(ErrorWrongTokenType(t.TokenType));
                    return new ConstantNumberNode(double.Parse(numToken.Lex, NumberStyles.Number, CultureInfo.InvariantCulture));
                case TokenType.Plus:
                    return Primary();
                case TokenType.Minus:
                    return new NegativeNode(Primary());
                case TokenType.Not:
                    return new NotNode(Primary());
                case TokenType.X:
                    return new VariableNode(0);
                case TokenType.Y:
                    return new VariableNode(1);
                case TokenType.Z:
                    return new VariableNode(2);
                case TokenType.String:
                    if (!(t is LexicalToken strToken))
                        throw new ArgumentException(ErrorWrongTokenType(t.TokenType));
                    return new StringNode(strToken.Lex);
                case TokenType.InterpolatedString:
                    if (!(t is InterpolatedStringToken token))
                        throw new ArgumentException(ErrorWrongTokenType(t.TokenType));
                    return new FormatFunction { FunctionName = "Format", Arguments = new AbstractSyntaxTree[] { new StringNode(token.Lex) }.Concat(token.Arguments).ToArray() };

                case TokenType.Lexical:
                    if (!(t is LexicalToken lexToken))
                        throw new ArgumentException(ErrorWrongTokenType(t.TokenType));
                    var lex = lexToken.Lex;
                    switch (lex.ToLower())
                    {
                        case "null":
                            return new NullNode();
                        case "pi":
                            return new ConstantNumberNode(Math.PI);
                        case "e":
                            return new ConstantNumberNode(Math.E);
                        case "true":
                            return new ValueNode(true);
                        case "false":
                            return new ValueNode(false);
                        default:
                            if (_customFunctions.TryGetFunction(lex, out var function))
                            {
                                if (function is ZeroArgFunction func0)
                                {
                                    string ex() => $"{lex} is a function that takes zero arguments. You must call it like this: \"{lex}()\"";

                                    if (_scanner.GetToken().TokenType != TokenType.LParen)
                                        throw new ParsingException(_scanner.Position, ex());
                                    if (_scanner.GetToken().TokenType != TokenType.RParen)
                                        throw new ParsingException(_scanner.Position, ex());

                                    return func0;
                                }
                                else if (function is OneArgFunction func1)
                                {
                                    // Create a formula1.
                                    string ex () => $"{lex} is a formula that takes one argument. You must specify the arguments like this: \"{lex}(3)\"";

                                    if (_scanner.GetToken().TokenType != TokenType.LParen)
                                        throw new ParsingException(_scanner.Position, ex());

                                    try
                                    {
                                        func1.Argument = Conditional();
                                    }
                                    catch (Exception e)
                                    {
                                        throw new ParsingException(_scanner.Position, ex(), new Exception(ex(), e));
                                    }

                                    if (_scanner.GetToken().TokenType != TokenType.RParen)
                                        throw new ParsingException(_scanner.Position, ex());

                                    return func1;
                                }
                                if (function is TwoArgFunction func2)
                                {
                                    // Create a formula2.
                                    string ex() => $"{lex} is a formula that takes two arguments. You must specify the arguments like this: \"{lex}(3;2)\"";

                                    if (_scanner.GetToken().TokenType != TokenType.LParen)
                                        throw new ParsingException(_scanner.Position, ex());

                                    try
                                    {
                                        func2.Argument1 = Conditional();
                                    }
                                    catch (Exception inner)
                                    {
                                        throw new ParsingException(_scanner.Position, ex(), inner);
                                    }

                                    if (_scanner.GetToken().TokenType == TokenType.Semicolon)
                                    {
                                        try
                                        {
                                            func2.Argument2 = Conditional();
                                        }
                                        catch (Exception inner)
                                        {
                                            throw new ParsingException(_scanner.Position, ex(), inner);
                                        }

                                        if (_scanner.GetToken().TokenType == TokenType.RParen)
                                        {
                                            return func2;
                                        }
                                    }
                                    throw new ParsingException(_scanner.Position, ex());
                                }
                                else if (function is ArbitraryArgFunction funcN)
                                {
                                    if (_scanner.GetToken().TokenType != TokenType.LParen)
                                        throw new ParsingException(_scanner.Position, $"You must specify arguments for {lex}. Those arguments must be enclosed in parentheses.");

                                    var trees = new List<AbstractSyntaxTree>();

                                    if (_scanner.GetToken().TokenType == TokenType.RParen)
                                    {
                                        funcN.Arguments = new AbstractSyntaxTree[0];
                                        return funcN;
                                    }
                                    else
                                    {
                                        _scanner.PutBackToken();
                                    }

                                    while (true)
                                    {
                                        try
                                        {
                                            trees.Add(Conditional());
                                        }
                                        catch (Exception e)
                                        {
                                            throw new ParsingException(_scanner.Position, $"Error parsing arguments for {lex}.", e);
                                        }

                                        var type = _scanner.GetToken().TokenType;
                                        switch (type)
                                        {
                                            case TokenType.RParen:
                                                funcN.Arguments = trees.ToArray();
                                                return funcN;
                                            case TokenType.Semicolon:
                                                break;
                                            default:
                                                throw new ParsingException(_scanner.Position, $"Error parsing arguments for {lex}. Invalid character: {type}. Expected either a comma, semicolon, or right parenthesis.");
                                        }
                                    }
                                }
                                else
                                {
                                    throw new ParsingException(_scanner.Position, $"{lex} is not a valid function. It must inherit from either {nameof(ZeroArgFunction)}, {nameof(OneArgFunction)}, {nameof(TwoArgFunction)}, or {nameof(ArbitraryArgFunction)}.");
                                }
                            }
                            else
                            {
                                string err;
                                var caseInsensitiveMatches = _customFunctions.Select(x => x.Name).Where(x => x?.ToLower() == lex.ToLower()).ToList();

                                switch (caseInsensitiveMatches.Count)
                                {
                                    case 0:
                                        err = $"{lex} is an invalid function name.";
                                        break;
                                    case 1:
                                        err = $"Functions are case-sensitive. \"{lex}\" is an invalid function name. Did you mean to call the function \"{caseInsensitiveMatches[0]}\"?";
                                        break;
                                    default:
                                        err = $"Functions are case-sensitive. \"{lex}\" is an invalid function name. Did you mean to call one of the following functions? {string.Join(", ", caseInsensitiveMatches.Select(x => $"\"{x}\"").MyToArray())}";
                                        break;
                                }

                                throw new ParsingException(_scanner.Position, err, new NotSupportedException(err));
                            }
                    }
                case TokenType.LBracket:
                    t = _scanner.GetToken();
                    var exc = new Exception("Variable accessors should come in the form [i], where i is an integer.");
                    int i;
                    if (t is LexicalToken)
                    {
                        if (int.TryParse((t as LexicalToken).Lex, out i))
                        {
                            try
                            {
                                return new VariableNode(i);
                            }
                            finally
                            {
                                if (_scanner.GetToken().TokenType != TokenType.RBracket)
                                    throw new ParsingException(_scanner.Position, exc.Message, exc);
                            }
                        }
                    }
                    throw exc;
                case TokenType.LParen:
                    var cond = Conditional();
                    if (_scanner.GetToken().TokenType != TokenType.RParen)
                        throw new ParsingException(_scanner.Position, "Mismatching parentheses");

                    return cond;
                default:
                    throw new ParsingException(_scanner.Position, "Invalid conversion string.");
            }
        }

        public void Dispose()
        {
            _scanner.Dispose();
        }
    }
}
