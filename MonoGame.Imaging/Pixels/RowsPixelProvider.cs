using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using MonoGame.Imaging.Utilities;
using StbSharp;

namespace MonoGame.Imaging.Pixels
{
    public readonly struct RowsPixelProvider : IPixelProvider
    {
        private static ConcurrentDictionary<VectorTypeInfo, Transform32Delegate> _transform32DelegateCache =
            new ConcurrentDictionary<VectorTypeInfo, Transform32Delegate>(VectorTypeInfoEqualityComparer.Instance);

        private delegate bool Transform32Delegate(
            ReadOnlySpan<byte> sourceRow,
            int components,
            int toRead,
            Span<byte> destination,
            ref int destinationOffset,
            ref PixelConverter32 pixelConverter);

        private readonly IReadOnlyPixelRows _pixels;
        private readonly Transform32Delegate _transform32;
        private readonly byte[] _rowBuffer; // TODO: replace with smarter buffer

        public int Components { get; }
        public int Width => _pixels.Size.Width;
        public int Height => _pixels.Size.Height;

        public RowsPixelProvider(IReadOnlyPixelRows pixels, int components)
        {
            _pixels = pixels;
            Components = components;
            _rowBuffer = new byte[pixels.Size.Width * pixels.ElementSize];

            if (!_transform32DelegateCache.TryGetValue(pixels.PixelType, out _transform32))
            {
                _transform32 = CreateTransform<Transform32Delegate>(nameof(Transform32), pixels.PixelType);
                _transform32DelegateCache.TryAdd(pixels.PixelType, _transform32);
            }
        }

        public unsafe void Fill(Span<byte> destination, int dataOffset)
        {
            int startPixelOffset = dataOffset / Components;
            int requestedPixelCount = (int)MathF.Ceiling(destination.Length / (float)Components);

            int column = startPixelOffset % Width;
            int row = startPixelOffset / Width;

            var converter32 = new PixelConverter32();
            int pixelSize = _pixels.PixelType.ElementSize;

            // each iteration is supposed to read pixels from a single row at the time
            int destinationOffset = 0;
            int pixelsLeft = requestedPixelCount;
            while (pixelsLeft > 0)
            {
                int lastByteOffset = destinationOffset;
                int count = Math.Min(pixelsLeft, _pixels.Size.Width - column);

                _pixels.GetPixelByteRow(column, row, _rowBuffer);

                if (_transform32.Invoke(
                    _rowBuffer, Components, count, destination,
                    ref destinationOffset, ref converter32))
                    goto ReadEnd;

                // copy over the remaining bytes,
                // as the Fill() caller may request less bytes than sizeof(TPixel)
                int bytesRead = destinationOffset - lastByteOffset;
                int leftoverBytes = Math.Min(Components, count * Components - bytesRead);

                for (int j = 0; j < leftoverBytes; j++)
                    destination[j + destinationOffset] = converter32.Raw[j];
                destinationOffset += leftoverBytes;

                // a case for code that copies bytes directly, 
                // not needing to copy leftovers
                ReadEnd:
                pixelsLeft -= count;

                column = 0; // read from row beginning on next loop
                row++; // and jump to the next row
            }
        }

        private static unsafe bool Transform32<TPixel>(
            ReadOnlySpan<TPixel> sourceRow,
            int components,
            int count,
            Span<byte> destination,
            ref int bufferOffset,
            ref PixelConverter32 pixelConverter)
            where TPixel : unmanaged, IPixel
        {
            // some for-loops in the following cases use "count - 1" so
            // we can copy leftover bytes if the request length is irregular
            int i = 0;
            Vector4 tmpVector;
            switch (components)
            {
                case 1:
                    for (; i < count; i++, bufferOffset++)
                    {
                        sourceRow[i].ToScaledVector4(out tmpVector);
                        pixelConverter.Gray.FromScaledVector4(tmpVector);
                        destination[bufferOffset] = pixelConverter.Raw[0];
                    }
                    return true;

                // TODO: deduplicate conversion code, think about some dynamic way of
                // creating conversion functions, maybe with some heavy LINQ expressions?

                case 2:
                    for (; i < count - 1; i++, bufferOffset += sizeof(GrayAlpha16))
                    {
                        sourceRow[i].ToScaledVector4(out tmpVector);
                        pixelConverter.GrayAlpha.FromScaledVector4(tmpVector);
                        for (int j = 0; j < sizeof(GrayAlpha16); j++)
                            destination[j + bufferOffset] = pixelConverter.Raw[j];
                    }

                    sourceRow[i].ToScaledVector4(out tmpVector);
                    pixelConverter.GrayAlpha.FromScaledVector4(tmpVector);
                    return false;

                case 3:
                    for (; i < count - 1; i++, bufferOffset += sizeof(Rgb24))
                    {
                        sourceRow[i].ToScaledVector4(out tmpVector);
                        pixelConverter.Rgb.FromScaledVector4(tmpVector);
                        for (int j = 0; j < sizeof(Rgb24); j++)
                            destination[j + bufferOffset] = pixelConverter.Raw[j];
                    }

                    sourceRow[i].ToScaledVector4(out tmpVector);
                    pixelConverter.Rgb.FromScaledVector4(tmpVector);
                    return false;

                case 4:
                    if (typeof(TPixel) == typeof(Color))
                    {
                        fixed (TPixel* srcPtr =sourceRow)
                        fixed (byte* dstPtr = destination)
                        {
                            int bytes = count * sizeof(TPixel);
                            Buffer.MemoryCopy(srcPtr, dstPtr + bufferOffset, bytes, bytes);
                            bufferOffset += bytes;
                        }
                        return true;
                    }
                    else
                    {
                        for (; i < count - 1; i++, bufferOffset += sizeof(Color))
                        {
                            sourceRow[i].ToColor(ref pixelConverter.Rgba);
                            for (int j = 0; j < sizeof(Color); j++)
                                destination[j + bufferOffset] = pixelConverter.Raw[j];
                        }
                        sourceRow[i].ToColor(ref pixelConverter.Rgba);
                        return false;
                    }

                default:
                    throw new NotSupportedException();
            }
        }

        public void Fill(Span<float> buffer, int dataOffset)
        {
            // TODO: create PixelConversionHelper128 for this

            throw new NotImplementedException();
        }

        #region Helpers

        [StructLayout(LayoutKind.Explicit)]
        private unsafe ref struct PixelConverter32
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

        private static TDelegate CreateTransform<TDelegate>(
            string transformMethodName, VectorTypeInfo pixelType)
            where TDelegate : Delegate
        {
            var transformMethod = typeof(RowsPixelProvider).GetMethod(
                transformMethodName, BindingFlags.NonPublic | BindingFlags.Static);

            var methodParams = transformMethod.GetParameters();
            var delegateParams = typeof(TDelegate).GetDelegateParameters();

            var lambdaParams = new List<ParameterExpression>(
                delegateParams.Select(x => Expression.Parameter(x.ParameterType)));

            var spanCastMethod = ImagingReflectionHelper.SpanCastMethod;
            var genericSpanCastMethod = spanCastMethod.MakeGenericMethod(typeof(byte), pixelType.Type);
            var pixelSpan = Expression.Call(genericSpanCastMethod, lambdaParams[0]);

            var callParams = new List<Expression>();
            callParams.Add(pixelSpan);
            callParams.AddRange(lambdaParams.Skip(1));

            var genericTransformMethod = transformMethod.MakeGenericMethod(pixelType.Type);
            var call = Expression.Call(genericTransformMethod, callParams);
            var lambda = Expression.Lambda<TDelegate>(call, lambdaParams);
            return lambda.Compile();
        }

        #endregion
    }
}