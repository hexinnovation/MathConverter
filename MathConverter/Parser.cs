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
                var result = Expression();
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
        private AbstractSyntaxTree Expression()
        {
            return Expression(Term());
        }
        private AbstractSyntaxTree Expression(AbstractSyntaxTree e)
        {
            var t = scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.Plus:
                    return Expression(new AddNode(e, Term()));
                case TokenType.Minus:
                    return Expression(new SubtractNode(e, Term()));
                default:
                    scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree Term()
        {
            return Term(Secondary());
        }
        private AbstractSyntaxTree Term(AbstractSyntaxTree e)
        {
            var t = scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.Times:
                    return Term(new MultiplyNode(e, Secondary()));
                case TokenType.Divide:
                    return Term(new DivideNode(e, Secondary()));
                case TokenType.X:
                case TokenType.Y:
                case TokenType.Z:
                case TokenType.Lexical:
                //case TokenType.Number: // WHAT IS THIS?
                case TokenType.LBracket:
                case TokenType.LParen:
                    scanner.PutBackToken();
                    return Term(new MultiplyNode(e, Secondary()));
                default:
                    scanner.PutBackToken();
                    return e;
            }
        }
        private AbstractSyntaxTree Secondary()
        {
            return RestSecondary(Primary());
        }
        private AbstractSyntaxTree RestSecondary(AbstractSyntaxTree e)
        {
            var t = scanner.GetToken();

            switch (t.TokenType)
            {
                case TokenType.Caret:
                    return RestSecondary(new ExponentNode(e, Primary()));
                case TokenType.Number:
                    if (e is VariableNode || e is ConstantNumberNode)
                    {
                        return RestSecondary(new ExponentNode(e, new ValueNode((t as LexicalToken).Lex)));
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
                    return new ValueNode((t as LexicalToken).Lex);
                case TokenType.Minus:
                    return new NegativeNode(Primary());
                case TokenType.X:
                    return new VariableNode(0);
                case TokenType.Y:
                    return new VariableNode(1);
                case TokenType.Z:
                    return new VariableNode(2);
                case TokenType.Lexical:
                    var lex = (t as LexicalToken).Lex;
                    Func<double, double> formula1 = null;
                    Func<double, double, double> formula2 = null;
                    Func<IEnumerable<object>, object> formulaN = null;
                    switch (lex)
                    {
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
                            formula1 = Math.Acos;
                            break;
                        case "asin":
                            formula1 = Math.Asin;
                            break;
                        case "atan":
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
                        case "atan2":
                            formula2 = Math.Atan2;
                            break;
                        case "log":
                            formula2 = Math.Log;
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
                        case "pi":
                            return new ConstantNumberNode(Math.PI);
                        case "e":
                            return new ConstantNumberNode(Math.E);
                        default:
                            var err = lex + " is an invalid formula name";
                            throw new ParsingException(scanner.Position, err, new NotSupportedException(err));
                    }

                    if (formula1 != null)
                    {
                        // Create a formula1.
                        var ex = lex + " is a formula that takes one argument.  You must specify the arguments like this: \"" + lex + "(3)\"";

                        if (scanner.GetToken().TokenType != TokenType.LParen)
                            throw new ParsingException(scanner.Position, ex);

                        AbstractSyntaxTree arg;

                        try
                        {
                            arg = Expression();
                        }
                        catch (Exception e)
                        {
                            throw new ParsingException(scanner.Position, ex, new Exception(ex, e));
                        }

                        if (scanner.GetToken().TokenType != TokenType.RParen)
                            throw new ParsingException(scanner.Position, ex);

                        return new FormulaNode1(formula1, arg);
                    }
                    else if (formula2 != null)
                    {
                        // Create a formula2.
                        var ex = lex + " is a formula that takes two arguments.  You must specify the arguments like this: \"" + lex + "(3;2)\"";

                        if (scanner.GetToken().TokenType != TokenType.LParen)
                            throw new Exception(ex);

                        AbstractSyntaxTree arg1, arg2;

                        try
                        {
                            arg1 = Expression();
                        }
                        catch (Exception)
                        {
                            throw new ParsingException(scanner.Position, ex);
                        }

                        if (scanner.GetToken().TokenType != TokenType.Semicolon)
                            throw new ParsingException(scanner.Position, ex);

                        try
                        {
                            arg2 = Expression();
                        }
                        catch (Exception)
                        {
                            throw new ParsingException(scanner.Position, ex);
                        }

                        if (scanner.GetToken().TokenType != TokenType.RParen)
                            throw new ParsingException(scanner.Position, ex);

                        return new FormulaNode2(formula2, arg1, arg2);
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
                                trees.Add(Expression());
                            }
                            catch (Exception e)
                            {
                                throw new ParsingException(scanner.Position, "Error parsing arguments for " + lex + ".", e);
                            }

                            var type = scanner.GetToken().TokenType;
                            switch (type)
                            {
                                case TokenType.Semicolon:
                                    // Parse the next argument
                                    break;
                                case TokenType.RParen:
                                    goto end;
                                default:
                                    throw new ParsingException(scanner.Position, "Error parsing arguments for " + lex + ". Invalid character: " + type + ". Expected either a comma, semicolon, or right parenthesis.");
                            }
                        }
                    end:
                        return new FormulaNodeN(formulaN, trees);
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
                    try
                    {
                        return Expression();
                    }
                    finally
                    {
                        if (scanner.GetToken().TokenType != TokenType.RParen)
                            throw new ParsingException(scanner.Position, "Mismatching parentheses");
                    }
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
