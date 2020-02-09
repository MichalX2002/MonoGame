using System;

namespace MonoGame.Framework
{
    public readonly struct VectorChannelInfo
    {
        /// <summary>
        /// Gets the size of the vector in bits.
        /// </summary>
        public int BitDepth { get; }

        /// <summary>
        /// Gets the smallets bit depth of all the vector channels.
        /// </summary>
        public int MinBitDepth => throw new NotImplementedException();

        /// <summary>
        /// Gets the largest bit depth of all the vector channels.
        /// </summary>
        public int MaxBitDepth => throw new NotImplementedException();

        public VectorChannelInfo(int bitDepth)
        {
            ArgumentGuard.AssertGreaterThanZero(bitDepth, nameof(bitDepth));

            BitDepth = bitDepth;
        }
    }
}
