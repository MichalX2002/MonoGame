using System;
using MonoGame.Framework.Memory;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// This class handles the queueing of batch items into the GPU by creating the triangle tesselations
    /// that are used to draw the sprite textures. This class supports int.MaxValue number of sprites to be
    /// batched and will process them into short.MaxValue groups (strided by 6 for the number of vertices
    /// sent to the GPU). 
    /// </summary>
    internal class SpriteBatcher : IDisposable
    {
        /*
         * Note that this class is fundamental to high performance for SpriteBatch games. Please exercise
         * caution when making changes to this class.
         */

        private const int InitialBatchSize = 256;

        private const int BufferSizeGrowth = 256;

        /// <summary>
        /// The maximum number of batch items that can be 
        /// drawn at once with <see cref="FlushVertexArray"/>.
        /// </summary>
        private const int MaxBatchSize = ushort.MaxValue / 4;

        private readonly GraphicsDevice _device;

        private int _itemCount;
        private SpriteBatchItem[] _batchItems;

        // TODO: allocate on Pinned Object Heap instead
        private UnmanagedMemory<VertexPositionColorTexture> _vertexBuffer;

        /// <summary>
        /// The index buffer values are constant and more indices are added as needed.
        /// </summary>
        private UnmanagedMemory<ushort> _indexBuffer;

        public bool IsDisposed { get; private set; }

        public SpriteBatcher(GraphicsDevice device)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));

            _vertexBuffer = new UnmanagedMemory<VertexPositionColorTexture>();
            _indexBuffer = new UnmanagedMemory<ushort>();

            _batchItems = new SpriteBatchItem[InitialBatchSize];
            for (int i = 0; i < _batchItems.Length; i++)
                _batchItems[i] = new SpriteBatchItem();

            SetupBuffers(InitialBatchSize, 0);
        }

        /// <summary>
        /// Calculates indices and resizes buffer capacity (up to <see cref="MaxBatchSize"/>).
        /// </summary>
        private void SetupBuffers(int itemCount, int oldItemCount)
        {
            int minVertices = itemCount * 4; // 4 vertices per item
            if (minVertices > _vertexBuffer.Length)
                _vertexBuffer.Length = minVertices;

            int minIndices = itemCount * 6; // 6 indices per item
            if (minIndices > _indexBuffer.Length)
            {
                _indexBuffer.Length = minIndices;

                int oldVertices = oldItemCount * 4;
                int oldIndices = oldItemCount * 6;
                var indexSpan = _indexBuffer.Span;
                for (int i = oldIndices, v = oldVertices; i < minIndices; i += 6, v += 4)
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
                    indexSpan[i + 0] = (ushort)(v + 0);
                    indexSpan[i + 1] = (ushort)(v + 1);
                    indexSpan[i + 2] = (ushort)(v + 2);

                    // Triangle 2
                    indexSpan[i + 3] = (ushort)(v + 1);
                    indexSpan[i + 4] = (ushort)(v + 3);
                    indexSpan[i + 5] = (ushort)(v + 2);
                }
            }
        }

        public SpriteBatchItem GetBatchItem()
        {
            if (_itemCount >= _batchItems.Length)
            {
                int oldSize = _batchItems.Length;
                int newSize = oldSize + oldSize / 2; // grow by x1.5
                newSize = (newSize + BufferSizeGrowth - 1) & ~(BufferSizeGrowth - 1); // grow in chunks

                Array.Resize(ref _batchItems, newSize);
                for (int i = oldSize; i < newSize; i++)
                    _batchItems[i] = new SpriteBatchItem();

                SetupBuffers(Math.Min(newSize, MaxBatchSize), oldSize);
            }
            return _batchItems[_itemCount++];
        }

        /// <summary>
        /// Sorts the batch items and then groups batch drawing into maximal allowed batch sets that do not
        /// overflow the 16-bit array indices for vertices.
        /// </summary>
        /// <param name="sortMode">The type of depth sorting desired for the rendering.</param>
        /// <param name="effect">The custom effect to apply to the drawn geometry</param>
        public unsafe void DrawBatch(SpriteSortMode sortMode, Effect effect)
        {
            if (effect != null && effect.IsDisposed)
                throw new ObjectDisposedException(nameof(effect));

            // nothing to do
            if (_itemCount == 0)
                return;

            // sort the batch items
            switch (sortMode)
            {
                case SpriteSortMode.Texture:
                case SpriteSortMode.FrontToBack:
                case SpriteSortMode.BackToFront:
                    Array.Sort(_batchItems, 0, _itemCount);
                    break;
            }

            // iterate through the batches, doing clamped sets of vertices at the time
            var dstVertexSpan = _vertexBuffer.Span;
            var indexSpan = _indexBuffer.Span;
            int itemIndex = 0;
            int itemsLeft = _itemCount;

            fixed (VertexPositionColorTexture* dstVertexPtr = dstVertexSpan)
            {
                while (itemsLeft > 0)
                {
                    int itemsToProcess = itemsLeft;
                    if (itemsToProcess > MaxBatchSize)
                        itemsToProcess = MaxBatchSize;

                    int count = 0;
                    Texture2D tex = null;

                    // TODO: create a path with sort-by-texture that does less ref-equality checks
                    // (having such a path allows copying multiple items at once)

                    // draw the batches
                    for (int i = 0; i < itemsToProcess; i++, count += 4)
                    {
                        var item = _batchItems[itemIndex++];

                        // if the texture changed, we need to flush and bind the new texture
                        if (!ReferenceEquals(item.Texture, tex))
                        {
                            FlushVertexArray(dstVertexSpan.Slice(0, count), indexSpan, effect, tex);
                            count = 0;

                            tex = item.Texture;
                            _device.Textures[0] = tex;
                        }
                        item.Texture = null; // release texture from item

                        dstVertexPtr[count + 0] = item.VertexTL;
                        dstVertexPtr[count + 1] = item.VertexTR;
                        dstVertexPtr[count + 2] = item.VertexBL;
                        dstVertexPtr[count + 3] = item.VertexBR;
                    }

                    // flush the remaining data
                    FlushVertexArray(dstVertexSpan.Slice(0, count), indexSpan, effect, tex);

                    itemsLeft -= itemsToProcess;
                }
            }

            unchecked
            {
                _device._graphicsMetrics._spriteCount += _itemCount;
            }
            _itemCount = 0;
        }

        /// <summary>
        /// Sends the triangle list to the graphics device. Here is where the actual drawing starts.
        /// </summary>
        /// <param name="vertices">The vertices to draw.</param>
        /// <param name="indices">The indices used to draw.</param>
        /// <param name="effect">The custom effect to apply to the geometry.</param>
        /// <param name="texture">The texture to draw.</param>
        private void FlushVertexArray(
            ReadOnlySpan<VertexPositionColorTexture> vertices, ReadOnlySpan<ushort> indices,
            Effect effect, Texture texture)
        {
            if (vertices.IsEmpty)
                return;

            const PrimitiveType primitiveType = PrimitiveType.TriangleList;
            int primitiveCount = vertices.Length / 2;

            // If the effect is not null, then apply each pass and render the geometry
            if (effect == null)
            {
                // If no custom effect is defined, then simply render.
                _device.DrawUserIndexedPrimitives(primitiveType, vertices, indices, primitiveCount);
            }
            else
            {
                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Whatever happens in pass.Apply, make sure the texture being drawn
                    // ends up in Textures[0].
                    _device.Textures[0] = texture;

                    _device.DrawUserIndexedPrimitives(primitiveType, vertices, indices, primitiveCount);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                _vertexBuffer.Dispose();
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
