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
    /// Packed vector type containing 16-bit floating-point XYZW components.
    /// <para>Ranges from [-1, -1, -1, 0] to [1, 1, 1, 1] in vector form.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HalfVector4 : IPackedPixel<HalfVector4, ulong>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float16, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Float16, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.Float16, VectorComponentChannel.Blue),
            new VectorComponent(VectorComponentType.Float16, VectorComponentChannel.Alpha));

        public static HalfVector4 One => new HalfVector4(HalfSingle.One);

        public HalfSingle X;
        public HalfSingle Y;
        public HalfSingle Z;
        public HalfSingle W;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public HalfVector4(HalfSingle x, HalfSingle y, HalfSingle z, HalfSingle w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        public HalfVector4(HalfSingle value) : this(value, value, value, value)
        {
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public HalfVector4(ulong packed) : this()
        {
            // TODO: Unsafe.SkipInit(out this)
            PackedValue = packed;
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ulong PackedValue
        {
            readonly get => UnsafeR.As<HalfVector4, ulong>(this);
            set => Unsafe.As<HalfVector4, ulong>(ref this) = value;
        }

        public void FromScaledVector(Vector3 scaledVector)
        {
            scaledVector *= 2;
            scaledVector -= Vector3.One;

            X = new HalfSingle(scaledVector.X);
            Y = new HalfSingle(scaledVector.Y);
            Z = new HalfSingle(scaledVector.Z);
            W = HalfSingle.One;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            scaledVector *= 2;
            scaledVector -= Vector4.One;

            X = new HalfSingle(scaledVector.X);
            Y = new HalfSingle(scaledVector.Y);
            Z = new HalfSingle(scaledVector.Z);
            W = new HalfSingle(scaledVector.W);
        }

        public readonly Vector3 ToScaledVector3()
        {
            var vector = ToVector3();
            vector += Vector3.One;
            vector /= 2f;
            return vector;
        }

        public readonly Vector4 ToScaledVector4()
        {
            var vector = ToVector4();
            vector += Vector4.One;
            vector /= 2f;
            return vector;
        }

        public void FromVector(Vector3 vector)
        {
            X = new HalfSingle(vector.X);
            Y = new HalfSingle(vector.Y);
            Z = new HalfSingle(vector.Z);
            W = HalfSingle.One;
        }

        public void FromVector(Vector4 vector)
        {
            X = new HalfSingle(vector.X);
            Y = new HalfSingle(vector.Y);
            Z = new HalfSingle(vector.Z);
            W = new HalfSingle(vector.W);
        }

        public readonly Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public readonly Vector4 ToVector4()
        {
            return new Vector4(ToVector3(), W);
        }

        #endregion

        #region IPixel

        public void FromAlpha(Alpha8 source) => this = One;

        public void FromAlpha(Alpha16 source) => this = One;

        public void FromAlpha(AlphaF source) => this = One;

        public Alpha8 ToAlpha8() => Alpha8.Opaque;

        public Alpha16 ToAlpha16() => Alpha16.Opaque;

        public AlphaF ToAlphaF() => AlphaF.Opaque;

        #endregion

        #region Equals

        public readonly bool Equals(HalfVector4 other)
        {
            return this == other;
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance is equal to a specified object.
        /// </summary>
        public override readonly bool Equals(object obj)
        {
            return obj is HalfVector4 other && Equals(other);
        }

        /// <summary>
        /// Compares the current instance to another to determine whether they are the same.
        /// </summary>
        public static bool operator ==(in HalfVector4 a, in HalfVector4 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        /// <summary>
        /// Compares the current instance to another to determine whether they are different.
        /// </summary>
        public static bool operator !=(in HalfVector4 a, in HalfVector4 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Returns a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(HalfVector4) + $"({ToVector4()})";

        /// <summary>
        /// Gets the hash code for the current instance.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
