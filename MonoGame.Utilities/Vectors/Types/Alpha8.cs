// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing an 8-bit W component.
    /// <para>
    /// Ranges from [1, 1, 1, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Alpha8 : IPackedPixel<Alpha8, byte>
    {
        public static Alpha8 Transparent => default;
        public static Alpha8 Opaque => new Alpha8(byte.MaxValue);

        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Alpha));

        public byte A;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        public Alpha8(byte value)
        {
            A = value;
        }

        /// <summary>
        /// Constructs the packed vector with a vector form value.
        /// </summary>
        /// <param name="alpha">The W component.</param>
        public Alpha8(float alpha)
        {
            A = ScalingHelper.ToUInt8(alpha);
        }

        #endregion

        #region IPackedVector

        public byte PackedValue { readonly get => A; set => A = value; }

        public void FromScaledVector(Vector3 scaledVector)
        {
            A = byte.MaxValue;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            A = ScalingHelper.ToUInt8(scaledVector.W);
        }

        public readonly Vector3 ToScaledVector3()
        {
            return Vector3.One;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(1, 1, 1, ToAlphaF());
        }

        #endregion

        #region IPixel.From

        public void FromAlpha(Alpha8 source)
        {
            this = source;
        }

        public void FromAlpha(Alpha16 source)
        {
            A = ScalingHelper.ToUInt8(source.A);
        }

        public void FromAlpha(AlphaF source)
        {
            A = ScalingHelper.ToUInt8(source.A);
        }

        public void FromGray(Gray8 source)
        {
            A = byte.MaxValue;
        }

        public void FromGray(Gray16 source)
        {
            A = byte.MaxValue;
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            A = source.A;
        }

        public void FromRgb(Rgb24 source)
        {
            A = byte.MaxValue;
        }

        public void FromRgb(Rgb48 source)
        {
            A = byte.MaxValue;
        }

        public void FromRgba(Color source)
        {
            A = source.A;
        }

        public void FromRgba(Rgba64 source)
        {
            A = ScalingHelper.ToUInt8(source.A);
        }

        #endregion

        #region IPixel.To

        public readonly Alpha8 ToAlpha8()
        {
            return this;
        }

        public readonly Alpha16 ToAlpha16()
        {
            return ScalingHelper.ToUInt16(A);
        }

        public readonly AlphaF ToAlphaF()
        {
            return ScalingHelper.ToFloat32(A);
        }

        public readonly Rgb24 ToRgb24()
        {
            return Rgb24.White;
        }

        public readonly Rgb48 ToRgb48()
        {
            return Rgb48.White;
        }

        public readonly Color ToRgba32()
        {
            return new Color(byte.MaxValue, A);
        }

        public readonly Rgba64 ToRgba64()
        {
            return new Rgba64(ushort.MaxValue, ScalingHelper.ToUInt16(A));
        }

        #endregion

        #region Equals

        public readonly bool Equals(Alpha8 other) => this == other;

        public override readonly bool Equals(object obj) => obj is Alpha8 other && Equals(other);

        public static bool operator ==(Alpha8 a, Alpha8 b) => a.PackedValue == b.PackedValue;

        public static bool operator !=(Alpha8 a, Alpha8 b) => a.PackedValue != b.PackedValue;

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Alpha8) + $"({A})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => A;

        #endregion

        public static implicit operator Alpha8(byte alpha) => new Alpha8(alpha);

        public static implicit operator byte(Alpha8 value) => value.A;
    }
}
