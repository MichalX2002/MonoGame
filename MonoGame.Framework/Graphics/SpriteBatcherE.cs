// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// This class handles the queueing of batch items into the GPU by creating the triangle tesselations
    /// that are used to draw the sprite textures. This class supports <see cref="int.MaxValue "/> number 
    /// of sprites to be batched at once and will process them into <see cref="MaximalBatchSize"/> groups
    /// (strided by 6 for the number of vertices sent to the GPU). 
    /// </summary>
	internal unsafe class SpriteBatcher : IDisposable
    {
        /*
         * Note that this class is fundamental to high performance for SpriteBatch games. Please exercise
         * caution when making changes to this class.
         */

        /// <summary> Initial size for a batch item queue. </summary>
        public const int InitialBatchSize = 64;

        /// <summary>
        /// The maximal number of batch items per draw call.
        /// </summary>
        private const int MaximalBatchSize = 10922;

        /// <summary> The target graphics device. </summary>
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
        /// Last list for predicting the next GetBatchItem list and reducing 
        /// the amount of dictionary lookups.
        /// </summary>
        private BatchItemList _lastList;

        /// <summary>
        /// Buffer for copying vertices from the enqueued batch items into
        /// easy-to-upload data for the unsafe buffers.
        /// </summary>
        private int _vertexBuildBufferBytes;
        private IntPtr _vertexBuildBuffer;
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

            _batches = new Dictionary<Texture2D, BatchItemList>(new Texture2DEqualityComparer());
            _listPool = new List<BatchItemList>();

            EnsureVertexBuildBuffer(InitialBatchSize);
            EnsureIndexBufferSize(InitialBatchSize);
        }

        private void EnsureVertexBuildBuffer(int itemCount)
        {
            if (itemCount > _vertexBuildBufferSize)
            {
                if (_vertexBuildBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_vertexBuildBuffer);
                    GC.RemoveMemoryPressure(_vertexBuildBufferBytes);
                }

                // 1 batch item has 4 vertices
                _vertexBuildBufferBytes = itemCount * 4 * sizeof(VertexPositionColorTexture);
                _vertexBuildBuffer = Marshal.AllocHGlobal(_vertexBuildBufferBytes);
                GC.AddMemoryPressure(_vertexBuildBufferBytes);

                _vertexBuildBufferSize = itemCount;
            }
        }

        /// <summary>
        /// Ensures that the index buffer contains enough indices 
        /// for the specified <paramref name="itemCount"/>
        /// (up to <see cref="MaximalBatchSize"/>).
        /// </summary>
        private void EnsureIndexBufferSize(int itemCount)
        {
            // 1 batch item needs 6 indices
            if (itemCount > _indices.IndexCount / 6)
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

                // 1 batch item needs 6 indices
                _indices.SetData(ptr, itemCount * 6);
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Returns a SpriteBatchItem from the item pool. 
        /// if there is none available grow the pool and initialize new items.
        /// </summary>
        /// <returns></returns>
        public ref SpriteBatchItem GetBatchItem(Texture2D texture)
        {
            BatchItemList list;

            // checking if the last list is the one we want is faster than a dictionary
            // lookup (if the same textures is drawn a lot in succession)
            if (_lastList != null && _lastList.LastTextureSortKey == texture.SortingKey)
            {
                list = _lastList;
            }
            else if (_batches.TryGetValue(texture, out list) == false)
            {
                list = GetBatchList(texture);
                _batches.Add(texture, list);
            }

            // resize the list if needed
            if (list.Count >= list.Capacity)
            {
                int newSize = list.Capacity + list.Capacity / 2; // grow by x1.5
                newSize = (newSize + 63) & (~63); // grow in chunks of 64
                list.Resize(newSize);
            }

            _lastList = list;
            return ref list.Ptr[list.Count++];
        }

        private BatchItemList GetBatchList(Texture2D texture)
        {
            int poolCount = _listPool.Count;
            if (poolCount > 0)
            {
                // try to find a list already assigned to the texture
                // (trying to predict last usage of the texture)
                for (int i = 0; i < poolCount; i++)
                {
                    var existingList = _listPool[i];
                    if (existingList.LastTextureSortKey == texture.SortingKey)
                    {
                        _listPool.RemoveAt(i);
                        return existingList;
                    }
                }

                // get the first list if any existing didn't match
                int index = poolCount - 1;
                var list = _listPool[index];
                _listPool.RemoveAt(index);

                list.LastTextureSortKey = texture.SortingKey;
                return list;
            }

            return new BatchItemList(texture.SortingKey);
        }

        /// <summary>
        /// Sorts the batch items and then groups batch drawing into maximal allowed batch sets that do not
        /// overflow the 16 bit array indices for vertices.
        /// </summary>
        /// <param name="sortMode">The type of depth sorting desired for the rendering.</param>
        /// <param name="effect">The custom effect to apply to the drawn geometry</param>
        public void DrawBatch(SpriteSortMode sortMode, Effect effect)
        {
            if (effect != null && effect.IsDisposed)
                throw new ObjectDisposedException(nameof(effect));

            foreach (var batch in _batches)
            {
                Texture2D texture = batch.Key;
                BatchItemList list = batch.Value;

                _device.Textures[0] = texture;

                // sort the batch items
                switch (sortMode)
                {
                    case SpriteSortMode.FrontToBack:
                    case SpriteSortMode.BackToFront:
                        list.Sort();
                        break;
                }

                SpriteBatchItem* itemPtr = list.Ptr;
                int itemsLeft = list.Count;
                while (itemsLeft > 0)
                {
                    // get maximal batch size, more than max won't have enough indices
                    int sliceItemCount = Math.Min(itemsLeft, MaximalBatchSize);
                    EnsureIndexBufferSize(sliceItemCount);
                    EnsureVertexBuildBuffer(sliceItemCount);

                    // setup fields to use in the copying loop
                    int vertexCount = sliceItemCount * 4;
                    int itemOffset = list.Count - itemsLeft;
                    var outputPtr = (VertexPositionColorTexture*)_vertexBuildBuffer;
                    for (int i = 0; i < vertexCount; i += 4)
                    {
                        // copy vertex data from items to build buffer
                        // one item contains 4 vertices
                        ref SpriteBatchItem item = ref itemPtr[i / 4 + itemOffset];
                        outputPtr[i] = item.VertexTL;
                        outputPtr[i + 1] = item.VertexTR;
                        outputPtr[i + 2] = item.VertexBL;
                        outputPtr[i + 3] = item.VertexBR;
                    }

                    // upload data from build buffer,
                    // 1 batch item has 4 vertices, therefore we use vertexCount
                    _vertices.SetData(_vertexBuildBuffer, vertexCount);
                    FlushVertexArray(vertexCount, effect, texture);
                    itemsLeft -= sliceItemCount;
                }

                unchecked
                {
                    _device._graphicsMetrics._spriteCount += list.Count;
                }

                // recycle the list
                list.Clear();
                _listPool.Add(list);
            }

            _lastList = null;
            _batches.Clear();
        }

        /// <summary>
        /// Sends data to the graphics device. Here is where the actual drawing starts.
        /// </summary>
        /// <param name="vertexCount">The amount of vertices.</param>
        /// <param name="effect">The custom effect to apply to the geometry</param>
        /// <param name="texture">The texture to draw.</param>
        private void FlushVertexArray(int vertexCount, Effect effect, Texture texture)
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

                if (_vertexBuildBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_vertexBuildBuffer);
                    _vertexBuildBuffer = IntPtr.Zero;

                    GC.RemoveMemoryPressure(_vertexBuildBufferBytes);
                    _vertexBuildBufferBytes = 0;
                }
                _vertexBuildBufferSize = 0;

                _batches = null;
                _listPool = null;

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

        private struct Texture2DEqualityComparer : IEqualityComparer<Texture2D>
        {
            public bool Equals(Texture2D x, Texture2D y)
            {
                return x.SortingKey.Equals(y.SortingKey);
            }

            public int GetHashCode(Texture2D obj)
            {
                return obj.SortingKey;
            }
        }

        private unsafe class BatchItemList : IDisposable
        {
            public bool IsDisposed { get; private set; }

            private IntPtr _safePtr;
            private int _allocatedBytes;

            /// <summary>
            /// Used to determine if this <see cref="BatchItemList"/> was previously used
            /// for a specific texture (to predict the needed size of the items array).
            /// </summary>
            public int LastTextureSortKey;

            public int Count;
            public SpriteBatchItem* Ptr;
            public int Capacity { get; private set; }

            public BatchItemList(int textureSortKey)
            {
                LastTextureSortKey = textureSortKey;

                Capacity = InitialBatchSize;

                int size = Capacity * sizeof(SpriteBatchItem);
                SetPtr(size, Marshal.AllocHGlobal(size));
            }

            public void Clear()
            {
                Count = 0;
            }

            private void SetPtr(int size, IntPtr ptr)
            {
                if (_allocatedBytes != 0)
                    GC.RemoveMemoryPressure(_allocatedBytes);

                GC.AddMemoryPressure(size);
                _allocatedBytes = size;

                _safePtr = ptr;
                Ptr = (SpriteBatchItem*)_safePtr;
            }

            public void Resize(int newSize)
            {
                if (newSize == 0)
                    return;

                Capacity = newSize;

                IntPtr size = new IntPtr(newSize * sizeof(SpriteBatchItem));
                IntPtr newPtr = Marshal.ReAllocHGlobal(_safePtr, size);
                SetPtr(size.ToInt32(), newPtr);
            }

            public void Sort()
            {
                var sorter = new QuickSorter(Ptr, Count);
                sorter.Sort(0, Count);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!IsDisposed)
                {
                    Marshal.FreeHGlobal(_safePtr);
                    _safePtr = IntPtr.Zero;
                    Ptr = null;

                    if (_allocatedBytes != 0)
                    {
                        GC.RemoveMemoryPressure(_allocatedBytes);
                        _allocatedBytes = 0;
                    }

                    IsDisposed = true;
                }
            }

            ~BatchItemList()
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
}
