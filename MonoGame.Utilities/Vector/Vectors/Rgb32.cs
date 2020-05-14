// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
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
    public struct Rgb32 : IPackedPixel<Rgb32, uint>
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

        [CLSCompliant(false)]
        public Rgb32(uint packedValue) : this()
        {
            PackedValue = packedValue;
        }

        public Rgb32(byte r, byte g, byte b, byte padding = default)
        {
            R = r;
            G = g;
            B = b;
            Padding = padding;
        }

        #endregion

        public readonly Vector3 ToVector3()
        {
            return new Vector3(R, G, B) / byte.MaxValue;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<Rgb32, uint>(this);
            set => Unsafe.As<Rgb32, uint>(ref this) = value;
        }

        public void FromScaledVector4(Vector4 vector)
        {
            UnsafeR.As<Rgb32, Rgb24>(this).FromScaledVector4(vector);
        }

        public readonly Vector4 ToScaledVector4()
        {
            return UnsafeR.As<Rgb32, Rgb24>(this).ToScaledVector4();
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            B = G = R = source.L;
        }

        public void FromGray16(Gray16 source)
        {
            B = G = R = PackedVectorHelper.DownScale16To8Bit(source.L);
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            B = G = R = source.L;
        }

        public void FromRgb24(Rgb24 source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

        public void FromColor(Color source)
        {
            R = source.R;
            G = source.G;
            B = source.B;
        }

        public void FromRgb48(Rgb48 source)
        {
            R = PackedVectorHelper.DownScale16To8Bit(source.R);
            G = PackedVectorHelper.DownScale16To8Bit(source.G);
            B = PackedVectorHelper.DownScale16To8Bit(source.B);
        }

        public void FromRgba64(Rgba64 source)
        {
            R = PackedVectorHelper.DownScale16To8Bit(source.R);
            G = PackedVectorHelper.DownScale16To8Bit(source.G);
            B = PackedVectorHelper.DownScale16To8Bit(source.B);
        }

        public readonly void ToColor(out Color destination)
        {
            destination.R = R;
            destination.G = G;
            destination.B = B;
            destination.A = byte.MaxValue;
        }

        #endregion

        #region Equals

        public readonly bool Equals(Rgb32 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Rgb32 other && Equals(other);
        }

        public static bool operator ==(in Rgb32 a, in Rgb32 b)
        {
            // we don't want to compare padding
            return a.R == b.R && a.G == b.G && a.B == b.B;
        }

        public static bool operator !=(in Rgb32 a, in Rgb32 b)
        {
            return !(a == b);
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Rgb32) + $"(R:{R}, G:{G}, B:{B})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => HashCode.Combine(R, G, B);

        #endregion
    }
}