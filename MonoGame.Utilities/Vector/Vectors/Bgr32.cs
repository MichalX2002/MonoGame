// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing three unsigned 8-bit XYZ components.
    /// Padded with an extra 8 bits.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr32 : IPixel<Bgr32>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Blue),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Padding));

        public byte B;
        public byte G;
        public byte R;
        public byte Padding;

        /// <summary>
        /// Gets or sets the RGB components of this struct as <see cref="Rgb24"/>
        /// </summary>
        public Bgr24 Bgr
        {
            readonly get => UnsafeR.As<Bgr32, Bgr24>(this);
            set => Unsafe.As<Bgr32, Bgr24>(ref this) = value;
        }

        #region Constructors

        public Bgr32(byte r, byte g, byte b, byte padding = default)
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
            B = (byte)scaledVector.Z;
            G = (byte)scaledVector.Y;
            R = (byte)scaledVector.X;
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

        public void FromAlpha(Alpha8 source)
        {
            B = G = R = byte.MaxValue;
        }

        public void FromAlpha(Alpha16 source)
        {
            B = G = R = byte.MaxValue;
        }

        public void FromAlpha(AlphaF source)
        {
            B = G = R = byte.MaxValue;
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

        public void FromRgb(Rgb48 source)
        {
            R = ScalingHelper.ToUInt8(source.R);
            G = ScalingHelper.ToUInt8(source.G);
            B = ScalingHelper.ToUInt8(source.B);
        }

        public void FromRgba(Color source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

        public void FromRgba(Rgba64 source)
        {
            R = ScalingHelper.ToUInt8(source.R);
            G = ScalingHelper.ToUInt8(source.G);
            B = ScalingHelper.ToUInt8(source.B);
        }

        #endregion

        #region IPixel.To

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

        public readonly Rgb24 ToRgb24()
        {
            return new Rgb24(R, G, B);
        }

        public readonly Color ToRgba32()
        {
            return new Color(R, G, B, byte.MaxValue);
        }

        public readonly Rgb48 ToRgb48()
        {
            return new Rgb48(
                ScalingHelper.ToUInt16(R),
                ScalingHelper.ToUInt16(G),
                ScalingHelper.ToUInt16(B));
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

        public readonly bool Equals(Bgr32 other) => this == other;

        public override readonly bool Equals(object obj) => obj is Bgr32 other && Equals(other);

        // we don't want to compare padding
        public static bool operator ==(in Bgr32 a, in Bgr32 b) => (a.B, a.G, a.R) == (b.B, b.G, b.R);

        public static bool operator !=(in Bgr32 a, in Bgr32 b) => (a.B, a.G, a.R) != (b.B, b.G, b.R);

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Bgr32) + $"(R:{R}, G:{G}, B:{B})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => HashCode.Combine(R, G, B);

        #endregion

        public static implicit operator Bgr24(in Bgr32 value) => value.Bgr;
    }
}