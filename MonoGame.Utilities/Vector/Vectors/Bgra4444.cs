// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing unsigned 4-bit XYZW components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra4444 : IPackedPixel<Bgra4444, ushort>
    {
        private const int MaxXYZW = 0x0F;

        private const float MaxValue = MaxXYZW;

        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Blue, 4),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Green, 4),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Red, 4),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Alpha, 4));

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Bgra4444(ushort packed)
        {
            PackedValue = packed;
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="scaledVector"><see cref="Vector4"/> containing the components.</param>
        public Bgra4444(Vector4 scaledVector) : this()
        {
            // TODO: Unsafe.SkipInit(out this)
            FromScaledVector(scaledVector);
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Bgra4444(float r, float g, float b, float a) : this(new Vector4(r, g, b, a))
        {
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        public void FromScaledVector(Vector3 scaledVector)
        {
            scaledVector *= MaxValue;
            scaledVector += new Vector3(0.5f);
            scaledVector.Clamp(0, MaxValue);

            PackedValue = (ushort)(
                (MaxXYZW << 12) |
                (((int)scaledVector.X & MaxXYZW) << 8) |
                (((int)scaledVector.Y & MaxXYZW) << 4) |
                ((int)scaledVector.Z & MaxXYZW));
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            scaledVector *= MaxValue;
            scaledVector += new Vector4(0.5f);
            scaledVector.Clamp(0, MaxValue);

            PackedValue = (ushort)(
                (((int)scaledVector.W) << 12) |
                (((int)scaledVector.X) << 8) |
                (((int)scaledVector.Y) << 4) |
                ((int)scaledVector.Z));
        }

        public readonly Vector3 ToScaledVector3()
        {
            return new Vector3(
                (PackedValue >> 8) & MaxXYZW,
                (PackedValue >> 4) & MaxXYZW,
                PackedValue & MaxXYZW) / MaxValue;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(
                (PackedValue >> 8) & MaxXYZW,
                (PackedValue >> 4) & MaxXYZW,
                PackedValue & MaxXYZW,
                PackedValue >> 12) / MaxValue;
        }

        #endregion

        #region Equals

        public readonly bool Equals(Bgra4444 other) => this == other;

        public override readonly bool Equals(object obj) => obj is Bgra4444 other && Equals(other);

        public static bool operator ==(Bgra4444 a, Bgra4444 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(Bgra4444 a, Bgra4444 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region IPixel.From

        public void FromAlpha(Alpha8 source)
        {
            PackedValue &= (ushort)((ScalingHelper.ToUInt4(source.A)) << 12);
        }

        public void FromAlpha(Alpha16 source)
        {
            PackedValue &= (ushort)((ScalingHelper.ToUInt4(source.A)) << 12);
        }

        public void FromAlpha(AlphaF source)
        {
            PackedValue &= (ushort)((ScalingHelper.ToUInt4(source.A)) << 12);
        }

        #endregion

        #region IPixel.To

        public readonly Alpha8 ToAlpha8()
        {
            return ScalingHelper.ToUInt8From4(PackedValue >> 12);
        }

        public readonly Alpha16 ToAlpha16()
        {
            return ScalingHelper.ToUInt16From4(PackedValue >> 12);
        }

        public readonly AlphaF ToAlphaF()
        {
            return ScalingHelper.ToFloat32FromUInt4(PackedValue >> 12);
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Bgra4444) + $"({ToScaledVector4()})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
