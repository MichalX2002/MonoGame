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
    /// Packed vector type containing four unsigned 16-bit XYZW components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Abgr32 : IPackedPixel<Abgr32, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Alpha),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Blue),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Red));

        public byte A;
        public byte B;
        public byte G;
        public byte R;

        /// <summary>
        /// Gets or sets the RGB components of this struct as <see cref="Bgr24"/>
        /// </summary>
        public Bgr24 Bgr
        {
            readonly get => UnsafeR.AddByteOffset(UnsafeR.As<Abgr32, Bgr24>(this), sizeof(byte));
            set => Unsafe.AddByteOffset(ref Unsafe.As<Abgr32, Bgr24>(ref this), (IntPtr)sizeof(byte)) = value;
        }

        #region Constructors

        [CLSCompliant(false)]
        public Abgr32(byte r, byte g, byte b, byte a)
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
            readonly get => UnsafeR.As<Abgr32, uint>(this);
            set => Unsafe.As<Abgr32, uint>(ref this) = value;
        }

        public void FromScaledVector(Vector3 scaledVector)
        {
            Bgr24 bgr = default; // TODO: Unsafe.SkipInit
            bgr.FromScaledVector(scaledVector);
            FromBgr(bgr);
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            Color rgba = default; // TODO: Unsafe.SkipInit
            rgba.FromScaledVector(scaledVector);
            FromRgba(rgba);
        }

        public readonly Vector3 ToScaledVector3()
        {
            return new Vector3(R, G, B);
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(R, G, B, A) / byte.MaxValue;
        }

        #endregion

        #region IPixel.From

        public void FromAlpha(Alpha8 source)
        {
            A = source.A;
            B = G = R = byte.MaxValue;
        }

        public void FromAlpha(Alpha16 source)
        {
            A = ScalingHelper.ToUInt8(source.A);
            B = G = R = byte.MaxValue;
        }

        public void FromAlpha(AlphaF source)
        {
            A = ScalingHelper.ToUInt8(source.A);
            B = G = R = byte.MaxValue;
        }

        public void FromGray(Gray8 source)
        {
            A = byte.MaxValue;
            B = G = R = source.L;
        }

        public void FromGray(Gray16 source)
        {
            A = byte.MaxValue;
            B = G = R = ScalingHelper.ToUInt8(source.L);
        }

        public void FromGray(GrayF source)
        {
            A = byte.MaxValue;
            B = G = R = ScalingHelper.ToUInt8(source.L);
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            A = source.A;
            B = G = R = source.L;
        }

        public void FromBgr(Bgr24 source)
        {
            A = byte.MaxValue;
            Bgr = source;
        }

        public void FromRgb(Rgb24 source)
        {
            A = byte.MaxValue;
            B = source.B;
            G = source.G;
            R = source.R;
        }

        public void FromRgb(Rgb48 source)
        {
            A = byte.MaxValue;
            B = ScalingHelper.ToUInt8(source.B);
            G = ScalingHelper.ToUInt8(source.G);
            R = ScalingHelper.ToUInt8(source.R);
        }

        public void FromRgba(Color source)
        {
            A = source.A;
            B = source.B;
            G = source.G;
            R = source.R;
        }

        public void FromRgba(Rgba64 source)
        {
            A = ScalingHelper.ToUInt8(source.A);
            B = ScalingHelper.ToUInt8(source.B);
            G = ScalingHelper.ToUInt8(source.G);
            R = ScalingHelper.ToUInt8(source.R);
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

        public readonly bool Equals(Abgr32 other) => this == other;

        public override readonly bool Equals(object obj) => obj is Abgr32 other && Equals(other);

        public static bool operator ==(in Abgr32 a, in Abgr32 b) => a.PackedValue == b.PackedValue;

        public static bool operator !=(in Abgr32 a, in Abgr32 b) => a.PackedValue != b.PackedValue;

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Abgr32) + $"(R:{R}, G:{G}, B:{B}, A:{A})";

        #endregion
    }
}