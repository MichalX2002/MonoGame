// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content.Pipeline.Processors
{
    public sealed class ModelMeshPartContent
    {
        private int _startIndex;
        private int _vertexOffset;

        internal ModelMeshPartContent() { }

        internal ModelMeshPartContent(VertexBufferContent vertexBuffer, IndexCollection indices, int vertexOffset,
                                      int numVertices, int startIndex, int primitiveCount)
        {
            VertexBuffer = vertexBuffer;
            IndexBuffer = indices;
            _vertexOffset = vertexOffset;
            NumVertices = numVertices;
            _startIndex = startIndex;
            PrimitiveCount = primitiveCount;
        }

        public IndexCollection IndexBuffer { get; }

        public MaterialContent Material { get; set; }

        public int NumVertices { get; }

        public int PrimitiveCount { get; }

        public int StartIndex => _startIndex;

        public object Tag { get; set; }

        public VertexBufferContent VertexBuffer { get; }

        public int VertexOffset => _vertexOffset;
    }
}
