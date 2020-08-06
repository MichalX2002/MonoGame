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
    /// The XYZ components use 10 bits each, and the W component uses 2 bits.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rgba1010102 : IPixel<Rgba1010102>, IPackedVector<uint>
    {
        private const int MaxXYZ = 0x03FF;
        private const int MaxW = 0x03;

        private static Vector3 MaxValue3 => new Vector3(MaxXYZ);
        private static Vector4 MaxValue4 => new Vector4(MaxXYZ, MaxXYZ, MaxXYZ, MaxW);

        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Red, 10),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Green, 10),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Blue, 10),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Alpha, 2));

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Rgba1010102(uint packed)
        {
            PackedValue = packed;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue { get; set; }

        public void FromScaledVector(Vector3 scaledVector)
        {
            scaledVector = VectorHelper.ScaledClamp(scaledVector);
            scaledVector *= MaxValue3;
            scaledVector += new Vector3(0.5f);

            PackedValue = (uint)(
                (((int)scaledVector.X & MaxXYZ) << 0) |
                (((int)scaledVector.Y & MaxXYZ) << 10) |
                (((int)scaledVector.Z & MaxXYZ) << 20) |
                (MaxW << 30));
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            scaledVector = VectorHelper.ScaledClamp(scaledVector);
            scaledVector *= MaxValue4;
            scaledVector += new Vector4(0.5f);

            PackedValue = (uint)(
                (((int)scaledVector.X & MaxXYZ) << 0) |
                (((int)scaledVector.Y & MaxXYZ) << 10) |
                (((int)scaledVector.Z & MaxXYZ) << 20) |
                (((int)scaledVector.W) & MaxW) << 30);
        }

        public void FromVector(Vector3 vector) => FromScaledVector(vector);
        public void FromVector(Vector4 vector) => FromScaledVector(vector);

        public readonly Vector3 ToScaledVector3()
        {
            return new Vector3(
                (PackedValue >> 0) & MaxXYZ,
                (PackedValue >> 10) & MaxXYZ,
                (PackedValue >> 20) & MaxXYZ) / MaxValue3;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(
                (PackedValue >> 0) & MaxXYZ,
                (PackedValue >> 10) & MaxXYZ,
                (PackedValue >> 20) & MaxXYZ,
                (PackedValue >> 30) & MaxW) / MaxValue4;
        }

        public readonly Vector3 ToVector3() => ToScaledVector3();
        public readonly Vector4 ToVector4() => ToScaledVector4();

        #endregion

        #region IPixel.From

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FromAlphaUInt2(byte value) => PackedValue = (ushort)(uint.MaxValue & (value << 30));

        public void FromAlpha(Alpha8 source) => FromAlphaUInt2(ScalingHelper.ToUInt2(source.A));
        public void FromAlpha(Alpha16 source) => FromAlphaUInt2(ScalingHelper.ToUInt2(source.A));
        public void FromAlpha(Alpha32 source) => FromAlphaUInt2(ScalingHelper.ToUInt2(source.A));
        public void FromAlpha(AlphaF source) => FromAlphaUInt2(ScalingHelper.ToUInt2(source.A));

        public void FromGray(Gray8 source) => FromColor(source.ToRgb24());
        public void FromGray(Gray16 source) => FromColor(source.ToRgb48());
        public void FromGray(Gray32 source) => FromScaledVector(source.ToScaledVector3());
        public void FromGray(GrayF source) => FromScaledVector(source.ToScaledVector3());
        public void FromGray(GrayAlpha16 source) => FromColor(source.ToRgba32());

        public void FromColor(Bgr565 source) => FromColor(source.ToRgb24());
        public void FromColor(Bgr24 source) => FromColor(source.ToRgb24());
        public void FromColor(Rgb24 source) => FromScaledVector(source.ToScaledVector3());
        public void FromColor(Rgb48 source) => FromScaledVector(source.ToScaledVector3());

        public void FromColor(Bgra4444 source) => FromColor(source.ToRgba32());
        public void FromColor(Bgra5551 source) => FromColor(source.ToRgba32());
        public void FromColor(Abgr32 source) => FromColor(source.ToRgba32());
        public void FromColor(Argb32 source) => FromColor(source.ToRgba32());
        public void FromColor(Bgra32 source) => FromColor(source.ToRgba32());
        public void FromColor(Rgba1010102 source) => this = source;
        public void FromColor(Color source) => FromScaledVector(source.ToScaledVector4());
        public void FromColor(Rgba64 source) => FromScaledVector(source.ToScaledVector4());

        #endregion

        #region IPixel.To

        public readonly Alpha8 ToAlpha8() => ScalingHelper.ToUInt8From2((byte)(PackedValue >> 30));
        public readonly Alpha16 ToAlpha16() => ScalingHelper.ToUInt16From2((byte)(PackedValue >> 30));
        public readonly AlphaF ToAlphaF() => ScalingHelper.ToFloat32FromUInt2((byte)(PackedValue >> 30));

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
        public readonly bool Equals(uint other) => PackedValue == other;

        public readonly bool Equals(Rgba1010102 other) => this == other;

        public static bool operator ==(Rgba1010102 a, Rgba1010102 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Rgba1010102 a, Rgba1010102 b) => a.PackedValue != b.PackedValue;

        #endregion

        #region Object overrides

        public override readonly bool Equals(object? obj) => obj is Rgba1010102 other && Equals(other);

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => HashCode.Combine(PackedValue);

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Rgba1010102) + $"({ToScaledVector4()})";

        #endregion
    }
}
