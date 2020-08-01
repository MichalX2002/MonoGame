// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing an 32-bit X component.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 0, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Red32 : IPixel<Red32>, IPackedVector<uint>
    {
        readonly VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.UInt32, VectorComponentChannel.Red));

        [CLSCompliant(false)]
        public uint R;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        /// <param name="red">The X component.</param>
        [CLSCompliant(false)]
        public Red32(uint red)
        {
            R = red;
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => R;
            set => R = value;
        }

        public void FromScaledVector(Vector3 scaledVector) => R = ScalingHelper.ToUInt32(scaledVector.X);
        public void FromScaledVector(Vector4 scaledVector) => R = ScalingHelper.ToUInt32(scaledVector.X);

        public readonly Vector3 ToScaledVector3() => new Vector3(ScalingHelper.ToFloat32(R), 0, 0);
        public readonly Vector4 ToScaledVector4() => new Vector4(ToScaledVector3(), 1);

        public readonly Vector3 ToVector3() => ToScaledVector3();
        public readonly Vector4 ToVector4() => ToScaledVector4();

        #endregion

        #region IPixel.From

        public void FromAlpha(Alpha8 source) => R = uint.MaxValue;
        public void FromAlpha(Alpha16 source) => R = uint.MaxValue;
        public void FromAlpha(Alpha32 source) => R = uint.MaxValue;
        public void FromAlpha(AlphaF source) => R = uint.MaxValue;

        public void FromGray(Gray8 source) => R = ScalingHelper.ToUInt32(source.L);
        public void FromGray(Gray16 source) => R = ScalingHelper.ToUInt32(source.L);
        public void FromGray(Gray32 source) => R = source.L;
        public void FromGray(GrayF source) => R = ScalingHelper.ToUInt32(source.L);
        public void FromGray(GrayAlpha16 source) => R = ScalingHelper.ToUInt32(source.L);

        public void FromColor(Bgr24 source) => R = ScalingHelper.ToUInt32(source.R);
        public void FromColor(Rgb24 source) => R = ScalingHelper.ToUInt32(source.R);
        public void FromColor(Rgb48 source) => R = ScalingHelper.ToUInt32(source.R);

        public void FromColor(Abgr32 source) => R = ScalingHelper.ToUInt32(source.R);
        public void FromColor(Argb32 source) => R = ScalingHelper.ToUInt32(source.R);
        public void FromColor(Bgra32 source) => R = ScalingHelper.ToUInt32(source.R);
        public void FromColor(Color source) => R = ScalingHelper.ToUInt32(source.R);
        public void FromColor(Rgba64 source) => R = ScalingHelper.ToUInt32(source.R);

        #endregion

        #region IPixel.To

        public readonly Alpha8 ToAlpha8() => Alpha8.Opaque;
        public readonly Alpha16 ToAlpha16() => Alpha16.Opaque;
        public readonly AlphaF ToAlphaF() => AlphaF.Opaque;

        public readonly Gray8 ToGray8() => ScalingHelper.ToUInt8((uint)(R * PixelHelper.GrayFactor.X));
        public readonly Gray16 ToGray16() => ScalingHelper.ToUInt8((uint)(R * PixelHelper.GrayFactor.X));
        public readonly GrayF ToGrayF() => new GrayF(ScalingHelper.ToFloat32((uint)(R * PixelHelper.GrayFactor.X)));
        public readonly GrayAlpha16 ToGrayAlpha16() => new GrayAlpha16(ToGray8(), byte.MaxValue);

        public readonly Rgb24 ToRgb24() => new Rgb24(ScalingHelper.ToUInt8(R), 0, 0);
        public readonly Rgb48 ToRgb48() => new Rgb48(ScalingHelper.ToUInt8(R), 0, 0);

        public readonly Color ToRgba32() => new Color(ScalingHelper.ToUInt8(R), byte.MinValue, byte.MinValue);
        public readonly Rgba64 ToRgba64() => new Rgba64(ScalingHelper.ToUInt8(R), ushort.MinValue, ushort.MinValue);

        #endregion

        #region Equals

        [CLSCompliant(false)]
        public readonly bool Equals(uint other) => PackedValue == other;

        public readonly bool Equals(Red32 other) => this == other;

        public static bool operator ==(Red32 a, Red32 b) => a.R == b.R;
        public static bool operator !=(Red32 a, Red32 b) => a.R != b.R;

        #endregion

        #region Object overrides

        public override readonly bool Equals(object? obj) => obj is Red32 other && Equals(other);

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => HashCode.Combine(R);

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Red32) + $"({R})";

        #endregion
    }
}
