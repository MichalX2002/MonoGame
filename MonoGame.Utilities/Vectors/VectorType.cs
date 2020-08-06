using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Vectors
{
    // Using a sealed generic class and overriding abstract members
    // allows the JIT to perform special optimizations.

    public abstract class VectorType
    {
        private static ConcurrentDictionary<Type, VectorType> Cache { get; } =
            new ConcurrentDictionary<Type, VectorType>();

        public VectorComponentInfo ComponentInfo { get; }

        public abstract Type Type { get; }
        public abstract int ElementSize { get; }

        public int BitDepth => ComponentInfo.BitDepth;

        public VectorType(VectorComponentInfo componentInfo)
        {
            ComponentInfo = componentInfo;
        }

        public static VectorType Get<TVector>()
            where TVector : unmanaged, IVector
        {
            // Using a static generic class should allow the JIT to devirtualize this getter,
            // granting performance equal to accessing a static field 
            // instead of a (expensive in comparison) dictionary lookup when using a Type.
            return VectorType<TVector>.Instance;
        }

        public static VectorType Get(Type vectorType)
        {
            static VectorType ValueFactory(Type type)
            {
                // MakeGenericType validates generic constraints.
                var helper = typeof(VectorType<>).MakeGenericType(type);
                
                var property = helper.GetProperty("Instance");
                if (property == null)
                    throw new Exception("Could not load required property for reflection.");

                return (VectorType)property.GetValue(null)!;
            }
            return Cache.GetOrAdd(vectorType, ValueFactory);
        }
    }

    /// <summary>
    /// </summary>
    /// <remarks>
    /// Marked as sealed to allow for JIT devirtualization.
    /// </remarks>
    internal sealed class VectorType<TVector> : VectorType
        where TVector : unmanaged, IVector
    {
        public static VectorType<TVector> Instance { get; } = new VectorType<TVector>();

        public override Type Type => typeof(TVector);
        public override int ElementSize => Unsafe.SizeOf<TVector>();

        public VectorType() : base(new TVector().ComponentInfo)
        {
        }
    }
}
