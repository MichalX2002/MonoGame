using System;
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

        /// <summary>
        /// Initial size for the batch item list.
        /// </summary>
        private const int InitialBatchSize = 256;

        /// <summary>
        /// The maximum number of batch items that can be processed per iteration
        /// </summary>
        private const int MaxBatchSize = ushort.MaxValue / 6 - 1; // 6 = 4 vertices unique and 2 shared, per quad

        /// <summary>
        /// The list of batch items to process.
        /// </summary>
        private SpriteBatchItem[] _batchItemList;

        /// <summary>
        /// Amount of batched items.
        /// </summary>
        private int _batchItemCount;

        /// <summary>
        /// The target graphics device.
        /// </summary>
        private readonly GraphicsDevice _device;
        
        /// <summary>
        /// Buffer for copying vertices from the enqueued batch items 
        /// and then drawing them.
        /// </summary>
        private int _vertexBufferBytes;
        private IntPtr _vertexBuffer;
        private VertexPositionColorTexture* _vertexBufferPtr;

        /// <summary>
        /// Index buffer; the values in this buffer are 
        /// constant and more indices are added as needed.
        /// </summary>
        private int _indexBufferBytes;
        private IntPtr _indexBuffer;

        public bool IsDisposed { get; private set; }

        public SpriteBatcher(GraphicsDevice device)
        {
            _device = device;

            _batchItemList = new SpriteBatchItem[InitialBatchSize];
            for (int i = 0; i < InitialBatchSize; i++)
                _batchItemList[i] = new SpriteBatchItem();

            EnsureCapacity(InitialBatchSize);
        }

        /// <summary>
        /// Ensures that there's enough indices and vertex buffer 
        /// capacity for the specified <paramref name="itemCount"/>
        /// (up to <see cref="MaxBatchSize"/>).
        /// </summary>
        private void EnsureCapacity(int itemCount)
        {
            int oldVertexCount = _vertexBufferBytes / 4 / sizeof(VertexPositionColorTexture);
            if (itemCount > oldVertexCount)
            {
                if (_vertexBufferBytes != 0)
                    GC.RemoveMemoryPressure(_vertexBufferBytes);

                // 1 batch item has 4 vertices
                _vertexBufferBytes = itemCount * 4 * sizeof(VertexPositionColorTexture);

                if (_vertexBuffer == IntPtr.Zero)
                    _vertexBuffer = Marshal.AllocHGlobal(_vertexBufferBytes);
                else
                    _vertexBuffer = Marshal.ReAllocHGlobal(_vertexBuffer, (IntPtr)_vertexBufferBytes);

                GC.AddMemoryPressure(_vertexBufferBytes);
                _vertexBufferPtr = (VertexPositionColorTexture*)_vertexBuffer;
            }

            // 1 batch item needs 6 indices
            int oldIndexCount = _indexBufferBytes / 6 / sizeof(ushort);
            if (itemCount > oldIndexCount)
            {
                if (_indexBufferBytes != 0)
                    GC.RemoveMemoryPressure(_indexBufferBytes);

                // 1 batch item needs 6 indices
                _indexBufferBytes = itemCount * 6 * sizeof(ushort);

                if(_indexBuffer == IntPtr.Zero)
                    _indexBuffer = Marshal.AllocHGlobal(_indexBufferBytes);
                else
                    _indexBuffer = Marshal.ReAllocHGlobal(_indexBuffer, (IntPtr)_indexBufferBytes);

                GC.AddMemoryPressure(_indexBufferBytes);

                ushort* indexPtr = (ushort*)_indexBuffer;
                for (int i = 0; i < itemCount; i++, indexPtr += 6)
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

            }
        }

        /// <summary>
        /// Reuse a previously allocated SpriteBatchItem from the item pool. 
        /// if there is none available grow the pool and initialize new items.
        /// </summary>
        /// <returns></returns>
        public SpriteBatchItem CreateBatchItem()
        {
            if (_batchItemCount >= _batchItemList.Length)
            {
                int oldSize = _batchItemList.Length;
                int newSize = oldSize + oldSize / 2; // grow by x1.5
                newSize = (newSize + 255) & (~255); // grow in chunks of 256.

                Array.Resize(ref _batchItemList, newSize);
                for (int i = oldSize; i < newSize; i++)
                    _batchItemList[i] = new SpriteBatchItem();

                EnsureCapacity(newSize);
            }
            return _batchItemList[_batchItemCount++];
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
            if (_batchItemCount == 0)
                return;

            // sort the batch items
            switch (sortMode)
            {
                case SpriteSortMode.Texture:
                case SpriteSortMode.FrontToBack:
                case SpriteSortMode.BackToFront:
                    Array.Sort(_batchItemList, 0, _batchItemCount);
                    break;
            }
            
            int batchCount = _batchItemCount;

            // Iterate through the batches, doing clamped sets of vertices only.
            while (batchCount > 0)
            {
                int vertexCount = 0;
                Texture2D tex = null;

                int batchesToProcess = batchCount;
                if (batchesToProcess > MaxBatchSize)
                    batchesToProcess = MaxBatchSize;

                // Draw the batches
                var vertexPtr = _vertexBufferPtr;
                for (int i = 0; i < batchesToProcess; i++, vertexCount += 4, vertexPtr += 4)
                {
                    SpriteBatchItem item = _batchItemList[_batchItemCount - batchCount];
                    // if the texture changed, we need to flush and bind the new texture
                    if (!ReferenceEquals(item.Texture, tex))
                    {
                        FlushVertexArray(vertexCount, effect, tex);

                        vertexCount = 0;
                        vertexPtr = _vertexBufferPtr;

                        tex = item.Texture;
                        _device.Textures[0] = tex;
                    }

                    // store the SpriteBatchItem data in our vertex buffer
                    *(vertexPtr) = item.VertexTL;
                    *(vertexPtr + 1) = item.VertexTR;
                    *(vertexPtr + 2) = item.VertexBL;
                    *(vertexPtr + 3) = item.VertexBR;

                    // Release the texture.
                    item.Texture = null;

                    // Update our count to continue culling down large batches
                    batchCount--;
                }

                // flush the remaining vertexArray data
                FlushVertexArray(vertexCount, effect, tex);
            }
            
            unchecked
            {
                _device._graphicsMetrics._spriteCount += _batchItemCount;
            }

            // return items to the pool.  
            _batchItemCount = 0;
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
                        PrimitiveType.TriangleList, _vertexBuffer, 0, vertexCount,
                        IndexElementSize.SixteenBits, _indexBuffer, 0, vertexCount / 2);
                }
            }
            else
            {
                // If no custom effect is defined, then simply render.
                _device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
                    PrimitiveType.TriangleList, _vertexBuffer, 0, vertexCount,
                    IndexElementSize.SixteenBits, _indexBuffer, 0, vertexCount / 2);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                _vertexBufferPtr = null;
                if (_vertexBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_vertexBuffer);
                    _vertexBuffer = IntPtr.Zero;

                    GC.RemoveMemoryPressure(_vertexBufferBytes);
                    _vertexBufferBytes = 0;
                }

                if(_indexBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_indexBuffer);
                    _indexBuffer = IntPtr.Zero;

                    GC.RemoveMemoryPressure(_indexBufferBytes);
                    _indexBufferBytes = 0;
                }
                
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
