using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MonoGame.Framework
{
    /// <summary>
    ///     A two dimensional size defined by two real numbers, a width and a height.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A size is a subspace of two-dimensional space, the area of which is described in terms of a two-dimensional
    ///         coordinate system, given by a reference point and two coordinate axes.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IEquatable{T}" />
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Size : IEquatable<Size>
    {
        /// <summary>
        ///     Returns a <see cref="Size" /> with <see cref="Width" /> and <see cref="Height" /> equal to <c>0f</c>.
        /// </summary>
        public static readonly Size Empty = new Size();

        /// <summary>
        ///     The horizontal component of this <see cref="Size" />.
        /// </summary>
        [DataMember] public int Width;

        /// <summary>
        ///     The vertical component of this <see cref="Size" />.
        /// </summary>
        [DataMember] public int Height;

        /// <summary>
        ///     Gets a value that indicates whether this <see cref="Size" /> is empty.
        /// </summary>
        public bool IsEmpty => Width == 0 && Height == 0;

        private string DebuggerDisplay => $"Width = {Width}, Height = {Height}";

        /// <summary>
        ///     Initializes a new instance of the <see cref="Size" /> structure from the specified dimensions.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets a <see cref="Point"/> representation for this object.
        /// </summary>
        public Point ToPoint() => new Point(Width, Height);

        /// <summary>
        /// Gets a <see cref="SizeF"/> representation for this object.
        /// </summary>
        public SizeF ToSizeF() => new SizeF(Width, Height);

        /// <summary>
        /// Gets a <see cref="Vector2"/> representation for this object.
        /// </summary>
        public Vector2 ToVector2() => new Vector2(Width, Height);

        /// <summary>
        ///     Calculates the <see cref="Size" /> representing the vector addition of two <see cref="Size" /> structures as if
        ///     they were <see cref="Vector2" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="Size" /> representing the vector addition of two <see cref="Size" /> structures as if they
        ///     were <see cref="Vector2" /> structures.
        /// </returns>
        public static Size operator +(in Size first, in Size second) => Add(first, second);

        /// <summary>
        ///     Calculates the <see cref="Size" /> representing the vector addition of two <see cref="Size" /> structures.
        /// </summary>
        /// <param name="a">The first size.</param>
        /// <param name="b">The second size.</param>
        /// <returns>
        ///     The <see cref="Size" /> representing the vector addition of two <see cref="Size" /> structures.
        /// </returns>
        public static Size Add(in Size a, in Size b) => new Size(a.Width + b.Width, a.Height + b.Height);

        /// <summary>
        /// Calculates the <see cref="Size" /> representing the vector subtraction of two <see cref="Size" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="Size" /> representing the vector subtraction of two <see cref="Size" /> structures.
        /// </returns>
        public static Size operator -(in Size first, in Size second) => Subtract(first, second);

        public static Size operator /(in Size size, int value) => new Size(size.Width / value, size.Height / value);

        public static Size operator *(in Size size, int value) => new Size(size.Width * value, size.Height * value);

        /// <summary>
        ///     Calculates the <see cref="Size" /> representing the vector subtraction of two <see cref="Size" /> structures.
        /// </summary>
        /// <param name="left">The first size.</param>
        /// <param name="right">The second size.</param>
        /// <returns>
        ///     The <see cref="Size" /> representing the vector subtraction of two <see cref="Size" /> structures.
        /// </returns>
        public static Size Subtract(in Size left, in Size right) =>
            new Size(left.Width - right.Width, left.Height - right.Height);

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Point" /> to a <see cref="Size" />.
        /// </summary>
        public static implicit operator Size(in Point point) => new Size(point.X, point.Y);

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Size" /> to a <see cref="SizeF" />.
        /// </summary>
        public static implicit operator SizeF(in Size size) => size.ToSizeF();

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="PointF" /> to a <see cref="Size" />.
        /// </summary>
        public static implicit operator PointF(in Size size) => new PointF(size.Width, size.Height);

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Size" /> to a <see cref="Vector2" />.
        /// </summary>
        public static implicit operator Vector2(in Size size) => new Vector2(size.Width, size.Height);

        /// <summary>
        ///     Performs an explicit conversion from a <see cref="Size" /> to a <see cref="Point" />.
        /// </summary>
        public static explicit operator Point(Size size) => new Point(size.Width, size.Height);

        /// <summary>
        ///     Compares two <see cref="Size" /> structures. The result specifies
        ///     whether the values of the <see cref="Width" /> and <see cref="Height" />
        ///     fields of the two <see cref="Size" /> structures are equal.
        /// </summary>
        public static bool operator ==(in Size a, in Size b) => (a.Width == b.Width) && (a.Height == b.Height);

        /// <summary>
        ///     Compares two <see cref="Size" /> structures. The result specifies
        ///     whether the values of the <see cref="Width" /> or <see cref="Height" />
        ///     fields of the two <see cref="Size" /> structures are unequal.
        /// </summary>
        public static bool operator !=(in Size a, in Size b) => !(a == b);

        /// <summary>
        ///     Indicates whether this <see cref="Size" /> is equal to another <see cref="Size" />.
        /// </summary>
        public bool Equals(Size other) => this == other;

        /// <summary>
        ///     Returns a value indicating whether this <see cref="Size" /> is equal to a specified object.
        /// </summary>
        public override bool Equals(object obj) => obj is Size size && Equals(size);

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="Size" />.
        /// </summary>
        public override string ToString() => $"Width: {Width}, Height: {Height}";

        /// <summary>
        ///     Returns a hash code of this <see cref="Size" />.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 7 + Width;
                return hash * 31 + Height;
            }
        }
    }
}