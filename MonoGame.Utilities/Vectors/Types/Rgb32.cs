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
    /// Packed vector type containing three unsigned 8-bit XYZ components.
    /// Padded with an extra 8 bits.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rgb32 : IPixel<Rgb32>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Blue),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Padding));

        public byte R;
        public byte G;
        public byte B;
        public byte Padding;

        /// <summary>
        /// Gets or sets the RGB components of this <see cref="Rgb32"/> as <see cref="Rgb24"/>
        /// </summary>
        public Rgb24 Rgb
        {
            readonly get => UnsafeR.As<Rgb32, Rgb24>(this);
            set => Unsafe.As<Rgb32, Rgb24>(ref this) = value;
        }

        #region Constructors

        public Rgb32(byte r, byte g, byte b, byte padding = default)
        {
            R = r;
            G = g;
            B = b;
            Padding = padding;
        }

        #endregion

        #region IPackedVector

        public void FromScaledVector(Vector3 scaledVector)
        {
            scaledVector = RgbaVectorHelper.ToScaledUInt8(scaledVector);
            R = (byte)scaledVector.X;
            G = (byte)scaledVector.Y;
            B = (byte)scaledVector.Z;
        }

        public readonly Vector3 ToScaledVector3()
        {
            return new Vector3(R, G, B) / byte.MaxValue;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            FromScaledVector(scaledVector.ToScaledVector3());
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(ToScaledVector3(), 1);
        }

        #endregion

        #region IPixel.From

        public Alpha8 ToAlpha8()
        {
            return Alpha8.Opaque;
        }

        public Alpha16 ToAlpha16()
        {
            return Alpha16.Opaque;
        }

        public AlphaF ToAlphaF()
        {
            return AlphaF.Opaque;
        }

        public void FromGray(Gray8 source)
        {
            B = G = R = source.L;
        }

        public void FromGray(Gray16 source)
        {
            B = G = R = ScalingHelper.ToUInt8(source.L);
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            B = G = R = source.L;
        }

        public void FromRgb(Rgb24 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

        public void FromRgba(Color source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

        public void FromRgb(Rgb48 source)
        {
            R = ScalingHelper.ToUInt8(source.R);
            G = ScalingHelper.ToUInt8(source.G);
            B = ScalingHelper.ToUInt8(source.B);
        }

        public void FromRgba(Rgba64 source)
        {
            R = ScalingHelper.ToUInt8(source.R);
            G = ScalingHelper.ToUInt8(source.G);
            B = ScalingHelper.ToUInt8(source.B);
        }

        #endregion

        #region IPixel.To

        public readonly Rgb24 ToRgb24()
        {
            return Rgb;
        }

        public readonly Rgb48 ToRgb48()
        {
            return new Rgb48(
                ScalingHelper.ToUInt16(R),
                ScalingHelper.ToUInt16(G),
                ScalingHelper.ToUInt16(B));
        }

        public readonly Color ToRgba32()
        {
            return new Color(R, G, B);
        }

        public readonly Rgba64 ToRgba64()
        {
            return new Rgba64(
                ScalingHelper.ToUInt16(R),
                ScalingHelper.ToUInt16(G),
                ScalingHelper.ToUInt16(B));
        }

        #endregion

        #region Equals

        public readonly bool Equals(Rgb32 other) => this == other;

        public override readonly bool Equals(object obj) => obj is Rgb32 other && Equals(other);

        // we don't want to compare padding
        public static bool operator ==(in Rgb32 a, in Rgb32 b) => (a.R, a.G, a.B) == (b.R, b.G, b.B);

        public static bool operator !=(in Rgb32 a, in Rgb32 b) => (a.R, a.G, a.B) != (b.R, b.G, b.B);

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Rgb32) + $"(R:{R}, G:{G}, B:{B})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => HashCode.Combine(R, G, B);

        #endregion

        public static implicit operator Rgb24(in Rgb32 value) => value.Rgb;
    }
}