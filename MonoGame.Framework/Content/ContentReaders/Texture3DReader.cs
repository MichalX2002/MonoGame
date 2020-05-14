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
                reader.GetGraphicsDevice(), width, height, depth, levelCount > 1, format);

            for (int i = 0; i < levelCount; i++)
            {
                int dataSize = reader.ReadInt32();
                using (var buffer = reader.ContentManager.GetScratchBuffer(dataSize))
                {
                    reader.Read(buffer.AsSpan(0, dataSize));
                    texture.SetData(i, 0, 0, width, height, 0, depth, buffer.AsSpan(0, dataSize));
                }

                // Calculate dimensions of next mip level.
                width = Math.Max(width / 2, 1);
                height = Math.Max(height / 2, 1);
                depth = Math.Max(depth / 2, 1);
            }

            return texture;
        }
    }
}
