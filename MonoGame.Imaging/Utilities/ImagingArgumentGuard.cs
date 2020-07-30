using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Imaging.Attributes.Format;
using MonoGame.Imaging.Coders;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    public static class ImagingArgumentGuard
    {
        public static unsafe void AssertValidByteStride(
            Type elementType, int width, int byteStride, string paramName)
        {
            ArgumentGuard.AssertGreaterThanZero(width, nameof(width));

            if (byteStride < width * Marshal.SizeOf(elementType))
                throw new ArgumentException(
                    "The stride must be equal to or greater than the row size in bytes (excluding padding).",
                    paramName);
        }

        public static void AssertNonEmptyRectangle(Rectangle rectangle, string paramName)
        {
            ArgumentGuard.AssertAtLeastZero(rectangle.X, paramName + ".X");
            ArgumentGuard.AssertAtLeastZero(rectangle.Y, paramName + ".Y");
            ArgumentGuard.AssertGreaterThanZero(rectangle.Width, paramName + ".Width");
            ArgumentGuard.AssertGreaterThanZero(rectangle.Height, paramName + ".Height");
        }

        public static void AssertNonEmptyRectangle(Rectangle? rectangle, string paramName)
        {
            if (rectangle.HasValue)
                AssertNonEmptyRectangle(rectangle.GetValueOrDefault(), paramName);
        }

        public static void AssertContigousLargeEnough(int length, int expectedLength, string paramName)
        {
            if (length < expectedLength)
                throw new ArgumentException(
                    "The memory is not large enough for the given dimensions.", paramName);
        }

        public static void AssertRectangleInSource(
            IPixelSource source, Rectangle rectangle, string rectParamName)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            // Rectangle.Contains would suffice, but exception details would suffer
            AssertNonEmptyRectangle(rectangle, rectParamName);

            if (rectangle.Width > source.Size.Width)
                throw new ArgumentOutOfRangeException(rectParamName + ".Width");
            if (rectangle.Height > source.Size.Height)
                throw new ArgumentOutOfRangeException(rectParamName + ".Height");

            if (rectangle.X + rectangle.Width > source.Size.Width)
                throw new ArgumentOutOfRangeException(rectParamName + ".X");
            if (rectangle.Y + rectangle.Height > source.Size.Height)
                throw new ArgumentOutOfRangeException(rectParamName + ".Y");
        }

        public static void AssertAnimationSupport(ImageFormat format, ImagingConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (!format.HasAttribute<IAnimatedFormatAttribute>())
                throw new AnimationNotSupportedException(format);
        }

        public static void AssertAnimationSupport(IImageCoder coder, ImagingConfig config)
        {
            if (coder == null)
                throw new ArgumentNullException(nameof(coder));

            AssertAnimationSupport(coder.Format, config);

            if (!coder.HasAttribute<IAnimatedFormatAttribute>())
                throw new AnimationNotImplementedException(coder);
        }
    }
}