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
        private SpriteBatchItem[] _batchItems;
        private UnmanagedPointer<VertexPositionColorTexture> _itemVertexBuffer;
        
        /// <summary>
        /// The index buffer values are constant and more indices are added as needed.
        /// </summary>
        private IndexBuffer _indexBuffer;

        private DynamicVertexBuffer _vertexBuffer;

        public bool IsDisposed { get; private set; }

        public SpriteBatcher(GraphicsDevice device)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));

            _indexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, 0, BufferUsage.WriteOnly);
            _vertexBuffer = new DynamicVertexBuffer(device, VertexPositionColorTexture.VertexDeclaration, 0, BufferUsage.WriteOnly);

            _itemVertexBuffer = new UnmanagedPointer<VertexPositionColorTexture>(InitialBatchSize * 4);
            _batchItems = new SpriteBatchItem[InitialBatchSize];
            for (int i = 0; i < InitialBatchSize; i++)
                _batchItems[i] = new SpriteBatchItem();

            EnsureCapacity(InitialBatchSize);
        }

        /// <summary>
        /// Ensures that there's enough indices and vertex buffer 
        /// capacity for the specified <paramref name="itemCount"/>
        /// (up to <see cref="MaxBatchSize"/>).
        /// </summary>
        private void EnsureCapacity(int itemCount)
        {
            int oldSize = _batchItems.Length;
            Array.Resize(ref _batchItems, itemCount);
            for (int i = oldSize; i < _batchItems.Length; i++)
                _batchItems[i] = new SpriteBatchItem();

            int min = Math.Min(itemCount, MaxBatchSize) * 4; // 4 vertices per item
            if (min > _itemVertexBuffer.Length)
                _itemVertexBuffer.Length = min;

            int newIndexCount = itemCount * 6; // 6 indices per item
            int oldIndexCount = _indexBuffer.IndexCount;
            if (newIndexCount > oldIndexCount)
            {
                // 1 batch item needs 6 indices
                IntPtr tmpMemory = Marshal.AllocHGlobal(newIndexCount * sizeof(ushort));
                try
                {
                    ushort* indexPtr = (ushort*)tmpMemory;
                    for (int i = 0, v = 0; i < newIndexCount; i += 6, v += 4)
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
                        indexPtr[i + 0] = (ushort)(v + 0);
                        indexPtr[i + 1] = (ushort)(v + 1);
                        indexPtr[i + 2] = (ushort)(v + 2);

                        // Triangle 2
                        indexPtr[i + 3] = (ushort)(v + 1);
                        indexPtr[i + 4] = (ushort)(v + 3);
                        indexPtr[i + 5] = (ushort)(v + 2);
                    }
                    _indexBuffer.SetData(tmpMemory, newIndexCount, SetDataOptions.Discard);
                }
                finally
                {
                    Marshal.FreeHGlobal(tmpMemory);
                }
            }
        }

        public SpriteBatchItem GetBatchItem()
        {
            if (_quadCount >= _batchItems.Length)
            {
                int oldSize = _batchItems.Length;
                int newSize = oldSize + oldSize / 2; // grow by x1.5
                newSize = (newSize + 255) & (~255); // grow in chunks of 256.
                EnsureCapacity(newSize);
            }
            return _batchItems[_quadCount++];
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
                    Array.Sort(_batchItems, 0, _quadCount);
                    break;
            }

            // iterate through the batches, doing clamped sets of vertices at the time
            VertexPositionColorTexture* quadBufferPtr = _itemVertexBuffer.Ptr;
            int itemsLeft = _quadCount;
            while (itemsLeft > 0)
            {
                int vertexCount = 0;
                Texture2D tex = null;

                int itemsToProcess = itemsLeft;
                if (itemsToProcess > MaxBatchSize)
                    itemsToProcess = MaxBatchSize;

                // draw the batches
                for (int i = 0; i < itemsToProcess; i++, vertexCount += 4)
                {
                    int offset = _quadCount - itemsLeft;
                    var item = _batchItems[offset];

                    // if the texture changed, we need to flush and bind the new texture
                    if (!ReferenceEquals(item.Texture, tex))
                    {
                        FlushVertexArray(vertexCount, effect, tex);

                        vertexCount = 0;
                        tex = item.Texture;
                        _device.Textures[0] = tex;
                    }
                    item.Texture = null; // release texture from item

                    quadBufferPtr[vertexCount + 0] = item.VertexTL;
                    quadBufferPtr[vertexCount + 1] = item.VertexTR;
                    quadBufferPtr[vertexCount + 2] = item.VertexBL;
                    quadBufferPtr[vertexCount + 3] = item.VertexBR;

                    itemsLeft--; // update our count to continue culling down large batches
                }

                // flush the remaining data
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
        /// <param name="count">The amount of vertices to draw.</param>
        /// <param name="effect">The custom effect to apply to the geometry.</param>
        /// <param name="texture">The texture to draw.</param>
        private void FlushVertexArray(int count, Effect effect, Texture texture)
        {
            if (count == 0)
                return;

            _vertexBuffer.SetData(
                0, _itemVertexBuffer.SafePtr, 0, count, sizeof(VertexPositionColorTexture), 0, SetDataOptions.Discard);

            _device.SetVertexBuffer(_vertexBuffer);
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

                    _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, count / 2);
                }
            }
            else
            {
                // If no custom effect is defined, then simply render.
                _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, count / 2);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                _itemVertexBuffer.Dispose();
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
