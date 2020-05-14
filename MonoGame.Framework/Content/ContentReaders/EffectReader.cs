// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
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
            using (var buffer = input.ContentManager.GetScratchBuffer(dataSize))
            {
                if (input.Read(buffer.AsSpan(0, dataSize)) != dataSize)
                    throw new InvalidDataException();

                var effect = new Effect(input.GetGraphicsDevice(), buffer.AsSpan(0, dataSize))
                {
                    Name = input.AssetName
                };
                return effect;
            }
        }
    }
}
