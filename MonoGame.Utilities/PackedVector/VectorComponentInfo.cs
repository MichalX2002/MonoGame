﻿using System;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Describes a vector and the data it stores.
    /// </summary>
    public readonly struct VectorComponentInfo
    {
        /// <summary>
        /// Gets the vector component definitions in data order.
        /// </summary>
        public ReadOnlyMemory<VectorComponent> Components { get; }

        /// <summary>
        /// Gets the sum of the all the component sizes in bits.
        /// </summary>
        public int BitDepth
        {
            get
            {
                int sum = 0;
                var span = Components.Span;
                for (int i = 0; i < span.Length; i++)
                    sum += span[i].Bits;
                return sum;
            }
        }

        /// <summary>
        /// Gets the bit size of the component with the smallest size.
        /// </summary>
        public int MinBitDepth
        {
            get
            {
                int min = int.MaxValue;
                var span = Components.Span;
                for (int i = 0; i < span.Length; i++)
                    min = Math.Min(min, span[i].Bits);
                return min;
            }
        }

        /// <summary>
        /// Gets the bit size of the component with the largest size.
        /// </summary>
        public int MaxBitDepth
        {
            get
            {
                int max = int.MinValue;
                var span = Components.Span;
                for (int i = 0; i < span.Length; i++)
                    max = Math.Max(max, span[i].Bits);
                return max;
            }
        }

        /// <summary>
        /// Constructs the <see cref="VectorComponentInfo"/> with the specified component definitions.
        /// </summary>
        /// <param name="components">The vector components in data order.</param>
        /// <exception cref="ArgumentNullException"><paramref name="components"/> is null.</exception>
        /// <exception cref="ArgumentEmptyException"><paramref name="components"/> is empty.</exception>
        public VectorComponentInfo(params VectorComponent[] components)
        {
            if (components == null) throw new ArgumentNullException(nameof(components));
            if (components.Length == 0) throw new ArgumentEmptyException(nameof(components));
            Components = (VectorComponent[])components.Clone();
        }
    }
}