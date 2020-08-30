// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Content.Pipeline.Processors;

namespace MonoGame.Framework.Content.Pipeline.Serialization.Compiler
{
    [ContentTypeWriter]
    internal class SoundEffectWriter : BuiltInContentWriter<SoundEffectContent>
    {
        /// <summary>
        /// Writes the value to the output.
        /// </summary>
        /// <param name="output">The output writer object.</param>
        /// <param name="value">The value to write to the output.</param>
        protected internal override void Write(ContentWriter output, SoundEffectContent value)
        {
            output.Write(value.format.Length);
            output.Write(value.format);

            output.Write(value.loopStart);
            output.Write(value.loopLength);
            output.WriteObject(value.duration);

            output.Write(value.dataLength);

            try
            {
                Span<byte> buffer = stackalloc byte[4096];
                int leftToRead = value.dataLength;
                while (leftToRead > 0)
                {
                    int read = value.data.Read(buffer.Slice(0, Math.Min(buffer.Length, leftToRead)));
                    if (read == 0)
                        break;

                    output.Write(buffer.Slice(0, read));
                    leftToRead -= read;
                }

                if (leftToRead > 0)
                    throw new ArgumentException(
                        "Failed to read the specified amount of data.", nameof(value));
            }
            finally
            {
                value.data.Dispose();
            }
        }
    }
}
