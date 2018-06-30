// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// This class handles the queueing of batch items into the GPU by creating the triangle tesselations
    /// that are used to draw the sprite textures. This class supports <see cref="int.MaxValue "/> number 
    /// of sprites to be batched at once and will process them into <see cref="MaximalBatchSize"/> groups
    /// (strided by 6 for the number of vertices sent to the GPU). 
    /// </summary>
	internal class SpriteBatcher : IDisposable
    {
        /*
         * Note that this class is fundamental to high performance for SpriteBatch games. Please exercise
         * caution when making changes to this class.
         */
         
        /// <summary>
        /// Initial  size for the batch item queue.
        /// </summary>
        public const int InitialBatchSize = 256;

        /// <summary>
        /// The maximal number of batch items per iteration.
        /// </summary>
        private const int MaximalBatchSize = 16384;
        
        /// <summary>
        /// The target graphics device.
        /// </summary>
        private readonly GraphicsDevice _device;

        /// <summary>
        /// Dictionary containing the batches that can be queried by texture
        /// to get the batch list.
        /// </summary>
        private Dictionary<Texture2D, BatchItemList> _batches;

        /// <summary>
        /// Pool for <see cref="BatchItemList"/>'s, used to not allocate 
        /// excessive amounts of garbage when batching.
        /// </summary>
        private List<BatchItemList> _listPool;
        
        /// <summary>
        /// Buffer for copying vertices from the enqueued batch items into
        /// easy-to-upload data for the unsafe buffers.
        /// </summary>
        private IntPtr _vertexBuildBuffer;
        private int _vertexBuildBufferSize;

        /// <summary>
        /// Vertex buffer.
        /// </summary>
        private UnsafeDynamicVertexBuffer _vertices;

        /// <summary>
        /// Index buffer; the values in this buffer are semi-constant and 
        /// more indices are added as needed.
        /// </summary>
        private UnsafeDynamicIndexBuffer _indices;

        public bool Disposed { get; private set; }
        
        public SpriteBatcher(GraphicsDevice device)
        {
            _device = device;
            _indices = new UnsafeDynamicIndexBuffer(_device, IndexElementSize.SixteenBits, BufferUsage.WriteOnly);
            _vertices = new UnsafeDynamicVertexBuffer(_device, VertexPositionColorTexture.VertexDeclaration, BufferUsage.WriteOnly);

            _batches = new Dictionary<Texture2D, BatchItemList>(new Texture2DEqualityComparer());
            _listPool = new List<BatchItemList>();

            EnsureVertexBuildBufferSize(InitialBatchSize);
            EnsureIndexBufferSize(InitialBatchSize);
        }

        private void EnsureVertexBuildBufferSize(int newSize)
        {
            if(newSize > _vertexBuildBufferSize)
            {
                if (_vertexBuildBuffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(_vertexBuildBuffer);

                // 1 batch item contains 4 vertex positions
                _vertexBuildBuffer = Marshal.AllocHGlobal(newSize * 4 * Marshal.SizeOf<VertexPositionColorTexture>());
                _vertexBuildBufferSize = newSize;
            }
        }

        /// <summary>
        /// Ensures that the index buffer contains enough indices 
        /// for the specified <paramref name="itemCount"/>
        /// (up to <see cref="MaximalBatchSize"/>).
        /// </summary>
        private void EnsureIndexBufferSize(int itemCount)
        {
            unsafe
            {
                // 1 batch item needs 6 indices
                IntPtr ptr = Marshal.AllocHGlobal(itemCount * 6 * sizeof(ushort));

                ushort* indexPtr = (ushort*)ptr;
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
                _indices.SetData(ptr, itemCount);
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Reuse a previously allocated SpriteBatchItem from the item pool. 
        /// if there is none available grow the pool and initialize new items.
        /// </summary>
        /// <returns></returns>
        public ref SpriteBatchItem GetBatchItem(Texture2D texture)
        {
            if (_batches.TryGetValue(texture, out BatchItemList list))
            {
                int oldSize = list.Items.Length;
                if (list.Count >= oldSize)
                {
                    int newSize = oldSize + oldSize / 2; // grow by x1.5
                    newSize = (newSize + 63) & (~63); // grow in chunks of 64.
                    list.Resize(newSize);
                }
            }
            else
            {
                list = GetBatchList(texture);
                _batches.Add(texture, list);
            }
            
            return ref list.Items[list.Count++];
        }

        private BatchItemList GetBatchList(Texture2D texture)
        {
            var existingList = _listPool.Find((x) => x.LastTextureSortKey == texture.SortingKey);
            if (existingList != null)
                return existingList;
            else
            {
                int count = _listPool.Count;
                if(count > 0)
                {
                    int index = count - 1;
                    BatchItemList item = _listPool[index];
                    item.LastTextureSortKey = texture.SortingKey;

                    _listPool.RemoveAt(index);
                    return item;
                }
            }

            return new BatchItemList(texture.SortingKey);
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
            
            foreach (var batch in _batches)
            {
                Texture2D texture = batch.Key;
                BatchItemList list = batch.Value;
                int itemsInList = list.Count;

                _device.Textures[0] = texture;

                // sort the batch items
                switch (sortMode)
                {
                    case SpriteSortMode.FrontToBack:
                    case SpriteSortMode.BackToFront:
                        list.Sort();
                        break;
                }

                fixed (SpriteBatchItem* itemPtr = list.Items)
                {
                    int itemsLeft = itemsInList;
                    while (itemsLeft > 0)
                    {
                        int sliceCount = Math.Min(itemsLeft, MaximalBatchSize);

                        EnsureVertexBuildBufferSize(sliceCount);
                        EnsureIndexBufferSize(sliceCount);

                        var vertexBufferPtr = (VertexPositionColorTexture*)_vertexBuildBuffer;
                        for (int i = 0; i < sliceCount; i++)
                        {
                            ref SpriteBatchItem item = ref itemPtr[i];
                            vertexBufferPtr[i * 4] = item.VertexTL;
                            vertexBufferPtr[i * 4 + 1] = item.VertexTR;
                            vertexBufferPtr[i * 4 + 2] = item.VertexBL;
                            vertexBufferPtr[i * 4 + 3] = item.VertexBR;
                        }

                        _vertices.SetData(_vertexBuildBuffer, sliceCount);

                        // flush vertex data
                        FlushVertexArray(sliceCount, effect, texture);
                        itemsLeft -= sliceCount;
                    }
                }

                // recycle the list
                list.Count = 0;
                _listPool.Add(list);

                unchecked
                {
                    _device._graphicsMetrics._spriteCount += itemsInList;
                }
            }
            
            _batches.Clear();
        }

        /// <summary>
        /// Sends data to the graphics device. Here is where the actual drawing starts.
        /// </summary>
        /// <param name="count">The amount of vertices.</param>
        /// <param name="effect">The custom effect to apply to the geometry</param>
        /// <param name="texture">The texture to draw.</param>
        private void FlushVertexArray(int count, Effect effect, Texture texture)
        {
            _device.Indices = _indices;
            _device.SetVertexBuffer(_vertices);

            // If the effect is not null, then apply each pass and render the geometry
            if (effect != null)
            {
                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Whatever happens in pass.Apply, make sure the texture being drawn
                    // ends up in Textures[0].
                    _device.Textures[0] = texture;

                    _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, count * 2);
                }
            }
            else
            {
                // If no custom effect is defined, then simply render.
                _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, count * 2);
            }

            _device.Indices = null;
            _device.SetVertexBuffer(null);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                _vertices.Dispose();
                _vertices = null;

                _indices.Dispose();
                _indices = null;

                if (_vertexBuildBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_vertexBuildBuffer);
                    _vertexBuildBuffer = IntPtr.Zero;
                }
                _vertexBuildBufferSize = 0;

                _batches = null;
                _listPool = null;

                Disposed = true;
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

        struct Texture2DEqualityComparer : IEqualityComparer<Texture2D>
        {
            public bool Equals(Texture2D x, Texture2D y)
            {
                return x.SortingKey.Equals(y.SortingKey);
            }

            public int GetHashCode(Texture2D obj)
            {
                return obj.SortingKey.GetHashCode();
            }
        }

        class BatchItemList
        {
            /// <summary>
            /// Used to determine if this <see cref="BatchItemList"/> was previously used
            /// for a specific texture (to predict the needed size of the items array).
            /// </summary>
            public int LastTextureSortKey;

            public int Count;
            public SpriteBatchItem[] Items;

            public BatchItemList(int textureSortKey)
            {
                LastTextureSortKey = textureSortKey;
                Items = new SpriteBatchItem[InitialBatchSize];
            }

            public void Resize(int newSize)
            {
                Array.Resize(ref Items, newSize);
            }

            public void Sort()
            {
                Array.Sort(Items, 0, Count);
            }
        }
    }
}

