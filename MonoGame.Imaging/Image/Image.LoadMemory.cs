using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        public static Image LoadMemory(
            Type pixelType, IReadOnlyMemory memory, Rectangle rectangle, int? byteStride = null)
        {
            if (!typeof(IPixel).IsAssignableFrom(pixelType))
                throw new ArgumentException(
                    $"The type does not implement {nameof(IPixel)}.", nameof(pixelType));

            ImagingArgumentGuard.AssertNonEmptyRectangle(rectangle, nameof(rectangle));

            int pixelSize = Marshal.SizeOf(pixelType);
            int srcByteStride;
            if (byteStride.HasValue)
            {
                ImagingArgumentGuard.AssertValidByteStride(
                    pixelType, rectangle.Width, byteStride.Value, nameof(byteStride));
                srcByteStride = byteStride.Value;
            }
            else
            {
                srcByteStride = pixelSize * rectangle.Width;
            }


        }

        public static Image<TPixel> LoadMemory<TPixel>(IReadOnlyMemory<TPixel> memory)
            where TPixel : unmanaged, IPixel
        {
            public Image ToImage(IMemory memory, int width, int height, bool leaveOpen)
            {
                return _toImageDelegate.Invoke(memory, width, height, leaveOpen);
            }
        }
    }
}
