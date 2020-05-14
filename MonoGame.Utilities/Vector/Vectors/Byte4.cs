// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing unsigned 8-bit XYZW integer components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [255, 255, 255, 255] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Byte4 : IPackedPixel<Byte4, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Blue),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Alpha));

        public byte X;
        public byte Y;
        public byte Z;
        public byte W;

        public Color Rgba32
        {
            readonly get => UnsafeR.As<Byte4, Color>(this);
            set => Unsafe.As<Byte4, Color>(ref this) = value;
        }

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Byte4(uint packed) : this()
        {
            // TODO: Unsafe.SkipInit(out this)
            PackedValue = packed;
        }

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public Byte4(byte x, byte y, byte z, byte w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<Byte4, uint>(this);
            set => Unsafe.As<Byte4, uint>(ref this) = value;
        }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            FromVector4(scaledVector * byte.MaxValue);
        }

        public readonly Vector4 ToScaledVector4()
        {
            return ToVector4() / byte.MaxValue;
        }

        public void FromVector4(Vector4 vector)
        {
            vector.Clamp(Vector4.Zero, Vector4.MaxValueByte);

            X = (byte)vector.X;
            Y = (byte)vector.Y;
            Z = (byte)vector.Z;
            W = (byte)vector.W;
        }

        public readonly Vector4 ToVector4()
        {
            return new Vector4(X, Y, Z, W);
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            X = Y = Z = source.L;
            W = byte.MaxValue;
        }

        public void FromGray16(Gray16 source)
        {
            X = Y = Z = PackedVectorHelper.DownScale16To8Bit(source.L);
            W = byte.MaxValue;
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            X = Y = Z = source.L;
            W = source.A;
        }

        public void FromRgb24(Rgb24 source)
        {
            X = source.R;
            Y = source.G;
            Z = source.B;
            W = byte.MaxValue;
        }

        public void FromRgb48(Rgb48 source)
        {
            X = PackedVectorHelper.DownScale16To8Bit(source.R);
            Y = PackedVectorHelper.DownScale16To8Bit(source.G);
            Z = PackedVectorHelper.DownScale16To8Bit(source.B);
            W = byte.MaxValue;
        }

        public void FromRgba64(Rgba64 source)
        {
            X = PackedVectorHelper.DownScale16To8Bit(source.R);
            Y = PackedVectorHelper.DownScale16To8Bit(source.G);
            Z = PackedVectorHelper.DownScale16To8Bit(source.B);
            W = PackedVectorHelper.DownScale16To8Bit(source.A);
        }

        public void FromColor(Color source)
        {
            Rgba32 = source;
        }

        public readonly Color ToColor()
        {
            return Rgba32;
        }

        #endregion

        #region Equals

        public readonly bool Equals(Byte4 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Byte4 other && Equals(other);
        }

        public static bool operator ==(in Byte4 a, in Byte4 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in Byte4 a, in Byte4 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        /// <summary>
        /// Gets the hexadecimal <see cref="string"/> representation of this <see cref="Rgba32"/>.
        /// </summary>
        public readonly string ToHex()
        {
            uint hexOrder = (uint)(W << 0 | Z << 8 | Y << 16 | X << 24);
            return hexOrder.ToString("x8");
        }

        #region Object Overrides

        /// <summary>
        /// Returns a <see cref="string"/> representation of this packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Byte4) + $"({ToVector4()}";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}

