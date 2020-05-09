// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
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

        public Color Rgba
        {
            readonly get => UnsafeUtils.As<Byte4, Color>(this);
            set => Unsafe.As<Byte4, Color>(ref this) = value;
        }

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Byte4(uint packed) : this()
        {
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

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Byte4(Vector4 vector) : this()
        {
            FromVector4(vector);
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Byte4(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeUtils.As<Byte4, uint>(this);
            set => Unsafe.As<Byte4, uint>(ref this) = value;
        }

        public void FromVector4(in Vector4 vector)
        {
            var v = Vector4.Clamp(vector, Vector4.Zero, Vector4.MaxValueByte);

            X = (byte)v.X;
            Y = (byte)v.Y;
            Z = (byte)v.Z;
            W = (byte)v.W;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = X;
            vector.Base.Y = Y;
            vector.Base.Z = Z;
            vector.Base.W = W;
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            var v = scaledVector * byte.MaxValue;
            FromVector4(v);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
            scaledVector /= byte.MaxValue;
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
            Rgba = source;
        }

        public readonly void ToColor(out Color destination)
        {
            destination = Rgba;
        }

        #endregion

        #region Equals

        public static bool operator ==(in Byte4 a, in Byte4 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in Byte4 a, in Byte4 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        public bool Equals(Byte4 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Byte4 other && Equals(other);
        }

        #endregion

        /// <summary>
        /// Gets the hexadecimal <see cref="string"/> representation of this <see cref="Rgba"/>.
        /// </summary>
        public string ToHex()
        {
            uint hexOrder = (uint)(W << 0 | Z << 8 | Y << 16 | X << 24);
            return hexOrder.ToString("x8");
        }

        #region Object Overrides

        /// <summary>
        /// Returns a <see cref="string"/> representation of this packed vector.
        /// </summary>
        public override string ToString() => nameof(Byte4) + $"({X}, {Y}, {Z}, {W})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}

