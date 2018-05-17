using System;

using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    class SkinnedEffectReader : ContentTypeReader<SkinnedEffect>
    {
        protected internal override SkinnedEffect Read(ContentReader input, SkinnedEffect existingInstance)
        {
            var effect = new SkinnedEffect(input.GraphicsDevice)
            {
                Texture = input.ReadExternalReference<Texture>() as Texture2D,
                WeightsPerVertex = input.ReadInt32(),
                DiffuseColor = input.ReadVector3(),
                EmissiveColor = input.ReadVector3(),
                SpecularColor = input.ReadVector3(),
                SpecularPower = input.ReadSingle(),
                Alpha = input.ReadSingle()
            };
            return effect;
        }
    }
}
