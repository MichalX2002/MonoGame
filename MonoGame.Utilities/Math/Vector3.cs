// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Framework
{
    /// <summary>
    /// Describes a 3D-vector.
    /// </summary>
#if XNADESIGNPROVIDED
    [System.ComponentModel.TypeConverter(typeof(Design.Vector3TypeConverter))]
#endif
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Vector3 : IEquatable<Vector3>, IPixel
    {
        #region Public Constants

        /// <summary>
        /// <see cref="Vector3"/> with all values set to <see cref="byte.MaxValue"/>.
        /// </summary>
        internal static readonly Vector3 MaxByteValue = new Vector3(byte.MaxValue);

        /// <summary>
        /// <see cref="Vector3"/> with all components set to <see cref="float.MaxValue"/>.
        /// </summary>
        public static readonly Vector3 MaxValue = new Vector3(float.MaxValue);

        /// <summary>
        /// <see cref="Vector3"/> with all components set to <see cref="float.MinValue"/>.
        /// </summary>
        public static readonly Vector3 MinValue = new Vector3(float.MinValue);

        /// <summary>
        /// <see cref="Vector3"/> with all components set to 0.
        /// </summary>
        public static readonly Vector3 Zero = new Vector3(0f);

        /// <summary>
        /// <see cref="Vector3"/> with all components set to 0.5.
        /// </summary>
        public static readonly Vector3 Half = new Vector3(0.5f);

        /// <summary>
        /// <see cref="Vector3"/> with all components set to 1.
        /// </summary>
        public static readonly Vector3 One = new Vector3(1f);

        /// <summary>
        /// <see cref="Vector3"/> with components 1, 0, 0.
        /// </summary>
        public static readonly Vector3 UnitX = new Vector3(1f, 0f, 0f);

        /// <summary>
        /// <see cref="Vector3"/> with components 0, 1, 0.
        /// </summary>
        public static readonly Vector3 UnitY = new Vector3(0f, 1f, 0f);

        /// <summary>
        /// <see cref="Vector3"/> with components 0, 0, 1.
        /// </summary>
        public static readonly Vector3 UnitZ = new Vector3(0f, 0f, 1f);

        /// <summary>
        /// <see cref="Vector3"/> with components 0, 1, 0.
        /// </summary>
        public static readonly Vector3 Up = new Vector3(0f, 1f, 0f);

        /// <summary>
        /// <see cref="Vector3"/> with components 0, -1, 0.
        /// </summary>
        public static readonly Vector3 Down = new Vector3(0f, -1f, 0f);

        /// <summary>
        /// <see cref="Vector3"/> with components 1, 0, 0.
        /// </summary>
        public static readonly Vector3 Right = new Vector3(1f, 0f, 0f);

        /// <summary>
        /// <see cref="Vector3"/> with components -1, 0, 0.
        /// </summary>
        public static readonly Vector3 Left = new Vector3(-1f, 0f, 0f);

        /// <summary>
        /// <see cref="Vector3"/> with components 0, 0, -1.
        /// </summary>
        public static readonly Vector3 Forward = new Vector3(0f, 0f, -1f);

        /// <summary>
        /// <see cref="Vector3"/> with components 0, 0, 1.
        /// </summary>
        public static readonly Vector3 Backward = new Vector3(0f, 0f, 1f);

        #endregion

        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(float) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(float) * 8),
            new VectorComponent(VectorComponentType.Blue, sizeof(float) * 8));

        /// <summary>
        /// The X coordinate of this <see cref="Vector3"/>.
        /// </summary>
        [DataMember]
        public float X;

        /// <summary>
        /// The Y coordinate of this <see cref="Vector3"/>.
        /// </summary>
        [DataMember]
        public float Y;

        /// <summary>
        /// The Z coordinate of this <see cref="Vector3"/>.
        /// </summary>
        [DataMember]
        public float Z;

        internal string DebuggerDisplay => string.Concat(
            X.ToString(), "  ",
            Y.ToString(), "  ",
            Z.ToString());

        #region Constructors

        /// <summary>
        /// Constructs a 3D vector with X, Y and Z from three values.
        /// </summary>
        /// <param name="x">The x coordinate in 3D-space.</param>
        /// <param name="y">The y coordinate in 3D-space.</param>
        /// <param name="z">The z coordinate in 3D-space.</param>
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Constructs a 3D vector with X, Y and Z set to the same value.
        /// </summary>
        /// <param name="value">The x, y and z coordinates in 3D-space.</param>
        public Vector3(float value)
        {
            X = value;
            Y = value;
            Z = value;
        }

        /// <summary>
        /// Constructs a 3D vector with X, Y from <see cref="Vector2"/> and Z from a scalar.
        /// </summary>
        /// <param name="value">The x and y coordinates in 3D-space.</param>
        /// <param name="z">The z coordinate in 3D-space.</param>
        public Vector3(in Vector2 value, float z)
        {
            X = value.X;
            Y = value.Y;
            Z = z;
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="Vector2"/> representation of this <see cref="Vector3"/>.
        /// </summary>
        public readonly Vector2 ToVector2() => UnsafeUtils.As<Vector3, Vector2>(this);

        #region IPackedVector

        public void FromVector4(Vector4 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public readonly Vector4 ToVector4() => new Vector4(X, Y, Z, 1);

        void IPackedVector.FromScaledVector4(Vector4 vector) => FromVector4(vector);

        readonly Vector4 IPackedVector.ToScaledVector4() => ToVector4();

        #endregion

        #region IPixel

        public readonly void ToColor(ref Color destination) => destination.FromVector4(ToVector4());

        void IPixel.FromGray8(Gray8 source) => FromVector4(source.ToScaledVector4());

        void IPixel.FromGray16(Gray16 source) => FromVector4(source.ToScaledVector4());

        void IPixel.FromGrayAlpha16(GrayAlpha16 source) => FromVector4(source.ToScaledVector4());

        void IPixel.FromRgb24(Rgb24 source) => FromVector4(source.ToScaledVector4());

        public void FromColor(Color source) => FromVector4(source.ToScaledVector4());

        void IPixel.FromRgb48(Rgb48 source) => FromVector4(source.ToScaledVector4());

        void IPixel.FromRgba64(Rgba64 source) => FromVector4(source.ToScaledVector4());

        #endregion

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains the cartesian
        /// coordinates of a vector specified in barycentric coordinates and relative to 3D-triangle.
        /// </summary>
        /// <param name="a">The first vector of 3D-triangle.</param>
        /// <param name="b">The second vector of 3D-triangle.</param>
        /// <param name="c">The third vector of 3D-triangle.</param>
        /// <param name="amount1">Barycentric scalar <c>b2</c> which represents a weighting factor towards second vector of 3D-triangle.</param>
        /// <param name="amount2">Barycentric scalar <c>b3</c> which represents a weighting factor towards third vector of 3D-triangle.</param>
        /// <returns>The cartesian translation of barycentric coordinates.</returns>
        public static Vector3 Barycentric(in Vector3 a, in Vector3 b, in Vector3 c, float amount1, float amount2) => new Vector3(
            MathHelper.Barycentric(a.X, b.X, c.X, amount1, amount2),
            MathHelper.Barycentric(a.Y, b.Y, c.Y, amount1, amount2),
            MathHelper.Barycentric(a.Z, b.Z, c.Z, amount1, amount2));

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains CatmullRom interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">The first vector in interpolation.</param>
        /// <param name="b">The second vector in interpolation.</param>
        /// <param name="c">The third vector in interpolation.</param>
        /// <param name="d">The fourth vector in interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of CatmullRom interpolation.</returns>
        public static Vector3 CatmullRom(in Vector3 a, in Vector3 b, in Vector3 c, in Vector3 d, float amount) => new Vector3(
            MathHelper.CatmullRom(a.X, b.X, c.X, d.X, amount),
            MathHelper.CatmullRom(a.Y, b.Y, c.Y, d.Y, amount),
            MathHelper.CatmullRom(a.Z, b.Z, c.Z, d.Z, amount));

        /// <summary>
        /// Round the members of this <see cref="Vector3"/> towards positive infinity.
        /// </summary>
        public void Ceiling()
        {
            X = MathF.Ceiling(X);
            Y = MathF.Ceiling(Y);
            Z = MathF.Ceiling(Z);
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains members from another vector rounded towards positive infinity.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The rounded <see cref="Vector3"/>.</returns>
        public static Vector3 Ceiling(in Vector3 value) => new Vector3(
            MathF.Ceiling(value.X),
            MathF.Ceiling(value.Y),
            MathF.Ceiling(value.Z));

        /// <summary>
        /// Clamps the specified value within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public static Vector3 Clamp(in Vector3 value, in Vector3 min, in Vector3 max)
        {
            return new Vector3(
                MathHelper.Clamp(value.X, min.X, max.X),
                MathHelper.Clamp(value.Y, min.Y, max.Y),
                MathHelper.Clamp(value.Z, min.Z, max.Z));
        }

        public static void Clamp(in Vector3 value, in Vector3 min, in Vector3 max, out Vector3 result)
        {
            result.X = MathHelper.Clamp(value.X, min.X, max.X);
            result.Y = MathHelper.Clamp(value.Y, min.Y, max.Y);
            result.Z = MathHelper.Clamp(value.Z, min.Z, max.Z);
        }

        /// <summary>
        /// Computes the cross product of two vectors.
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>The cross product of two vectors.</returns>
        public static Vector3 Cross(in Vector3 left, in Vector3 right) => new Vector3(
            left.Y * right.Z - right.Y * left.Z,
            -(left.X * right.Z - right.X * left.Z),
            left.X * right.Y - right.X * left.Y);

        /// <summary>
        /// Returns the distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The distance between two vectors.</returns>
        public static float Distance(in Vector3 a, in Vector3 b)
        {
            float result = DistanceSquared(a, b);
            return MathF.Sqrt(result);
        }

        /// <summary>
        /// Returns the squared distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The squared distance between two vectors.</returns>
        public static float DistanceSquared(in Vector3 a, in Vector3 b) =>
            (a.X - b.X) * (a.X - b.X) +
            (a.Y - b.Y) * (a.Y - b.Y) +
            (a.Z - b.Z) * (a.Z - b.Z);

        /// <summary>
        /// Returns a dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product of two vectors.</returns>
        public static float Dot(in Vector3 a, in Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public override bool Equals(object obj) => obj is Vector3 other && Equals(other);

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="other">The <see cref="Vector3"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public bool Equals(Vector3 other) => this == other;

        /// <summary>
        /// Round the members of this <see cref="Vector3"/> towards negative infinity.
        /// </summary>
        public void Floor()
        {
            X = MathF.Floor(X);
            Y = MathF.Floor(Y);
            Z = MathF.Floor(Z);
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains members from another vector rounded towards negative infinity.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The rounded <see cref="Vector3"/>.</returns>
        public static Vector3 Floor(in Vector3 value) => new Vector3(
            MathF.Floor(value.X),
            MathF.Floor(value.Y),
            MathF.Floor(value.Z));

        /// <summary>
        /// Gets the hash code of this <see cref="Vector3"/>.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int code = 7 + X.GetHashCode();
                code = code * 31 + Y.GetHashCode();
                return code * 31 + Z.GetHashCode();
            }
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains hermite spline interpolation.
        /// </summary>
        /// <param name="position1">The first position vector.</param>
        /// <param name="tangent1">The first tangent vector.</param>
        /// <param name="position2">The second position vector.</param>
        /// <param name="tangent2">The second tangent vector.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The hermite spline interpolation vector.</returns>
        public static Vector3 Hermite(
            in Vector3 position1, in Vector3 tangent1, in Vector3 position2, in Vector3 tangent2, float amount) => new Vector3(
            MathHelper.Hermite(position1.X, tangent1.X, position2.X, tangent2.X, amount),
            MathHelper.Hermite(position1.Y, tangent1.Y, position2.Y, tangent2.Y, amount),
            MathHelper.Hermite(position1.Z, tangent1.Z, position2.Z, tangent2.Z, amount));

        /// <summary>
        /// Returns the length of this <see cref="Vector3"/>.
        /// </summary>
        public readonly float Length() => MathF.Sqrt((X * X) + (Y * Y) + (Z * Z));

        /// <summary>
        /// Returns the squared length of this <see cref="Vector3"/>.
        /// </summary>
        public readonly float LengthSquared() => (X * X) + (Y * Y) + (Z * Z);

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains linear interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector3 Lerp(in Vector3 a, in Vector3 b, float amount) => new Vector3(
            MathHelper.Lerp(a.X, b.X, amount),
            MathHelper.Lerp(a.Y, b.Y, amount),
            MathHelper.Lerp(a.Z, b.Z, amount));

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains linear interpolation of the specified vectors.
        /// Uses <see cref="MathHelper.LerpPrecise"/> on MathHelper for the interpolation.
        /// Less efficient but more precise compared to <see cref="Lerp(Vector3, Vector3, float)"/>.
        /// See remarks section of <see cref="MathHelper.LerpPrecise"/> on MathHelper for more info.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector3 LerpPrecise(in Vector3 a, in Vector3 b, float amount) => new Vector3(
            MathHelper.LerpPrecise(a.X, b.X, amount),
            MathHelper.LerpPrecise(a.Y, b.Y, amount),
            MathHelper.LerpPrecise(a.Z, b.Z, amount));

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a maximal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector3"/> with maximal values from the two vectors.</returns>
        public static Vector3 Max(in Vector3 a, in Vector3 b) => new Vector3(
            MathHelper.Max(a.X, b.X),
            MathHelper.Max(a.Y, b.Y),
            MathHelper.Max(a.Z, b.Z));

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a minimal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector3"/> with minimal values from the two vectors.</returns>
        public static Vector3 Min(in Vector3 a, in Vector3 b) => new Vector3(
            MathHelper.Min(a.X, b.X),
            MathHelper.Min(a.Y, b.Y),
            MathHelper.Min(a.Z, b.Z));

        /// <summary>
        /// Performs vector addition on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector to add.</param>
        /// <param name="b">The second vector to add.</param>
        /// <returns>The result of the vector addition.</returns>
        public static Vector3 Add(in Vector3 a, in Vector3 b) => a + b;

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains subtraction of on <see cref="Vector3"/> from a another.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/>.</param>
        /// <param name="right">Source <see cref="Vector3"/>.</param>
        /// <returns>The result of the vector subtraction.</returns>
        public static Vector3 Subtract(in Vector3 left, in Vector3 right) => left - right;

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains the specified vector inversion.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The result of the vector inversion.</returns>
        public static Vector3 Negate(in Vector3 value) => -value;

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a multiplication of two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/>.</param>
        /// <param name="b">Source <see cref="Vector3"/>.</param>
        /// <returns>The result of the vector multiplication.</returns>
        public static Vector3 Multiply(in Vector3 a, in Vector3 b) => a * b;

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a multiplication of <see cref="Vector3"/> and a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>The result of the vector multiplication with a scalar.</returns>
        public static Vector3 Multiply(in Vector3 value, float scaleFactor) => value * scaleFactor;

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by the components of another <see cref="Vector3"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/>.</param>
        /// <param name="right">Divisor <see cref="Vector3"/>.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector3 Divide(in Vector3 left, in Vector3 right) => left / right;

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="divider">Divisor scalar.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector3 Divide(in Vector3 value, float divider) => value / divider;

        /// <summary>
        /// Turns this <see cref="Vector3"/> to a unit vector with the same direction.
        /// </summary>
        public void Normalize()
        {
            float factor = 1f / MathF.Sqrt((X * X) + (Y * Y) + (Z * Z));
            X *= factor;
            Y *= factor;
            Z *= factor;
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a normalized values from another vector.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>Unit vector.</returns>
        public static Vector3 Normalize(in Vector3 value)
        {
            float factor = 1f / MathF.Sqrt((value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z));
            return new Vector3(value.X * factor, value.Y * factor, value.Z * factor);
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains reflect vector of the given vector and normal.
        /// </summary>
        /// <param name="vector">Source <see cref="Vector3"/>.</param>
        /// <param name="normal">Reflection normal.</param>
        /// <returns>Reflected vector.</returns>
        public static Vector3 Reflect(in Vector3 vector, in Vector3 normal)
        {
            // I is the original array
            // N is the normal of the incident plane
            // R = I - (2 * N * ( DotProduct[ I,N] ))

            // inline the dotProduct here instead of calling method
            float dotProduct = (vector.X * normal.X) + (vector.Y * normal.Y) + (vector.Z * normal.Z);
            return new Vector3(
                vector.X - 2.0f * normal.X * dotProduct,
                vector.Y - 2.0f * normal.Y * dotProduct,
                vector.Z - 2.0f * normal.Z * dotProduct);
        }

        /// <summary>
        /// Round the members of this <see cref="Vector3"/> towards the nearest integer value.
        /// </summary>
        public void Round()
        {
            X = MathF.Round(X);
            Y = MathF.Round(Y);
            Z = MathF.Round(Z);
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains members from 
        /// another vector rounded to the nearest integer value.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The rounded <see cref="Vector3"/>.</returns>
        public static Vector3 Round(in Vector3 value) => new Vector3(
            MathF.Round(value.X),
            MathF.Round(value.Y),
            MathF.Round(value.Z));

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains cubic interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/>.</param>
        /// <param name="b">Source <see cref="Vector3"/>.</param>
        /// <param name="amount">Weighting value.</param>
        /// <returns>Cubic interpolation of the specified vectors.</returns>
        public static Vector3 SmoothStep(in Vector3 a, in Vector3 b, float amount) => new Vector3(
            MathHelper.SmoothStep(a.X, b.X, amount),
            MathHelper.SmoothStep(a.Y, b.Y, amount),
            MathHelper.SmoothStep(a.Z, b.Z, amount));

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Vector3"/> in the format:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Z:[<see cref="Z"/>]}
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Vector3"/>.</returns>
        public override string ToString() => "{X:" + X + " Y:" + Y + " Z:" + Z + "}";

        #region Transform

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a transformation of 3D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">Source <see cref="Vector3"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector3"/>.</returns>
        public static Vector3 Transform(in Vector3 position, in Matrix matrix)
        {
            return new Vector3(
                (position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41,
                (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42,
                (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43);
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a transformation of 3D-vector by the specified <see cref="Quaternion"/>, representing the rotation.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <returns>Transformed <see cref="Vector3"/>.</returns>
        public static Vector3 Transform(in Vector3 value, in Quaternion rotation)
        {
            float x = 2 * (rotation.Y * value.Z - rotation.Z * value.Y);
            float y = 2 * (rotation.Z * value.X - rotation.X * value.Z);
            float z = 2 * (rotation.X * value.Y - rotation.Y * value.X);

            return new Vector3(
                value.X + x * rotation.W + (rotation.Y * z - rotation.Z * y),
                value.Y + y * rotation.W + (rotation.Z * x - rotation.X * z),
                value.Z + z * rotation.W + (rotation.X * y - rotation.Y * x));
        }

        /// <summary>
        /// Apply transformation on vectors by the specified <see cref="Matrix"/> and places the results in a span.
        /// </summary>
        /// <param name="source">The source span of vectors.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">The destination span.</param>
        public static void Transform(ReadOnlySpan<Vector3> source, in Matrix matrix, Span<Vector3> destination)
        {
            ArgumentGuard.AssertSourceLargerThanDestination(source, destination);

            // TODO: Are there options on some platforms to implement a vectorized version of this?
            for (int i = 0; i < source.Length; i++)
            {
                Vector3 position = source[i];
                destination[i] = new Vector3(
                    (position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41,
                    (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42,
                    (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43);
            }
        }

        /// <summary>
        /// Apply transformation on vectors by the specified <see cref="Quaternion"/> and places the results in a span.
        /// </summary>
        /// <param name="source">The source span of vectors.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> containing the rotation.</param>
        /// <param name="destination">The destination span.</param>
        public static void Transform(ReadOnlySpan<Vector3> source, in Quaternion rotation, Span<Vector3> destination)
        {
            ArgumentGuard.AssertSourceLargerThanDestination(source, destination);

            // TODO: Are there options on some platforms to implement a vectorized version of this?
            for (int i = 0; i < source.Length; i++)
            {
                Vector3 position = source[i];

                float x = 2 * (rotation.Y * position.Z - rotation.Z * position.Y);
                float y = 2 * (rotation.Z * position.X - rotation.X * position.Z);
                float z = 2 * (rotation.X * position.Y - rotation.Y * position.X);

                destination[i] = new Vector3(
                    position.X + x * rotation.W + (rotation.Y * z - rotation.Z * y),
                    position.Y + y * rotation.W + (rotation.Z * x - rotation.X * z),
                    position.Z + z * rotation.W + (rotation.X * y - rotation.Y * x));
            }
        }

        #endregion

        #region TransformNormal

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a transformation of the specified normal by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="normal">Source <see cref="Vector3"/> which represents a normal vector.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed normal.</returns>
        public static Vector3 TransformNormal(in Vector3 normal, in Matrix matrix)
        {
            return new Vector3(
                (normal.X * matrix.M11) + (normal.Y * matrix.M21) + (normal.Z * matrix.M31),
                (normal.X * matrix.M12) + (normal.Y * matrix.M22) + (normal.Z * matrix.M32),
                (normal.X * matrix.M13) + (normal.Y * matrix.M23) + (normal.Z * matrix.M33));
        }

        /// <summary>
        /// Apply transformation on normals by the specified <see cref="Matrix"/> and places the results in a span.
        /// </summary>
        /// <param name="source">The source span of vectors.</param>
        /// <param name="rotation">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">The destination span.</param>
        public static void TransformNormal(ReadOnlySpan<Vector3> source, in Matrix matrix, Span<Vector3> destination)
        {
            ArgumentGuard.AssertSourceLargerThanDestination(source, destination);

            for (int i = 0; i < source.Length; i++)
            {
                Vector3 normal = source[i];
                destination[i] = new Vector3(
                    (normal.X * matrix.M11) + (normal.Y * matrix.M21) + (normal.Z * matrix.M31),
                    (normal.X * matrix.M12) + (normal.Y * matrix.M22) + (normal.Z * matrix.M32),
                    (normal.X * matrix.M13) + (normal.Y * matrix.M23) + (normal.Z * matrix.M33));
            }
        }

        #endregion

        /// <summary>
        /// Deconstruction method for <see cref="Vector3"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Deconstruct(out float x, out float y, out float z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        #region Operators

        /// <summary>
        /// Compares whether two <see cref="Vector3"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Vector3"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Vector3"/> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(in Vector3 a, in Vector3 b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

        /// <summary>
        /// Compares whether two <see cref="Vector3"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Vector3"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Vector3"/> instance on the right of the not equal sign.</param>
        /// <returns><see langword="true"/> if the instances are not equal; <see langword="false"/> otherwise.</returns>	
        public static bool operator !=(in Vector3 a, in Vector3 b) => !(a == b);

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/> on the left of the add sign.</param>
        /// <param name="b">Source <see cref="Vector3"/> on the right of the add sign.</param>
        /// <returns>Sum of the vectors.</returns>
        public static Vector3 operator +(in Vector3 a, in Vector3 b) => new Vector3(
            a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        /// <summary>
        /// Negates values in the specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/> on the right of the sub sign.</param>
        /// <returns>Result of the inversion.</returns>
        public static Vector3 operator -(in Vector3 value) => new Vector3(-value.X, -value.Y, -value.Z);

        /// <summary>
        /// Subtracts a <see cref="Vector3"/> from a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/> on the left of the sub sign.</param>
        /// <param name="right">Source <see cref="Vector3"/> on the right of the sub sign.</param>
        /// <returns>Result of the vector subtraction.</returns>
        public static Vector3 operator -(in Vector3 left, in Vector3 right) => new Vector3(
            left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/> on the left of the mul sign.</param>
        /// <param name="b">Source <see cref="Vector3"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication.</returns>
        public static Vector3 operator *(in Vector3 a, in Vector3 b) => new Vector3(
            a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector3 operator *(in Vector3 value, float scaleFactor)
        {
            return new Vector3(
                value.X * scaleFactor,
                value.Y * scaleFactor,
                value.Z * scaleFactor);
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="scaleFactor">Scalar value on the left of the mul sign.</param>
        /// <param name="value">Source <see cref="Vector3"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector3 operator *(float scaleFactor, in Vector3 value) => value * scaleFactor;

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by the components of another <see cref="Vector3"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/> on the left of the div sign.</param>
        /// <param name="right">Divisor <see cref="Vector3"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector3 operator /(in Vector3 left, in Vector3 right) => new Vector3(
            left.X / right.X, left.Y / right.Y, left.Z / right.Z);

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/> on the left of the div sign.</param>
        /// <param name="divider">Divisor scalar on the right of the div sign.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector3 operator /(in Vector3 value, float divider)
        {
            float factor = 1f / divider;
            return new Vector3(
                value.X * factor,
                value.Y * factor,
                value.Z * factor);
        }

        #endregion
    }
}
