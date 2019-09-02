// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content
{
    internal class Texture3DReader : ContentTypeReader<Texture3D>
    {
        protected internal override Texture3D Read(ContentReader reader, Texture3D existingInstance)
        {
            var format = (SurfaceFormat)reader.ReadInt32();
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            int depth = reader.ReadInt32();
            int levelCount = reader.ReadInt32();

            var texture = existingInstance ?? new Texture3D(
                reader.GraphicsDevice, width, height, depth, levelCount > 1, format);

            for (int i = 0; i < levelCount; i++)
            {
                int dataSize = reader.ReadInt32();
                byte[] data = reader.ContentManager.GetScratchBuffer(dataSize);
                try
                {
                    if(reader.Read(data, 0, dataSize) != dataSize)
                        throw new InvalidDataException();

                    texture.SetData(i, 0, 0, width, height, 0, depth, data.AsSpan(0, dataSize));
                }
                finally
                {
                    reader.ContentManager.ReturnScratchBuffer(data);
                }

                // Calculate dimensions of next mip level.
                width = Math.Max(width >> 1, 1);
                height = Math.Max(height >> 1, 1);
                depth = Math.Max(depth >> 1, 1);
            }

            return texture;
        }
    }
}
