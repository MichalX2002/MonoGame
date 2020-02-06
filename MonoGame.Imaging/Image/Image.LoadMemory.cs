using System;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        public static Image LoadMemory(
            IReadOnlyMemory memory, Rectangle rectangle, PixelTypeInfo pixelType, int? byteStride = null)
        {
            if (memory == null) throw new ArgumentNullException(nameof(memory));
            if (pixelType == null) throw new ArgumentNullException(nameof(pixelType));
            ImagingArgumentGuard.AssertNonEmptyRectangle(rectangle, nameof(rectangle));

            if (pixelType.BitDepth % 8 != 0)
                throw new NotImplementedException(
                    "Only byte-aligned pixels can currently be read.");

            int pixelSize = pixelType.ElementSize;
            int srcByteStride;
            if (byteStride.HasValue)
            {
                ImagingArgumentGuard.AssertValidByteStride(
                    pixelType.Type, rectangle.Width, byteStride.Value, nameof(byteStride));
                srcByteStride = byteStride.Value;
            }
            else
            {
                srcByteStride = pixelSize * rectangle.Width;
            }


        }

        public static Image LoadMemory(
            IReadOnlyMemory memory, Size size, PixelTypeInfo pixelType, int? byteStride = null)
        {
            return LoadMemory(memory, new Rectangle(size), pixelType, byteStride);
        }

        public static Image LoadMemory(
            IReadOnlyMemory memory, int width, int height, PixelTypeInfo pixelType, int? byteStride = null)
        {
            return LoadMemory(memory, new Size(width, height), pixelType, byteStride);
        }

        public static Image<TPixel> LoadMemory<TPixel>(IReadOnlyMemory<TPixel> memory)
            where TPixel : unmanaged, IPixel
        {
            Image ToImage(IReadOnlyMemory roMemory, int width, int height, bool leaveOpen)
            {
                return _toImageDelegate.Invoke(roMemory, width, height, leaveOpen);
            }
        }
    }
}
