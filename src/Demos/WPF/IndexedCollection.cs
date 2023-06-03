#define NOTIFY_CHANGE

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MathConverterDemo
{
    /// <summary>
    /// A collection of <see cref="IndexedCollection&lt;T&gt;"/>s. This class handles maintaining the <see cref="IndexedElement&lt;T&gt;.Index"/> property of its elements.
    /// DO NOT ADD THE SAME EXACT <see cref="IndexedElement&lt;T&gt;"/> to multiple <see cref="IndexedCollection&lt;T&gt;"/>s.
    /// </summary>
    /// <typeparam name="T">The type of indexed elements we are going to store.</typeparam>
    public class IndexedCollection<T> : ObservableCollection<IndexedElement<T>>
    {
        /// <summary>
        /// Creates and initializes a new <see cref="IndexedCollection&lt;T&gt;"/> with the default values for all properties.
        /// </summary>
        public IndexedCollection()
        {

        }
        /// <summary>
        /// Adds an object to the end of the <see cref="IndexedCollection&lt;T&gt;"/>
        /// </summary>
        /// <param name="item">The item to be added at the end of the <see cref="IndexedCollection&lt;T&gt;"/>. This value can be null.</param>
        public void Add(T item)
        {
            this.Add(new IndexedElement<T>(item));
        }
        /// <summary>
        ///     Inserts an element into the <see cref="IndexedCollection&lt;T&gt;"/>
        ///     at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        public void Insert(int index, T item)
        {
            this.Insert(index, new IndexedElement<T>(item));
        }
        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        public void SetItem(int index, T item)
        {
            this.SetItem(index, new IndexedElement<T>(item));
        }
        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the item should be inserted</param>
        /// <param name="item">The object to insert</param>
        protected override void InsertItem(int index, IndexedElement<T> item)
        {
            base.InsertItem(index, item);

            for (int i = index; i < Count; i++)
            {
                this[i].Index = i;
            }
        }
        /// <summary>
        /// Moves the item at the specified index to a new location in the collection.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
        /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            base.MoveItem(oldIndex, newIndex);

            if (oldIndex > newIndex)
            {
                while (oldIndex >= newIndex)
                {
                    this[oldIndex].Index = oldIndex--;
                }
            }
            else
            {
                while (oldIndex <= newIndex)
                {
                    this[oldIndex].Index = oldIndex++;
                }
            }
        }
        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            for (int i = index; i < Count; i++)
            {
                Items[i].Index = i;
            }
        }
        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, IndexedElement<T> item)
        {
            item.Index = index;
            base.SetItem(index, item);
        }
    }
    /// <summary>
    /// An element of an <see cref="IndexedCollection&lt;T&gt;"/>
    /// </summary>
    /// <typeparam name="T">The type of value</typeparam>
    public class IndexedElement<T>
#if NOTIFY_CHANGE
        : INotifyPropertyChanged
#endif
    {
        /// <summary>
        /// Creates a new <see cref="IndexedElement&lt;T&gt;"/> with the default values for all properties
        /// </summary>
        public IndexedElement(T Value)
        {
            _index = -1;
            _value = Value;
        }

        #region public T Value { get; set; }
        /// <summary>
        /// Gets or sets the Value
        /// </summary>
#if !NOTIFY_CHANGE
        public T Value { get; set; }
#else
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    RaisePropertyChangedEvent("Value");
                }
            }
        } T _value;
#endif
        #endregion
        #region public int Index { get; set; }
        /// <summary>
        /// Gets or sets the Index
        /// </summary>
#if !NOTIFY_CHANGE
        public int Index { get; set; }
#else
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    RaisePropertyChangedEvent("Index");
                }
            }
        } int _index;
#endif
        #endregion

#if NOTIFY_CHANGE
        #region public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="PropertyName">The name of the property whose value is changing</param>
        private void RaisePropertyChangedEvent(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
        /// <summary>
        /// An event that we raise whenever a property's value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
#endif
    }
}