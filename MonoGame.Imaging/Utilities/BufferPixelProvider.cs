using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using StbSharp;

namespace MonoGame.Imaging.Pixels
{
    public readonly struct BufferPixelProvider : IPixelProvider
    {
        private readonly IReadOnlyPixelBuffer _pixels;

        public int Width => _pixels.Width;
        public int Height => _pixels.Height;
        public int Components { get; }

        public BufferPixelProvider(IReadOnlyPixelBuffer pixels, int components)
        {
            _pixels = pixels;
            Components = components;
        }

        [StructLayout(LayoutKind.Explicit)]
        unsafe ref struct PixelConverter32
        {
            [FieldOffset(0)]
            public fixed byte Raw[4];

            [FieldOffset(0)]
            public Gray8 Gray;

            [FieldOffset(0)]
            public GrayAlpha88 GrayAlpha;

            [FieldOffset(0)]
            public Rgb24 Rgb;

            [FieldOffset(0)]
            public Color Rgba;
        }

        public unsafe void Fill(Span<byte> destination, int dataOffset)
        {
            int startPixelOffset = dataOffset / Components;
            int requestedPixelCount = (int)Math.Ceiling(destination.Length / (double)Components);

            int offsetX = startPixelOffset % Width;
            int offsetY = startPixelOffset / Width;

            var convertHelper = new PixelConverter32();
            int pixelSize = _pixels.PixelType.ElementSize;

            // each iteration is supposed to read pixels from a single row at the time
            int bufferOffset = 0;
            int pixelsLeft = requestedPixelCount;
            while (pixelsLeft > 0)
            {
                int lastByteOffset = bufferOffset;
                int toRead = Math.Min(pixelsLeft, _pixels.Width - offsetX);
                var srcByteRow = _pixels.GetPixelByteRowSpan(offsetY);

                if (Transform32())
                    goto ReadEnd;

                // copy over the remaining bytes,
                // as the Fill() caller may request less bytes than sizeof(TPixel)
                int bytesRead = bufferOffset - lastByteOffset;
                int leftoverBytes = Math.Min(Components, toRead * pixelSize - bytesRead);

                for (int j = 0; j < leftoverBytes; j++)
                    destination[j + bufferOffset] = convertHelper.Raw[j];
                bufferOffset += leftoverBytes;

                // a case for code that copies bytes directly, 
                // not needing to copy leftovers
                ReadEnd:
                pixelsLeft -= toRead;

                offsetX = 0; // read from row beginning on next loop
                offsetY++; // and jump to the next row
            }
        }

        private unsafe bool Transform32<TPixel>(
            ReadOnlySpan<TPixel> srcRow,
            int toRead,
            int offsetX,
            Span<byte> destination,
            ref int bufferOffset, 
            ref PixelConverter32 pixelConverter)
            where TPixel : unmanaged, IPixel
        {
            // some for-loops in the following cases use "toRead - 1" so
            // we can copy leftover bytes if the request length is irregular
            int i = 0;
            switch (Components)
            {
                case 1:
                    for (; i < toRead; i++, bufferOffset++)
                    {
                        pixelConverter.Gray.FromScaledVector4(srcRow[i + offsetX].ToScaledVector4());
                        destination[bufferOffset] = pixelConverter.Raw[0];
                    }
                    return true;

                // TODO: deduplicate conversion code, think about some dynamic way of
                // creating conversion functions, maybe with some heavy LINQ expressions?

                case 2:
                    for (; i < toRead - 1; i++, bufferOffset += sizeof(GrayAlpha88))
                    {
                        pixelConverter.GrayAlpha.FromScaledVector4(srcRow[i + offsetX].ToScaledVector4());
                        for (int j = 0; j < sizeof(GrayAlpha88); j++)
                            destination[j + bufferOffset] = pixelConverter.Raw[j];
                    }
                    pixelConverter.GrayAlpha.FromScaledVector4(srcRow[i + 1 + offsetX].ToScaledVector4());
                    return false;

                case 3:
                    for (; i < toRead - 1; i++, bufferOffset += sizeof(Rgb24))
                    {
                        pixelConverter.Rgb.FromScaledVector4(srcRow[i + offsetX].ToScaledVector4());
                        for (int j = 0; j < sizeof(Rgb24); j++)
                            destination[j + bufferOffset] = pixelConverter.Raw[j];
                    }
                    pixelConverter.Rgb.FromScaledVector4(srcRow[i + 1 + offsetX].ToScaledVector4());
                    return false;

                case 4:
                    if (typeof(TPixel) == typeof(Color))
                    {
                        fixed (TPixel* srcPtr = &MemoryMarshal.GetReference(srcRow))
                        fixed (byte* dstPtr = &MemoryMarshal.GetReference(destination))
                        {
                            int bytes = toRead * sizeof(TPixel);
                            int byteOffsetX = offsetX * sizeof(TPixel);
                            Buffer.MemoryCopy(srcPtr + byteOffsetX, dstPtr + bufferOffset, bytes, bytes);
                            bufferOffset += bytes;
                        }
                        return true;
                    }
                    else
                    {
                        for (; i < toRead - 1; i++, bufferOffset += sizeof(Color))
                        {
                            srcRow[i + offsetX].ToColor(ref pixelConverter.Rgba);
                            for (int j = 0; j < sizeof(Color); j++)
                                destination[j + bufferOffset] = pixelConverter.Raw[j];
                        }
                        srcRow[i + 1 + offsetX].ToColor(ref pixelConverter.Rgba);
                    }
                    return false;
            }

            throw new InvalidOperationException();
        }

        public void Fill(Span<float> buffer, int dataOffset)
        {
            // TODO: add PixelConversionHelper128 for this

            throw new NotImplementedException();
        }
    }
}