// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content
{
    class EnvironmentMapEffectReader : ContentTypeReader<EnvironmentMapEffect>
    {
        protected internal override EnvironmentMapEffect Read(ContentReader input, EnvironmentMapEffect existingInstance)
        {
            var effect = new EnvironmentMapEffect(input.GraphicsDevice)
            {
                Texture = input.ReadExternalReference<Texture>() as Texture2D,
                EnvironmentMap = input.ReadExternalReference<TextureCube>() as TextureCube,
                EnvironmentMapAmount = input.ReadSingle(),
                EnvironmentMapSpecular = input.ReadVector3(),
                FresnelFactor = input.ReadSingle(),
                DiffuseColor = input.ReadVector3(),
                EmissiveColor = input.ReadVector3(),
                Alpha = input.ReadSingle()
            };
            return effect;
        }
    }
}
