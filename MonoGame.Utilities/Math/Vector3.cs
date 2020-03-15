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

        internal string DebuggerDisplay => string.Concat(
            X.ToString(), "  ",
            Y.ToString(), "  ",
            Z.ToString());

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

        #region IPackedVector

        public void FromVector4(in Vector4 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.X = X;
            vector.Y = Y;
            vector.Z = Z;
            vector.W = 1f;
        }

        void IPackedVector.FromScaledVector4(in Vector4 scaledVector) => FromVector4(scaledVector);

        readonly void IPackedVector.ToScaledVector4(out Vector4 scaledVector) => ToVector4(out scaledVector);

        #endregion

        #region IPixel

        public void FromColor(Color source)
        {
            source.ToScaledVector4(out var vector);
            FromVector4(vector);
        }

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

        public readonly void ToColor(ref Color destination)
        {
            ToVector4(out var vector);
            destination.FromVector4(vector);
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
        /// <param name="result">The cartesian translation of barycentric coordinates.</param>
        public static void Barycentric(
            in Vector3 a, in Vector3 b, in Vector3 c,
            float amount1, float amount2,
            out Vector3 result)
        {
            result.X = MathHelper.Barycentric(a.X, b.X, c.X, amount1, amount2);
            result.Y = MathHelper.Barycentric(a.Y, b.Y, c.Y, amount1, amount2);
            result.Z = MathHelper.Barycentric(a.Z, b.Z, c.Z, amount1, amount2);
        }

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
            Barycentric(a, b, c, amount1, amount2, out var result);
            return result;
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
        /// <param name="result">The result of CatmullRom interpolation.</param>
        public static void CatmullRom(
            in Vector3 a, in Vector3 b, in Vector3 c, in Vector3 d,
            float amount,
            out Vector3 result)
        {
            result.X = MathHelper.CatmullRom(a.X, b.X, c.X, d.X, amount);
            result.Y = MathHelper.CatmullRom(a.Y, b.Y, c.Y, d.Y, amount);
            result.Z = MathHelper.CatmullRom(a.Z, b.Z, c.Z, d.Z, amount);
        }

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
            CatmullRom(a, b, c, d, amount, out var result);
            return result;
        }

        #endregion

        #region Ceiling

        /// <summary>
        /// Rounds components towards positive infinity and stores them.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="result">The rounded <see cref="Vector3"/>.</param>
        public static void Ceiling(in Vector3 value, out Vector3 result)
        {
            result.X = MathF.Ceiling(value.X);
            result.Y = MathF.Ceiling(value.Y);
            result.Z = MathF.Ceiling(value.Z);
        }

        /// <summary>
        /// Rounds components towards positive infinity and returns them.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The rounded <see cref="Vector3"/>.</returns>
        public static Vector3 Ceiling(in Vector3 value)
        {
            Ceiling(value, out var result);
            return result;
        }

        /// <summary>
        /// Round components towards positive infinity.
        /// </summary>
        public void Ceiling()
        {
            Ceiling(this, out this);
        }

        #endregion

        #region Clamp

        /// <summary>
        /// Clamps the specified vector within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <param name="result">The clamped value.</param>
        public static void Clamp(in Vector3 value, in Vector3 min, in Vector3 max, out Vector3 result)
        {
            result.X = MathHelper.Clamp(value.X, min.X, max.X);
            result.Y = MathHelper.Clamp(value.Y, min.Y, max.Y);
            result.Z = MathHelper.Clamp(value.Z, min.Z, max.Z);
        }

        /// <summary>
        /// Clamps the specified vector within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public static Vector3 Clamp(in Vector3 value, in Vector3 min, in Vector3 max)
        {
            Clamp(value, min, max, out var result);
            return result;
        }

        /// <summary>
        /// Clamp this vector within a range.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        public void Clamp(in Vector3 min, in Vector3 max)
        {
            Clamp(this, min, max, out this);
        }

        /// <summary>
        /// Clamps the specified vector within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <param name="result">The clamped value.</param>
        public static void Clamp(in Vector3 value, float min, float max, out Vector3 result)
        {
            result.X = MathHelper.Clamp(value.X, min, max);
            result.Y = MathHelper.Clamp(value.Y, min, max);
            result.Z = MathHelper.Clamp(value.Z, min, max);
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
            Clamp(value, min, max, out var result);
            return result;
        }

        /// <summary>
        /// Clamp this vector within a range.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        public void Clamp(float min, float max)
        {
            Clamp(this, min, max, out this);
        }

        #endregion

        #region Cross

        /// <summary>
        /// Computes the cross product of two vectors.
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <param name="result">The cross product of two vectors.</param>
        public static void Cross(in Vector3 left, in Vector3 right, out Vector3 result)
        {
            result.X = left.Y * right.Z - right.Y * left.Z;
            result.Y = -(left.X * right.Z - right.X * left.Z);
            result.Z = left.X * right.Y - right.X * left.Y;
        }

        /// <summary>
        /// Computes the cross product of two vectors.
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>The cross product of two vectors.</returns>
        public static Vector3 Cross(in Vector3 left, in Vector3 right)
        {
            Cross(left, right, out var result);
            return result;
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
        /// <param name="result">The squared distance between two vectors.</param>
        public static void DistanceSquared(in Vector3 a, in Vector3 b, out float result)
        {
            result =
                (a.X - b.X) * (a.X - b.X) +
                (a.Y - b.Y) * (a.Y - b.Y) +
                (a.Z - b.Z) * (a.Z - b.Z);
        }

        /// <summary>
        /// Returns the squared distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The squared distance between two vectors.</returns>
        public static float DistanceSquared(in Vector3 a, in Vector3 b)
        {
            DistanceSquared(a, b, out var result);
            return result;
        }

        #endregion

        #region Distance

        /// <summary>
        /// Returns the distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="result">The distance between two vectors.</param>
        public static void Distance(in Vector3 a, in Vector3 b, out float result)
        {
            DistanceSquared(a, b, out var squareResult);
            result = MathF.Sqrt(squareResult);
        }

        /// <summary>
        /// Returns the distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The distance between two vectors.</returns>
        public static float Distance(in Vector3 a, in Vector3 b)
        {
            Distance(a, b, out var result);
            return result;
        }

        #endregion

        #region Dot

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="result">The dot product of two vectors.</param>
        public static void Dot(in Vector3 a, in Vector3 b, out float result)
        {
            result = a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product of two vectors.</returns>
        public static float Dot(in Vector3 a, in Vector3 b)
        {
            Dot(a, b, out var result);
            return result;
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
            return this == other;
        }

        /// <summary>
        /// Compares whether two <see cref="Vector3"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Vector3"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Vector3"/> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(in Vector3 a, in Vector3 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        /// <summary>
        /// Compares whether two <see cref="Vector3"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Vector3"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Vector3"/> instance on the right of the not equal sign.</param>
        /// <returns><see langword="true"/> if the instances are not equal; <see langword="false"/> otherwise.</returns>	
        public static bool operator !=(in Vector3 a, in Vector3 b)
        {
            return !(a == b);
        }

        #endregion

        #region Floor

        /// <summary>
        /// Rounds components towards negative infinity and stores them.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="result">The rounded <see cref="Vector3"/>.</param>
        public static void Floor(in Vector3 value, out Vector3 result)
        {
            result.X = MathF.Floor(value.X);
            result.Y = MathF.Floor(value.Y);
            result.Z = MathF.Floor(value.Z);
        }

        /// <summary>
        /// Rounds components towards negative infinity and returns them.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The rounded <see cref="Vector3"/>.</returns>
        public static Vector3 Floor(in Vector3 value)
        {
            Floor(value, out var result);
            return result;
        }

        /// <summary>
        /// Round components towards negative infinity.
        /// </summary>
        public void Floor()
        {
            Floor(this, out this);
        }

        #endregion

        #region GetHashCode

        /// <summary>
        /// Gets the hash code of this <see cref="Vector3"/>.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

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
        /// <param name="result">The hermite spline interpolation vector.</param>
        public static void Hermite(
            in Vector3 position1, in Vector3 tangent1,
            in Vector3 position2, in Vector3 tangent2,
            float amount,
            out Vector3 result)
        {
            result.X = MathHelper.Hermite(position1.X, tangent1.X, position2.X, tangent2.X, amount);
            result.Y = MathHelper.Hermite(position1.Y, tangent1.Y, position2.Y, tangent2.Y, amount);
            result.Z = MathHelper.Hermite(position1.Z, tangent1.Z, position2.Z, tangent2.Z, amount);
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
            in Vector3 position1, in Vector3 tangent1,
            in Vector3 position2, in Vector3 tangent2,
            float amount)
        {
            Hermite(position1, tangent1, position2, tangent2, amount, out var result);
            return result;
        }

        #endregion

        #region LengthSquared

        /// <summary>
        /// Returns the squared length of this <see cref="Vector3"/>.
        /// </summary>
        public readonly float LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z);
        }

        #endregion

        #region Length

        /// <summary>
        /// Returns the length of this <see cref="Vector3"/>.
        /// </summary>
        public readonly float Length()
        {
            return MathF.Sqrt(LengthSquared());
        }

        #endregion

        #region Lerp

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains linear interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <param name="result">The result of linear interpolation of the specified vectors.</param>
        public static void Lerp(in Vector3 a, in Vector3 b, float amount, out Vector3 result)
        {
            result.X = MathHelper.Lerp(a.X, b.X, amount);
            result.Y = MathHelper.Lerp(a.Y, b.Y, amount);
            result.Z = MathHelper.Lerp(a.Z, b.Z, amount);
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains linear interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector3 Lerp(in Vector3 a, in Vector3 b, float amount)
        {
            Lerp(a, b, amount, out var result);
            return result;
        }

        #endregion

        #region LerpPrecise

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains linear interpolation of the specified vectors.
        /// Less efficient but more precise compared to <see cref="Lerp(in Vector3, in Vector3, float, out Vector3)"/>.
        /// See remarks section of <see cref="MathHelper.LerpPrecise"/> for more info.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <param name="result">The result of linear interpolation of the specified vectors.</param>
        public static void LerpPrecise(in Vector3 a, in Vector3 b, float amount, out Vector3 result)
        {
            result.X = MathHelper.LerpPrecise(a.X, b.X, amount);
            result.Y = MathHelper.LerpPrecise(a.Y, b.Y, amount);
            result.Z = MathHelper.LerpPrecise(a.Z, b.Z, amount);
        }

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
            LerpPrecise(a, b, amount, out var result);
            return result;
        }

        #endregion

        #region Max

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a maximal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="result">The <see cref="Vector3"/> with maximal values from the two vectors.</param>
        public static void Max(in Vector3 a, in Vector3 b, out Vector3 result)
        {
            result.X = Math.Max(a.X, b.X);
            result.Y = Math.Max(a.Y, b.Y);
            result.Z = Math.Max(a.Z, b.Z);
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a maximal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector3"/> with maximal values from the two vectors.</returns>
        public static Vector3 Max(in Vector3 a, in Vector3 b)
        {
            Max(a, b, out var result);
            return result;
        }

        #endregion

        #region Min

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a minimal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="result">The <see cref="Vector3"/> with minimal values from the two vectors.</param>
        public static void Min(in Vector3 a, in Vector3 b, out Vector3 result)
        {
            result.X = Math.Min(a.X, b.X);
            result.Y = Math.Min(a.Y, b.Y);
            result.Z = Math.Min(a.Z, b.Z);
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a minimal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector3"/> with minimal values from the two vectors.</returns>
        public static Vector3 Min(in Vector3 a, in Vector3 b)
        {
            Min(a, b, out var result);
            return result;
        }

        #endregion

        #region Add (operator +)

        /// <summary>
        /// Performs vector addition on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector to add.</param>
        /// <param name="b">The second vector to add.</param>
        /// <param name="result">The result of the vector addition.</param>
        public static void Add(in Vector3 a, in Vector3 b, out Vector3 result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;
        }

        /// <summary>
        /// Performs vector addition on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector to add.</param>
        /// <param name="b">The second vector to add.</param>
        /// <returns>The result of the vector addition.</returns>
        public static Vector3 Add(in Vector3 a, in Vector3 b)
        {
            Add(a, b, out var result);
            return result;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/> on the left of the add sign.</param>
        /// <param name="b">Source <see cref="Vector3"/> on the right of the add sign.</param>
        /// <returns>Sum of the vectors.</returns>
        public static Vector3 operator +(in Vector3 a, in Vector3 b)
        {
            return Add(a, b);
        }

        #endregion

        #region Subtract (operator -)

        /// <summary>
        /// Performs vector subtraction on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/>.</param>
        /// <param name="right">Source <see cref="Vector3"/>.</param>
        /// <param name="result">The result of the vector subtraction.</param>
        public static void Subtract(in Vector3 left, in Vector3 right, out Vector3 result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
        }

        /// <summary>
        /// Performs vector subtraction on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/>.</param>
        /// <param name="right">Source <see cref="Vector3"/>.</param>
        /// <returns>The result of the vector subtraction.</returns>
        public static Vector3 Subtract(in Vector3 left, in Vector3 right)
        {
            Subtract(left, right, out var result);
            return result;
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/> on the left of the sub sign.</param>
        /// <param name="right">Source <see cref="Vector3"/> on the right of the sub sign.</param>
        /// <returns>Result of the vector subtraction.</returns>
        public static Vector3 operator -(in Vector3 left, in Vector3 right)
        {
            return Subtract(left, right);
        }

        #endregion

        #region Negate (operator -)

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains the specified vector inversion.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="result">The result of the vector inversion.</param>
        public static void Negate(in Vector3 value, out Vector3 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains the specified vector inversion.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The result of the vector inversion.</returns>
        public static Vector3 Negate(in Vector3 value)
        {
            Negate(value, out var result);
            return result;
        }

        /// <summary>
        /// Negates values in the specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/> on the right of the sub sign.</param>
        /// <returns>Result of the inversion.</returns>
        public static Vector3 operator -(in Vector3 value)
        {
            return Negate(value);
        }

        #endregion

        #region Multiply (operator *)

        /// <summary>
        /// Performs vector multiplication on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/>.</param>
        /// <param name="b">Source <see cref="Vector3"/>.</param>
        /// <param name="result">The result of the vector multiplication.</param>
        public static void Multiply(in Vector3 a, in Vector3 b, out Vector3 result)
        {
            result.X = a.X * b.X;
            result.Y = a.Y * b.Y;
            result.Z = a.Z * b.Z;
        }

        /// <summary>
        /// Performs vector multiplication on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/>.</param>
        /// <param name="b">Source <see cref="Vector3"/>.</param>
        /// <returns>The result of the vector multiplication.</returns>
        public static Vector3 Multiply(in Vector3 a, in Vector3 b)
        {
            Multiply(a, b, out var result);
            return result;
        }

        /// <summary>
        /// Multiplies a <see cref="Vector3"/> by a scalar value.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <param name="result">The result of the vector multiplication with a scalar.</param>
        public static void Multiply(in Vector3 value, float scaleFactor, out Vector3 result)
        {
            result.X = value.X * scaleFactor;
            result.Y = value.Y * scaleFactor;
            result.Z = value.Z * scaleFactor;
        }

        /// <summary>
        /// Multiplies a <see cref="Vector3"/> by a scalar value.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>The result of the vector multiplication with a scalar.</returns>
        public static Vector3 Multiply(in Vector3 value, float scaleFactor)
        {
            Multiply(value, scaleFactor, out var result);
            return result;
        }

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/> on the left of the mul sign.</param>
        /// <param name="b">Source <see cref="Vector3"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication.</returns>
        public static Vector3 operator *(in Vector3 a, in Vector3 b)
        {
            return Multiply(a, b);
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector3 operator *(in Vector3 value, float scaleFactor)
        {
            return Multiply(value, scaleFactor);
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="scaleFactor">Scalar value on the left of the mul sign.</param>
        /// <param name="value">Source <see cref="Vector3"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector3 operator *(float scaleFactor, in Vector3 value)
        {
            return Multiply(value, scaleFactor);
        }

        #endregion

        #region Divide (operator /)

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by the components of another <see cref="Vector3"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/>.</param>
        /// <param name="right">Divisor <see cref="Vector3"/>.</param>
        /// <param name="result">The result of dividing the vectors.</param>
        public static void Divide(in Vector3 left, in Vector3 right, out Vector3 result)
        {
            result.X = left.X / right.X;
            result.Y = left.Y / right.Y;
            result.Z = left.Z / right.Z;
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by the components of another <see cref="Vector3"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/>.</param>
        /// <param name="right">Divisor <see cref="Vector3"/>.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector3 Divide(in Vector3 left, in Vector3 right)
        {
            Divide(left, right, out var result);
            return result;
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="divider">Divisor scalar.</param>
        /// <param name="result">The result of dividing a vector by a scalar.</param>
        public static void Divide(in Vector3 value, float divider, out Vector3 result)
        {
            result.X = value.X / divider;
            result.Y = value.Y / divider;
            result.Z = value.Z / divider;
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="divider">Divisor scalar.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector3 Divide(in Vector3 value, float divider)
        {
            Divide(value, divider, out var result);
            return result;
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by the components of another <see cref="Vector3"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector3"/> on the left of the div sign.</param>
        /// <param name="right">Divisor <see cref="Vector3"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector3 operator /(in Vector3 left, in Vector3 right)
        {
            return Divide(left, right);
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector3"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/> on the left of the div sign.</param>
        /// <param name="divider">Divisor scalar on the right of the div sign.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector3 operator /(in Vector3 value, float divider)
        {
            return Divide(value, divider);
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a normalized values from another vector.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="result">The normalized unit vector.</param>
        public static void Normalize(in Vector3 value, out Vector3 result)
        {
            float factor = 1f / value.Length();
            result.X = value.X * factor;
            result.Y = value.Y * factor;
            result.Z = value.Z * factor;
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a normalized values from another vector.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The normalized unit vector.</returns>
        public static Vector3 Normalize(in Vector3 value)
        {
            Normalize(value, out var result);
            return result;
        }

        /// <summary>
        /// Normalizes this <see cref="Vector3"/> to a unit vector with the same direction.
        /// </summary>
        public void Normalize()
        {
            Normalize(this, out this);
        }

        #endregion

        #region Reflect

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains reflect vector of the given vector and normal.
        /// </summary>
        /// <param name="vector">Source <see cref="Vector3"/>.</param>
        /// <param name="normal">Reflection normal.</param>
        /// <param name="result">The reflect vector.</param>
        public static void Reflect(in Vector3 vector, in Vector3 normal, out Vector3 result)
        {
            // I is the original array
            // N is the normal of the incident plane
            // R = I - (2 * N * ( DotProduct[ I,N] ))

            Dot(vector, normal, out float dot);
            dot *= 2f;
            result = vector - normal * dot;
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains reflect vector of the given vector and normal.
        /// </summary>
        /// <param name="vector">Source <see cref="Vector3"/>.</param>
        /// <param name="normal">Reflection normal.</param>
        /// <returns>The reflect vector.</returns>
        public static Vector3 Reflect(in Vector3 vector, in Vector3 normal)
        {
            Reflect(vector, normal, out var result);
            return result;
        }

        #endregion

        #region Round

        /// <summary>
        /// Rounds components towards the nearest integer value and stores them.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="result">The rounded <see cref="Vector3"/>.</param>
        public static void Round(in Vector3 value, out Vector3 result)
        {
            result.X = MathF.Round(value.X);
            result.Y = MathF.Round(value.Y);
            result.Z = MathF.Round(value.Z);
        }

        /// <summary>
        /// Rounds components towards the nearest integer value and returns them.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <returns>The rounded <see cref="Vector3"/>.</returns>
        public static Vector3 Round(in Vector3 value)
        {
            Round(value, out var result);
            return result;
        }

        /// <summary>
        /// Round components towards the nearest integer value.
        /// </summary>
        public void Round()
        {
            Round(this, out this);
        }

        #endregion

        #region SmoothStep

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains cubic interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/>.</param>
        /// <param name="b">Source <see cref="Vector3"/>.</param>
        /// <param name="amount">Weighting value.</param>
        /// <param name="result">Cubic interpolation of the specified vectors.</param>
        public static void SmoothStep(in Vector3 a, in Vector3 b, float amount, out Vector3 result)
        {
            result.X = MathHelper.SmoothStep(a.X, b.X, amount);
            result.Y = MathHelper.SmoothStep(a.Y, b.Y, amount);
            result.Z = MathHelper.SmoothStep(a.Z, b.Z, amount);
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains cubic interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector3"/>.</param>
        /// <param name="b">Source <see cref="Vector3"/>.</param>
        /// <param name="amount">Weighting value.</param>
        /// <returns>Cubic interpolation of the specified vectors.</returns>
        public static Vector3 SmoothStep(in Vector3 a, in Vector3 b, float amount)
        {
            SmoothStep(a, b, amount, out var result);
            return result;
        }

        #endregion

        #region ToString

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Vector3"/> in the format:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Z:[<see cref="Z"/>]}
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Vector3"/>.</returns>
        public override readonly string ToString()
        {
            return "{X:" + X + " Y:" + Y + " Z:" + Z + "}";
        }

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
        /// <param name="result">Transformed <see cref="Vector3"/>.</param>
        public static void Transform(in Vector3 position, in Matrix matrix, out Vector3 result)
        {
            result.X = (position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41;
            result.Y = (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42;
            result.Z = (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43;
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a transformation of 3D-vector by the specified <see cref="Quaternion"/>,
        /// representing the rotation.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <param name="result">Transformed <see cref="Vector3"/>.</param>
        public static void Transform(in Vector3 value, in Quaternion rotation, out Vector3 result)
        {
            float x = 2 * (rotation.Y * value.Z - rotation.Z * value.Y);
            float y = 2 * (rotation.Z * value.X - rotation.X * value.Z);
            float z = 2 * (rotation.X * value.Y - rotation.Y * value.X);

            result.X = value.X + x * rotation.W + (rotation.Y * z - rotation.Z * y);
            result.Y = value.Y + y * rotation.W + (rotation.Z * x - rotation.X * z);
            result.Z = value.Z + z * rotation.W + (rotation.X * y - rotation.Y * x);
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a transformation of 3D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">Source <see cref="Vector3"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector3"/>.</returns>
        public static Vector3 Transform(in Vector3 position, in Matrix matrix)
        {
            Transform(position, matrix, out var result);
            return result;
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
            Transform(value, rotation, out var result);
            return result;
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
        /// Apply transformation on vectors by the specified <see cref="Quaternion"/> and stores the results in a span.
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
        /// Creates a new <see cref="Vector3"/> that contains a transformation of
        /// the specified normal by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="normal">Source <see cref="Vector3"/> which represents a normal vector.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">Transformed normal.</param>
        public static void TransformNormal(in Vector3 normal, in Matrix matrix, out Vector3 result)
        {
            result.X = (normal.X * matrix.M11) + (normal.Y * matrix.M21) + (normal.Z * matrix.M31);
            result.Y = (normal.X * matrix.M12) + (normal.Y * matrix.M22) + (normal.Z * matrix.M32);
            result.Z = (normal.X * matrix.M13) + (normal.Y * matrix.M23) + (normal.Z * matrix.M33);
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains a transformation of
        /// the specified normal by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="normal">Source <see cref="Vector3"/> which represents a normal vector.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed normal.</returns>
        public static Vector3 TransformNormal(in Vector3 normal, in Matrix matrix)
        {
            TransformNormal(normal, matrix, out var result);
            return result;
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
            {
                Vector3 normal = source[i];
                destination[i] = new Vector3(
                    (normal.X * matrix.M11) + (normal.Y * matrix.M21) + (normal.Z * matrix.M31),
                    (normal.X * matrix.M12) + (normal.Y * matrix.M22) + (normal.Z * matrix.M32),
                    (normal.X * matrix.M13) + (normal.Y * matrix.M23) + (normal.Z * matrix.M33));
            }
        }

        #endregion
    }
}
