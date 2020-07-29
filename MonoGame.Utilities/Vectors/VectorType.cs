using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    public class VectorType
    {
        private static ConcurrentDictionary<Type, VectorType> GetCache { get; } =
            new ConcurrentDictionary<Type, VectorType>();

        public Type Type { get; }
        public int ElementSize { get; }
        public VectorComponentInfo ComponentInfo { get; }

        public int BitDepth => ComponentInfo.BitDepth;

        public VectorType(Type type, VectorComponentInfo componentInfo)
        {
            Type = type;
            ElementSize = Marshal.SizeOf(type);
            ComponentInfo = componentInfo;
        }

        public static VectorType Get<TVector>()
            where TVector : unmanaged, IVector
        {
            // Using a static generic class should allow the JIT to devirtualize the getter,
            // granting performance equal to accessing a static field 
            // instead of a (expensive in comparison) dictionary lookup when using a Type.
            return Helper<TVector>.Instance;
        }

        public static VectorType Get(Type vectorType)
        {
            static VectorType ValueFactory(Type type)
            {
                // MakeGenericType validates generic constraints.
                var helper = typeof(Helper<>).MakeGenericType(type);
                var property = helper.GetProperties()[0];
                return (VectorType)property.GetValue(null)!;
            }
            return GetCache.GetOrAdd(vectorType, ValueFactory);
        }

        private static class Helper<TVector>
            where TVector : unmanaged, IVector
        {
            public static VectorType Instance { get; } =
                new VectorType(typeof(TVector), new TVector().ComponentInfo);
        }
    }
}
