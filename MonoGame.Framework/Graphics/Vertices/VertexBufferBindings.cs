// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;


namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Stores the vertex buffers to be bound to the input assembler stage.
    /// </summary>
    internal sealed partial class VertexBufferBindings : VertexInputLayout
    {
        private readonly VertexBuffer[] _vertexBuffers;
        private readonly int[] _vertexOffsets;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBufferBindings" /> class.
        /// </summary>
        /// <param name="maxVertexBufferSlots">The maximum number of vertex buffer slots.</param>
        public VertexBufferBindings(int maxVertexBufferSlots)
            : base(new VertexDeclaration[maxVertexBufferSlots], new int[maxVertexBufferSlots], 0)
        {
            _vertexBuffers = new VertexBuffer[maxVertexBufferSlots];
            _vertexOffsets = new int[maxVertexBufferSlots];
        }

        /// <summary>
        /// Clears the vertex buffer slots.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the input layout was changed; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool Clear()
        {
            if (Count == 0)
                return false;

            Array.Clear(VertexDeclarations, 0, Count);
            Array.Clear(InstanceFrequencies, 0, Count);
            Array.Clear(_vertexBuffers, 0, Count);
            Array.Clear(_vertexOffsets, 0, Count);
            Count = 0;
            return true;
        }

        /// <summary>
        /// Binds the specified vertex buffer to the first input slot.
        /// </summary>
        /// <param name="vertexBuffer">The vertex buffer.</param>
        /// <param name="vertexOffset">
        /// The offset (in vertices) from the beginning of the vertex buffer to the first vertex to 
        /// use.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the input layout was changed; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool Set(VertexBuffer vertexBuffer, int vertexOffset)
        {
            Debug.Assert(vertexBuffer != null);
            Debug.Assert(0 <= vertexOffset && vertexOffset < vertexBuffer.Capacity);

            if (Count == 1
                && InstanceFrequencies[0] == 0
                && _vertexBuffers[0] == vertexBuffer
                && _vertexOffsets[0] == vertexOffset)
            {
                return false;
            }

            VertexDeclarations[0] = vertexBuffer.VertexDeclaration;
            InstanceFrequencies[0] = 0;
            _vertexBuffers[0] = vertexBuffer;
            _vertexOffsets[0] = vertexOffset;
            if (Count > 1)
            {
                Array.Clear(VertexDeclarations, 1, Count - 1);
                Array.Clear(InstanceFrequencies, 1, Count - 1);
                Array.Clear(_vertexBuffers, 1, Count - 1);
                Array.Clear(_vertexOffsets, 1, Count - 1);
            }

            Count = 1;
            return true;
        }

        /// <summary>
        /// Binds the the specified vertex buffers to the input slots.
        /// </summary>
        /// <param name="vertexBufferBindings">The vertex buffer bindings.</param>
        /// <returns>
        /// <see langword="true"/> if the input layout was changed; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool Set(ReadOnlySpan<VertexBufferBinding> vertexBufferBindings)
        {
            Debug.Assert(!vertexBufferBindings.IsEmpty);
            Debug.Assert(vertexBufferBindings.Length <= _vertexBuffers.Length);

            bool isDirty = false;
            for (int i = 0; i < vertexBufferBindings.Length; i++)
            {
                Debug.Assert(vertexBufferBindings[i].VertexBuffer != null);

                if (InstanceFrequencies[i] == vertexBufferBindings[i].InstanceFrequency
                    && _vertexBuffers[i] == vertexBufferBindings[i].VertexBuffer
                    && _vertexOffsets[i] == vertexBufferBindings[i].VertexOffset)
                {
                    continue;
                }

                VertexDeclarations[i] = vertexBufferBindings[i].VertexBuffer.VertexDeclaration;
                InstanceFrequencies[i] = vertexBufferBindings[i].InstanceFrequency;
                _vertexBuffers[i] = vertexBufferBindings[i].VertexBuffer;
                _vertexOffsets[i] = vertexBufferBindings[i].VertexOffset;
                isDirty = true;
            }

            if (Count > vertexBufferBindings.Length)
            {
                int startIndex = vertexBufferBindings.Length;
                int length = Count - startIndex;
                Array.Clear(VertexDeclarations, startIndex, length);
                Array.Clear(InstanceFrequencies, startIndex, length);
                Array.Clear(_vertexBuffers, startIndex, length);
                Array.Clear(_vertexOffsets, startIndex, length);
                isDirty = true;
            }

            Count = vertexBufferBindings.Length;
            return isDirty;
        }

        public bool Set(params VertexBufferBinding[] vertexBufferBindings)
        {
            return Set(vertexBufferBindings.AsSpan());
        }

        /// <summary>
        /// Gets vertex buffer bound to the specified input slots.
        /// </summary>
        /// <returns>The vertex buffer binding.</returns>
        public VertexBufferBinding Get(int slot)
        {
            Debug.Assert(0 <= slot && slot < Count);

            return new VertexBufferBinding(
                _vertexBuffers[slot],
                _vertexOffsets[slot],
                InstanceFrequencies[slot]);
        }

        /// <summary>
        /// Gets vertex buffers bound to the input slots.
        /// </summary>
        /// <param name="destination">Destination for the vertex buffer bindings.</param>
        public void Get(Span<VertexBufferBinding> destination)
        {
            if (destination.Length < Count)
                throw new ArgumentException("Not enough space.", nameof(destination));

            for (int i = 0; i < Count; i++)
                destination[i] = Get(i);
        }
    }
}
