using System;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.Vector;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        // TODO: move these methods somewhere else

        private static void ConvertPixelsCore<TPixelFrom, TPixelTo>(
           ReadOnlySpan<byte> source, Span<byte> destination)
           where TPixelFrom : unmanaged, IPixel
           where TPixelTo : unmanaged, IPixel
        {
            ConvertPixelBytes<TPixelFrom, TPixelTo>(source, destination);
        }

        public static void ConvertPixels<TPixelFrom, TPixelTo>(
            ReadOnlySpan<TPixelFrom> source, Span<TPixelTo> destination)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            if (destination.Length < source.Length)
                throw new ArgumentException("Destination is too short.");

            if (typeof(TPixelFrom) == typeof(TPixelTo))
            {
                var src = MemoryMarshal.Cast<TPixelFrom, TPixelTo>(source);
                src.CopyTo(destination);
            }
            else
            {
                for (int x = 0; x < source.Length; x++)
                {
                    source[x].ToScaledVector4(out Vector4 tmp);
                    destination[x].FromScaledVector4(tmp);
                }
            }
        }

        public static void ConvertPixelBytes<TPixelFrom, TPixelTo>(
            ReadOnlySpan<byte> source, Span<byte> destination)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            var src = MemoryMarshal.Cast<byte, TPixelFrom>(source);
            var dst = MemoryMarshal.Cast<byte, TPixelTo>(destination);
            ConvertPixels(src, dst);
        }

        public static void ConvertPixelBytes(
            VectorTypeInfo sourceType, VectorTypeInfo destinationType,
            ReadOnlySpan<byte> source, Span<byte> destination)
        {
            var convertDelegate = GetConvertPixelsDelegate(sourceType, destinationType);
            convertDelegate.Invoke(source, destination);
        }
    }
}
