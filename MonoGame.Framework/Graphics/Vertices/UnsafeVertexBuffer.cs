using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class UnsafeVertexBuffer : VertexBufferBase
    {
        private readonly bool _isDynamic;
        
        public BufferUsage BufferUsage { get; private set; }

        protected UnsafeVertexBuffer(GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, BufferUsage bufferUsage, bool dynamic)
        {
            this.GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(
                nameof(graphicsDevice), FrameworkResources.ResourceCreationWhenDeviceIsNull);
            this.VertexDeclaration = vertexDeclaration;
            this.BufferUsage = bufferUsage;

            // Make sure the graphics device is assigned in the vertex declaration.
            if (vertexDeclaration.GraphicsDevice != graphicsDevice)
                vertexDeclaration.GraphicsDevice = graphicsDevice;

            _isDynamic = dynamic;

            PlatformConstruct();
        }

        public UnsafeVertexBuffer(GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, BufferUsage bufferUsage) :
            this(graphicsDevice, vertexDeclaration, bufferUsage, false)
        {
        }

        public UnsafeVertexBuffer(GraphicsDevice graphicsDevice, Type type, BufferUsage bufferUsage) :
            this(graphicsDevice, VertexDeclaration.FromType(type), bufferUsage, false)
        {
        }

        /// <summary>
        /// The GraphicsDevice is resetting, so GPU resources must be recreated.
        /// </summary>
        internal protected override void GraphicsDeviceResetting()
        {
            PlatformGraphicsDeviceResetting();
        }
        
        public void GetData(IntPtr buffer, int startIndex, int elementCount, int vertexStride)
        {
            PlatformGetData(buffer, startIndex, elementCount, vertexStride);
        }

        public void SetData(IntPtr data, int elementCount)
        {
            SetData(data, elementCount, VertexDeclaration.VertexStride);
        }

        public void SetData(IntPtr data, int elementCount, int elementSize)
        {
            int stride = VertexDeclaration.VertexStride;
            SetData(data, 0, elementCount, elementSize, stride, SetDataOptions.Discard);
        }   
        
        public void SetData(IntPtr data, int startIndex, int elementCount, int elementSize, int vertexStride, SetDataOptions options)
        {
            if (elementSize < vertexStride)
                throw new ArgumentOutOfRangeException(
                    nameof(vertexStride), "Element size is misaligned with stride.");

            if (elementCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(elementCount), $"Cannot upload zero elements.");

            PlatformSetData(data, startIndex, elementCount, elementSize, vertexStride, options);
        }
    }
}
