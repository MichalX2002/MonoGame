// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Numerics;
using MonoGame.Framework.Content.Pipeline.Graphics;

namespace MonoGame.Framework.Content.Pipeline.Serialization.Compiler
{
    [ContentTypeWriter]
    class SkinnedEffectWriter : BuiltInContentWriter<SkinnedMaterialContent>
    {
        protected internal override void Write(ContentWriter output, SkinnedMaterialContent value)
        {
            bool hasTexture = value.Textures.ContainsKey(SkinnedMaterialContent.TextureKey);
            output.WriteExternalReference(hasTexture ? value.Texture : null);

            output.Write(value.WeightsPerVertex.GetValueOrDefault(4));
            output.Write(value.DiffuseColor ?? Vector3.One);
            output.Write(value.EmissiveColor ?? Vector3.Zero);
            output.Write(value.SpecularColor ?? Vector3.Zero);
            output.Write(value.SpecularPower ?? 0);
            output.Write(value.Alpha ?? 1.0f);
        }
    }
}
