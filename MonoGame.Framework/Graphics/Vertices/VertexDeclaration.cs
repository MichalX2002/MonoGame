// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using MonoGame.Utilities;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Defines per-vertex data of a vertex buffer.
    /// </summary>
    /// <remarks>
    /// <see cref="VertexDeclaration"/> implements <see cref="IEquatable{T}"/> and can be used as
    /// a key in a dictionary. Two vertex declarations are considered equal if the vertices are
    /// structurally equivalent, i.e. the vertex elements and the vertex stride are identical. (The
    /// properties <see cref="GraphicsResource.Name"/> and <see cref="GraphicsResource.Tag"/> are
    /// ignored in <see cref="GetHashCode"/> and <see cref="Equals(VertexDeclaration)"/>!)
    /// </remarks>
    public partial class VertexDeclaration : GraphicsResource, IEquatable<VertexDeclaration>
    {
        // Note for future refactoring:
        // For XNA-compatibility VertexDeclaration is derived from GraphicsResource, which means it
        // has GraphicsDevice, Name, Tag and implements IDisposable. This is unnecessary in
        // MonoGame. VertexDeclaration.GraphicsDevice is never set.
        // --> VertexDeclaration should be a lightweight immutable type. No base class, no IDisposable.
        //     (Use the internal type Data. Do not expose a constructor. Use a factory method to
        //     cache the vertex declarations.)

        #region ----- Data shared between structurally identical vertex declarations -----

        private sealed class Data : IEquatable<Data>
        {
            private readonly int _hashCode;

            public readonly int VertexStride;
            public VertexElement[] Elements;

            public Data(int vertexStride, VertexElement[] elements)
            {
                VertexStride = vertexStride;
                Elements = elements;

                // Pre-calculate hash code for fast comparisons and lookup in dictionaries.
                unchecked
                {
                    _hashCode = 7 + elements[0].GetHashCode();
                    for (int i = 1; i < elements.Length; i++)
                        _hashCode = _hashCode * 31 + elements[i].GetHashCode();

                    _hashCode = _hashCode * 31 + elements.Length;
                    _hashCode = _hashCode * 31 + vertexStride;
                }
            }

            
            public bool Equals(Data other)
            {
                if (other is null)
                    return false;

                if (ReferenceEquals(this, other))
                    return true;

                if (_hashCode != other._hashCode ||
                    VertexStride != other.VertexStride ||
                    Elements.Length != other.Elements.Length)
                    return false;

                for (int i = 0; i < Elements.Length; i++)
                    if (!Elements[i].Equals(other.Elements[i]))
                        return false;

                return true;
            }

            public override bool Equals(object obj) => Equals(obj as Data);

            public override int GetHashCode() => _hashCode;
        }
        #endregion


        #region ----- VertexDeclaration Cache -----

        private static readonly Dictionary<Data, VertexDeclaration> _vertexDeclarationCache;

        static VertexDeclaration()
        {
            _vertexDeclarationCache = new Dictionary<Data, VertexDeclaration>();
        }

        internal static VertexDeclaration GetOrCreate(int vertexStride, VertexElement[] elements)
        {
            lock (_vertexDeclarationCache)
            {
                var data = new Data(vertexStride, elements);
                if (!_vertexDeclarationCache.TryGetValue(data, out VertexDeclaration vertexDeclaration))
                {
                    // Data.Elements have already been set in the Data ctor. However, entries
                    // in the vertex declaration cache must be immutable. Therefore, we create a 
                    // copy of the array, which the user cannot access.
                    data.Elements = (VertexElement[])elements.Clone();

                    vertexDeclaration = new VertexDeclaration(data);
                    _vertexDeclarationCache[data] = vertexDeclaration;
                }

                return vertexDeclaration;
            }
        }

        private VertexDeclaration(Data data)
        {
            _data = data;
        }

        #endregion

        private readonly Data _data;

        /// <summary>
        /// Gets the internal vertex elements array.
        /// </summary>
        /// <value>The internal vertex elements array.</value>
        internal VertexElement[] InternalVertexElements => _data.Elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexDeclaration"/> class.
        /// </summary>
        /// <param name="elements">The vertex elements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="elements"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentEmptyException"><paramref name="elements"/> is empty.</exception>
        public VertexDeclaration(params VertexElement[] elements) : this(GetVertexStride(elements), elements)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexDeclaration"/> class.
        /// </summary>
        /// <param name="vertexStride">The size of a vertex (including padding) in bytes.</param>
        /// <param name="elements">The vertex elements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="elements"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentEmptyException"><paramref name="elements"/> is empty.</exception>
        public VertexDeclaration(int vertexStride, params VertexElement[] elements)
        {
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            if (elements.Length == 0) throw new ArgumentEmptyException(nameof(elements));

            lock (_vertexDeclarationCache)
            {
                var data = new Data(vertexStride, elements);
                if (_vertexDeclarationCache.TryGetValue(data, out var vertexDeclaration))
                {
                    // Reuse existing data.
                    _data = vertexDeclaration._data;
                }
                else
                {
                    // Cache new vertex declaration.
                    data.Elements = (VertexElement[])elements.Clone();
                    _data = data;
                    _vertexDeclarationCache[data] = this;
                }
            }
        }

        private static int GetVertexStride(VertexElement[] elements)
		{
			int max = 0;
			for (var i = 0; i < elements.Length; i++)
			{
                var start = elements[i].Offset + elements[i].VertexElementFormat.GetSize();
				if (max < start)
					max = start;
			}
			return max;
		}

        /// <summary>
        /// Returns the <see cref="VertexDeclaration"/> for Type.
        /// </summary>
        /// <param name="vertexType">A value type which implements the <see cref="IVertexType"/> interface.</param>
        /// <remarks>
        /// Prefer to use <see cref="VertexDeclarationCache{T}"/> when the declaration lookup
        /// can be performed with a templated type.
        /// </remarks>
		internal static VertexDeclaration FromType(Type vertexType)
		{
			if (vertexType == null)
				throw new ArgumentNullException(nameof(vertexType));

            if (!ReflectionHelpers.IsValueType(vertexType))
				throw new ArgumentException("Must be value type.", nameof(vertexType));

            var type = Activator.CreateInstance(vertexType) as IVertexType;
            var vertexDeclaration = type.VertexDeclaration;

            if (type == null)
				throw new ArgumentException(
                    $"{nameof(vertexType)} does not inherit {nameof(IVertexType)}.", nameof(vertexType));

			if (vertexDeclaration == null)
				throw new Exception($"{nameof(IVertexType)}.{nameof(IVertexType.VertexDeclaration)} cannot be null.");

			return vertexDeclaration;
		}

        /// <summary>
        /// Creates a copy of the vertex elements.
        /// </summary>
        public VertexElement[] GetVertexElements() => (VertexElement[])_data.Elements.Clone();

        /// <summary>
        /// Gets the size of a vertex (including padding) in bytes.
        /// </summary>
        public int VertexStride => _data.VertexStride;

        /// <summary>
        /// Determines whether the specified <see cref="VertexDeclaration"/> is equal to this instance.
        /// </summary>
        public bool Equals(VertexDeclaration other) => other != null && ReferenceEquals(_data, other._data);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        public override bool Equals(object obj) => Equals(obj as VertexDeclaration);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        public override int GetHashCode() => _data.GetHashCode();

        /// <summary>
        /// Compares two <see cref="VertexElement"/> instances to determine whether they are the same.
        /// </summary>
        public static bool operator ==(VertexDeclaration a, VertexDeclaration b) => Equals(a, b);

        /// <summary>
        /// Compares two <see cref="VertexElement"/> instances to determine whether they are different.
        /// </summary>
        public static bool operator !=(VertexDeclaration a, VertexDeclaration b) => !(a == b);
    }
}
