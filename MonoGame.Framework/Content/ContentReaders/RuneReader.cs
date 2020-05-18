// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Text;

namespace MonoGame.Framework.Content
{
    internal class RuneReader : ContentTypeReader<Rune>
    {
        public RuneReader()
        {
        }

        protected internal override Rune Read(ContentReader input, Rune existingInstance)
        {
            return (Rune)input.Read7BitEncodedInt();
        }
    }
}
