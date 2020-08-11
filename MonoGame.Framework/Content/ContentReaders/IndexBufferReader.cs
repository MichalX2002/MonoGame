// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.IO;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content
{
    class IndexBufferReader : ContentTypeReader<IndexBuffer>
    {
        protected internal override IndexBuffer Read(ContentReader input, IndexBuffer existingInstance)
        {
            IndexBuffer indexBuffer = existingInstance;

            bool sixteenBits = input.ReadBoolean();
            int dataSize = input.ReadInt32();

            var elementType = sixteenBits ? IndexElementType.Int16 : IndexElementType.Int32;
            int elementCount = dataSize / elementType.TypeSize();

            using (var buffer = input.ContentManager.GetScratchBuffer(dataSize))
            {
                if (input.Read(buffer.AsSpan(0, dataSize)) != dataSize)
                    throw new InvalidDataException();

                if (indexBuffer == null || 
                    indexBuffer.ElementType != elementType ||
                    indexBuffer.Capacity < elementCount)
                {
                    indexBuffer = new IndexBuffer(
                        input.GetGraphicsDevice(), elementType, elementCount, BufferUsage.None);
                }

                indexBuffer.SetData(buffer.AsSpan().Slice(0, dataSize));
                return indexBuffer;
            }
        }
    }
}