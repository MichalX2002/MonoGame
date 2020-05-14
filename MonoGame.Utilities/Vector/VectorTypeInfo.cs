using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    public class VectorTypeInfo : IEquatable<VectorTypeInfo>
    {
        private static ConcurrentDictionary<Type, VectorTypeInfo> InfoCache { get; } =
            new ConcurrentDictionary<Type, VectorTypeInfo>();

        public Type Type { get; }
        public int ElementSize { get; }
        public VectorComponentInfo ComponentInfo { get; }

        public int BitDepth => ComponentInfo.BitDepth;

        public VectorTypeInfo(Type type)
        {
            if (!typeof(IVector).IsAssignableFrom(type))
                throw new ArgumentException(
                    $"The type does not implement {nameof(IVector)}.", nameof(type));

            Type = type;
            ElementSize = Marshal.SizeOf(type);

            var vectorInstance = (IVector)Activator.CreateInstance(type);
            ComponentInfo = vectorInstance.ComponentInfo;
        }

        public bool Equals(VectorTypeInfo other)
        {
            // only compare the type as every other property is dependent on it
            return Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return obj is VectorTypeInfo info && Equals(info);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type);
        }

        public static VectorTypeInfo Get(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!InfoCache.TryGetValue(type, out var info))
            {
                info = new VectorTypeInfo(type);
                InfoCache.TryAdd(type, info);
            }
            return info;
        }

        public static VectorTypeInfo Get<TPackedVector>()
            where TPackedVector : unmanaged, IVector
        {
            return Get(typeof(TPackedVector));
        }
    }
}
