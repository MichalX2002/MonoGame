// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using MonoGame.Utilities.IO;
using System;

namespace Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler
{
    [ContentTypeWriter]
    class SoundEffectWriter : BuiltInContentWriter<SoundEffectContent>
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
            output.Write(value.duration);

            output.Write(value.dataLength);

            byte[] buffer = RecyclableMemoryManager.Instance.GetBlock();
            try
            {
                long leftToRead = value.dataLength;
                int read;
                while (leftToRead > 0 && (read = value.data.Read(buffer, 0, (int)Math.Min(leftToRead, buffer.Length))) > 0)
                {
                    output.Write(buffer, 0, read);
                    leftToRead -= read;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                RecyclableMemoryManager.Instance.ReturnBlock(buffer, null);
                value.data.Dispose();
            }
        }
    }
}
