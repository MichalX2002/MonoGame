using System;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.Vectors;

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

            if (typeof(TPixelFrom) == typeof(TPixelTo))
            {
                var src = MemoryMarshal.Cast<TPixelFrom, TPixelTo>(source);
                src.CopyTo(destination);
            }
            else if (typeof(TPixelFrom) == typeof(Alpha8))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Alpha8>(source);
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromAlpha(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Alpha16))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Alpha16>(source);
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromAlpha(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(AlphaF))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, AlphaF>(source);
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromAlpha(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Gray8))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Gray8>(source);
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromGray(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Gray16))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Gray16>(source);
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromGray(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(GrayAlpha16))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, GrayAlpha16>(source);
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromGrayAlpha(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(GrayF))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, GrayF>(source);
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromGray(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Rgb24))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Rgb24>(source);
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromRgb(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Color))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Color>(source);
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromRgba(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Rgb48))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Rgb48>(source);
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromRgb(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Rgba64))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Rgba64>(source);
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromRgba(typedSource[x]);
            }
            else if (typeof(TPixelTo) == typeof(Gray8))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, Gray8>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToGray8();
            }
            else if (typeof(TPixelTo) == typeof(Gray16))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, Gray16>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToGray16();
            }
            else if (typeof(TPixelTo) == typeof(GrayF))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, GrayF>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToGrayF();
            }
            else if (typeof(TPixelTo) == typeof(GrayAlpha16))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, GrayAlpha16>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToGrayAlpha16();
            }
            else if (typeof(TPixelTo) == typeof(Color))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, Color>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToRgba32();
            }
            else
            {
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromScaledVector(source[x].ToScaledVector4());
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
            VectorType sourceType, VectorType destinationType,
            ReadOnlySpan<byte> source, Span<byte> destination)
        {
            var convertDelegate = GetConvertPixelsDelegate(sourceType, destinationType);
            convertDelegate.Invoke(source, destination);
        }
    }
}
