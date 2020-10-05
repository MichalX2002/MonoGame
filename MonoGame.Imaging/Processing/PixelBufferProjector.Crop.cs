using System;
using MonoGame.Framework;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public static partial class PixelBufferProjector
    {
        #region Crop (IReadOnlyPixelBuffer)

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IReadOnlyPixelBuffer Crop(
            this IReadOnlyPixelBufferContext context, Rectangle sourceRectangle)
        {
            if (context.CheckBounds(sourceRectangle))
                return context;

            return new ReadOnlyCropBuffer(context.Pixels, sourceRectangle);
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IReadOnlyPixelBuffer Crop(
            this IReadOnlyPixelBufferContext context, int x, int y, int width, int height)
        {
            return Crop(context, new Rectangle(x, y, width, height));
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IReadOnlyPixelBuffer Crop(
            this IReadOnlyPixelBufferContext context, int x, int y)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return Crop(context, new Rectangle(x, y, context.Width - x, context.Height - y));
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IReadOnlyPixelBuffer<TPixel> Crop<TPixel>(
            this IReadOnlyPixelBufferContext<TPixel> context, Rectangle sourceRectangle)
            where TPixel : unmanaged, IPixel
        {
            if (context.CheckBounds(sourceRectangle))
                return context;

            return new ReadOnlyCropBuffer<TPixel>(context.Pixels, sourceRectangle);
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IReadOnlyPixelBuffer<TPixel> Crop<TPixel>(
            this IReadOnlyPixelBufferContext<TPixel> context, int x, int y, int width, int height)
            where TPixel : unmanaged, IPixel
        {
            return Crop(context, new Rectangle(x, y, width, height));
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IReadOnlyPixelBuffer<TPixel> Crop<TPixel>(
            this IReadOnlyPixelBufferContext<TPixel> context, int x, int y)
            where TPixel : unmanaged, IPixel
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return Crop(context, new Rectangle(x, y, context.Width - x, context.Height - y));
        }

        #endregion

        #region Crop (IPixelBuffer)

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IPixelBuffer Crop(this IPixelBufferContext context, Rectangle sourceRectangle)
        {
            return new CropBuffer(context?.Pixels, sourceRectangle);
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IPixelBuffer Crop(this IPixelBufferContext context, int x, int y, int width, int height)
        {
            return Crop(context, new Rectangle(x, y, width, height));
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IPixelBuffer Crop(this IPixelBufferContext context, int x, int y)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return Crop(context, new Rectangle(x, y, context.Width - x, context.Height - y));
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IPixelBuffer<TPixel> Crop<TPixel>(
            this IPixelBufferContext<TPixel> context, Rectangle sourceRectangle)
            where TPixel : unmanaged, IPixel
        {
            if (context.CheckBounds(sourceRectangle))
                return context;

            return new CropBuffer<TPixel>(context.Pixels, sourceRectangle);
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IPixelBuffer<TPixel> Crop<TPixel>(
            this IPixelBufferContext<TPixel> context, int x, int y, int width, int height)
            where TPixel : unmanaged, IPixel
        {
            return Crop(context, new Rectangle(x, y, width, height));
        }

        /// <summary>
        /// Creates a cropped view of the source context.
        /// </summary>
        public static IPixelBuffer<TPixel> Crop<TPixel>(
            this IPixelBufferContext<TPixel> context, int x, int y)
            where TPixel : unmanaged, IPixel
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return Crop(context, new Rectangle(x, y, context.Width - x, context.Height - y));
        }

        #endregion

        #region ReadOnlyCropBuffer Implementation

        private class ReadOnlyCropBuffer : IReadOnlyPixelBuffer
        {
            protected IReadOnlyPixelBuffer Pixels { get; }
            public Rectangle SourceRectangle { get; }

            public int Length => Width * Height;
            public int ElementSize => Pixels.ElementSize;

            public int Width => SourceRectangle.Width;
            public int Height => SourceRectangle.Height;
            public VectorType PixelType => Pixels.PixelType;

            public ReadOnlyCropBuffer(IReadOnlyPixelBuffer pixels, Rectangle sourceRectangle)
            {
                Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
                SourceRectangle = sourceRectangle;
            }

            public void GetPixelByteRow(int x, int y, Span<byte> destination)
            {
                AssertValidRange(x, y, destination.Length, "requested");

                Pixels.GetPixelByteRow(x + SourceRectangle.X, y + SourceRectangle.Y, destination);
            }

            public ReadOnlySpan<byte> GetPixelByteRowSpan(int row)
            {
                AssertValidRange(row);

                var span = Pixels.GetPixelByteRowSpan(row + SourceRectangle.Y);
                return span.Slice(
                    SourceRectangle.X * Pixels.ElementSize,
                    SourceRectangle.Width * Pixels.ElementSize);
            }

            protected void AssertValidRange(int y)
            {
                if ((uint)y >= (uint)SourceRectangle.Height)
                    throw new ArgumentOutOfRangeException(
                        nameof(y), $"The row is not within the cropped view.");
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

        private class ReadOnlyCropBuffer<TPixel> : ReadOnlyCropBuffer, IReadOnlyPixelBuffer<TPixel>
            where TPixel : unmanaged, IPixel
        {
            private new IReadOnlyPixelBuffer<TPixel> Pixels => (IReadOnlyPixelBuffer<TPixel>)base.Pixels;

            public ReadOnlyCropBuffer(
                IReadOnlyPixelBuffer<TPixel> buffer, Rectangle sourceRectangle) : base(buffer, sourceRectangle)
            {
            }

            public void GetPixelRow(int x, int y, Span<TPixel> destination)
            {
                AssertValidRange(x, y, destination.Length, "requested");

                Pixels.GetPixelRow(x + SourceRectangle.X, y + SourceRectangle.Y, destination);
            }

            public ReadOnlySpan<TPixel> GetPixelRowSpan(int row)
            {
                AssertValidRange(row);

                var span = Pixels.GetPixelRowSpan(row + SourceRectangle.Y);
                return span.Slice(SourceRectangle.X, SourceRectangle.Width);
            }
        }

        #endregion

        #region CropBuffer Implementation

        private class CropBuffer : ReadOnlyCropBuffer, IPixelBuffer
        {
            private new IPixelBuffer Pixels => (IPixelBuffer)base.Pixels;

            public CropBuffer(
                IPixelBuffer buffer, Rectangle sourceRectangle) : base(buffer, sourceRectangle)
            {
            }

            public void SetPixelByteRow(int x, int y, ReadOnlySpan<byte> data)
            {
                AssertValidRange(x, y, data.Length, "given");

                Pixels.SetPixelByteRow(x + SourceRectangle.X, y + SourceRectangle.Y, data);
            }

            public new Span<byte> GetPixelByteRowSpan(int row)
            {
                AssertValidRange(row);

                var span = Pixels.GetPixelByteRowSpan(row + SourceRectangle.Y);
                return span.Slice(
                    SourceRectangle.X * Pixels.ElementSize,
                    SourceRectangle.Width * Pixels.ElementSize);
            }
        }

        private class CropBuffer<TPixel> : CropBuffer, IPixelBuffer<TPixel>
            where TPixel : unmanaged, IPixel
        {
            private new IPixelBuffer<TPixel> Pixels => (IPixelBuffer<TPixel>)base.Pixels;

            public CropBuffer(
                IPixelBuffer<TPixel> buffer, Rectangle sourceRectangle) : base(buffer, sourceRectangle)
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

            public Span<TPixel> GetPixelRowSpan(int row)
            {
                AssertValidRange(row);

                var span = Pixels.GetPixelRowSpan(row + SourceRectangle.Y);
                return span.Slice(SourceRectangle.X, SourceRectangle.Width);
            }

            ReadOnlySpan<TPixel> IReadOnlyPixelBuffer<TPixel>.GetPixelRowSpan(int row)
            {
                AssertValidRange(row);

                var span = Pixels.GetPixelRowSpan(row + SourceRectangle.Y);
                return span.Slice(SourceRectangle.X, SourceRectangle.Width);
            }
        }

        #endregion
    }
}