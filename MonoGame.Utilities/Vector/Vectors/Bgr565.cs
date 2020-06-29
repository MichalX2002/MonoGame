// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
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
        private const int MaxXZ = 0x1F;
        private const int MaxY = 0x3F;

        private static Vector3 MaxValue => new Vector3(MaxXZ, MaxY, MaxXZ);

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

        public void FromScaledVector(Vector3 scaledVector)
        {
            scaledVector *= MaxValue;
            scaledVector += new Vector3(0.5f);
            scaledVector.Clamp(Vector3.Zero, MaxValue);

            PackedValue = (ushort)(
                (((int)scaledVector.X & MaxXZ) << 11) |
                (((int)scaledVector.Y & MaxY) << 5) |
                ((int)scaledVector.Z & MaxXZ));
        }

        public readonly Vector3 ToScaledVector3()
        {
            return new Vector3(
                (PackedValue >> 11) & MaxXZ,
                (PackedValue >> 5) & MaxY,
                PackedValue & MaxXZ) / MaxValue;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            FromScaledVector(scaledVector.ToVector3());
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(ToScaledVector3(), 1);
        }

        #endregion

        #region IPixel

        #region From

        public void FromAlpha(Alpha8 source)
        {
            PackedValue = ushort.MaxValue;
        }

        public void FromAlpha(Alpha16 source)
        {
            PackedValue = ushort.MaxValue;
        }

        public void FromAlpha(AlphaF source)
        {
            PackedValue = ushort.MaxValue;
        }

        #endregion

        #region To

        public readonly Alpha8 ToAlpha8()
        {
            return Alpha8.Opaque;
        }

        public readonly Alpha16 ToAlpha16()
        {
            return Alpha16.Opaque;
        }

        public readonly AlphaF ToAlphaF()
        {
            return AlphaF.Opaque;
        }

        #endregion

        #endregion

        #region Equals

        public readonly bool Equals(Bgr565 other) => this == other;

        public override readonly bool Equals(object obj) => obj is Bgr565 other && Equals(other);

        public static bool operator ==(Bgr565 a, Bgr565 b) => a.PackedValue == b.PackedValue;

        public static bool operator !=(Bgr565 a, Bgr565 b) => a.PackedValue != b.PackedValue;

        #endregion

        #region Object overrides

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
