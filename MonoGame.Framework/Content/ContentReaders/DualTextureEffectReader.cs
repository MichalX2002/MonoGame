// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content
{
    class DualTextureEffectReader : ContentTypeReader<DualTextureEffect>
    {
        protected internal override DualTextureEffect Read(ContentReader input, DualTextureEffect existingInstance)
        {
            DualTextureEffect effect = new DualTextureEffect(input.GetGraphicsDevice())
            {
                Texture = input.ReadExternalReference<Texture>() as Texture2D,
                Texture2 = input.ReadExternalReference<Texture>() as Texture2D,
                DiffuseColor = input.ReadVector3(),
                Alpha = input.ReadSingle(),
                VertexColorEnabled = input.ReadBoolean()
            };
            return effect;
		}
	}
}

