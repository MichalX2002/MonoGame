// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class UnsafeIndexBuffer : IndexBufferBase
    {
        public BufferUsage BufferUsage { get; private set; }

        protected UnsafeIndexBuffer(GraphicsDevice graphicsDevice, IndexElementSize indexElementSize, BufferUsage usage, bool dynamic)
        {
            this.GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(
                nameof(graphicsDevice), FrameworkResources.ResourceCreationWhenDeviceIsNull);

            this.IndexElementSize = indexElementSize;
            this.BufferUsage = usage;

            _isDynamic = dynamic;

            PlatformConstruct();
        }

        public UnsafeIndexBuffer(GraphicsDevice graphicsDevice, IndexElementSize indexElementSize, BufferUsage bufferUsage) :
            this(graphicsDevice, indexElementSize, bufferUsage, false)
        {
        }

        public void GetData(IntPtr buffer, int startIndex, int elementCount)
        {
            if (elementCount > IndexCount)
                throw new ArgumentOutOfRangeException(nameof(elementCount),
                    "Cannot retrieve more indices then the buffer currently holds.");
            
            PlatformGetData(0, buffer, startIndex, elementCount);
        }

        public void GetData(IntPtr buffer)
        {
            GetData(buffer, 0, IndexCount);
        }

        public void SetData(IntPtr data, int elementCount)
        {
            SetData(data, 0, elementCount, SetDataOptions.Discard);
        }

        public void SetData(IntPtr data, int startIndex, int elementCount, SetDataOptions options)
        {
            if (elementCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(elementCount), "Cannot upload zero elements.");

            IndexCount = elementCount;
            PlatformSetData(0, data, startIndex, elementCount, options);
        }
    }
}
