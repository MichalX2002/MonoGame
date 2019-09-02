#region License
/*
MIT License
Copyright © 2006 The Mono.Xna Team

All rights reserved.

Authors:
 * Rob Loach

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System.Runtime.Serialization;

namespace MonoGame.Framework.Graphics
{
    [DataContract]
    public class DisplayMode
    {
        public SurfaceFormat Format { get; }
        public int Height { get; }
        public int Width { get; }

        public float AspectRatio => Width / (float)Height;
        public Rectangle TitleSafeArea => GraphicsDevice.GetTitleSafeArea(0, 0, Width, Height);

        internal DisplayMode(int width, int height, SurfaceFormat format)
        {
            this.Width = width;
            Height = height;
            Format = format;
        }

        #region Operators

        public static bool operator !=(DisplayMode left, DisplayMode right)
        {
            return !(left == right);
        }

        public static bool operator ==(DisplayMode left, DisplayMode right)
        {
            if (ReferenceEquals(left, right)) //Same object or both are null
                return true;

            if (left is null || right is null)
                return false;

            return (left.Format == right.Format) 
                && (left.Height == right.Height) 
                && (left.Width == right.Width);
        }

        #endregion Operators

        #region Public Methods

        public override bool Equals(object obj)
        {
            return obj is DisplayMode other && this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int code = Width.GetHashCode();
                code = code * 23 + Height.GetHashCode();
                return code * 23 + Format.GetHashCode();
            }
        }

        public override string ToString()
        {
            return "{Width:" + Width + " Height:" + Height + " Format:" + Format + " AspectRatio:" + AspectRatio + "}";
        }

        #endregion Public Methods
    }
}
