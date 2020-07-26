// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Graphics
{
    public partial class TextureCube : Texture
    {
        /// <summary>
        /// Gets the width and height of the cube map face in pixels.
        /// </summary>
        /// <value>The width and height of a cube map face in pixels.</value>
        public int Size { get; internal set; }

        public TextureCube(
            GraphicsDevice graphicsDevice, int size, bool mipMap, SurfaceFormat format)
            : this(graphicsDevice, size, mipMap, format, false)
        {
        }

        internal TextureCube(
            GraphicsDevice graphicsDevice, int size, bool mipMap, SurfaceFormat format, bool renderTarget)
            : base(graphicsDevice)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), "Cube size must be greater than zero");

            Size = size;
            Format = format;
            LevelCount = mipMap ? CalculateMipLevels(size) : 1;

            void Construct() => PlatformConstruct(graphicsDevice, size, mipMap, format, renderTarget);
            if (IsValidThreadContext)
                Construct();
            else
                Threading.BlockOnMainThread(Construct);
        }

        #region GetData

        public void GetData<T>(
            CubeMapFace cubeMapFace, int level, Rectangle? rect, Span<T> destination)
            where T : unmanaged
        {
            AssertMainThread(true);

            ValidateParams<T>(level, rect, destination.Length, out Rectangle checkedRect);

            PlatformGetData(cubeMapFace, level, checkedRect, MemoryMarshal.AsBytes(destination));
        }

        /// <summary>
        /// Gets a copy of cube texture data specifying a cubemap face.
        /// </summary>
        /// <param name="cubeMapFace">The cube map face.</param>
        /// <param name="destination">The data destination.</param>
        public void GetData<T>(CubeMapFace cubeMapFace, Span<T> destination)
            where T : unmanaged
        {
            GetData(cubeMapFace, 0, null, destination);
        }

        #endregion

        #region SetData

        public void SetData<T>(
            CubeMapFace face, int level, Rectangle? rect, ReadOnlySpan<T> data)
            where T : unmanaged
        {
            AssertMainThread(true);

            ValidateParams<T>(level, rect, data.Length, out Rectangle checkedRect);

            PlatformSetData(face, level, checkedRect, data);
        }

        public void SetData<T>(CubeMapFace face, ReadOnlySpan<T> data)
            where T : unmanaged
        {
            SetData(face, 0, null, data);
        }

        public void SetData<T>(
            CubeMapFace face, int level, Rectangle? rect, Span<T> data)
            where T : unmanaged
        {
            SetData(face, 0, rect, (ReadOnlySpan<T>)data);
        }

        public void SetData<T>(CubeMapFace face, Span<T> data)
            where T : unmanaged
        {
            SetData(face, (ReadOnlySpan<T>)data);
        }

        #endregion

        private unsafe void ValidateParams<T>(
            int level, Rectangle? rect, int dataLength, out Rectangle checkedRect)
            where T : unmanaged
        {
            var textureBounds = new Rectangle(0, 0, Math.Max(Size >> level, 1), Math.Max(Size >> level, 1));
            checkedRect = rect ?? textureBounds;

            if (level < 0 || level >= LevelCount)
                throw new ArgumentException(
                    $"{nameof(level)} must be smaller than the number of levels in this texture.", nameof(level));

            if (!textureBounds.Contains(checkedRect) || checkedRect.Width <= 0 || checkedRect.Height <= 0)
                throw new ArgumentException("Rectangle must be inside the texture bounds", nameof(rect));

            var fSize = Format.GetSize();
            if (sizeof(T) > fSize || fSize % sizeof(T) != 0)
                throw new ArgumentException(
                    $"Type {nameof(T)} is of an invalid size for the format of this texture.", nameof(T));

            int dataByteSize;
            if (Format.IsCompressedFormat())
            {
                // round x and y down to next multiple of four; width and height up to next multiple of four
                var roundedWidth = (checkedRect.Width + 3) & ~0x3;
                var roundedHeight = (checkedRect.Height + 3) & ~0x3;
                checkedRect = new Rectangle(checkedRect.X & ~0x3, checkedRect.Y & ~0x3,
#if OPENGL
                    // OpenGL only: The last two mip levels require the width and height to be
                    // passed as 2x2 and 1x1, but there needs to be enough data passed to occupy
                    // a 4x4 block.
                    checkedRect.Width < 4 && textureBounds.Width < 4 ? textureBounds.Width : roundedWidth,
                    checkedRect.Height < 4 && textureBounds.Height < 4 ? textureBounds.Height : roundedHeight);
#else
                    roundedWidth, roundedHeight);
#endif
                dataByteSize = roundedWidth * roundedHeight * fSize / 16;
            }
            else
            {
                dataByteSize = checkedRect.Width * checkedRect.Height * fSize;
            }

            if (dataLength * sizeof(T) != dataByteSize)
                throw new ArgumentException(
                    $"Buffer length is not the right size, " +
                    $"expected {dataLength * sizeof(T)} bytes, but is {dataByteSize}.");
        }
    }
}

