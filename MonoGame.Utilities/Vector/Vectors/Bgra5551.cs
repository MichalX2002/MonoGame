// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
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
        private const int MaxXYZ = 0x1F;
        private const int MaxW = 0x01;

        private static Vector4 MaxValue => new Vector4(MaxXYZ, MaxXYZ, MaxXYZ, MaxW);

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

        public void FromScaledVector(Vector4 scaledVector)
        {
            scaledVector *= MaxValue;
            scaledVector += new Vector4(0.5f);
            scaledVector.Clamp(Vector4.Zero, MaxValue);

            PackedValue = (ushort)(
                (((int)scaledVector.X & MaxXYZ) << 10) |
                (((int)scaledVector.Y & MaxXYZ) << 5) |
                (((int)scaledVector.Z & MaxXYZ) << 0) |
                (((int)scaledVector.W) & MaxW) << 15);
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(
                (PackedValue >> 10) & MaxXYZ,
                (PackedValue >> 5) & MaxXYZ,
                (PackedValue >> 0) & MaxXYZ,
                (PackedValue >> 15) & MaxW) / MaxValue;
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

        #region Object overrides

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