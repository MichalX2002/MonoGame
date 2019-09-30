using System;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Utilities.PackedVector;
using StbSharp;

namespace MonoGame.Imaging.Pixels
{
    public readonly struct ImagePixelProvider<TPixel> : IPixelProvider
        where TPixel : unmanaged, IPixel
    {
        private readonly IReadOnlyPixelRows<TPixel> _pixels;

        public int Width => _pixels.Width;
        public int Height => _pixels.Height;
        public int Components { get; }

        public ImagePixelProvider(IReadOnlyPixelRows<TPixel> pixels, int components)
        {
            _pixels = pixels;
            Components = components;
        }

        public unsafe void Fill(Span<byte> buffer, int dataOffset)
        {
            int startPixelOffset = dataOffset / Components;
            int requestedPixelCount = (int)Math.Ceiling(buffer.Length / (double)Components);

            int startY = startPixelOffset / Width;
            int startX = startY * Width - startPixelOffset;

            Span<byte> byteReadBuffer = stackalloc byte[Math.Min(sizeof(TPixel) * Width, 4096)];
            Span<TPixel> readBuffer = MemoryMarshal.Cast<byte, TPixel>(byteReadBuffer);

            byte* castTmp = stackalloc byte[4];
            var graySpan = new Span<Gray8>(castTmp, 1);
            var grayAlphaSpan = new Span<GrayAlpha16>(castTmp, 1);
            var rgbSpan = new Span<Rgb24>(castTmp, 1);
            var rgbaSpan = new Span<Color>(castTmp, 1);

            int byteOffset = 0;
            int pixelsLeft = requestedPixelCount;
            while (pixelsLeft > 0)
            {
                int lastByteOffset = byteOffset;
                int toRead = Math.Min(pixelsLeft, readBuffer.Length);
                _pixels.GetPixelRow(startX + byteOffset / Components, startY, readBuffer.Slice(0, toRead));
                
                // some for-loops in the folling cases use "toRead - 1" so
                // we can copy leftover bytes if the request length is irregular
                switch (Components)
                {
                    case 1:
                        for (int i = 0; i < toRead; i++, byteOffset++)
                        {
                            graySpan[0].FromVector4(readBuffer[i].ToVector4());
                            buffer[byteOffset] = castTmp[0];
                        }
                        goto ReadEnd;

                    case 2:
                        for (int i = 0; i < toRead - 1; i++, byteOffset += 2)
                        {
                            grayAlphaSpan[0].FromVector4(readBuffer[i].ToVector4());
                            for (int j = 0; j < 2; j++)
                                buffer[j + byteOffset] = castTmp[j];
                        }
                        break;

                    case 3:
                        for (int i = 0; i < toRead - 1; i++, byteOffset += 3)
                        {
                            rgbSpan[0].FromVector4(readBuffer[i].ToVector4());
                            for (int j = 0; j < 3; j++)
                                buffer[j + byteOffset] = castTmp[j];
                        }
                        break;

                    case 4:
                        if (typeof(TPixel) == typeof(Color))
                        {
                            fixed (TPixel* srcPtr = &MemoryMarshal.GetReference(readBuffer))
                            fixed (byte* dstPtr = &MemoryMarshal.GetReference(buffer))
                            {
                                int bytes = toRead * sizeof(TPixel);
                                Buffer.MemoryCopy(srcPtr, dstPtr + byteOffset, bytes, bytes);
                                byteOffset += bytes;
                            }
                            goto ReadEnd;
                        }
                        else
                        {
                            for (int i = 0; i < toRead - 1; i++, byteOffset += 4)
                            {
                                readBuffer[i].ToColor(ref rgbaSpan[0]);
                                for (int j = 0; j < 4; j++)
                                    buffer[j + byteOffset] = castTmp[j];
                            }
                        }
                        break;
                }

                // copy over the remaining bytes, as Fill() may
                // request less bytes than sizeof(TPixel)
                int bytesRead = byteOffset - lastByteOffset;
                int leftoverBytes = Math.Min(Components, bytesRead - toRead * sizeof(TPixel));
                for (int j = 0; j < leftoverBytes; j++)
                    buffer[j + byteOffset] = castTmp[j];
                byteOffset += leftoverBytes;

                // a case for code that copies bytes directly, not needing to copy leftovers
                ReadEnd:
                pixelsLeft -= toRead;
            }
        }

        public void Fill(Span<float> buffer, int dataOffset)
        {
            throw new NotImplementedException();
        }
    }
}