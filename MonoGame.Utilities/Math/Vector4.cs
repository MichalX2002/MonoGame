// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using MonoGame.Framework.PackedVector;
using FastVector4 = System.Numerics.Vector4;

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

        [IgnoreDataMember]
        public FastVector4 Base;

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
        /// The w coordinate of this <see cref="Vector4"/>.
        /// </summary>
        [DataMember]
        public float W { readonly get => Base.W; set => Base.W = value; }

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
            Base = new FastVector4(x, y, z, w);
        }

        /// <summary>
        /// Constructs a 4D vector with XZ from a <see cref="Vector2"/> and ZW from the scalars.
        /// </summary>
        /// <param name="value">The x and y coordinates in 4D-space.</param>
        /// <param name="z">The z coordinate in 4D-space.</param>
        /// <param name="w">The w coordinate in 4D-space.</param>
        public Vector4(in Vector2 value, float z, float w)
        {
            Base = new FastVector4(value.Base, z, w);
        }

        /// <summary>
        /// Constructs a 4D vector with XYZ from a <see cref="Vector3"/> and W from a scalar.
        /// </summary>
        /// <param name="value">The x, y and z coordinates in 4D-space.</param>
        /// <param name="w">The w coordinate in 4D-space.</param>
        public Vector4(in Vector3 value, float w)
        {
            Base = new FastVector4(value.Base, w);
        }

        /// <summary>
        /// Constructs a 4D vector with XYZW set to the same value.
        /// </summary>
        /// <param name="value">The x, y, z and w coordinates in 4D-space.</param>
        public Vector4(float value)
        {
            Base = new FastVector4(value);
        }

        #endregion

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

        #region Add (operator +)

        /// <summary>
        /// Performs vector addition on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector to add.</param>
        /// <param name="b">The second vector to add.</param>
        /// <returns>The result of the vector addition.</returns>
        public static Vector4 Add(in Vector4 a, in Vector4 b)
        {
            return FastVector4.Add(a, b);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector4"/> on the left of the add sign.</param>
        /// <param name="b">Source <see cref="Vector4"/> on the right of the add sign.</param>
        /// <returns>Sum of the vectors.</returns>
        public static Vector4 operator +(in Vector4 a, in Vector4 b)
        {
            return a.Base + b.Base;
        }

        #endregion

        #region Barycentric

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
        public static Vector4 Barycentric(
            in Vector4 a, in Vector4 b, in Vector4 c, float amount1, float amount2)
        {
            return new Vector4(
                MathHelper.Barycentric(a.X, b.X, c.X, amount1, amount2),
                MathHelper.Barycentric(a.Y, b.Y, c.Y, amount1, amount2),
                MathHelper.Barycentric(a.Z, b.Z, c.Z, amount1, amount2),
                MathHelper.Barycentric(a.W, b.W, c.W, amount1, amount2));
        }

        #endregion

        #region CatmullRom

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains CatmullRom interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">The first vector in interpolation.</param>
        /// <param name="b">The second vector in interpolation.</param>
        /// <param name="c">The third vector in interpolation.</param>
        /// <param name="d">The fourth vector in interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of CatmullRom interpolation.</returns>
        public static Vector4 CatmullRom(in Vector4 a, in Vector4 b, in Vector4 c, in Vector4 d, float amount)
        {
            return new Vector4(
                MathHelper.CatmullRom(a.X, b.X, c.X, d.X, amount),
                MathHelper.CatmullRom(a.Y, b.Y, c.Y, d.Y, amount),
                MathHelper.CatmullRom(a.Z, b.Z, c.Z, d.Z, amount),
                MathHelper.CatmullRom(a.W, b.W, c.W, d.W, amount));
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
        public static Vector4 Clamp(in Vector4 value, in Vector4 min, in Vector4 max)
        {
            return FastVector4.Clamp(value, min, max);
        }

        /// <summary>
        /// Clamps this <see cref="Vector4"/> within a range.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public void Clamp(in Vector4 min, in Vector4 max)
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
        public static Vector4 Clamp(in Vector4 value, in float min, in float max)
        {
            return FastVector4.Clamp(value, new FastVector4(min), new FastVector4(max));
        }

        /// <summary>
        /// Clamps this <see cref="Vector4"/> within a range.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public void Clamp(float min, float max)
        {
            this = Clamp(this, min, max);
        }

        #endregion

        #region Deconstruct

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

        #endregion

        #region Distance

        /// <summary>
        /// Returns the distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The distance between two vectors.</returns>
        public static float Distance(in Vector4 a, in Vector4 b)
        {
            return FastVector4.Distance(a, b);
        }

        #endregion

        #region DistanceSquared

        /// <summary>
        /// Returns the squared distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The squared distance between two vectors.</returns>
        public static float DistanceSquared(in Vector4 a, in Vector4 b)
        {
            return FastVector4.DistanceSquared(a, b);
        }

        #endregion

        #region Divide (operator /)

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by the components of another <see cref="Vector4"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/>.</param>
        /// <param name="right">Divisor <see cref="Vector4"/>.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector4 Divide(in Vector4 left, in Vector4 right)
        {
            return FastVector4.Divide(left.Base, right.Base);
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="divisor">Divisor scalar.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector4 Divide(in Vector4 value, float divisor)
        {
            return FastVector4.Divide(value.Base, divisor);
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by the components of another <see cref="Vector4"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/> on the left of the div sign.</param>
        /// <param name="right">Divisor <see cref="Vector4"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector4 operator /(in Vector4 left, in Vector4 right)
        {
            return left.Base / right.Base;
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/> on the left of the div sign.</param>
        /// <param name="divisor">Divisor scalar on the right of the div sign.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector4 operator /(in Vector4 value, float divisor)
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
        public static float Dot(in Vector4 a, in Vector4 b)
        {
            return FastVector4.Dot(a, b);
        }

        #endregion

        #region Equals (operator ==, !=)

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public readonly override bool Equals(object obj)
        {
            return obj is Vector4 other ? this == other : false;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Vector4"/>.
        /// </summary>
        /// <param name="other">The <see cref="Vector4"/> to compare.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public readonly bool Equals(Vector4 other)
        {
            return Base.Equals(other.Base);
        }

        /// <summary>
        /// Compares whether two <see cref="Vector4"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Vector4"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Vector4"/> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(in Vector4 a, in Vector4 b)
        {
            return a.Base == b.Base;
        }

        /// <summary>
        /// Compares whether two <see cref="Vector4"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Vector4"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Vector4"/> instance on the right of the not equal sign.</param>
        /// <returns><see langword="true"/> if the instances are not equal; <see langword="false"/> otherwise.</returns>	
        public static bool operator !=(in Vector4 a, in Vector4 b)
        {
            return a.Base != b.Base;
        }

        #endregion

        #region GetHashCode

        /// <summary>
        /// Gets the hash code of this <see cref="Vector4"/>.
        /// </summary>
        public readonly override int GetHashCode()
        {
            return Base.GetHashCode();
        }

        #endregion

        #region Hermite

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
            in Vector4 position1, in Vector4 tangent1,
            in Vector4 position2, in Vector4 tangent2,
            float amount)
        {
            return new Vector4(
                MathHelper.Hermite(position1.X, tangent1.X, position2.X, tangent2.X, amount),
                MathHelper.Hermite(position1.Y, tangent1.Y, position2.Y, tangent2.Y, amount),
                MathHelper.Hermite(position1.Z, tangent1.Z, position2.Z, tangent2.Z, amount),
                MathHelper.Hermite(position1.W, tangent1.W, position2.W, tangent2.W, amount));
        }

        #endregion

        #region Length

        /// <summary>
        /// Returns the length of this <see cref="Vector4"/>.
        /// </summary>
        public readonly float Length()
        {
            return Base.Length();
        }

        #endregion

        #region LengthSquared

        /// <summary>
        /// Returns the squared length of this <see cref="Vector4"/>.
        /// </summary>
        public readonly float LengthSquared()
        {
            return Base.LengthSquared();
        }

        #endregion

        #region Lerp

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains linear interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector4 Lerp(in Vector4 a, in Vector4 b, float amount)
        {
            return FastVector4.Lerp(a, b, amount);
        }

        #endregion

        #region LerpPrecise

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains linear interpolation of the specified vectors.
        /// Less efficient but more precise compared to <see cref="Lerp(in Vector4, in Vector4, float)"/>.
        /// See remarks section of <see cref="MathHelper.LerpPrecise"/> for more info.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector4 LerpPrecise(in Vector4 a, in Vector4 b, float amount)
        {
            return new Vector4(
                MathHelper.LerpPrecise(a.X, b.X, amount),
                MathHelper.LerpPrecise(a.Y, b.Y, amount),
                MathHelper.LerpPrecise(a.Z, b.Z, amount),
                MathHelper.LerpPrecise(a.W, b.W, amount));
        }

        #endregion

        #region Max

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a maximal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector4"/> with maximal values from the two vectors.</returns>
        public static Vector4 Max(in Vector4 a, in Vector4 b)
        {
            return FastVector4.Max(a, b);
        }

        #endregion

        #region Min

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a minimal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector4"/> with minimal values from the two vectors.</returns>
        public static Vector4 Min(in Vector4 a, in Vector4 b)
        {
            return FastVector4.Min(a, b);
        }

        #endregion

        #region Multiply (operator *)

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a multiplication of two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector4"/>.</param>
        /// <param name="b">Source <see cref="Vector4"/>.</param>
        /// <returns>The result of the vector multiplication.</returns>
        public static Vector4 Multiply(in Vector4 a, in Vector4 b)
        {
            return FastVector4.Multiply(a, b);
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a multiplication of <see cref="Vector4"/> and a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>The result of the vector multiplication with a scalar.</returns>
        public static Vector4 Multiply(in Vector4 value, float scaleFactor)
        {
            return FastVector4.Multiply(value, scaleFactor);
        }

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="a">Source <see cref="Vector4"/> on the left of the mul sign.</param>
        /// <param name="b">Source <see cref="Vector4"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication.</returns>
        public static Vector4 operator *(in Vector4 a, in Vector4 b)
        {
            return a.Base * b.Base;
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector4 operator *(in Vector4 value, float scaleFactor)
        {
            return value.Base * scaleFactor;
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="scaleFactor">Scalar value on the left of the mul sign.</param>
        /// <param name="value">Source <see cref="Vector4"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector4 operator *(float scaleFactor, in Vector4 value)
        {
            return scaleFactor * value.Base;
        }

        #endregion

        #region Negate (operator -)

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains the specified vector inversion.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <returns>The result of the vector inversion.</returns>
        public static Vector4 Negate(in Vector4 value)
        {
            return FastVector4.Negate(value);
        }

        /// <summary>
        /// Inverts values in the specified <see cref="Vector4"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/> on the right of the sub sign.</param>
        /// <returns>Result of the inversion.</returns>
        public static Vector4 operator -(in Vector4 value)
        {
            return -value.Base;
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a normalized values from another vector.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <returns>Unit vector.</returns>
        public static Vector4 Normalize(in Vector4 value)
        {
            return FastVector4.Normalize(value);
        }

        /// <summary>
        /// Turns this <see cref="Vector4"/> to a unit vector with the same direction.
        /// </summary>
        public void Normalize()
        {
            this = Normalize(this);
        }

        #endregion

        #region Round

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains members from 
        /// another vector rounded to the nearest integer value.
        /// </summary>
        public static Vector4 Round(in Vector4 value)
        {
            return new Vector4(
                MathF.Round(value.X),
                MathF.Round(value.Y),
                MathF.Round(value.Z),
                MathF.Round(value.W));
        }

        /// <summary>
        /// Round the members of this <see cref="Vector4"/> towards the nearest integer value.
        /// </summary>
        public void Round()
        {
            this = Round(this);
        }

        #endregion

        #region SmoothStep

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains cubic interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector4"/>.</param>
        /// <param name="b">Source <see cref="Vector4"/>.</param>
        /// <param name="amount">Weighting value.</param>
        /// <returns>Cubic interpolation of the specified vectors.</returns>
        public static Vector4 SmoothStep(in Vector4 a, in Vector4 b, float amount)
        {
            return new Vector4(
                MathHelper.SmoothStep(a.X, b.X, amount),
                MathHelper.SmoothStep(a.Y, b.Y, amount),
                MathHelper.SmoothStep(a.Z, b.Z, amount),
                MathHelper.SmoothStep(a.W, b.W, amount));
        }

        #endregion

        #region Subtract (operator -)

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains subtraction of on <see cref="Vector4"/> from a another.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/>.</param>
        /// <param name="right">Source <see cref="Vector4"/>.</param>
        /// <returns>The result of the vector subtraction.</returns>
        public static Vector4 Subtract(in Vector4 left, in Vector4 right)
        {
            return FastVector4.Subtract(left, right);
        }

        /// <summary>
        /// Subtracts a <see cref="Vector4"/> from a <see cref="Vector4"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/> on the left of the sub sign.</param>
        /// <param name="right">Source <see cref="Vector4"/> on the right of the sub sign.</param>
        /// <returns>Result of the vector subtraction.</returns>
        public static Vector4 operator -(in Vector4 left, in Vector4 right)
        {
            return left.Base - right.Base;
        }

        #endregion

        #region Transform

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation 
        /// of 2D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector2"/>.</returns>
        public static Vector4 Transform(in Vector2 value, in Matrix matrix)
        {
            return FastVector4.Transform(value, matrix);
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation 
        /// of 2D-vector by the specified <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector2"/>.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <returns>Transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(in Vector2 value, in Quaternion rotation)
        {
            return FastVector4.Transform(value, rotation);
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation
        /// of 3D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(in Vector3 value, in Matrix matrix)
        {
            return FastVector4.Transform(value, matrix);
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation 
        /// of 3D-vector by the specified <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <returns>Transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(in Vector3 value, in Quaternion rotation)
        {
            return FastVector4.Transform(value, rotation);
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation 
        /// of 4D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(in Vector4 value, in Matrix matrix)
        {
            return FastVector4.Transform(value, matrix);
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a transformation 
        /// of 4D-vector by the specified <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <returns>Transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(in Vector4 value, in Quaternion rotation)
        {
            return FastVector4.Transform(value, rotation);
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

        #region ToString

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Vector4"/>.
        /// </summary>
        /// <returns>The string representation of the current instance.</returns>
        public override readonly string ToString()
        {
            return Base.ToString();
        }

        #endregion

        #region ToVector2

        /// <summary>
        /// Gets the <see cref="Vector2"/> representation of this vector.
        /// </summary>
        public readonly Vector2 ToVector2() => UnsafeUtils.As<Vector4, Vector2>(this);

        #endregion

        #region ToVector3

        /// <summary>
        /// Gets the <see cref="Vector3"/> representation of this vector.
        /// </summary>
        public readonly Vector3 ToVector3() => UnsafeUtils.As<Vector4, Vector3>(this);

        #endregion

        public static implicit operator FastVector4(in Vector4 value)
        {
            return value.Base;
        }

        public static implicit operator Vector4(in FastVector4 value)
        {
            return new Vector4 { Base = value };
        }
    }
}
