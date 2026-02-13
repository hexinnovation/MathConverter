using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace HexInnovation
{
#if NET5_0_OR_GREATER
#pragma warning disable CA1716 // Identifiers should not match keywords
#endif
    public abstract class Operator
#if NET5_0_OR_GREATER
#pragma warning restore CA1716 // Identifiers should not match keywords
#endif
    {
        /// <summary>
        /// The binary "^" operator. This operator returns the first operand raised to the power of the second.
        /// If either operand is null, it will return null, and it only supports numeric types.
        /// It is designed explicitly different from the C# "^" (exclusive or) operator.
        /// </summary>
        public static readonly BinaryOperator Exponentiation = OperatorTypes.Exponentiation;

        /// <summary>
        /// The binary "+" operator. This operator adds two operands together.
        /// It is designed to work like the "+" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#addition-operator"/>
        /// </summary>
        public static readonly BinaryOperator Addition = OperatorTypes.Addition;

        /// <summary>
        /// The binary "-" operator. This operator subtracts one operand from another.
        /// It is designed to work like the "-" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#subtraction-operator"/>
        /// </summary>
        public static readonly BinaryOperator Subtraction = OperatorTypes.Subtraction;

        /// <summary>
        /// The binary "*" operator. This operator multiplies one operand by another.
        /// It is designed to work like the "*" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#multiplication-operator"/>
        /// </summary>
        public static readonly BinaryOperator Multiply = OperatorTypes.Multiply;

        /// <summary>
        /// The binary "/" operator. This operator divides one operand by another.
        /// It is designed to work like the "/" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#division-operator"/>
        /// </summary>
        public static readonly BinaryOperator Division = OperatorTypes.Division;

        /// <summary>
        /// The binary "%" operator. This operator divides one operand by another and returns the remainder.
        /// It is designed to work like the "%" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#remainder-operator"/>
        /// </summary>
        public static readonly BinaryOperator Remainder = OperatorTypes.Remainder;

        /// <summary>
        /// The binary "&&" operator. This operator explicitly does not do a bitwise-and, and is only applicable for boolean operands, or types that define a "false" operator and a "&" operator.
        /// If the first operand is false, the second operand will not be evaluated.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#conditional-logical-operators"/>
        /// </summary>
        public static readonly BinaryOperator And = OperatorTypes.And;

        /// <summary>
        /// The binary "||" operator. This operator explicitly does not do a bitwise-and, and is only applicable for boolean operands, or types that define a "true" operator and a "|" operator.
        /// If the first operand is true, the second operand will not be evaluated.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#conditional-logical-operators"/>
        /// </summary>
        public static readonly BinaryOperator Or = OperatorTypes.Or;

        /// <summary>
        /// The binary "??" operator.
        /// It is designed to work like the "??" operator in C#.
        /// It evaluates and returns the first object, unless it is null, in which case it will evaluate and return the second object.
        /// The second expression is only evaluated if the first object is null.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#the-null-coalescing-operator"/>
        /// </summary>
        public static readonly BinaryOperator NullCoalescing = OperatorTypes.NullCoalescing;

        /// <summary>
        /// The binary "!=" operator.
        /// It is designed to work like the "!=" operator in C#, returning true if the two arguments are not equal, or false if they are equal.
        /// Note: If no custom operator is available and we need to evaluate arbitrary objects using the default <c>x != y</c>, we will prefer <c>!x.Equals(y)</c> instead, provided one of the operands is not null.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#relational-and-type-testing-operators"/>
        /// </summary>
        public static readonly BinaryOperator Inequality = OperatorTypes.Inequality;

        /// <summary>
        /// The binary "==" operator.
        /// It is designed to work like the "==" operator in C#, returning true if the two arguments are equal, or false if not.
        /// Note: If no custom operator is available and we need to evaluate arbitrary objects using the default <c>x == y</c>, we will prefer <c>x.Equals(y)</c> instead, provided one of the operands is not null.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#relational-and-type-testing-operators"/>
        /// </summary>
        public static readonly BinaryOperator Equality = OperatorTypes.Equality;

        /// <summary>
        /// The binary "&lt;" operator.
        /// It is designed to work like the "&lt;" operator.
        /// It works on numeric types (after converting the operands to double), and also works on any type that defines a "&lt;" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#relational-and-type-testing-operators"/>
        /// </summary>
        public static readonly BinaryOperator LessThan = OperatorTypes.LessThan;

        /// <summary>
        /// The binary "&lt;=" operator.
        /// It is designed to work like the "&lt;=" operator.
        /// It works on numeric types (after converting the operands to double), and also works on any type that defines a "&lt;=" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#relational-and-type-testing-operators"/>
        /// </summary>
        public static readonly BinaryOperator LessThanOrEqual = OperatorTypes.LessThanOrEqual;

        /// <summary>
        /// The binary ">" operator.
        /// It is designed to work like the ">" operator.
        /// It works on numeric types (after converting the operands to double), and also works on any type that defines a ">" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#relational-and-type-testing-operators"/>
        /// </summary>
        public static readonly BinaryOperator GreaterThan = OperatorTypes.GreaterThan;

        /// <summary>
        /// The binary ">=" operator.
        /// It is designed to work like the ">=" operator.
        /// It works on numeric types (after converting the operands to double), and also works on any type that defines a ">=" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#relational-and-type-testing-operators"/>
        /// </summary>
        public static readonly BinaryOperator GreaterThanOrEqual = OperatorTypes.GreaterThanOrEqual;

        /// <summary>
        /// The unary "!" operator.
        /// It is designed to work like the "!" operator.
        /// It works on boolean arguments, or any type that defines a "!" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#logical-negation-operator" />
        /// </summary>
        public static readonly UnaryOperator LogicalNot = OperatorTypes.LogicalNot;

        /// <summary>
        /// The unary "-" operator.
        /// It is designed to work like the unary "-" operator.
        /// It works on numeric arguments (after converting the operand to double), and also works on any type that defines a "-" operator.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#unary-minus-operator"/>
        /// </summary>
        public static readonly UnaryOperator UnaryNegation = OperatorTypes.UnaryNegation;

        private class OperatorInfo
        {
            public MethodInfo Method { get; set; }
            public ParameterInfo[] Parameters { get; set; }

            public bool IsApplicable(Type[] operandTypes)
            {
                // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#applicable-function-member

                // This logic is simplified because operators cannot be called with ref/out or optional parameters.
                if (Parameters.Length != operandTypes.Length)
                    return false;

                // Can we implicitly convert the parameters to the necessary type?
                for (int i = 0; i < Parameters.Length; i++)
                {
                    if (!DoesImplicitConversionExist(operandTypes[i], Parameters[i].ParameterType, false))
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
#if NET5_0_OR_GREATER
#pragma warning disable CA1720 // Identifier contains type name
#endif
            String = 4,
            StringObject = 8,
            Object = 16,
#if NET5_0_OR_GREATER
#pragma warning restore CA1720 // Identifier contains type name
#endif
        }
        private static IEnumerable<Type> GetTypeAndSubtypes(Type type)
        {
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#base-types

            if (type is null)
                yield break;

            type = Nullable.GetUnderlyingType(type) ?? type;

            yield return type;

            if (type.IsEnum)
                yield return typeof(Enum);

            if (type.IsValueType)
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
            while ((type = type.BaseType) != null)
            {
                yield return type;
            }
        }
        private static IEnumerable<Type> GetTypeAndSubtypes(params Type[] types)
        {
            return types.SelectMany(GetTypeAndSubtypes).Distinct();
        }

        private static List<OperatorInfo> GetPossibleOperators(string operatorName, params Type[] operandTypes)
        {
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#candidate-user-defined-operators
            return [.. GetTypeAndSubtypes(operandTypes).SelectMany(type =>
                    type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Where(method => method.Name == operatorName)
                        .Select(method => new OperatorInfo
                        {
                            Method = method,
                            Parameters = method.GetParameters()
                        })
                        .Where(p => p.IsApplicable(operandTypes))
                )];
        }
        private static List<OperatorInfo> GetPossibleOperators(string operatorName, params object[] operands)
        {
            return GetPossibleOperators(operatorName, [.. operands.Select(p => p?.GetType() ?? typeof(object))]);
        }

        internal static List<MethodInfo> GetImplicitOperatorPath(string operatorName, Type typeFrom, Type typeTo)
        {
            var implicitOperators = GetPossibleOperators(operatorName, typeFrom)
                .ToDictionary(p => p.Method.ReturnType, p => new List<MethodInfo> { p.Method });
            var previousLevelOperators = implicitOperators.Keys.Distinct().ToList();
            var underlyingType = Nullable.GetUnderlyingType(typeTo);

            implicitOperators[typeFrom] = [];

            if (previousLevelOperators.Count > 0)
            {
                while (previousLevelOperators.Count > 0)
                {
                    if (implicitOperators.TryGetValue(typeTo, out var x))
                        return x;

                    if (underlyingType is not null && implicitOperators.TryGetValue(underlyingType, out x))
                        return x;

                    var previousLevelOperatorsCopy = previousLevelOperators.ToList();
                    previousLevelOperators.Clear();

                    foreach (var type in previousLevelOperatorsCopy)
                    {
                        var possibleOperators = GetPossibleOperators(operatorName, type);

                        foreach (var nextLevelOperator in possibleOperators)
                        {
                            if (!implicitOperators.ContainsKey(nextLevelOperator.Method.ReturnType))
                            {
                                implicitOperators[nextLevelOperator.Method.ReturnType] = [.. implicitOperators[type], nextLevelOperator.Method];
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
                        return conversionPath.Value;

                    if (underlyingType is not null && DoesImplicitConversionExist(conversionPath.Key, underlyingType, false))
                        return conversionPath.Value;
                }
            }

            // There's no implicit conversion from typeFrom to typeTo.
            return null;
        }
        /// <summary>
        /// Checks to see if there is a path to implicitly convert from one type to another.
        /// Effectively, this method tells you if <see cref="DoImplicitConversion(object, Type)"/> will succeed.
        /// </summary>
        /// <param name="typeFrom">The type to convert from.</param>
        /// <param name="typeTo">The type to convert to.</param>
        /// <param name="allowImplicitOperator">True to allow implicit operators (e.g. what the compiler uses to implicitly convert int to double). Generally you want to specify <c>true</c> here.</param>
        internal static bool DoesImplicitConversionExist(Type typeFrom, Type typeTo, bool allowImplicitOperator)
        {
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/conversions#implicit-numeric-conversions

            // We're not worrying about nullable types.
            if (typeFrom is null)
                return true;

            // For numeric types, we allow more conversion types. We implicitly convert from unsigned values to signed and from any type to itself.
            // Essentially, this method is here to see if we can call DoImplicitConversion to safely convert numeric types up to bigger types.
            if (typeFrom == typeof(sbyte) || typeFrom == typeof(byte))
                return typeTo == typeof(sbyte) || typeTo == typeof(short) || typeTo == typeof(ushort) ||
                       typeTo == typeof(int) || typeTo == typeof(uint) || typeTo == typeof(long) ||
                       typeTo == typeof(ulong) || typeTo == typeof(float) || typeTo == typeof(double) ||
                       typeTo == typeof(decimal);

            if (typeFrom == typeof(short) || typeFrom == typeof(ushort))
                return typeTo == typeof(ushort) || typeTo == typeof(short) || typeTo == typeof(int) ||
                       typeTo == typeof(uint) || typeTo == typeof(long) || typeTo == typeof(ulong) ||
                       typeTo == typeof(float) || typeTo == typeof(double) || typeTo == typeof(decimal);

            if (typeFrom == typeof(int) || typeFrom == typeof(uint))
                return typeTo == typeof(uint) || typeTo == typeof(int) || typeTo == typeof(long) ||
                       typeTo == typeof(ulong) || typeTo == typeof(float) || typeTo == typeof(double) ||
                       typeTo == typeof(decimal);

            if (typeFrom == typeof(long) || typeFrom == typeof(ulong))
                return typeTo == typeof(long) || typeTo == typeof(ulong) || typeTo == typeof(float) ||
                       typeTo == typeof(double) || typeTo == typeof(decimal);

            if (typeFrom == typeof(char))
                return typeTo == typeof(char) || typeTo == typeof(ushort) || typeTo == typeof(int) ||
                       typeTo == typeof(uint) || typeTo == typeof(long) || typeTo == typeof(ulong) ||
                       typeTo == typeof(float) || typeTo == typeof(double) || typeTo == typeof(decimal);

            if (typeFrom == typeof(float))
                return typeTo == typeof(float) || typeTo == typeof(double);

            if (typeFrom == typeof(decimal))
                return typeTo == typeof(decimal) || typeTo == typeof(double);

            if (typeFrom == typeof(double))
                return typeTo == typeof(double);

            // Ignoring Implicit enumeration conversions section...
            // Ignoring Implicit interpolated string conversions...
            // Ignoring Implicit nullable conversions...

            // Reference conversions!
            if (typeTo == typeof(object))
                return true;

            if (GetTypeAndSubtypes(typeFrom).Contains(typeTo))
                return true;

            if (typeTo.IsInterface && typeFrom.GetInterfaces().Contains(typeTo))
                return true;

            if (typeFrom.IsArray && typeTo.IsArray && typeFrom.GetArrayRank() == typeTo.GetArrayRank())
                return !typeFrom.IsValueType && !typeTo.IsValueType && DoesImplicitConversionExist(typeFrom.GetElementType(), typeTo.GetElementType(), true);

            if (typeFrom.IsArray && GetTypeAndSubtypes(typeof(Array)).Contains(typeTo))
                return true;

            if (typeFrom.IsArray && typeFrom.GetArrayRank() == 1 && GetTypeAndSubtypes(typeof(IList<>)).Contains(typeTo))
                return DoesImplicitConversionExist(typeFrom.GetElementType(), typeTo.GetGenericArguments()[0], true);

            // MathConverter doesn't support delegates.

            if (allowImplicitOperator && GetImplicitOperatorPath("op_Implicit", typeFrom, typeTo) is not null)
                return true;

            // Check True/False operators.
            if ((typeTo == typeof(bool) || typeTo == typeof(bool?)) && GetImplicitOperatorPath("op_True", typeFrom, typeTo) is not null)
                return true;

            // This is probably good enough???
            return false;
        }

        /// <summary>
        /// Implicitly converts an object to a given type. This method can throw an exception if the conversion fails.
        /// See <see cref="DoesImplicitConversionExist(Type, Type, bool)"/>
        /// </summary>
        /// <param name="from">The object to convert.</param>
        /// <param name="typeTo">The type to convert to.</param>
        internal static object DoImplicitConversion(object from, Type typeTo)
        {
            // Converting null to a class
            if (from is null && !typeTo.IsValueType)
                return null;

            var structTypeTo = Nullable.GetUnderlyingType(typeTo);

            // Converting null to a Nullable<T>.
            if (from is null && structTypeTo is { IsValueType: true })
                return Activator.CreateInstance(typeTo);

            // Convert from T to Nullable<T>.
            if (structTypeTo != null && from != null && from.GetType() == structTypeTo)
                return from;

            object ConvertWithOperators(List<MethodInfo> chain)
            {
                chain.ForEach(x => from = x.Invoke(null, [from]));

                // After we finish the implicit operator(s), we may need to do another implicit conversion (e.g. to go from int to Nullable<int>).
                return DoImplicitConversion(from, typeTo);
            }

            // Check implicit casts.
            if (GetImplicitOperatorPath("op_Implicit", from?.GetType() ?? typeof(object), typeTo) is { Count: > 0 } implicitCasts)
                return ConvertWithOperators(implicitCasts);

            // Check true operator
            if ((typeTo == typeof(bool) || typeTo == typeof(bool?)) && GetImplicitOperatorPath("op_True", from?.GetType() ?? typeof(object), typeTo) is { Count: > 0 } trueOperator)
                return ConvertWithOperators(trueOperator);

            // We might have to convert to a non-nullable type.
            if (structTypeTo != null && from != null)
            {
                // If we're converting a char, we'll always convert to integer first.
                if (from is char)
                    from = Convert.ToInt32(from, CultureInfo.InvariantCulture);

                return Convert.ChangeType(from, structTypeTo, CultureInfo.InvariantCulture);
            }

            return Convert.ChangeType(from, typeTo, CultureInfo.InvariantCulture);
        }

        internal static bool? TryConvertToBool(object value) =>
            value is bool b ?
                b :
                value == null ?
                null :
                DoesImplicitConversionExist(value.GetType(), typeof(bool?), true) ?
                (bool)DoImplicitConversion(value, typeof(bool?)) :
                null;
        protected static InvalidOperationException InvalidOperator(string operatorSymbols, params object[] operands)
        {
            var argTypes = operands.Select(x => x == null ? "null" : $"'{x.GetType().FullName}'").ToList();

            return new InvalidOperationException($"Cannot apply operator '{operatorSymbols}' to operand{(operands.Length == 1 ? "" : "s")} of type {string.Join(" ", argTypes.Take(argTypes.Count - 1))}{(argTypes.Count == 1 ? "" : " and ")}{argTypes.Last()}");
        }
        protected MethodInfo GetUserDefinedOperator(out bool convertToDoubles, params object[] operands)
        {
            // Without implicit operators, the only types that will convert to double are built-in numeric structs (char, byte, sbyte, int, long, float, decimal, etc.)
            convertToDoubles = (SupportedOperands & Operands.Number) == Operands.Number && operands.All(p => DoesImplicitConversionExist(p?.GetType(), typeof(double), false));

            if (convertToDoubles)
                // If we're trying to compare int < decimal, we might run in to problems where it picks the wrong operator.
                // Here, we'll force any operand resolutions to be in the double class.
                operands = [.. operands.Select(p => DoImplicitConversion(p, typeof(double?)))];

            var candidateUserDefinedOperators = GetPossibleOperators(OperatorName, operands);
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
                                    throw new InvalidOperationException($"Unreachable: {nameof(GetUserDefinedOperator)} is in an invalid state.");
                            }
                        }
                    }

                    return candidateUserDefinedOperators.Count == 1 ?
                        candidateUserDefinedOperators[0].Method :
                        throw new AmbiguousMatchException(
                            $"Could not identify which {OperatorType} operator to apply to type{(operands.Length == 1 ? "" : "s")} {string.Join(" and ", operands.Select(p => p.GetType().FullName ?? "null"))} between the following options:{string.Concat<string>(candidateUserDefinedOperators.Select(p => $"{Environment.NewLine}{p}"))}");
            }
        }

        // Don't allow other assemblies to subclass. The only valid Operator subclasses are BinaryOperator and UnaryOperator.
        internal Operator(OperatorTypes operatorType)
        {
            OperatorType = operatorType;

            // Make sure we're not using a binary operator on a unary operator or vice versa.
            if (!Type.IsInstanceOfType(this))
                throw new InvalidCastException($"The {OperatorType} operator is a {Type.Name}, not a {GetType().Name}.");
        }

        protected OperatorTypes OperatorType { get; }
        protected string OperatorName =>
            OperatorType switch
            {
                OperatorTypes.Exponentiation or
                OperatorTypes.NullCoalescing => null,

                OperatorTypes.And => "op_BitwiseAnd",
                OperatorTypes.Or => "op_BitwiseOr",
                OperatorTypes.Addition => "op_Addition",
                OperatorTypes.Subtraction => "op_Subtraction",
                OperatorTypes.Multiply => "op_Multiply",
                OperatorTypes.Division => "op_Division",
                OperatorTypes.Remainder => "op_Modulus",
                OperatorTypes.Inequality => "op_Inequality",
                OperatorTypes.Equality => "op_Equality",
                OperatorTypes.LessThan => "op_LessThan",
                OperatorTypes.LessThanOrEqual => "op_LessThanOrEqual",
                OperatorTypes.GreaterThan => "op_GreaterThan",
                OperatorTypes.GreaterThanOrEqual => "op_GreaterThanOrEqual",
                OperatorTypes.LogicalNot => "op_LogicalNot",
                OperatorTypes.UnaryNegation => "op_UnaryNegation",
                _ => throw new NotSupportedException($"The {OperatorType} operator is not supported")
            };

        protected Operands SupportedOperands =>
            OperatorType switch
            {
                OperatorTypes.NullCoalescing or
                OperatorTypes.And or
                OperatorTypes.Or => Operands.CompletelyCustom,

                OperatorTypes.Addition => Operands.Number | Operands.String | Operands.StringObject,

                OperatorTypes.Exponentiation or
                OperatorTypes.Subtraction or
                OperatorTypes.Multiply or
                OperatorTypes.Division or
                OperatorTypes.Remainder or
                OperatorTypes.LessThan or
                OperatorTypes.LessThanOrEqual or
                OperatorTypes.GreaterThan or
                OperatorTypes.GreaterThanOrEqual or
                OperatorTypes.UnaryNegation => Operands.Number,

                OperatorTypes.Equality or
                OperatorTypes.Inequality => Operands.Number | Operands.Boolean | Operands.String | Operands.Object,

                OperatorTypes.LogicalNot => Operands.Boolean,

                _ => throw new NotSupportedException($"The {OperatorType} operator is not supported"),
            };
        private Type Type =>
            OperatorType switch
            {
                OperatorTypes.NullCoalescing or
                OperatorTypes.And or
                OperatorTypes.Or or
                OperatorTypes.Addition or
                OperatorTypes.Exponentiation or
                OperatorTypes.Subtraction or
                OperatorTypes.Multiply or
                OperatorTypes.Division or
                OperatorTypes.Remainder or
                OperatorTypes.LessThan or
                OperatorTypes.LessThanOrEqual or
                OperatorTypes.GreaterThan or
                OperatorTypes.GreaterThanOrEqual or
                OperatorTypes.Equality or
                OperatorTypes.Inequality => typeof(BinaryOperator),

                OperatorTypes.LogicalNot or
                OperatorTypes.UnaryNegation => typeof(UnaryOperator),

                _ => throw new NotSupportedException($"The {OperatorType} operator is not supported")
            };

        public string OperatorSymbols =>
            OperatorType switch
            {
                OperatorTypes.Exponentiation => "^",
                OperatorTypes.Addition => "+",
                OperatorTypes.Subtraction => "-",
                OperatorTypes.Multiply => "*",
                OperatorTypes.Division => "/",
                OperatorTypes.Remainder => "%",
                OperatorTypes.And => "&&",
                OperatorTypes.Or => "||",
                OperatorTypes.NullCoalescing => "??",
                OperatorTypes.Inequality => "!=",
                OperatorTypes.Equality => "==",
                OperatorTypes.LessThan => "<",
                OperatorTypes.LessThanOrEqual => "<=",
                OperatorTypes.GreaterThan => ">",
                OperatorTypes.GreaterThanOrEqual => ">=",
                OperatorTypes.LogicalNot => "!",
                OperatorTypes.UnaryNegation => "-",
                _ => throw new NotSupportedException()
            };

        public object EvaluateWithNullOperands() =>
            OperatorType switch
            {
                OperatorTypes.Exponentiation or
                OperatorTypes.Addition or
                OperatorTypes.Subtraction or
                OperatorTypes.Multiply or
                OperatorTypes.Division or
                OperatorTypes.Remainder or
                OperatorTypes.And or
                OperatorTypes.LogicalNot or
                OperatorTypes.UnaryNegation => null,

                OperatorTypes.LessThan or
                OperatorTypes.LessThanOrEqual or
                OperatorTypes.GreaterThan or
                OperatorTypes.GreaterThanOrEqual or
                OperatorTypes.Inequality => false,

                OperatorTypes.Equality => true,

                _ => throw new NotSupportedException()
            };

        public sealed override string ToString() => OperatorSymbols;
    }
    public sealed class UnaryOperator : Operator
    {
        private UnaryOperator(OperatorTypes operatorType) : base(operatorType) { }

        public static implicit operator UnaryOperator(OperatorTypes operatorType) => new(operatorType);

        private double ApplyDefaultOperator(double operand) =>
            OperatorType switch
            {
                OperatorTypes.UnaryNegation => -operand,
                _ => throw new NotSupportedException()
            };
        private bool ApplyDefaultOperator(bool operand) =>
            OperatorType switch
            {
                OperatorTypes.LogicalNot => !operand,
                _ => throw new NotSupportedException()
            };

        public object Evaluate(object operand)
        {
            if (operand is null)
                return EvaluateWithNullOperands();

            var @operator = GetUserDefinedOperator(out var convertToDoubles, operand);

            if (@operator is null)
            {
                // Fall back to predefined operator.
                // We simply apply any numeric operations with values converted to doubles.
                if ((SupportedOperands & Operands.Number) == Operands.Number && DoesImplicitConversionExist(operand.GetType(), typeof(double), true))
                    return ApplyDefaultOperator((double)DoImplicitConversion(operand, typeof(double)));

                // This operator supports one boolean operand.
                return (SupportedOperands & Operands.Boolean) == Operands.Boolean && operand is bool boolean
                    ? (object)ApplyDefaultOperator(boolean)
                    : throw InvalidOperator(OperatorSymbols, operand);
            }
            else
            {
                if (convertToDoubles)
                    operand = DoImplicitConversion(operand, typeof(double?));

                // Invoke the operator!
                return @operator.Invoke(null, [operand]);
            }
        }
    }
    public sealed class BinaryOperator : Operator
    {
        private BinaryOperator(OperatorTypes operatorType) : base(operatorType) { }

        public static implicit operator BinaryOperator(OperatorTypes operatorType) => new(operatorType);

        private object ApplyCustomOperator(object l, Func<object> evaluateRightOperand)
        {
            switch (OperatorType)
            {
                case OperatorTypes.NullCoalescing:
                    // There's really nothing special going on here with type conversion. This way we only evaluate the right syntax tree if the left returns null.
                    return l ?? evaluateRightOperand();
                case OperatorTypes.And:
                case OperatorTypes.Or:
                    // "&" and "|" operators are special, because we're trying to evaluate them as though they were "&&" and "||" Operators.
                    // This means that we might not need to evaluate the right tree.
                    // See https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#conditional-logical-operators

                    object EvaluateRightAsBool(bool convertToBool)
                    {
                        var r = evaluateRightOperand();
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
                        if (l == null || x.Value == (OperatorType == OperatorTypes.And))
                        {
                            var r = evaluateRightOperand();

                            // Is there an "&" operator? If l is null, we'll check for an "&" operator between r & r.
                            var @operator = GetUserDefinedOperator(out _, l ?? r, r);

                            if (@operator != null)
                            {
                                // If so, evaluate the operator.
                                return @operator.Invoke(null, [l, r]);
                            }
                        }

                        // If not, evaluate it
                        return OperatorType switch
                        {
                            OperatorTypes.And =>
                                x switch
                                {
                                    true => EvaluateRightAsBool(l is bool),
                                    false => l,
                                    null => EvaluateRightAsBool(false) is { } r && TryConvertToBool(r) == false ? r : null
                                },
                            OperatorTypes.Or =>
                                x switch
                                {
                                    true => l,
                                    false => EvaluateRightAsBool(l is bool),
                                    null => EvaluateRightAsBool(false) is { } r && TryConvertToBool(r) == true ? r : null
                                },

                            _ => throw new NotSupportedException()
                        };
                    }
                    else
                    {
                        // The first operand doesn't convert to a boolean. We can't use "&&" on it.
                        throw InvalidOperator(OperatorSymbols, l, evaluateRightOperand());
                    }
                default:
                    throw new NotSupportedException();
            }
        }
        private object ApplyDefaultOperator(double? x, double? y) =>
            OperatorType switch
            {
                OperatorTypes.Exponentiation when x.HasValue && y.HasValue => Math.Pow(x.Value, y.Value),
                OperatorTypes.Exponentiation => null,
                OperatorTypes.Addition => x + y,
                OperatorTypes.Subtraction => x - y,
                OperatorTypes.Multiply => x * y,
                OperatorTypes.Division => x / y,
                OperatorTypes.Remainder => x % y,
                OperatorTypes.Equality => x == y,
                OperatorTypes.Inequality => x != y,
                OperatorTypes.LessThan => x < y,
                OperatorTypes.LessThanOrEqual => x <= y,
                OperatorTypes.GreaterThan => x > y,
                OperatorTypes.GreaterThanOrEqual => x >= y,
                _ => throw new NotSupportedException()
            };
        private object ApplyDefaultOperator(string x, string y) =>
            OperatorType switch
            {
                OperatorTypes.Addition => x + y,
                OperatorTypes.Inequality => x != y,
                OperatorTypes.Equality => x == y,
                _ => throw new NotSupportedException()
            };
        private string ApplyDefaultOperator(object x, string y) =>
            OperatorType switch
            {
                OperatorTypes.Addition => x + y,
                _ => throw new NotSupportedException()
            };
        private string ApplyDefaultOperator(string x, object y) =>
            OperatorType switch
            {
                OperatorTypes.Addition => x + y,
                _ => throw new NotSupportedException()
            };
        private bool? ApplyDefaultOperator(bool? x, bool? y) =>
            OperatorType switch
            {
                OperatorTypes.Inequality => x != y,
                OperatorTypes.Equality => x == y,
                _ => throw new NotSupportedException()
            };

        private bool? ApplyDefaultOperator(object x, object y) =>

            (x, y, OperatorType) switch
            {
                (null, not null, _) => ApplyDefaultOperator(y, x),

                (null, null, OperatorTypes.Inequality or OperatorTypes.Equality) => OperatorType == OperatorTypes.Equality,

                (not null, _, OperatorTypes.Inequality) => !x.Equals(y),
                (not null, _, OperatorTypes.Equality) => x.Equals(y),

                _ => throw new NotSupportedException()
            };

        public object DoEvaluate(object x, Func<object> evaluateRightOperand)
        {
            if (SupportedOperands == Operands.CompletelyCustom)
                return ApplyCustomOperator(x, evaluateRightOperand);

            var y = evaluateRightOperand();

            if (x == null && y == null)
                return EvaluateWithNullOperands();

            var @operator = GetUserDefinedOperator(out bool convertToDoubles, x, y);

            if (@operator == null)
            {
                // Fall back to predefined operator.
                // We simply apply any numeric operations with values converted to doubles.
                if ((SupportedOperands & Operands.Number) == Operands.Number &&
                    DoesImplicitConversionExist(x?.GetType(), typeof(double), true) &&
                    DoesImplicitConversionExist(y?.GetType(), typeof(double), true))
                    return ApplyDefaultOperator(
                        x == null ? null : (double?)DoImplicitConversion(x, typeof(double?)),
                        y == null ? null : (double?)DoImplicitConversion(y, typeof(double?)));

                if ((SupportedOperands & Operands.Boolean) == Operands.Boolean &&
                    // This operator supports two boolean arguments.
                    (x is bool or null) && (y is bool or null))
                    return ApplyDefaultOperator((bool?)x, (bool?)y);

                if (x is string || y is string)
                {
                    if ((SupportedOperands & Operands.String) == Operands.String && x is string xStr && y is string yStr)
                        // This operator supports two string arguments.
                        return ApplyDefaultOperator(xStr, yStr);

                    if ((SupportedOperands & Operands.StringObject) == Operands.StringObject)
                        // This operator supports one string and one object operand
                        return x is string xStr2 ? ApplyDefaultOperator(xStr2, y) : ApplyDefaultOperator(x, (string)y);
                }

                if ((SupportedOperands & Operands.Object) == Operands.Object)
                    // This operator supports any arbitrary operands.
                    return ApplyDefaultOperator(x, y);

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
                return @operator.Invoke(null, [x, y]);
            }
        }
        public object Evaluate(object x, object y) => DoEvaluate(x, () => y);
        public object Evaluate(AbstractSyntaxTree left, AbstractSyntaxTree right, CultureInfo cultureInfo, object[] bindingValues) => DoEvaluate(left.Evaluate(cultureInfo, bindingValues), () => right.Evaluate(cultureInfo, bindingValues));
    }
    /// <summary>
    /// The ternary "?:" operator.
    /// It is designed to work like the "?:" operator.
    /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#conditional-operator"/>
    /// </summary>
    public static class TernaryOperator
    {
        public static object Evaluate(AbstractSyntaxTree condition, AbstractSyntaxTree positive, AbstractSyntaxTree negative, CultureInfo cultureInfo, object[] bindingValues)
        {
            return condition.Evaluate(cultureInfo, bindingValues) is { } c ?
                (Operator.TryConvertToBool(c) switch
                {
                    true => positive,
                    false => negative,
                    _ => throw InvalidCondition(c)
                }).Evaluate(cultureInfo, bindingValues) :
                throw InvalidCondition(null);
        }

        private static InvalidOperationException InvalidCondition(object condition) => new($"Cannot apply operator '?:' when the first operand is {(condition == null ? "null" : $"of type '{condition.GetType().FullName}'")}");
    }
    public enum OperatorTypes
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
