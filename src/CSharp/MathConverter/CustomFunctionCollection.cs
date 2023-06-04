using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HexInnovation
{
    public class CustomFunctionCollection : ICollection<CustomFunctionDefinition>, IList
    {
        private readonly Dictionary<string, Type> _functions = new Dictionary<string, Type>();
        public int Count => _functions.Count;
        public bool IsReadOnly => false;

        public void Add(CustomFunctionDefinition item)
        {
            if (item == null)
            {
                throw new NullReferenceException($"The {nameof(CustomFunctionDefinition)} cannot be null.");
            }
            else if (item.Function == null || !typeof(CustomFunction).IsAssignableFrom(item.Function))
            {
                throw new NullReferenceException($"The {nameof(CustomFunctionDefinition.Function)} property must be an instance of {nameof(CustomFunction)}.");
            }
            else
            {
                if (item.Name == null)
                {
                    throw new NullReferenceException($"The {nameof(CustomFunctionDefinition.Name)} property must not be null.");
                }
                else if (_functions.ContainsKey(item.Name))
                {
                    throw new ArgumentException($"A function with the name \"{item.Name}\" has already been added.");
                }
                else
                {
                    switch (item.Name)
                    {
                        case "e":
                        case "pi":
                        case "null":
                        case "true":
                        case "x":
                        case "y":
                        case "z":
                            throw new ArgumentException($"\"{item.Name}\" is a reserved keyword. You cannot add a function with that name.");
                    }
                    _functions[item.Name] = item.Function;
                }
            }
        }
        public void Clear()
        {
            _functions.Clear();

#if WINDOWS_UWP
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
#elif !NETSTANDARD1_0 && !NETSTANDARD1_3
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
#endif
            {
                // If we create a MathConverter in XAML where we add a CustomFunctionDefinition to the MathConverter, then the designer calls Clear(), and then
                // we get design-time exceptions any time we reference any default function. So if the designer calls Clear(), we will register the default functions again.
#if !NETSTANDARD1_0 && !NETSTANDARD1_3

                RegisterDefaultFunctions();
#endif
            }
        }
        public void RegisterDefaultFunctions()
        {
            Add(new CustomFunctionDefinition { Name = "Now", Function = typeof(NowFunction) });
            Add(new CustomFunctionDefinition { Name = "Cos", Function = typeof(CosFunction) });
            Add(new CustomFunctionDefinition { Name = "Sin", Function = typeof(SinFunction) });
            Add(new CustomFunctionDefinition { Name = "Tan", Function = typeof(TanFunction) });
            Add(new CustomFunctionDefinition { Name = "Abs", Function = typeof(AbsFunction) });
            Add(new CustomFunctionDefinition { Name = "Acos", Function = typeof(AcosFunction) });
            Add(new CustomFunctionDefinition { Name = "ArcCos", Function = typeof(AcosFunction) });
            Add(new CustomFunctionDefinition { Name = "Asin", Function = typeof(AsinFunction) });
            Add(new CustomFunctionDefinition { Name = "ArcSin", Function = typeof(AsinFunction) });
            Add(new CustomFunctionDefinition { Name = "Atan", Function = typeof(AtanFunction) });
            Add(new CustomFunctionDefinition { Name = "ArcTan", Function = typeof(AtanFunction) });
            Add(new CustomFunctionDefinition { Name = "Ceil", Function = typeof(CeilingFunction) });
            Add(new CustomFunctionDefinition { Name = "Ceiling", Function = typeof(CeilingFunction) });
            Add(new CustomFunctionDefinition { Name = "Floor", Function = typeof(FloorFunction) });
            Add(new CustomFunctionDefinition { Name = "Sqrt", Function = typeof(SqrtFunction) });
            Add(new CustomFunctionDefinition { Name = "Deg", Function = typeof(DegreesFunction) });
            Add(new CustomFunctionDefinition { Name = "Degrees", Function = typeof(DegreesFunction) });
            Add(new CustomFunctionDefinition { Name = "Rad", Function = typeof(RadiansFunction) });
            Add(new CustomFunctionDefinition { Name = "Radians", Function = typeof(RadiansFunction) });
            Add(new CustomFunctionDefinition { Name = "ToLower", Function = typeof(ToLowerFunction) });
            Add(new CustomFunctionDefinition { Name = "LCase", Function = typeof(ToLowerFunction) });
            Add(new CustomFunctionDefinition { Name = "ToUpper", Function = typeof(ToUpperFunction) });
            Add(new CustomFunctionDefinition { Name = "UCase", Function = typeof(ToUpperFunction) });
#if !XAMARIN
            Add(new CustomFunctionDefinition { Name = "VisibleOrCollapsed", Function = typeof(VisibleOrCollapsedFunction) });
            Add(new CustomFunctionDefinition { Name = "VisibleOrHidden", Function = typeof(VisibleOrHiddenFunction) });
#endif
            Add(new CustomFunctionDefinition { Name = "TryParseDouble", Function = typeof(TryParseDoubleFunction) });
            Add(new CustomFunctionDefinition { Name = "GetType", Function = typeof(GetTypeFunction) });
            Add(new CustomFunctionDefinition { Name = "StartsWith", Function = typeof(StartsWithFunction) });
            Add(new CustomFunctionDefinition { Name = "ConvertType", Function = typeof(ConvertTypeFunction) });
            Add(new CustomFunctionDefinition { Name = "Contains", Function = typeof(ContainsFunction) });
            Add(new CustomFunctionDefinition { Name = "EndsWith", Function = typeof(EndsWithFunction) });
            Add(new CustomFunctionDefinition { Name = "Log", Function = typeof(LogFunction) });
            Add(new CustomFunctionDefinition { Name = "Atan2", Function = typeof(Atan2Function) });
            Add(new CustomFunctionDefinition { Name = "ArcTan2", Function = typeof(Atan2Function) });
            Add(new CustomFunctionDefinition { Name = "IsNull", Function = typeof(IsNullFunction) });
            Add(new CustomFunctionDefinition { Name = "IfNull", Function = typeof(IsNullFunction) });
            Add(new CustomFunctionDefinition { Name = "Round", Function = typeof(RoundFunction) });
            Add(new CustomFunctionDefinition { Name = "And", Function = typeof(AndFunction) });
            Add(new CustomFunctionDefinition { Name = "Nor", Function = typeof(NorFunction) });
            Add(new CustomFunctionDefinition { Name = "Or", Function = typeof(OrFunction) });
            Add(new CustomFunctionDefinition { Name = "Max", Function = typeof(MaxFunction) });
            Add(new CustomFunctionDefinition { Name = "Min", Function = typeof(MinFunction) });
            Add(new CustomFunctionDefinition { Name = "Avg", Function = typeof(AverageFunction) });
            Add(new CustomFunctionDefinition { Name = "Average", Function = typeof(AverageFunction) });
            Add(new CustomFunctionDefinition { Name = "Format", Function = typeof(FormatFunction) });
            Add(new CustomFunctionDefinition { Name = "Concat", Function = typeof(ConcatFunction) });
            Add(new CustomFunctionDefinition { Name = "Join", Function = typeof(JoinFunction) });
            Add(new CustomFunctionDefinition { Name = "Throw", Function = typeof(ThrowFunction) });
            Add(new CustomFunctionDefinition { Name = "UnsetValue", Function = typeof(UnsetValueFunction) });
        }
        public bool Contains(CustomFunctionDefinition item)
        {
            if (item == null)
                throw new NullReferenceException($"The {nameof(CustomFunctionDefinition)} must not be null.");
            return _functions.TryGetValue(item.Name, out var @type) && type == item.Function;
        }
        private IEnumerable<CustomFunctionDefinition> ToIEnumerable()
        {
            return _functions.Select(x => new CustomFunctionDefinition { Name = x.Key, Function = x.Value });
        }
        public void CopyTo(CustomFunctionDefinition[] array, int arrayIndex)
        {
            CopyTo((Array)array, arrayIndex);
        }
        public IEnumerator<CustomFunctionDefinition> GetEnumerator()
        {
            return ToIEnumerable().GetEnumerator();
        }
        public bool Remove(CustomFunctionDefinition item)
        {
            if (Contains(item))
            {
                // ReSharper disable once PossibleNullReferenceException => if item is null, Contains(item) will throw.
                return _functions.Remove(item.Name);
            }
            else
            {
                return false;
            }
        }
        public bool Remove(string functionName)
        {
            return _functions.Remove(functionName);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ToIEnumerable().GetEnumerator();
        }

        public bool TryGetFunction(string functionName, out CustomFunction function)
        {
            if (_functions.TryGetValue(functionName, out var type))
            {
                function = Activator.CreateInstance(type) as CustomFunction;

                if (function != null)
                {
                    function.FunctionName = functionName;
                }
            }
            else
            {
                function = null;
            }

            return function != null;
        }

        public object SyncRoot => this;
        public bool IsSynchronized => false;
        public bool IsFixedSize => false;

        public object this[int index]
        {
            get => ToIEnumerable().Skip(index).First();
            set => throw new NotSupportedException();
        }

        public void CopyTo(Array array, int index)
        {
            Array.Copy(ToIEnumerable().ToArray(), 0, array, index, Count);
        }

        public int Add(object value)
        {
            if (value is CustomFunctionDefinition x)
            {
                Add(x);
                return Count - 1;
            }
            else
                throw new ArgumentException("You can only add {CustomFunctionDefinition} objects.", nameof(value));
        }

        public bool Contains(object value)
        {
            if (value is CustomFunctionDefinition x)
                return Contains(x);
            else
                return false;
        }

        public int IndexOf(object value)
        {
            if (value is CustomFunctionDefinition x)
                return IndexOf(x);
            else
                return -1;
        }

        public void Insert(int index, object value)
        {
            Add(value);
        }

        public void Remove(object value)
        {
            if (value is CustomFunctionDefinition x)
                Remove(x);
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
    }
    public class CustomFunctionDefinition
    {
        public string Name { get; set; }
        public Type Function { get; set; }
    }
}
