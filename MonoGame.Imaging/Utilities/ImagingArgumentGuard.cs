using System;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Utilities
{
    public static class ImagingArgumentGuard
    {
        public static unsafe void AssertValidByteStride<TPixel>(
            int width, int stride, string paramName)
            where TPixel : unmanaged, IPixel
        {
            CommonArgumentGuard.AssertAboveZero(width, nameof(width));
            
            if (stride < width * sizeof(TPixel))
                throw new ArgumentException(
                    "The stride must be equal to or greater than the row size in bytes (without padding).", 
                    paramName);
        }

        public static void AssertNonEmptyRectangle(Rectangle rectangle, string paramName)
        {
            CommonArgumentGuard.AssertAtleastZero(rectangle.X, paramName + ".X");
            CommonArgumentGuard.AssertAtleastZero(rectangle.Y, paramName + ".Y");
            CommonArgumentGuard.AssertAboveZero(rectangle.Width, paramName + ".Width");
            CommonArgumentGuard.AssertAboveZero(rectangle.Height, paramName + ".Height");
        }

        public static void AssertContigousLargeEnough(int sourceLength, int pixelCount, string paramName)
        {
            if (sourceLength < pixelCount)
                throw new ArgumentException(
                    "The contigous memory is not large enough for the given dimensions.", paramName);
        }

        public static void AssertValidFrameLimit(int? frameLimit, string paramName)
        {
            if (frameLimit.HasValue)
                CommonArgumentGuard.AssertAboveZero(frameLimit.Value, paramName);
        }

        public static void AssertAnimationSupported(IImageCoder coder)
        {
            if (!coder.Format.SupportsAnimation)
                throw new ImageCoderException(coder.Format, "The image format does not support animation.");

            if (!coder.SupportsAnimation)
                throw new ImageCoderException(coder.Format, "The coder does not support animation.");
        }

        public static void AssertRectangleInSource<TPixel>(
            IPixelSource<TPixel> source, Rectangle rect, string rectParamName)
            where TPixel : unmanaged, IPixel
        {
            // Rectangle.Contains would suffice, but exception readability would suffer
            AssertNonEmptyRectangle(rect, rectParamName);

            if (rect.Width > source.Width)
                throw new ArgumentOutOfRangeException(rectParamName + ".Width");
            if (rect.Height > source.Height)
                throw new ArgumentOutOfRangeException(rectParamName + ".Height");

            if (rect.X + rect.Width > source.Width)
                throw new ArgumentOutOfRangeException(rectParamName + ".X");
            if (rect.Y + rect.Height > source.Height)
                throw new ArgumentOutOfRangeException(rectParamName + ".Y");
        }
    }
}