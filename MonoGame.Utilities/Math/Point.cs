// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Serialization;

namespace MonoGame.Framework
{
    /// <summary>
    /// Describes a 2D-point.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Point : IEquatable<Point>
    {
        /// <summary>
        /// <see cref="Point"/> with the coordinates (0, 0).
        /// </summary>
        public static Point Zero => default;

        #region Public Fields

        /// <summary>
        /// The x coordinate of this <see cref="Point"/>.
        /// </summary>
        [DataMember]
        public int X;

        /// <summary>
        /// The y coordinate of this <see cref="Point"/>.
        /// </summary>
        [DataMember]
        public int Y;

        #endregion

        #region Properties

        internal string DebuggerDisplay => string.Concat(
            X.ToString(), "  ",
            Y.ToString(), "  ");

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a point with X and Y from two values.
        /// </summary>
        /// <param name="x">The x coordinate in 2d-space.</param>
        /// <param name="y">The y coordinate in 2d-space.</param>
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs a point with X and Y set to the same value.
        /// </summary>
        /// <param name="value">The x and y coordinates in 2d-space.</param>
        public Point(int value)
        {
            X = value;
            Y = value;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Adds two points.
        /// </summary>
        /// <param name="value1">Source <see cref="Point"/> on the left of the add sign.</param>
        /// <param name="value2">Source <see cref="Point"/> on the right of the add sign.</param>
        /// <returns>Sum of the points.</returns>
        public static Point operator +(Point value1, Point value2) =>
            new Point(value1.X + value2.X, value1.Y + value2.Y);

        /// <summary>
        /// Subtracts a <see cref="Point"/> from a <see cref="Point"/>.
        /// </summary>
        /// <param name="value1">Source <see cref="Point"/> on the left of the sub sign.</param>
        /// <param name="value2">Source <see cref="Point"/> on the right of the sub sign.</param>
        /// <returns>Result of the subtraction.</returns>
        public static Point operator -(Point value1, Point value2) =>
            new Point(value1.X - value2.X, value1.Y - value2.Y);

        /// <summary>
        /// Multiplies the components of two points by each other.
        /// </summary>
        /// <param name="value1">Source <see cref="Point"/> on the left of the mul sign.</param>
        /// <param name="value2">Source <see cref="Point"/> on the right of the mul sign.</param>
        /// <returns>Result of the multiplication.</returns>
        public static Point operator *(Point value1, Point value2) =>
            new Point(value1.X * value2.X, value1.Y * value2.Y);

        /// <summary>
        /// Divides the components of a <see cref="Point"/> by the components of another <see cref="Point"/>.
        /// </summary>
        /// <param name="source">Source <see cref="Point"/> on the left of the div sign.</param>
        /// <param name="divisor">Divisor <see cref="Point"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the points.</returns>
        public static Point operator /(Point source, Point divisor)
        {
            return new Point(source.X / divisor.X, source.Y / divisor.Y);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets a <see cref="Vector2"/> representation of this object.
        /// </summary>
        public readonly Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        /// <summary>
        /// Gets a <see cref="PointF"/> representation of this object.
        /// </summary>
        public readonly PointF ToPointF()
        {
            return new PointF(X, Y);
        }

        /// <summary>
        /// Gets a <see cref="SizeF"/> representation of this object.
        /// </summary>
        public readonly SizeF ToSizeF()
        {
            return new SizeF(X, Y);
        }

        /// <summary>
        /// Gets a <see cref="Size"/> representation of this object.
        /// </summary>
        public readonly Size ToSize()
        {
            return UnsafeR.As<Point, Size>(this);
        }

        /// <summary>
        /// Deconstruction method for <see cref="Point"/>.
        /// </summary>
        public readonly void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        #endregion

        #region Equals

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Point"/>.
        /// </summary>
        /// <param name="other">The <see cref="Point"/> to compare.</param>
        public readonly bool Equals(Point other) => this == other;

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        public override readonly bool Equals(object obj) => obj is Point other && Equals(other);

        /// <summary>
        /// Compares whether two <see cref="Point"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Point"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Point"/> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(Point a, Point b)
        {
            return (a.X == b.X) && (a.Y == b.Y);
        }

        /// <summary>
        /// Compares whether two <see cref="Point"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Point"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Point"/> instance on the right of the not equal sign.</param>
        /// <returns><see langword="true"/> if the instances are not equal; <see langword="false"/> otherwise.</returns>	
        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Returns a hash code of this <see cref="Point"/>.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(X, Y);

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Point"/>.
        /// </summary>
        public override string ToString() => $"{{X:{X}, Y:{Y}}}";

        #endregion


        #region Explicit operators

        public static explicit operator Point(in Vector2 vector)
        {
            return vector.ToPoint();
        }

        public static explicit operator Point(in PointF point)
        {
            return point.ToPoint();
        }

        public static explicit operator Point(in SizeF size)
        {
            return size.ToPoint();
        }

        #endregion

        #region Implicit Operators

        public static implicit operator Vector2(in Point size)
        {
            return size.ToVector2();
        }

        public static implicit operator Point(in Size point)
        {
            return point.ToPoint();
        }

        public static implicit operator PointF(in Point size)
        {
            return size.ToPointF();
        }

        public static implicit operator Size(in Point point)
        {
            return point.ToSize();
        }

        public static implicit operator SizeF(in Point size)
        {
            return size.ToSizeF();
        }

        #endregion
    }
}


