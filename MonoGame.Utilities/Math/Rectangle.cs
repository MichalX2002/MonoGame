// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace MonoGame.Framework
{
    /// <summary>
    /// Describes a 2D-rectangle. 
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct Rectangle : IEquatable<Rectangle>
    {
        /// <summary>
        /// The x coordinate of the top-left corner of this <see cref="Rectangle"/>.
        /// </summary>
        [DataMember] public int X;

        /// <summary>
        /// The y coordinate of the top-left corner of this <see cref="Rectangle"/>.
        /// </summary>
        [DataMember] public int Y;

        /// <summary>
        /// The width of this <see cref="Rectangle"/>.
        /// </summary>
        [DataMember] public int Width;

        /// <summary>
        /// The height of this <see cref="Rectangle"/>.
        /// </summary>
        [DataMember] public int Height;

        #region Public Properties

        /// <summary>
        /// Returns a <see cref="Rectangle"/> with X=0, Y=0, Width=0, Height=0.
        /// </summary>
        public static readonly Rectangle Empty = new Rectangle();

        /// <summary>
        /// Returns the x coordinate of the left edge of this <see cref="Rectangle"/>.
        /// </summary>
        public int Left => X;

        /// <summary>
        /// Returns the x coordinate of the right edge of this <see cref="Rectangle"/>.
        /// </summary>
        public int Right => X + Width;

        /// <summary>
        /// Returns the y coordinate of the top edge of this <see cref="Rectangle"/>.
        /// </summary>
        public int Top => Y;

        /// <summary>
        /// Returns the y coordinate of the bottom edge of this <see cref="Rectangle"/>.
        /// </summary>
        public int Bottom => Y + Height;

        /// <summary>
        /// Whether or not this <see cref="Rectangle"/> has a <see cref="Width"/> and
        /// <see cref="Height"/> of 0, and a <see cref="Position"/> of (0, 0).
        /// </summary>
        public bool IsEmpty => (Width == 0) && (Height == 0) && (X == 0) && (Y == 0);

        /// <summary>
        /// The top-left coordinates of this <see cref="Rectangle"/>.
        /// </summary>
        public Point Position
        {
            get => new Point(X, Y);
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
            get => new Size(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        /// A <see cref="Point"/> located in the center of this <see cref="Rectangle"/>.
        /// </summary>
        /// <remarks>
        /// If <see cref="Width"/> or <see cref="Height"/> is an odd number,
        /// the center point will be rounded down.
        /// </remarks>
        public Point Center => new Point(X + (Width / 2), Y + (Height / 2));

        #endregion

        #region Internal Properties

        internal string DebugDisplayString
        {
            get => string.Concat(
                    X, "  ",
                    Y, "  ",
                    Width, "  ",
                    Height
                    );
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

        #endregion

        #region Operators

        public static Rectangle operator +(in Rectangle a, in Rectangle b)
        {
            return new Rectangle(
                a.X + b.X,
                a.Y + b.Y,
                a.Width + b.Width,
                a.Height + b.Height);
        }

        public static Rectangle operator -(in Rectangle a, in Rectangle b)
        {
            return new Rectangle(
                a.X - b.X,
                a.Y - b.Y,
                a.Width - b.Width,
                a.Height - b.Height);
        }

        /// <summary>
        /// Compares whether two <see cref="Rectangle"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Rectangle"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Rectangle"/> instance on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(in Rectangle a, in Rectangle b)
        {
            return (a.X == b.X) && (a.Y == b.Y) && (a.Width == b.Width) && (a.Height == b.Height);
        }

        /// <summary>
        /// Compares whether two <see cref="Rectangle"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Rectangle"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Rectangle"/> instance on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(in Rectangle a, in Rectangle b)
        {
            return !(a == b);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets whether or not the provided coordinates lie within the bounds of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="x">The x coordinate of the point to check for containment.</param>
        /// <param name="y">The y coordinate of the point to check for containment.</param>
        /// <returns><c>true</c> if the provided coordinates lie inside this <see cref="Rectangle"/>; <c>false</c> otherwise.</returns>
        public bool Contains(int x, int y) =>
            (X <= x) && (x < (X + Width)) && (Y <= y) && (y < (Y + Height));

        /// <summary>
        /// Gets whether or not the provided coordinates lie within the bounds of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="x">The x coordinate of the point to check for containment.</param>
        /// <param name="y">The y coordinate of the point to check for containment.</param>
        /// <returns><c>true</c> if the provided coordinates lie inside this <see cref="Rectangle"/>; <c>false</c> otherwise.</returns>
        public bool Contains(float x, float y) => 
            (X <= x) && (x < (X + Width)) && (Y <= y) && (y < (Y + Height));

        /// <summary>
        /// Gets whether or not the provided <see cref="Point"/> lies within the bounds of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="Rectangle"/>.</param>
        /// <returns><c>true</c> if the provided <see cref="Point"/> lies inside this <see cref="Rectangle"/>; <c>false</c> otherwise.</returns>
        public bool Contains(Point value) =>
            (X <= value.X) && (value.X < (X + Width)) &&
            (Y <= value.Y) && (value.Y < (Y + Height));

        /// <summary>
        /// Gets whether or not the provided <see cref="Vector2"/> lies within the bounds of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="Rectangle"/>.</param>
        /// <returns><c>true</c> if the provided <see cref="Vector2"/> lies inside this <see cref="Rectangle"/>; <c>false</c> otherwise.</returns>
        public bool Contains(Vector2 value) =>
            (X <= value.X) && (value.X < (X + Width)) &&
            (Y <= value.Y) && (value.Y < (Y + Height));

        /// <summary>
        /// Gets whether or not the provided <see cref="Rectangle"/> lies within the bounds of this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="value">The <see cref="Rectangle"/> to check for inclusion in this <see cref="Rectangle"/>.</param>
        /// <returns><c>true</c> if the provided <see cref="Rectangle"/>'s bounds lie entirely inside this <see cref="Rectangle"/>; <c>false</c> otherwise.</returns>
        public bool Contains(in Rectangle value) => 
            (X <= value.X) && ((value.X + value.Width) <= (X + Width)) && 
            (Y <= value.Y) && ((value.Y + value.Height) <= (Y + Height));

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is Rectangle other && Equals(other);

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Rectangle"/>.
        /// </summary>
        public bool Equals(Rectangle other) => this == other;

        /// <summary>
        /// Gets the hash code of this <see cref="Rectangle"/>.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 7 + X.GetHashCode();
                hash = hash * 31 + Y.GetHashCode();
                hash = hash * 31 + Width.GetHashCode();
                return hash * 31 + Height.GetHashCode();
            }
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
        /// <returns><c>true</c> if other <see cref="Rectangle"/> intersects with this rectangle; <c>false</c> otherwise.</returns>
        public bool Intersects(in Rectangle value)
        {
            return value.Left < Right &&
                   Left < value.Right &&
                   value.Top < Bottom &&
                   Top < value.Bottom;
        }

        /// <summary>
        /// Creates a new <see cref="Rectangle"/> that contains overlapping region of two other rectangles.
        /// </summary>
        /// <param name="first">The first <see cref="Rectangle"/>.</param>
        /// <param name="second">The second <see cref="Rectangle"/>.</param>
        /// <returns>Overlapping region of the two rectangles.</returns>
        public static Rectangle Intersect(in Rectangle first, in Rectangle second)
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
        /// Returns a <see cref="string"/> representation of this <see cref="Rectangle"/> in the format:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Width:[<see cref="Width"/>] Height:[<see cref="Height"/>]}
        /// </summary>
        /// <returns><see cref="string"/> representation of this <see cref="Rectangle"/>.</returns>
        public override string ToString() =>
            "{X:" + X + " Y:" + Y + " Width:" + Width + " Height:" + Height + "}";

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
        public void Deconstruct(out int x, out int y, out int width, out int height)
        {
            x = X;
            y = Y;
            width = Width;
            height = Height;
        }

        public RectangleF ToRectangleF() => new RectangleF(X, Y, Width, Height);

        #endregion
    }
}
