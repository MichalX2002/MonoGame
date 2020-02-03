using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Pixels
{
    public class PixelTypeInfo
    {
        private static ConcurrentDictionary<Type, PixelTypeInfo> InfoCache { get; } =
            new ConcurrentDictionary<Type, PixelTypeInfo>();

        public Type Type { get; }
        public int ElementSize { get; }
        public int BitDepth { get; }

        public PixelTypeInfo(Type type, int bitDepth)
        {
            if (!typeof(IPixel).IsAssignableFrom(type))
                throw new ArgumentException(
                    $"The type does not implement {nameof(IPixel)}.", nameof(type));

            CommonArgumentGuard.AssertAboveZero(bitDepth, nameof(bitDepth));

            Type = type;
            ElementSize = Marshal.SizeOf(type);
            BitDepth = bitDepth;
        }

        public PixelTypeInfo(Type type) : this(type, Marshal.SizeOf(type) * 8)
        {
        }

        public static PixelTypeInfo Get(Type type)
        {
            if(!InfoCache.TryGetValue(type, out var info))
            {
                info = new PixelTypeInfo(type);
                InfoCache.TryAdd(type, info);
            }
            return info;
        }
    }
}
