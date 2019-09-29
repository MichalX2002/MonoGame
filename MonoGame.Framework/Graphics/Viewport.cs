// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.Serialization;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Describes the view bounds for render-target surface.
    /// </summary>
    [DataContract]
    public struct Viewport
    {
        #region Properties

        /// <summary>
        /// The x coordinate of the beginning of this viewport.
        /// </summary>
        [DataMember]
        public int X { get; set; }

        /// <summary>
        /// The y coordinate of the beginning of this viewport.
        /// </summary>
        [DataMember]
        public int Y { get; set; }

        /// <summary>
        /// The width of the bounds in pixels.
        /// </summary>
        [DataMember]
        public int Width { get; set; }

        /// <summary>
        /// The height of the bounds in pixels.
        /// </summary>
        [DataMember]
        public int Height { get; set; }

        /// <summary>
        /// The lower limit of depth of this viewport.
        /// </summary>
        [DataMember]
        public float MinDepth { get; set; }

        /// <summary>
        /// The upper limit of depth of this viewport.
        /// </summary>
        [DataMember]
        public float MaxDepth { get; set; }

        #endregion

        /// <summary>
        /// Gets the aspect ratio of this <see cref="Viewport"/>, which is width / height. 
        /// </summary>
        public float AspectRatio
        {
            get
            {
                if ((Height != 0) && (Width != 0))
                    return Width / (float)Height;
                return 0f;
            }
        }

        /// <summary>
        /// Gets or sets a boundary of this <see cref="Viewport"/>.
        /// </summary>
        public Rectangle Bounds
        {
            get => new Rectangle(X, Y, Width, Height);
            set
            {
                X = value.X;
                Y = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        /// Returns the subset of the viewport that is guaranteed to be visible on a lower quality display.
        /// </summary>
		public Rectangle TitleSafeArea => GraphicsDevice.GetTitleSafeArea(X, Y, Width, Height);

        /// <summary>
        /// Constructs a viewport from the given values. The <see cref="MinDepth"/> will be 0.0 and <see cref="MaxDepth"/> will be 1.0.
        /// </summary>
        /// <param name="x">The x coordinate of the upper-left corner of the view bounds in pixels.</param>
        /// <param name="y">The y coordinate of the upper-left corner of the view bounds in pixels.</param>
        /// <param name="width">The width of the view bounds in pixels.</param>
        /// <param name="height">The height of the view bounds in pixels.</param>
        public Viewport(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            MinDepth = 0f;
            MaxDepth = 1f;
        }

        /// <summary>
        /// Constructs a viewport from the given values.
        /// </summary>
        /// <param name="x">The x coordinate of the upper-left corner of the view bounds in pixels.</param>
        /// <param name="y">The y coordinate of the upper-left corner of the view bounds in pixels.</param>
        /// <param name="width">The width of the view bounds in pixels.</param>
        /// <param name="height">The height of the view bounds in pixels.</param>
        /// <param name="minDepth">The lower limit of depth.</param>
        /// <param name="maxDepth">The upper limit of depth.</param>
        public Viewport(int x, int y, int width, int height, float minDepth, float maxDepth)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            MinDepth = minDepth;
            MaxDepth = maxDepth;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Viewport"/> struct.
        /// </summary>
        /// <param name="bounds">A <see cref="Rectangle"/> that defines the location and size of the <see cref="Viewport"/> in a render target.</param>
		public Viewport(in Rectangle bounds) : this(bounds.X, bounds.Y, bounds.Width, bounds.Height)
        {
        }

        /// <summary>
        /// Projects a <see cref="Vector3"/> from world space into screen space.
        /// </summary>
        /// <param name="source">The <see cref="Vector3"/> to project.</param>
        /// <param name="projection">The projection <see cref="Matrix"/>.</param>
        /// <param name="view">The view <see cref="Matrix"/>.</param>
        /// <param name="world">The world <see cref="Matrix"/>.</param>
        /// <returns></returns>
        public Vector3 Project(in Vector3 source, in Matrix projection, in Matrix view, in Matrix world)
        {
            Matrix matrix = Matrix.Multiply(Matrix.Multiply(world, view), projection);
            Vector3 vector = Vector3.Transform(source, matrix);
            float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;
            if (!WithinEpsilon(a, 1f))
            {
                vector.X /= a;
                vector.Y /= a;
                vector.Z /= a;
            }
            vector.X = (((vector.X + 1f) * 0.5f) * Width) + X;
            vector.Y = (((-vector.Y + 1f) * 0.5f) * Height) + Y;
            vector.Z = (vector.Z * (MaxDepth - MinDepth)) + MinDepth;
            return vector;
        }

        /// <summary>
        /// Unprojects a <see cref="Vector3"/> from screen space into world space.
        /// Note source.Z must be less than or equal to MaxDepth.
        /// </summary>
        /// <param name="source">The <see cref="Vector3"/> to unproject.</param>
        /// <param name="projection">The projection <see cref="Matrix"/>.</param>
        /// <param name="view">The view <see cref="Matrix"/>.</param>
        /// <param name="world">The world <see cref="Matrix"/>.</param>
        /// <returns></returns>
        public Vector3 Unproject(in Vector3 source, in Matrix projection, in Matrix view, in Matrix world)
        {
            Matrix matrix = Matrix.Invert(Matrix.Multiply(Matrix.Multiply(world, view), projection));
            Vector3 usource = new Vector3(
                (((source.X - X) / Width) * 2f) - 1f,
                -((((source.Y - Y) / Height) * 2f) - 1f),
                (source.Z - MinDepth) / (MaxDepth - MinDepth));

            Vector3 vector = Vector3.Transform(usource, matrix);
            float a = (((usource.X * matrix.M14) + (usource.Y * matrix.M24)) + (usource.Z * matrix.M34)) + matrix.M44;
            if (!WithinEpsilon(a, 1f))
            {
                vector.X /= a;
                vector.Y /= a;
                vector.Z /= a;
            }
            return vector;
        }

        private static bool WithinEpsilon(float a, float b)
        {
            float num = a - b;
            return ((-1.401298E-45f <= num) && (num <= float.Epsilon));
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Viewport"/> in the format:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Width:[<see cref="Width"/>] Height:[<see cref="Height"/>] MinDepth:[<see cref="MinDepth"/>] MaxDepth:[<see cref="MaxDepth"/>]}
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Viewport"/>.</returns>
        public override string ToString()
        {
            return "{X:" + X + " Y:" + Y + " Width:" + Width + " Height:" + Height + " MinDepth:" + MinDepth + " MaxDepth:" + MaxDepth + "}";
        }
    }
}