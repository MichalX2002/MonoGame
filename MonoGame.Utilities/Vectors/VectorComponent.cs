using System;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Describes a single vector component.
    /// </summary>
    public struct VectorComponent : IEquatable<VectorComponent>
    {
        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        public VectorComponentType Type { get; }

        /// <summary>
        /// Gets the channel of the component.
        /// </summary>
        public VectorComponentChannel Channel { get; }

        /// <summary>
        /// Gets the size of the component in bits.
        /// </summary>
        public int Bits { get; }

        /// <summary>
        /// Constructs the <see cref="VectorComponent"/>.
        /// </summary>
        /// <param name="type">The type of data that will be stored in the component.</param>
        /// <param name="channel">The channel type that the component represents.</param>
        /// <param name="bits">The size of the component in bits.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bits"/> is less or equal to zero.
        /// </exception>
        public VectorComponent(VectorComponentType type, VectorComponentChannel channel, int bits = 0)
        {
            if (bits < 0)
                throw new ArgumentOutOfRangeException(nameof(bits));

            if (bits == 0 && type != VectorComponentType.Undefined)
            {
                if (type == VectorComponentType.BitField)
                    throw new ArgumentException(
                        $"The bit count may not be zero if the component type is of " +
                        $"{VectorComponentType.BitField}.", nameof(bits));

                bits = type.SizeInBytes() * 8;
            }

            Type = type;
            Channel = channel;
            Bits = bits;
        }

        public bool Equals(VectorComponent other)
        {
            return Type == other.Type
                && Channel == other.Channel
                && Bits == other.Bits;
        }

        public override bool Equals(object obj)
        {
            return obj is VectorComponent comp && Equals(comp);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Channel, Bits);
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
