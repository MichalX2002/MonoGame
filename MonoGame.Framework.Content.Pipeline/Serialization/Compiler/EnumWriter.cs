// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Content.Pipeline.Serialization.Compiler
{
    /// <summary>
    /// Writes the enum value to the output.
    /// Usually 32 bit, but can be other sizes if <typeparamref name="TEnum"/> is not of type <see cref="int"/>.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to write.</typeparam>
    [ContentTypeWriter]
    class EnumWriter<TEnum> : BuiltInContentWriter<TEnum>
        where TEnum : Enum
    {
        Type _underlyingType;
        ContentTypeWriter _underlyingTypeWriter;

        /// <inheritdoc/>
        internal override void OnAddedToContentWriter(ContentWriter output)
        {
            base.OnAddedToContentWriter(output);
            _underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
            _underlyingTypeWriter = output.GetTypeWriter(_underlyingType);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(EnumReader<>).FullName + "[[" + GetRuntimeType(targetPlatform) + "]]";
        }

        protected internal override void Write(ContentWriter output, TEnum value)
        {
            output.WriteRawObject(Convert.ChangeType(value, _underlyingType), _underlyingTypeWriter);
        }
    }
}