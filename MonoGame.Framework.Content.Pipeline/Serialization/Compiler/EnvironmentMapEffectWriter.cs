// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Content.Pipeline.Graphics;

namespace MonoGame.Framework.Content.Pipeline.Serialization.Compiler
{
    [ContentTypeWriter]
    class EnvironmentMapEffectWriter : BuiltInContentWriter<EnvironmentMapMaterialContent>
    {
        protected internal override void Write(ContentWriter output, EnvironmentMapMaterialContent value)
        {
            output.WriteExternalReference(value.Textures.ContainsKey(EnvironmentMapMaterialContent.TextureKey) ? value.Texture : null);
            output.WriteExternalReference(value.Textures.ContainsKey(EnvironmentMapMaterialContent.EnvironmentMapKey) ? value.EnvironmentMap : null);
            output.Write(value.EnvironmentMapAmount ?? 1.0f);
            output.Write(value.EnvironmentMapSpecular ?? Vector3.Zero);
            output.Write(value.DiffuseColor ?? Vector3.One);
            output.Write(value.EmissiveColor ?? Vector3.Zero);
            output.Write(value.Alpha ?? 1.0f);
        }
    }
}
