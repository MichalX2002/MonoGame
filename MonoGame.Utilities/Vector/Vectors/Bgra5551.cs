// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing unsigned XYZW components.
    /// The XYZ components use 5 bits each, and the W component uses 1 bit.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra5551 : IPackedPixel<Bgra5551, ushort>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Blue, 5),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Green, 5),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Red, 5),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Alpha, 1));

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Bgra5551(ushort packed)
        {
            PackedValue = packed;
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            scaledVector *= 31f;
            scaledVector += Vector4.Half;
            scaledVector.Clamp(Vector4.Zero, Vector4.One);

            PackedValue = (ushort)(
                (((int)scaledVector.X & 0x1F) << 10) |
                (((int)scaledVector.Y & 0x1F) << 5) |
                (((int)scaledVector.Z & 0x1F) << 0) |
                (((int)(scaledVector.W / 31f) & 0x1) << 15));
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(
                (PackedValue >> 10) & 0x1F,
                (PackedValue >> 5) & 0x1F,
                (PackedValue >> 0) & 0x1F,
                ((PackedValue >> 15) & 0x01) * 31f) / 31f;
        }

        #endregion

        #region Equals

        public readonly bool Equals(Bgra5551 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Bgra5551 other && Equals(other);
        }

        public static bool operator ==(Bgra5551 a, Bgra5551 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(Bgra5551 a, Bgra5551 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Bgra5551) + $"({ToScaledVector4()}";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}