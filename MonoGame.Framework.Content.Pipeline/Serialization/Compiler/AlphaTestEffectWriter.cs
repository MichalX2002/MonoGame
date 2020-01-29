// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Content.Pipeline.Serialization.Compiler
{
    [ContentTypeWriter]
    class AlphaTestEffectWriter : BuiltInContentWriter<AlphaTestMaterialContent>
    {
        protected internal override void Write(ContentWriter output, AlphaTestMaterialContent value)
        {
            output.WriteExternalReference(value.Textures.ContainsKey(AlphaTestMaterialContent.TextureKey) ? value.Texture : null);
            output.Write((int)(value.AlphaFunction ?? CompareFunction.Greater));
            output.Write((int)(value.ReferenceAlpha ?? 0));
            output.Write(value.DiffuseColor.GetValueOrDefault());
            output.Write(value.Alpha.GetValueOrDefault());
            output.Write(value.VertexColorEnabled.GetValueOrDefault());
        }
    }
}
