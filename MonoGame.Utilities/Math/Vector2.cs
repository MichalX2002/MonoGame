// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using MonoGame.Framework.PackedVector;
using FastVector2 = System.Numerics.Vector2;
using FastVector4 = System.Numerics.Vector4;

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

        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(float) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(float) * 8));

        private string DebuggerDisplay => string.Concat(
            X.ToString(), "  ",
            Y.ToString());

        [IgnoreDataMember]
        public FastVector2 Base;

        /// <summary>
        /// The x coordinate of this <see cref="Vector2"/>.
        /// </summary>
        [DataMember]
        public float X { readonly get => Base.X; set => Base.X = value; }

        /// <summary>
        /// The y coordinate of this <see cref="Vector2"/>.
        /// </summary>
        [DataMember]
        public float Y { readonly get => Base.Y; set => Base.Y = value; }

        #region Constructors

        /// <summary>
        /// Constructs a 2D vector with X and Y from two values.
        /// </summary>
        /// <param name="x">The x coordinate in 2D-space.</param>
        /// <param name="y">The y coordinate in 2D-space.</param>
        public Vector2(float x, float y)
        {
            Base = new FastVector2(x, y);
        }

        /// <summary>
        /// Constructs a 2D vector with X and Y set to the same value.
        /// </summary>
        /// <param name="value">The x and y coordinates in 2D-space.</param>
        public Vector2(float value)
        {
            Base = new FastVector2(value);
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ulong PackedValue
        {
            readonly get => UnsafeUtils.As<Vector2, ulong>(this);
            set => Unsafe.As<Vector2, ulong>(ref this) = value;
        }

        public void FromVector4(in Vector4 vector)
        {
            Base.X = vector.Base.X;
            Base.Y = vector.Base.Y;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base = new FastVector4(Base, 0, 1);
        }

        void IPackedVector.FromScaledVector4(in Vector4 scaledVector)
        {
            FromVector4(scaledVector);
        }

        readonly void IPackedVector.ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
        }

        #endregion

        #region IPixel

        public void FromColor(Color source) => FromVector4(source.ToScaledVector4());

        void IPixel.FromGray8(Gray8 source) => FromVector4(source.ToScaledVector4());

        void IPixel.FromGray16(Gray16 source) => FromVector4(source.ToScaledVector4());

        void IPixel.FromGrayAlpha16(GrayAlpha16 source) => FromVector4(source.ToScaledVector4());

        void IPixel.FromRgb24(Rgb24 source) => FromVector4(source.ToScaledVector4());

        void IPixel.FromRgb48(Rgb48 source) => FromVector4(source.ToScaledVector4());

        public void FromRgba64(Rgba64 source) => FromVector4(source.ToScaledVector4());

        public readonly void ToColor(ref Color destination)
        {
            ToVector4(out var vector);
            destination.FromScaledVector4(vector);
        }

        #endregion

        #region ToVector3

        /// <summary>
        /// Gets the <see cref="Vector3"/> representation of this <see cref="Vector2"/>.
        /// </summary>
        public readonly Vector3 ToVector3() => new Vector3(X, Y, 0);

        #endregion

        #region Add (operator +)

        /// <summary>
        /// Performs vector addition on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector to add.</param>
        /// <param name="b">The second vector to add.</param>
        /// <returns>The result of the vector addition.</returns>
        public static Vector2 Add(in Vector2 a, in Vector2 b)
        {
            return FastVector2.Add(a, b);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector2"/> on the left of the add sign.</param>
        /// <param name="b">Source <see cref="Vector2"/> on the right of the add sign.</param>
        /// <returns>Sum of the vectors.</returns>
        public static Vector2 operator +(in Vector2 a, in Vector2 b)
        {
            return a.Base + b.Base;
        }

        #endregion

        #region Barycentric

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains the cartesian coordinates of 
        /// a vector specified in barycentric coordinates and relative to 2D-triangle.
        /// </summary>
        /// <param name="a">The first vector of 2D-triangle.</param>
        /// <param name="b">The second vector of 2D-triangle.</param>
        /// <param name="c">The third vector of 2D-triangle.</param>
        /// <param name="amount1">Barycentric scalar <c>b2</c> which represents a weighting factor towards second vector of 2D-triangle.</param>
        /// <param name="amount2">Barycentric scalar <c>b3</c> which represents a weighting factor towards third vector of 2D-triangle.</param>
        /// <returns>The cartesian translation of barycentric coordinates.</returns>
        public static Vector2 Barycentric(
            in Vector2 a, in Vector2 b, in Vector2 c, float amount1, float amount2)
        {
            return new Vector2(
                MathHelper.Barycentric(a.X, b.X, c.X, amount1, amount2),
                MathHelper.Barycentric(a.Y, b.Y, c.Y, amount1, amount2));
        }

        #endregion

        #region CatmullRom

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
            in Vector2 a, in Vector2 b, in Vector2 c, in Vector2 d, float amount)
        {
            return new Vector2(
                MathHelper.CatmullRom(a.X, b.X, c.X, d.X, amount),
                MathHelper.CatmullRom(a.Y, b.Y, c.Y, d.Y, amount));
        }

        #endregion

        #region Ceiling

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains members
        /// from another vector rounded towards positive infinity.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <returns>The rounded <see cref="Vector2"/>.</returns>
        public static Vector2 Ceiling(in Vector2 value)
        {
            return new Vector2(
                MathF.Ceiling(value.X),
                MathF.Ceiling(value.Y));
        }

        /// <summary>
        /// Round the members of this <see cref="Vector2"/> towards positive infinity.
        /// </summary>
        public void Ceiling()
        {
            this = Ceiling(this);
        }

        #endregion

        #region Clamp

        /// <summary>
        /// Clamps the specified value within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public static Vector2 Clamp(in Vector2 value, in Vector2 min, in Vector2 max)
        {
            return FastVector2.Clamp(value, min, max);
        }

        /// <summary>
        /// Clamps this vector within a range.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        public void Clamp(in Vector2 min, in Vector2 max)
        {
            this = Clamp(this, min, max);
        }

        /// <summary>
        /// Clamps the specified value within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public static Vector2 Clamp(in Vector2 value, float min, float max)
        {
            return FastVector2.Clamp(value, new FastVector2(min), new FastVector2(max));
        }

        /// <summary>
        /// Clamps this vector within a range.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        public void Clamp(float min, float max)
        {
            this = Clamp(this, min, max);
        }

        #endregion

        #region Distance

        /// <summary>
        /// Returns the distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The distance between two vectors.</returns>
        public static float Distance(in Vector2 a, in Vector2 b)
        {
            return FastVector2.Distance(a, b);
        }

        #endregion

        #region DistanceSquared

        /// <summary>
        /// Returns the squared distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The squared distance between two vectors.</returns>
        public static float DistanceSquared(in Vector2 a, in Vector2 b)
        {
            return FastVector2.DistanceSquared(a, b);
        }

        #endregion

        #region Divide (operator /)

        /// <summary>
        /// Divides the components of a <see cref="Vector2"/> by the components of another <see cref="Vector2"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector2"/>.</param>
        /// <param name="right">Divisor <see cref="Vector2"/>.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector2 Divide(in Vector2 left, in Vector2 right)
        {
            return FastVector2.Divide(left, right);
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector2"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <param name="divisor">Divisor scalar.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector2 Divide(in Vector2 value, float divisor)
        {
            return FastVector2.Divide(value, divisor);
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector2"/> by the components of another <see cref="Vector2"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector2"/> on the left of the div sign.</param>
        /// <param name="right">Divisor <see cref="Vector2"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector2 operator /(in Vector2 left, in Vector2 right)
        {
            return left.Base / right.Base;
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector2"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/> on the left of the div sign.</param>
        /// <param name="divisor">Divisor scalar on the right of the div sign.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector2 operator /(in Vector2 value, float divisor)
        {
            return value.Base / divisor;
        }

        #endregion

        #region Dot

        /// <summary>
        /// Returns a dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product of two vectors.</returns>
        public static float Dot(in Vector2 a, in Vector2 b)
        {
            return FastVector2.Dot(a, b);
        }

        #endregion

        #region Equals (operator ==, !=)

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is Vector2 other && this == other;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Vector2"/>.
        /// </summary>
        /// <param name="other">The <see cref="Vector2"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public bool Equals(Vector2 other)
        {
            return Base.Equals(other.Base);
        }

        /// <summary>
        /// Compares whether two <see cref="Vector2"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Vector2"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Vector2"/> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(in Vector2 a, in Vector2 b)
        {
            return a.Base == b.Base;
        }

        /// <summary>
        /// Compares whether two <see cref="Vector2"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Vector2"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Vector2"/> instance on the right of the not equal sign.</param>
        /// <returns><see langword="true"/> if the instances are not equal; <see langword="false"/> otherwise.</returns>	
        public static bool operator !=(in Vector2 a, in Vector2 b)
        {
            return a.Base != b.Base;
        }

        #endregion

        #region Floor

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains members 
        /// from another vector rounded towards negative infinity.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <returns>The rounded <see cref="Vector2"/>.</returns>
        public static Vector2 Floor(in Vector2 value)
        {
            return new Vector2(
                MathF.Floor(value.X),
                MathF.Floor(value.Y));
        }

        /// <summary>
        /// Round the members of this <see cref="Vector2"/> towards negative infinity.
        /// </summary>
        public void Floor()
        {
            this = Floor(this);
        }

        #endregion

        #region GetHashCode

        /// <summary>
        /// Gets the hash code of this <see cref="Vector2"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="Vector2"/>.</returns>
        public override int GetHashCode() => HashCode.Combine(X, Y);

        #endregion

        #region Hermite

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
            in Vector2 position1, in Vector2 tangent1, 
            in Vector2 position2, in Vector2 tangent2, 
            float amount)
        {
            return new Vector2(
                MathHelper.Hermite(position1.X, tangent1.X, position2.X, tangent2.X, amount),
                MathHelper.Hermite(position1.Y, tangent1.Y, position2.Y, tangent2.Y, amount));
        }

        #endregion

        #region Length

        /// <summary>
        /// Returns the length of this <see cref="Vector2"/>.
        /// </summary>
        public readonly float Length()
        {
            return Base.Length();
        }

        #endregion

        #region LengthSquared

        /// <summary>
        /// Returns the squared length of this <see cref="Vector2"/>.
        /// </summary>
        public readonly float LengthSquared()
        {
            return Base.LengthSquared();
        }

        #endregion

        #region Lerp

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains linear interpolation of the specified vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector2 Lerp(in Vector2 value1, in Vector2 value2, float amount)
        {
            return FastVector2.Lerp(value1, value2, amount);
        }

        #endregion

        #region PreciseLerp

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains linear interpolation of the specified vectors.
        /// Less efficient but more precise compared to <see cref="Lerp(Vector2, Vector2, float)"/>.
        /// See remarks section of <see cref="MathHelper.LerpPrecise"/> on MathHelper for more info.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector2 LerpPrecise(in Vector2 value1, in Vector2 value2, float amount)
        {
            return new Vector2(
                MathHelper.LerpPrecise(value1.X, value2.X, amount),
                MathHelper.LerpPrecise(value1.Y, value2.Y, amount));
        }

        #endregion

        #region Max

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a maximal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector2"/> with maximal values from the two vectors.</returns>
        public static Vector2 Max(in Vector2 a, in Vector2 b)
        {
            return FastVector2.Max(a, b);
        }

        #endregion

        #region Min

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a minimal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector2"/> with minimal values from the two vectors.</returns>
        public static Vector2 Min(in Vector2 a, in Vector2 b)
        {
            return FastVector2.Min(a, b);
        }

        #endregion

        #region Multiply (operator *)

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a multiplication of two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector2"/>.</param>
        /// <param name="b">Source <see cref="Vector2"/>.</param>
        /// <returns>The result of the vector multiplication.</returns>
        public static Vector2 Multiply(in Vector2 a, in Vector2 b)
        {
            return FastVector2.Multiply(a, b);
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a multiplication of <see cref="Vector2"/> and a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>The result of the vector multiplication with a scalar.</returns>
        public static Vector2 Multiply(in Vector2 value, float scaleFactor)
        {
            return FastVector2.Multiply(value, scaleFactor);
        }

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="a">Source <see cref="Vector2"/> on the left of the mul sign.</param>
        /// <param name="b">Source <see cref="Vector2"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication.</returns>
        public static Vector2 operator *(in Vector2 a, in Vector2 b)
        {
            return a.Base * b.Base;
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector2 operator *(in Vector2 value, float scaleFactor)
        {
            return value.Base * scaleFactor;
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="scaleFactor">Scalar value on the left of the mul sign.</param>
        /// <param name="value">Source <see cref="Vector2"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector2 operator *(float scaleFactor, in Vector2 value)
        {
            return scaleFactor * value.Base;
        }

        #endregion

        #region Negate (operator -)

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains the specified vector inversion.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <returns>The result of the vector inversion.</returns>
        public static Vector2 Negate(in Vector2 value)
        {
            return FastVector2.Negate(value);
        }

        /// <summary>
        /// Negates values in the specified <see cref="Vector2"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/> on the right of the sub sign.</param>
        /// <returns>Result of the inversion.</returns>
        public static Vector2 operator -(in Vector2 value)
        {
            return -value.Base;
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a normalized values from another vector.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <returns>Unit vector.</returns>
        public static Vector2 Normalize(in Vector2 value)
        {
            return FastVector2.Normalize(value);
        }

        /// <summary>
        /// Turns this <see cref="Vector2"/> to a unit vector with the same direction.
        /// </summary>
        public void Normalize()
        {
            this = Normalize(this);
        }

        #endregion

        #region Reflect

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains reflect vector of the given vector and normal.
        /// </summary>
        /// <param name="vector">Source <see cref="Vector2"/>.</param>
        /// <param name="normal">Reflection normal.</param>
        /// <returns>Reflected vector.</returns>
        public static Vector2 Reflect(in Vector2 vector, in Vector2 normal)
        {
            return FastVector2.Reflect(vector, normal);
        }

        #endregion

        #region Round

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains members from 
        /// another vector rounded to the nearest integer value.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <returns>The rounded <see cref="Vector2"/>.</returns>
        public static Vector2 Round(in Vector2 value)
        {
            return new Vector2(
                MathF.Round(value.X),
                MathF.Round(value.Y));
        }

        /// <summary>
        /// Round the members of this <see cref="Vector2"/> to the nearest integer value.
        /// </summary>
        public void Round()
        {
            this = Round(this);
        }

        #endregion

        #region SmoothStep

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

        #endregion

        #region Subtract (operator -)

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains subtraction of on <see cref="Vector2"/> from a another.
        /// </summary>
        /// <param name="left">Source <see cref="Vector2"/>.</param>
        /// <param name="right">Source <see cref="Vector2"/>.</param>
        /// <returns>The result of the vector subtraction.</returns>
        public static Vector2 Subtract(in Vector2 left, in Vector2 right)
        {
            return FastVector2.Subtract(left, right);
        }

        /// <summary>
        /// Subtracts a <see cref="Vector2"/> from a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector2"/> on the left of the sub sign.</param>
        /// <param name="right">Source <see cref="Vector2"/> on the right of the sub sign.</param>
        /// <returns>Result of the vector subtraction.</returns>
        public static Vector2 operator -(in Vector2 left, in Vector2 right)
        {
            return left.Base - right.Base;
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Vector2"/>.
        /// </summary>
        public override string ToString() => Base.ToString();

        /// <summary>
        /// Gets a <see cref="Point"/> representation for this object.
        /// </summary>
        public readonly Point ToPoint() => new Point((int)X, (int)Y);

        /// <summary>
        /// Deconstruction method for <see cref="Vector2"/>.
        /// </summary>
        public readonly void Deconstruct(out float x, out float y)
        {
            x = X;
            y = Y;
        }

        #region Transform

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a
        /// transformation of 2D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">Source <see cref="Vector2"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector2"/>.</returns>
        public static Vector2 Transform(in Vector2 position, in Matrix matrix)
        {
            return FastVector2.Transform(position, matrix);
        }

        /// <summary>
        /// Transforms the <see cref="Vector2"/> by the specified <see cref="Quaternion"/> that represents rotation.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <returns>Transformed <see cref="Vector2"/>.</returns>
        public static Vector2 Transform(in Vector2 value, in Quaternion rotation)
        {
            return FastVector2.Transform(value, rotation);
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
            ArgumentGuard.AssertSourceLargerThanDestination(source, destination);

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
            ArgumentGuard.AssertSourceLargerThanDestination(source, destination);

            for (int i = 0; i < source.Length; i++)
                destination[i] = Transform(source[i], rotation);
        }

        #endregion

        #region TransformNormal

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a 
        /// transformation of the specified normal by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="normal">Source <see cref="Vector2"/> which represents a normal vector.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed normal.</returns>
        public static Vector2 TransformNormal(in Vector2 normal, in Matrix matrix)
        {
            return FastVector2.TransformNormal(normal, matrix);
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
            ArgumentGuard.AssertSourceLargerThanDestination(source, destination);

            for (int i = 0; i < source.Length; i++)
                destination[i] = TransformNormal(source[i], matrix);
        }

        #endregion

        public static implicit operator FastVector2(in Vector2 value)
        {
            return value.Base;
        }

        public static implicit operator Vector2(in FastVector2 value)
        {
            return new Vector2 { Base = value };
        }
    }
}
