// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using MonoGame.Framework.PackedVector;
using FastVector3 = System.Numerics.Vector3;
using FastVector4 = System.Numerics.Vector4;

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
    public struct Vector3 : IPixel<Vector3>, IEquatable<Vector3>
    {
        #region Public Constants

        /// <summary>
        /// <see cref="Vector3"/> with all components set to <see cref="float.MaxValue"/>.
        /// </summary>
        public static Vector3 MaxValue { get; } = new Vector3(float.MaxValue);

        /// <summary>
        /// <see cref="Vector3"/> with all components set to <see cref="float.MinValue"/>.
        /// </summary>
        public static Vector3 MinValue { get; } = new Vector3(float.MinValue);

        /// <summary>
        /// <see cref="Vector3"/> with all components set to 0.
        /// </summary>
        public static Vector3 Zero { get; } = new Vector3(0f);

        /// <summary>
        /// <see cref="Vector3"/> with all components set to 0.5.
        /// </summary>
        public static Vector3 Half { get; } = new Vector3(0.5f);

        /// <summary>
        /// <see cref="Vector3"/> with all components set to 1.
        /// </summary>
        public static Vector3 One { get; } = new Vector3(1f);

        /// <summary>
        /// <see cref="Vector3"/> with components 1, 0, 0.
        /// </summary>
        public static Vector3 UnitX { get; } = new Vector3(1f, 0f, 0f);

        /// <summary>
        /// <see cref="Vector3"/> with components 0, 1, 0.
        /// </summary>
        public static Vector3 UnitY { get; } = new Vector3(0f, 1f, 0f);

        /// <summary>
        /// <see cref="Vector3"/> with components 0, 0, 1.
        /// </summary>
        public static Vector3 UnitZ { get; } = new Vector3(0f, 0f, 1f);

        /// <summary>
        /// <see cref="Vector3"/> with components 0, 1, 0.
        /// </summary>
        public static Vector3 Up { get; } = new Vector3(0f, 1f, 0f);

        /// <summary>
        /// <see cref="Vector3"/> with components 0, -1, 0.
        /// </summary>
        public static Vector3 Down { get; } = new Vector3(0f, -1f, 0f);

        /// <summary>
        /// <see cref="Vector3"/> with components 1, 0, 0.
        /// </summary>
        public static Vector3 Right { get; } = new Vector3(1f, 0f, 0f);

        /// <summary>
        /// <see cref="Vector3"/> with components -1, 0, 0.
        /// </summary>
        public static Vector3 Left { get; } = new Vector3(-1f, 0f, 0f);

        /// <summary>
        /// <see cref="Vector3"/> with components 0, 0, -1.
        /// </summary>
        public static Vector3 Forward { get; } = new Vector3(0f, 0f, -1f);

        /// <summary>
        /// <see cref="Vector3"/> with components 0, 0, 1.
        /// </summary>
        public static Vector3 Backward { get; } = new Vector3(0f, 0f, 1f);

        #endregion

        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Blue));

        internal string DebuggerDisplay => string.Concat(
            X.ToString(), "  ",
            Y.ToString(), "  ",
            Z.ToString());

        [IgnoreDataMember]
        public FastVector3 Base;

        /// <summary>
        /// The x coordinate of this <see cref="Vector4"/>.
        /// </summary>
        [DataMember]
        public float X { readonly get => Base.X; set => Base.X = value; }

        /// <summary>
        /// The y coordinate of this <see cref="Vector4"/>.
        /// </summary>
        [DataMember]
        public float Y { readonly get => Base.Y; set => Base.Y = value; }

        /// <summary>
        /// The z coordinate of this <see cref="Vector4"/>.
        /// </summary>
        [DataMember]
        public float Z { readonly get => Base.Z; set => Base.Z = value; }

        /// <summary>
        /// Gets or sets the x and y coordinates as a <see cref="Vector2"/>.
        /// </summary>
        [IgnoreDataMember]
        public Vector2 XY { readonly get => ToVector2(); set { X = value.X; Y = value.Y; } }

        /// <summary>
        /// Gets or sets the z and y coordinates as a <see cref="Vector2"/>.
        /// </summary>
        [IgnoreDataMember]
        public Vector2 ZY { readonly get => new Vector2(Z, Y); set { Z = value.X; Y = value.Y; } }

        #region Constructors

        /// <summary>
        /// Constructs a 3D vector with X, Y and Z from three values.
        /// </summary>
        /// <param name="x">The x coordinate in 3D-space.</param>
        /// <param name="y">The y coordinate in 3D-space.</param>
        /// <param name="z">The z coordinate in 3D-space.</param>
        public Vector3(float x, float y, float z)
        {
            Base = new FastVector3(x, y, z);
        }

        /// <summary>
        /// Constructs a 3D vector with X, Y and Z set to the same value.
        /// </summary>
        /// <param name="value">The x, y and z coordinates in 3D-space.</param>
        public Vector3(float value)
        {
            Base = new FastVector3(value);
        }

        /// <summary>
        /// Constructs a 3D vector with X, Y from <see cref="Vector2"/> and Z from a scalar.
        /// </summary>
        /// <param name="value">The x and y coordinates in 3D-space.</param>
        /// <param name="z">The z coordinate in 3D-space.</param>
        public Vector3(in Vector2 value, float z)
        {
            Base = new FastVector3(value.Base, z);
        }

        #endregion

        #region IPackedVector

        public void FromVector4(in Vector4 vector)
        {
            Base.X = vector.Base.X;
            Base.Y = vector.Base.Y;
            Base.Z = vector.Base.Z;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base = new FastVector4(Base, 1);
        }

        void IVector.FromScaledVector4(in Vector4 scaledVector)
        {
            FromVector4(scaledVector);
        }

        readonly void IVector.ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
        }

        #endregion

        #region IPixel

        void IPixel.FromGray8(Gray8 source)
        {
            source.ToScaledVector4(out var vector);
            FromVector4(vector);
        }

        void IPixel.FromGray16(Gray16 source)
        {
            source.ToScaledVector4(out var vector);
            FromVector4(vector);
        }

        void IPixel.FromGrayAlpha16(GrayAlpha16 source)
        {
            source.ToScaledVector4(out var vector);
            FromVector4(vector);
        }

        void IPixel.FromRgb24(Rgb24 source)
        {
            source.ToScaledVector4(out var vector);
            FromVector4(vector);
        }

        void IPixel.FromRgb48(Rgb48 source)
        {
            source.ToScaledVector4(out var vector);
            FromVector4(vector);
        }

        void IPixel.FromRgba64(Rgba64 source)
        {
            source.ToScaledVector4(out var vector);
            FromVector4(vector);
        }

        #endregion

        #region Barycentric

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
        public static Vector3 Barycentric(in Vector3 a, in Vector3 b, in Vector3 c, float amount1, float amount2)
        {
            return new Vector3(
                MathHelper.Barycentric(a.X, b.X, c.X, amount1, amount2),
                MathHelper.Barycentric(a.Y, b.Y, c.Y, amount1, amount2),
                MathHelper.Barycentric(a.Z, b.Z, c.Z, amount1, amount2));
        }

        #endregion

        #region CatmullRom

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains CatmullRom interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">The first vector in interpolation.</param>
        /// <param name="b">The second vector in interpolation.</param>
        /// <param name="c">The third vector in interpolation.</param>
        /// <param name="d">The fourth vector in interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of CatmullRom interpolation.</returns>
        public static Vector3 CatmullRom(in Vector3 a, in Vector3 b, in Vector3 c, in Vector3 d, float amount)
        {
            return new Vector3(
                MathHelper.CatmullRom(a.X, b.X, c.X, d.X, amount),
                MathHelper.CatmullRom(a.Y, b.Y, c.Y, d.Y, amount),
                MathHelper.CatmullRom(a.Z, b.Z, c.Z, d.Z, amount));
        }

        #endregion

        #region Ceiling

        /// <summary>
        /// Rounds components towards positive infinity and returns them.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The rounded <see cref="Vector3"/>.</returns>
        public static Vector3 Ceiling(in Vector3 value)
        {
            return new Vector3(
                MathF.Ceiling(value.X),
                MathF.Ceiling(value.Y),
                MathF.Ceiling(value.Z));
        }

        /// <summary>
        /// Round components towards positive infinity.
        /// </summary>
        public void Ceiling()
        {
            this = Ceiling(this);
        }

        #endregion

        #region Clamp

        /// <summary>
        /// Clamps the specified vector within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public static Vector3 Clamp(in Vector3 value, in Vector3 min, in Vector3 max)
        {
            return FastVector3.Clamp(value, min, max);
        }

        /// <summary>
        /// Clamp this vector within a range.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        public void Clamp(in Vector3 min, in Vector3 max)
        {
            this = Clamp(this, min, max);
        }

        /// <summary>
        /// Clamps the specified vector within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public static Vector3 Clamp(in Vector3 value, float min, float max)
        {
            return FastVector3.Clamp(value, new FastVector3(min), new FastVector3(max));
        }

        /// <summary>
        /// Clamp this vector within a range.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        public void Clamp(float min, float max)
        {
            this = Clamp(this, min, max);
        }

        #endregion

        #region Cross

        /// <summary>
        /// Computes the cross product of two vectors.
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>The cross product of two vectors.</returns>
        public static Vector3 Cross(in Vector3 left, in Vector3 right)
        {
            return FastVector3.Cross(left, right);
        }

        #endregion

        #region Deconstruct

        /// <summary>
        /// Deconstruction method for <see cref="Vector3"/>.
        /// </summary>
        public readonly void Deconstruct(out float x, out float y, out float z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        #endregion

        #region DistanceSquared

        /// <summary>
        /// Returns the squared distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The squared distance between two vectors.</returns>
        public static float DistanceSquared(in Vector3 a, in Vector3 b)
        {
            return FastVector3.DistanceSquared(a, b);
        }

        #endregion

        #region Distance

        /// <summary>
        /// Returns the distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The distance between two vectors.</returns>
        public static float Distance(in Vector3 a, in Vector3 b)
        {
            return FastVector3.Distance(a, b);
        }

        #endregion

        #region Dot

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product of two vectors.</returns>
        public static float Dot(in Vector3 a, in Vector3 b)
        {
            return FastVector3.Dot(a, b);
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
            return obj is Vector3 other && this == other;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="other">The <see cref="Vector3"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public bool Equals(Vector3 other)
        {
            return Base.Equals(other.Base);
        }

        /// <summary>
        /// Compares whether two <see cref="Vector3"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Vector3"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Vector3"/> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(in Vector3 a, in Vector3 b)
        {
            return a.Base == b.Base;
        }

        /// <summary>
        /// Compares whether two <see cref="Vector3"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Vector3"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Vector3"/> instance on the right of the not equal sign.</param>
        /// <returns><see langword="true"/> if the instances are not equal; <see langword="false"/> otherwise.</returns>	
        public static bool operator !=(in Vector3 a, in Vector3 b)
        {
            return a.Base != b.Base;
        }

        #endregion

        #region Floor

        /// <summary>
        /// Rounds components towards negative infinity and returns them.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The rounded <see cref="Vector3"/>.</returns>
        public static Vector3 Floor(in Vector3 value)
        {
            return new Vector3(
                MathF.Floor(value.X),
                MathF.Floor(value.Y),
                MathF.Floor(value.Z));
        }

        /// <summary>
        /// Round components towards negative infinity.
        /// </summary>
        public void Floor()
        {
            this = Floor(this);
        }

        #endregion

        #region GetHashCode

        /// <summary>
        /// Gets the hash code of this <see cref="Vector3"/>.
        /// </summary>
        public readonly override int GetHashCode() => Base.GetHashCode();

        #endregion

        #region Hermite

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
            in Vector3 position1, in Vector3 tangent1,
            in Vector3 position2, in Vector3 tangent2,
            float amount)
        {
            return new Vector3(
                MathHelper.Hermite(position1.X, tangent1.X, position2.X, tangent2.X, amount),
                MathHelper.Hermite(position1.Y, tangent1.Y, position2.Y, tangent2.Y, amount),
                MathHelper.Hermite(position1.Z, tangent1.Z, position2.Z, tangent2.Z, amount));
        }

        #endregion

        #region Length

        /// <summary>
        /// Returns the length of this <see cref="Vector3"/>.
        /// </summary>
        public readonly float Length()
        {
            return Base.Length();
        }

        #endregion

        #region LengthSquared

        /// <summary>
        /// Returns the squared length of this <see cref="Vector3"/>.
        /// </summary>
        public readonly float LengthSquared()
        {
            return Base.LengthSquared();
        }

        #endregion

        #region Lerp

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains linear interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector3 Lerp(in Vector3 a, in Vector3 b, float amount)
        {
            return FastVector3.Lerp(a, b, amount);
        }

        #endregion

        #region LerpPrecise

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains linear interpolation of the specified vectors.
        /// Less efficient but more precise compared to <see cref="Lerp(in Vector3, in Vector3, float)"/>.
        /// See remarks section of <see cref="MathHelper.LerpPrecise"/> for more info.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector3 LerpPrecise(in Vector3 a, in Vector3 b, float amount)
        {
            return new Vector3(
                MathHelper.LerpPrecise(a.X, b.X, amount),
                MathHelper.LerpPrecise(a.Y, b.Y, amount),
                MathHelper.LerpPrecise(a.Z, b.Z, amount));
        }

        #endregion

        #region Max

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a maximal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector3"/> with maximal values from the two vectors.</returns>
        public static Vector3 Max(in Vector3 a, in Vector3 b)
        {
            return FastVector3.Max(a, b);
        }

        #endregion

        #region Min

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a minimal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector3"/> with minimal values from the two vectors.</returns>
        public static Vector3 Min(in Vector3 a, in Vector3 b)
        {
            return FastVector3.Min(a, b);
        }

        #endregion

        #region Add (operator +)

        /// <summary>
        /// Performs vector addition on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector to add.</param>
        /// <param name="b">The second vector to add.</param>
        /// <returns>The result of the vector addition.</returns>
        public static Vector3 Add(in Vector3 a, in Vector3 b)
        {
            return FastVector3.Add(a, b);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/> on the left of the add sign.</param>
        /// <param name="b">Source <see cref="Vector3"/> on the right of the add sign.</param>
        /// <returns>Sum of the vectors.</returns>
        public static Vector3 operator +(in Vector3 a, in Vector3 b)
        {
            return a.Base + b.Base;
        }

        #endregion

        #region Subtract (operator -)

        /// <summary>
        /// Performs vector subtraction on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/>.</param>
        /// <param name="right">Source <see cref="Vector3"/>.</param>
        /// <returns>The result of the vector subtraction.</returns>
        public static Vector3 Subtract(in Vector3 left, in Vector3 right)
        {
            return FastVector3.Subtract(left, right);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/> on the left of the sub sign.</param>
        /// <param name="right">Source <see cref="Vector3"/> on the right of the sub sign.</param>
        /// <returns>Result of the vector subtraction.</returns>
        public static Vector3 operator -(in Vector3 left, in Vector3 right)
        {
            return left.Base - right.Base;
        }

        #endregion

        #region Negate (operator -)

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains the specified vector inversion.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The result of the vector inversion.</returns>
        public static Vector3 Negate(in Vector3 value)
        {
            return FastVector3.Negate(value);
        }

        /// <summary>
        /// Negates values in the specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/> on the right of the sub sign.</param>
        /// <returns>Result of the inversion.</returns>
        public static Vector3 operator -(in Vector3 value)
        {
            return -value.Base;
        }

        #endregion

        #region Multiply (operator *)

        /// <summary>
        /// Performs vector multiplication on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/>.</param>
        /// <param name="b">Source <see cref="Vector3"/>.</param>
        /// <returns>The result of the vector multiplication.</returns>
        public static Vector3 Multiply(in Vector3 a, in Vector3 b)
        {
            return FastVector3.Multiply(a, b);
        }

        /// <summary>
        /// Multiplies a <see cref="Vector3"/> by a scalar value.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>The result of the vector multiplication with a scalar.</returns>
        public static Vector3 Multiply(in Vector3 value, float scaleFactor)
        {
            return FastVector3.Multiply(value, scaleFactor);
        }

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/> on the left of the mul sign.</param>
        /// <param name="b">Source <see cref="Vector3"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication.</returns>
        public static Vector3 operator *(in Vector3 a, in Vector3 b)
        {
            return a.Base * b.Base;
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector3 operator *(in Vector3 value, float scaleFactor)
        {
            return value.Base * scaleFactor;
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="scaleFactor">Scalar value on the left of the mul sign.</param>
        /// <param name="value">Source <see cref="Vector3"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector3 operator *(float scaleFactor, in Vector3 value)
        {
            return scaleFactor * value.Base;
        }

        #endregion

        #region Divide (operator /)

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by the components of another <see cref="Vector3"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/>.</param>
        /// <param name="right">Divisor <see cref="Vector3"/>.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector3 Divide(in Vector3 left, in Vector3 right)
        {
            return FastVector3.Divide(left, right);
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="divisor">Divisor scalar.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector3 Divide(in Vector3 value, float divisor)
        {
            return FastVector3.Divide(value, divisor);
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by the components of another <see cref="Vector3"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/> on the left of the div sign.</param>
        /// <param name="right">Divisor <see cref="Vector3"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector3 operator /(in Vector3 left, in Vector3 right)
        {
            return left.Base / right.Base;
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/> on the left of the div sign.</param>
        /// <param name="divisor">Divisor scalar on the right of the div sign.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector3 operator /(in Vector3 value, float divisor)
        {
            return value.Base / divisor;
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a normalized values from another vector.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The normalized unit vector.</returns>
        public static Vector3 Normalize(in Vector3 value)
        {
            return FastVector3.Normalize(value);
        }

        /// <summary>
        /// Normalizes this <see cref="Vector3"/> to a unit vector with the same direction.
        /// </summary>
        public void Normalize()
        {
            this = Normalize(this);
        }

        #endregion

        #region Reflect

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains reflect vector of the given vector and normal.
        /// </summary>
        /// <param name="vector">Source <see cref="Vector3"/>.</param>
        /// <param name="normal">Reflection normal.</param>
        /// <returns>The reflect vector.</returns>
        public static Vector3 Reflect(in Vector3 vector, in Vector3 normal)
        {
            return FastVector3.Reflect(vector, normal);
        }

        #endregion

        #region Round

        /// <summary>
        /// Rounds components towards the nearest integer value and returns them.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The rounded <see cref="Vector3"/>.</returns>
        public static Vector3 Round(in Vector3 value)
        {
            return new Vector3(
                MathF.Round(value.X),
                MathF.Round(value.Y),
                MathF.Round(value.Z));
        }

        /// <summary>
        /// Round components towards the nearest integer value.
        /// </summary>
        public void Round()
        {
            this = Round(this);
        }

        #endregion

        #region SmoothStep

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains cubic interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/>.</param>
        /// <param name="b">Source <see cref="Vector3"/>.</param>
        /// <param name="amount">Weighting value.</param>
        /// <returns>Cubic interpolation of the specified vectors.</returns>
        public static Vector3 SmoothStep(in Vector3 a, in Vector3 b, float amount)
        {
            return new Vector3(
                MathHelper.SmoothStep(a.X, b.X, amount),
                MathHelper.SmoothStep(a.Y, b.Y, amount),
                MathHelper.SmoothStep(a.Z, b.Z, amount));
        }

        #endregion

        #region ToString

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Vector3"/> in the format:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Z:[<see cref="Z"/>]}
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Vector3"/>.</returns>
        public override readonly string ToString() => Base.ToString();

        #endregion

        #region ToVector2

        /// <summary>
        /// Gets the <see cref="Vector2"/> representation of this <see cref="Vector3"/>.
        /// </summary>
        public readonly Vector2 ToVector2() => UnsafeUtils.As<Vector3, Vector2>(this);

        #endregion

        #region Transform

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a transformation of 3D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">Source <see cref="Vector3"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector3"/>.</returns>
        public static Vector3 Transform(in Vector3 position, in Matrix matrix)
        {
            return FastVector3.Transform(position, matrix);
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a transformation of 3D-vector by the specified <see cref="Quaternion"/>,
        /// representing the rotation.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <returns>Transformed <see cref="Vector3"/>.</returns>
        public static Vector3 Transform(in Vector3 value, in Quaternion rotation)
        {
            return FastVector3.Transform(value, rotation);
        }

        /// <summary>
        /// Apply transformation on vectors by the specified <see cref="Matrix"/> and stores the results in a span.
        /// </summary>
        /// <param name="source">The source span of vectors.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">The destination span.</param>
        public static void Transform(ReadOnlySpan<Vector3> source, in Matrix matrix, Span<Vector3> destination)
        {
            ArgumentGuard.AssertSourceLargerThanDestination(source, destination);

            for (int i = 0; i < source.Length; i++)
                destination[i] = Transform(source[i], matrix);
        }

        /// <summary>
        /// Apply transformation on vectors by the specified <see cref="Quaternion"/> and stores the results in a span.
        /// </summary>
        /// <param name="source">The source span of vectors.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> containing the rotation.</param>
        /// <param name="destination">The destination span.</param>
        public static void Transform(ReadOnlySpan<Vector3> source, in Quaternion rotation, Span<Vector3> destination)
        {
            ArgumentGuard.AssertSourceLargerThanDestination(source, destination);

            for (int i = 0; i < source.Length; i++)
                destination[i] = Transform(source[i], rotation);
        }

        #endregion

        #region TransformNormal

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a transformation of
        /// the specified normal by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="normal">Source <see cref="Vector3"/> which represents a normal vector.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed normal.</returns>
        public static Vector3 TransformNormal(in Vector3 normal, in Matrix matrix)
        {
            return FastVector3.TransformNormal(normal, matrix);
        }

        /// <summary>
        /// Apply transformation on normals by the specified <see cref="Matrix"/> and stores the results in a span.
        /// </summary>
        /// <param name="source">The source span of vectors.</param>
        /// <param name="rotation">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">The destination span.</param>
        public static void TransformNormal(ReadOnlySpan<Vector3> source, in Matrix matrix, Span<Vector3> destination)
        {
            ArgumentGuard.AssertSourceLargerThanDestination(source, destination);

            for (int i = 0; i < source.Length; i++)
                destination[i] = TransformNormal(source[i], matrix);
        }

        #endregion

        public static implicit operator FastVector3(in Vector3 value)
        {
            return value.Base;
        }

        public static implicit operator Vector3(in FastVector3 value)
        {
            return new Vector3 { Base = value };
        }
    }
}
