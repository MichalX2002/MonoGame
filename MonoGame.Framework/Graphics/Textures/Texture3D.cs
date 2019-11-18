// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Graphics
{
    public partial class Texture3D : Texture
    {
        public int Width { get; }
        public int Height { get; }
        public int Depth { get; }

        #region Constructors

        public Texture3D(
            GraphicsDevice graphicsDevice, int width, int height, int depth, bool mipMap, SurfaceFormat format)
            : this(graphicsDevice, width, height, depth, mipMap, format, false)
        {
        }

        protected Texture3D(
            GraphicsDevice graphicsDevice, int width, int height, int depth,
            bool mipMap, SurfaceFormat format, bool renderTarget)
        {
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(
                nameof(graphicsDevice), FrameworkResources.ResourceCreationWhenDeviceIsNull);

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");
            if (depth <= 0)
                throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be greater than zero.");

            Width = width;
            Height = height;
            Depth = depth;
            _levelCount = 1;
            _format = format;

            PlatformConstruct(graphicsDevice, width, height, depth, mipMap, format, renderTarget);
        }

        #endregion

        #region SetData

        public void SetData<T>(
            int level, int left, int top, int right, int bottom, int front, int back, ReadOnlySpan<T> data)
            where T : unmanaged
        {
            ValidateParams(level, left, top, right, bottom, front, back, data);

            var width = right - left;
            var height = bottom - top;
            var depth = back - front;

            PlatformSetData(level, left, top, right, bottom, front, back, width, height, depth, data);
        }

        public void SetData<T>(
            int level, int left, int top, int right, int bottom, int front, int back, Span<T> data)
            where T : unmanaged
        {
            SetData(level, left, top, right, bottom, front, back, (ReadOnlySpan<T>)data);
        }

        public void SetData<T>(ReadOnlySpan<T> data) where T : unmanaged
        {
            SetData(0, 0, 0, Width, Height, 0, Depth, data);
        }

        public void SetData<T>(Span<T> data) where T : unmanaged
        {
            SetData((ReadOnlySpan<T>)data);
        }

        #endregion

        #region GetData

        /// <summary>
        /// Gets a copy of 3D texture data, 
        /// specifying a mipmap level, source box, start index, and number of elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
        /// <param name="level">Mipmap level.</param>
        /// <param name="left">Position of the left side of the box on the x-axis.</param>
        /// <param name="top">Position of the top of the box on the y-axis.</param>
        /// <param name="right">Position of the right side of the box on the x-axis.</param>
        /// <param name="bottom">Position of the bottom of the box on the y-axis.</param>
        /// <param name="front">Position of the front of the box on the z-axis.</param>
        /// <param name="back">Position of the back of the box on the z-axis.</param>
        /// <param name="destination">The destination for the texture data.</param>
        public void GetData<T>(
            int level, int left, int top, int right, int bottom, int front, int back, Span<T> destination)
            where T : unmanaged
        {
            ValidateParams<T>(level, left, top, right, bottom, front, back, destination);
            PlatformGetData(level, left, top, right, bottom, front, back, destination);
        }

        /// <summary>
        /// Gets a copy of 3D texture data, specifying a start index and number of elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
        /// <param name="destination">The destination for the texture data.</param>
        public void GetData<T>(Span<T> destination) where T : unmanaged
        {
            GetData(0, 0, 0, Width, Height, 0, Depth, destination);
        }

        #endregion

        private unsafe void ValidateParams<T>(
            int level, int left, int top, int right, int bottom, int front, int back,
            ReadOnlySpan<T> data)
            where T : unmanaged
        {
            if (data.IsEmpty)
                throw new ArgumentEmptyException(nameof(data));

            var texWidth = Math.Max(Width >> level, 1);
            var texHeight = Math.Max(Height >> level, 1);
            var texDepth = Math.Max(Depth >> level, 1);
            var width = right - left;
            var height = bottom - top;
            var depth = back - front;

            if (left < 0 || top < 0 || back < 0 || right > texWidth || bottom > texHeight || front > texDepth)
                throw new ArgumentException("Area must be within texture bounds.");
            if (left >= right || top >= bottom || front >= back)
                throw new ArgumentException("Box size and box position must be greater than zero.");
            if (level < 0 || level >= LevelCount)
                throw new ArgumentException(
                    $"Level must be less than the number of levels in this texture.", nameof(level));

            var formatSize = Format.GetSize();
            if (sizeof(T) > formatSize || formatSize % sizeof(T) != 0)
                throw new ArgumentException(
                    $"{nameof(T)} is of an invalid size for the format of this texture.", nameof(T));

            int bytes = width * height * depth * formatSize;
            if (data.Length * sizeof(T) < bytes)
                throw new ArgumentException(
                    $"{nameof(data.Length)} is not the right size, " +
                    $"{nameof(data.Length)} * sizeof({nameof(T)}) is {data.Length * sizeof(T)}, " +
                    $"but data size is {bytes}.", nameof(data.Length));
        }
    }
}