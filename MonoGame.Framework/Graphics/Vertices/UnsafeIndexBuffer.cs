using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class UnsafeIndexBuffer : IndexBufferBase
    {
        private readonly bool _isDynamic;

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

        protected int GetIndexElementSize()
        {
            return this.IndexElementSize == IndexElementSize.SixteenBits ? 2 : 4;
        }

        public UnsafeIndexBuffer(GraphicsDevice graphicsDevice, IndexElementSize indexElementSize, BufferUsage bufferUsage) :
            this(graphicsDevice, indexElementSize, bufferUsage, false)
        {
        }

        /// <summary>
        /// The GraphicsDevice is resetting, so GPU resources must be recreated.
        /// </summary>
        internal protected override void GraphicsDeviceResetting()
        {
            PlatformGraphicsDeviceResetting();
        }

        public void GetData(IntPtr buffer, int startIndex, int elementCount)
        {
            if (elementCount > IndexCount)
                throw new ArgumentOutOfRangeException(nameof(elementCount),
                    "Cannot retrieve more indices then the buffer currently holds.");

            PlatformGetData(buffer, startIndex, elementCount);
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

            PlatformSetData(data, startIndex, elementCount, options);
        }
    }
}
