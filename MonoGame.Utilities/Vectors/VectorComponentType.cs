
namespace MonoGame.Framework.Vectors
{
    public enum VectorComponentType
    {
        Undefined,

        /// <summary>
        /// Represents a unsigned bit field component that has a specialized encoding.
        /// </summary>
        BitField,

        /// <summary>
        /// Represents a signed bit field component that has a specialized encoding.
        /// </summary>
        SignedBitField,

        Int8,
        UInt8,

        Int16,
        UInt16,

        Int32,
        UInt32,

        Int64,
        UInt64,

        Float16,
        Float32,
        Float64,
    }
}
