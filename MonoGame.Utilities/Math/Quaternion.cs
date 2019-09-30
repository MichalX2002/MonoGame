// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MonoGame.Framework
{
    /// <summary>
    /// An efficient mathematical representation for three dimensional rotations.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct Quaternion : IEquatable<Quaternion>
    {
        #region Public Fields

        /// <summary>
        /// The x coordinate of this <see cref="Quaternion"/>.
        /// </summary>
        [DataMember]
        public float X;

        /// <summary>
        /// The y coordinate of this <see cref="Quaternion"/>.
        /// </summary>
        [DataMember]
        public float Y;

        /// <summary>
        /// The z coordinate of this <see cref="Quaternion"/>.
        /// </summary>
        [DataMember]
        public float Z;

        /// <summary>
        /// The rotation component of this <see cref="Quaternion"/>.
        /// </summary>
        [DataMember]
        public float W;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a quaternion with X, Y, Z and W from four values.
        /// </summary>
        /// <param name="x">The x coordinate in 3d-space.</param>
        /// <param name="y">The y coordinate in 3d-space.</param>
        /// <param name="z">The z coordinate in 3d-space.</param>
        /// <param name="w">The rotation component.</param>
        public Quaternion(float x, float y, float z, float w)
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
        /// <param name="w">The rotation component.</param>
        public Quaternion(Vector3 value, float w)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = w;
        }

        /// <summary>
        /// Constructs a quaternion from <see cref="Vector4"/>.
        /// </summary>
        /// <param name="value">The x, y, z coordinates in 3d-space and the rotation component.</param>
        public Quaternion(Vector4 value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = value.W;
        }

        #endregion

        /// <summary>
        /// Returns a quaternion representing no rotation.
        /// </summary>
        public static readonly Quaternion Identity = new Quaternion(0, 0, 0, 1);

        #region Internal Properties

        internal string DebugDisplayString
        {
            get
            {
                if (this == Identity)
                    return "Identity";

                return string.Concat(
                    X.ToString(), " ",
                    Y.ToString(), " ",
                    Z.ToString(), " ",
                    W.ToString());
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> that contains the sum of two quaternions.
        /// </summary>
        /// <param name="a">Source <see cref="Quaternion"/>.</param>
        /// <param name="b">Source <see cref="Quaternion"/>.</param>
        /// <returns>The result of the quaternion addition.</returns>
        public static Quaternion Add(in Quaternion a, in Quaternion b) => a + b;

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> that contains concatenation between two quaternion.
        /// </summary>
        /// <param name="left">The first <see cref="Quaternion"/> to concatenate.</param>
        /// <param name="right">The second <see cref="Quaternion"/> to concatenate.</param>
        /// <returns>The result of rotation of <paramref name="left"/> followed by <paramref name="right"/> rotation.</returns>
        public static Quaternion Concatenate(in Quaternion left, in Quaternion right)
        {
            return new Quaternion(
                (right.X * left.W) + (left.X * right.W) + ((right.Y * left.Z) - (right.Z * left.Y)),
                (right.Y * left.W) + (left.Y * right.W) + ((right.Z * left.X) - (right.X * left.Z)),
                (right.Z * left.W) + (left.Z * right.W) + ((right.X * left.Y) - (right.Y * left.X)),
                (right.W * left.W) - ((right.X * left.X) + (right.Y * left.Y) + (right.Z * left.Z)));
        }

        /// <summary>
        /// Transforms this quaternion into its conjugated version.
        /// </summary>
        public void Conjugate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
        }

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> that contains conjugated version of the specified quaternion.
        /// </summary>
        /// <param name="value">The quaternion which values will be used to create the conjugated version.</param>
        /// <returns>The conjugate version of the specified quaternion.</returns>
        public static Quaternion Conjugate(in Quaternion value)
        {
            return new Quaternion(-value.X, -value.Y, -value.Z, value.W);
        }

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> from the specified axis and angle.
        /// </summary>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle in radians.</param>
        /// <returns>The new quaternion builded from axis and angle.</returns>
        public static Quaternion CreateFromAxisAngle(in Vector3 axis, float angle)
        {
            float half = angle * 0.5f;
            float sin = (float)Math.Sin(half);
            float cos = (float)Math.Cos(half);
            return new Quaternion(axis.X * sin, axis.Y * sin, axis.Z * sin, cos);
        }

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> from the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <returns>A quaternion composed from the rotation part of the matrix.</returns>
        public static Quaternion CreateFromRotationMatrix(in Matrix matrix)
        {
            Quaternion quaternion;
            float sqrt;
            float half;

            float scale = matrix.M11 + matrix.M22 + matrix.M33;
            if (scale > 0f)
            {
                sqrt = (float)Math.Sqrt(scale + 1f);
                quaternion.W = sqrt * 0.5f;
                sqrt = 0.5f / sqrt;

                quaternion.X = (matrix.M23 - matrix.M32) * sqrt;
                quaternion.Y = (matrix.M31 - matrix.M13) * sqrt;
                quaternion.Z = (matrix.M12 - matrix.M21) * sqrt;
                return quaternion;
            }

            if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                sqrt = (float)Math.Sqrt(1f + matrix.M11 - matrix.M22 - matrix.M33);
                half = 0.5f / sqrt;

                quaternion.X = 0.5f * sqrt;
                quaternion.Y = (matrix.M12 + matrix.M21) * half;
                quaternion.Z = (matrix.M13 + matrix.M31) * half;
                quaternion.W = (matrix.M23 - matrix.M32) * half;
                return quaternion;
            }

            if (matrix.M22 > matrix.M33)
            {
                sqrt = (float)Math.Sqrt(1f + matrix.M22 - matrix.M11 - matrix.M33);
                half = 0.5f / sqrt;

                quaternion.X = (matrix.M21 + matrix.M12) * half;
                quaternion.Y = 0.5f * sqrt;
                quaternion.Z = (matrix.M32 + matrix.M23) * half;
                quaternion.W = (matrix.M31 - matrix.M13) * half;
                return quaternion;
            }

            sqrt = (float)Math.Sqrt(1f + matrix.M33 - matrix.M11 - matrix.M22);
            half = 0.5f / sqrt;

            quaternion.X = (matrix.M31 + matrix.M13) * half;
            quaternion.Y = (matrix.M32 + matrix.M23) * half;
            quaternion.Z = 0.5f * sqrt;
            quaternion.W = (matrix.M12 - matrix.M21) * half;
            return quaternion;
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
            float halfRoll = roll * 0.5f;
            float halfPitch = pitch * 0.5f;
            float halfYaw = yaw * 0.5f;

            float sinRoll = (float)Math.Sin(halfRoll);
            float cosRoll = (float)Math.Cos(halfRoll);
            float sinPitch = (float)Math.Sin(halfPitch);
            float cosPitch = (float)Math.Cos(halfPitch);
            float sinYaw = (float)Math.Sin(halfYaw);
            float cosYaw = (float)Math.Cos(halfYaw);

            return new Quaternion(
                (cosYaw * sinPitch * cosRoll) + (sinYaw * cosPitch * sinRoll),
                (sinYaw * cosPitch * cosRoll) - (cosYaw * sinPitch * sinRoll),
                (cosYaw * cosPitch * sinRoll) - (sinYaw * sinPitch * cosRoll),
                (cosYaw * cosPitch * cosRoll) + (sinYaw * sinPitch * sinRoll));
        }

        /// <summary>
        /// Divides a <see cref="Quaternion"/> by the other <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="a">Source <see cref="Quaternion"/>.</param>
        /// <param name="b">Divisor <see cref="Quaternion"/>.</param>
        /// <returns>The result of dividing the quaternions.</returns>
        public static Quaternion Divide(in Quaternion a, in Quaternion b)
        {
            float n14 =
                (b.X * b.X) +
                (b.Y * b.Y) +
                (b.Z * b.Z) +
                (b.W * b.W);

            float n5 = 1f / n14;
            float n4 = -b.X * n5;
            float n3 = -b.Y * n5;
            float n2 = -b.Z * n5;
            float n1 = b.W * n5;
            float n13 = (a.Y * n2) - (a.Z * n3);
            float n12 = (a.Z * n4) - (a.X * n2);
            float n11 = (a.X * n3) - (a.Y * n4);
            float n10 = (a.X * n4) + (a.Y * n3) + (a.Z * n2);

            return new Quaternion(
                (a.X * n1) + (n4 * a.W) + n13,
                (a.Y * n1) + (n3 * a.W) + n12,
                (a.Z * n1) + (n2 * a.W) + n11,
                (a.W * n1) - n10);
        }

        /// <summary>
        /// Returns a dot product of two quaternions.
        /// </summary>
        /// <param name="a">The first quaternion.</param>
        /// <param name="b">The second quaternion.</param>
        /// <returns>The dot product of two quaternions.</returns>
        public static float Dot(in Quaternion a, in Quaternion b)
        {
            return (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z) + (a.W * b.W);
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is Quaternion other ? this == other : false;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="other">The <see cref="Quaternion"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public bool Equals(Quaternion other)
        {
            return this == other;
        }

        /// <summary>
        /// Gets the hash code of this <see cref="Quaternion"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="Quaternion"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int code = X.GetHashCode();
                code = code * 23 + Y.GetHashCode();
                code = code * 23 + Z.GetHashCode();
                code = code * 23 + W.GetHashCode();
                return code;
            }
        }

        /// <summary>
        /// Returns the inverse quaternion which represents the opposite rotation.
        /// </summary>
        /// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
        /// <returns>The inverse quaternion.</returns>
        public static Quaternion Inverse(in Quaternion quaternion)
        {
            float num2 =
                (quaternion.X * quaternion.X) +
                (quaternion.Y * quaternion.Y) +
                (quaternion.Z * quaternion.Z) +
                (quaternion.W * quaternion.W);

            float num = 1f / num2;

            return new Quaternion(
                -quaternion.X * num,
                -quaternion.Y * num,
                -quaternion.Z * num,
                quaternion.W * num);
        }

        /// <summary>
        /// Returns the magnitude of the quaternion components.
        /// </summary>
        /// <returns>The magnitude of the quaternion components.</returns>
        public float Length()
        {
            return (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
        }

        /// <summary>
        /// Returns the squared magnitude of the quaternion components.
        /// </summary>
        /// <returns>The squared magnitude of the quaternion components.</returns>
        public float LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z) + (W * W);
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
            float num = amount;
            float num2 = 1f - num;
            Quaternion quaternion = new Quaternion();
            float num5 = (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z) + (a.W * b.W);
            if (num5 >= 0f)
            {
                quaternion.X = (num2 * a.X) + (num * b.X);
                quaternion.Y = (num2 * a.Y) + (num * b.Y);
                quaternion.Z = (num2 * a.Z) + (num * b.Z);
                quaternion.W = (num2 * a.W) + (num * b.W);
            }
            else
            {
                quaternion.X = (num2 * a.X) - (num * b.X);
                quaternion.Y = (num2 * a.Y) - (num * b.Y);
                quaternion.Z = (num2 * a.Z) - (num * b.Z);
                quaternion.W = (num2 * a.W) - (num * b.W);
            }
            float num4 =
                (quaternion.X * quaternion.X) +
                (quaternion.Y * quaternion.Y) +
                (quaternion.Z * quaternion.Z) +
                (quaternion.W * quaternion.W);

            float num3 = 1f / ((float)Math.Sqrt(num4));
            quaternion.X *= num3;
            quaternion.Y *= num3;
            quaternion.Z *= num3;
            quaternion.W *= num3;
            return quaternion;
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
            float num2;
            float num3;
            Quaternion quaternion;
            float num = amount;
            float num4 =
                (a.X * b.X) +
                (a.Y * b.Y) +
                (a.Z * b.Z) +
                (a.W * b.W);

            bool flag = false;
            if (num4 < 0f)
            {
                flag = true;
                num4 = -num4;
            }
            if (num4 > 0.999999f)
            {
                num3 = 1f - num;
                num2 = flag ? -num : num;
            }
            else
            {
                float num5 = (float)Math.Acos(num4);
                float num6 = (float)(1.0 / Math.Sin(num5));
                num3 = ((float)Math.Sin((1f - num) * num5)) * num6;
                num2 = flag ? (((float)-Math.Sin(num * num5)) * num6) : (((float)Math.Sin(num * num5)) * num6);
            }
            quaternion.X = (num3 * a.X) + (num2 * b.X);
            quaternion.Y = (num3 * a.Y) + (num2 * b.Y);
            quaternion.Z = (num3 * a.Z) + (num2 * b.Z);
            quaternion.W = (num3 * a.W) + (num2 * b.W);
            return quaternion;
        }

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> that contains subtraction of one <see cref="Quaternion"/> from another.
        /// </summary>
        /// <param name="left">Source <see cref="Quaternion"/>.</param>
        /// <param name="right">Source <see cref="Quaternion"/>.</param>
        /// <returns>The result of the quaternion subtraction.</returns>
        public static Quaternion Subtract(in Quaternion left, in Quaternion right)
        {
            return left - right;
        }

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> that contains a multiplication of <see cref="Quaternion"/> and a scalar.
        /// </summary>
        /// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>The result of the quaternion multiplication with a scalar.</returns>
        public static Quaternion Multiply(in Quaternion quaternion, float scaleFactor)
        {
            return quaternion * scaleFactor;
        }

        /// <summary>
        /// Creates a new <see cref="Quaternion"/> that contains a multiplication of two quaternions.
        /// </summary>
        /// <param name="left">Source <see cref="Quaternion"/>.</param>
        /// <param name="right">Source <see cref="Quaternion"/>.</param>
        /// <returns>The result of the quaternion multiplication.</returns>
        public static Quaternion Multiply(in Quaternion left, in Quaternion right)
        {
            return left * right;
        }

        #endregion

        /// <summary>
        /// Flips the sign of the all the quaternion components.
        /// </summary>
        /// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
        /// <returns>The result of the quaternion negation.</returns>
        public static Quaternion Negate(in Quaternion quaternion)
        {
            return -quaternion;
        }

        /// <summary>
        /// Scales the quaternion magnitude to unit length.
        /// </summary>
        public void Normalize()
        {
            float num = 1f / ((float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W)));
            X *= num;
            Y *= num;
            Z *= num;
            W *= num;
        }

        /// <summary>
        /// Scales the quaternion magnitude to unit length.
        /// </summary>
        /// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
        /// <returns>The unit length quaternion.</returns>
        public static Quaternion Normalize(in Quaternion quaternion)
        {
            float num = 1f / ((float)Math.Sqrt(
                (quaternion.X * quaternion.X) +
                (quaternion.Y * quaternion.Y) +
                (quaternion.Z * quaternion.Z) + 
                (quaternion.W * quaternion.W)));

            return new Quaternion(quaternion.X * num, quaternion.Y * num, quaternion.Z * num, quaternion.W * num);
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Quaternion"/> in the format:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Z:[<see cref="Z"/>] W:[<see cref="W"/>]}
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Quaternion"/>.</returns>
        public override string ToString()
        {
            return "{X:" + X + " Y:" + Y + " Z:" + Z + " W:" + W + "}";
        }

        /// <summary>
        /// Gets a <see cref="Vector4"/> representation for this object.
        /// </summary>
        /// <returns>A <see cref="Vector4"/> representation for this object.</returns>
        public Vector4 ToVector4()
        {
            return new Vector4(X, Y, Z, W);
        }

        public void Deconstruct(out float x, out float y, out float z, out float w)
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
            return new Quaternion(
                a.X + b.X,
                a.Y + b.Y,
                a.Z + b.Z,
                a.W + b.W);
        }

        /// <summary>
        /// Divides a <see cref="Quaternion"/> by the other <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Quaternion"/> on the left of the div sign.</param>
        /// <param name="right">Divisor <see cref="Quaternion"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the quaternions.</returns>
        public static Quaternion operator /(in Quaternion left, in Quaternion right)
        {
            float x = left.X;
            float y = left.Y;
            float z = left.Z;
            float w = left.W;
            float num14 = (right.X * right.X) + (right.Y * right.Y) + (right.Z * right.Z) + (right.W * right.W);
            float num5 = 1f / num14;
            float num4 = -right.X * num5;
            float num3 = -right.Y * num5;
            float num2 = -right.Z * num5;
            float num = right.W * num5;
            float num13 = (y * num2) - (z * num3);
            float num12 = (z * num4) - (x * num2);
            float num11 = (x * num3) - (y * num4);
            float num10 = (x * num4) + (y * num3) + (z * num2);

            return new Quaternion(
                (x * num) + (num4 * w) + num13,
                (y * num) + (num3 * w) + num12,
                (z * num) + (num2 * w) + num11,
                (w * num) - num10);
        }

        /// <summary>
        /// Compares whether two <see cref="Quaternion"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Quaternion"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Quaternion"/> instance on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(in Quaternion a, in Quaternion b)
        {
            return (a.X == b.X) 
                && (a.Y == b.Y) 
                && (a.Z == b.Z)
                && (a.W == b.W);
        }

        /// <summary>
        /// Compares whether two <see cref="Quaternion"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Quaternion"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Quaternion"/> instance on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(in Quaternion a, in Quaternion b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Multiplies two quaternions.
        /// </summary>
        /// <param name="left">Source <see cref="Quaternion"/> on the left of the mul sign.</param>
        /// <param name="right">Source <see cref="Quaternion"/> on the right of the mul sign.</param>
        /// <returns>Result of the quaternions multiplication.</returns>
        public static Quaternion operator *(in Quaternion left, in Quaternion right)
        {
            float n1 = (left.Y * right.Z) - (left.Z * right.Y);
            float n2 = (left.Z * right.X) - (left.X * right.Z);
            float n3 = (left.X * right.Y) - (left.Y * right.X);
            float n4 = (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);

            return new Quaternion(
                (left.X * right.W) + (right.X * left.W) + n1,
                (left.Y * right.W) + (right.Y * left.W) + n2,
                (left.Z * right.W) + (right.Z * left.W) + n3,
                (left.W * right.W) - n4);
        }

        /// <summary>
        /// Multiplies the components of quaternion by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Quaternion"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the quaternion multiplication with a scalar.</returns>
        public static Quaternion operator *(in Quaternion value, float scaleFactor)
        {
            return new Quaternion(
                value.X * scaleFactor,
                value.Y * scaleFactor,
                value.Z * scaleFactor,
                value.W * scaleFactor);
        }

        /// <summary>
        /// Subtracts a <see cref="Quaternion"/> from a <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Quaternion"/> on the left of the sub sign.</param>
        /// <param name="right">Source <see cref="Quaternion"/> on the right of the sub sign.</param>
        /// <returns>Result of the quaternion subtraction.</returns>
        public static Quaternion operator -(in Quaternion left, in Quaternion right)
        {
            return new Quaternion(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z,
                left.W - right.W);

        }

        /// <summary>
        /// Flips the sign of the all the quaternion components.
        /// </summary>
        /// <param name="value">Source <see cref="Quaternion"/> on the right of the sub sign.</param>
        /// <returns>The result of the quaternion negation.</returns>
        public static Quaternion operator -(in Quaternion value)
        {
            return new Quaternion(-value.X, -value.Y, -value.Z, -value.W);
        }

        #endregion
    }
}