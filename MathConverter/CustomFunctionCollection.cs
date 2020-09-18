using System;
using System.Collections;
using System.Collections.Generic;
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
                    _functions[item.Name] = item.Function;
                }
            }
        }
        public void Clear()
        {
            _functions.Clear();
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
