// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Defines a single element in a vertex.
    /// </summary>
    public partial struct VertexElement : IEquatable<VertexElement>
    {
        /// <summary>
        /// Gets or sets the offset in bytes from the beginning of the stream to the vertex element.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets the data format.
        /// </summary>
        public VertexElementFormat VertexElementFormat { get; set; }

        /// <summary>
        /// Gets or sets the HLSL semantic of the element in the vertex shader input.
        /// </summary>
        public VertexElementUsage VertexElementUsage { get; set; }

        /// <summary>
        /// Gets or sets the semantic index.
        /// <para>Required if the semantic is used for more than one vertex element.</para>
        /// </summary>
        /// <remarks>
        /// Usage indices in a vertex declaration usually start with 0. When multiple vertex buffers
        /// are bound to the input assembler stage (see <see cref="GraphicsDevice.SetVertexBuffers"/>),
        /// the usage indices are internally adjusted based on the order in which the vertex buffers are bound.
        /// </remarks>
        public int UsageIndex { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexElement"/> struct.
        /// </summary>
        /// <param name="offset">The offset in bytes from the beginning of the stream to the vertex element.</param>
        /// <param name="elementFormat">The element format.</param>
        /// <param name="elementUsage">The HLSL semantic of the element in the vertex shader input-signature.</param>
        /// <param name="usageIndex">The semantic index, which is required if the semantic is used for more than one vertex element.</param>
        public VertexElement(
            int offset, VertexElementFormat elementFormat, VertexElementUsage elementUsage, int usageIndex)
        {
            Offset = offset;
            VertexElementFormat = elementFormat;
            VertexElementUsage = elementUsage;
            UsageIndex = usageIndex;
        }

        /// <summary>
        /// Compares two <see cref="VertexElement"/> instances to determine whether they are the same.
        /// </summary>
        public static bool operator ==(in VertexElement a, in VertexElement b)
        {
            return a.Offset == b.Offset
                && a.VertexElementFormat == b.VertexElementFormat
                && a.VertexElementUsage == b.VertexElementUsage
                && a.UsageIndex == b.UsageIndex;
        }

        /// <summary>
        /// Compares two <see cref="VertexElement"/> instances to determine whether they are different.
        /// </summary>
        public static bool operator !=(in VertexElement a, in VertexElement b) => !(a == b);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        public override bool Equals(object obj) => obj is VertexElement other && Equals(other);

        /// <summary>
        /// Determines whether the specified <see cref="VertexElement"/> is equal to this instance.
        /// </summary>
        public bool Equals(VertexElement other) => this == other;

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        public override string ToString() =>
            "{Offset:" + Offset + " Format:" + VertexElementFormat +
            " Usage:" + VertexElementUsage + " UsageIndex: " + UsageIndex + "}";

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            // Optimized hash:
            // - DirectX 11 has max 32 registers. A register is max 16 byte. _offset is in the range
            //   0 to 512 (exclusive). --> _offset needs 9 bit.
            // - VertexElementFormat has 12 values. --> _format needs 4 bit.
            // - VertexElementUsage has 13 values. --> _usage needs 4 bit.
            // - DirectX 11 has max 32 registers. --> _usageIndex needs 6 bit.
            // (Note: If these assumptions are correct we get a unique hash code. If these 
            // assumptions are not correct, we still get a useful hash code because we use XOR.)
            int hashCode = Offset;
            hashCode ^= (int)VertexElementFormat << 9;
            hashCode ^= (int)VertexElementUsage << (9 + 4);
            hashCode ^= UsageIndex << (9 + 4 + 4);
            return hashCode;
        }
    }
}
