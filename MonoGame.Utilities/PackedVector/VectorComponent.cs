using System;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Describes a single vector component.
    /// </summary>
    public struct VectorComponent
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
    }
}
