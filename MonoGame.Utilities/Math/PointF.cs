using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Xna.Framework
{
    // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 3.2; A Math and Geometry Primer - Coordinate Systems and Points. pg 35

    /// <summary>
    ///     A two-dimensional point defined by a 2-tuple of real numbers, (x, y).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A point is a position in two-dimensional space, the location of which is described in terms of a
    ///         two-dimensional coordinate system, given by a reference point, called the origin, and two coordinate axes.
    ///     </para>
    ///     <para>
    ///         A common two-dimensional coordinate system is the Cartesian (or rectangular) coordinate system where the
    ///         coordinate axes, conventionally denoted the X axis and Y axis, are perpindicular to each other. For the
    ///         three-dimensional rectangular coordinate system, the third axis is called the Z axis.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IEquatable{T}" />
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    [DataContract]
    public struct PointF : IEquatable<PointF>
    {
        /// <summary>
        ///     Returns a <see cref="PointF" /> with <see cref="X" /> and <see cref="Y" /> equal to <c>0.0f</c>.
        /// </summary>
        public static readonly PointF Zero = new PointF();

        /// <summary>
        ///     Returns a <see cref="PointF" /> with <see cref="X" /> and <see cref="Y" /> set to not a number.
        /// </summary>
        public static readonly PointF NaN = new PointF(float.NaN, float.NaN);

        /// <summary>
        ///     The x-coordinate of this <see cref="PointF" />.
        /// </summary>
        [DataMember] public float X;

        /// <summary>
        ///     The y-coordinate of this <see cref="PointF" />.
        /// </summary>
        [DataMember] public float Y;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PointF" /> structure from the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }
        
        /// <summary>
        /// Returns the <see cref="Vector2"/> representation of this instance.
        /// </summary>
        /// <returns><see cref="Vector2"/></returns>
        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        /// <summary>
        ///     Compares two <see cref="PointF" /> structures. The result specifies
        ///     whether the values of the <see cref="X" /> and <see cref="Y" />
        ///     fields of the two <see cref="PointF" />
        ///     structures are equal.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="X" /> and <see cref="Y" />
        ///     fields of the two <see cref="PointF" />
        ///     structures are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(PointF first, PointF second)
        {
            return first.Equals(second);
        }

        /// <summary>
        ///     Indicates whether this <see cref="PointF" /> is equal to another <see cref="PointF" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="PointF" /> is equal to the <paramref name="point" /> parameter; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(PointF point)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return (point.X == X) && (point.Y == Y);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="PointF" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this  <see cref="PointF" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is PointF point)
                return Equals(point);
            return false;
        }

        /// <summary>
        ///     Compares two <see cref="PointF" /> structures. The result specifies
        ///     whether the values of the <see cref="X" /> or <see cref="Y" />
        ///     fields of the two <see cref="PointF" />
        ///     structures are unequal.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="X" /> or <see cref="Y" />
        ///     fields of the two <see cref="PointF" />
        ///     structures are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(PointF first, PointF second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Calculates the <see cref="PointF" /> representing the addition of a <see cref="PointF" /> and a
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="vector">The vector.</param>
        /// <returns>
        ///     The <see cref="PointF" /> representing the addition of a <see cref="PointF" /> and a <see cref="Vector2" />.
        /// </returns>
        public static PointF operator +(PointF point, Vector2 vector)
        {
            return Add(point, vector);
        }

        /// <summary>
        ///     Calculates the <see cref="PointF" /> representing the addition of a <see cref="PointF" /> and a
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="vector">The vector.</param>
        /// <returns>
        ///     The <see cref="PointF" /> representing the addition of a <see cref="PointF" /> and a <see cref="Vector2" />.
        /// </returns>
        public static PointF Add(PointF point, Vector2 vector)
        {
            return new PointF
            {
                X = point.X + vector.X,
                Y = point.Y + vector.Y
            };
        }

        /// <summary>
        ///     Calculates the <see cref="PointF" /> representing the subtraction of a <see cref="PointF" /> and a
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="vector">The vector.</param>
        /// <returns>
        ///     The <see cref="PointF" /> representing the substraction of a <see cref="PointF" /> and a <see cref="Vector2" />.
        /// </returns>
        public static PointF operator -(PointF point, Vector2 vector)
        {
            return Subtract(point, vector);
        }

        /// <summary>
        ///     Calculates the <see cref="PointF" /> representing the addition of a <see cref="PointF" /> and a
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="vector">The vector.</param>
        /// <returns>
        ///     The <see cref="PointF" /> representing the substraction of a <see cref="PointF" /> and a <see cref="Vector2" />.
        /// </returns>
        public static PointF Subtract(PointF point, Vector2 vector)
        {
            return new PointF
            {
                X = point.X - vector.X,
                Y = point.Y - vector.Y
            };
        }

        /// <summary>
        ///     Calculates the <see cref="Vector2" /> representing the displacement of two <see cref="PointF" /> structures.
        /// </summary>
        /// <param name="point2">The second point.</param>
        /// <param name="point1">The first point.</param>
        /// <returns>
        ///     The <see cref="Vector2" /> representing the displacement of two <see cref="PointF" /> structures.
        /// </returns>
        public static Vector2 operator -(PointF point1, PointF point2)
        {
            return Displacement(point1, point2);
        }

        /// <summary>
        ///     Calculates the <see cref="Vector2" /> representing the displacement of two <see cref="PointF" /> structures.
        /// </summary>
        /// <param name="point2">The second point.</param>
        /// <param name="point1">The first point.</param>
        /// <returns>
        ///     The <see cref="Vector2" /> representing the displacement of two <see cref="PointF" /> structures.
        /// </returns>
        public static Vector2 Displacement(PointF point2, PointF point1)
        {
            return new Vector2
            {
                X = point2.X - point1.X,
                Y = point2.Y - point1.Y
            };
        }

        /// <summary>
        ///     Translates a <see cref='PointF' /> by a given <see cref='SizeF' />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static PointF operator +(PointF point, SizeF size)
        {
            return Add(point, size);
        }

        /// <summary>
        ///     Translates a <see cref='PointF' /> by a given <see cref='SizeF' />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static PointF Add(PointF point, SizeF size)
        {
            return new PointF(point.X + size.Width, point.Y + size.Height);
        }

        /// <summary>
        ///     Translates a <see cref='PointF' /> by the negative of a given <see cref='SizeF' />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static PointF operator -(PointF point, SizeF size)
        {
            return Subtract(point, size);
        }

        /// <summary>
        ///     Translates a <see cref='PointF' /> by the negative of a given <see cref='SizeF' /> .
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static PointF Subtract(PointF point, SizeF size)
        {
            return new PointF(point.X - size.Width, point.Y - size.Height);
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="PointF" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="PointF" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 3 + X.GetHashCode();
                return hash * 7 + Y.GetHashCode();
            }
        }

        /// <summary>
        ///     Calculates the <see cref="PointF" /> that contains the minimal coordinate values from two <see cref="PointF" />
        ///     structures.
        ///     structures.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>
        ///     The the <see cref="PointF" /> that contains the minimal coordinate values from two <see cref="PointF" />
        ///     structures.
        /// </returns>
        public static PointF Minimum(PointF first, PointF second)
        {
            return new PointF(
                first.X < second.X ? first.X : second.X,
                first.Y < second.Y ? first.Y : second.Y);
        }

        /// <summary>
        ///     Calculates the <see cref="PointF" /> that contains the maximal coordinate values from two <see cref="PointF" />
        ///     structures.
        ///     structures.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>
        ///     The the <see cref="PointF" /> that contains the maximal coordinate values from two <see cref="PointF" />
        ///     structures.
        /// </returns>
        public static PointF Maximum(PointF first, PointF second)
        {
            return new PointF(
                first.X > second.X ? first.X : second.X, 
                first.Y > second.Y ? first.Y : second.Y);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="PointF" /> to a position <see cref="Vector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The resulting <see cref="Vector2" />.
        /// </returns>
        public static implicit operator Vector2(PointF point)
        {
            return new Vector2(point.X, point.Y);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Vector2" /> to a position <see cref="PointF" />.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>
        ///     The resulting <see cref="PointF" />.
        /// </returns>
        public static implicit operator PointF(Vector2 vector)
        {
            return new PointF(vector.X, vector.Y);
        }


        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Point" /> to a <see cref="PointF" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The resulting <see cref="PointF" />.
        /// </returns>
        public static implicit operator PointF(Point point)
        {
            return new PointF(point.X, point.Y);
        }


        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="PointF" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="PointF" />.
        /// </returns>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        internal string DebugDisplayString => ToString();
    }
}