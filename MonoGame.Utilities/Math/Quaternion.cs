// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using FastQuaternion = System.Numerics.Quaternion;

namespace MonoGame.Framework
{
    /// <summary>
    /// An efficient mathematical representation for three dimensional rotations.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Quaternion : IEquatable<Quaternion>
    {
        /// <summary>
        /// Gets a quaternion representing no rotation (0, 0, 0, 1).
        /// </summary>
        public static Quaternion Identity { get; } = FastQuaternion.Identity;

        [IgnoreDataMember]
        public FastQuaternion Base;

        #region Properties

        /// <summary>
        /// The x coordinate of this <see cref="Quaternion"/>.
        /// </summary>
        [DataMember]
        public float X { readonly get => Base.X; set => Base.X = value; }

        /// <summary>
        /// The y coordinate of this <see cref="Quaternion"/>.
        /// </summary>
        [DataMember]
        public float Y { readonly get => Base.Y; set => Base.Y = value; }

        /// <summary>
        /// The z coordinate of this <see cref="Quaternion"/>.
        /// </summary>
        [DataMember]
        public float Z { readonly get => Base.Z; set => Base.Z = value; }

        /// <summary>
        /// The rotation component of this <see cref="Quaternion"/>.
        /// </summary>
        [DataMember]
        public float W { readonly get => Base.W; set => Base.W = value; }

        /// <summary>
        /// Gets whether the current instance is the identity quaternion.
        /// </summary>
        [IgnoreDataMember]
        public readonly bool IsIdentity => Base.IsIdentity;

        internal string DebuggerDisplay => IsIdentity ? "Identity" : string.Concat(
            X.ToString(), " ",
            Y.ToString(), " ",
            Z.ToString(), " ",
            W.ToString());

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a quaternion with X, Y, Z and W from four values.
        /// </summary>
        /// <param name="x">The x coordinate in 3d-space.</param>
        /// <param name="y">The y coordinate in 3d-space.</param>
        /// <param name="z">The z coordinate in 3d-space.</param>
        /// <param name="w">The rotation component.</param>
        public Quaternion(float x, float y, float z, float w) : this()
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Constructs a quaternion with X, Y, Z from <see cref="Vector3"/> and rotation component from a scalar.
        /// </summary>
        /// <param name="value">The x, y, z coordinates in 3d-space.</param>
        /// <param name="scalarPart">The rotation component.</param>
        public Quaternion(in Vector3 value, float scalarPart) : 
            this(value.X, value.Y, value.Z, scalarPart)
        {
        }

        /// <summary>
        /// Constructs a quaternion from <see cref="Vector4"/>.
        /// </summary>
        /// <param name="value">The x, y, z coordinates in 3d-space and the rotation component.</param>
        public Quaternion(in Vector4 value) : 
            this(value.X, value.Y, value.Z, value.W)
        {
        }

        #endregion

        #region ToVector4

        /// <summary>
        /// Gets a <see cref="Vector4"/> representation for this object.
        /// </summary>
        public readonly Vector4 ToVector4() => UnsafeUtils.As<Quaternion, Vector4>(this);
        
        #endregion

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> that contains the sum of two quaternions.
        /// </summary>
        /// <param name="a">Source <see cref="Quaternion"/>.</param>
        /// <param name="b">Source <see cref="Quaternion"/>.</param>
        /// <returns>The result of the quaternion addition.</returns>
        public static Quaternion Add(in Quaternion a, in Quaternion b)
        {
            return FastQuaternion.Add(a, b);
        }

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> that contains concatenation between two quaternion.
        /// </summary>
        /// <param name="left">The first <see cref="Quaternion"/> to concatenate.</param>
        /// <param name="right">The second <see cref="Quaternion"/> to concatenate.</param>
        /// <returns>The result of rotation of <paramref name="left"/> followed by <paramref name="right"/> rotation.</returns>
        public static Quaternion Concatenate(in Quaternion left, in Quaternion right)
        {
            return FastQuaternion.Concatenate(left, right);
        }

        #region Conjugate

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> that contains conjugated version of the specified quaternion.
        /// </summary>
        /// <param name="value">The quaternion which values will be used to create the conjugated version.</param>
        /// <returns>The conjugate version of the specified quaternion.</returns>
        public static Quaternion Conjugate(in Quaternion value)
        {
            return FastQuaternion.Conjugate(value);
        }

        /// <summary>
        /// Transforms this quaternion into its conjugated version.
        /// </summary>
        public void Conjugate()
        {
            this = Conjugate(this);
        }

        #endregion

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> from the specified axis and angle.
        /// </summary>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle in radians.</param>
        /// <returns>The new quaternion builded from axis and angle.</returns>
        public static Quaternion CreateFromAxisAngle(in Vector3 axis, float angle)
        {
            return FastQuaternion.CreateFromAxisAngle(axis, angle);
        }

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> from the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <returns>A quaternion composed from the rotation part of the matrix.</returns>
        public static Quaternion CreateFromRotationMatrix(in Matrix matrix)
        {
            return FastQuaternion.CreateFromRotationMatrix(matrix);
        }

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> from the specified yaw, pitch and roll angles.
        /// </summary>
        /// <param name="yaw">Yaw around the y axis in radians.</param>
        /// <param name="pitch">Pitch around the x axis in radians.</param>
        /// <param name="roll">Roll around the z axis in radians.</param>
        /// <returns>A new quaternion from the concatenated yaw, pitch, and roll angles.</returns>
        public static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            return FastQuaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
        }

