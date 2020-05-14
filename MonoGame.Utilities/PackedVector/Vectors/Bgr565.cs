// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing unsigned XYZ components.
    /// The XZ components use 5 bits each, and the Y component uses 6 bits.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr565 : IPackedPixel<Bgr565, ushort>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Blue, 5),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Green, 6),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Red, 5));

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        /// <param name="alpha">The alpha component.</param>
        [CLSCompliant(false)]
        public Bgr565(ushort packed)
        {
            PackedValue = packed;
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            scaledVector *= 31;
            scaledVector += Vector4.Half;
            scaledVector.Clamp(Vector4.Zero, Vector4.One);

            PackedValue = (ushort)(
                (((int)scaledVector.X & 0x1F) << 11) |
                (((int)(scaledVector.Y * 2.032258f) & 0x3F) << 5) |
                ((int)scaledVector.Z & 0x1F));
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(
                (PackedValue >> 11) & 0x1F,
                ((PackedValue >> 5) & 0x3F) / 2.032258f,
                PackedValue & 0x1F,
                31f) / 31f;
        }

        #endregion

        #region Equals

        public readonly bool Equals(Bgr565 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Bgr565 other && Equals(other);
        }

        public static bool operator ==(Bgr565 a, Bgr565 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(Bgr565 a, Bgr565 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Bgr565) + $"({ToScaledVector4().ToVector3()})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
