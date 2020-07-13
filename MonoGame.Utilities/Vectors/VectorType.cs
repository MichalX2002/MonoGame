using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    //[Obsolete("This class will be replaced in favor of C# (9 or 10) shapes/role/type classes.")]
    public class VectorType
    {
        private static MethodInfo? CreateMethod { get; set; }

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
            return GetCache.GetOrAdd(typeof(TVector), (type) => Create<TVector>());
        }

        public static VectorType Get(Type type)
        {
            return GetCache.GetOrAdd(type, (type) =>
            {
                if (!typeof(IVector).IsAssignableFrom(type))
                    throw new ArgumentException(
                        $"The type does not implement {nameof(IVector)}.", nameof(type));

                if (CreateMethod == null)
                    CreateMethod = typeof(VectorType).GetMethod(
                        nameof(Create), BindingFlags.NonPublic | BindingFlags.Static)!;

                // We call the generic method with reflection to satisfy the unmanaged constraint.
                var genericMethod = CreateMethod.MakeGenericMethod(type);
                return (VectorType)genericMethod.Invoke(null, null)!;
            });
        }

        private static VectorType Create<TVector>()
            where TVector : unmanaged, IVector
        {
            return new VectorType(typeof(TVector), new TVector().ComponentInfo);
        }
    }
}
