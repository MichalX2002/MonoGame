﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
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

            byte[] data = input.ContentManager.GetScratchBuffer(dataSize);
            try
            {
                if (input.Read(data, 0, dataSize) != dataSize)
                    throw new InvalidDataException();

                if (indexBuffer == null)
                {
                    indexBuffer = new IndexBuffer(
                        input.GetGraphicsDevice(),
                        sixteenBits ? IndexElementSize.Short16 : IndexElementSize.Int32,
                        dataSize / (sixteenBits ? 2 : 4), BufferUsage.None);
                }

                indexBuffer.SetData(data.AsSpan().Slice(0, dataSize));
            }
            finally
            {
                input.ContentManager.ReturnScratchBuffer(data);
            }

            return indexBuffer;
        }
    }
}