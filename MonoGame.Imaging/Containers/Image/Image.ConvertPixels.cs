using System;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.Vector;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        public static void ConvertPixels<TPixelFrom, TPixelTo>(
            ReadOnlySpan<TPixelFrom> source, Span<TPixelTo> destination)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            if (destination.Length < source.Length)
                throw new ArgumentException("Destination is too short.");

            // TODO: finish types

            if (typeof(TPixelFrom) == typeof(TPixelTo))
            {
                var src = MemoryMarshal.Cast<TPixelFrom, TPixelTo>(source);
                src.CopyTo(destination);
            }
            else if (typeof(TPixelTo) == typeof(Color))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, Color>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToColor();
            }
            else if (typeof(TPixelFrom) == typeof(Color))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Color>(source);
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromColor(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Rgb24))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Rgb24>(source);
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromRgb24(typedSource[x]);
            }
            else
            {
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromScaledVector4(source[x].ToScaledVector4());
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
