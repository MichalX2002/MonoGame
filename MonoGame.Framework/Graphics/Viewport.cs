// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.Serialization;

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// Describes the view bounds for render-target surface.
    /// </summary>
    [DataContract]
    public struct Viewport
    {
        private int y;
        private int height;
        private float maxDepth;

        #region Properties

        /// <summary>
        /// The height of the bounds in pixels.
        /// </summary>
        [DataMember]
        public int Height
        {
			get {
				return this.height;
			}
			set {
				height = value;
			}
		}

        /// <summary>
        /// The upper limit of depth of this viewport.
        /// </summary>
        [DataMember]
        public float MaxDepth
        {
			get {
				return this.maxDepth;
			}
			set {
				maxDepth = value;
			}
		}

        /// <summary>
        /// The lower limit of depth of this viewport.
        /// </summary>
        [DataMember]
        public float MinDepth { get; set; }

        /// <summary>
        /// The width of the bounds in pixels.
        /// </summary>
        [DataMember]
        public int Width { get; set; }

        /// <summary>
        /// The y coordinate of the beginning of this viewport.
        /// </summary>
        [DataMember]
        public int Y
        {
			get {
				return this.y;

			}
			set {
				y = value;
			}
		}

        /// <summary>
        /// The x coordinate of the beginning of this viewport.
        /// </summary>
        [DataMember]
        public int X { get; set; }

        #endregion

        /// <summary>
        /// Gets the aspect ratio of this <see cref="Viewport"/>, which is width / height. 
        /// </summary>
        public float AspectRatio 
		{
			get
			{
				if ((height != 0) && (Width != 0))
				{
					return Width / (float)height;
				}
				return 0f;
			}
		}
		
        /// <summary>
        /// Gets or sets a boundary of this <see cref="Viewport"/>.
        /// </summary>
		public Rectangle Bounds 
		{
            get
            {
                return new Rectangle(X, y, Width, height);
            }
				
			set
			{				
				X = value.X;
				y = value.Y;
				Width = value.Width;
				height = value.Height;
			}
		}

        /// <summary>
        /// Returns the subset of the viewport that is guaranteed to be visible on a lower quality display.
        /// </summary>
		public Rectangle TitleSafeArea 
		{
			get { return GraphicsDevice.GetTitleSafeArea(X, y, Width, height); }
		}

        /// <summary>
        /// Constructs a viewport from the given values. The <see cref="MinDepth"/> will be 0.0 and <see cref="MaxDepth"/> will be 1.0.
        /// </summary>
        /// <param name="x">The x coordinate of the upper-left corner of the view bounds in pixels.</param>
        /// <param name="y">The y coordinate of the upper-left corner of the view bounds in pixels.</param>
        /// <param name="width">The width of the view bounds in pixels.</param>
        /// <param name="height">The height of the view bounds in pixels.</param>
        public Viewport(int x, int y, int width, int height)
		{
			this.X = x;
		    this.y = y;
		    this.Width = width;
		    this.height = height;
		    this.MinDepth = 0.0f;
		    this.maxDepth = 1.0f;
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
        public Viewport(int x, int y, int width, int height,float minDepth,float maxDepth)
        {
            this.X = x;
            this.y = y;
            this.Width = width;
            this.height = height;
            this.MinDepth = minDepth;
            this.maxDepth = maxDepth;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Viewport"/> struct.
        /// </summary>
        /// <param name="bounds">A <see cref="Rectangle"/> that defines the location and size of the <see cref="Viewport"/> in a render target.</param>
		public Viewport(Rectangle bounds) : this(bounds.X, bounds.Y, bounds.Width, bounds.Height)
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
        public Vector3 Project(Vector3 source, Matrix projection, Matrix view, Matrix world)
        {
            Matrix matrix = Matrix.Multiply(Matrix.Multiply(world, view), projection);
		    Vector3 vector = Vector3.Transform(source, matrix);
		    float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;
		    if (!WithinEpsilon(a, 1f))
		    {
		        vector.X = vector.X / a;
		        vector.Y = vector.Y / a;
		        vector.Z = vector.Z / a;
		    }
		    vector.X = (((vector.X + 1f) * 0.5f) * this.Width) + this.X;
		    vector.Y = (((-vector.Y + 1f) * 0.5f) * this.height) + this.y;
		    vector.Z = (vector.Z * (this.maxDepth - this.MinDepth)) + this.MinDepth;
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
        public Vector3 Unproject(Vector3 source, Matrix projection, Matrix view, Matrix world)
        {
             Matrix matrix = Matrix.Invert(Matrix.Multiply(Matrix.Multiply(world, view), projection));
		    source.X = (((source.X - this.X) / this.Width) * 2f) - 1f;
		    source.Y = -((((source.Y - this.y) / this.height) * 2f) - 1f);
		    source.Z = (source.Z - this.MinDepth) / (this.maxDepth - this.MinDepth);
		    Vector3 vector = Vector3.Transform(source, matrix);
		    float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;
		    if (!WithinEpsilon(a, 1f))
		    {
		        vector.X = vector.X / a;
		        vector.Y = vector.Y / a;
		        vector.Z = vector.Z / a;
		    }
		    return vector;

        }
		
		private static bool WithinEpsilon(float a, float b)
		{
		    float num = a - b;
		    return ((-1.401298E-45f <= num) && (num <= float.Epsilon));
		}

        /// <summary>
        /// Returns a <see cref="String"/> representation of this <see cref="Viewport"/> in the format:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Width:[<see cref="Width"/>] Height:[<see cref="Height"/>] MinDepth:[<see cref="MinDepth"/>] MaxDepth:[<see cref="MaxDepth"/>]}
        /// </summary>
        /// <returns>A <see cref="String"/> representation of this <see cref="Viewport"/>.</returns>
        public override string ToString ()
	    {
	        return "{X:" + X + " Y:" + y + " Width:" + Width + " Height:" + height + " MinDepth:" + MinDepth + " MaxDepth:" + maxDepth + "}";
	    }
    }
}

