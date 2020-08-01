using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.Vectors;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        // TODO: move to PixelOperations class

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static void ConvertPixels<TPixelFrom, TPixelTo>(
            ReadOnlySpan<TPixelFrom> source, Span<TPixelTo> destination)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            if (destination.Length < source.Length)
                throw new ArgumentException("Destination is too short.");

            // TODO: check performance of unsafe
            // TODO: add more types

            // This massive check should be JITted down to the respective type.

            if (typeof(TPixelFrom) == typeof(TPixelTo))
            {
                var src = MemoryMarshal.Cast<TPixelFrom, TPixelTo>(source);
                src.CopyTo(destination);
            }
            #region From

            #region Alpha
            else if (typeof(TPixelFrom) == typeof(Alpha8))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Alpha8>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromAlpha(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Alpha16))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Alpha16>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromAlpha(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(AlphaF))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, AlphaF>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromAlpha(typedSource[x]);
            }
            #endregion

            #region Gray
            else if (typeof(TPixelFrom) == typeof(Gray8))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Gray8>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromGray(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Gray16))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Gray16>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromGray(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(GrayAlpha16))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, GrayAlpha16>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromGray(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(GrayF))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, GrayF>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromGray(typedSource[x]);
            }
            #endregion

            #region Color
            else if (typeof(TPixelFrom) == typeof(Rgb24))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Rgb24>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromColor(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Color))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Color>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromColor(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Rgb48))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Rgb48>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromColor(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Rgba64))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Rgba64>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromColor(typedSource[x]);
            }
            #endregion

            #region Arbitrary Color
            else if (typeof(TPixelFrom) == typeof(Abgr32))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Abgr32>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromColor(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Argb32))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Argb32>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromColor(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Bgr565))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Bgr565>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromColor(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Bgr24))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Bgr24>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromColor(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Bgra5551))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Bgra5551>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromColor(typedSource[x]);
            }
            else if (typeof(TPixelFrom) == typeof(Bgra32))
            {
                var typedSource = MemoryMarshal.Cast<TPixelFrom, Bgra32>(source);
                for (int x = 0; x < typedSource.Length; x++)
                    destination[x].FromColor(typedSource[x]);
            }
            #endregion

            #endregion

            #region To

            #region Alpha
            else if (typeof(TPixelTo) == typeof(Alpha8))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, Alpha8>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToAlpha8();
            }
            else if (typeof(TPixelTo) == typeof(Alpha16))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, Alpha16>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToAlpha16();
            }
            else if (typeof(TPixelTo) == typeof(AlphaF))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, AlphaF>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToAlphaF();
            }
            #endregion

            #region Gray
            else if (typeof(TPixelTo) == typeof(Gray8))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, Gray8>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToGray8();
            }
            else if (typeof(TPixelTo) == typeof(GrayAlpha16))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, GrayAlpha16>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToGrayAlpha16();
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
            #endregion

            #region Color
            else if (typeof(TPixelTo) == typeof(Rgb24))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, Rgb24>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToRgb24();
            }
            else if (typeof(TPixelTo) == typeof(Color))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, Color>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToRgba32();
            }
            else if (typeof(TPixelTo) == typeof(Rgb48))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, Rgb48>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToRgb48();
            }
            else if (typeof(TPixelTo) == typeof(Rgba64))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, Rgba64>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToRgba64();
            }
            else if (typeof(TPixelTo) == typeof(RgVector))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, RgVector>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToRgbVector().Rg;
            }
            else if (typeof(TPixelTo) == typeof(RgbVector))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, RgbVector>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToRgbVector();
            }
            else if (typeof(TPixelTo) == typeof(RgbaVector))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, RgbaVector>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x] = source[x].ToRgbaVector();
            }
            #endregion

            #region Extra
            // TODO: move to IPixel?
            else if (typeof(TPixelTo) == typeof(Red8))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, Red8>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x].FromColor(source[x].ToRgb24());
            }
            else if (typeof(TPixelTo) == typeof(Rg16))
            {
                var typedDestination = MemoryMarshal.Cast<TPixelTo, Rg16>(destination);
                for (int x = 0; x < source.Length; x++)
                    typedDestination[x].FromColor(source[x].ToRgb24());
            }
            #endregion

            #endregion
            else
            {
                for (int x = 0; x < source.Length; x++)
                    destination[x].FromScaledVector(source[x].ToScaledVector4());
            }
        }

        public static void ConvertPixels<TPixelFrom, TPixelTo>(
            Span<TPixelFrom> source, Span<TPixelTo> destination)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            ConvertPixels((ReadOnlySpan<TPixelFrom>)source, destination);
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
