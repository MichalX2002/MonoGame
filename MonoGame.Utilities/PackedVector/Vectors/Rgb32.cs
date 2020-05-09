// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
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
            readonly get => UnsafeUtils.As<Rgb32, Rgb24>(this);
            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
            }
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

        public Rgb32(Vector4 vector) : this()
        {
            FromVector4(vector);
        }

        public Rgb32(Vector3 vector) : this(new Vector4(vector, 1))
        {
        }

        public Rgb32(float x, float y, float z) : this(new Vector3(x, y, z))
        {
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
            readonly get => UnsafeUtils.As<Rgb32, uint>(this);
            set => Unsafe.As<Rgb32, uint>(ref this) = value;
        }

        public void FromVector4(in Vector4 vector)
        {
            UnsafeUtils.As<Rgb32, Rgb24>(this).FromVector4(vector);
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            UnsafeUtils.As<Rgb32, Rgb24>(this).ToVector4(out vector);
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            FromVector4(scaledVector);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
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

        public static bool operator ==(in Rgb32 a, in Rgb32 b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B;
        }

        public static bool operator !=(in Rgb32 a, in Rgb32 b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is Rgb32 other && Equals(other);
        }

        public bool Equals(Rgb32 other)
        {
            return this == other;
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(Rgb32) + $"(R:{R}, G:{G}, B:{B})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(R, G, B);

        #endregion
    }
}