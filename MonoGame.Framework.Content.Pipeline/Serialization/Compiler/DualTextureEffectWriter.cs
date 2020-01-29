// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Content.Pipeline.Graphics;

namespace MonoGame.Framework.Content.Pipeline.Serialization.Compiler
{
    [ContentTypeWriter]
    class DualTextureEffectWriter : BuiltInContentWriter<DualTextureMaterialContent>
    {
        protected internal override void Write(ContentWriter output, DualTextureMaterialContent value)
        {
            bool hasTexture = value.Textures.ContainsKey(DualTextureMaterialContent.TextureKey);
            output.WriteExternalReference(hasTexture ? value.Texture : null);

            bool hasTexture2 = value.Textures.ContainsKey(DualTextureMaterialContent.Texture2Key);
            output.WriteExternalReference(hasTexture2 ? value.Texture2 : null);
            
            output.Write(value.DiffuseColor ?? Vector3.One);
            output.Write(value.Alpha ?? 1.0f);
            output.Write(value.VertexColorEnabled ?? false);
        }
    }
}
