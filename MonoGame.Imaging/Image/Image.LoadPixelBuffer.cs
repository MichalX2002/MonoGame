using System;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging
{
    public static partial class Image
    {
        #region LoadPixelBuffer<TPixelFrom, TPixelTo>

        // TODO: optimize by replacing Span<>.CopyTo with faster memcpy
        public static Image<TPixelTo> LoadPixelBuffer<TPixelFrom, TPixelTo>(
            IReadOnlyPixelBuffer<TPixelFrom> pixels, Rectangle sourceRectangle, ImagingConfig config)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            if (pixels == null) throw new ArgumentEmptyException(nameof(pixels));
            if (config == null) throw new ArgumentNullException(nameof(config));
            ImagingArgumentGuard.AssertNonEmptyRectangle(sourceRectangle, nameof(sourceRectangle));

            var image = new Image<TPixelTo>(sourceRectangle.Width, sourceRectangle.Height);
            if (pixels is IReadOnlyPixelMemory<TPixelTo> typeEqualMemory)
            {
                if (!typeEqualMemory.HasPadding() &&
                    sourceRectangle.Position == Point.Zero &&
                    sourceRectangle.Width == pixels.Width &&
                    sourceRectangle.Height == pixels.Height)
                {
                    typeEqualMemory.GetPixelSpan().CopyTo(image.GetPixelSpan());
                }
                else
                {
                    for (int y = 0; y < sourceRectangle.Height; y++)
                    {
                        var srcRow = typeEqualMemory.GetPixelRowSpan(sourceRectangle.Y + y);
                        srcRow.Slice(sourceRectangle.X, sourceRectangle.Width).CopyTo(image.GetPixelRowSpan(y));
                    }
                }
            }
            else if (pixels is IReadOnlyPixelBuffer<TPixelTo> typeEqualBuffer)
            {
                for (int y = 0; y < sourceRectangle.Height; y++)
                {
                    var srcRow = typeEqualBuffer.GetPixelRowSpan(sourceRectangle.Y + y);
                    srcRow.Slice(sourceRectangle.X, sourceRectangle.Width).CopyTo(image.GetPixelRowSpan(y));
                }
            }
            else
            {
                for (int y = 0; y < sourceRectangle.Height; y++)
                {
                    var srcRow = pixels.GetPixelRowSpan(sourceRectangle.Y + y);
                    var dstRow = image.GetPixelRowSpan(y);
                    for (int x = 0; x < sourceRectangle.Width; x++)
                        dstRow[x].FromScaledVector4(srcRow[x + sourceRectangle.X].ToScaledVector4());
                }
            }
            return image;
        }

        #endregion

        #region LoadPixelBuffer<TPixelFrom, TPixelTo> (Overloads) 


        public static Image<TPixelTo> LoadPixelBuffer<TPixelFrom, TPixelTo>(
            IReadOnlyPixelBuffer<TPixelFrom> pixels, Rectangle sourceRectangle)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixelBuffer<TPixelFrom, TPixelTo>(pixels, sourceRectangle, ImagingConfig.Default);
        }

        public static Image<TPixelTo> LoadPixelBuffer<TPixelFrom, TPixelTo>(
            IReadOnlyPixelBuffer<TPixelFrom> pixels)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixelBuffer<TPixelFrom, TPixelTo>(pixels, pixels.GetBounds());
        }

        #endregion

       #region LoadPixelBuffer<TPixel> (Overloads)

        public static Image<TPixel> LoadPixelBuffer<TPixel>(
            IReadOnlyPixelBuffer<TPixel> pixels, Rectangle sourceRectangle, ImagingConfig config)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixelBuffer<TPixel, TPixel>(pixels, sourceRectangle, config);
        }

        public static Image<TPixel> LoadPixelBuffer<TPixel>(
            IReadOnlyPixelBuffer<TPixel> pixels, Rectangle sourceRectangle)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixelBuffer(pixels, sourceRectangle, ImagingConfig.Default);
        }

        public static Image<TPixel> LoadPixelBuffer<TPixel>(
            IReadOnlyPixelBuffer<TPixel> pixels)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixelBuffer(pixels, pixels.GetBounds());
        }

        #endregion
    }
}
