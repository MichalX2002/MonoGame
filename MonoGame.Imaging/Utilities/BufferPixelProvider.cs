using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;
using StbSharp;

namespace MonoGame.Imaging.Pixels
{
    public readonly struct BufferPixelProvider : IPixelProvider
    {
        private static MethodInfo _spanCastMethod;
        private static ConcurrentDictionary<Type, Transform32Delegate> _transform32Cache;

        private delegate bool Transform32Delegate(
            ReadOnlySpan<byte> srcRow,
            int column,
            int components,
            int toRead,
            Span<byte> destination,
            ref int bufferOffset,
            ref PixelConverter32 pixelConverter);

        private readonly IReadOnlyPixelBuffer _pixels;
        private readonly Transform32Delegate _transform32;

        public int Components { get; }
        public int Width => _pixels.Width;
        public int Height => _pixels.Height;

        static BufferPixelProvider()
        {
            _spanCastMethod = typeof(MemoryMarshal).GetMember(
                nameof(MemoryMarshal.Cast),
                MemberTypes.Method,
                BindingFlags.Public | BindingFlags.Static)
                .Cast<MethodInfo>()
                .First(x => x.ReturnType.GetGenericTypeDefinition() == typeof(ReadOnlySpan<>));

            _transform32Cache = new ConcurrentDictionary<Type, Transform32Delegate>();
        }

        public BufferPixelProvider(IReadOnlyPixelBuffer pixels, int components)
        {
            _pixels = pixels;
            Components = components;

            var pixelType = pixels.PixelType.Type;
            if (!_transform32Cache.TryGetValue(pixelType, out _transform32))
            {
                _transform32 = CreateTransform<Transform32Delegate>(nameof(Transform32), pixelType);
                _transform32Cache.TryAdd(pixelType, _transform32);
            }
        }

        public unsafe void Fill(Span<byte> destination, int dataOffset)
        {
            int startPixelOffset = dataOffset / Components;
            int requestedPixelCount = (int)Math.Ceiling(destination.Length / (double)Components);

            int column = startPixelOffset % Width;
            int row = startPixelOffset / Width;

            var converter32 = new PixelConverter32();
            int pixelSize = _pixels.PixelType.ElementSize;

            // each iteration is supposed to read pixels from a single row at the time
            int bufferOffset = 0;
            int pixelsLeft = requestedPixelCount;
            while (pixelsLeft > 0)
            {
                int lastByteOffset = bufferOffset;
                int count = Math.Min(pixelsLeft, _pixels.Width - column);
                var srcByteRow = _pixels.GetPixelByteRowSpan(row);

                if (_transform32.Invoke(
                    srcByteRow, column, Components, count, destination,
                    ref bufferOffset, ref converter32))
                    goto ReadEnd;

                // copy over the remaining bytes,
                // as the Fill() caller may request less bytes than sizeof(TPixel)
                int bytesRead = bufferOffset - lastByteOffset;
                int leftoverBytes = Math.Min(Components, count * pixelSize - bytesRead);

                for (int j = 0; j < leftoverBytes; j++)
                    destination[j + bufferOffset] = converter32.Raw[j];
                bufferOffset += leftoverBytes;

                // a case for code that copies bytes directly, 
                // not needing to copy leftovers
                ReadEnd:
                pixelsLeft -= count;

                column = 0; // read from row beginning on next loop
                row++; // and jump to the next row
            }
        }

        private static unsafe bool Transform32<TPixel>(
            ReadOnlySpan<TPixel> srcRow,
            int column,
            int components,
            int count,
            Span<byte> destination,
            ref int bufferOffset,
            ref PixelConverter32 pixelConverter)
            where TPixel : unmanaged, IPixel
        {
            // some for-loops in the following cases use "toRead - 1" so
            // we can copy leftover bytes if the request length is irregular
            int i = 0;
            switch (components)
            {
                case 1:
                    for (; i < count; i++, bufferOffset++)
                    {
                        pixelConverter.Gray.FromScaledVector4(srcRow[i + column].ToScaledVector4());
                        destination[bufferOffset] = pixelConverter.Raw[0];
                    }
                    return true;

                // TODO: deduplicate conversion code, think about some dynamic way of
                // creating conversion functions, maybe with some heavy LINQ expressions?

                case 2:
                    for (; i < count - 1; i++, bufferOffset += sizeof(GrayAlpha88))
                    {
                        pixelConverter.GrayAlpha.FromScaledVector4(srcRow[i + column].ToScaledVector4());
                        for (int j = 0; j < sizeof(GrayAlpha88); j++)
                            destination[j + bufferOffset] = pixelConverter.Raw[j];
                    }
                    pixelConverter.GrayAlpha.FromScaledVector4(srcRow[i + 1 + column].ToScaledVector4());
                    return false;

                case 3:
                    for (; i < count - 1; i++, bufferOffset += sizeof(Rgb24))
                    {
                        pixelConverter.Rgb.FromScaledVector4(srcRow[i + column].ToScaledVector4());
                        for (int j = 0; j < sizeof(Rgb24); j++)
                            destination[j + bufferOffset] = pixelConverter.Raw[j];
                    }
                    pixelConverter.Rgb.FromScaledVector4(srcRow[i + 1 + column].ToScaledVector4());
                    return false;

                case 4:
                    if (typeof(TPixel) == typeof(Color))
                    {
                        fixed (TPixel* srcPtr = &MemoryMarshal.GetReference(srcRow))
                        fixed (byte* dstPtr = &MemoryMarshal.GetReference(destination))
                        {
                            int bytes = count * sizeof(TPixel);
                            int byteOffsetX = column * sizeof(TPixel);
                            Buffer.MemoryCopy(srcPtr + byteOffsetX, dstPtr + bufferOffset, bytes, bytes);
                            bufferOffset += bytes;
                        }
                        return true;
                    }
                    else
                    {
                        for (; i < count - 1; i++, bufferOffset += sizeof(Color))
                        {
                            srcRow[i + column].ToColor(ref pixelConverter.Rgba);
                            for (int j = 0; j < sizeof(Color); j++)
                                destination[j + bufferOffset] = pixelConverter.Raw[j];
                        }
                        srcRow[i + 1 + column].ToColor(ref pixelConverter.Rgba);
                    }
                    return false;
            }

            throw new InvalidOperationException();
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
            public GrayAlpha88 GrayAlpha;

            [FieldOffset(0)]
            public Rgb24 Rgb;

            [FieldOffset(0)]
            public Color Rgba;
        }

        private static TDelegate CreateTransform<TDelegate>(
            string transformMethodName, Type pixelType)
            where TDelegate : Delegate
        {
            var transformMethod = typeof(BufferPixelProvider).GetMethod(
                transformMethodName, BindingFlags.NonPublic | BindingFlags.Static);

            var methodParams = transformMethod.GetParameters();
            var delegateParams = typeof(TDelegate).GetDelegateParameters();

            var lambdaParams = new List<ParameterExpression>(
                delegateParams.Select(x => Expression.Parameter(x.ParameterType)));

            var genericSpanCastMethod = _spanCastMethod.MakeGenericMethod(typeof(byte), pixelType);
            var pixelSpan = Expression.Call(genericSpanCastMethod, lambdaParams[0]);

            var callParams = new List<Expression>();
            callParams.Add(pixelSpan);
            callParams.AddRange(lambdaParams.Skip(1));

            var genericTransformMethod = transformMethod.MakeGenericMethod(pixelType);
            var call = Expression.Call(genericTransformMethod, callParams);
            var lambda = Expression.Lambda<TDelegate>(call, lambdaParams);
            return lambda.Compile();
        }

        #endregion
    }
}