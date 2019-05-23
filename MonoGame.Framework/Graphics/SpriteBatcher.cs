using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// This class handles the queueing of batch items into the GPU by creating the triangle tesselations
    /// that are used to draw the sprite textures. This class supports int.MaxValue number of sprites to be
    /// batched and will process them into short.MaxValue groups (strided by 6 for the number of vertices
    /// sent to the GPU). 
    /// </summary>
    internal unsafe class SpriteBatcher : IDisposable
    {
        /*
         * Note that this class is fundamental to high performance for SpriteBatch games. Please exercise
         * caution when making changes to this class.
         */

        private const int InitialBatchSize = 256;

        /// <summary>
        /// The maximum number of batch items that can be 
        /// drawn at once with <see cref="FlushVertexArray"/>.
        /// </summary>
        private const int MaxBatchSize = ushort.MaxValue / 4;

        private readonly GraphicsDevice _device;

        private int _quadCount;
        private Texture2D[] _textureList;
        private UnmanagedPointer<float> _sortKeyList;
        private UnmanagedPointer<SpriteQuad> _quadList;
        
        /// <summary>
        /// The index buffer values are constant and more indices are added as needed.
        /// </summary>
        private IndexBuffer _indexBuffer;

        public bool IsDisposed { get; private set; }

        public SpriteBatcher(GraphicsDevice device)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));

            _indexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, 0, BufferUsage.WriteOnly);
            _textureList = new Texture2D[InitialBatchSize];
            _sortKeyList = new UnmanagedPointer<float>(InitialBatchSize);
            _quadList = new UnmanagedPointer<SpriteQuad>(InitialBatchSize);

            EnsureCapacity(InitialBatchSize);
        }

        /// <summary>
        /// Ensures that there's enough indices and vertex buffer 
        /// capacity for the specified <paramref name="itemCount"/>
        /// (up to <see cref="MaxBatchSize"/>).
        /// </summary>
        private void EnsureCapacity(int itemCount)
        {
            _sortKeyList.Length = itemCount;
            _quadList.Length = itemCount;

            if (_textureList.Length < itemCount)
                Array.Resize(ref _textureList, itemCount);

            // 1 batch item needs 6 indices
            int newIndexCount = itemCount * 6;
            int oldIndexCount = _indexBuffer.IndexCount;
            if (newIndexCount > oldIndexCount)
            {
                // 1 batch item needs 6 indices
                IntPtr tmpMemory = Marshal.AllocHGlobal(newIndexCount * sizeof(ushort));
                try
                {
                    ushort* indexPtr = (ushort*)tmpMemory;
                    for (int i = 0; i < newIndexCount; i++, indexPtr += 6)
                    {
                        /*
                         *  TL    TR    0,1,2,3 = index offsets for vertex indices
                         *   0----1     TL,TR,BL,BR are vertex references in SpriteBatchItem.   
                         *   |   /|    
                         *   |  / |
                         *   | /  |
                         *   |/   |
                         *   2----3
                         *  BL    BR
                         */

                        // Triangle 1
                        *(indexPtr) = (ushort)(i * 4);
                        *(indexPtr + 1) = (ushort)(i * 4 + 1);
                        *(indexPtr + 2) = (ushort)(i * 4 + 2);

                        // Triangle 2
                        *(indexPtr + 3) = (ushort)(i * 4 + 1);
                        *(indexPtr + 4) = (ushort)(i * 4 + 3);
                        *(indexPtr + 5) = (ushort)(i * 4 + 2);
                    }

                    _indexBuffer.SetData(tmpMemory, newIndexCount, SetDataOptions.Discard);
                }
                finally
                {
                    Marshal.FreeHGlobal(tmpMemory);
                }
            }
        }

        public void PushQuad(Texture2D texture, in SpriteQuad quad, float sortKey)
        {
            if (_quadCount >= _quadList.Length)
            {
                int oldSize = _quadList.Length;
                int newSize = oldSize + oldSize / 2; // grow by x1.5
                newSize = (newSize + 255) & (~255); // grow in chunks of 256.

                EnsureCapacity(newSize);
            }

            _textureList[_quadCount] = texture;
            _quadList[_quadCount] = quad;
            _sortKeyList[_quadCount] = sortKey;
            _quadCount++;
        }

        /// <summary>
        /// Sorts the batch items and then groups batch drawing into maximal allowed batch sets that do not
        /// overflow the 16 bit array indices for vertices.
        /// </summary>
        /// <param name="sortMode">The type of depth sorting desired for the rendering.</param>
        /// <param name="effect">The custom effect to apply to the drawn geometry</param>
        public unsafe void DrawBatch(SpriteSortMode sortMode, Effect effect)
        {
            if (effect != null && effect.IsDisposed)
                throw new ObjectDisposedException(nameof(effect));

            // nothing to do
            if (_quadCount == 0)
                return;

            // sort the batch items
            switch (sortMode)
            {
                case SpriteSortMode.Texture:
                case SpriteSortMode.FrontToBack:
                case SpriteSortMode.BackToFront:
                    UnmanagedSort.Sort(_sortKeyList.Ptr, _quadList.Ptr, 0, _quadCount);
                    break;
            }
            
            int itemCount = _quadCount;

            // Iterate through the batches, doing clamped sets of vertices only.
            while (itemCount > 0)
            {
                int vertexCount = 0;
                Texture2D tex = null;

                int itemsToProcess = itemCount;
                if (itemsToProcess > MaxBatchSize)
                    itemsToProcess = MaxBatchSize;

                // Draw the batches
                for (int i = 0; i < itemsToProcess; i++, vertexCount += 4)
                {
                    // if the texture changed, we need to flush and bind the new texture
                    int texIndex = _quadCount - itemCount;
                    if (!ReferenceEquals(_textureList[texIndex], tex))
                    {
                        FlushVertexArray(vertexCount, effect, tex);

                        vertexCount = 0;
                        tex = _textureList[texIndex];
                        _device.Textures[0] = tex;
                    }

                    // Update our count to continue culling down large batches
                    itemCount--;
                }

                // flush the remaining vertexArray data
                FlushVertexArray(vertexCount, effect, tex);
            }
            
            unchecked
            {
                _device._graphicsMetrics._spriteCount += _quadCount;
            }
            _quadCount = 0;
        }

        /// <summary>
        /// Sends the triangle list to the graphics device. Here is where the actual drawing starts.
        /// </summary>
        /// <param name="vertexCount">The amount of vertices to draw.</param>
        /// <param name="effect">The custom effect to apply to the geometry.</param>
        /// <param name="texture">The texture to draw.</param>
        private void FlushVertexArray(int vertexCount, Effect effect, Texture texture)
        {
            if (vertexCount == 0)
                return;

            var old = _device.Indices;
            _device.Indices = _indexBuffer;

            // If the effect is not null, then apply each pass and render the geometry
            if (effect != null)
            {
                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Whatever happens in pass.Apply, make sure the texture being drawn
                    // ends up in Textures[0].
                    _device.Textures[0] = texture;

                    _device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
                        PrimitiveType.TriangleList, _quadList.SafePtr, 0, 0, vertexCount / 2);
                }
            }
            else
            {
                // If no custom effect is defined, then simply render.
                _device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
                        PrimitiveType.TriangleList, _quadList.SafePtr, 0, 0, vertexCount / 2);
            }

            _device.Indices = old;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                _quadList.Dispose();
                _sortKeyList.Dispose();
                
                IsDisposed = true;
            }
        }

        ~SpriteBatcher()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
