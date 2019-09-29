// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using TOutput = System.DateTime;

namespace MonoGame.Framework.Content.Pipeline.Serialization.Compiler
{
    /// <summary>
    /// Writes the DateTime value to the output.
    /// </summary>
    [ContentTypeWriter]
    class DateTimeWriter : BuiltInContentWriter<TOutput>
    {
        /// <summary>
        /// Writes the value to the output.
        /// </summary>
        /// <param name="output">The output writer object.</param>
        /// <param name="value">The value to write to the output.</param>
        protected internal override void Write(ContentWriter output, TOutput value)
        {
            ulong ticks = (ulong)value.Ticks & ~((ulong)0xC << 62);
            ulong kind = (ulong)value.Kind << 62;
            output.Write((ulong)(ticks | kind));
        }
    }
}
