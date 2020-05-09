// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
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
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public HalfVector3(Vector3 vector)
        {
            this = Pack(vector);
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public HalfVector3(float x, float y, float z) : this(new Vector3(x, y, z))
        {
        }

        #endregion

        private static HalfVector3 Pack(in Vector3 vector)
        {
            return new HalfVector3(
                new HalfSingle(vector.X),
                new HalfSingle(vector.Y),
                new HalfSingle(vector.Z));
        }

        public readonly Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        #region IPackedVector

        public void FromVector4(in Vector4 vector)
        {
            this = Pack(vector.XYZ);
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = X;
            vector.Base.Y = Y;
            vector.Base.Z = Z;
            vector.Base.W = 1;
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            var v = scaledVector * 2;
            v -= Vector4.One;
            FromVector4(v);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
            scaledVector += Vector4.One;
            scaledVector /= 2;
        }

        #endregion

        #region Equals

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

        /// <summary>
        /// Returns a value that indicates whether the current instance is equal to a specified object.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is HalfVector3 other && Equals(other);
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance is equal to a specified object.
        /// </summary>
        public bool Equals(HalfVector3 other)
        {
            return this == other;
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Returns a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(HalfVector3) + $"({ToVector3()})";

        /// <summary>
        /// Gets the hash code for the current instance.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        #endregion
    }
}
