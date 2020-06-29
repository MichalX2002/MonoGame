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
    /// Packed vector type containing four unsigned 16-bit XYZW components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra32 : IPackedPixel<Bgra32, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Blue),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Alpha));

        public byte B;
        public byte G;
        public byte R;
        public byte A;

        /// <summary>
        /// Gets or sets the RGB components of this struct as <see cref="Rgb24"/>
        /// </summary>
        public Bgr24 Bgr
        {
            readonly get => UnsafeR.As<Bgra32, Bgr24>(this);
            set => Unsafe.As<Bgra32, Bgr24>(ref this) = value;
        }

        #region Constructors

        [CLSCompliant(false)]
        public Bgra32(uint packedValue) : this()
        {
            PackedValue = packedValue;
        }

        [CLSCompliant(false)]
        public Bgra32(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<Bgra32, uint>(this);
            set => Unsafe.As<Bgra32, uint>(ref this) = value;
        }

        public void FromScaledVector(Vector3 vector)
        {
            Bgr24 bgr = default;
            bgr.FromScaledVector(vector);
            FromBgr(bgr);
        }

        public readonly Vector3 ToScaledVector3()
        {
            return new Vector3(R, G, B) / byte.MaxValue;
        }

        public void FromScaledVector(Vector4 vector)
        {
            Color rgba = default;
            rgba.FromScaledVector(vector);
            FromRgba(rgba);
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(R, G, B, A) / byte.MaxValue;
        }

        #endregion

        #region IPixel.From

        public void FromAlpha(Alpha8 source)
        {
            B = G = R = byte.MinValue;
            A = source.A;
        }

        public void FromAlpha(Alpha16 source)
        {
            B = G = R = byte.MinValue;
            A = ScalingHelper.ToUInt8(source.A);
        }

        public void FromAlpha(AlphaF source)
        {
            B = G = R = byte.MinValue;
            A = ScalingHelper.ToUInt8(source.A);
        }

        public void FromGray(Gray8 source)
        {
            B = G = R = source.L;
            A = byte.MaxValue;
        }

        public void FromGray(Gray16 source)
        {
            B = G = R = ScalingHelper.ToUInt8(source.L);
            A = byte.MaxValue;
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            B = G = R = source.L;
            A = source.A;
        }

        public void FromBgr(Bgr24 source)
        {
            Bgr = source;
            A = byte.MaxValue;
        }

        public void FromRgb(Rgb24 source)
        {
            B = source.B;
            G = source.G;
            R = source.R;
            A = byte.MaxValue;
        }

        public void FromRgba(Color source)
        {
            B = source.B;
            G = source.G;
            R = source.R;
            A = source.A;
        }

        public void FromRgb(Rgb48 source)
        {
            B = ScalingHelper.ToUInt8(source.B);
            G = ScalingHelper.ToUInt8(source.G);
            R = ScalingHelper.ToUInt8(source.R);
            A = byte.MaxValue;
        }

        public void FromRgba(Rgba64 source)
        {
            B = ScalingHelper.ToUInt8(source.B);
            G = ScalingHelper.ToUInt8(source.G);
            R = ScalingHelper.ToUInt8(source.R);
            A = ScalingHelper.ToUInt8(source.A);
        }

        #endregion

        #region IPixel.To

        public readonly Alpha8 ToAlpha8()
        {
            return A;
        }

        public readonly Alpha16 ToAlpha16()
        {
            return ScalingHelper.ToUInt16(A);
        }

        public readonly AlphaF ToAlphaF()
        {
            return ScalingHelper.ToFloat32(A);
        }

        public readonly Bgr24 ToBgr24()
        {
            return Bgr;
        }

        public readonly Rgb24 ToRgb24()
        {
            return new Rgb24(R, G, B);
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
            return new Color(R, G, B, A);
        }

        public readonly Rgba64 ToRgba64()
        {
            return new Rgba64(
                ScalingHelper.ToUInt16(R),
                ScalingHelper.ToUInt16(G),
                ScalingHelper.ToUInt16(B),
                ScalingHelper.ToUInt16(A));
        }

        #endregion

        #region Equals

        public readonly bool Equals(Bgra32 other) => this == other;

        public override readonly bool Equals(object obj) => obj is Bgra32 other && Equals(other);

        public static bool operator ==(in Bgra32 a, in Bgra32 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in Bgra32 a, in Bgra32 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Bgra32) + $"(R:{R}, G:{G}, B:{B}, A:{A})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}