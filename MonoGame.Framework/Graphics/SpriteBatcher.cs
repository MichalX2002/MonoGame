

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
        private const int InitialBatchSize = 128;
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
        /// Buffer for copying vertices from the enqueued batch items into
        /// easy-to-upload data for the unsafe buffers.
        /// </summary>
        private int _vertexBuildBufferBytes;
        private IntPtr _safeVertexBuildBuffer;
        private VertexPositionColorTexture* _vertexBuildBuffer;
        private int _vertexBuildBufferSize;

        /// <summary> Vertex buffer. </summary>
        private UnsafeDynamicVertexBuffer _vertices;

        /// <summary>
        /// Index buffer; the values in this buffer are constant and 
        /// more indices are added as needed.
        /// </summary>
        private UnsafeDynamicIndexBuffer _indices;

        public bool IsDisposed { get; private set; }

        public SpriteBatcher(GraphicsDevice device)
        {
            _device = device;

            _indices = new UnsafeDynamicIndexBuffer(_device, IndexElementSize.SixteenBits, BufferUsage.WriteOnly);
            _vertices = new UnsafeDynamicVertexBuffer(_device, VertexPositionColorTexture.VertexDeclaration, BufferUsage.WriteOnly);

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
            if (itemCount > _vertexBuildBufferSize)
            {
                if (_vertexBuildBufferBytes != 0)
                    GC.RemoveMemoryPressure(_vertexBuildBufferBytes);

                // 1 batch item has 4 vertices
                _vertexBuildBufferBytes = itemCount * 4 * sizeof(VertexPositionColorTexture);

                if (_safeVertexBuildBuffer == IntPtr.Zero)
                    _safeVertexBuildBuffer = Marshal.AllocHGlobal(_vertexBuildBufferBytes);
                else
                    _safeVertexBuildBuffer = Marshal.ReAllocHGlobal(_safeVertexBuildBuffer, (IntPtr)_vertexBuildBufferBytes);
                
                _vertexBuildBuffer = (VertexPositionColorTexture*)_safeVertexBuildBuffer;
                _vertexBuildBufferSize = itemCount;

                GC.AddMemoryPressure(_vertexBuildBufferBytes);
            }

            // 1 batch item needs 6 indices
            if (itemCount > _indices.IndexCount / 6)
            {
                // 1 batch item needs 6 indices
                IntPtr tempPtr = Marshal.AllocHGlobal(itemCount * 6 * sizeof(ushort));

                ushort* indexPtr = (ushort*)tempPtr;
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

                // 1 batch item needs 6 indices
                _indices.SetData(tempPtr, itemCount * 6);
                Marshal.FreeHGlobal(tempPtr);
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
                var oldSize = _batchItemList.Length;
                var newSize = oldSize + oldSize / 2; // grow by x1.5
                newSize = (newSize + 63) & (~63); // grow in chunks of 64.
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
                throw new ObjectDisposedException("effect");

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
                var vertexPtr = _vertexBuildBuffer;
                for (int i = 0; i < batchesToProcess; i++, vertexCount += 4, vertexPtr += 4)
                {
                    SpriteBatchItem item = _batchItemList[_batchItemCount - batchCount];
                    // if the texture changed, we need to flush and bind the new texture
                    if (!ReferenceEquals(item.Texture, tex))
                    {
                        FlushVertexArray(vertexCount, effect, tex);

                        vertexCount = 0;
                        vertexPtr = _vertexBuildBuffer;

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
            
            _vertices.SetData(_safeVertexBuildBuffer, vertexCount);
            _device.SetVertexBuffer(_vertices);
            _device.Indices = _indices;

            // If the effect is not null, then apply each pass and render the geometry
            if (effect != null)
            {
                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Whatever happens in pass.Apply, make sure the texture being drawn
                    // ends up in Textures[0].
                    _device.Textures[0] = texture;

                    _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount / 2);
                }
            }
            else
            {
                // If no custom effect is defined, then simply render.
                _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount / 2);
            }

            _device.Indices = null;
            _device.SetVertexBuffer(null);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                _vertices.Dispose();
                _vertices = null;

                _indices.Dispose();
                _indices = null;

                _vertexBuildBuffer = null;
                if (_safeVertexBuildBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_safeVertexBuildBuffer);
                    _safeVertexBuildBuffer = IntPtr.Zero;

                    GC.RemoveMemoryPressure(_vertexBuildBufferBytes);
                    _vertexBuildBufferBytes = 0;
                }
                _vertexBuildBufferSize = 0;
                
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
