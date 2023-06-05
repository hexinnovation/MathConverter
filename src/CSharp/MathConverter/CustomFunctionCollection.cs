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
            Add(CustomFunctionDefinition.Create<NowFunction>("Now"));
            Add(CustomFunctionDefinition.Create<CosFunction>("Cos"));
            Add(CustomFunctionDefinition.Create<SinFunction>("Sin"));
            Add(CustomFunctionDefinition.Create<TanFunction>("Tan"));
            Add(CustomFunctionDefinition.Create<AbsFunction>("Abs"));
            Add(CustomFunctionDefinition.Create<AcosFunction>("Acos"));
            Add(CustomFunctionDefinition.Create<AcosFunction>("ArcCos"));
            Add(CustomFunctionDefinition.Create<AsinFunction>("Asin"));
            Add(CustomFunctionDefinition.Create<AsinFunction>("ArcSin"));
            Add(CustomFunctionDefinition.Create<AtanFunction>("Atan"));
            Add(CustomFunctionDefinition.Create<AtanFunction>("ArcTan"));
            Add(CustomFunctionDefinition.Create<CeilingFunction>("Ceil"));
            Add(CustomFunctionDefinition.Create<CeilingFunction>("Ceiling"));
            Add(CustomFunctionDefinition.Create<FloorFunction>("Floor"));
            Add(CustomFunctionDefinition.Create<SqrtFunction>("Sqrt"));
            Add(CustomFunctionDefinition.Create<DegreesFunction>("Deg"));
            Add(CustomFunctionDefinition.Create<DegreesFunction>("Degrees"));
            Add(CustomFunctionDefinition.Create<RadiansFunction>("Rad"));
            Add(CustomFunctionDefinition.Create<RadiansFunction>("Radians"));
            Add(CustomFunctionDefinition.Create<ToLowerFunction>("ToLower"));
            Add(CustomFunctionDefinition.Create<ToLowerFunction>("LCase"));
            Add(CustomFunctionDefinition.Create<ToUpperFunction>("ToUpper"));
            Add(CustomFunctionDefinition.Create<ToUpperFunction>("UCase"));
#if !XAMARIN
            Add(CustomFunctionDefinition.Create<VisibleOrCollapsedFunction>("VisibleOrCollapsed"));
            Add(CustomFunctionDefinition.Create<VisibleOrHiddenFunction>("VisibleOrHidden"));
#endif
            Add(CustomFunctionDefinition.Create<TryParseDoubleFunction>("TryParseDouble"));
            Add(CustomFunctionDefinition.Create<GetTypeFunction>("GetType"));
            Add(CustomFunctionDefinition.Create<StartsWithFunction>("StartsWith"));
            Add(CustomFunctionDefinition.Create<ConvertTypeFunction>("ConvertType"));
            Add(CustomFunctionDefinition.Create<EnumEqualsFunction>("EnumEquals"));
            Add(CustomFunctionDefinition.Create<ContainsFunction>("Contains"));
            Add(CustomFunctionDefinition.Create<EndsWithFunction>("EndsWith"));
            Add(CustomFunctionDefinition.Create<LogFunction>("Log"));
            Add(CustomFunctionDefinition.Create<Atan2Function>("Atan2"));
            Add(CustomFunctionDefinition.Create<Atan2Function>("ArcTan2"));
            Add(CustomFunctionDefinition.Create<IsNullFunction>("IsNull"));
            Add(CustomFunctionDefinition.Create<IsNullFunction>("IfNull"));
            Add(CustomFunctionDefinition.Create<RoundFunction>("Round"));
            Add(CustomFunctionDefinition.Create<AndFunction>("And"));
            Add(CustomFunctionDefinition.Create<NorFunction>("Nor"));
            Add(CustomFunctionDefinition.Create<OrFunction>("Or"));
            Add(CustomFunctionDefinition.Create<MaxFunction>("Max"));
            Add(CustomFunctionDefinition.Create<MinFunction>("Min"));
            Add(CustomFunctionDefinition.Create<AverageFunction>("Avg"));
            Add(CustomFunctionDefinition.Create<AverageFunction>("Average"));
            Add(CustomFunctionDefinition.Create<FormatFunction>("Format"));
            Add(CustomFunctionDefinition.Create<ConcatFunction>("Concat"));
            Add(CustomFunctionDefinition.Create<JoinFunction>("Join"));
            Add(CustomFunctionDefinition.Create<ThrowFunction>("Throw"));
            Add(CustomFunctionDefinition.Create<UnsetValueFunction>("UnsetValue"));
            Add(CustomFunctionDefinition.Create<TryCatchFunction>("TryCatch"));
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
        /// <summary>
        /// The name of the function. For example, if we choose "MyCustomFunction",
        /// you can invoke the function like "MyCustomFunction(x)"
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The type of the function. This type must extend <see cref="CustomFunction"/>
        /// </summary>
        public Type Function { get; set; }

        public static CustomFunctionDefinition Create<T>(string name)
            where T : CustomFunction
        {
            return new CustomFunctionDefinition { Name = name, Function = typeof(T) };
        }
    }
}
