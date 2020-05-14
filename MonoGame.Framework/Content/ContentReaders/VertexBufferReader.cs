// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.IO;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content
{
    class VertexBufferReader : ContentTypeReader<VertexBuffer>
    {
        protected internal override VertexBuffer Read(ContentReader input, VertexBuffer existingInstance)
        {
            var vertexBuffer = existingInstance;

            var declaration = input.ReadRawObject<VertexDeclaration>();
            int elementCount = (int)input.ReadUInt32();
            int dataSize = elementCount * declaration.VertexStride;

            using (var data = input.ContentManager.GetScratchBuffer(dataSize))
            {
                if (input.Read(data.AsSpan(0, dataSize)) != dataSize)
                    throw new InvalidDataException();

                if (vertexBuffer == null ||
                    vertexBuffer.VertexDeclaration != declaration ||
                    vertexBuffer.Capacity < elementCount)
                {
                    vertexBuffer = new VertexBuffer(
                        input.GetGraphicsDevice(), declaration, elementCount, BufferUsage.None);
                }

                vertexBuffer.SetData(data.AsSpan(0, dataSize));
                return vertexBuffer;
            }
        }
    }
}