﻿using System;
using MonoGame.Framework;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public static partial class PixelRowsProjector
    {
        #region Crop (IReadOnlyPixelRows)

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IReadOnlyPixelRows Crop(
            this IReadOnlyPixelRowsContext context, Rectangle sourceRectangle)
        {
            if (CheckBounds(context, sourceRectangle))
                return context;

            return new ReadOnlyCropRows(context.Pixels, sourceRectangle);
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IReadOnlyPixelRows Crop(
            this IReadOnlyPixelRowsContext context, int x, int y, int width, int height)
        {
            return Crop(context, new Rectangle(x, y, width, height));
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IReadOnlyPixelRows Crop(
            this IReadOnlyPixelRowsContext context, int x, int y)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return Crop(context, new Rectangle(x, y, context.Width - x, context.Height - y));
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IReadOnlyPixelRows<TPixel> Crop<TPixel>(
            this IReadOnlyPixelRowsContext<TPixel> context, Rectangle sourceRectangle)
            where TPixel : unmanaged, IPixel
        {
            if (CheckBounds(context, sourceRectangle))
                return context;

            return new ReadOnlyCropRows<TPixel>(context.Pixels, sourceRectangle);
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IReadOnlyPixelRows<TPixel> Crop<TPixel>(
            this IReadOnlyPixelRowsContext<TPixel> context, int x, int y, int width, int height)
            where TPixel : unmanaged, IPixel
        {
            return Crop(context, new Rectangle(x, y, width, height));
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IReadOnlyPixelRows<TPixel> Crop<TPixel>(
            this IReadOnlyPixelRowsContext<TPixel> context, int x, int y)
            where TPixel : unmanaged, IPixel
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return Crop(context, new Rectangle(x, y, context.Width - x, context.Height - y));
        }

        #endregion

        #region Crop (IPixelRows)

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IPixelRows Crop(this IPixelRowsContext context, Rectangle sourceRectangle)
        {
            return new CropRows(context?.Pixels, sourceRectangle);
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IPixelRows Crop(this IPixelRowsContext context, int x, int y, int width, int height)
        {
            return Crop(context, new Rectangle(x, y, width, height));
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IPixelRows Crop(this IPixelRowsContext context, int x, int y)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return Crop(context, new Rectangle(x, y, context.Width - x, context.Height - y));
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IPixelRows<TPixel> Crop<TPixel>(
            this IPixelRowsContext<TPixel> context, Rectangle sourceRectangle)
            where TPixel : unmanaged, IPixel
        {
            if (CheckBounds(context, sourceRectangle))
                return context;

            return new CropRows<TPixel>(context.Pixels, sourceRectangle);
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IPixelRows<TPixel> Crop<TPixel>(
            this IPixelRowsContext<TPixel> context, int x, int y, int width, int height)
            where TPixel : unmanaged, IPixel
        {
            return Crop(context, new Rectangle(x, y, width, height));
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IPixelRows<TPixel> Crop<TPixel>(
            this IPixelRowsContext<TPixel> context, int x, int y)
            where TPixel : unmanaged, IPixel
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return Crop(context, new Rectangle(x, y, context.Width - x, context.Height - y));
        }

        #endregion

        #region ReadOnlyCropRows Implementation

        private class ReadOnlyCropRows : IReadOnlyPixelRows
        {
            protected IReadOnlyPixelRows Pixels { get; }
            public Rectangle SourceRectangle { get; }

            public int Length => Width * Height;
            public int ElementSize => Pixels.ElementSize;

            public int Width => SourceRectangle.Width;
            public int Height => SourceRectangle.Height;
            public VectorType PixelType => Pixels.PixelType;

            public ReadOnlyCropRows(IReadOnlyPixelRows pixels, Rectangle sourceRectangle)
            {
                Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
                SourceRectangle = sourceRectangle;
            }

            public void GetPixelByteRow(int x, int y, Span<byte> destination)
            {
                AssertValidRange(x, y, destination.Length, "requested");

                Pixels.GetPixelByteRow(x + SourceRectangle.X, y + SourceRectangle.Y, destination);
            }

            protected void AssertValidRange(int x, int y, int count, string verb)
            {
                if ((uint)(x + count) > (uint)(SourceRectangle.X + SourceRectangle.Width))
                    throw new ArgumentOutOfRangeException(
                        nameof(x), $"The {verb} amount of pixels exceeds the cropped row.");

                if ((uint)y >= (uint)SourceRectangle.Height)
                    throw new ArgumentOutOfRangeException(
                        nameof(y), $"The {verb} row is not within the cropped view.");
            }

            public void Dispose()
            {
            }
        }

        private class ReadOnlyCropRows<TPixel> : ReadOnlyCropRows, IReadOnlyPixelRows<TPixel>
            where TPixel : unmanaged, IPixel
        {
            private new IReadOnlyPixelRows<TPixel> Pixels => (IReadOnlyPixelRows<TPixel>)base.Pixels;

            public ReadOnlyCropRows(IReadOnlyPixelRows<TPixel> rows, Rectangle sourceRectangle) : base(rows, sourceRectangle)
            {
            }

            public void GetPixelRow(int x, int y, Span<TPixel> destination)
            {
                AssertValidRange(x, y, destination.Length, "requested");

                Pixels.GetPixelRow(x + SourceRectangle.X, y + SourceRectangle.Y, destination);
            }
        }

        #endregion

        #region CropRows Implementation

        private class CropRows : ReadOnlyCropRows, IPixelRows
        {
            private new IPixelRows Pixels => (IPixelRows)base.Pixels;

            public CropRows(IPixelRows rows, Rectangle sourceRectangle) : base(rows, sourceRectangle)
            {
            }

            public void SetPixelByteRow(int x, int y, ReadOnlySpan<byte> data)
            {
                AssertValidRange(x, y, data.Length, "given");

                Pixels.SetPixelByteRow(x + SourceRectangle.X, y + SourceRectangle.Y, data);
            }
        }

        private class CropRows<TPixel> : CropRows, IPixelRows<TPixel>
            where TPixel : unmanaged, IPixel
        {
            private new IPixelRows<TPixel> Pixels => (IPixelRows<TPixel>)base.Pixels;

            public CropRows(IPixelRows<TPixel> rows, Rectangle sourceRectangle) : base(rows, sourceRectangle)
            {
            }

            public void GetPixelRow(int x, int y, Span<TPixel> destination)
            {
                AssertValidRange(x, y, destination.Length, "requested");

                Pixels.GetPixelRow(x + SourceRectangle.X, y + SourceRectangle.Y, destination);
            }

            public void SetPixelRow(int x, int y, ReadOnlySpan<TPixel> data)
            {
                AssertValidRange(x, y, data.Length, "given");

                Pixels.SetPixelRow(x + SourceRectangle.X, y + SourceRectangle.Y, data);
            }
        }

        #endregion
    }
}