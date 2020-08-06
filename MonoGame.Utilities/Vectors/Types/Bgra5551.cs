// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing unsigned XYZW components.
    /// The XYZ components use 5 bits each, and the W component uses 1 bit.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra5551 : IPixel<Bgra5551>, IPackedVector<ushort>
    {
        public const int MaxXYZ = 0x1F;
        public const int MaxW = 0x01;

        private static Vector3 MaxValue3 => new Vector3(MaxXYZ);
        private static Vector4 MaxValue4 => new Vector4(MaxXYZ, MaxXYZ, MaxXYZ, MaxW);

        readonly VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
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

        public void FromScaledVector(Vector3 scaledVector)
        {
            scaledVector = VectorHelper.ScaledClamp(scaledVector);
            scaledVector *= MaxValue3;
            scaledVector += new Vector3(0.5f);

            PackedValue = (ushort)(
                ((int)scaledVector.X << 10) |
                ((int)scaledVector.Y << 5) |
                ((int)scaledVector.Z << 0) |
                (MaxW << 15));
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            scaledVector = VectorHelper.ScaledClamp(scaledVector);
            scaledVector *= MaxValue4;
            scaledVector += new Vector4(0.5f);

            PackedValue = (ushort)(
                ((int)scaledVector.X << 10) |
                ((int)scaledVector.Y << 5) |
                ((int)scaledVector.Z << 0) |
                ((int)scaledVector.W) << 15);
        }

        public void FromVector(Vector3 vector) => FromScaledVector(vector);
        public void FromVector(Vector4 vector) => FromScaledVector(vector);

        public readonly Vector3 ToScaledVector3()
        {
            return new Vector3(
                (PackedValue >> 10) & MaxXYZ,
                (PackedValue >> 5) & MaxXYZ,
                (PackedValue >> 0) & MaxXYZ) / MaxValue3;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(
                (PackedValue >> 10) & MaxXYZ,
                (PackedValue >> 5) & MaxXYZ,
                (PackedValue >> 0) & MaxXYZ,
                PackedValue >> 15) / MaxValue4;
        }

        public readonly Vector3 ToVector3() => ToScaledVector3();
        public readonly Vector4 ToVector4() => ToScaledVector4();

        #endregion

        #region IPixel.From

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FromAlphaUInt1(byte value) => PackedValue = (ushort)(ushort.MaxValue & (value << 15));

        public void FromAlpha(Alpha8 source) => FromAlphaUInt1(ScalingHelper.ToUInt1(source.A));
        public void FromAlpha(Alpha16 source) => FromAlphaUInt1(ScalingHelper.ToUInt1(source.A));
        public void FromAlpha(Alpha32 source) => FromAlphaUInt1(ScalingHelper.ToUInt1(source.A));
        public void FromAlpha(AlphaF source) => FromAlphaUInt1(ScalingHelper.ToUInt1(source.A));

        public void FromGray(Gray8 source) => FromColor(source.ToRgb24());
        public void FromGray(Gray16 source) => FromColor(source.ToRgb48());
        public void FromGray(Gray32 source) => FromScaledVector(source.ToScaledVector3());
        public void FromGray(GrayF source) => FromScaledVector(source.ToScaledVector3());
        public void FromGray(GrayAlpha16 source) => FromColor(source.ToRgba32());

        public void FromColor(Bgr565 source)
        {
            ushort packedSource = source.PackedValue;
            PackedValue = (ushort)(
                (((packedSource >> 0) & Bgr565.MaxXZ) << 0) |
                ((((packedSource >> 5) & Bgr565.MaxY) / 2) << 5) |
                (((packedSource >> 11) & Bgr565.MaxXZ) << 10) |
                (MaxW << 15));
        }

        public void FromColor(Bgr24 source) => FromColor(source.ToRgb24());
        public void FromColor(Rgb24 source) => FromScaledVector(source.ToScaledVector3());
        public void FromColor(Rgb48 source) => FromScaledVector(source.ToScaledVector3());

        public void FromColor(Bgra4444 source)
        {
            ushort packedSource = source.PackedValue;
            PackedValue = (ushort)(
                ((((packedSource >> 0) & Bgra4444.MaxXYZW) * 2) << 0) |
                ((((packedSource >> 4) & Bgra4444.MaxXYZW) * 2) << 5) |
                ((((packedSource >> 8) & Bgra4444.MaxXYZW) * 2) << 10) |
                ((int)((packedSource >> 12) / (float)Bgra4444.MaxXYZW + 0.5f) << 15));
        }

        public void FromColor(Bgra5551 source) => this = source;
        public void FromColor(Abgr32 source) => FromColor(source.ToRgba32());
        public void FromColor(Argb32 source) => FromColor(source.ToRgba32());
        public void FromColor(Bgra32 source) => FromColor(source.ToRgba32());
        public void FromColor(Rgba1010102 source) => FromScaledVector(source.ToScaledVector4());
        public void FromColor(Color source) => FromScaledVector(source.ToScaledVector4());
        public void FromColor(Rgba64 source) => FromScaledVector(source.ToScaledVector4());

        #endregion

        #region IPixel.To

        public readonly Alpha8 ToAlpha8() => ScalingHelper.ToUInt8From1((byte)(PackedValue >> 15));
        public readonly Alpha16 ToAlpha16() => ScalingHelper.ToUInt16From1((byte)(PackedValue >> 15));
        public readonly AlphaF ToAlphaF() => ScalingHelper.ToFloat32FromUInt1((byte)(PackedValue >> 15));

        public readonly Gray8 ToGray8() => PixelHelper.ToGray8(this);
        public readonly Gray16 ToGray16() => PixelHelper.ToGray16(this);
        public readonly GrayF ToGrayF() => PixelHelper.ToGrayF(this);
        public readonly GrayAlpha16 ToGrayAlpha16() => PixelHelper.ToGrayAlpha16(this);

        public readonly Rgb24 ToRgb24() => ScaledVectorHelper.ToRgb24(this);
        public readonly Rgb48 ToRgb48() => ScaledVectorHelper.ToRgb48(this);

        public readonly Color ToRgba32() => ScaledVectorHelper.ToRgba32(this);
        public readonly Rgba64 ToRgba64() => ScaledVectorHelper.ToRgba64(this);

        #endregion

        #region Equals

        [CLSCompliant(false)]
        public readonly bool Equals(ushort other) => PackedValue == other;

        public readonly bool Equals(Bgra5551 other) => this == other;

        public static bool operator ==(Bgra5551 a, Bgra5551 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Bgra5551 a, Bgra5551 b) => a.PackedValue != b.PackedValue;

        #endregion

        #region Object overrides

        public override readonly bool Equals(object? obj) => obj is Bgra5551 other && Equals(other);

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => HashCode.Combine(PackedValue);

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Bgra5551) + $"({ToScaledVector4()}";

        #endregion
    }
}