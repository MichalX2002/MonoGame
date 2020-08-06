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

        public bool IsValid => Bits != 0;

        /// <summary>
        /// Constructs the <see cref="VectorComponent"/>.
        /// </summary>
        /// <param name="type">The type of data that will be stored in the component.</param>
        /// <param name="channel">The channel type that the component represents.</param>
        /// <param name="bits">The size of the component in bits.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bits"/> is less than or equal to zero or 
        /// <paramref name="bits"/> is zero and <paramref name="type"/> is <see cref="VectorComponentType.BitField"/>.
        /// </exception>
        public VectorComponent(VectorComponentType type, VectorComponentChannel channel, int? bits = null)
        {
            if (bits.HasValue)
            {
                if (bits.Value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(bits));

                Bits = bits.Value;
            }
            else
            {
                if (type == VectorComponentType.BitField ||
                    type == VectorComponentType.Undefined)
                    throw new ArgumentException(
                        "The bit size could not be inferred from the given component type.");

                Bits = type.SizeInBytes() * 8;
            }

            Type = type;
            Channel = channel;
        }

        public bool Equals(VectorComponent other)
        {
            return this == other;
        }

        public override bool Equals(object? obj)
        {
            return obj is VectorComponent comp && Equals(comp);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Channel, Bits);
        }

        public override string ToString()
        {
            return Type == VectorComponentType.BitField || Type == VectorComponentType.SignedBitField
                ? $"{Channel} {Type}[{Bits}]"
                : $"{Channel} {Type}";
        }

        public static bool operator ==(VectorComponent a, VectorComponent b)
        {
            return a.Type == b.Type
                && a.Channel == b.Channel
                && a.Bits == b.Bits;
        }

        public static bool operator !=(VectorComponent a, VectorComponent b)
        {
            return !(a == b);
        }
    }
}
