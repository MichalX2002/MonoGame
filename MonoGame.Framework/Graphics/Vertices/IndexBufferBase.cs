// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Graphics
{
    public abstract partial class IndexBufferBase : BufferBase
    {
        private IndexElementSize __indexElementSize;
        internal int _indexElementSize;

        public int IndexCount { get; protected set; }
        public IndexElementSize IndexElementSize
        {
            get => __indexElementSize;
            protected set
            {
                _indexElementSize = (value == IndexElementSize.SixteenBits ? 2 : 4);
                __indexElementSize = value;
            }
        }
    }
}
