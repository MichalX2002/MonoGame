// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content
{
    class SkinnedEffectReader : ContentTypeReader<SkinnedEffect>
    {
        protected internal override SkinnedEffect Read(ContentReader input, SkinnedEffect existingInstance)
        {
            var effect = new SkinnedEffect(input.GetGraphicsDevice())
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
