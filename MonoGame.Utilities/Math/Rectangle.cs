// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Numerics;

namespace MonoGame.Framework
{
    // TODO: rename to Rect

    /// <summary>
    /// Describes a 2D-rectangle. 
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Rectangle : IEquatable<Rectangle>
    {
        /// <summary>
        /// The x coordinate of the top-left corner of this <see cref="Rectangle"/>.
        /// </summary>
        [DataMember] 
        public int X;

        /// <summary>
        /// The y coordinate of the top-left corner of this <see cref="Rectangle"/>.
        /// </summary>
        [DataMember] 
        public int Y;

        /// <summary>
        /// The width of this <see cref="Rectangle"/>.
        /// </summary>
        [DataMember] 
        public int Width;

        /// <summary>
        /// The height of this <see cref="Rectangle"/>.
        /// </summary>
        [DataMember]
        public int Height;

        internal string DebuggerDisplay => string.Concat(
            X.ToString(), "  ",
            Y.ToString(), "  ",
            Width.ToString(), "  ",
            Height.ToString());

        #region Public Properties

        /// <summary>
        /// Returns a <see cref="Rectangle"/> with X=0, Y=0, Width=0, Height=0.
        /// </summary>
        public static Rectangle Empty => default;

        /// <summary>
        /// Returns the x coordinate of the left edge of this <see cref="Rectangle"/>.
        /// </summary>
        public readonly int Left => X;

        /// <summary>
        /// Returns the x coordinate of the right edge of this <see cref="Rectangle"/>.
        /// </summary>
        public readonly int Right => X + Width;

        /// <summary>
        /// Returns the y coordinate of the top edge of this <see cref="Rectangle"/>.
        /// </summary>
        public readonly int Top => Y;

        /// <summary>
        /// Returns the y coordinate of the bottom edge of this <see cref="Rectangle"/>.
        /// </summary>
        public readonly int Bottom => Y + Height;

        /// <summary>
        /// Whether or not this <see cref="Rectangle"/> has a <see cref="Width"/> and
        /// <see cref="Height"/> of 0, and a <see cref="Position"/> of (0, 0).
        /// </summary>
        public readonly bool IsEmpty => (Width == 0) && (Height == 0) && (X == 0) && (Y == 0);

        /// <summary>
        /// A <see cref="Point"/> located in the center of this <see cref="Rectangle"/>.
        /// </summary>
        /// <remarks>
        /// If <see cref="Width"/> or <see cref="Height"/> is an odd number,
        /// the center point will be rounded down.
        /// </remarks>
        public readonly Point Center => new Point(X + (Width / 2), Y + (Height / 2));

        /// <summary>
        /// The top-left coordinates of this <see cref="Rectangle"/>.
        /// </summary>
        public Point Position
        {
            readonly get => new Point(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        /// The width-height coordinates of this <see cref="Rectangle"/>.
        /// </summary>
        public Size Size
        {
            readonly get => new Size(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="Rectangle"/> struct, with the specified
        /// position, width, and height.
        /// </summary>
        /// <param name="x">The x coordinate of the top-left corner of the created <see cref="Rectangle"/>.</param>
        /// <param name="y">The y coordinate of the top-left corner of the created <see cref="Rectangle"/>.</param>
        /// <param name="width">The width of the created <see cref="Rectangle"/>.</param>
        /// <param name="height">The height of the created <see cref="Rectangle"/>.</param>
        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Rectangle"/> struct, with the specified
        /// location and size.
        /// </summary>
        /// <param name="location">The x and y coordinates of the top-left corner of the created <see cref="Rectangle"/>.</param>
        /// <param name="size">The width and height of the created <see cref="Rectangle"/>.</param>
        public Rectangle(Point location, Size size)
        {
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Rectangle"/> struct,
        /// with the specified size, where x and y are zero.
        /// </summary>
        /// <param name="size">The width and height of the created <see cref="Rectangle"/>.</param>
        public Rectangle(Size size) : this(Point.Zero, size)
        {
        }

        #endregion

        #region Operators

        public static Rectangle operator +(Rectangle a, Rectangle b)
        {
            return new Rectangle(
                a.X + b.X,
                a.Y + b.Y,
                a.Width + b.Width,
                a.Height + b.Height);
        }

        public static Rectangle operator -(Rectangle a, Rectangle b)
        {
            return new Rectangle(
                a.X - b.X,
                a.Y - b.Y,
                a.Width - b.Width,
                a.Height - b.Height);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets whether or not the provided coordinates lie within the bounds of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="x">The x coordinate of the point to check for containment.</param>
        /// <param name="y">The y coordinate of the point to check for containment.</param>
        /// <returns><see langword="true"/> if the provided coordinates lie inside this <see cref="Rectangle"/>; <see langword="false"/> otherwise.</returns>
        public readonly bool Contains(int x, int y)
        {
            return (X <= x)
                && (x < (X + Width))
                && (Y <= y) 
                && (y < (Y + Height));
        }

        /// <summary>
        /// Gets whether or not the provided coordinates lie within the bounds of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="x">The x coordinate of the point to check for containment.</param>
        /// <param name="y">The y coordinate of the point to check for containment.</param>
        /// <returns><see langword="true"/> if the provided coordinates lie inside this <see cref="Rectangle"/>; <see langword="false"/> otherwise.</returns>
        public readonly bool Contains(float x, float y)
        {
            return (X <= x)
                && (x < (X + Width))
                && (Y <= y) 
                && (y < (Y + Height));
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Point"/> lies within the bounds of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="Rectangle"/>.</param>
        /// <returns><see langword="true"/> if the provided <see cref="Point"/> lies inside this <see cref="Rectangle"/>; <see langword="false"/> otherwise.</returns>
        public readonly bool Contains(Point value)
        {
            return (X <= value.X) && (value.X < (X + Width)) 
                && (Y <= value.Y) && (value.Y < (Y + Height));
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="PointF"/> lies within the bounds of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="Rectangle"/>.</param>
        /// <returns><see langword="true"/> if the provided <see cref="PointF"/> lies inside this <see cref="Rectangle"/>; <see langword="false"/> otherwise.</returns>
        public readonly bool Contains(PointF value)
        {
            return (X <= value.X) && (value.X < (X + Width)) 
                && (Y <= value.Y) && (value.Y < (Y + Height));
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Rectangle"/> lies within the bounds of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="value">The <see cref="Rectangle"/> to check for inclusion in this <see cref="Rectangle"/>.</param>
        /// <returns><see langword="true"/> if the provided <see cref="Rectangle"/>'s bounds lie entirely inside this <see cref="Rectangle"/>; <see langword="false"/> otherwise.</returns>
        public readonly bool Contains(Rectangle value)
        {
            return (X <= value.X) && ((value.X + value.Width) <= (X + Width)) 
                && (Y <= value.Y) && ((value.Y + value.Height) <= (Y + Height));
        }

        /// <summary>
        /// Adjusts the edges of this <see cref="Rectangle"/> by specified horizontal and vertical amounts. 
        /// </summary>
        /// <param name="horizontalAmount">Value to adjust the left and right edges.</param>
        /// <param name="verticalAmount">Value to adjust the top and bottom edges.</param>
        public void Inflate(int horizontalAmount, int verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount * 2;
            Height += verticalAmount * 2;
        }

        /// <summary>
        /// Adjusts the edges of this <see cref="Rectangle"/> by specified horizontal and vertical amounts. 
        /// </summary>
        /// <param name="horizontalAmount">Value to adjust the left and right edges.</param>
        /// <param name="verticalAmount">Value to adjust the top and bottom edges.</param>
        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            X -= (int)horizontalAmount;
            Y -= (int)verticalAmount;
            Width += (int)horizontalAmount * 2;
            Height += (int)verticalAmount * 2;
        }

        /// <summary>
        /// Gets whether or not the other <see cref="Rectangle"/> intersects with this rectangle.
        /// </summary>
        /// <param name="value">The other rectangle for testing.</param>
        /// <returns><see langword="true"/> if other <see cref="Rectangle"/> intersects with this rectangle; <see langword="false"/> otherwise.</returns>
        public readonly bool Intersects(Rectangle value)
        {
            return value.Left < Right
                && Left < value.Right 
                && value.Top < Bottom 
                && Top < value.Bottom;
        }

        /// <summary>
        /// Creates a new <see cref="Rectangle"/> that contains overlapping region of two other rectangles.
        /// </summary>
        /// <param name="first">The first <see cref="Rectangle"/>.</param>
        /// <param name="second">The second <see cref="Rectangle"/>.</param>
        /// <returns>Overlapping region of the two rectangles.</returns>
        public static Rectangle Intersect(Rectangle first, Rectangle second)
        {
            if (first.Intersects(second))
            {
                int right_side = Math.Min(first.X + first.Width, second.X + second.Width);
                int left_side = Math.Max(first.X, second.X);
                int top_side = Math.Max(first.Y, second.Y);
                int bottom_side = Math.Min(first.Y + first.Height, second.Y + second.Height);
                return new Rectangle(left_side, top_side, right_side - left_side, bottom_side - top_side);
            }
            else
            {
                return Empty;
            }
        }

        /// <summary>
        /// Changes the <see cref="Position"/> of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="offsetX">The x coordinate to add to this <see cref="Rectangle"/>.</param>
        /// <param name="offsetY">The y coordinate to add to this <see cref="Rectangle"/>.</param>
        public void Offset(int offsetX, int offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        /// <summary>
        /// Changes the <see cref="Position"/> of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="offsetX">The x coordinate to add to this <see cref="Rectangle"/>.</param>
        /// <param name="offsetY">The y coordinate to add to this <see cref="Rectangle"/>.</param>
        public void Offset(float offsetX, float offsetY)
        {
            X += (int)offsetX;
            Y += (int)offsetY;
        }

        /// <summary>
        /// Changes the <see cref="Position"/> of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="amount">The x and y components to add to this <see cref="Rectangle"/>.</param>
        public void Offset(Point amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        /// <summary>
        /// Changes the <see cref="Position"/> of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="amount">The x and y components to add to this <see cref="Rectangle"/>.</param>
        public void Offset(Vector2 amount)
        {
            X += (int)amount.X;
            Y += (int)amount.Y;
        }

        /// <summary>
        /// Creates a new <see cref="Rectangle"/> that completely contains two other rectangles.
        /// </summary>
        /// <param name="a">The first <see cref="Rectangle"/>.</param>
        /// <param name="b">The second <see cref="Rectangle"/>.</param>
        /// <returns>The union of the two rectangles.</returns>
        public static Rectangle Union(Rectangle a, Rectangle b)
        {
            int x = Math.Min(a.X, b.X);
            int y = Math.Min(a.Y, b.Y);

            return new Rectangle(x, y,
                Math.Max(a.Right, b.Right) - x,
                Math.Max(a.Bottom, b.Bottom) - y);
        }

        /// <summary>
        /// Deconstruction method for <see cref="Rectangle"/>.
        /// </summary>
        public readonly void Deconstruct(out int x, out int y, out int width, out int height)
        {
            x = X;
            y = Y;
            width = Width;
            height = Height;
        }

        public readonly RectangleF ToRectangleF()
        {
            return new RectangleF(X, Y, Width, Height);
        }

        #endregion

        #region Equals

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Rectangle"/>.
        /// </summary>  
        public readonly bool Equals(Rectangle other)
        {
            return this == other;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>.
        /// </summary>
        public override readonly bool Equals(object? obj)
        {
            return obj is Rectangle other && Equals(other);
        }

        /// <summary>
        /// Compares whether two <see cref="Rectangle"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Rectangle"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Rectangle"/> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(Rectangle a, Rectangle b)
        {
            return (a.X == b.X)
                && (a.Y == b.Y)
                && (a.Width == b.Width)
                && (a.Height == b.Height);
        }

        /// <summary>
        /// Compares whether two <see cref="Rectangle"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Rectangle"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Rectangle"/> instance on the right of the not equal sign.</param>
        /// <returns><see langword="true"/> if the instances are not equal; <see langword="false"/> otherwise.</returns>
        public static bool operator !=(Rectangle a, Rectangle b)
        {
            return !(a == b);
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets the hash code of this <see cref="Rectangle"/>.
        /// </summary>
        public override readonly int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Rectangle"/>.
        /// </summary>
        /// <returns><see cref="string"/> representation of this <see cref="Rectangle"/>.</returns>
        public override readonly string ToString()
        {
            return $"(X:{X}, Y:{Y}, W:{Width}, H:{Height})";
        }

        #endregion
    }
}
