using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HexInnovation;

internal class Parser : IDisposable
{
    public static AbstractSyntaxTree[] Parse(CustomFunctionCollection customFunctions, string expression)
    {
        using var p = new Parser(customFunctions, expression);

        try
        {
            return [.. p.ConverterParameter()];
        }
        catch (ParsingException e)
        {
            throw new ArgumentException($"Failed to parse the expression:{Environment.NewLine}{expression}{Environment.NewLine}See the inner exception for details.", e);
        }
    }

    private static string ErrorWrongTokenType(TokenType type) => $"MathConverter internal exception: if TokenType is {type}, the token must be a {(type == TokenType.InterpolatedString ? nameof(InterpolatedStringToken) : nameof(LexicalToken))}.";

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

        return _scanner.Peek().TokenType
        switch
        {
            TokenType.Semicolon or TokenType.RCurlyBracket or TokenType.Colon => result,
            _ => throw new ParsingException(_scanner, "Error parsing interpolated string. Could not find closing curly bracket (or a colon, comma, or semicolon) after the argument."),
        };
    }
    private IEnumerable<AbstractSyntaxTree> ConverterParameter()
    {
        while (true)
        {
            var result = Conditional();

            switch (_scanner.GetToken().TokenType)
            {
                case TokenType.EOF:
                    yield return result;
                    yield break;
                case TokenType.Semicolon:
                    yield return result;
                    break;
                default:
                    throw new ParsingException(_scanner, "The conversion parameter could not be parsed to a valid string.");
            }
        }
    }
    private AbstractSyntaxTree Conditional() => Conditional(NullCoalescing());
    private AbstractSyntaxTree Conditional(AbstractSyntaxTree e)
    {
        if (_scanner.GetToken().TokenType is TokenType.QuestionMark)
        {
            var then = Conditional();

            return _scanner.GetToken().TokenType is TokenType.Colon ? Conditional(new TernaryNode(e, then, Conditional())) : throw new ParsingException(_scanner, "Could not find the ':' to terminate the ternary ('?:') statement");
        }

        _scanner.PutBackToken();
        return e;
    }
    private AbstractSyntaxTree NullCoalescing() => NullCoalescing(ConditionalOr());
    private AbstractSyntaxTree NullCoalescing(AbstractSyntaxTree e)
    {
        if (_scanner.GetToken().TokenType is TokenType.DoubleQuestionMark)
            return NullCoalescing(new NullCoalescingNode(e, ConditionalOr()));

        _scanner.PutBackToken();
        return e;
    }
    private AbstractSyntaxTree ConditionalOr() => ConditionalOr(ConditionalAnd());
    private AbstractSyntaxTree ConditionalOr(AbstractSyntaxTree e)
    {
        if (_scanner.GetToken().TokenType is TokenType.Or)
            return ConditionalOr(new OrNode(e, ConditionalAnd()));

        _scanner.PutBackToken();
        return e;
    }
    private AbstractSyntaxTree ConditionalAnd() => ConditionalAnd(Equality());
    private AbstractSyntaxTree ConditionalAnd(AbstractSyntaxTree e)
    {
        if (_scanner.GetToken().TokenType is TokenType.And)
            return ConditionalAnd(new AndNode(e, Equality()));

        _scanner.PutBackToken();
        return e;
    }
    private AbstractSyntaxTree Equality() => Equality(Relational());
    private AbstractSyntaxTree Equality(AbstractSyntaxTree e)
    {
        switch (_scanner.GetToken().TokenType)
        {
            case TokenType.DoubleEqual:
                return Equality(new EqualNode(e, Relational()));
            case TokenType.NotEqual:
                return Equality(new NotEqualNode(e, Relational()));
        }

        _scanner.PutBackToken();
        return e;
    }
    private AbstractSyntaxTree Relational() => Relational(Additive());
    private AbstractSyntaxTree Relational(AbstractSyntaxTree e)
    {
        switch (_scanner.GetToken().TokenType)
        {
            case TokenType.LessThan:
                return Relational(new LessThanNode(e, Additive()));
            case TokenType.LessThanEqual:
                return Relational(new LessThanEqualNode(e, Additive()));
            case TokenType.GreaterThan:
                return Relational(new GreaterThanNode(e, Additive()));
            case TokenType.GreaterThanEqual:
                return Relational(new GreaterThanEqualNode(e, Additive()));
        }

        _scanner.PutBackToken();
        return e;
    }
    private AbstractSyntaxTree Additive() => Additive(Multiplicative());
    private AbstractSyntaxTree Additive(AbstractSyntaxTree e)
    {
        switch (_scanner.GetToken().TokenType)
        {
            case TokenType.Plus:
                return Additive(new AddNode(e, Multiplicative()));
            case TokenType.Minus:
                return Additive(new SubtractNode(e, Multiplicative()));
        }

        _scanner.PutBackToken();
        return e;
    }
    private AbstractSyntaxTree Multiplicative() => Multiplicative(Exponent());
    private AbstractSyntaxTree Multiplicative(AbstractSyntaxTree e)
    {
        switch (_scanner.GetToken().TokenType)
        {
            case TokenType.Modulo:
                return Multiplicative(new ModuloNode(e, Exponent()));
            case TokenType.Times:
                return Multiplicative(new MultiplyNode(e, Exponent()));
            case TokenType.Divide:
                return Multiplicative(new DivideNode(e, Exponent()));
            case TokenType.X or TokenType.Y or TokenType.Z or TokenType.Lexical or TokenType.LBracket or TokenType.LParen:
                _scanner.PutBackToken();
                return Multiplicative(new MultiplyNode(e, Exponent()));
        }

        _scanner.PutBackToken();
        return e;
    }

    private AbstractSyntaxTree Exponent() => Exponent(Primary());
    private AbstractSyntaxTree Exponent(AbstractSyntaxTree e)
    {
        switch (_scanner.GetToken())
        {
            case { TokenType: TokenType.Caret }:
                return Exponent(new ExponentNode(e, Primary()));
            case { TokenType: TokenType.Number } t when e is VariableNode:
                return (t is LexicalToken lex) ?
                    Exponent(new ExponentNode(e, new ValueNode(double.Parse(lex.Lex, NumberStyles.Number, CultureInfo.InvariantCulture)))) :
                    throw new ArgumentException(ErrorWrongTokenType(t.TokenType));
        }

        _scanner.PutBackToken();
        return e;
    }
    private AbstractSyntaxTree Primary()
    {
        switch (_scanner.GetToken())
        {
            case LexicalToken { TokenType: TokenType.Number } number:
                return new ValueNode(double.Parse(number.Lex, NumberStyles.Number, CultureInfo.InvariantCulture));
            case { TokenType: TokenType.Plus }:
                return Primary();
            case { TokenType: TokenType.Minus }:
                return new NegativeNode(Primary());
            case { TokenType: TokenType.Not }:
                return new NotNode(Primary());
            case { TokenType: TokenType.X }:
                return new VariableNode(0);
            case { TokenType: TokenType.Y }:
                return new VariableNode(1);
            case { TokenType: TokenType.Z }:
                return new VariableNode(2);
            case LexicalToken { TokenType: TokenType.String } @string:
                return new StringNode(@string.Lex);
            case InterpolatedStringToken { TokenType: TokenType.InterpolatedString } interpolated:
                return new FormatFunction { FunctionName = "Format", Parameters = [new StringNode(interpolated.Lex), .. interpolated.Arguments] };

            case { TokenType: TokenType.String or TokenType.Number or TokenType.InterpolatedString } t:
                throw new ArgumentException(ErrorWrongTokenType(t.TokenType));

            case { TokenType: TokenType.Lexical } t and not LexicalToken { Lex: { } }:
                throw new ArgumentException(ErrorWrongTokenType(t.TokenType));

            case LexicalToken { TokenType: TokenType.Lexical, Lex: { } lex }:
                switch (lex)
                {
                    case "null":
                        return new NullNode();
                    case "pi":
                        return new ValueNode(Math.PI);
                    case "e":
                        return new ValueNode(Math.E);
                    case "true":
                        return new ValueNode(true);
                    case "false":
                        return new ValueNode(false);
                    default:
                        if (_customFunctions.TryGetFunction(lex, out var function))
                        {
                            if (_scanner.GetToken().TokenType is not TokenType.LParen)
                                throw new ParsingException(_scanner, $"You must specify arguments for {lex} function. Those arguments must be enclosed in parentheses.");

                            function.Parameters = [];

                            if (_scanner.GetToken().TokenType != TokenType.RParen)
                            {
                                _scanner.PutBackToken();
                                while (true)
                                {
                                    try
                                    {
                                        function.Parameters.Add(Conditional());
                                    }
                                    catch (Exception e)
                                    {
                                        throw new ParsingException(_scanner, $"Error parsing arguments for {lex} function.", e);
                                    }

                                    var type = _scanner.GetToken().TokenType;

                                    if (type == TokenType.RParen)
                                        break;

                                    if (type is not TokenType.Semicolon)
                                        throw new ParsingException(_scanner, $"Error parsing arguments for {lex} function. Invalid character: {type}. Expected either a comma, semicolon, or right parenthesis.");
                                }
                            }

                            return function.IsValidNumberOfParameters(function.Parameters.Count) ? function :
                                throw new ParsingException(_scanner, $"The {lex} function cannot accept {function.Parameters.Count} parameter{(function.Parameters.Count == 1 ? "" : "s")}.");
                        }
                        else
                        {
                            var caseInsensitiveMatches = _customFunctions.Select(x => x.Name).Where(x => x?.Equals(lex, StringComparison.OrdinalIgnoreCase) is true).ToList();

                            string err = caseInsensitiveMatches.Count switch
                            {
                                0 => $"{lex} is an invalid function name.",
                                1 => $"Functions are case-sensitive. \"{lex}\" is an invalid function name. Did you mean to call the function \"{caseInsensitiveMatches[0]}\"?",
                                _ => $"Functions are case-sensitive. \"{lex}\" is an invalid function name. Did you mean to call one of the following functions? {string.Join(", ", caseInsensitiveMatches.Select(x => $"\"{x}\""))}",
                            };

                            throw new ParsingException(_scanner, err, new NotSupportedException(err));
                        }
                }
            case { TokenType: TokenType.LBracket }:
                const string message = "Variable accessors should come in the form [i], where i is an integer.";

                if (_scanner.GetToken() is LexicalToken { Lex: { } lex2 } && int.TryParse(lex2, out var i))
                {
                    return _scanner.GetToken().TokenType == TokenType.RBracket ?
                        new VariableNode(i) :
                        throw new ParsingException(_scanner, message);
                }

                throw new ParsingException(_scanner, message);

            case { TokenType: TokenType.LParen }:
                var cond = Conditional();
                if (_scanner.GetToken().TokenType != TokenType.RParen)
                    throw new ParsingException(_scanner, "Mismatching parentheses");

                return cond;
            default:
                throw new ParsingException(_scanner, "Invalid conversion string.");
        }
    }

    public void Dispose() => _scanner.Dispose();
}
