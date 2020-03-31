// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Graphics
{
    public partial class VertexDeclaration
    {
        /// <summary>
        /// Data shared between structurally identical vertex declarations.
        /// </summary>
        private class Data : IEquatable<Data>
        {
            private readonly int _hashCode;

            public readonly int VertexStride;
            public readonly ReadOnlyMemory<VertexElement> Elements;

            public Data(int vertexStride, ReadOnlyMemory<VertexElement> elements)
            {
                VertexStride = vertexStride;
                Elements = elements;

                // Pre-calculate hash code for fast comparisons and lookup in dictionaries.
                unchecked
                {
                    var e = Elements.Span;
                    _hashCode = 7 + e[0].GetHashCode();
                    for (int i = 1; i < e.Length; i++)
                        _hashCode = _hashCode * 31 + e[i].GetHashCode();

                    _hashCode = _hashCode * 31 + e.Length;
                    _hashCode = _hashCode * 31 + vertexStride;
                }
            }


            public bool Equals(Data other)
            {
                if (ReferenceEquals(this, other))
                    return true;

                if (_hashCode != other._hashCode ||
                    VertexStride != other.VertexStride ||
                    Elements.Length != other.Elements.Length)
                    return false;

                return Elements.Span.SequenceEqual(other.Elements.Span);
            }

            public override bool Equals(object obj)
            {
                return obj is Data other && Equals(other);
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
        }
    }
}
