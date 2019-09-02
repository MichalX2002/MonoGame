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

                switch (Components)
                {
                    case 1:
                        for (int i = 0; i < toRead; i++, byteOffset++)
                        {
                            graySpan[0].FromScaledVector4(readBuffer[i].ToScaledVector4());
                            buffer[byteOffset] = castTmp[0];
                        }
                        goto ReadEnd;

                    case 2:
                        for (int i = 0; i < toRead - 1; i++, byteOffset += Components)
                        {
                            grayAlphaSpan[0].FromScaledVector4(readBuffer[i].ToScaledVector4());
                            for (int j = 0; j < Components; j++)
                                buffer[j + byteOffset] = castTmp[j];
                        }
                        break;

                    case 3:
                        for (int i = 0; i < toRead - 1; i++, byteOffset += Components)
                        {
                            rgbSpan[0].FromScaledVector4(readBuffer[i].ToScaledVector4());
                            for (int j = 0; j < Components; j++)
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
                                byte* dst = dstPtr + byteOffset;
                                Buffer.MemoryCopy((byte*)srcPtr, dst, bytes, bytes);
                                byteOffset += bytes;
                            }
                            goto ReadEnd;
                        }
                        else
                        {
                            for (int i = 0; i < toRead - 1; i++, byteOffset += Components)
                            {
                                readBuffer[i].ToColor(ref rgbaSpan[0]);
                                for (int j = 0; j < Components; j++)
                                    buffer[j + byteOffset] = castTmp[j];
                            }
                        }
                        break;
                }

                // copy over the remaining bytes
                int bytesRead = byteOffset - lastByteOffset;
                int leftover = Math.Min(Components, bytesRead - toRead * sizeof(TPixel));
                for (int j = 0; j < leftover; j++)
                    buffer[j + byteOffset] = castTmp[j];

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