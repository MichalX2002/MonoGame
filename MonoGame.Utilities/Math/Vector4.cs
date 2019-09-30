// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.Serialization;
using System.Diagnostics;
using MonoGame.Utilities;
using System.Diagnostics.CodeAnalysis;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Framework
{
    /// <summary>
    /// Describes a 4D-vector.
    /// </summary>
#if XNADESIGNPROVIDED
    [System.ComponentModel.TypeConverter(typeof(Design.Vector4TypeConverter))]
#endif
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct Vector4 : IEquatable<Vector4>, IPixel
    {
        #region Public Constants

        /// <summary>
        /// Returns a <see cref="Vector4"/> with components 0, 0, 0, 0.
        /// </summary>
        public static readonly Vector4 Zero = new Vector4(0, 0, 0, 0);

        /// <summary>
        /// Returns a <see cref="Vector4"/> with components 1, 1, 1, 1.
        /// </summary>
        public static readonly Vector4 One = new Vector4(1f, 1f, 1f, 1f);

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

        #region Public Fields

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

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the x and y coordinates as a (x,y) <see cref="Vector2"/>.
        /// </summary>
        [IgnoreDataMember]
        public Vector2 XY { get => new Vector2(X, Y); set { X = value.X; Y = value.Y; } }

        /// <summary>
        /// Gets or sets the z and w coordinates as a (z,w) <see cref="Vector2"/>.
        /// </summary>
        [IgnoreDataMember]
        public Vector2 ZW { get => new Vector2(Z, W); set { Z = value.X; W = value.Y; } }

        /// <summary>
        /// Gets or sets the z and y coordinates as a (z,y) <see cref="Vector2"/>.
        /// </summary>
        [IgnoreDataMember]
        public Vector2 ZY { get => new Vector2(Z, Y); set { Z = value.X; Y = value.Y; } }

        /// <summary>
        /// Gets or sets the x and w coordinates as a (x,w) <see cref="Vector2"/>.
        /// </summary>
        [IgnoreDataMember]
        public Vector2 XW { get => new Vector2(X, W); set { X = value.X; W = value.Y; } }

        #endregion

        internal string DebugDisplayString
        {
            get => string.Concat(
                    X.ToString(), "  ",
                    Y.ToString(), "  ",
                    Z.ToString(), "  ",
                    W.ToString());
        }

        /// <summary>
        /// Constructs a 3D vector with X, Y, Z and W from four values.
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
        /// Constructs a 3D vector with X and Z from <see cref="Vector2"/> and Z and W from the scalars.
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
        /// Constructs a 3D vector with X, Y, Z from <see cref="Vector3"/> and W from a scalar.
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
        /// Constructs a 4D vector with X, Y, Z and W set to the same value.
        /// </summary>
        /// <param name="value">The x, y, z and w coordinates in 4D-space.</param>
        public Vector4(float value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }

        /// <summary>
        /// Gets the <see cref="Vector3"/> representation of this vector.
        /// </summary>
        public Vector3 ToVector3() => new Vector3(X, Y, Z);

        /// <inheritdoc/>
        void IPackedVector.FromVector4(Vector4 vector) => this = Clamp(vector, Zero, One);

        /// <inheritdoc/>
        Vector4 IPackedVector.ToVector4() => this;

        /// <inheritdoc/>
        public void FromArgb32(Argb32 source) => this = source.ToVector4();

        /// <inheritdoc />
        public void FromBgr24(Bgr24 source) => this = source.ToVector4();

        /// <inheritdoc/>
        public void FromBgra32(Bgra32 source) => this = source.ToVector4();

        /// <inheritdoc/>
        public void FromBgra5551(Bgra5551 source) => this = source.ToVector4();

        /// <inheritdoc />
        public void FromGray8(Gray8 source) => this = source.ToVector4();

        /// <inheritdoc />
        public void FromGray16(Gray16 source) => this = source.ToVector4();

        /// <inheritdoc />
        public void FromRgb24(Rgb24 source) => this = source.ToVector4();

        /// <inheritdoc/>
        public void FromColor(Color source) => this = source.ToVector4();

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source) => this = source.ToVector4();

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source) => this = source.ToVector4();

        /// <inheritdoc />
        public void ToColor(ref Color dest) => dest.FromVector4(this);

        /// <summary>
        /// Performs vector addition on <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first vector to add.</param>
        /// <param name="b">The second vector to add.</param>
        /// <returns>The result of the vector addition.</returns>
        public static Vector4 Add(in Vector4 a, in Vector4 b) => a + b;

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains the cartesian coordinates of a vector specified in barycentric coordinates and relative to 4D-triangle.
        /// </summary>
        /// <param name="a">The first vector of 4D-triangle.</param>
        /// <param name="b">The second vector of 4D-triangle.</param>
        /// <param name="c">The third vector of 4D-triangle.</param>
        /// <param name="amount1">Barycentric scalar <c>b2</c> which represents a weighting factor towards second vector of 4D-triangle.</param>
        /// <param name="amount2">Barycentric scalar <c>b3</c> which represents a weighting factor towards third vector of 4D-triangle.</param>
        /// <returns>The cartesian translation of barycentric coordinates.</returns>
        public static Vector4 Barycentric(in Vector4 a, in Vector4 b, in Vector4 c, float amount1, float amount2)
        {
            return new Vector4(
                MathHelper.Barycentric(a.X, b.X, c.X, amount1, amount2),
                MathHelper.Barycentric(a.Y, b.Y, c.Y, amount1, amount2),
                MathHelper.Barycentric(a.Z, b.Z, c.Z, amount1, amount2),
                MathHelper.Barycentric(a.W, b.W, c.W, amount1, amount2));
        }

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

        /// <summary>
        /// Clamps the specified value within a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The clamped value.</returns>
        public static Vector4 Clamp(in Vector4 value, in Vector4 min, in Vector4 max)
        {
            return new Vector4(
                MathHelper.Clamp(value.X, min.X, max.X),
                MathHelper.Clamp(value.Y, min.Y, max.Y),
                MathHelper.Clamp(value.Z, min.Z, max.Z),
                MathHelper.Clamp(value.W, min.W, max.W));
        }

        /// <summary>
        /// Returns the distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The distance between two vectors.</returns>
        public static float Distance(in Vector4 a, in Vector4 b)
        {
            return (float)Math.Sqrt(DistanceSquared(a, b));
        }

        /// <summary>
        /// Returns the squared distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The squared distance between two vectors.</returns>
        public static float DistanceSquared(in Vector4 a, in Vector4 b)
        {
              return (a.X - b.X) * (a.X - b.X) +
                     (a.Y - b.Y) * (a.Y - b.Y) +
                     (a.Z - b.Z) * (a.Z - b.Z) +
                     (a.W - b.W) * (a.W - b.W);
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by the components of another <see cref="Vector4"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/>.</param>
        /// <param name="right">Divisor <see cref="Vector4"/>.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector4 Divide(in Vector4 left, in Vector4 right) => left / right;

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="divider">Divisor scalar.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector4 Divide(in Vector4 value, float divider) => value / divider;

        /// <summary>
        /// Returns a dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product of two vectors.</returns>
        public static float Dot(in Vector4 a, in Vector4 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj) => obj is Vector4 other ? this == other : false;

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Vector4"/>.
        /// </summary>
        /// <param name="other">The <see cref="Vector4"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public bool Equals(Vector4 other) => this == other;

        /// <summary>
        /// Gets the hash code of this <see cref="Vector4"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="Vector4"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int code = X.GetHashCode();
                code = code * 23 + Y.GetHashCode();
                code = code * 23 + Z.GetHashCode();
                return code * 23 + W.GetHashCode();
            }
        }

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
        /// <returns>The length of this <see cref="Vector4"/>.</returns>
        public float Length() => (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));

        /// <summary>
        /// Returns the squared length of this <see cref="Vector4"/>.
        /// </summary>
        /// <returns>The squared length of this <see cref="Vector4"/>.</returns>
        public float LengthSquared() => (X * X) + (Y * Y) + (Z * Z) + (W * W);

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains linear interpolation of the specified vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static Vector4 Lerp(in Vector4 a, in Vector4 b, float amount)
        {
            return new Vector4(
                MathHelper.Lerp(a.X, b.X, amount),
                MathHelper.Lerp(a.Y, b.Y, amount),
                MathHelper.Lerp(a.Z, b.Z, amount),
                MathHelper.Lerp(a.W, b.W, amount));
        }

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
        public static Vector4 LerpPrecise(in Vector4 a, in Vector4 b, float amount)
        {
            return new Vector4(
                MathHelper.LerpPrecise(a.X, b.X, amount),
                MathHelper.LerpPrecise(a.Y, b.Y, amount),
                MathHelper.LerpPrecise(a.Z, b.Z, amount),
                MathHelper.LerpPrecise(a.W, b.W, amount));
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a maximal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector4"/> with maximal values from the two vectors.</returns>
        public static Vector4 Max(in Vector4 a, in Vector4 b)
        {
            return new Vector4(
               MathHelper.Max(a.X, b.X),
               MathHelper.Max(a.Y, b.Y),
               MathHelper.Max(a.Z, b.Z),
               MathHelper.Max(a.W, b.W));
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a minimal values from the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The <see cref="Vector4"/> with minimal values from the two vectors.</returns>
        public static Vector4 Min(in Vector4 a, in Vector4 b)
        {
            return new Vector4(
               MathHelper.Min(a.X, b.X),
               MathHelper.Min(a.Y, b.Y),
               MathHelper.Min(a.Z, b.Z),
               MathHelper.Min(a.W, b.W));
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a multiplication of two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector4"/>.</param>
        /// <param name="b">Source <see cref="Vector4"/>.</param>
        /// <returns>The result of the vector multiplication.</returns>
        public static Vector4 Multiply(in Vector4 a, in Vector4 b) => a * b;

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a multiplication of <see cref="Vector4"/> and a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>The result of the vector multiplication with a scalar.</returns>
        public static Vector4 Multiply(in Vector4 value, float scaleFactor) => value * scaleFactor;

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains the specified vector inversion.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <returns>The result of the vector inversion.</returns>
        public static Vector4 Negate(in Vector4 value) => -value;

        /// <summary>
        /// Turns this <see cref="Vector4"/> to a unit vector with the same direction.
        /// </summary>
        public void Normalize()
        {
            float factor = (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
            factor = 1f / factor;
            X *= factor;
            Y *= factor;
            Z *= factor;
            W *= factor;
        }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains a normalized values from another vector.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <returns>Unit vector.</returns>
        public static Vector4 Normalize(in Vector4 value)
        {
            float factor = (float)Math.Sqrt((value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z) + (value.W * value.W));
            factor = 1f / factor;
            return new Vector4(value.X * factor, value.Y * factor, value.Z * factor, value.W * factor);
        }

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

        /// <summary>
        /// Creates a new <see cref="Vector4"/> that contains subtraction of on <see cref="Vector4"/> from a another.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/>.</param>
        /// <param name="right">Source <see cref="Vector4"/>.</param>
        /// <returns>The result of the vector subtraction.</returns>
        public static Vector4 Subtract(in Vector4 left, in Vector4 right) => left - right;

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
        /// Creates a new <see cref="Vector4"/> that contains a transformation of 3D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector3"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(in Vector3 value, in Matrix matrix)
        {
            return new Vector4(
                (value.X * matrix.M11) + (value.Y * matrix.M21) + (value.Z * matrix.M31) + matrix.M41,
                (value.X * matrix.M12) + (value.Y * matrix.M22) + (value.Z * matrix.M32) + matrix.M42,
                (value.X * matrix.M13) + (value.Y * matrix.M23) + (value.Z * matrix.M33) + matrix.M43,
                (value.X * matrix.M14) + (value.Y * matrix.M24) + (value.Z * matrix.M34) + matrix.M44);
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
        /// Creates a new <see cref="Vector4"/> that contains a transformation of 4D-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(in Vector4 value, in Matrix matrix)
        {
            return new Vector4(
                (value.X * matrix.M11) + (value.Y * matrix.M21) + (value.Z * matrix.M31) + (value.W * matrix.M41),
                (value.X * matrix.M12) + (value.Y * matrix.M22) + (value.Z * matrix.M32) + (value.W * matrix.M42),
                (value.X * matrix.M13) + (value.Y * matrix.M23) + (value.Z * matrix.M33) + (value.W * matrix.M43),
                (value.X * matrix.M14) + (value.Y * matrix.M24) + (value.Z * matrix.M34) + (value.W * matrix.M44));
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
            CommonArgumentGuard.CheckSrcDstSpans(source, destination);

            for (var i = 0; i < source.Length; i++)
                destination[i] = Transform(source[i], matrix);
        }

        /// <summary>
        /// Apply transformation on all vectors within span of <see cref="Vector4"/> by the specified <see cref="Quaternion"/> and places the results in an another array.
        /// </summary>
        /// <param name="sourceArray">Source span.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
        /// <param name="destinationArray">Destination span.</param>
        public static void Transform(
            ReadOnlySpan<Vector4> source, in Quaternion rotation, Span<Vector4> destination)
        {
            CommonArgumentGuard.CheckSrcDstSpans(source, destination);
            for (var i = 0; i < source.Length; i++)
                destination[i] = Transform(source[i], rotation);
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Vector4"/> in the format:
        /// {X:{X} Y:{Y} Z:{Z} W:{W}}
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Vector4"/>.</returns>
        public override string ToString()
        {
            return "{X:" + X + " Y:" + Y + " Z:" + Z + " W:" + W + "}";
        }

        /// <summary>
        /// Deconstruction method for <see cref="Vector4"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public void Deconstruct(out float x, out float y, out float z, out float w)
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
        public static Vector4 operator -(in Vector4 value)
        {
            return new Vector4(-value.X, -value.Y, -value.Z, -value.W);
        }

        /// <summary>
        /// Compares whether two <see cref="Vector4"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Vector4"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Vector4"/> instance on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(in Vector4 a, in Vector4 b)
        {
            return a.X == b.X
                && a.Y == b.Y
                && a.Z == b.Z
                && a.W == b.W;
        }

        /// <summary>
        /// Compares whether two <see cref="Vector4"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Vector4"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Vector4"/> instance on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>	
        public static bool operator !=(in Vector4 a, in Vector4 b) => !(a == b);

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Source <see cref="Vector4"/> on the left of the add sign.</param>
        /// <param name="b">Source <see cref="Vector4"/> on the right of the add sign.</param>
        /// <returns>Sum of the vectors.</returns>
        public static Vector4 operator +(in Vector4 a, in Vector4 b) =>
            new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

        /// <summary>
        /// Subtracts a <see cref="Vector4"/> from a <see cref="Vector4"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/> on the left of the sub sign.</param>
        /// <param name="right">Source <see cref="Vector4"/> on the right of the sub sign.</param>
        /// <returns>Result of the vector subtraction.</returns>
        public static Vector4 operator -(in Vector4 left, in Vector4 right) =>
            new Vector4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="a">Source <see cref="Vector4"/> on the left of the mul sign.</param>
        /// <param name="b">Source <see cref="Vector4"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication.</returns>
        public static Vector4 operator *(in Vector4 a, in Vector4 b) =>
            new Vector4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector4 operator *(in Vector4 value, float scaleFactor)
        {
            return new Vector4(
                value.X * scaleFactor,
                value.Y * scaleFactor,
                value.Z * scaleFactor,
                value.W * scaleFactor);
        }

        /// <summary>
        /// Multiplies the components of vector by a scalar.
        /// </summary>
        /// <param name="scaleFactor">Scalar value on the left of the mul sign.</param>
        /// <param name="value">Source <see cref="Vector4"/> on the right of the mul sign.</param>
        /// <returns>Result of the vector multiplication with a scalar.</returns>
        public static Vector4 operator *(float scaleFactor, in Vector4 value)
        {
            return new Vector4(
                value.W * scaleFactor,
                value.X * scaleFactor,
                value.Y * scaleFactor,
                value.Z * scaleFactor);
        }

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by the components of another <see cref="Vector4"/>.
        /// </summary>
        /// <param name="left">Source <see cref="Vector4"/> on the left of the div sign.</param>
        /// <param name="right">Divisor <see cref="Vector4"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the vectors.</returns>
        public static Vector4 operator /(in Vector4 left, in Vector4 right) =>
            new Vector4(left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W);

        /// <summary>
        /// Divides the components of a <see cref="Vector4"/> by a scalar.
        /// </summary>
        /// <param name="value">Source <see cref="Vector4"/> on the left of the div sign.</param>
        /// <param name="divider">Divisor scalar on the right of the div sign.</param>
        /// <returns>The result of dividing a vector by a scalar.</returns>
        public static Vector4 operator /(in Vector4 value, float divider)
        {
            float factor = 1f / divider;
            return new Vector4(value.X * factor, value.Y * factor, value.Z * factor, value.W * factor);
        }
    }
}
