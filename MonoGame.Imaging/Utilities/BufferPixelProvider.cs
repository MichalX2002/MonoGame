using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Utilities.PackedVector;
using StbSharp;

namespace MonoGame.Imaging.Pixels
{
    public readonly struct BufferPixelProvider<TPixel> : IPixelProvider
        where TPixel : unmanaged, IPixel
    {
        private readonly IReadOnlyPixelBuffer<TPixel> _pixels;

        public int Width => _pixels.Width;
        public int Height => _pixels.Height;
        public int Components { get; }

        public BufferPixelProvider(IReadOnlyPixelBuffer<TPixel> pixels, int components)
        {
            _pixels = pixels;
            Components = components;
        }

        // TODO: add PixelConversionHelper128 for the Fill(Span<float>)

        [StructLayout(LayoutKind.Explicit)]
        unsafe ref struct PixelConversionHelper32
        {
            [FieldOffset(0)]
            public fixed byte Raw[4];

            [FieldOffset(0)]
            public Gray8 Gray;

            [FieldOffset(0)]
            public GrayAlpha16 GrayAlpha;

            [FieldOffset(0)]
            public Rgb24 Rgb;

            [FieldOffset(0)]
            public Color Rgba;
        }

        public unsafe void Fill(Span<byte> buffer, int dataOffset)
        {
            int startPixelOffset = dataOffset / Components;
            int requestedPixelCount = (int)Math.Ceiling(buffer.Length / (double)Components);

            int offsetX = startPixelOffset % Width;
            int offsetY = startPixelOffset / Width;

            var convertHelper = new PixelConversionHelper32();
            
            // each iteration is supposed to read pixels from a single row at the time
            int bufferOffset = 0;
            int pixelsLeft = requestedPixelCount;
            while (pixelsLeft > 0)
            {
                int lastByteOffset = bufferOffset;
                int toRead = Math.Min(pixelsLeft, _pixels.Width - offsetX);
                var srcRow = _pixels.GetPixelRowSpan(offsetY);
                
                // some for-loops in the following cases use "toRead - 1" so
                // we can copy leftover bytes if the request length is irregular
                int i = 0;
                switch (Components)
                {
                    case 1:
                        for (; i < toRead; i++, bufferOffset++)
                        {
                            convertHelper.Gray.FromScaledVector4(srcRow[i + offsetX].ToScaledVector4());
                            buffer[bufferOffset] = convertHelper.Raw[0];
                        }
                        goto ReadEnd;

                    // TODO: deduplicate conversion code, think about some dynamic way of
                    // creating conversion functions, maybe with some heavy LINQ expressions?

                    case 2:
                        for (; i < toRead - 1; i++, bufferOffset += sizeof(GrayAlpha16))
                        {
                            convertHelper.GrayAlpha.FromScaledVector4(srcRow[i + offsetX].ToScaledVector4());
                            for (int j = 0; j < sizeof(GrayAlpha16); j++)
                                buffer[j + bufferOffset] = convertHelper.Raw[j];
                        }
                        convertHelper.GrayAlpha.FromScaledVector4(srcRow[i + 1 + offsetX].ToScaledVector4());
                        break;

                    case 3:
                        for (; i < toRead - 1; i++, bufferOffset += sizeof(Rgb24))
                        {
                            convertHelper.Rgb.FromScaledVector4(srcRow[i + offsetX].ToScaledVector4());
                            for (int j = 0; j < sizeof(Rgb24); j++)
                                buffer[j + bufferOffset] = convertHelper.Raw[j];
                        }
                        convertHelper.Rgb.FromScaledVector4(srcRow[i + 1 + offsetX].ToScaledVector4());
                        break;

                    case 4:
                        if (typeof(TPixel) == typeof(Color))
                        {
                            fixed (TPixel* srcPtr = &MemoryMarshal.GetReference(srcRow))
                            fixed (byte* dstPtr = &MemoryMarshal.GetReference(buffer))
                            {
                                int bytes = toRead * sizeof(TPixel);
                                Buffer.MemoryCopy(
                                    srcPtr + offsetX, dstPtr + bufferOffset, bytes, bytes);
                                bufferOffset += bytes;
                            }
                            goto ReadEnd;
                        }
                        else
                        {
                            for (; i < toRead - 1; i++, bufferOffset += sizeof(Color))
                            {
                                srcRow[i + offsetX].ToColor(ref convertHelper.Rgba);
                                for (int j = 0; j < sizeof(Color); j++)
                                    buffer[j + bufferOffset] = convertHelper.Raw[j];
                            }
                            srcRow[i + 1 + offsetX].ToColor(ref convertHelper.Rgba);
                        }
                        break;
                }

                // copy over the remaining bytes,
                // as the Fill() caller may request less bytes than sizeof(TPixel)
                int bytesRead = bufferOffset - lastByteOffset;
                int leftoverBytes = Math.Min(
                    Components, toRead * sizeof(TPixel) - bytesRead);

                for (int j = 0; j < leftoverBytes; j++)
                    buffer[j + bufferOffset] = convertHelper.Raw[j];
                bufferOffset += leftoverBytes;

                // a case for code that copies bytes directly, 
                // not needing to copy leftovers
                ReadEnd:
                pixelsLeft -= toRead;

                offsetX = 0; // read from row beginning on next loop
                offsetY++; // and jump to the next row
            }
        }

        public void Fill(Span<float> buffer, int dataOffset)
        {
            throw new NotImplementedException();
        }
    }
}