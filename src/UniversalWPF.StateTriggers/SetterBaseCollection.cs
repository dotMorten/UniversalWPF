using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UniversalWPF
{
    /// <summary>
    /// Represents a collection of objects that inherit from <see cref="SetterBase"/>.
    /// </summary>
    public class SetterBaseCollection : IEnumerable<SetterBase>, IList<SetterBase>, IList
    {
        private List<SetterBase> _items = new List<SetterBase>();

        public SetterBase this[int index]
        {
            get => _items[index];
            set
            {
                _items[index] = value;
                CollectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc cref="ICollection.Count" />
        public int Count => _items.Count;

        /// <inheritdoc cref="IList.IsReadOnly" />
        public bool IsReadOnly => false;

        /// <inheritdoc cref="ICollection{T}.Add(T)" />
        public void Add(SetterBase item)
        {
            _items.Add(item);
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc cref="IList.Clear()" />
        public void Clear()
        {
            _items.Clear();
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc cref="ICollection{T}.Contains(T)" />
        public bool Contains(SetterBase item) => _items.Contains(item);

        /// <inheritdoc cref="ICollection{T}.CopyTo(T[], int)" />
        public void CopyTo(SetterBase[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        /// <inheritdoc cref="IList{T}.IndexOf(T)" />
        public int IndexOf(SetterBase item) => _items.IndexOf(item);

        /// <inheritdoc cref="IList{T}.Insert(int, T)" />
        public void Insert(int index, SetterBase item)
        {
            _items.Insert(index, item);
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc cref="ICollection{T}.Remove(T)" />
        public bool Remove(SetterBase item)
        {
            bool result = _items.Remove(item);
            if (result)
                CollectionChanged?.Invoke(this, EventArgs.Empty);
            return result;
        }

        /// <inheritdoc cref="IList.RemoveAt(int)" />
        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        IEnumerator<SetterBase> IEnumerable<SetterBase>.GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();



        bool IList.IsReadOnly => false;

        bool IList.IsFixedSize => false;

        int ICollection.Count => Count;

        object ICollection.SyncRoot => null;

        bool ICollection.IsSynchronized => false;

        object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        int IList.Add(object value)
        {
            Add((SetterBase)value);
            return Count - 1;
        }

        bool IList.Contains(object value) => Contains((SetterBase)value);

        int IList.IndexOf(object value) => IndexOf((SetterBase)value);

        void IList.Insert(int index, object value) => Insert(index, (SetterBase)value);

        void IList.Remove(object value) => Remove((SetterBase)value);

        void IList.RemoveAt(int index) => RemoveAt(index);

        void ICollection.CopyTo(Array array, int index) => ((IList)_items).CopyTo(array, index);

        internal event EventHandler CollectionChanged;
    }
}
