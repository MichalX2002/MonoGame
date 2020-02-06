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

        public PixelTypeInfo(Type type, int? bitDepth = null)
        {
            if (!typeof(IPixel).IsAssignableFrom(type))
                throw new ArgumentException(
                    $"The type does not implement {nameof(IPixel)}.", nameof(type));

            if (bitDepth.HasValue)
                CommonArgumentGuard.AssertAboveZero(bitDepth.Value, nameof(bitDepth));

            Type = type;
            ElementSize = Marshal.SizeOf(type);

            BitDepth = bitDepth ?? ((IPixel)Activator.CreateInstance(type)).BitDepth;
            if (BitDepth <= 0)
                throw new ArgumentException(
                    $"{nameof(IPixel)}.{nameof(IPixel.BitDepth)} returned an invalid value.", nameof(type));
        }

        public static PixelTypeInfo Get(Type type)
        {
            if (!InfoCache.TryGetValue(type, out var info))
            {
                info = new PixelTypeInfo(type);
                InfoCache.TryAdd(type, info);
            }
            return info;
        }
    }
}
