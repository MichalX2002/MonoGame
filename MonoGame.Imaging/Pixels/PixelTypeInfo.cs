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
        public VectorChannelInfo ChannelInfo { get; }

        public int BitDepth => ChannelInfo.BitDepth;

        public PixelTypeInfo(Type type)
        {
            if (!typeof(IPixel).IsAssignableFrom(type))
                throw new ArgumentException(
                    $"The type does not implement {nameof(IPixel)}.", nameof(type));

            Type = type;
            ElementSize = Marshal.SizeOf(type);

            var pixelInstance = (IPixel)Activator.CreateInstance(type);
            ChannelInfo = pixelInstance.ChannelInfo;
        }

        public static PixelTypeInfo Get(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!InfoCache.TryGetValue(type, out var info))
            {
                info = new PixelTypeInfo(type);
                InfoCache.TryAdd(type, info);
            }
            return info;
        }

        public static PixelTypeInfo Get<TPixel>()
            where TPixel : unmanaged, IPixel
        {
            return Get(typeof(TPixel));
        }
    }
}
