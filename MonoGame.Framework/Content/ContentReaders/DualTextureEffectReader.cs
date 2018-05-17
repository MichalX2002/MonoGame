using System;

using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    class DualTextureEffectReader : ContentTypeReader<DualTextureEffect>
    {
        protected internal override DualTextureEffect Read(ContentReader input, DualTextureEffect existingInstance)
        {
            DualTextureEffect effect = new DualTextureEffect(input.GraphicsDevice)
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

