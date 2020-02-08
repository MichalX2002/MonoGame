using System;
using MonoGame.Framework;
using MonoGame.Imaging.Attributes;
using MonoGame.Imaging.Attributes.Format;
using MonoGame.Imaging.Coding;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.PackedVector;
using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    public static class ImagingArgumentGuard
    {
        public static unsafe void AssertValidByteStride(
            Type elementType, int width, int byteStride, string paramName)
        {
            ArgumentGuard.AssertAboveZero(width, nameof(width));
            
            if (byteStride < width * Marshal.SizeOf(elementType))
                throw new ArgumentException(
                    "The stride must be equal to or greater than the row size in bytes (excluding padding).", 
                    paramName);
        }

        public static void AssertNonEmptyRectangle(Rectangle rectangle, string paramName)
        {
            ArgumentGuard.AssertAtleastZero(rectangle.X, paramName + ".X");
            ArgumentGuard.AssertAtleastZero(rectangle.Y, paramName + ".Y");
            ArgumentGuard.AssertAboveZero(rectangle.Width, paramName + ".Width");
            ArgumentGuard.AssertAboveZero(rectangle.Height, paramName + ".Height");
        }

        public static void AssertContigousLargeEnough(int sourceLength, int pixelCount, string paramName)
        {
            if (sourceLength < pixelCount)
                throw new ArgumentException(
                    "The contigous memory is not large enough for the given dimensions.", paramName);
        }

        public static void AssertRectangleInSource(
            IPixelSource source, Rectangle rectangle, string rectParamName)
        {
            // Rectangle.Contains would suffice, but exception details would suffer
            AssertNonEmptyRectangle(rectangle, rectParamName);

            if (rectangle.Width > source.Width)
                throw new ArgumentOutOfRangeException(rectParamName + ".Width");
            if (rectangle.Height > source.Height)
                throw new ArgumentOutOfRangeException(rectParamName + ".Height");

            if (rectangle.X + rectangle.Width > source.Width)
                throw new ArgumentOutOfRangeException(rectParamName + ".X");
            if (rectangle.Y + rectangle.Height > source.Height)
                throw new ArgumentOutOfRangeException(rectParamName + ".Y");
        }

        public static void AssertAnimationSupport(ImageFormat format, ImagingConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (config.ShouldThrowOnException<AnimationNotSupportedException>())
                if (!format.HasAttribute<IAnimatedFormatAttribute>())
                    throw new AnimationNotSupportedException(format);
        }

        public static void AssertAnimationSupport(IImageCoder coder, ImagingConfig config)
        {
            AssertAnimationSupport(coder.Format, config);

            if (config.ShouldThrowOnException<AnimationNotImplementedException>())
                if (!coder.HasAttribute<IAnimatedFormatAttribute>())
                    throw new AnimationNotImplementedException(coder);
        }
    }
}