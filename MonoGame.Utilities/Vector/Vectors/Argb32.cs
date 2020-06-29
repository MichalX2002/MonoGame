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
    public struct Argb32 : IPackedPixel<Argb32, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Alpha),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Blue));

        public byte A;
        public byte R;
        public byte G;
        public byte B;

        /// <summary>
        /// Gets or sets the RGB components of this struct as <see cref="Rgb24"/>
        /// </summary>
        public Rgb24 Rgb
        {
            readonly get => UnsafeR.AddByteOffset(UnsafeR.As<Argb32, Rgb24>(this), sizeof(byte));
            set => Unsafe.AddByteOffset(ref Unsafe.As<Argb32, Rgb24>(ref this), (IntPtr)sizeof(byte)) = value;
        }

        #region Constructors

        [CLSCompliant(false)]
        public Argb32(byte r, byte g, byte b, byte a)
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
            readonly get => UnsafeR.As<Argb32, uint>(this);
            set => Unsafe.As<Argb32, uint>(ref this) = value;
        }

        public void FromScaledVector(Vector3 scaledVector)
        {
            Rgb24 rgb = default; // TODO: Unsafe.SkipInit
            rgb.FromScaledVector(scaledVector);
            FromRgb(rgb);
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            Color rgba = default; // TODO: Unsafe.SkipInit
            rgba.FromScaledVector(scaledVector);
            FromRgba(rgba);
        }

        public readonly Vector3 ToScaledVector3()
        {
            return new Vector3(R, G, B) / byte.MaxValue;
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
            R = G = B = byte.MinValue;
        }

        public void FromAlpha(Alpha16 source)
        {
            A = ScalingHelper.ToUInt8(source.A);
            R = G = B = byte.MinValue;
        }

        public void FromAlpha(AlphaF source)
        {
            A = ScalingHelper.ToUInt8(source.A);
            R = G = B = byte.MinValue;
        }

        public void FromGray(Gray8 source)
        {
            A = byte.MaxValue;
            R = G = B = source.L;
        }

        public void FromGray(Gray16 source)
        {
            A = byte.MaxValue;
            R = G = B = ScalingHelper.ToUInt8(source.L);
        }

        public void FromGray(GrayF source)
        {
            A = byte.MaxValue;
            R = G = B = ScalingHelper.ToUInt8(source.L);
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            A = source.A;
            R = G = B = source.L;
        }

        public void FromRgb(Rgb24 source)
        {
            A = byte.MaxValue;
            Rgb = source;
        }

        public void FromRgb(Rgb48 source)
        {
            A = byte.MaxValue;
            Rgb = source.ToRgb24();
        }

        public void FromRgba(Color source)
        {
            A = source.A;
            R = source.R;
            G = source.G;
            B = source.B;
        }

        public void FromRgba(Rgba64 source)
        {
            A = ScalingHelper.ToUInt8(source.A);
            R = ScalingHelper.ToUInt8(source.R);
            G = ScalingHelper.ToUInt8(source.G);
            B = ScalingHelper.ToUInt8(source.B);
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

        public readonly bool Equals(Argb32 other) => this == other;

        public override readonly bool Equals(object obj) => obj is Argb32 other && Equals(other);

        public static bool operator ==(in Argb32 a, in Argb32 b) => a.PackedValue == b.PackedValue;

        public static bool operator !=(in Argb32 a, in Argb32 b) => a.PackedValue != b.PackedValue;

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Argb32) + $"(R:{R}, G:{G}, B:{B}, A:{A})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}