        /// <summary>
        /// Divides a <see cref="Quaternion"/> by the other <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="a">Source <see cref="Quaternion"/>.</param>
        /// <param name="b">Divisor <see cref="Quaternion"/>.</param>
        /// <returns>The result of dividing the quaternions.</returns>
        public static Quaternion Divide(in Quaternion a, in Quaternion b)
        {
            return FastQuaternion.Divide(a, b);
        }

        /// <summary>
        /// Returns a dot product of two quaternions.
        /// </summary>
        /// <param name="a">The first quaternion.</param>
        /// <param name="b">The second quaternion.</param>
        /// <returns>The dot product of two quaternions.</returns>
        public static float Dot(in Quaternion a, in Quaternion b)
        {
            return FastQuaternion.Dot(a, b);
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is Quaternion other ? this == other : false;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="other">The <see cref="Quaternion"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public bool Equals(Quaternion other)
        {
            return Base.Equals(other.Base);
        }

        /// <summary>
        /// Gets the hash code of this <see cref="Quaternion"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="Quaternion"/>.</returns>
        public override int GetHashCode() => Base.GetHashCode();

        /// <summary>
        /// Returns the inverse quaternion which represents the opposite rotation.
        /// </summary>
        /// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
        /// <returns>The inverse quaternion.</returns>
        public static Quaternion Inverse(in Quaternion quaternion)
        {
            return FastQuaternion.Inverse(quaternion);
        }

        /// <summary>
        /// Calculates the squared magnitude of the quaternion components.
        /// </summary>
        /// <returns>The squared magnitude of the quaternion components.</returns>
        public float LengthSquared()
        {
            return Base.LengthSquared();
        }

        /// <summary>
        /// Calculates the magnitude of the quaternion components.
        /// </summary>
        /// <returns>The magnitude of the quaternion components.</returns>
        public float Length()
        {
            return Base.Length();
        }

        /// <summary>
        /// Performs a linear blend between two quaternions.
        /// </summary>
        /// <param name="a">Source <see cref="Quaternion"/>.</param>
        /// <param name="b">Source <see cref="Quaternion"/>.</param>
        /// <param name="amount">The blend amount where 0 returns <paramref name="a"/> and 1 <paramref name="b"/>.</param>
        /// <returns>The result of linear blending between two quaternions.</returns>
        public static Quaternion Lerp(in Quaternion a, in Quaternion b, float amount)
        {
            return FastQuaternion.Lerp(a, b, amount);
        }

        /// <summary>
        /// Performs a spherical linear blend between two quaternions.
        /// </summary>
        /// <param name="a">Source <see cref="Quaternion"/>.</param>
        /// <param name="b">Source <see cref="Quaternion"/>.</param>
        /// <param name="amount">The blend amount where 0.0 returns <paramref name="a"/> and 1.0 <paramref name="b"/>.</param>
        /// <returns>The result of spherical linear blending between two quaternions.</returns>
        public static Quaternion Slerp(in Quaternion a, in Quaternion b, float amount)
        {
            return FastQuaternion.Slerp(a,b, amount);
        }

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> that contains subtraction of one <see cref="Quaternion"/> from another.
        /// </summary>
        /// <param name="left">Source <see cref="Quaternion"/>.</param>
        /// <param name="right">Source <see cref="Quaternion"/>.</param>
        /// <returns>The result of the quaternion subtraction.</returns>
        public static Quaternion Subtract(in Quaternion left, in Quaternion right)
        {
            return FastQuaternion.Subtract(left, right);
        }

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> that contains a multiplication of <see cref="Quaternion"/> and a scalar.
        /// </summary>
        /// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>The result of the quaternion multiplication with a scalar.</returns>
        public static Quaternion Multiply(in Quaternion quaternion, float scaleFactor)
        {
            return FastQuaternion.Multiply(quaternion, scaleFactor);
        }

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> that contains a multiplication of two quaternions.
        /// </summary>
        /// <param name="left">Source <see cref="Quaternion"/>.</param>
        /// <param name="right">Source <see cref="Quaternion"/>.</param>
        /// <returns>The result of the quaternion multiplication.</returns>
        public static Quaternion Multiply(in Quaternion left, in Quaternion right)
        {
            return FastQuaternion.Multiply(left, right);
        }

        /// <summary>
        /// Flips the sign of the all the quaternion components.
        /// </summary>
        /// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
        /// <returns>The result of the quaternion negation.</returns>
        public static Quaternion Negate(in Quaternion quaternion)
        {
            return FastQuaternion.Negate(quaternion);
        }

        #region Normalize

        /// <summary>
        /// Scales the quaternion magnitude to unit length.
        /// </summary>
        /// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
        /// <returns>The unit length quaternion.</returns>
        public static Quaternion Normalize(in Quaternion quaternion)
        {
            return FastQuaternion.Normalize(quaternion);
        }

        /// <summary>
        /// Scales the quaternion magnitude to unit length.
        /// </summary>
        public void Normalize()
        {
            this = Normalize(this);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Quaternion"/>.
        /// </summary>
        public override string ToString() => Base.ToString();

        public readonly void Deconstruct(out float x, out float y, out float z, out float w)
        {
            x = X;
            y = Y;
            z = Z;
            w = W;
        }

        #region Operators

        /// <summary>
        /// Adds two quaternions.
        /// </summary>
        /// <param name="a">Source <see cref="Quaternion"/> on the left of the add sign.</param>
        /// <param name="b">Source <see cref="Quaternion"/> on the right of the add sign.</param>
        /// <returns>Sum of the vectors.</returns>
        public static Quaternion operator +(in Quaternion a, in Quaternion b)
        {
            return a.Base + b.Base;
        }

        /// <summary>
        /// Divides a <see cref="Quaternion"/> by the other <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Quaternion"/> on the left of the div sign.</param>
        /// <param name="right">Divisor <see cref="Quaternion"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the quaternions.</returns>
        public static Quaternion operator /(in Quaternion left, in Quaternion right)
        {
            return left.Base / right.Base;
        }

        /// <summary>
        /// Compares whether two <see cref="Quaternion"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Quaternion"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Quaternion"/> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(in Quaternion a, in Quaternion b)
        {
            return a.Base == b.Base;
        }

        /// <summary>
        /// Compares whether two <see cref="Quaternion"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Quaternion"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Quaternion"/> instance on the right of the not equal sign.</param>
        /// <returns><see langword="true"/> if the instances are not equal; <see langword="false"/> otherwise.</returns>
        public static bool operator !=(in Quaternion a, in Quaternion b)
        {
            return a.Base != b.Base;
        }

        /// <summary>
        /// Multiplies two quaternions.
        /// </summary>
        /// <param name="left">Source <see cref="Quaternion"/> on the left of the mul sign.</param>
        /// <param name="right">Source <see cref="Quaternion"/> on the right of the mul sign.</param>
        /// <returns>Result of the quaternions multiplication.</returns>
        public static Quaternion operator *(in Quaternion left, in Quaternion right)
        {
            return left.Base * right.Base;
        }

        /// <summary>
        /// Multiplies the components of quaternion by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Quaternion"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the quaternion multiplication with a scalar.</returns>
        public static Quaternion operator *(in Quaternion value, float scaleFactor)
        {
            return value.Base * scaleFactor;
        }

        /// <summary>
        /// Subtracts a <see cref="Quaternion"/> from a <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Quaternion"/> on the left of the sub sign.</param>
        /// <param name="right">Source <see cref="Quaternion"/> on the right of the sub sign.</param>
        /// <returns>Result of the quaternion subtraction.</returns>
        public static Quaternion operator -(in Quaternion left, in Quaternion right)
        {
            return left.Base - right.Base;
        }

        /// <summary>
        /// Flips the sign of the all the quaternion components.
        /// </summary>
        /// <param name="value">Source <see cref="Quaternion"/> on the right of the sub sign.</param>
        /// <returns>The result of the quaternion negation.</returns>
        public static Quaternion operator -(in Quaternion value)
        {
            return -value.Base;
        }

        public static implicit operator FastQuaternion(in Quaternion quaternion)
        {
            return quaternion.Base;
        }

        public static implicit operator Quaternion(in FastQuaternion quaternion)
        {
            return new Quaternion { Base = quaternion };
        }

        #endregion
    }
}