// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace MonoGame.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides methods and properties for maintaining a vertex channel.
    /// This is a generic implementation of <see cref="VertexChannel"/> and, therefore, can handle strongly typed content data.
    /// </summary>
    public sealed class VertexChannel<T> : VertexChannel, IList<T>, IReadOnlyList<T>, ICollection<T>, IEnumerable<T>
    {
        private List<T> _items;

        /// <summary>
        /// Gets the strongly-typed list for the base class to access.
        /// </summary>
        internal override IList Items => _items;

        /// <summary>
        /// Gets the type of data contained in this channel.
        /// </summary>
        public override Type ElementType => typeof(T);

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        public new T this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        /// <summary>
        /// true if this object is read-only; false otherwise.
        /// </summary>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// Creates an instance of <see cref="VertexChannel"/>.
        /// </summary>
        /// <param name="name">Name of the channel.</param>
        internal VertexChannel(string name) : base(name)
        {
            _items = new List<T>();
        }

        static VertexChannel()
        {
            // Some platforms (such as Windows Store) don't support TypeConverter, which
            // is normally referenced with an attribute on the target type. To keep them
            // out of the main assembly, they are registered here before their use.

            //TypeDescriptor.AddAttributes(typeof(Single), new TypeConverterAttribute(typeof(SingleTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(Vector2), new TypeConverterAttribute(typeof(Vector2TypeConverter)));
            TypeDescriptor.AddAttributes(typeof(Vector3), new TypeConverterAttribute(typeof(Vector3TypeConverter)));
            TypeDescriptor.AddAttributes(typeof(Vector4), new TypeConverterAttribute(typeof(Vector4TypeConverter)));
            //TypeDescriptor.AddAttributes(typeof(IPackedVector), new TypeConverterAttribute(typeof(PackedVectorTypeConverter)));
        }

        /// <summary>
        /// Determines whether the specified element is in the channel.
        /// </summary>
        /// <param name="item">Element being searched for.</param>
        /// <returns>true if the element is present; false otherwise.</returns>
        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the channel to an array, starting at the specified index.
        /// </summary>
        /// <param name="array">Array that will receive the copied channel elements.</param>
        /// <param name="arrayIndex">Starting index for copy operation.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets an enumerator interface for reading channel content.
        /// </summary>
        /// <returns>Enumeration of the channel content.</returns>
        public new IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Gets the index of the specified item.
        /// </summary>
        /// <param name="item">Item whose index is to be retrieved.</param>
        /// <returns>Index of specified item.</returns>
        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        /// <summary>
        /// Inserts the range of values from the enumerable into the channel.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="data">The data to insert into the channel.</param>
        internal override void InsertRange(int index, IEnumerable data)
        {
            if ((index < 0) || (index > _items.Count))
                throw new ArgumentOutOfRangeException(nameof(index));
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (!(data is IEnumerable<T>))
                throw new ArgumentException("Value does not implement generic enumerable.", nameof(data));

            _items.InsertRange(index, (IEnumerable<T>)data);
        }

        /// <summary>
        /// Reads channel content and automatically converts it to the specified vector format.
        /// </summary>
        /// <typeparam name="TTarget">Target vector format for the converted channel data.</typeparam>
        /// <returns>The converted channel data.</returns>
        public override IEnumerable<TTarget> ReadConvertedContent<TTarget>()
        {
            if (typeof(TTarget).IsAssignableFrom(typeof(T)))
                return _items.Cast<TTarget>();

            return Convert<TTarget>(_items);
        }

        private static IEnumerable<TTarget> Convert<TTarget>(IEnumerable<T> items)
        {
            // The following formats are supported:
            // - Single
            // - Vector2 Structure
            // - Vector3 Structure
            // - Vector4 Structure
            // - Any implementation of IPackedVector Interface.

            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (!converter.CanConvertTo(typeof(TTarget)))
            {
                // If you got this exception, check out the static constructor above
                // to make sure your type is registered.
                throw new NotImplementedException(
                    string.Format("TypeConverter for {0} -> {1} is not implemented.",
                    typeof(T).Name, typeof(TTarget).Name));
            }

            foreach (var item in items)
                yield return (TTarget)converter.ConvertTo(item, typeof(TTarget));
        }

        /// <summary>
        /// Adds a new element to the end of the collection.
        /// </summary>
        /// <param name="value">The element to add.</param>
        void ICollection<T>.Add(T value)
        {
            ((ICollection<T>)Items).Add(value);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        void ICollection<T>.Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Removes a specified element from the collection.
        /// </summary>
        /// <param name="value">The element to remove.</param>
        /// <returns>true if the channel was removed; false otherwise.</returns>
        bool ICollection<T>.Remove(T value)
        {
            return ((ICollection<T>)Items).Remove(value);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified position.
        /// </summary>
        /// <param name="index">Index at which to insert the element.</param>
        /// <param name="value">The element to insert.</param>
        void IList<T>.Insert(int index, T value)
        {
            Items.Insert(index, value);
        }

        /// <summary>
        /// Removes the element at the specified index position.
        /// </summary>
        /// <param name="index">Index of the element to remove.</param>
        void IList<T>.RemoveAt(int index)
        {
            Items.RemoveAt(index);
        }

        /// <summary>
        /// Removes a range of values from the channel.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count"> The number of elements to remove.</param>
        internal override void RemoveRange(int index, int count)
        {
            _items.RemoveRange(index, count);
        }
    }
}
