// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Threading;

namespace MonoGame.Framework.Graphics
{
    public abstract partial class Texture : GraphicsResource
    {
        private static int _lastSortingKey = 0;

        public SurfaceFormat Format { get; internal set; }
        public int LevelCount { get; internal set; }

        /// <summary>
        ///   Gets a unique identifier of this texture for sorting purposes.
        /// </summary>
        /// <remarks>
        ///   The value is an implementation detail and may change between application launches or MonoGame versions.
        ///   It is only guaranteed to stay consistent during application lifetime.
        /// </remarks>
        /// <remarks>
        ///   Example; this value is used by <see cref="SpriteBatch"/> when drawing with <see cref="SpriteSortMode.Texture"/>.
        /// </remarks>
        public int SortingKey { get; } = Interlocked.Increment(ref _lastSortingKey);

        public Texture(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        internal static void ValidateSizes(int elementCount, int elementSize, int minimumByteSize)
        {
            if (elementCount * elementSize < minimumByteSize)
                throw new ArgumentException(
                    $"Given amount in bytes is {elementCount * elementSize}, " +
                    $"but at least {minimumByteSize} bytes are required.");
        }

        internal static int CalculateMipLevels(int width, int height = 0, int depth = 0)
        {
            int levels = 1;
            int size = Math.Max(Math.Max(width, height), depth);
            while (size > 1)
            {
                size /= 2;
                levels++;
            }
            return levels;
        }

        internal static void GetSizeForLevel(int width, int height, int level, out int w, out int h)
        {
            w = width;
            h = height;
            while (level > 0)
            {
                --level;
                w /= 2;
                h /= 2;
            }
            if (w == 0)
                w = 1;
            if (h == 0)
                h = 1;
        }

        internal static void GetSizeForLevel(int width, int height, int depth, int level, out int w, out int h, out int d)
        {
            w = width;
            h = height;
            d = depth;
            while (level > 0)
            {
                --level;
                w /= 2;
                h /= 2;
                d /= 2;
            }
            if (w == 0)
                w = 1;
            if (h == 0)
                h = 1;
            if (d == 0)
                d = 1;
        }

        internal int GetPitch(int width)
        {
            Debug.Assert(width > 0, "The width is negative.");

            switch (Format)
            {
                case SurfaceFormat.Dxt1:
                case SurfaceFormat.Dxt1SRgb:
                case SurfaceFormat.Dxt1a:
                case SurfaceFormat.RgbPvrtc2Bpp:
                case SurfaceFormat.RgbaPvrtc2Bpp:
                case SurfaceFormat.RgbEtc1:
                case SurfaceFormat.Rgb8Etc2:
                case SurfaceFormat.Srgb8Etc2:
                case SurfaceFormat.Rgb8A1Etc2:
                case SurfaceFormat.Srgb8A1Etc2:
                case SurfaceFormat.Dxt3:
                case SurfaceFormat.Dxt3SRgb:
                case SurfaceFormat.Dxt5:
                case SurfaceFormat.Dxt5SRgb:
                case SurfaceFormat.RgbPvrtc4Bpp:
                case SurfaceFormat.RgbaPvrtc4Bpp:
                    return (width + 3) / 4 * Format.GetSize();

                default:
                    return width * Format.GetSize();
            };
        }

        protected override void GraphicsDeviceResetting()
        {
            PlatformGraphicsDeviceResetting();
        }

        /// <summary>
        /// Releases resources held by the <see cref="Texture"/>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            PlatformDispose(disposing);

            base.Dispose(disposing);
        }
    }
}

