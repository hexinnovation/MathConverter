using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexInnovation
{
    class Parser : IDisposable
    {
        public static AbstractSyntaxTree[] Parse(string expression)
        {
            using (var p = new Parser(expression))
            {
                return p.ConverterParameter().ToArray();
            }
        }


        private Scanner scanner;
        private Parser(string expression)
        {
            scanner = new Scanner(expression);
        }
        private IEnumerable<AbstractSyntaxTree> ConverterParameter()
        {
            while (true)
            {
                var result = Conditional();
                var t = scanner.GetToken();

                switch (t.TokenType)
                {
                    case TokenType.EOF:
                        yield return result;
                        yield break;
                    case TokenType.Semicolon:
                        yield return result;
                        break;
                    default:
                        throw new ParsingException(scanner.Position, "The conversion parameter could not be parsed to a valid string.");
                }
            }
        }
        private AbstractSyntaxTree Conditional()
        {
            return Conditional(ConditionalOr());
        }
        private AbstractSyntaxTree Conditional(AbstractSyntaxTree e)
        {
            var t = scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.QuestionMark:
                    var then = ConditionalOr();
                    t = scanner.GetToken();
                    switch (t.TokenType)
                    {
                        case TokenType.Colon:
                            return Conditional(new TernaryNode(e, then, ConditionalOr()));
                        default:
                            throw new ParsingException(scanner.Position, "Could not find the ':' to terminate the ternary ('?:') statement");
                    }
                default:
                    scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree ConditionalOr()
        {
            return ConditionalOr(ConditionalAnd());
        }
        private AbstractSyntaxTree ConditionalOr(AbstractSyntaxTree e)
        {
            var t = scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.Or:
                    return ConditionalOr(new OrNode(e, ConditionalAnd()));
                default:
                    scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree ConditionalAnd()
        {
            return ConditionalAnd(Equality());
        }
        private AbstractSyntaxTree ConditionalAnd(AbstractSyntaxTree e)
        {
            var t = scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.And:
                    return ConditionalAnd(new AndNode(e, Equality()));
                default:
                    scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree Equality()
        {
            return Equality(Relational());
        }
        private AbstractSyntaxTree Equality(AbstractSyntaxTree e)
        {
            var t = scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.DoubleEqual:
                    return Equality(new EqualNode(e, Relational()));
                case TokenType.NotEqual:
                    return Equality(new NotEqualNode(e, Relational()));
                default:
                    scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree Relational()
        {
            return Relational(Additive());
        }
        private AbstractSyntaxTree Relational(AbstractSyntaxTree e)
        {
            var t = scanner.GetToken();

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
                    scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree Additive()
        {
            return Additive(Multiplicative());
        }
        private AbstractSyntaxTree Additive(AbstractSyntaxTree e)
        {
            var t = scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.Plus:
                    return Additive(new AddNode(e, Multiplicative()));
                case TokenType.Minus:
                    return Additive(new SubtractNode(e, Multiplicative()));
                default:
                    scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree Multiplicative()
        {
            return Multiplicative(Exponent());
        }
        private AbstractSyntaxTree Multiplicative(AbstractSyntaxTree e)
        {
            var t = scanner.GetToken();

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
                    scanner.PutBackToken();
                    return Multiplicative(new MultiplyNode(e, Exponent()));
                default:
                    scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree Exponent()
        {
            return Exponent(Primary());
        }
        private AbstractSyntaxTree Exponent(AbstractSyntaxTree e)
        {
            var t = scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.Caret:
                    return Exponent(new ExponentNode(e, Primary()));
                case TokenType.Number:
                    if (e is VariableNode)
                    {
                        return Exponent(new ExponentNode(e, new ConstantNumberNode(double.Parse((t as LexicalToken).Lex))));
                    }
                    else
                    {
                        scanner.PutBackToken();
                        return e;
                    }
                default:
                    scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree Primary()
        {
            var t = scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.Number:
                    return new ConstantNumberNode(double.Parse((t as LexicalToken).Lex));
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
                    return new StringNode((t as LexicalToken).Lex);
                case TokenType.Lexical:
                    var lex = (t as LexicalToken).Lex;
                    Func<object> formula0 = null;
                    Func<double, double> formula1 = null;
                    Func<object, object, object> formula2 = null;
                    Func<IEnumerable<object>, object> formulaN = null;
                    switch (lex)
                    {
                        case "null":
                            return new NullNode();
                        case "NOW":
                        case "Now":
                        case "now":
                            formula0 = () => DateTime.Now;
                            break;
                        case "COS":
                        case "Cos":
                        case "cos":
                            formula1 = Math.Cos;
                            break;
                        case "SIN":
                        case "Sin":
                        case "sin":
                            formula1 = Math.Sin;
                            break;
                        case "TAN":
                        case "Tan":
                        case "tan":
                            formula1 = Math.Tan;
                            break;
                        case "ABS":
                        case "Abs":
                        case "abs":
                            formula1 = Math.Abs;
                            break;
                        case "ACOS":
                        case "Acos":
                        case "acos":
                            formula1 = Math.Acos;
                            break;
                        case "ASIN":
                        case "Asin":
                        case "asin":
                            formula1 = Math.Asin;
                            break;
                        case "ATAN":
                        case "Atan":
                        case "atan":
                            formula1 = Math.Atan;
                            break;
                        case "CEIL":
                        case "Ceil":
                        case "ceil":
                        case "CEILING":
                        case "Ceiling":
                        case "ceiling":
                            formula1 = Math.Ceiling;
                            break;
                        case "FLOOR":
                        case "Floor":
                        case "floor":
                            formula1 = Math.Floor;
                            break;
                        case "SQRT":
                        case "Sqrt":
                        case "sqrt":
                            formula1 = Math.Sqrt;
                            break;
                        case "DEGREES":
                        case "Degrees":
                        case "degrees":
                        case "DEG":
                        case "Deg":
                        case "deg":
                            formula1 = x => x / Math.PI * 180;
                            break;
                        case "RADIANS":
                        case "Radians":
                        case "radians":
                        case "RAD":
                        case "Rad":
                        case "rad":
                            formula1 = x => x / 180 * Math.PI;
                            break;
                        case "ROUND":
                        case "Round":
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
                                        throw new Exception(string.Format("Error calling Math.Round({0}, {1}):\r\n{1} is not an integer.", a, y));
                                }
                                else
                                {
                                    return null;
                                }
                            };
                            break;
                        case "ATAN2":
                        case "Atan2":
                        case "atan2":
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
                        case "LOG":
                        case "Log":
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
                        case "ISNULL":
                        case "Isnull":
                        case "isnull":
                        case "IFNULL":
                        case "Ifnull":
                        case "ifnull":
                            formula2 = (x, y) => ReferenceEquals(x, null) ? y : x;
                            break;
                        case "AND":
                        case "And":
                        case "and":
                            formulaN = FormulaNodeN.And;
                            break;
                        case "NOR":
                        case "Nor":
                        case "nor":
                            formulaN = FormulaNodeN.Nor;
                            break;
                        case "OR":
                        case "Or":
                        case "or":
                            formulaN = FormulaNodeN.Or;
                            break;
                        case "MAX":
                        case "Max":
                        case "max":
                            formulaN = FormulaNodeN.Max;
                            break;
                        case "MIN":
                        case "Min":
                        case "min":
                            formulaN = FormulaNodeN.Min;
                            break;
                        case "AVG":
                        case "Avg":
                        case "avg":
                        case "AVERAGE":
                        case "Average":
                        case "average":
                            formulaN = FormulaNodeN.Average;
                            break;
                        case "PI":
                        case "pi":
                            return new ConstantNumberNode(Math.PI);
                        case "E":
                        case "e":
                            return new ConstantNumberNode(Math.E);
                        case "FORMAT":
                        case "Format":
                        case "format":
                            formulaN = FormulaNodeN.Format;
                            break;
                        default:
                            var err = lex + " is an invalid formula name";
                            throw new ParsingException(scanner.Position, err, new NotSupportedException(err));
                    }

                    if (formula0 != null)
                    {
                        var ex = lex + " is a formula that takes zero arguments. You must call it like this: \"" + lex + "()\"";

                        if (scanner.GetToken().TokenType != TokenType.LParen)
                            throw new ParsingException(scanner.Position, ex);
                        if (scanner.GetToken().TokenType != TokenType.RParen)
                            throw new ParsingException(scanner.Position, ex);

                        return new FormulaNode0(lex, formula0);
                    }
                    else if (formula1 != null)
                    {
                        // Create a formula1.
                        var ex = lex + " is a formula that takes one argument.  You must specify the arguments like this: \"" + lex + "(3)\"";

                        if (scanner.GetToken().TokenType != TokenType.LParen)
                            throw new ParsingException(scanner.Position, ex);

                        AbstractSyntaxTree arg;

                        try
                        {
                            arg = Conditional();
                        }
                        catch (Exception e)
                        {
                            throw new ParsingException(scanner.Position, ex, new Exception(ex, e));
                        }

                        if (scanner.GetToken().TokenType != TokenType.RParen)
                            throw new ParsingException(scanner.Position, ex);

                        return new FormulaNode1(lex, formula1, arg);
                    }
                    else if (formula2 != null)
                    {
                        // Create a formula2.
                        var ex = lex + " is a formula that takes two arguments.  You must specify the arguments like this: \"" + lex + "(3;2)\"";
                        if (lex == "round")
                            ex = "round is a formula that takes one or two argments. You must specify the argument(s) like this: round(4.693) or round(4.693;2)";

                        if (scanner.GetToken().TokenType != TokenType.LParen)
                            throw new ParsingException(scanner.Position, ex);

                        AbstractSyntaxTree arg1, arg2;

                        try
                        {
                            arg1 = Conditional();
                        }
                        catch (Exception inner)
                        {
                            throw new ParsingException(scanner.Position, ex, inner);
                        }

                        switch (scanner.GetToken().TokenType)
                        {
                            case TokenType.Semicolon:
                                try
                                {
                                    arg2 = Conditional();
                                }
                                catch (Exception inner)
                                {
                                    throw new ParsingException(scanner.Position, ex, inner);
                                }

                                if (scanner.GetToken().TokenType == TokenType.RParen)
                                {
                                    return new FormulaNode2(lex, formula2, arg1, arg2);
                                }

                                break;
                            case TokenType.RParen:
                                if (lex == "round")
                                {
                                    return new FormulaNode2(lex, formula2, arg1, new ConstantNumberNode(0));
                                }
                                break;
                        }
                        throw new ParsingException(scanner.Position, ex);
                    }
                    else
                    {
                        // Create a formulaN.
                        if (scanner.GetToken().TokenType != TokenType.LParen)
                            throw new ParsingException(scanner.Position, "You must specify arguments for " + lex + ".  Those arguments must be enclosed in parentheses.");

                        var trees = new List<AbstractSyntaxTree>();

                        if (scanner.GetToken().TokenType == TokenType.RParen)
                        {
                            throw new ParsingException(scanner.Position, "You must specify at least one argument for " + lex + ".");
                        }
                        else
                        {
                            scanner.PutBackToken();
                        }

                        while (true)
                        {
                            try
                            {
                                trees.Add(Conditional());
                            }
                            catch (Exception e)
                            {
                                throw new ParsingException(scanner.Position, "Error parsing arguments for " + lex + ".", e);
                            }

                            var type = scanner.GetToken().TokenType;
                            switch (type)
                            {
                                case TokenType.RParen:
                                    return new FormulaNodeN(lex, formulaN, trees);
                                case TokenType.Semicolon:
                                    break;
                                default:
                                    throw new ParsingException(scanner.Position, "Error parsing arguments for " + lex + ". Invalid character: " + type + ". Expected either a comma, semicolon, or right parenthesis.");
                            }
                        }
                    }
                case TokenType.LBracket:
                    t = scanner.GetToken();
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
                                if (scanner.GetToken().TokenType != TokenType.RBracket)
                                    throw new ParsingException(scanner.Position, exc.Message, exc);
                            }
                        }
                    }
                    throw exc;
                case TokenType.LParen:
                    var cond = Conditional();
                    if (scanner.GetToken().TokenType != TokenType.RParen)
                        throw new ParsingException(scanner.Position, "Mismatching parentheses");

                    return cond;
                default:
                    throw new ParsingException(scanner.Position, "Invalid conversion string.");
            }
        }

        ~Parser()
        {
            Dispose();
        }
        public void Dispose()
        {
            scanner.Dispose();
        }
    }
}
