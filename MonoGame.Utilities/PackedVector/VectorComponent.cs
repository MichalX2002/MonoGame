using System;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Describes a single vector component.
    /// </summary>
    public struct VectorComponent : IEquatable<VectorComponent>
    {
        /// <summary>
        /// Gets the data type of the component.
        /// </summary>
        public VectorComponentType Type { get; }

        /// <summary>
        /// Gets the size of the component in bits.
        /// </summary>
        public int Bits { get; }

        /// <summary>
        /// Constructs the <see cref="VectorComponent"/>.
        /// </summary>
        /// <param name="type">The type of data that will be stored in the component.</param>
        /// <param name="bits">The size of the component in bits.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bits"/> is less or equal to zero.
        /// </exception>
        public VectorComponent(VectorComponentType type, int bits)
        {
            ArgumentGuard.AssertGreaterThanZero(bits, nameof(bits));

            Type = type;
            Bits = bits;
        }

        public bool Equals(VectorComponent other)
        {
            return Type == other.Type
                && Bits == other.Bits;
        }

        public override bool Equals(object obj)
        {
            return obj is VectorComponent comp && Equals(comp);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Bits);
        }

        public static bool operator ==(VectorComponent a, VectorComponent b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(VectorComponent a, VectorComponent b)
        {
            return !(a == b);
        }
    }
}
