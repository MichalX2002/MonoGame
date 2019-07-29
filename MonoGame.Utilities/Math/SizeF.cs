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
    public struct SizeF : IEquatable<SizeF>
    {
        /// <summary>
        ///     Returns a <see cref="SizeF" /> with <see cref="Width" /> and <see cref="Height" /> equal to <c>0.0f</c>.
        /// </summary>
        public static readonly SizeF Empty = new SizeF();

        /// <summary>
        ///     The horizontal component of this <see cref="SizeF" />.
        /// </summary>
        [DataMember] public float Width;

        /// <summary>
        ///     The vertical component of this <see cref="SizeF" />.
        /// </summary>
        [DataMember] public float Height;

        /// <summary>
        ///     Gets a value that indicates whether this <see cref="SizeF" /> is empty.
        /// </summary>
        // ReSharper disable CompareOfFloatsByEqualityOperator
        public bool IsEmpty => (Width == 0) && (Height == 0);

        // ReSharper restore CompareOfFloatsByEqualityOperator

        /// <summary>
        ///     Initializes a new instance of the <see cref="SizeF" /> structure from the specified dimensions.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public SizeF(float width, float height)
        {
            Width = width;
            Height = height;
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
        /// Gets a two-component <see cref="Size"/> representation for this object.
        /// </summary>
        /// <returns></returns>
        public Size ToSize()
        {
            return new Size((int)Width, (int)Height);
        }

        /// <summary>
        ///     Compares two <see cref="SizeF" /> structures. The result specifies
        ///     whether the values of the <see cref="Width" /> and <see cref="Height" />
        ///     fields of the two <see cref="PointF" /> structures are equal.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Width" /> and <see cref="Height" />
        ///     fields of the two <see cref="PointF" /> structures are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(SizeF first, SizeF second)
        {
            return first.Equals(second);
        }

        /// <summary>
        ///     Indicates whether this <see cref="SizeF" /> is equal to another <see cref="SizeF" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="PointF" /> is equal to the <paramref name="size" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(SizeF size)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return (Width == size.Width) && (Height == size.Height);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="SizeF" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this  <see cref="SizeF" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is SizeF size)
                return Equals(size);
            return false;
        }

        /// <summary>
        ///     Compares two <see cref="SizeF" /> structures. The result specifies
        ///     whether the values of the <see cref="Width" /> or <see cref="Height" />
        ///     fields of the two <see cref="SizeF" /> structures are unequal.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Width" /> or <see cref="Height" />
        ///     fields of the two <see cref="SizeF" /> structures are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(SizeF first, SizeF second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Calculates the <see cref="SizeF" /> representing the vector addition of two <see cref="SizeF" /> structures as if
        ///     they
        ///     were <see cref="Vector2" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="SizeF" /> representing the vector addition of two <see cref="SizeF" /> structures as if they
        ///     were <see cref="Vector2" /> structures.
        /// </returns>
        public static SizeF operator +(SizeF first, SizeF second)
        {
            return Add(first, second);
        }

        /// <summary>
        ///     Calculates the <see cref="SizeF" /> representing the vector addition of two <see cref="SizeF" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="SizeF" /> representing the vector addition of two <see cref="SizeF" /> structures.
        /// </returns>
        public static SizeF Add(SizeF first, SizeF second)
        {
            return new SizeF
            {
                Width = first.Width + second.Width,
                Height = first.Height + second.Height
            };
        }

        /// <summary>
        /// Calculates the <see cref="SizeF" /> representing the vector subtraction of two <see cref="SizeF" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="SizeF" /> representing the vector subtraction of two <see cref="SizeF" /> structures.
        /// </returns>
        public static SizeF operator -(SizeF first, SizeF second)
        {
            return Subtract(first, second);
        }

        public static SizeF operator /(SizeF size, float value)
        {
            return new SizeF(size.Width / value, size.Height / value);
        }

        public static SizeF operator *(SizeF size, float value)
        {
            return new SizeF(size.Width * value, size.Height * value);
        }

        /// <summary>
        ///     Calculates the <see cref="SizeF" /> representing the vector subtraction of two <see cref="SizeF" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="SizeF" /> representing the vector subtraction of two <see cref="SizeF" /> structures.
        /// </returns>
        public static SizeF Subtract(SizeF first, SizeF second)
        {
            return new SizeF
            {
                Width = first.Width - second.Width,
                Height = first.Height - second.Height
            };
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="SizeF" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="PointF" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 3 + Width.GetHashCode();
                return hash * 7 + Height.GetHashCode();
            }
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="PointF" /> to a <see cref="SizeF" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The resulting <see cref="SizeF" />.
        /// </returns>
        public static implicit operator SizeF(PointF point)
        {
            return new SizeF(point.X, point.Y);
        }


        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Point" /> to a <see cref="SizeF" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The resulting <see cref="SizeF" />.
        /// </returns>
        public static implicit operator SizeF(Point point)
        {
            return new SizeF(point.X, point.Y);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="PointF" /> to a <see cref="SizeF" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The resulting <see cref="PointF" />.
        /// </returns>
        public static implicit operator PointF(SizeF size)
        {
            return new PointF(size.Width, size.Height);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="SizeF" /> to a <see cref="Vector2" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The resulting <see cref="Vector2" />.
        /// </returns>
        public static implicit operator Vector2(SizeF size)
        {
            return new Vector2(size.Width, size.Height);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Vector2" /> to a <see cref="SizeF" />.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>
        ///     The resulting <see cref="SizeF" />.
        /// </returns>
        public static implicit operator SizeF(Vector2 vector)
        {
            return new SizeF(vector.X, vector.Y);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="SizeF" /> to a <see cref="Size" />.
        /// </summary>
        /// <param name="size">The vector.</param>
        /// <returns>
        ///     The resulting <see cref="SizeF" />.
        /// </returns>
        public static explicit operator Size(SizeF size)
        {
            return size.ToSize();
        }

        /// <summary>
        ///     Performs an explicit conversion from a <see cref="SizeF" /> to a <see cref="Point" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The resulting <see cref="Point" />.
        /// </returns>
        public static explicit operator Point(SizeF size)
        {
            return new Point((int)size.Width, (int)size.Height);
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="SizeF" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="SizeF" />.
        /// </returns>
        public override string ToString()
        {
            return $"Width: {Width}, Height: {Height}";
        }

        internal string DebugDisplayString => ToString();
    }
}