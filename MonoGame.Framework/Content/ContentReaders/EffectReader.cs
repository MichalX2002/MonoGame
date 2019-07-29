// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content
{
    internal class EffectReader : ContentTypeReader<Effect>
    {
        public EffectReader()
        {
        }

        protected internal override Effect Read(ContentReader input, Effect existingInstance)
        {
            int dataSize = input.ReadInt32();
            byte[] data = input.ContentManager.GetScratchBuffer(dataSize);
            input.Read(data, 0, dataSize);
            var effect = new Effect(input.GraphicsDevice, data, 0, dataSize)
            {
                Name = input.AssetName
            };
            return effect;
        }
    }
}
