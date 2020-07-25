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
    /// Packed vector type containing unsigned 8-bit XYZW integer components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [255, 255, 255, 255] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Byte4 : IPackedPixel<Byte4, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Blue),
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Alpha));

        public byte X;
        public byte Y;
        public byte Z;
        public byte W;

        public Color Rgba
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

        public void FromScaledVector(Vector3 scaledVector)
        {
            FromVector(scaledVector * byte.MaxValue);
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            FromVector(scaledVector * byte.MaxValue);
        }

        public void FromVector(Vector3 vector)
        {
            vector = VectorHelper.ZeroMax(vector, byte.MaxValue);

            X = (byte)vector.X;
            Y = (byte)vector.Y;
            Z = (byte)vector.Z;
        }

        public void FromVector(Vector4 vector)
        {
            vector = VectorHelper.ZeroMax(vector, byte.MaxValue);

            X = (byte)vector.X;
            Y = (byte)vector.Y;
            Z = (byte)vector.Z;
            W = (byte)vector.W;
        }

        public readonly Vector3 ToScaledVector3()
        {
            return ToVector3() / byte.MaxValue;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return ToVector4() / byte.MaxValue;
        }

        public readonly Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public readonly Vector4 ToVector4()
        {
            return new Vector4(X, Y, Z, W);
        }

        #endregion

        #region IPixel

        public void FromAlpha(Alpha8 source)
        {
            X = Y = Z = byte.MaxValue;
            W = source.A;
        }

        public void FromAlpha(Alpha16 source)
        {
            X = Y = Z = byte.MaxValue;
            W = ScalingHelper.ToUInt8(source.A);
        }

        public void FromAlpha(AlphaF source)
        {
            X = Y = Z = byte.MaxValue;
            W = ScalingHelper.ToUInt8(source.A);
        }

        public void FromGray(Gray8 source)
        {
            X = Y = Z = source.L;
            W = byte.MaxValue;
        }

        public void FromGray(Gray16 source)
        {
            X = Y = Z = ScalingHelper.ToUInt8(source.L);
            W = byte.MaxValue;
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            X = Y = Z = source.L;
            W = source.A;
        }

        public void FromRgb(Rgb24 source)
        {
            X = source.R;
            Y = source.G;
            Z = source.B;
            W = byte.MaxValue;
        }

        public void FromRgb(Rgb48 source)
        {
            X = ScalingHelper.ToUInt8(source.R);
            Y = ScalingHelper.ToUInt8(source.G);
            Z = ScalingHelper.ToUInt8(source.B);
            W = byte.MaxValue;
        }

        public void FromRgba(Rgba64 source)
        {
            X = ScalingHelper.ToUInt8(source.R);
            Y = ScalingHelper.ToUInt8(source.G);
            Z = ScalingHelper.ToUInt8(source.B);
            W = ScalingHelper.ToUInt8(source.A);
        }

        public void FromRgba(Color source)
        {
            Rgba = source;
        }

        public readonly Alpha8 ToAlpha8()
        {
            return W;
        }

        public readonly Alpha16 ToAlpha16()
        {
            return ScalingHelper.ToUInt16(W);
        }

        public readonly AlphaF ToAlphaF()
        {
            return ScalingHelper.ToFloat32(W);
        }

        public readonly Color ToColor()
        {
            return Rgba;
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
        /// Gets the hexadecimal <see cref="string"/> representation of this <see cref="ToRgba32()"/>.
        /// </summary>
        public readonly string ToHex()
        {
            uint hexOrder = (uint)(W << 0 | Z << 8 | Y << 16 | X << 24);
            return hexOrder.ToString("x8");
        }

        #region Object overrides

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

