// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Framework
{
    /// <summary>
    /// Describes a 4D-vector.
    /// </summary>
#if XNADESIGNPROVIDED
    [System.ComponentModel.TypeConverter(typeof(Design.Vector4TypeConverter))]
#endif
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Vector4 : IEquatable<Vector4>, IPixel
    {
        #region Public Constants

        /// <summary>
        /// Returns a <see cref="Vector4"/> with all components set to 0.
        /// </summary>
        public static readonly Vector4 Zero = new Vector4(0, 0, 0, 0);

        /// <summary>
        /// Returns a <see cref="Vector4"/> with all components set to 0.5.
        /// </summary>
        public static readonly Vector4 Half = new Vector4(0.5f);

        /// <summary>
        /// Returns a <see cref="Vector4"/> with all components set to 1.
        /// </summary>
        public static readonly Vector4 One = new Vector4(1f);

        /// <summary>
        /// Returns a <see cref="Vector4"/> with components 1, 0, 0, 0.
        /// </summary>
        public static readonly Vector4 UnitX = new Vector4(1f, 0f, 0f, 0f);

        /// <summary>
        /// Returns a <see cref="Vector4"/> with components 0, 1, 0, 0.
        /// </summary>
        public static readonly Vector4 UnitY = new Vector4(0f, 1f, 0f, 0f);

        /// <summary>
        /// Returns a <see cref="Vector4"/> with components 0, 0, 1, 0.
        /// </summary>
        public static readonly Vector4 UnitZ = new Vector4(0f, 0f, 1f, 0f);

        /// <summary>
        /// Returns a <see cref="Vector4"/> with components 0, 0, 0, 1.
        /// </summary>
        public static readonly Vector4 UnitW = new Vector4(0, 0, 0, 1);

        #endregion

        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(float) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(float) * 8),
            new VectorComponent(VectorComponentType.Blue, sizeof(float) * 8),
            new VectorComponent(VectorComponentType.Alpha, sizeof(float) * 8));

        private string DebuggerDisplay => string.Concat(
            X.ToString(), "  ",
            Y.ToString(), "  ",
            Z.ToString(), "  ",
            W.ToString());

        /// <summary>
        /// The x coordinate of this <see cref="Vector4"/>.
        /// </summary>
        [DataMember]
        public float X;

        /// <summary>
        /// The y coordinate of this <see cref="Vector4"/>.
        /// </summary>
        [DataMember]
        public float Y;

        /// <summary>
        /// The z coordinate of this <see cref="Vector4"/>.
        /// </summary>
        [DataMember]
        public float Z;

        /// <summary>
        /// The w coordinate of this <see cref="Vector4"/>.
        /// </summary>
        [DataMember]
        public float W;

        #region Public Properties

        /// <summary>
        /// Gets or sets the x and y coordinates as a <see cref="Vector2"/>.
        /// </summary>
        [IgnoreDataMember]
        public Vector2 XY { readonly get => ToVector2(); set { X = value.X; Y = value.Y; } }

        /// <summary>
        /// Gets or sets the z and w coordinates as a <see cref="Vector2"/>.
        /// </summary>
        [IgnoreDataMember]
        public Vector2 ZW { readonly get => new Vector2(Z, W); set { Z = value.X; W = value.Y; } }

        /// <summary>
        /// Gets or sets the z and y coordinates as a <see cref="Vector2"/>.
        /// </summary>
        [IgnoreDataMember]
        public Vector2 ZY { readonly get => new Vector2(Z, Y); set { Z = value.X; Y = value.Y; } }

        /// <summary>
        /// Gets or sets the x and w coordinates as a <see cref="Vector2"/>.
        /// </summary>
        [IgnoreDataMember]
        public Vector2 XW { readonly get => new Vector2(X, W); set { X = value.X; W = value.Y; } }

        /// <summary>
        /// Gets or sets the x and y coordinates as a <see cref="Vector2"/>.
        /// </summary>
        [IgnoreDataMember]
        public Vector3 XYZ { readonly get => ToVector3(); set { X = value.X; Y = value.Y; Z = value.Z; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a 4D vector with XYZW values.
        /// </summary>
        /// <param name="x">The x coordinate in 4D-space.</param>
        /// <param name="y">The y coordinate in 4D-space.</param>
        /// <param name="z">The z coordinate in 4D-space.</param>
        /// <param name="w">The w coordinate in 4D-space.</param>
        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Constructs a 4D vector with XZ from a <see cref="Vector2"/> and ZW from the scalars.
        /// </summary>
        /// <param name="value">The x and y coordinates in 4D-space.</param>
        /// <param name="z">The z coordinate in 4D-space.</param>
        /// <param name="w">The w coordinate in 4D-space.</param>
        public Vector4(in Vector2 value, float z, float w)
        {
            X = value.X;
            Y = value.Y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Constructs a 4D vector with XYZ from a <see cref="Vector3"/> and W from a scalar.
        /// </summary>
        /// <param name="value">The x, y and z coordinates in 4D-space.</param>
        /// <param name="w">The w coordinate in 4D-space.</param>
        public Vector4(in Vector3 value, float w)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = w;
        }

        /// <summary>
        /// Constructs a 4D vector with XYZW set to the same value.
        /// </summary>
        /// <param name="value">The x, y, z and w coordinates in 4D-space.</param>
        public Vector4(float value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="Vector2"/> representation of this vector.
        /// </summary>
        public readonly Vector2 ToVector2() => UnsafeUtils.As<Vector4, Vector2>(this);

        /// <summary>
        /// Gets the <see cref="Vector3"/> representation of this vector.
        /// </summary>
        public readonly Vector3 ToVector3() => UnsafeUtils.As<Vector4, Vector3>(this);

        #region IPackedVector

        void IPackedVector.FromVector4(in Vector4 vector) => this = vector;

        readonly void IPackedVector.ToVector4(out Vector4 vector) => vector = this;

        void IPackedVector.FromScaledVector4(in Vector4 scaledVector) => this = scaledVector;

        readonly void IPackedVector.ToScaledVector4(out Vector4 scaledVector) => scaledVector = this;

        #endregion

        #region IPixel

        public void FromColor(Color source) => source.ToScaledVector4(out this);

        void IPixel.FromGray8(Gray8 source) => source.ToScaledVector4(out this);

        void IPixel.FromGray16(Gray16 source) => source.ToScaledVector4(out this);

        void IPixel.FromGrayAlpha16(GrayAlpha16 source) => source.ToScaledVector4(out this);

        void IPixel.FromRgb24(Rgb24 source) => source.ToScaledVector4(out this);

        void IPixel.FromRgb48(Rgb48 source) => source.ToScaledVector4(out this);

        void IPixel.FromRgba64(Rgba64 source) => source.ToScaledVector4(out this);

        public readonly void ToColor(ref Color destination) => destination.FromScaledVector4(this);

        #endregion

        #region Add

        /// <summary>
        /// Performs vector addition on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector to add.</param>
        /// <param name="b">The second vector to add.</param>
        /// <param name="result">The result of the vector addition.</param>
        public static void Add(in Vector4 a, in Vector4 b, out Vector4 result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;
            result.W = a.W + b.W;
        }

        /// <summary>
        /// Performs vector addition on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector to add.</param>
        /// <param name="b">The second vector to add.</param>
        /// <returns>The result of the vector addition.</returns>
        public static Vector4 Add(in Vector4 a, in Vector4 b)
        {
            Add(a, b, out var result);
            return result;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector4"/> on the left of the add sign.</param>
        /// <param name="b">Source <see cref="Vector4"/> on the right of the add sign.</param>
        /// <returns>Sum of the vectors.</returns>
        public static Vector4 operator +(in Vector4 a, in Vector4 b)
        {
            return Add(a, b);
        }

        #endregion

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains the cartesian coordinates
        /// of a vector specified in barycentric coordinates and relative to 4D-triangle.
        /// </summary>
        /// <param name="a">The first vector of 4D-triangle.</param>
        /// <param name="b">The second vector of 4D-triangle.</param>
        /// <param name="c">The third vector of 4D-triangle.</param>
        /// <param name="amount1">Barycentric scalar <c>b2</c> which represents a weighting factor towards second vector of 4D-triangle.</param>
        /// <param name="amount2">Barycentric scalar <c>b3</c> which represents a weighting factor towards third vector of 4D-triangle.</param>
        /// <returns>The cartesian translation of barycentric coordinates.</returns>
        public static Vector4 Barycentric(in Vector4 a, in Vector4 b, in Vector4 c, float amount1, float amount2) => new Vector4(
            MathHelper.Barycentric(a.X, b.X, c.X, amount1, amount2),
            MathHelper.Barycentric(a.Y, b.Y, c.Y, amount1, amount2),
            MathHelper.Barycentric(a.Z, b.Z, c.Z, amount1, amount2),
            MathHelper.Barycentric(a.W, b.W, c.W, amount1, amount2));

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains CatmullRom interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">The first vector in interpolation.</param>
        /// <param name="b">The second vector in interpolation.</param>
        /// <param name="c">The third vector in interpolation.</param>
        /// <param name="d">The fourth vector in interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of CatmullRom interpolation.</returns>
        public static Vector4 CatmullRom(in Vector4 a, in Vector4 b, in Vector4 c, in Vector4 d, float amount) => new Vector4(
            MathHelper.CatmullRom(a.X, b.X, c.X, d.X, amount),
            MathHelper.CatmullRom(a.Y, b.Y, c.Y, d.Y, amount),
            MathHelper.CatmullRom(a.Z, b.Z, c.Z, d.Z, amount),
            MathHelper.CatmullRom(a.W, b.W, c.W, d.W, amount));

        #region Clamp

        /// <summary>
        /// Clamps the specified value within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <param name="result">The clamped value.</param>
        public static void Clamp(in Vector4 value, in Vector4 min, in Vector4 max, out Vector4 result)
        {
            result.X = MathHelper.Clamp(value.X, min.X, max.X);
            result.Y = MathHelper.Clamp(value.Y, min.Y, max.Y);
            result.Z = MathHelper.Clamp(value.Z, min.Z, max.Z);
            result.W = MathHelper.Clamp(value.W, min.W, max.W);
        }

        /// <summary>
        /// Clamps the specified value within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public static Vector4 Clamp(in Vector4 value, in Vector4 min, in Vector4 max)
        {
            Clamp(value, min, max, out var result);
            return result;
        }

        /// <summary>
        /// Clamps this <see cref="Vector4"/> within a range.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public void Clamp(in Vector4 min, in Vector4 max)
        {
            Clamp(this, min, max, out this);
        }

        /// <summary>
        /// Clamps the specified value within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <param name="result">The clamped value.</param>
        public static void Clamp(in Vector4 value, float min, float max, out Vector4 result)
        {
            result.X = MathHelper.Clamp(value.X, min, max);
            result.Y = MathHelper.Clamp(value.Y, min, max);
            result.Z = MathHelper.Clamp(value.Z, min, max);
            result.W = MathHelper.Clamp(value.W, min, max);
        }

        /// <summary>
        /// Clamps the specified value within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public static Vector4 Clamp(in Vector4 value, in float min, in float max)
        {
            Clamp(value, min, max, out var result);
            return result;
        }

        /// <summary>
        /// Clamps this <see cref="Vector4"/> within a range.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public void Clamp(float min, float max)
        {
            Clamp(this, min, max, out this);
        }

        #endregion

        /// <summary>
        /// Returns the distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The distance between two vectors.</returns>
        public static float Distance(in Vector4 a, in Vector4 b) => MathF.Sqrt(DistanceSquared(a, b));

        /// <summary>
        /// Returns the squared distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The squared distance between two vectors.</returns>
        public static float DistanceSquared(in Vector4 a, in Vector4 b) =>
            (a.X - b.X) * (a.X - b.X) +
            (a.Y - b.Y) * (a.Y - b.Y) +
            (a.Z - b.Z) * (a.Z - b.Z) +
            (a.W - b.W) * (a.W - b.W);

        #region Divide

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by the components of another <see cref="Vector4"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/>.</param>
        /// <param name="right">Divisor <see cref="Vector4"/>.</param>
        /// <param name="result">The result of dividing the vectors.</param>
        public static void Divide(in Vector4 left, in Vector4 right, out Vector4 result)
        {
            result.X = left.X / right.X;
            result.Y = left.Y / right.Y;
            result.Z = left.Z / right.Z;
            result.W = left.W / right.W;
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by the components of another <see cref="Vector4"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/>.</param>
        /// <param name="right">Divisor <see cref="Vector4"/>.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector4 Divide(in Vector4 left, in Vector4 right)
        {
            Divide(left, right, out var result);
            return result;
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="divider">Divisor scalar.</param>
        /// <param name="result">The result of dividing a vector by a scalar.</param>
        public static void Divide(in Vector4 value, float divider, out Vector4 result)
        {
            result.X = value.X / divider;
            result.Y = value.Y / divider;
            result.Z = value.Z / divider;
            result.W = value.W / divider;
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="divider">Divisor scalar.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector4 Divide(in Vector4 value, float divider)
        {
            Divide(value, divider, out var result);
            return result;
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by the components of another <see cref="Vector4"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/> on the left of the div sign.</param>
        /// <param name="right">Divisor <see cref="Vector4"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector4 operator /(in Vector4 left, in Vector4 right)
        {
            return Divide(left, right);
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/> on the left of the div sign.</param>
        /// <param name="divider">Divisor scalar on the right of the div sign.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector4 operator /(in Vector4 value, float divider)
        {
            return Divide(value, divider);
        }

        #endregion

        /// <summary>
        /// Returns a dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product of two vectors.</returns>
        public static float Dot(in Vector4 a, in Vector4 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public override bool Equals(object obj) => obj is Vector4 other ? this == other : false;

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Vector4"/>.
        /// </summary>
        /// <param name="other">The <see cref="Vector4"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public bool Equals(Vector4 other) => this == other;

        /// <summary>
        /// Gets the hash code of this <see cref="Vector4"/>.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains hermite spline interpolation.
        /// </summary>
        /// <param name="position1">The first position vector.</param>
        /// <param name="tangent1">The first tangent vector.</param>
        /// <param name="position2">The second position vector.</param>
        /// <param name="tangent2">The second tangent vector.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The hermite spline interpolation vector.</returns>
        public static Vector4 Hermite(
            in Vector4 position1, in Vector4 tangent1, in Vector4 position2, in Vector4 tangent2, float amount)
        {
            return new Vector4(
                MathHelper.Hermite(position1.X, tangent1.X, position2.X, tangent2.X, amount),
                MathHelper.Hermite(position1.Y, tangent1.Y, position2.Y, tangent2.Y, amount),
                MathHelper.Hermite(position1.Z, tangent1.Z, position2.Z, tangent2.Z, amount),
                MathHelper.Hermite(position1.W, tangent1.W, position2.W, tangent2.W, amount));
        }

        /// <summary>
        /// Returns the length of this <see cref="Vector4"/>.
        /// </summary>
        public readonly float Length() => MathF.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));

        /// <summary>
        /// Returns the squared length of this <see cref="Vector4"/>.
        /// </summary>
        public readonly float LengthSquared() => (X * X) + (Y * Y) + (Z * Z) + (W * W);

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains linear interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector4 Lerp(in Vector4 a, in Vector4 b, float amount) => new Vector4(
            MathHelper.Lerp(a.X, b.X, amount),
            MathHelper.Lerp(a.Y, b.Y, amount),
            MathHelper.Lerp(a.Z, b.Z, amount),
            MathHelper.Lerp(a.W, b.W, amount));

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains linear interpolation of the specified vectors.
        /// Uses <see cref="MathHelper.LerpPrecise"/> on MathHelper for the interpolation.
        /// Less efficient but more precise compared to <see cref="Vector4.Lerp(Vector4, Vector4, float)"/>.
        /// See remarks section of <see cref="MathHelper.LerpPrecise"/> on MathHelper for more info.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector4 LerpPrecise(in Vector4 a, in Vector4 b, float amount) => new Vector4(
            MathHelper.LerpPrecise(a.X, b.X, amount),
            MathHelper.LerpPrecise(a.Y, b.Y, amount),
            MathHelper.LerpPrecise(a.Z, b.Z, amount),
            MathHelper.LerpPrecise(a.W, b.W, amount));

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a maximal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector4"/> with maximal values from the two vectors.</returns>
        public static Vector4 Max(in Vector4 a, in Vector4 b) => new Vector4(
            Math.Max(a.X, b.X),
            Math.Max(a.Y, b.Y),
            Math.Max(a.Z, b.Z),
            Math.Max(a.W, b.W));

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a minimal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector4"/> with minimal values from the two vectors.</returns>
        public static Vector4 Min(in Vector4 a, in Vector4 b) => new Vector4(
            Math.Min(a.X, b.X),
            Math.Min(a.Y, b.Y),
            Math.Min(a.Z, b.Z),
            Math.Min(a.W, b.W));

        #region Multiply (operator *)

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a multiplication of two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector4"/>.</param>
        /// <param name="b">Source <see cref="Vector4"/>.</param>
        /// <param name="result">The result of the vector multiplication.</param>
        public static void Multiply(in Vector4 a, in Vector4 b, out Vector4 result)
        {
            result.X = a.X * b.X;
            result.Y = a.Y * b.Y;
            result.Z = a.Z * b.Z;
            result.W = a.W * b.W;
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a multiplication of two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector4"/>.</param>
        /// <param name="b">Source <see cref="Vector4"/>.</param>
        /// <returns>The result of the vector multiplication.</returns>
        public static Vector4 Multiply(in Vector4 a, in Vector4 b)
        {
            Multiply(a, b, out var result);
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a multiplication of <see cref="Vector4"/> and a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <param name="result">The result of the vector multiplication with a scalar.</param>
        public static void Multiply(in Vector4 value, float scaleFactor, out Vector4 result)
        {
            result.X = value.X * scaleFactor;
            result.Y = value.Y * scaleFactor;
            result.Z = value.Z * scaleFactor;
            result.W = value.W * scaleFactor;
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a multiplication of <see cref="Vector4"/> and a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>The result of the vector multiplication with a scalar.</returns>
        public static Vector4 Multiply(in Vector4 value, float scaleFactor)
        {
            Multiply(value, scaleFactor, out var result);
            return result;
        }

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="a">Source <see cref="Vector4"/> on the left of the mul sign.</param>
        /// <param name="b">Source <see cref="Vector4"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication.</returns>
        public static Vector4 operator *(in Vector4 a, in Vector4 b)
        {
            return Multiply(a, b);
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector4 operator *(in Vector4 value, float scaleFactor)
        {
            return Multiply(value, scaleFactor);
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="scaleFactor">Scalar value on the left of the mul sign.</param>
        /// <param name="value">Source <see cref="Vector4"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector4 operator *(float scaleFactor, in Vector4 value)
        {
            return Multiply(value, scaleFactor);
        }

        #endregion

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains the specified vector inversion.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <returns>The result of the vector inversion.</returns>
        public static Vector4 Negate(in Vector4 value) => -value;

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a normalized values from another vector.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <returns>Unit vector.</returns>
        public static Vector4 Normalize(in Vector4 value) => value / value.Length();

        /// <summary>
        /// Turns this <see cref="Vector4"/> to a unit vector with the same direction.
        /// </summary>
        public void Normalize() => Divide(this, Length(), out this);

        /// <summary>
        /// Round the members of this <see cref="Vector4"/> towards the nearest integer value.
        /// </summary>
        public void Round()
        {
            X = MathF.Round(X);
            Y = MathF.Round(Y);
            Z = MathF.Round(Z);
            W = MathF.Round(W);
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains members from 
        /// another vector rounded to the nearest integer value.
        /// </summary>
        public static Vector4 Round(in Vector4 value) => new Vector4(
            MathF.Round(value.X),
            MathF.Round(value.Y),
            MathF.Round(value.Z),
            MathF.Round(value.W));

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains cubic interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector4"/>.</param>
        /// <param name="b">Source <see cref="Vector4"/>.</param>
        /// <param name="amount">Weighting value.</param>
        /// <returns>Cubic interpolation of the specified vectors.</returns>
        public static Vector4 SmoothStep(in Vector4 a, in Vector4 b, float amount) => new Vector4(
                MathHelper.SmoothStep(a.X, b.X, amount),
                MathHelper.SmoothStep(a.Y, b.Y, amount),
                MathHelper.SmoothStep(a.Z, b.Z, amount),
                MathHelper.SmoothStep(a.W, b.W, amount));

        #region Subtract

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains subtraction of on <see cref="Vector4"/> from a another.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/>.</param>
        /// <param name="right">Source <see cref="Vector4"/>.</param>
        /// <param name="result">The result of the vector subtraction.</param>
        public static void Subtract(in Vector4 left, in Vector4 right, out Vector4 result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            result.W = left.W - right.W;
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains subtraction of on <see cref="Vector4"/> from a another.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/>.</param>
        /// <param name="right">Source <see cref="Vector4"/>.</param>
        /// <returns>The result of the vector subtraction.</returns>
        public static Vector4 Subtract(in Vector4 left, in Vector4 right)
        {
            Subtract(left, right, out var result);
            return result;
        }

        /// <summary>
        /// Subtracts a <see cref="Vector4"/> from a <see cref="Vector4"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/> on the left of the sub sign.</param>
        /// <param name="right">Source <see cref="Vector4"/> on the right of the sub sign.</param>
        /// <returns>Result of the vector subtraction.</returns>
        public static Vector4 operator -(in Vector4 left, in Vector4 right)
        {
            return Subtract(left, right);
        }

        #endregion

        #region Transform

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation of 2D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(in Vector2 value, in Matrix matrix)
        {
            return new Vector4(
                (value.X * matrix.M11) + (value.Y * matrix.M21) + matrix.M41,
                (value.X * matrix.M12) + (value.Y * matrix.M22) + matrix.M42,
                (value.X * matrix.M13) + (value.Y * matrix.M23) + matrix.M43,
                (value.X * matrix.M14) + (value.Y * matrix.M24) + matrix.M44);
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation of 2D-vector by the specified <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <returns>Transformed <see cref="Vector4"/>.</returns>
        [SuppressMessage("Remove unused parameter", "IDE0060")]
        public static Vector4 Transform(in Vector2 value, in Quaternion rotation)
        {
            // TODO: implement me
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation of 
        /// 3D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">Transformed <see cref="Vector4"/>.</param>
        public static void Transform(in Vector3 value, in Matrix matrix, out Vector4 result)
        {
            result.X = (value.X * matrix.M11) + (value.Y * matrix.M21) + (value.Z * matrix.M31) + matrix.M41;
            result.Y = (value.X * matrix.M12) + (value.Y * matrix.M22) + (value.Z * matrix.M32) + matrix.M42;
            result.Z = (value.X * matrix.M13) + (value.Y * matrix.M23) + (value.Z * matrix.M33) + matrix.M43;
            result.W = (value.X * matrix.M14) + (value.Y * matrix.M24) + (value.Z * matrix.M34) + matrix.M44;
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation of
        /// 3D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(in Vector3 value, in Matrix matrix)
        {
            Transform(value, matrix, out var result);
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation of 3D-vector by the specified <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <returns>Transformed <see cref="Vector4"/>.</returns>
        [SuppressMessage("Remove unused parameter", "IDE0060")]
        public static Vector4 Transform(in Vector3 value, in Quaternion rotation)
        {
            // TODO: implement me
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation of 
        /// 4D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">Transformed <see cref="Vector4"/>.</param>
        public static void Transform(in Vector4 value, in Matrix matrix, out Vector4 result)
        {
            result.X = (value.X * matrix.M11) + (value.Y * matrix.M21) + (value.Z * matrix.M31) + (value.W * matrix.M41);
            result.Y = (value.X * matrix.M12) + (value.Y * matrix.M22) + (value.Z * matrix.M32) + (value.W * matrix.M42);
            result.Z = (value.X * matrix.M13) + (value.Y * matrix.M23) + (value.Z * matrix.M33) + (value.W * matrix.M43);
            result.W = (value.X * matrix.M14) + (value.Y * matrix.M24) + (value.Z * matrix.M34) + (value.W * matrix.M44);
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation of 
        /// 4D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(in Vector4 value, in Matrix matrix)
        {
            Transform(value, matrix, out var result);
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation of 4D-vector by the specified <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <returns>Transformed <see cref="Vector4"/>.</returns>
        [SuppressMessage("Remove unused parameter", "IDE0060")]
        public static Vector4 Transform(in Vector4 value, in Quaternion rotation)
        {
            // TODO: implement me
            throw new NotImplementedException();
        }

        /// <summary>
        /// Apply transformation on vectors by the specified <see cref="Matrix"/> and places the results in a span.
        /// </summary>
        /// <param name="source">Source span.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">Destination span.</param>
        public static void Transform(
            ReadOnlySpan<Vector4> source, in Matrix matrix, Span<Vector4> destination)
        {
            ArgumentGuard.AssertSourceLargerThanDestination(source, destination);

            for (var i = 0; i < source.Length; i++)
                destination[i] = Transform(source[i], matrix);
        }

        /// <summary>
        /// Apply transformation on all vectors within span of <see cref="Vector4"/> by the 
        /// specified <see cref="Quaternion"/> and places the results in an another span.
        /// </summary>
        /// <param name="source">Source span.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <param name="destination">Destination span.</param>
        public static void Transform(
            ReadOnlySpan<Vector4> source, in Quaternion rotation, Span<Vector4> destination)
        {
            ArgumentGuard.AssertSourceLargerThanDestination(source, destination);
            for (var i = 0; i < source.Length; i++)
                destination[i] = Transform(source[i], rotation);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Vector4"/> in the format:
        /// {X:{X} Y:{Y} Z:{Z} W:{W}}
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Vector4"/>.</returns>
        public override readonly string ToString()
        {
            return "{X:" + X + " Y:" + Y + " Z:" + Z + " W:" + W + "}";
        }

        /// <summary>
        /// Deconstruction method for <see cref="Vector4"/>.
        /// </summary>
        public readonly void Deconstruct(out float x, out float y, out float z, out float w)
        {
            x = X;
            y = Y;
            z = Z;
            w = W;
        }

        /// <summary>
        /// Inverts values in the specified <see cref="Vector4"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/> on the right of the sub sign.</param>
        /// <returns>Result of the inversion.</returns>
        public static Vector4 operator -(in Vector4 value) => new Vector4(-value.X, -value.Y, -value.Z, -value.W);

        /// <summary>
        /// Compares whether two <see cref="Vector4"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Vector4"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Vector4"/> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(in Vector4 a, in Vector4 b) =>
            a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

        /// <summary>
        /// Compares whether two <see cref="Vector4"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Vector4"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Vector4"/> instance on the right of the not equal sign.</param>
        /// <returns><see langword="true"/> if the instances are not equal; <see langword="false"/> otherwise.</returns>	
        public static bool operator !=(in Vector4 a, in Vector4 b) => !(a == b);
    }
}
