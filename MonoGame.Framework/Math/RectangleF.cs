using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Xna.Framework
{
    // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 77 

    /// <summary>
    ///     An axis-aligned, four sided, two dimensional box defined by a top-left position (<see cref="X" /> and
    ///     <see cref="Y" />) and a size (<see cref="Width" /> and <see cref="Height" />).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         An <see cref="RectangleF" /> is categorized by having its faces oriented in such a way that its
    ///         face normals are at all times parallel with the axes of the given coordinate system.
    ///     </para>
    ///     <para>
    ///         The bounding <see cref="RectangleF" /> of a rotated <see cref="RectangleF" /> will be equivalent or larger
    ///         in size than the original depending on the angle of rotation.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IEquatable{T}" />
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct RectangleF : IEquatable<RectangleF>, IShapeF
    {
        /// <summary>
        ///     The <see cref="RectangleF" /> with <see cref="X" />, <see cref="Y" />, <see cref="Width" /> and
        ///     <see cref="Height" /> all set to <code>0.0f</code>.
        /// </summary>
        public static readonly RectangleF Empty = new RectangleF();

        /// <summary>
        ///     The x-coordinate of the top-left corner position of this <see cref="RectangleF" />.
        /// </summary>
        [DataMember] public float X;

        /// <summary>
        ///     The y-coordinate of the top-left corner position of this <see cref="RectangleF" />.
        /// </summary>
        [DataMember] public float Y;

        /// <summary>
        ///     The width of this <see cref="RectangleF" />.
        /// </summary>
        [DataMember] public float Width;

        /// <summary>
        ///     The height of this <see cref="RectangleF" />.
        /// </summary>
        [DataMember] public float Height;

        /// <summary>
        ///     Gets the x-coordinate of the left edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Left => X;

        /// <summary>
        ///     Gets the x-coordinate of the right edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Right => X + Width;

        /// <summary>
        ///     Gets the y-coordinate of the top edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Top => Y;

        /// <summary>
        ///     Gets the y-coordinate of the bottom edge of this <see cref="RectangleF" />.
        /// </summary>
        public float Bottom => Y + Height;

        /// <summary>
        ///     Gets a value indicating whether this <see cref="RectangleF" /> has a <see cref="X" />, <see cref="Y" />,
        ///     <see cref="Width" />,
        ///     <see cref="Height" /> all equal to <code>0.0f</code>.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => Width.Equals(0) && Height.Equals(0) && X.Equals(0) && Y.Equals(0);

        /// <summary>
        ///     Gets the <see cref="PointF" /> representing the the top-left of this <see cref="RectangleF" />.
        /// </summary>
        public PointF Position
        {
            get => new PointF(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        ///     Gets the <see cref="SizeF" /> representing the extents of this <see cref="RectangleF" />.
        /// </summary>
        public SizeF Size
        {
            get => new SizeF(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        ///     Gets the <see cref="PointF" /> representing the center of this <see cref="RectangleF" />.
        /// </summary>
        public PointF Center => new PointF(X + Width * 0.5f, Y + Height * 0.5f);

        /// <summary>
        ///     Gets the <see cref="PointF" /> representing the top-left of this <see cref="RectangleF" />.
        /// </summary>
        public PointF TopLeft => new PointF(X, Y);

        /// <summary>
        ///     Gets the <see cref="PointF" /> representing the bottom-right of this <see cref="RectangleF" />.
        /// </summary>
        public PointF BottomRight => new PointF(X + Width, Y + Height);

        /// <summary>
        ///     Initializes a new instance of the <see cref="RectangleF" /> structure from the specified top-left xy-coordinate
        ///     <see cref="float" />s, width <see cref="float" /> and height <see cref="float" />.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RectangleF" /> structure from the specified top-left
        ///     <see cref="PointF" /> and the extents <see cref="SizeF" />.
        /// </summary>
        /// <param name="position">The top-left point.</param>
        /// <param name="size">The extents.</param>
        public RectangleF(PointF position, PointF size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> from a minimum <see cref="PointF" /> and maximum
        ///     <see cref="PointF" />.
        /// </summary>
        /// <param name="minimum">The minimum point.</param>
        /// <param name="maximum">The maximum point.</param>
        /// <param name="result">The resulting rectangle.</param>
        public static void CreateFrom(PointF minimum, PointF maximum, out RectangleF result)
        {
            result.X = minimum.X;
            result.Y = minimum.Y;
            result.Width = maximum.X - minimum.X;
            result.Height = maximum.Y - minimum.Y;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> from a minimum <see cref="PointF" /> and maximum
        ///     <see cref="PointF" />.
        /// </summary>
        /// <param name="minimum">The minimum point.</param>
        /// <param name="maximum">The maximum point.</param>
        /// <returns>The resulting <see cref="RectangleF" />.</returns>
        public static RectangleF CreateFrom(PointF minimum, PointF maximum)
        {
            CreateFrom(minimum, maximum, out RectangleF result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> that contains the two specified
        ///     <see cref="RectangleF" /> structures.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <param name="result">The resulting rectangle that contains both the <paramref name="first" /> and the
        ///     <paramref name="second" />.</param>
        public static void Union(RectangleF first, RectangleF second, out RectangleF result)
        {
            result.X = Math.Min(first.X, second.X);
            result.Y = Math.Min(first.Y, second.Y);
            result.Width = Math.Max(first.Right, second.Right) - result.X;
            result.Height = Math.Max(first.Bottom, second.Bottom) - result.Y;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> that contains the two specified
        ///     <see cref="RectangleF" /> structures.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <returns>
        ///     An <see cref="RectangleF" /> that contains both the <paramref name="first" /> and the
        ///     <paramref name="second" />.
        /// </returns>
        public static RectangleF Union(RectangleF first, RectangleF second)
        {
            Union(first, second, out RectangleF result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> that contains both the specified <see cref="RectangleF" /> and this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     An <see cref="RectangleF" /> that contains both the <paramref name="rectangle" /> and
        ///     this <see cref="RectangleF" />.
        /// </returns>
        public RectangleF Union(RectangleF rectangle)
        {
            Union(this, rectangle, out RectangleF result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> that is in common between the two specified
        ///     <see cref="RectangleF" /> structures.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <param name="result">The resulting rectangle that is in common between both the <paramref name="first" /> and
        ///     the <paramref name="second" />, if they intersect; otherwise, <see cref="Empty"/>.</param>
        public static void Intersection(RectangleF first, RectangleF second, out RectangleF result)
        {
            var firstMinimum = first.TopLeft;
            var firstMaximum = first.BottomRight;
            var secondMinimum = second.TopLeft;
            var secondMaximum = second.BottomRight;
            
            var minimum = PointF.Maximum(firstMinimum, secondMinimum);
            var maximum = PointF.Minimum(firstMaximum, secondMaximum);

            if ((maximum.X < minimum.X) || (maximum.Y < minimum.Y))
                result = RectangleF.Empty;
            else
                result = CreateFrom(minimum, maximum);
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> that is in common between the two specified
        ///     <see cref="RectangleF" /> structures.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <returns>
        ///     A <see cref="RectangleF" /> that is in common between both the <paramref name="first" /> and
        ///     the <paramref name="second" />, if they intersect; otherwise, <see cref="Empty"/>.
        /// </returns>
        public static RectangleF Intersection(RectangleF first, RectangleF second)
        {
            Intersection(first, second, out RectangleF result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> that is in common between the specified
        ///     <see cref="RectangleF" /> and this <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     A <see cref="RectangleF" /> that is in common between both the <paramref name="rectangle" /> and
        ///     this <see cref="RectangleF"/>, if they intersect; otherwise, <see cref="Empty"/>.
        /// </returns>
        public RectangleF Intersection(RectangleF rectangle)
        {
            Intersection(this, rectangle, out RectangleF result);
            return result;
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="RectangleF" /> structures intersect.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="first" /> intersects with the <paramref name="second" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Intersects(RectangleF first, RectangleF second)
        {
            return first.X < second.X + second.Width && first.X + first.Width > second.X &&
                   first.Y < second.Y + second.Height && first.Y + first.Height > second.Y;
        }
        
        /// <summary>
        ///     Determines whether the specified <see cref="RectangleF" /> intersects with this
        ///     <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="rectangle" /> intersects with this
        ///     <see cref="RectangleF" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Intersects(RectangleF rectangle)
        {
            return Intersects(this, rectangle);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="RectangleF" /> contains the specified
        ///     <see cref="PointF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="rectangle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool Contains(RectangleF rectangle, PointF point)
        {
            return 
                rectangle.X <= point.X &&
                point.X < rectangle.X + rectangle.Width &&
                rectangle.Y <= point.Y &&
                point.Y < rectangle.Y + rectangle.Height;
        }

        /// <summary>
        ///     Determines whether this <see cref="RectangleF" /> contains the specified
        ///     <see cref="PointF" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the this <see cref="RectangleF"/> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Contains(PointF point)
        {
            return Contains(this, point);
        }

        /// <summary>
        ///     Determines whether this <see cref="RectangleF" /> contains the specified
        ///     <see cref="RectangleF" />.
        /// </summary>
        public bool Contains(RectangleF value)
        {
            return
                X < value.X &&
                value.X + value.Width <= X + Width &&
                Y < value.Y &&
                value.Y + value.Height <= Y + Height;
        }

        /// <summary>
        ///     Computes the squared distance from this <see cref="RectangleF"/> to a <see cref="PointF"/>.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The squared distance from this <see cref="RectangleF"/> to the <paramref name="point"/>.</returns>
        public float SquaredDistanceTo(PointF point)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.1.3.1; Basic Primitive Tests - Closest-point Computations - Distance of Point to AABB.  pg 130-131
            var squaredDistance = 0.0f;
            var minimum = TopLeft;
            var maximum = BottomRight;

            // for each axis add up the excess distance outside the box

            // x-axis
            if (point.X < minimum.X)
            {
                var distance = minimum.X - point.X;
                squaredDistance += distance * distance;
            }
            else if (point.X > maximum.X)
            {
                var distance = maximum.X - point.X;
                squaredDistance += distance * distance;
            }

            // y-axis
            if (point.Y < minimum.Y)
            {
                var distance = minimum.Y - point.Y;
                squaredDistance += distance * distance;
            }
            else if (point.Y > maximum.Y)
            {
                var distance = maximum.Y - point.Y;
                squaredDistance += distance * distance;
            }
            return squaredDistance;
        }

        /// <summary>
        ///     Computes the distance from this <see cref="RectangleF"/> to a <see cref="PointF"/>.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The distance from this <see cref="RectangleF"/> to the <paramref name="point"/>.</returns>
        public float DistanceTo(PointF point)
        {
            return (float)Math.Sqrt(SquaredDistanceTo(point));
        }

        //TODO: Document this.
        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount * 2;
            Height += verticalAmount * 2;
        }

        //TODO: Document this.
        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        //TODO: Document this.
        public void Offset(PointF amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        public static RectangleF operator +(RectangleF a, RectangleF b)
        {
            return new RectangleF(
                a.X + b.X,
                a.Y + b.Y,
                a.Width + b.Width,
                a.Height + b.Height);
        }

        public static RectangleF operator -(RectangleF a, RectangleF b)
        {
            return new RectangleF(
                a.X - b.X,
                a.Y - b.Y,
                a.Width - b.Width,
                a.Height - b.Height);
        }

        /// <summary>
        ///     Compares two <see cref="RectangleF" /> structures. The result specifies whether the values of the
        ///     <see cref="X" />, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height" /> fields of the two <see cref="RectangleF" /> structures
        ///     are equal.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the values of the
        ///     <see cref="X" />, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height" /> fields of the two <see cref="RectangleF" /> structures
        ///     are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(RectangleF first, RectangleF second)
        {
            return first.Equals(second);
        }

        /// <summary>
        ///     Compares two <see cref="RectangleF" /> structures. The result specifies whether the values of the
        ///     <see cref="X" />, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height" /> fields of the two <see cref="RectangleF" /> structures
        ///     are unequal.
        /// </summary>
        /// <param name="first">The first rectangle.</param>
        /// <param name="second">The second rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the values of the
        ///     <see cref="X" />, <see cref="Y"/>, <see cref="Width"/> and <see cref="Height" /> fields of the two <see cref="RectangleF" /> structures
        ///     are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(RectangleF first, RectangleF second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Indicates whether this <see cref="RectangleF" /> is equal to another <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="RectangleF" /> is equal to the <paramref name="rectangle" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(RectangleF rectangle)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return X == rectangle.X && Y == rectangle.Y && Width == rectangle.Width && Height == rectangle.Height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="RectangleF" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="RectangleF" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if(obj is RectangleF rect)
                return Equals(rect);
            return false;
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="RectangleF" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="RectangleF" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 7 + X.GetHashCode();
                hashCode = (hashCode * 3) + Y.GetHashCode();
                hashCode = (hashCode * 3) + Width.GetHashCode();
                hashCode = (hashCode * 3) + Height.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Rectangle" /> to a <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="RectangleF" />.
        /// </returns>
        public static implicit operator RectangleF(Rectangle rectangle)
        {
            return new RectangleF
            {
                X = rectangle.X,
                Y = rectangle.Y,
                Width = rectangle.Width,
                Height = rectangle.Height
            };
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Rectangle" /> to a <see cref="RectangleF" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="RectangleF" />.
        /// </returns>
        /// <remarks>
        ///     <para>A loss of precision may occur due to the truncation from <see cref="float" /> to <see cref="int" />.</para>
        /// </remarks>
        public static explicit operator Rectangle(RectangleF rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }
        
        public Rectangle ToRectangle()
        {
            return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="RectangleF" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="RectangleF" />.
        /// </returns>
        public override string ToString()
        {
            return $"{{X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
        }

        internal string DebugDisplayString => string.Concat(X, "  ", Y, "  ", Width, "  ", Height);
    }
}