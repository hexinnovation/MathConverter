using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace HexInnovation
{
    public abstract class Operator
    {
        /// <summary>
        /// The binary "^" operator. This operator returns the first operand raised to the power of the second.
        /// If either operand is null, it will return null, and it only supports numeric types.
        /// It is designed explicitly different from the C# "^" (exclusive or) operator.
        /// </summary>
        public static readonly BinaryOperator Exponentiation = OperationType.Exponentiation;

        /// <summary>
        /// The binary "+" operator. This operator adds two operands together.
        /// It is designed to work like the "+" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#addition-operator"/>
        /// </summary>
        public static readonly BinaryOperator Addition = OperationType.Addition;

        /// <summary>
        /// The binary "-" operator. This operator subtracts one operand from another.
        /// It is designed to work like the "-" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#subtraction-operator"/>
        /// </summary>
        public static readonly BinaryOperator Subtraction = OperationType.Subtraction;

        /// <summary>
        /// The binary "*" operator. This operator multiplies one operand by another.
        /// It is designed to work like the "*" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#multiplication-operator"/>
        /// </summary>
        public static readonly BinaryOperator Multiply = OperationType.Multiply;

        /// <summary>
        /// The binary "/" operator. This operator divides one operand by another.
        /// It is designed to work like the "/" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#division-operator"/>
        /// </summary>
        public static readonly BinaryOperator Division = OperationType.Division;

        /// <summary>
        /// The binary "%" operator. This operator divides one operand by another and returns the remainder.
        /// It is designed to work like the "%" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#remainder-operator"/>
        /// </summary>
        public static readonly BinaryOperator Remainder = OperationType.Remainder;

        /// <summary>
        /// The binary "&&" operator. This operator explicitly does not do a bitwise-and, and is only applicable for boolean operands, or types that define a "false" operator and a "&" operator.
        /// If the first operand is false, the second operand will not be evaluated.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#conditional-logical-operators"/>
        /// </summary>
        public static readonly BinaryOperator And = OperationType.And;

        /// <summary>
        /// The binary "||" operator. This operator explicitly does not do a bitwise-and, and is only applicable for boolean operands, or types that define a "true" operator and a "|" operator.
        /// If the first operand is true, the second operand will not be evaluated.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#conditional-logical-operators"/>
        /// </summary>
        public static readonly BinaryOperator Or = OperationType.Or;

        /// <summary>
        /// The binary "??" operator.
        /// It is designed to work like the "??" operator in C#.
        /// It evaluates and returns the first object, unless it is null, in which case it will evaluate and return the second object.
        /// The second expression is only evaluated if the first object is null.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#the-null-coalescing-operator"/>
        /// </summary>
        public static readonly BinaryOperator NullCoalescing = OperationType.NullCoalescing;

        /// <summary>
        /// The binary "!=" operator.
        /// It is designed to work like the "!=" operator in C#, returning true if the two arguments are not equal, or false if they are equal.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#relational-and-type-testing-operators"/>
        /// </summary>
        public static readonly BinaryOperator Inequality = OperationType.Inequality;

        /// <summary>
        /// The binary "==" operator.
        /// It is designed to work like the "==" operator in C#, returning true if the two arguments are equal, or false if not.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#relational-and-type-testing-operators"/>
        /// </summary>
        public static readonly BinaryOperator Equality = OperationType.Equality;

        /// <summary>
        /// The binary "&lt;" operator.
        /// It is designed to work like the "&lt;" operator.
        /// It works on numeric types (after converting the operands to double), and also works on any type that defines a "&lt;" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#relational-and-type-testing-operators"/>
        /// </summary>
        public static readonly BinaryOperator LessThan = OperationType.LessThan;

        /// <summary>
        /// The binary "&lt;=" operator.
        /// It is designed to work like the "&lt;=" operator.
        /// It works on numeric types (after converting the operands to double), and also works on any type that defines a "&lt;=" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#relational-and-type-testing-operators"/>
        /// </summary>
        public static readonly BinaryOperator LessThanOrEqual = OperationType.LessThanOrEqual;

        /// <summary>
        /// The binary ">" operator.
        /// It is designed to work like the ">" operator.
        /// It works on numeric types (after converting the operands to double), and also works on any type that defines a ">" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#relational-and-type-testing-operators"/>
        /// </summary>
        public static readonly BinaryOperator GreaterThan = OperationType.GreaterThan;

        /// <summary>
        /// The binary ">=" operator.
        /// It is designed to work like the ">=" operator.
        /// It works on numeric types (after converting the operands to double), and also works on any type that defines a ">=" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#relational-and-type-testing-operators"/>
        /// </summary>
        public static readonly BinaryOperator GreaterThanOrEqual = OperationType.GreaterThanOrEqual;

        /// <summary>
        /// The unary "!" operator.
        /// It is designed to work like the "!" operator.
        /// It works on boolean arguments, or any type that defines a "!" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#logical-negation-operator" />
        /// </summary>
        public static readonly UnaryOperator LogicalNot = OperationType.LogicalNot;

        /// <summary>
        /// The unary "-" operator.
        /// It is designed to work like the unary "-" operator.
        /// It works on numeric arguments (after converting the operand to double), and also works on any type that defines a "-" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#unary-minus-operator"/>
        /// </summary>
        public static readonly UnaryOperator UnaryNegation = OperationType.UnaryNegation;

        private class OperatorInfo
        {
            public MethodInfo Method { get; set; }
            public ParameterInfo[] Parameters { get; set; }

            public bool IsApplicable(Type[] parameters)
            {
                // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#applicable-function-member

                // This logic is simplified because operators cannot be called with ref/out or optional parameters.
                if (Parameters.Length != parameters.Length)
                    return false;

                // Can we implicitly convert the parameters to the necessary type?
                for (int i = 0; i < Parameters.Length; i++)
                {
                    if (!DoesImplicitConversionExist(parameters[i], Parameters[i].ParameterType, false))
                        return false;
                }

                return true;
            }
            public override string ToString() => $"{Method}";
        }
        [Flags]
        protected enum Operands
        {
            CompletelyCustom = 0,
            Number = 1,
            Boolean = 2,
            String = 4,
            StringObject = 8,
            Object = 16,
        }
        private static IEnumerable<Type> GetTypeAndSubtypes(Type type)
        {
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#base-types

            if (type == null)
            {
                yield break;
            }

            type = Nullable.GetUnderlyingType(type) ?? type;

            yield return type;

            if (type.GetTypeInfo().IsEnum)
            {
                yield return typeof(Enum);
            }

            if (type.GetTypeInfo().IsValueType)
            {
                yield return typeof(ValueType);
                yield return typeof(object);
                yield break;
            }

            if (type.IsArray)
            {
                yield return typeof(Array);
                yield return typeof(object);
                yield break;
            }

            // Not sure what to do about delegate types.

            // Interface and classes...
            while ((type = type.GetTypeInfo().BaseType) != null)
            {
                yield return type;
            }
        }
        private static IEnumerable<Type> GetTypeAndSubtypes(params Type[] parameters)
        {
            return parameters.SelectMany(GetTypeAndSubtypes).Distinct();
        }

        private static List<OperatorInfo> GetPossibleOperators(string operatorName, params Type[] parameterTypes)
        {
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#candidate-user-defined-operators
            return GetTypeAndSubtypes(parameterTypes).SelectMany(type =>
                    type.GetPublicStaticMethods()
                        .Where(method => method.Name == operatorName)
                        .Select(method => new OperatorInfo
                        {
                            Method = method,
                            Parameters = method.GetParameters()
                        })
                        .Where(p => p.IsApplicable(parameterTypes))
                )
                .ToList();
        }
        private static List<OperatorInfo> GetPossibleOperators(string operatorName, params object[] parameters)
        {
            return GetPossibleOperators(operatorName, parameters.Select(p => p?.GetType() ?? typeof(object)).ToArray());
        }

        protected internal static List<MethodInfo> GetImplicitOperatorPath(string operatorName, Type typeFrom, Type typeTo)
        {
            var implicitOperators = GetPossibleOperators(operatorName, typeFrom)
                .ToDictionary(p => p.Method.ReturnType, p => new List<MethodInfo> { p.Method });
            var previousLevelOperators = implicitOperators.Keys.Distinct().ToList();
            var underlyingType = Nullable.GetUnderlyingType(typeTo);

            implicitOperators[typeFrom] = new List<MethodInfo>(0);

            if (previousLevelOperators.Any())
            {
                while (previousLevelOperators.Any())
                {
                    if (implicitOperators.ContainsKey(typeTo))
                    {
                        return implicitOperators[typeTo];
                    }
                    else if (underlyingType != null && implicitOperators.ContainsKey(underlyingType))
                    {
                        return implicitOperators[underlyingType];
                    }

                    var previousLevelOperatorsCopy = previousLevelOperators.ToList();
                    previousLevelOperators.Clear();

                    foreach (var type in previousLevelOperatorsCopy)
                    {
                        var possibleOperators = GetPossibleOperators(operatorName, type).ToList();

                        foreach (var nextLevelOperator in possibleOperators)
                        {
                            if (!implicitOperators.ContainsKey(nextLevelOperator.Method.ReturnType))
                            {
                                implicitOperators[nextLevelOperator.Method.ReturnType] = implicitOperators[type].Concat(new[] { nextLevelOperator.Method }).ToList();
                                previousLevelOperators.Add(nextLevelOperator.Method.ReturnType);
                            }
                        }
                    }
                }


                // Favor a path requiring as few implicit conversions as possible.
                foreach (var conversionPath in implicitOperators.OrderBy(p => p.Value.Count))
                {
                    // Prevent infinite recursion by refusing to implicitly convert again.
                    if (DoesImplicitConversionExist(conversionPath.Key, typeTo, false))
                    {
                        return conversionPath.Value;
                    }
                    else if (underlyingType != null && DoesImplicitConversionExist(conversionPath.Key, underlyingType, false))
                    {
                        return conversionPath.Value;
                    }
                }
            }

            // There's no implicit conversion from typeFrom to typeTo.
            return null;
        }
        protected internal static bool DoesImplicitConversionExist(Type typeFrom, Type typeTo, bool allowImplicitOperator)
        {
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/conversions#implicit-numeric-conversions
            if (typeFrom == null)
            {
                // We're not worrying about nullable types.
                return true;
            }
            // For numeric types, we allow more conversion types. We implicitly convert from unsigned values to signed and from any type to itself.
            // Essentially, this method is here to see if we can call DoImplicitConversion to safely convert numeric types up to bigger types.
            else if (typeFrom == typeof(sbyte) || typeFrom == typeof(byte))
            {
                return typeTo == typeof(sbyte) || typeTo == typeof(short) || typeTo == typeof(ushort) ||
                       typeTo == typeof(int) || typeTo == typeof(uint) || typeTo == typeof(long) ||
                       typeTo == typeof(ulong) || typeTo == typeof(float) || typeTo == typeof(double) ||
                       typeTo == typeof(decimal);
            }
            else if (typeFrom == typeof(short) || typeFrom == typeof(ushort))
            {
                return typeTo == typeof(ushort) || typeTo == typeof(short) || typeTo == typeof(int) ||
                       typeTo == typeof(uint) || typeTo == typeof(long) || typeTo == typeof(ulong) ||
                       typeTo == typeof(float) || typeTo == typeof(double) || typeTo == typeof(decimal);
            }
            else if (typeFrom == typeof(int) || typeFrom == typeof(uint))
            {
                return typeTo == typeof(uint) || typeTo == typeof(int) || typeTo == typeof(long) ||
                       typeTo == typeof(ulong) || typeTo == typeof(float) || typeTo == typeof(double) ||
                       typeTo == typeof(decimal);
            }
            else if (typeFrom == typeof(long) || typeFrom == typeof(ulong))
            {
                return typeTo == typeof(long) || typeTo == typeof(ulong) || typeTo == typeof(float) ||
                       typeTo == typeof(double) || typeTo == typeof(decimal);
            }
            else if (typeFrom == typeof(char))
            {
                return typeTo == typeof(char) || typeTo == typeof(ushort) || typeTo == typeof(int) ||
                       typeTo == typeof(uint) || typeTo == typeof(long) || typeTo == typeof(ulong) ||
                       typeTo == typeof(float) || typeTo == typeof(double) || typeTo == typeof(decimal);
            }
            else if (typeFrom == typeof(float))
            {
                return typeTo == typeof(float) || typeTo == typeof(double);
            }
            else if (typeFrom == typeof(decimal))
            {
                return typeTo == typeof(decimal) || typeTo == typeof(double);
            }
            else if (typeFrom == typeof(double))
            {
                return typeTo == typeof(double);
            }
            // Ignoring Implicit enumeration conversions section...
            // Ignoring Implicit interpolated string conversions...
            // Ignoring Implicit nullable conversions...

            // Reference conversions!
            if (typeTo == typeof(object))
                return true;

            if (GetTypeAndSubtypes(typeFrom).Contains(typeTo))
                return true;

            if (typeTo.GetTypeInfo().IsInterface && typeFrom.GetInterfaces().Contains(typeTo))
                return true;

            if (typeFrom.IsArray && typeTo.IsArray && typeFrom.GetArrayRank() == typeTo.GetArrayRank())
            {
                return !typeFrom.GetTypeInfo().IsValueType && !typeTo.GetTypeInfo().IsValueType && DoesImplicitConversionExist(typeFrom.GetElementType(), typeTo.GetElementType(), true);
            }
            if (typeFrom.IsArray && GetTypeAndSubtypes(typeof(Array)).Contains(typeTo))
            {
                return true;
            }
            if (typeFrom.IsArray && typeFrom.GetArrayRank() == 1 && GetTypeAndSubtypes(typeof(IList<>)).Contains(typeTo))
            {
                return DoesImplicitConversionExist(typeFrom.GetElementType(), typeTo.GetGenericArguments()[0], true);
            }
            // MathConverter doesn't support delegates.

            if (allowImplicitOperator && GetImplicitOperatorPath("op_Implicit", typeFrom, typeTo) != null)
            {
                return true;
            }

            // Check To/False operators.
            if (typeTo == typeof(bool) || typeTo == typeof(bool?))
            {
                var operators = GetImplicitOperatorPath("op_True", typeFrom, typeTo);
                if (operators != null)
                {
                    return true;
                }
            }

            // This is probably good enough???
            return false;
        }
        protected internal static object DoImplicitConversion(object from, Type typeTo)
        {
            var typeToIsValueType = typeTo.GetTypeInfo().IsValueType;

            // If we're trying to convert null to a nullable type, let's just return null.
            if (from == null && (!typeToIsValueType || Nullable.GetUnderlyingType(typeTo)?.GetTypeInfo().IsValueType == true))
            {
                if (typeToIsValueType)
                {
                    // Nullable<T>.
                    return Activator.CreateInstance(typeTo);
                }
                else
                {
                    return null;
                }
            }

            var structTypeTo = Nullable.GetUnderlyingType(typeTo);
            if (structTypeTo != null && from != null && from.GetType() == structTypeTo)
            {
                return from;
            }

            // Check implicit casts.
            var implicitCasts = GetImplicitOperatorPath("op_Implicit", from?.GetType() ?? typeof(object), typeTo);

            if (implicitCasts?.Any() == true)
            {
                implicitCasts.ForEach(cast => from = cast.Invoke(null, new[] { from }));

                // After we finish the implicit operator(s), we may need to do another implicit conversion.
                return DoImplicitConversion(from, typeTo);
            }

            // Check true operator
            if (typeTo == typeof(bool) || typeTo == typeof(bool?))
            {
                var trueOperator = GetImplicitOperatorPath("op_True", from?.GetType() ?? typeof(object), typeTo);
                if (trueOperator?.Any() == true)
                {
                    trueOperator.ForEach(oper => from = oper.Invoke(null, new[] { from }));

                    return from;
                }
            }

            // We might have to convert to a non-nullable type.
            if (structTypeTo != null && from != null)
            {
                // If we're converting a char, we'll always convert to integer first.
                if (from is char)
                    from = Convert.ToInt32(from);

                return Convert.ChangeType(from, structTypeTo);
            }

            return Convert.ChangeType(from, typeTo);
        }

        protected internal static bool? TryConvertToBool(object value)
        {
            if (value is bool b)
            {
                return b;
            }
            else if (value == null)
            {
                return null;
            }

            if (DoesImplicitConversionExist(value.GetType(), typeof(bool?), true))
            {
                return (bool)DoImplicitConversion(value, typeof(bool?));
            }

            return null;
        }
        protected static InvalidOperationException InvalidOperator(string operatorSymbols, params object[] arguments)
        {
            var argTypes = arguments.Select(x => x == null ? "null" : $"'{x.GetType().FullName}'").ToList();

            return new InvalidOperationException($"Cannot apply operator '{operatorSymbols}' to operand{(arguments.Length == 1 ? "" : "s")} of type {string.Join(" ", argTypes.Take(argTypes.Count - 1).MyToArray())}{(argTypes.Count == 1 ? "" : " and ")}{argTypes.Last()}");
        }
        protected MethodInfo GetUserDefinedOperator(out bool convertToDoubles, params object[] parameters)
        {
            if ((SupportedOperands & Operands.Number) == Operands.Number && parameters.All(p => DoesImplicitConversionExist(p?.GetType(), typeof(double), false)))
            // Without implicit operators, the only types that will convert to double are built-in numeric structs (char, byte, sbyte, int, long, float, decimal, etc.)
            {
                // If we're trying to compare int < decimal, we might run in to problems where it picks the wrong operator.
                // Here, we'll force any operand resolutions to be in the double class.
                parameters = parameters.Select(p => DoImplicitConversion(p, typeof(double?))).ToArray();
                convertToDoubles = true;
            }
            else
            {
                convertToDoubles = false;
            }

            var candidateUserDefinedOperators = GetPossibleOperators(OperatorName, parameters);
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#binary-operator-overload-resolution

            switch (candidateUserDefinedOperators.Count)
            {
                case 0:
                    convertToDoubles = false;
                    return null;
                case 1:
                    return candidateUserDefinedOperators[0].Method;
                default:
                    // We need to figure out which operator is the most specific!
                    for (var i = 0; i < candidateUserDefinedOperators.Count; i++)
                    {
                        // Loop through all candidates.
                        var thisCandidate = candidateUserDefinedOperators[i];

                        for (int j = i + 1; j < candidateUserDefinedOperators.Count; j++)
                        {
                            // Loop through the other candidates we haven't already compared to.
                            var otherCandidate = candidateUserDefinedOperators[j];

                            // 0 means equal, -1 means thisCandidate is better, 1 means otherCandidate is better, null means we can't decide.
                            int? betterCandidate = 0;

                            for (int k = 0; k < thisCandidate.Parameters.Length && betterCandidate.HasValue; k++)
                            {
                                var thisParameter = thisCandidate.Parameters[k].ParameterType;
                                var otherParameter = otherCandidate.Parameters[k].ParameterType;

                                if (thisParameter == otherParameter)
                                    continue;

                                if (DoesImplicitConversionExist(thisParameter, otherParameter, true))
                                {
                                    // thisCandidate is more specific (and is therefore a better match) for this parameter!
                                    switch (betterCandidate.Value)
                                    {
                                        case 0:
                                        case -1:
                                            betterCandidate = -1;
                                            break;
                                        case 1:
                                            betterCandidate = null;
                                            break;
                                    }
                                }
                                else if (DoesImplicitConversionExist(otherParameter, thisParameter, true))
                                {
                                    // otherCandidate is more specific (and is therefore a better match) for this parameter!
                                    switch (betterCandidate.Value)
                                    {
                                        case 0:
                                        case 1:
                                            betterCandidate = 1;
                                            break;
                                        case -1:
                                            betterCandidate = null;
                                            break;
                                    }
                                }
                            }

                            // After having looped through all of the parameters comparing thisCandidate and otherCandidate,
                            // we've figured out one of four possibilities:
                            switch (betterCandidate)
                            {
                                case null:
                                case 0:
                                    // Either they're both the same, or they're both better/worse than each other. Either way, there's no clear-cut winner, so we'll keep both options.
                                    break;
                                case -1:
                                    // This option is better! We'll remove the other candidate!
                                    candidateUserDefinedOperators.RemoveAt(j);
                                    j--;
                                    break;
                                case 1:
                                    // The other option is better! We need to remove this candidate!
                                    candidateUserDefinedOperators.RemoveAt(i);
                                    i--;
                                    j = candidateUserDefinedOperators.Count;
                                    break;
                                default:
                                    throw new Exception(
                                        $"MathConverter internal exception: {nameof(GetUserDefinedOperator)} is in an invalid state.");
                            }
                        }
                    }

                    if (candidateUserDefinedOperators.Count == 1)
                    {
                        return candidateUserDefinedOperators[0].Method;
                    }
                    else
                    {
                        throw new AmbiguousMatchException(
                            $"Could not identify which {OperatorType} operator to apply to type{(parameters.Length == 1 ? "" : "s")} {string.Join(" and ", parameters.Select(p => p.GetType().FullName ?? "null").MyToArray())} between the following options:{string.Concat(candidateUserDefinedOperators.Select(p => $"{Environment.NewLine}{p}").MyToArray())}");
                    }
            }
        }

        protected Operator(OperationType operatorType)
        {
            OperatorType = operatorType;

            if (!Type.IsInstanceOfType(this))
            {
                throw new InvalidCastException($"The {OperatorType} operator is a {Type.Name}, not a {GetType().Name}.");
            }
        }

        protected readonly OperationType OperatorType;
        protected string OperatorName
        {
            get
            {
                switch (OperatorType)
                {
                    case OperationType.Exponentiation:
                    case OperationType.NullCoalescing:
                        return null;
                    case OperationType.And:
                        return "op_BitwiseAnd";
                    case OperationType.Or:
                        return "op_BitwiseOr";
                    case OperationType.Addition:
                        return "op_Addition";
                    case OperationType.Subtraction:
                        return "op_Subtraction";
                    case OperationType.Multiply:
                        return "op_Multiply";
                    case OperationType.Division:
                        return "op_Division";
                    case OperationType.Remainder:
                        return "op_Modulus";
                    case OperationType.Inequality:
                        return "op_Inequality";
                    case OperationType.Equality:
                        return "op_Equality";
                    case OperationType.LessThan:
                        return "op_LessThan";
                    case OperationType.LessThanOrEqual:
                        return "op_LessThanOrEqual";
                    case OperationType.GreaterThan:
                        return "op_GreaterThan";
                    case OperationType.GreaterThanOrEqual:
                        return "op_GreaterThanOrEqual";
                    case OperationType.LogicalNot:
                        return "op_LogicalNot";
                    case OperationType.UnaryNegation:
                        return "op_UnaryNegation";
                    default:
                        throw new NotSupportedException($"The {OperatorType} operator is not supported");
                }
            }
        }
        protected Operands SupportedOperands
        {
            get
            {
                switch (OperatorType)
                {
                    case OperationType.NullCoalescing:
                    case OperationType.And:
                    case OperationType.Or:
                        return Operands.CompletelyCustom;
                    case OperationType.Addition:
                        return Operands.Number | Operands.String | Operands.StringObject;
                    case OperationType.Exponentiation:
                    case OperationType.Subtraction:
                    case OperationType.Multiply:
                    case OperationType.Division:
                    case OperationType.Remainder:
                    case OperationType.LessThan:
                    case OperationType.LessThanOrEqual:
                    case OperationType.GreaterThan:
                    case OperationType.GreaterThanOrEqual:
                    case OperationType.UnaryNegation:
                        return Operands.Number;
                    case OperationType.Equality:
                    case OperationType.Inequality:
                        return Operands.Number | Operands.Boolean | Operands.String | Operands.Object;
                    case OperationType.LogicalNot:
                        return Operands.Boolean;
                    default:
                        throw new NotSupportedException($"The {OperatorType} operator is not supported");
                }
            }
        }
        private Type Type
        {
            get
            {
                switch (OperatorType)
                {
                    case OperationType.NullCoalescing:
                    case OperationType.And:
                    case OperationType.Or:
                    case OperationType.Addition:
                    case OperationType.Exponentiation:
                    case OperationType.Subtraction:
                    case OperationType.Multiply:
                    case OperationType.Division:
                    case OperationType.Remainder:
                    case OperationType.LessThan:
                    case OperationType.LessThanOrEqual:
                    case OperationType.GreaterThan:
                    case OperationType.GreaterThanOrEqual:
                    case OperationType.Equality:
                    case OperationType.Inequality:
                        return typeof(BinaryOperator);
                    case OperationType.LogicalNot:
                    case OperationType.UnaryNegation:
                        return typeof(UnaryOperator);
                    default:
                        throw new NotSupportedException($"The {OperatorType} operator is not supported");
                }
            }
        }
        public string OperatorSymbols
        {
            get
            {
                switch (OperatorType)
                {
                    case OperationType.Exponentiation:
                        return "^";
                    case OperationType.Addition:
                        return "+";
                    case OperationType.Subtraction:
                        return "-";
                    case OperationType.Multiply:
                        return "*";
                    case OperationType.Division:
                        return "/";
                    case OperationType.Remainder:
                        return "%";
                    case OperationType.And:
                        return "&&";
                    case OperationType.Or:
                        return "||";
                    case OperationType.NullCoalescing:
                        return "??";
                    case OperationType.Inequality:
                        return "!=";
                    case OperationType.Equality:
                        return "==";
                    case OperationType.LessThan:
                        return "<";
                    case OperationType.LessThanOrEqual:
                        return "<=";
                    case OperationType.GreaterThan:
                        return ">";
                    case OperationType.GreaterThanOrEqual:
                        return ">=";
                    case OperationType.LogicalNot:
                        return "!";
                    case OperationType.UnaryNegation:
                        return "!";
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public object EvaluateWithNullArguments()
        {
            switch (OperatorType)
            {
                case OperationType.Exponentiation:
                case OperationType.Addition:
                case OperationType.Subtraction:
                case OperationType.Multiply:
                case OperationType.Division:
                case OperationType.Remainder:
                case OperationType.And:
                case OperationType.LogicalNot:
                case OperationType.UnaryNegation:
                    return null;
                case OperationType.LessThan:
                case OperationType.LessThanOrEqual:
                case OperationType.GreaterThanOrEqual:
                case OperationType.GreaterThan:
                    return false;
                case OperationType.Inequality:
                    return false;
                case OperationType.Equality:
                    return true;
                default:
                    throw new NotSupportedException();
            }
        }

        public sealed override string ToString() => OperatorSymbols;
    }
    public sealed class UnaryOperator : Operator
    {
        private UnaryOperator(OperationType operatorType) : base(operatorType) { }

        public static implicit operator UnaryOperator(OperationType operatorType) => new UnaryOperator(operatorType);

        private double ApplyDefaultOperator(double operand)
        {
            switch (OperatorType)
            {
                case OperationType.UnaryNegation:
                    return -operand;
                default:
                    throw new NotSupportedException();
            }
        }
        private bool ApplyDefaultOperator(bool operand)
        {
            switch (OperatorType)
            {
                case OperationType.LogicalNot:
                    return !operand;
                default:
                    throw new NotSupportedException();
            }

        }

        public object Evaluate(object operand)
        {
            if (operand == null)
            {
                return EvaluateWithNullArguments();
            }

            var @operator = GetUserDefinedOperator(out var convertToDoubles, operand);

            if (@operator == null)
            {
                // Fall back to predefined operator.
                // We simply apply any numeric operations with values converted to doubles.
                if ((SupportedOperands & Operands.Number) == Operands.Number)
                {
                    // This operator supports two numeric arguments.
                    if (DoesImplicitConversionExist(operand.GetType(), typeof(double), true))
                    {
                        return ApplyDefaultOperator((double)DoImplicitConversion(operand, typeof(double)));
                    }
                }

                if ((SupportedOperands & Operands.Boolean) == Operands.Boolean)
                {
                    // This operator supports two boolean arguments.
                    if (operand is bool boolean)
                    {
                        return ApplyDefaultOperator(boolean);
                    }
                }

                throw InvalidOperator(OperatorSymbols, operand);
            }
            else
            {
                if (convertToDoubles)
                    operand = DoImplicitConversion(operand, typeof(double?));

                // Invoke the operator!
                return @operator.Invoke(null, new[] { operand });
            }
        }
    }
    public sealed class BinaryOperator : Operator
    {
        private BinaryOperator(OperationType operatorType) : base(operatorType) { }

        public static implicit operator BinaryOperator(OperationType operatorType) => new BinaryOperator(operatorType);

        private object ApplyCustomOperator(object l, Func<object> evaluateRight)
        {
            switch (OperatorType)
            {
                case OperationType.NullCoalescing:
                    // There's really nothing special going on here with type conversion. This way we only evaluate the right syntax tree if the left returns null.
                    return l ?? evaluateRight();
                case OperationType.And:
                case OperationType.Or:
                    // "&" and "|" operators are special, because we're trying to evaluate them as though they were "&&" and "||" Operators.
                    // This means that we might not need to evaluate the right tree.
                    // See https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#conditional-logical-operators

                    object EvaluateRightAsBool(bool convertToBool)
                    {
                        var r = evaluateRight();
                        var tryConvert = TryConvertToBool(r);

                        if (tryConvert == null && r != null)
                        {
                            // We can't convert the operand to a boolean.
                            throw InvalidOperator(OperatorSymbols, l, r);
                        }
                        return convertToBool ? tryConvert : r;
                    }

                    var x = TryConvertToBool(l);

                    if (x.HasValue || l == null)
                    {
                        // We might need to evaluate the right operand.
                        if (l == null || x.Value == (OperatorType == OperationType.And))
                        {
                            var r = evaluateRight();

                            // Is there an "&" operator? If l is null, we'll check for an "&" operator between r & r.
                            var @operator = GetUserDefinedOperator(out _, l ?? r, r);

                            if (@operator != null)
                            {
                                // If so, evaluate the operator.
                                return @operator.Invoke(null, new[] { l, r });
                            }
                        }

                        // If not, evaluate it 
                        switch (OperatorType)
                        {
                            case OperationType.And:
                                if (x.HasValue)
                                {
                                    return x.Value ? EvaluateRightAsBool(l is bool) : l;
                                }
                                else
                                {
                                    var r = EvaluateRightAsBool(false);
                                    return TryConvertToBool(r) == false ? r : null;
                                }
                            case OperationType.Or:
                                if (x.HasValue)
                                {
                                    return x.Value ? l : EvaluateRightAsBool(l is bool);
                                }
                                else
                                {
                                    var r = EvaluateRightAsBool(false);
                                    return TryConvertToBool(r) == true ? r : null;
                                }
                            default:
                                throw new NotSupportedException();
                        }
                    }
                    else
                    {
                        // The first operand doesn't convert to a boolean. We can't use "&&" on it.
                        throw InvalidOperator(OperatorSymbols, l, evaluateRight());
                    }
                default:
                    throw new NotSupportedException();
            }
        }
        private object ApplyDefaultOperator(double? x, double? y)
        {
            switch (OperatorType)
            {
                case OperationType.Exponentiation:
                    return (!x.HasValue || !y.HasValue) ? new double?() : Math.Pow(x.Value, y.Value);
                case OperationType.Addition:
                    return x + y;
                case OperationType.Subtraction:
                    return x - y;
                case OperationType.Multiply:
                    return x * y;
                case OperationType.Division:
                    return x / y;
                case OperationType.Remainder:
                    return x % y;
                case OperationType.Equality:
                    return x == y;
                case OperationType.Inequality:
                    return x != y;
                case OperationType.LessThan:
                    return x < y;
                case OperationType.LessThanOrEqual:
                    return x <= y;
                case OperationType.GreaterThan:
                    return x > y;
                case OperationType.GreaterThanOrEqual:
                    return x >= y;
                default:
                    throw new NotSupportedException();
            }
        }
        private object ApplyDefaultOperator(string x, string y)
        {
            switch (OperatorType)
            {
                case OperationType.Addition:
                    return x + y;
                case OperationType.Inequality:
                    return x != y;
                case OperationType.Equality:
                    return x == y;
                default:
                    throw new NotSupportedException();
            }
        }
        private string ApplyDefaultOperator(object x, string y)
        {
            switch (OperatorType)
            {
                case OperationType.Addition:
                    return x + y;
                default:
                    throw new NotSupportedException();
            }
        }
        private string ApplyDefaultOperator(string x, object y)
        {
            switch (OperatorType)
            {
                case OperationType.Addition:
                    return x + y;
                default:
                    throw new NotSupportedException();
            }
        }
        private bool? ApplyDefaultOperator(bool? x, bool? y)
        {
            switch (OperatorType)
            {
                case OperationType.Inequality:
                    return x != y;
                case OperationType.Equality:
                    return x == y;
                default:
                    throw new NotSupportedException();
            }
        }
        private bool? ApplyDefaultOperator(object x, object y)
        {
            switch (OperatorType)
            {
                case OperationType.Inequality:
                    return x != y;
                case OperationType.Equality:
                    return x == y;
                default:
                    throw new NotSupportedException();
            }
        }

        private object Evaluate(object x, Func<object> evaluateRight)
        {
            if (SupportedOperands == Operands.CompletelyCustom)
            {
                return ApplyCustomOperator(x, evaluateRight);
            }

            var y = evaluateRight();

            if (x == null && y == null)
            {
                return EvaluateWithNullArguments();
            }

            var @operator = GetUserDefinedOperator(out bool convertToDoubles, x, y);

            if (@operator == null)
            {
                // Fall back to predefined operator.
                // We simply apply any numeric operations with values converted to doubles.
                if ((SupportedOperands & Operands.Number) == Operands.Number)
                {
                    // This operator supports two numeric arguments.
                    if (DoesImplicitConversionExist(x?.GetType(), typeof(double), true) &&
                        DoesImplicitConversionExist(y?.GetType(), typeof(double), true))
                    {
                        return ApplyDefaultOperator(x == null ? null : (double?)DoImplicitConversion(x, typeof(double?)),
                            y == null ? null : (double?)DoImplicitConversion(y, typeof(double?)));
                    }
                }

                if ((SupportedOperands & Operands.Boolean) == Operands.Boolean)
                {
                    // This operator supports two boolean arguments.
                    if ((x is bool || x == null) && (y is bool || y == null))
                    {
                        return ApplyDefaultOperator((bool?)x, (bool?)y);
                    }
                }

                if (x is string || y is string)
                {
                    if ((SupportedOperands & Operands.String) == Operands.String && x is string xStr && y is string yStr)
                    {
                        // This operator supports two string arguments.
                        return ApplyDefaultOperator(xStr, yStr);
                    }

                    if ((SupportedOperands & Operands.StringObject) == Operands.StringObject)
                    {
                        // This operator supports one string and one object operand
                        if (x is string xStr2)
                        {
                            return ApplyDefaultOperator(xStr2, y);
                        }
                        else
                        {
                            return ApplyDefaultOperator(x, (string)y);
                        }
                    }
                }

                if ((SupportedOperands & Operands.Object) == Operands.Object)
                {
                    // This operator supports any arbitrary operands.
                    return ApplyDefaultOperator(x, y);
                }

                throw InvalidOperator(OperatorSymbols, x, y);
            }
            else
            {
                if (convertToDoubles)
                {
                    x = DoImplicitConversion(x, typeof(double?));
                    y = DoImplicitConversion(y, typeof(double?));
                }
                // Invoke the operator!
                return @operator.Invoke(null, new[] { x, y });
            }
        }
        public object Evaluate(object x, object y) => Evaluate(x, () => y);
        public object Evaluate(AbstractSyntaxTree left, AbstractSyntaxTree right, CultureInfo cultureInfo, object[] parameters)
        {
            return Evaluate(left.Evaluate(cultureInfo, parameters), () => right.Evaluate(cultureInfo, parameters));
        }
    }
    /// <summary>
    /// The ternary "?:" operator.
    /// It is designed to work like the "?:" operator.
    /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#conditional-operator"/>
    /// </summary>
    public static class TernaryOperator
    {
        public static object Evaluate(AbstractSyntaxTree condition, AbstractSyntaxTree positive, AbstractSyntaxTree negative, CultureInfo cultureInfo, object[] parameters)
        {
            var conditionObj = condition.Evaluate(cultureInfo, parameters);
            var conditionBool = Operator.TryConvertToBool(conditionObj);

            if (conditionBool.HasValue)
            {
                return (conditionBool.Value ? positive : negative).Evaluate(cultureInfo, parameters);
            }
            else
            {
                throw new InvalidOperationException($"Cannot apply operator '?:' when the first operand is {(conditionObj == null ? "null" : $"of type '{conditionObj.GetType().FullName}'")}");
            }
        }
    }
    public enum OperationType
    {
        Exponentiation,
        Addition,
        Subtraction,
        Multiply,
        Division,
        Remainder,
        And,
        Or,
        NullCoalescing,
        Inequality,
        Equality,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LogicalNot,
        UnaryNegation,
    }
}
