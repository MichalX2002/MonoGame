// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Content.Pipeline.Serialization.Compiler
{
    /// <summary>
    /// Writes a <see cref="BoundingBox"/> value to the output.
    /// </summary>
    [ContentTypeWriter]
    class BoundingBoxWriter : BuiltInContentWriter<BoundingBox>
    {
        /// <summary>
        /// Writes the value to the output.
        /// </summary>
        /// <param name="output">The output writer object.</param>
        /// <param name="value">The value to write to the output.</param>
        protected internal override void Write(ContentWriter output, BoundingBox value)
        {
            output.Write(value.Min);
            output.Write(value.Max);
        }
    }
}
