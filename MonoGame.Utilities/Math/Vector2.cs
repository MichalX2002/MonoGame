// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using MonoGame.Utilities;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Framework
{
    /// <summary>
    /// Describes a 2D-vector.
    /// <para>
    /// Ranges from [-1, -1, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
#if XNADESIGNPROVIDED
    [System.ComponentModel.TypeConverter(typeof(Design.Vector2TypeConverter))]
#endif
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Vector2 : IEquatable<Vector2>, IPackedVector<ulong>, IPixel
    {
        /// <summary>
        /// <see cref="Vector2"/> with all values set to <see cref="byte.MaxValue"/>.
        /// </summary>
        internal static readonly Vector2 MaxBytes = new Vector2(byte.MaxValue);

        #region Public Constants

        /// <summary>
        /// Returns a <see cref="Vector2"/> with all components set to 0.
        /// </summary>
        public static readonly Vector2 Zero = new Vector2(0f);

        /// <summary>
        /// Returns a <see cref="Vector2"/> with all components set 0.5.
        /// </summary>
        public static readonly Vector2 Half = new Vector2(0.5f);

        /// <summary>
        /// Returns a <see cref="Vector2"/> with all components set to 1.
        /// </summary>
        public static readonly Vector2 One = new Vector2(1f);

        /// <summary>
        /// Returns a <see cref="Vector2"/> with components 1, 0.
        /// </summary>
        public static readonly Vector2 UnitX = new Vector2(1f, 0f);

        /// <summary>
        /// Returns a <see cref="Vector2"/> with components 0, 1.
        /// </summary>
        public static readonly Vector2 UnitY = new Vector2(0f, 1f);

        #endregion

        #region Public Fields

        /// <summary>
        /// The x coordinate of this <see cref="Vector2"/>.
        /// </summary>
        [DataMember]
        public float X;

        /// <summary>
        /// The y coordinate of this <see cref="Vector2"/>.
        /// </summary>
        [DataMember]
        public float Y;

        #endregion

        private string DebuggerDisplay => string.Concat(
            X.ToString(), "  ",
            Y.ToString());

        #region Constructors

        /// <summary>
        /// Constructs a 2D vector with X and Y from two values.
        /// </summary>
        /// <param name="x">The x coordinate in 2D-space.</param>
        /// <param name="y">The y coordinate in 2D-space.</param>
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs a 2D vector with X and Y set to the same value.
        /// </summary>
        /// <param name="value">The x and y coordinates in 2D-space.</param>
        public Vector2(float value)
        {
            X = value;
            Y = value;
        }

        #endregion

        #region IPackedVector

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public ulong PackedValue
        {
            get => Unsafe.As<Vector2, ulong>(ref this);
            set => Unsafe.As<Vector2, ulong>(ref this) = value;
        }

        /// <inheritdoc/>
        public void FromVector4(Vector4 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <inheritdoc/>
        public readonly Vector4 ToVector4() => new Vector4(X, Y, 0, 1);

        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        public readonly Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc/>
        public readonly void ToColor(ref Color destination) => destination.FromVector4(ToVector4());

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a <see cref="Vector3"/> representation for this
        /// object with <see cref="Vector3.Z"/> axis set to 0.
        /// </summary>
        /// <returns>A <see cref="Vector3"/> representation for this object.</returns>
        public readonly Vector3 ToVector3() => new Vector3(X, Y, 0);

        /// <summary>
        /// Performs vector addition on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector to add.</param>
        /// <param name="b">The second vector to add.</param>
        /// <returns>The result of the vector addition.</returns>
        public static Vector2 Add(in Vector2 a, in Vector2 b) => a + b;

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains the cartesian coordinates of a vector specified in barycentric coordinates and relative to 2D-triangle.
        /// </summary>
        /// <param name="a">The first vector of 2D-triangle.</param>
        /// <param name="b">The second vector of 2D-triangle.</param>
        /// <param name="c">The third vector of 2D-triangle.</param>
        /// <param name="amount1">Barycentric scalar <c>b2</c> which represents a weighting factor towards second vector of 2D-triangle.</param>
        /// <param name="amount2">Barycentric scalar <c>b3</c> which represents a weighting factor towards third vector of 2D-triangle.</param>
        /// <returns>The cartesian translation of barycentric coordinates.</returns>
        public static Vector2 Barycentric(
            in Vector2 a, in Vector2 b, in Vector2 c, float amount1, float amount2) => new Vector2(
                MathHelper.Barycentric(a.X, b.X, c.X, amount1, amount2),
                MathHelper.Barycentric(a.Y, b.Y, c.Y, amount1, amount2));

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains CatmullRom interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">The first vector in interpolation.</param>
        /// <param name="b">The second vector in interpolation.</param>
        /// <param name="c">The third vector in interpolation.</param>
        /// <param name="d">The fourth vector in interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of CatmullRom interpolation.</returns>
        public static Vector2 CatmullRom(
            in Vector2 a, in Vector2 b, in Vector2 c, in Vector2 d, float amount) => new Vector2(
                MathHelper.CatmullRom(a.X, b.X, c.X, d.X, amount),
                MathHelper.CatmullRom(a.Y, b.Y, c.Y, d.Y, amount));

        /// <summary>
        /// Round the members of this <see cref="Vector2"/> towards positive infinity.
        /// </summary>
        public void Ceiling()
        {
            X = (float)Math.Ceiling(X);
            Y = (float)Math.Ceiling(Y);
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains members from another vector rounded towards positive infinity.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <returns>The rounded <see cref="Vector2"/>.</returns>
        public static Vector2 Ceiling(in Vector2 value) => new Vector2(
            (float)Math.Ceiling(value.X),
            (float)Math.Ceiling(value.Y));

        /// <summary>
        /// Clamps the specified value within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public static Vector2 Clamp(in Vector2 value, in Vector2 min, in Vector2 max) => new Vector2(
            MathHelper.Clamp(value.X, min.X, max.X),
            MathHelper.Clamp(value.Y, min.Y, max.Y));

        /// <summary>
        /// Returns the distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The distance between two vectors.</returns>
        public static float Distance(in Vector2 a, in Vector2 b)
        {
            float v1 = a.X - b.X, v2 = a.Y - b.Y;
            return (float)Math.Sqrt((v1 * v1) + (v2 * v2));
        }

        /// <summary>
        /// Returns the squared distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The squared distance between two vectors.</returns>
        public static float DistanceSquared(in Vector2 a, in Vector2 b)
        {
            float v1 = a.X - b.X, v2 = a.Y - b.Y;
            return (v1 * v1) + (v2 * v2);
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector2"/> by the components of another <see cref="Vector2"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector2"/>.</param>
        /// <param name="right">Divisor <see cref="Vector2"/>.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector2 Divide(in Vector2 left, in Vector2 right) => left / right;

        /// <summary>
        /// Divides the components of a <see cref="Vector2"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <param name="divider">Divisor scalar.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector2 Divide(in Vector2 value, float divider) => value / divider;

        /// <summary>
        /// Returns a dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product of two vectors.</returns>
        public static float Dot(in Vector2 a, in Vector2 b) => (a.X * b.X) + (a.Y * b.Y);

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj) => obj is Vector2 other && Equals(other);

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Vector2"/>.
        /// </summary>
        /// <param name="other">The <see cref="Vector2"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public bool Equals(Vector2 other) => this == other;

        /// <summary>
        /// Round the members of this <see cref="Vector2"/> towards negative infinity.
        /// </summary>
        public void Floor()
        {
            X = (float)Math.Floor(X);
            Y = (float)Math.Floor(Y);
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains members from another vector rounded towards negative infinity.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <returns>The rounded <see cref="Vector2"/>.</returns>
        public static Vector2 Floor(in Vector2 value) => new Vector2(
                (float)Math.Floor(value.X),
                (float)Math.Floor(value.Y));

        /// <summary>
        /// Gets the hash code of this <see cref="Vector2"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="Vector2"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int code = 7 + X.GetHashCode();
                return code * 31 + Y.GetHashCode();
            }
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains hermite spline interpolation.
        /// </summary>
        /// <param name="position1">The first position vector.</param>
        /// <param name="tangent1">The first tangent vector.</param>
        /// <param name="position2">The second position vector.</param>
        /// <param name="tangent2">The second tangent vector.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The hermite spline interpolation vector.</returns>
        public static Vector2 Hermite(
            in Vector2 position1, in Vector2 tangent1, in Vector2 position2, in Vector2 tangent2, float amount) =>
            new Vector2(
                MathHelper.Hermite(position1.X, tangent1.X, position2.X, tangent2.X, amount),
                MathHelper.Hermite(position1.Y, tangent1.Y, position2.Y, tangent2.Y, amount));

        /// <summary>
        /// Returns the length of this <see cref="Vector2"/>.
        /// </summary>
        public float Length() => (float)Math.Sqrt(LengthSquared());

        /// <summary>
        /// Returns the squared length of this <see cref="Vector2"/>.
        /// </summary>
        public float LengthSquared() => (X * X) + (Y * Y);

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains linear interpolation of the specified vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector2 Lerp(in Vector2 value1, in Vector2 value2, float amount) => new Vector2(
                MathHelper.Lerp(value1.X, value2.X, amount),
                MathHelper.Lerp(value1.Y, value2.Y, amount));

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains linear interpolation of the specified vectors.
        /// Uses <see cref="MathHelper.LerpPrecise"/> on MathHelper for the interpolation.
        /// Less efficient but more precise compared to <see cref="Lerp(Vector2, Vector2, float)"/>.
        /// See remarks section of <see cref="MathHelper.LerpPrecise"/> on MathHelper for more info.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector2 LerpPrecise(in Vector2 value1, in Vector2 value2, float amount) => new Vector2(
                MathHelper.LerpPrecise(value1.X, value2.X, amount),
                MathHelper.LerpPrecise(value1.Y, value2.Y, amount));

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a maximal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector2"/> with maximal values from the two vectors.</returns>
        public static Vector2 Max(in Vector2 a, in Vector2 b)
        {
            return new Vector2(
                a.X > b.X ? a.X : b.X,
                a.Y > b.Y ? a.Y : b.Y);
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a minimal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector2"/> with minimal values from the two vectors.</returns>
        public static Vector2 Min(in Vector2 a, in Vector2 b)
        {
            return new Vector2(
                a.X < b.X ? a.X : b.X,
                a.Y < b.Y ? a.Y : b.Y);
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a multiplication of two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector2"/>.</param>
        /// <param name="b">Source <see cref="Vector2"/>.</param>
        /// <returns>The result of the vector multiplication.</returns>
        public static Vector2 Multiply(in Vector2 a, in Vector2 b) => a * b;

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a multiplication of <see cref="Vector2"/> and a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>The result of the vector multiplication with a scalar.</returns>
        public static Vector2 Multiply(in Vector2 value, float scaleFactor) => value * scaleFactor;

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains the specified vector inversion.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <returns>The result of the vector inversion.</returns>
        public static Vector2 Negate(in Vector2 value) => -value;

        /// <summary>
        /// Turns this <see cref="Vector2"/> to a unit vector with the same direction.
        /// </summary>
        public void Normalize()
        {
            float val = 1f / (float)Math.Sqrt((X * X) + (Y * Y));
            X *= val;
            Y *= val;
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a normalized values from another vector.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <returns>Unit vector.</returns>
        public static Vector2 Normalize(in Vector2 value)
        {
            float factor = 1f / (float)Math.Sqrt((value.X * value.X) + (value.Y * value.Y));
            return new Vector2(value.X * factor, value.Y * factor);
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains reflect vector of the given vector and normal.
        /// </summary>
        /// <param name="vector">Source <see cref="Vector2"/>.</param>
        /// <param name="normal">Reflection normal.</param>
        /// <returns>Reflected vector.</returns>
        public static Vector2 Reflect(in Vector2 vector, in Vector2 normal)
        {
            float val = 2.0f * ((vector.X * normal.X) + (vector.Y * normal.Y));
            return new Vector2(vector.X - (normal.X * val), vector.Y - (normal.Y * val));
        }

        /// <summary>
        /// Round the members of this <see cref="Vector2"/> to the nearest integer value.
        /// </summary>
        public void Round()
        {
            X = (float)Math.Round(X);
            Y = (float)Math.Round(Y);
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains members from another vector rounded to the nearest integer value.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <returns>The rounded <see cref="Vector2"/>.</returns>
        public static Vector2 Round(in Vector2 value)
        {
            return new Vector2((float)Math.Round(value.X), (float)Math.Round(value.Y));
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains cubic interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector2"/>.</param>
        /// <param name="b">Source <see cref="Vector2"/>.</param>
        /// <param name="amount">Weighting value.</param>
        /// <returns>Cubic interpolation of the specified vectors.</returns>
        public static Vector2 SmoothStep(in Vector2 a, in Vector2 b, float amount)
        {
            return new Vector2(
                MathHelper.SmoothStep(a.X, b.X, amount),
                MathHelper.SmoothStep(a.Y, b.Y, amount));
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains subtraction of on <see cref="Vector2"/> from a another.
        /// </summary>
        /// <param name="left">Source <see cref="Vector2"/>.</param>
        /// <param name="right">Source <see cref="Vector2"/>.</param>
        /// <returns>The result of the vector subtraction.</returns>
        public static Vector2 Subtract(in Vector2 left, in Vector2 right) => left - right;

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Vector2"/> in the format:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>]}
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Vector2"/>.</returns>
        public override string ToString()
        {
            return "{X:" + X + " Y:" + Y + "}";
        }

        /// <summary>
        /// Gets a <see cref="Point"/> representation for this object.
        /// </summary>
        /// <returns>A <see cref="Point"/> representation for this object.</returns>
        public Point ToPoint()
        {
            return new Point((int)X, (int)Y);
        }

        /// <summary>
        /// Deconstruction method for <see cref="Vector2"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Deconstruct(out float x, out float y)
        {
            x = X;
            y = Y;
        }

        #endregion

        #region Transform & TransformNormal

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a transformation of 2D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">Source <see cref="Vector2"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector2"/>.</returns>
        public static Vector2 Transform(in Vector2 position, in Matrix matrix)
        {
            return new Vector2(
                (position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41,
                (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42);
        }

        /// <summary>
        /// Transforms the <see cref="Vector2"/> by the specified <see cref="Quaternion"/> that represents rotation.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <returns>Transformed <see cref="Vector2"/>.</returns>
        public static Vector2 Transform(in Vector2 value, in Quaternion rotation)
        {
            var rot1 = new Vector3(rotation.X + rotation.X, rotation.Y + rotation.Y, rotation.Z + rotation.Z);
            var rot2 = new Vector3(rotation.X, rotation.X, rotation.W);
            var rot3 = new Vector3(1, rotation.Y, rotation.Z);
            var rot4 = rot1 * rot2;
            var rot5 = rot1 * rot3;

            return new Vector2(
                (float)(value.X * (1.0 - rot5.Y - rot5.Z) + value.Y * (rot4.Y - (double)rot4.Z)),
                (float)(value.X * (rot4.Y + (double)rot4.Z) + value.Y * (1.0 - rot4.X - rot5.Z)));
        }

        /// <summary>
        /// Applies transformation on <see cref="Vector2"/> normals by the 
        /// specified <see cref="Matrix"/> and places the results in another span.
        /// </summary>
        /// <param name="source">Source span.</param>
        /// <param name="rotation">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">Destination span.</param>
        public static void Transform(
            ReadOnlySpan<Vector2> source, in Matrix matrix, Span<Vector2> destination)
        {
            CommonArgumentGuard.AssertSourceLargerThanDestination(source, destination);

            for (int i = 0; i < source.Length; i++)
                destination[i] = Transform(source[i], matrix);
        }

        /// <summary>
        /// Applies transformation on <see cref="Vector2"/> normals by the 
        /// specified <see cref="Quaternion"/> and places the results in another span.
        /// </summary>
        /// <param name="source">Source span.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <param name="destination">Destination span.</param>
        public static void Transform(
            ReadOnlySpan<Vector2> source, in Quaternion rotation, Span<Vector2> destination)
        {
            CommonArgumentGuard.AssertSourceLargerThanDestination(source, destination);

            for (int i = 0; i < source.Length; i++)
                destination[i] = Transform(source[i], rotation);
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a transformation of the specified normal by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="normal">Source <see cref="Vector2"/> which represents a normal vector.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed normal.</returns>
        public static Vector2 TransformNormal(in Vector2 normal, in Matrix matrix)
        {
            return new Vector2(
                (normal.X * matrix.M11) + (normal.Y * matrix.M21),
                (normal.X * matrix.M12) + (normal.Y * matrix.M22));
        }

        /// <summary>
        /// Applies transformation on <see cref="Vector2"/> normals by the 
        /// specified <see cref="Matrix"/> and places the results in another span.
        /// </summary>
        /// <param name="source">Source span.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">Destination span.</param>
        public static void TransformNormal(
            ReadOnlySpan<Vector2> source, in Matrix matrix, Span<Vector2> destination)
        {
            CommonArgumentGuard.AssertSourceLargerThanDestination(source, destination);

            for (int i = 0; i < source.Length; i++)
                destination[i] = TransformNormal(source[i], matrix);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Negates values in the specified <see cref="Vector2"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/> on the right of the sub sign.</param>
        /// <returns>Result of the inversion.</returns>
        public static Vector2 operator -(in Vector2 value)
        {
            return new Vector2(-value.X, -value.Y);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector2"/> on the left of the add sign.</param>
        /// <param name="b">Source <see cref="Vector2"/> on the right of the add sign.</param>
        /// <returns>Sum of the vectors.</returns>
        public static Vector2 operator +(in Vector2 a, in Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// Subtracts a <see cref="Vector2"/> from a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector2"/> on the left of the sub sign.</param>
        /// <param name="right">Source <see cref="Vector2"/> on the right of the sub sign.</param>
        /// <returns>Result of the vector subtraction.</returns>
        public static Vector2 operator -(in Vector2 left, in Vector2 right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="a">Source <see cref="Vector2"/> on the left of the mul sign.</param>
        /// <param name="b">Source <see cref="Vector2"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication.</returns>
        public static Vector2 operator *(in Vector2 a, in Vector2 b)
        {
            return new Vector2(a.X * b.X, a.Y * b.Y);
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector2 operator *(in Vector2 value, float scaleFactor)
        {
            return new Vector2(value.X * scaleFactor, value.Y * scaleFactor);
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="scaleFactor">Scalar value on the left of the mul sign.</param>
        /// <param name="value">Source <see cref="Vector2"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector2 operator *(float scaleFactor, in Vector2 value)
        {
            return new Vector2(value.X * scaleFactor, value.Y * scaleFactor);
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector2"/> by the components of another <see cref="Vector2"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector2"/> on the left of the div sign.</param>
        /// <param name="right">Divisor <see cref="Vector2"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector2 operator /(in Vector2 left, in Vector2 right)
        {
            return new Vector2(left.X / right.X, left.Y / right.Y);
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector2"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/> on the left of the div sign.</param>
        /// <param name="divider">Divisor scalar on the right of the div sign.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector2 operator /(in Vector2 value, float divider)
        {
            return new Vector2(value.X / divider, value.Y / divider);
        }

        /// <summary>
        /// Compares whether two <see cref="Vector2"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Vector2"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Vector2"/> instance on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(in Vector2 a, in Vector2 b) => a.X == b.X && a.Y == b.Y;

        /// <summary>
        /// Compares whether two <see cref="Vector2"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Vector2"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Vector2"/> instance on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>	
        public static bool operator !=(in Vector2 a, in Vector2 b) => !(a == b);

        #endregion
    }
}
