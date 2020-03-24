// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content
{
    internal class SpriteFontReader : ContentTypeReader<SpriteFont>
    {
        public SpriteFontReader()
        {
        }

        protected internal override SpriteFont Read(ContentReader input, SpriteFont existingInstance)
        {
            if (existingInstance != null)
            {
                // Read the texture into the existing texture instance
                input.ReadObject(existingInstance.Texture);
                
                // FIXME: is it really needed to read theses objects?

                // discard the rest of the SpriteFont data as 
                // we are only reloading GPU resources for now
                input.ReadObject<List<Rectangle>>();
                input.ReadObject<List<Rectangle>>();
                input.ReadObject<List<char>>();
                input.ReadInt32();
                input.ReadSingle();
                input.ReadObject<List<Vector3>>();
                if (input.ReadBoolean())
                    input.ReadChar();
                
                return existingInstance;
            }
            else
            {
                // Create a fresh SpriteFont instance
                var texture = input.ReadObject<Texture2D>();
                var glyphs = input.ReadObject<List<Rectangle>>();
                var cropping = input.ReadObject<List<Rectangle>>();
                var charMap = input.ReadObject<List<char>>();
                int lineSpacing = input.ReadInt32();
                float spacing = input.ReadSingle();
                var kerning = input.ReadObject<List<Vector3>>();
                var defaultCharacter = input.ReadBoolean() ? input.ReadChar() : (char?)null;
                
                return new SpriteFont(
                    texture, glyphs, cropping, charMap, lineSpacing, spacing, kerning, defaultCharacter);
            }
        }
    }
}
