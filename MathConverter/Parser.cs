using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace HexInnovation
{
    class Parser : IDisposable
    {
        public static AbstractSyntaxTree[] Parse(string expression)
        {
            using (var p = new Parser(expression))
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
        private Parser(string expression)
        {
            _scanner = new Scanner(this, expression);
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
                        
                        return Exponent(new ExponentNode(e, new ConstantNumberNode(double.Parse((t as LexicalToken).Lex, NumberStyles.Number, CultureInfo.InvariantCulture))));
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
                    return new FormulaNodeN("Format", FormulaNodeN.Format, new AbstractSyntaxTree[] { new StringNode(token.Lex) }.Union(token.Arguments));

                case TokenType.Lexical:
                    if (!(t is LexicalToken lexToken))
                        throw new ArgumentException(ErrorWrongTokenType(t.TokenType));
                    var lex = lexToken.Lex;
                    Func<object> formula0 = null;
                    Func<double, double> formula1 = null;
                    Func<object, object> formula1_obj = null;
                    Func<object, object, object> formula2 = null;
                    Func<CultureInfo, IEnumerable<object>, object> formulaN = null;
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
                            switch (lex.ToLower())
                            {
                                case "now":
                                    formula0 = () => DateTime.Now;
                                    break;
                                case "cos":
                                    formula1 = Math.Cos;
                                    break;
                                case "sin":
                                    formula1 = Math.Sin;
                                    break;
                                case "tan":
                                    formula1 = Math.Tan;
                                    break;
                                case "abs":
                                    formula1 = Math.Abs;
                                    break;
                                case "acos":
                                case "arccos":
                                    formula1 = Math.Acos;
                                    break;
                                case "asin":
                                case "arcsin":
                                    formula1 = Math.Asin;
                                    break;
                                case "atan":
                                case "arctan":
                                    formula1 = Math.Atan;
                                    break;
                                case "ceil":
                                case "ceiling":
                                    formula1 = Math.Ceiling;
                                    break;
                                case "floor":
                                    formula1 = Math.Floor;
                                    break;
                                case "sqrt":
                                    formula1 = Math.Sqrt;
                                    break;
                                case "degrees":
                                case "deg":
                                    formula1 = x => x / Math.PI * 180;
                                    break;
                                case "radians":
                                case "rad":
                                    formula1 = x => x / 180 * Math.PI;
                                    break;
                                case "tolower":
                                case "lcase":
                                    formula1_obj = x => x == null ? null : $"{x}".ToLowerInvariant();
                                    break;
                                case "toupper":
                                case "ucase":
                                    formula1_obj = x => x == null ? null : $"{x}".ToUpperInvariant();
                                    break;
                                case "startswith":
                                    formula2 = (x, y) => x is string str1 && (y is string || y?.ToString().Length > 0) ? str1.StartsWith($"{y}") : new bool?();
                                    break;
                                case "endswith":
                                    formula2 = (x, y) => x is string str1 && (y is string || y?.ToString().Length > 0) ? str1.EndsWith($"{y}") : new bool?();
                                    break;
                                case "visibleorcollapsed":
                                    formula1_obj = x => x is bool val && val ? Visibility.Visible : Visibility.Collapsed;
                                    break;
                                case "visibleorhidden":
                                    formula1_obj = x => x is bool val && val ? Visibility.Visible : Visibility.Hidden;
                                    break;
                                case "round":
                                    formula2 = (x, y) =>
                                    {
                                        var a = MathConverter.ConvertToDouble(x);
                                        var b = MathConverter.ConvertToDouble(y);
                                        if (a.HasValue && b.HasValue)
                                        {
                                            if (b.Value == (int)b.Value)
                                                return Math.Round(a.Value, (int)b.Value);
                                            else
                                                throw new Exception($"Error calling Math.Round({a}, {y}):\r\n{y} is not an integer.");
                                        }
                                        else
                                        {
                                            return null;
                                        }
                                    };
                                    break;
                                case "atan2":
                                case "arctan2":
                                    formula2 = (x, y) =>
                                    {
                                        var a = MathConverter.ConvertToDouble(x);
                                        var b = MathConverter.ConvertToDouble(y);
                                        if (a.HasValue && b.HasValue)
                                            return Math.Atan2(a.Value, b.Value);
                                        else
                                            return null;
                                    };
                                    break;
                                case "log":
                                    formula2 = (x, y) =>
                                    {
                                        var a = MathConverter.ConvertToDouble(x);
                                        var b = MathConverter.ConvertToDouble(y);
                                        if (a.HasValue && b.HasValue)
                                            return Math.Log(a.Value, b.Value);
                                        else
                                            return null;
                                    };
                                    break;
                                case "isnull":
                                case "ifnull":
                                    formula2 = (x, y) => x ?? y;
                                    break;
                                case "and":
                                    formulaN = FormulaNodeN.And;
                                    break;
                                case "nor":
                                    formulaN = FormulaNodeN.Nor;
                                    break;
                                case "or":
                                    formulaN = FormulaNodeN.Or;
                                    break;
                                case "max":
                                    formulaN = FormulaNodeN.Max;
                                    break;
                                case "min":
                                    formulaN = FormulaNodeN.Min;
                                    break;
                                case "avg":
                                case "average":
                                    formulaN = FormulaNodeN.Average;
                                    break;
                                case "format":
                                    formulaN = FormulaNodeN.Format;
                                    break;
                                case "concat":
                                    formulaN = FormulaNodeN.Concat;
                                    break;
                                case "join":
                                    formulaN = FormulaNodeN.Join;
                                    break;
                                case "contains":
                                    formula2 = (x, y) =>
                                    {
                                        if (x is string str1 && (y is string || $"{y}".Length > 0))
                                        {
                                            return str1.Contains($"{y}");
                                        } 
                                        else if (x is IEnumerable @enum)
                                        {
                                            return @enum.OfType<object>().Contains(y);
                                        }
                                        else
                                        {
                                            return null;
                                        }
                                    };
                                    break;
                                default:
                                    var err = $"{lex} is an invalid formula name";
                                    throw new ParsingException(_scanner.Position, err, new NotSupportedException(err));
                            }
                            break;
                    }

                    if (formula0 != null)
                    {
                        var ex = $"{lex} is a formula that takes zero arguments. You must call it like this: \"{lex}()\"";

                        if (_scanner.GetToken().TokenType != TokenType.LParen)
                            throw new ParsingException(_scanner.Position, ex);
                        if (_scanner.GetToken().TokenType != TokenType.RParen)
                            throw new ParsingException(_scanner.Position, ex);

                        return new FormulaNode0(lex, formula0);
                    }
                    else if (formula1 != null || formula1_obj != null)
                    {
                        // Create a formula1.
                        var ex = $"{lex} is a formula that takes one argument.  You must specify the arguments like this: \"{lex}(3)\"";

                        if (_scanner.GetToken().TokenType != TokenType.LParen)
                            throw new ParsingException(_scanner.Position, ex);

                        AbstractSyntaxTree arg;

                        try
                        {
                            arg = Conditional();
                        }
                        catch (Exception e)
                        {
                            throw new ParsingException(_scanner.Position, ex, new Exception(ex, e));
                        }

                        if (_scanner.GetToken().TokenType != TokenType.RParen)
                            throw new ParsingException(_scanner.Position, ex);

                        if (formula1 != null)
                        {
                            formula1_obj = x =>
                            {
                                var val = MathConverter.ConvertToDouble(x);
                                if (val.HasValue)
                                    return formula1(val.Value);
                                else
                                    return null;
                            };
                        }

                        return new FormulaNode1(lex, formula1_obj, arg);
                    }
                    else if (formula2 != null)
                    {
                        // Create a formula2.
                        var ex = $"{lex} is a formula that takes two argument.  You must specify the arguments like this: \"{lex}(3;2)\"";
                        if (lex.ToLower() == "round")
                            ex = "round is a formula that takes one or two argments. You must specify the argument(s) like this: round(4.693) or round(4.693;2)";

                        if (_scanner.GetToken().TokenType != TokenType.LParen)
                            throw new ParsingException(_scanner.Position, ex);

                        AbstractSyntaxTree arg1, arg2;

                        try
                        {
                            arg1 = Conditional();
                        }
                        catch (Exception inner)
                        {
                            throw new ParsingException(_scanner.Position, ex, inner);
                        }

                        switch (_scanner.GetToken().TokenType)
                        {
                            case TokenType.Semicolon:
                                try
                                {
                                    arg2 = Conditional();
                                }
                                catch (Exception inner)
                                {
                                    throw new ParsingException(_scanner.Position, ex, inner);
                                }

                                if (_scanner.GetToken().TokenType == TokenType.RParen)
                                {
                                    return new FormulaNode2(lex, formula2, arg1, arg2);
                                }

                                break;
                            case TokenType.RParen:
                                if (lex.ToLower() == "round")
                                {
                                    return new FormulaNode2(lex, formula2, arg1, new ConstantNumberNode(0));
                                }
                                break;
                        }
                        throw new ParsingException(_scanner.Position, ex);
                    }
                    else
                    {
                        // Create a formulaN.
                        if (_scanner.GetToken().TokenType != TokenType.LParen)
                            throw new ParsingException(_scanner.Position, $"You must specify arguments for {lex}.  Those arguments must be enclosed in parentheses.");

                        var trees = new List<AbstractSyntaxTree>();

                        if (_scanner.GetToken().TokenType == TokenType.RParen)
                        {
                            throw new ParsingException(_scanner.Position, $"You must specify at least one argument for {lex}.");
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
                                    return new FormulaNodeN(lex, formulaN, trees);
                                case TokenType.Semicolon:
                                    break;
                                default:
                                    throw new ParsingException(_scanner.Position, $"Error parsing arguments for {lex}. Invalid character: {type}. Expected either a comma, semicolon, or right parenthesis.");
                            }
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
