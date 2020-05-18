// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Content.Pipeline.Graphics;

namespace MonoGame.Framework.Content.Pipeline.Serialization.Compiler
{
    [ContentTypeWriter]
    public class SpriteFontContentWriter : ContentTypeWriter<SpriteFontContent>
    {
        protected internal override void Write(ContentWriter output, SpriteFontContent value)
        {
            output.WriteObject(value.Texture);
            output.WriteObject(value.Regions);
            output.WriteObject(value.Croppings);
            output.WriteObject(value.CharacterMap);
            output.Write(value.VerticalLineSpacing);
            output.Write(value.HorizontalSpacing);
            output.WriteObject(value.Kerning);

            var hasDefChar = value.DefaultCharacter.HasValue;
            output.Write(hasDefChar);
            if (hasDefChar)
                output.Write(value.DefaultCharacter.Value.Value);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // Base the reader type string from a known public class in the same namespace in the same assembly
            Type type = typeof(ContentReader);
            string readerType = type.Namespace + ".SpriteFontReader, " + type.Assembly.FullName;
            return readerType;
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            // Base the reader type string from a known public class in the same namespace in the same assembly
            Type type = typeof(ContentReader);
            string readerType = type.Namespace + ".SpriteFontReader, " + type.AssemblyQualifiedName;
            return readerType;
        }

        protected internal override bool ShouldCompressContent(TargetPlatform targetPlatform, object value)
        {
            return false;
        }
    }
}
