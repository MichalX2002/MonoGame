// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing an 16-bit W component.
    /// <para>
    /// Ranges from [1, 1, 1, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Alpha16 : IPixel<Alpha16>, IPackedVector<ushort>
    {
        public static Alpha16 Transparent => default;
        public static Alpha16 Opaque => new Alpha16(ushort.MaxValue);

        readonly VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.UInt16, VectorComponentChannel.Alpha));

        [CLSCompliant(false)]
        public ushort A;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        [CLSCompliant(false)]
        public Alpha16(ushort value)
        {
            A = value;
        }

        /// <summary>
        /// Constructs the packed vector with a vector form value.
        /// </summary>
        /// <param name="alpha">The W component.</param>
        public Alpha16(float alpha)
        {
            A = ScalingHelper.ToUInt16(alpha);
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue
        {
            readonly get => A;
            set => A = value;
        }

        public void FromScaledVector(Vector3 scaledVector) => A = ushort.MaxValue;
        public void FromScaledVector(Vector4 scaledVector) => A = ScalingHelper.ToUInt16(scaledVector.W);

        public readonly Vector3 ToScaledVector3() => Vector3.One;
        public readonly Vector4 ToScaledVector4() => new Vector4(ToScaledVector3(), ScalingHelper.ToFloat32(A));

        public readonly Vector3 ToVector3() => ToScaledVector3();
        public readonly Vector4 ToVector4() => ToScaledVector4();

        #endregion

        #region IPixel.From

        public void FromAlpha(Alpha8 source) => A = ScalingHelper.ToUInt16(source.A);
        public void FromAlpha(Alpha16 source) => this = source;
        public void FromAlpha(Alpha32 source) => A = ScalingHelper.ToUInt16(source.A);
        public void FromAlpha(AlphaF source) => A = ScalingHelper.ToUInt16(source.A);

        public void FromGray(Gray8 source) => A = ushort.MaxValue;
        public void FromGray(Gray16 source) => A = ushort.MaxValue;
        public void FromGray(Gray32 source) => A = ushort.MaxValue;
        public void FromGray(GrayF source) => A = ushort.MaxValue;
        public void FromGray(GrayAlpha16 source) => A = ScalingHelper.ToUInt16(source.A);

        public void FromColor(Bgr24 source) => A = ushort.MaxValue;
        public void FromColor(Rgb24 source) => A = ushort.MaxValue;
        public void FromColor(Rgb48 source) => A = ushort.MaxValue;

        public void FromColor(Abgr32 source) => A = ScalingHelper.ToUInt16(source.A);
        public void FromColor(Argb32 source) => A = ScalingHelper.ToUInt16(source.A);
        public void FromColor(Bgra32 source) => A = ScalingHelper.ToUInt16(source.A);
        public void FromColor(Color source) => A = ScalingHelper.ToUInt16(source.A);
        public void FromColor(Rgba64 source) => A = source.A;

        #endregion

        #region IPixel.To

        public readonly Alpha8 ToAlpha8() => ScalingHelper.ToUInt8(A);
        public readonly Alpha16 ToAlpha16() => this;
        public readonly AlphaF ToAlphaF() => ScalingHelper.ToFloat32(A);

        public readonly Gray8 ToGray8() => Gray8.White;
        public readonly Gray16 ToGray16() => Gray16.White;
        public readonly GrayF ToGrayF() => GrayF.White;
        public readonly GrayAlpha16 ToGrayAlpha16() => GrayAlpha16.OpaqueWhite;


        public readonly Rgb24 ToRgb24() => Rgb24.White;
        public readonly Rgb48 ToRgb48() => Rgb48.White;

        public readonly Color ToRgba32() => new Color(byte.MaxValue, ScalingHelper.ToUInt8(A));
        public readonly Rgba64 ToRgba64() => new Rgba64(ushort.MaxValue, A);

        #endregion

        #region Equals

        [CLSCompliant(false)]
        public readonly bool Equals(ushort other) => PackedValue == other;

        public readonly bool Equals(Alpha16 other) => this == other;

        public static bool operator ==(Alpha16 a, Alpha16 b) => a.A == b.A;
        public static bool operator !=(Alpha16 a, Alpha16 b) => a.A != b.A;

        #endregion

        #region Object overrides

        public override readonly bool Equals(object? obj) => obj is Alpha16 other && Equals(other);

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => HashCode.Combine(A);

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Alpha16) + $"({A})";

        #endregion

        [CLSCompliant(false)]
        public static implicit operator Alpha16(ushort alpha) => new Alpha16(alpha);

        [CLSCompliant(false)]
        public static implicit operator ushort(Alpha16 value) => value.A;
    }
}
