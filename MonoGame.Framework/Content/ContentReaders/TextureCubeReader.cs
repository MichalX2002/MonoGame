// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content
{
    internal class TextureCubeReader : ContentTypeReader<TextureCube>
    {

        protected internal override TextureCube Read(ContentReader reader, TextureCube existingInstance)
        {
            SurfaceFormat surfaceFormat = (SurfaceFormat)reader.ReadInt32();
            int size = reader.ReadInt32();
            int levels = reader.ReadInt32();

            TextureCube textureCube = existingInstance ??
                new TextureCube(reader.GraphicsDevice, size, levels > 1, surfaceFormat);

            for (int face = 0; face < 6; face++)
            {
                for (int i = 0; i < levels; i++)
                {
                    int faceSize = reader.ReadInt32();
                    byte[] faceData = reader.ContentManager.GetScratchBuffer(faceSize);
                    reader.Read(faceData, 0, faceSize);
                    textureCube.SetData((CubeMapFace)face, i, null, faceData, 0, faceSize);
                }
            }

            return textureCube;
        }
    }
}
