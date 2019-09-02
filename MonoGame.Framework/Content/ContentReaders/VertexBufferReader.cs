// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content
{
    class VertexBufferReader : ContentTypeReader<VertexBuffer>
    {
        protected internal override VertexBuffer Read(ContentReader input, VertexBuffer existingInstance)
        {
            var declaration = input.ReadRawObject<VertexDeclaration>();
            var vertexCount = (int)input.ReadUInt32();
            int dataSize = vertexCount * declaration.VertexStride;

            byte[] data = input.ContentManager.GetScratchBuffer(dataSize);
            try
            {
                if (input.Read(data, 0, dataSize) != dataSize)
                    throw new InvalidDataException();

                var buffer = new VertexBuffer(input.GraphicsDevice, declaration, vertexCount, BufferUsage.None);
                buffer.SetData(data.AsSpan(0, dataSize));
                return buffer;
            }
            finally
            {
                input.ContentManager.ReturnScratchBuffer(data);
            }
        }
    }
}