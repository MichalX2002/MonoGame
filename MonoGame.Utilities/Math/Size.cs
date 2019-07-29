using System;
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
    public struct Size : IEquatable<Size>
    {
        /// <summary>
        ///     Returns a <see cref="Size" /> with <see cref="Width" /> and <see cref="Height" /> equal to <c>0.0f</c>.
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
        public bool IsEmpty => (Width == 0) && (Height == 0);

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
        /// Gets a two-component <see cref="Point"/> representation for this object.
        /// </summary>
        /// <returns></returns>
        public Point ToPoint()
        {
            return new Point(Width, Height);
        }

        /// <summary>
        /// Gets a two-component <see cref="SizeF"/> representation for this object.
        /// </summary>
        /// <returns></returns>
        public SizeF ToSizeF()
        {
            return new SizeF(Width, Height);
        }

        /// <summary>
        /// Gets a two-component <see cref="Vector2"/> representation for this object.
        /// </summary>
        /// <returns></returns>
        public Vector2 ToVector2()
        {
            return new Vector2(Width, Height);
        }

        /// <summary>
        ///     Compares two <see cref="Size" /> structures. The result specifies
        ///     whether the values of the <see cref="Width" /> and <see cref="Height" />
        ///     fields of the two <see cref="Size" /> structures are equal.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Width" /> and <see cref="Height" />
        ///     fields of the two <see cref="Size" /> structures are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Size first, Size second)
        {
            return first.Equals(second);
        }
        
        /// <summary>
        ///     Indicates whether this <see cref="Size" /> is equal to another <see cref="Size" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Size" /> is equal to the <paramref name="size" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(Size size)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return (Width == size.Width) && (Height == size.Height);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="Size" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this  <see cref="Size" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Size size)
                return Equals(size);
            return false;
        }

        /// <summary>
        ///     Compares two <see cref="Size" /> structures. The result specifies
        ///     whether the values of the <see cref="Width" /> or <see cref="Height" />
        ///     fields of the two <see cref="Size" /> structures are unequal.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Width" /> or <see cref="Height" />
        ///     fields of the two <see cref="Size" /> structures are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Size first, Size second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Calculates the <see cref="Size" /> representing the vector addition of two <see cref="Size" /> structures as if
        ///     they
        ///     were <see cref="Vector2" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="Size" /> representing the vector addition of two <see cref="Size" /> structures as if they
        ///     were <see cref="Vector2" /> structures.
        /// </returns>
        public static Size operator +(Size first, Size second)
        {
            return Add(first, second);
        }

        /// <summary>
        ///     Calculates the <see cref="Size" /> representing the vector addition of two <see cref="Size" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="Size" /> representing the vector addition of two <see cref="Size" /> structures.
        /// </returns>
        public static Size Add(Size first, Size second)
        {
            return new Size
            {
                Width = first.Width + second.Width,
                Height = first.Height + second.Height
            };
        }

        /// <summary>
        /// Calculates the <see cref="Size" /> representing the vector subtraction of two <see cref="Size" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="Size" /> representing the vector subtraction of two <see cref="Size" /> structures.
        /// </returns>
        public static Size operator -(Size first, Size second)
        {
            return Subtract(first, second);
        }

        public static Size operator /(Size size, int value)
        {
            return new Size(size.Width / value, size.Height / value);
        }

        public static Size operator *(Size size, int value)
        {
            return new Size(size.Width * value, size.Height * value);
        }

        /// <summary>
        ///     Calculates the <see cref="Size" /> representing the vector subtraction of two <see cref="Size" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="Size" /> representing the vector subtraction of two <see cref="Size" /> structures.
        /// </returns>
        public static Size Subtract(Size first, Size second)
        {
            return new Size
            {
                Width = first.Width - second.Width,
                Height = first.Height - second.Height
            };
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="Size" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="Size" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 3 + Width;
                return hash * 7 + Height;
            }
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Point" /> to a <see cref="Size" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The resulting <see cref="Size" />.
        /// </returns>
        public static implicit operator Size(Point point)
        {
            return new Size(point.X, point.Y);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Size" /> to a <see cref="SizeF" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The resulting <see cref="Size" />.
        /// </returns>
        public static implicit operator SizeF(Size size)
        {
            return size.ToSizeF();
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="PointF" /> to a <see cref="Size" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The resulting <see cref="PointF" />.
        /// </returns>
        public static implicit operator PointF(Size size)
        {
            return new PointF(size.Width, size.Height);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Size" /> to a <see cref="Vector2" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The resulting <see cref="Vector2" />.
        /// </returns>
        public static implicit operator Vector2(Size size)
        {
            return new Vector2(size.Width, size.Height);
        }

        /// <summary>
        ///     Performs an explicit conversion from a <see cref="Size" /> to a <see cref="Point" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The resulting <see cref="Point" />.
        /// </returns>
        public static explicit operator Point(Size size)
        {
            return new Point(size.Width, size.Height);
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="Size" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="Size" />.
        /// </returns>
        public override string ToString()
        {
            return $"Width: {Width}, Height: {Height}";
        }

        internal string DebugDisplayString => ToString();
    }
}