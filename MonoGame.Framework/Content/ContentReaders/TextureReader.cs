// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content
{
    internal class TextureReader : ContentTypeReader<Texture>
    {
        protected internal override Texture Read(ContentReader reader, Texture existingInstance)
        {
            return existingInstance;
        }
    }
}