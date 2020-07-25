// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing 16-bit floating-point XYZ components.
    /// <para>Ranges from [-1, -1, -1, 1] to [1, 1, 1, 1] in vector form.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HalfVector3 : IPixel<HalfVector3>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float16, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Float16, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.Float16, VectorComponentChannel.Blue));

        public static HalfVector3 One => new HalfVector3(HalfSingle.One);

        public HalfSingle X;
        public HalfSingle Y;
        public HalfSingle Z;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public HalfVector3(HalfSingle x, HalfSingle y, HalfSingle z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        public HalfVector3(HalfSingle value) : this(value, value, value)
        {
        }

        #endregion

        #region IPackedVector

        public void FromScaledVector(Vector3 scaledVector)
        {
            scaledVector *= 2;
            scaledVector -= Vector3.One;

            X = new HalfSingle(scaledVector.X);
            Y = new HalfSingle(scaledVector.Y);
            Z = new HalfSingle(scaledVector.Z);
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            FromScaledVector(scaledVector.ToVector3());
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
            return new Vector4(ToScaledVector3(), 1);
        }

        public void FromVector(Vector3 vector)
        {
            X = new HalfSingle(vector.X);
            Y = new HalfSingle(vector.Y);
            Z = new HalfSingle(vector.Z);
        }

        public void FromVector(Vector4 vector)
        {
            FromVector(vector.ToVector3());
        }

        public readonly Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public readonly Vector4 ToVector4()
        {
            return new Vector4(ToVector3(), 1);
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

        /// <summary>
        /// Returns a value that indicates whether the current instance is equal to a specified object.
        /// </summary>
        public override readonly bool Equals(object obj)
        {
            return obj is HalfVector3 other && Equals(other);
        }

        public readonly bool Equals(HalfVector3 other)
        {
            return this == other;
        }

        /// <summary>
        /// Compares the current instance to another to determine whether they are the same.
        /// </summary>
        public static bool operator ==(in HalfVector3 a, in HalfVector3 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        /// <summary>
        /// Compares the current instance to another to determine whether they are different.
        /// </summary>
        public static bool operator !=(in HalfVector3 a, in HalfVector3 b)
        {
            return !(a == b);
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Returns a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(HalfVector3) + $"({ToScaledVector3()})";

        /// <summary>
        /// Gets the hash code for the current instance.
        /// </summary>
        public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z);

        #endregion
    }
}
