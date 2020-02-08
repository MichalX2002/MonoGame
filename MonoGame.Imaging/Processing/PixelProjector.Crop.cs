using System;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging.Processing
{
    public static partial class PixelProjector
    {
        #region Crop (IReadOnlyPixelRows)

        /// <summary>
        /// Creates a cropped view of the source image.
        /// </summary>
        public static IReadOnlyPixelRows Crop(
            this ReadOnlyPixelRowsContext context, Rectangle sourceRectangle)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Pixels.GetBounds() == sourceRectangle) return context;
            ImagingArgumentGuard.AssertRectangleInSource(context, sourceRectangle, nameof(sourceRectangle));

            return new CropReadOnlyRows(context.Pixels, sourceRectangle);
        }

        /// <summary>
        /// Creates a cropped view of the source image.
        /// </summary>
        public static IReadOnlyPixelRows Crop(
            this ReadOnlyPixelRowsContext context, int x, int y, int width, int height)
        {
            return Crop(context, new Rectangle(x, y, width, height));
        }

        #endregion

        #region Crop (IPixelRows)

        /// <summary>
        /// Creates a cropped view of the source image.
        /// </summary>
        public static IPixelRows Crop(this PixelRowsContext context, Rectangle sourceRectangle)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Pixels.GetBounds() == sourceRectangle) return context;
            ImagingArgumentGuard.AssertRectangleInSource(context, sourceRectangle, nameof(sourceRectangle));

            return new CropRows(context.Pixels, sourceRectangle);
        }

        /// <summary>
        /// Creates a cropped view of the source image.
        /// </summary>
        public static IPixelRows Crop(this PixelRowsContext context, int x, int y, int width, int height)
        {
            return Crop(context, new Rectangle(x, y, width, height));
        }

        #endregion

        private class CropReadOnlyRows : IReadOnlyPixelRows
        {
            protected IReadOnlyPixelRows Pixels { get; }
            public Rectangle SourceRectangle { get; }

            public int Width => SourceRectangle.Width;
            public int Height => SourceRectangle.Height;
            public int Count => Width * Height;

            public int ElementSize => Pixels.ElementSize;
            public PixelTypeInfo PixelType => Pixels.PixelType;

            public CropReadOnlyRows(IReadOnlyPixelRows pixels, Rectangle sourceRectangle)
            {
                Pixels = pixels;
                SourceRectangle = sourceRectangle;
            }

            public void GetPixelByteRow(int x, int y, Span<byte> destination)
            {
                AssertValidRange(x, y, destination.Length, "requested");

                Pixels.GetPixelByteRow(x + SourceRectangle.X, y + SourceRectangle.Y, destination);
            }

            protected void AssertValidRange(int x, int y, int count, string verb)
            {
                ArgumentGuard.AssertAtleastZero(x, nameof(x));
                if (x + count > SourceRectangle.X + Width)
                    throw new ArgumentOutOfRangeException(
                        nameof(x), $"The {verb} amount of pixels exceeds the cropped row.");

                ArgumentGuard.AssertAtleastZero(y, nameof(y));
                if (y >= Height)
                    throw new ArgumentOutOfRangeException(
                        nameof(y), $"The {verb} row is not within the cropped view.");
            }

            public void Dispose()
            {
            }
        }

        private class CropRows : CropReadOnlyRows, IPixelRows
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
    }
}