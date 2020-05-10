// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Graphics
{
    public partial class VertexBuffer : BufferBase
    {
        /// <summary>
        /// The vertex declaration of the vertex type that this buffer contains.
        /// </summary>
        public VertexDeclaration VertexDeclaration { get; protected set; }

        #region Constructors

        protected VertexBuffer(
            GraphicsDevice graphicsDevice,
            VertexDeclaration vertexDeclaration,
            int capacity,
            BufferUsage bufferUsage,
            bool isDynamic) :
            base(graphicsDevice, capacity, bufferUsage, isDynamic)
        {
            // Make sure the graphics device is assigned in the vertex declaration.
            if (vertexDeclaration.GraphicsDevice != graphicsDevice)
                vertexDeclaration.GraphicsDevice = graphicsDevice;

            VertexDeclaration = vertexDeclaration;

            if (IsValidThreadContext)
                PlatformConstruct();
            else
                Threading.BlockOnMainThread(PlatformConstruct);
        }

        public VertexBuffer(
            GraphicsDevice graphicsDevice,
            VertexDeclaration vertexDeclaration,
            int capacity,
            BufferUsage bufferUsage) :
            this(graphicsDevice, vertexDeclaration, capacity, bufferUsage, false)
        {
        }

        public VertexBuffer(
            GraphicsDevice graphicsDevice,
            Type vertexType,
            int capacity,
            BufferUsage bufferUsage) :
            this(graphicsDevice, VertexDeclaration.FromType(vertexType), capacity, bufferUsage, false)
        {
        }

        #endregion

        #region GetData

        /// <summary>
        /// Get the vertex data from this vertex buffer with an optional buffer offset.
        /// </summary>
        /// <typeparam name="T">The struct you want to fill.</typeparam>
        /// <param name="destination">The span to be filled.</param>
        /// <param name="byteOffset">The offset to the first element in the vertex buffer in bytes.</param>
        /// <param name="elementStride">The size of how a vertex buffer element should be interpreted.</param>
        /// <remarks>
        /// Note that this pulls data from VRAM into main memory and because of that it's an expensive operation.
        /// It is often a better idea to keep a copy of the data in main memory.
        /// </remarks>
        public unsafe void GetData<T>(
            int byteOffset, Span<T> destination, int elementStride = 0)
            where T : unmanaged
        {
            if (BufferUsage == BufferUsage.WriteOnly)
                throw new InvalidOperationException(FrameworkResources.WriteOnlyResource);

            AssertMainThread(true);

            if (elementStride == 0)
                elementStride = sizeof(T);

            int bufferSize = Capacity * VertexDeclaration.VertexStride;
            int requestedBytes = destination.Length * elementStride;

            if (elementStride > bufferSize)
                throw new ArgumentOutOfRangeException(
                    nameof(elementStride), "The vertex stride may not be larger than the buffer capacity.");

            if (requestedBytes > bufferSize)
                throw new ArgumentOutOfRangeException(
                    nameof(destination), "The amount of data requested exceeds the buffer capacity.");

            if (byteOffset + requestedBytes > bufferSize)
                throw new ArgumentOutOfRangeException(
                    nameof(byteOffset), "The requested range reaches beyond the buffer.");

            PlatformGetData(byteOffset, destination, elementStride);
        }

        public void GetData<T>(Span<T> destination, int elementStride = 0)
            where T : unmanaged
        {
            GetData(0, destination, elementStride);
        }

        #endregion

        #region SetData

        /// <summary>
        /// Sets the vertex buffer data.
        /// </summary>
        /// <typeparam name="T">Type of elements in the data array.</typeparam>
        /// <param name="byteOffset">Offset in bytes from the beginning of the vertex buffer to start copying to.</param>
        /// <param name="data">The span of vertex data.</param>
        /// <param name="elementStride">
        /// Specifies how far apart, in bytes, elements from <paramref name="data"/> should be when 
        /// they are copied into the vertex buffer.
        /// If you specify <c>sizeof(T)</c>, elements from <paramref name="data"/> will be copied into the 
        /// vertex buffer with no padding between each element.
        /// If you specify a value greater than <c>sizeof(T)</c>, elements from <paramref name="data"/> will be copied 
        /// into the vertex buffer with padding between each element.
        /// If you specify <c>0</c> for this parameter, it will be treated as if you had specified <c>sizeof(T)</c>.
        /// With the exception of <c>0</c>, you must specify a value greater than or equal to <c>sizeof(T)</c>.
        /// </param>
        /// <param name="options">The options to use when flushing the data.</param>
        /// <remarks>
        /// Example: If you only want to set the texture coordinate component of the vertex data,
        /// you would call this method as follows (note the use of <paramref name="byteOffset"/>:
        /// <code>
        /// var texCoords = new Vector2[vertexCount];
        /// vertexBuffer.SetData(offsetInBytes: 12, texCoords);
        /// </code>
        /// </remarks>
        public unsafe void SetData<T>(
            int byteOffset,
            ReadOnlySpan<T> data,
            int elementStride = 0,
            SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            if (data.IsEmpty)
                throw new ArgumentEmptyException(nameof(data));

            AssertMainThread(true);

            if (elementStride == 0)
                elementStride = sizeof(T);

            var vertexByteSize = data.Length * VertexDeclaration.VertexStride;
            if (elementStride > vertexByteSize)
                throw new ArgumentOutOfRangeException(
                    nameof(elementStride), "Data stride can not be larger than the vertex buffer size.");

            if (data.Length > 1 && data.Length * sizeof(T) > vertexByteSize)
                throw new InvalidOperationException(
                    "The vertex stride is larger than the vertex buffer.");

            if (elementStride < sizeof(T))
                throw new ArgumentOutOfRangeException(
                    $"The data stride must be greater than or equal to the size of the specified data ({sizeof(T)}).");

            PlatformSetData(byteOffset, data, elementStride, options);
            Count = data.Length * sizeof(T) / VertexDeclaration.VertexStride;
        }

        public unsafe void SetData<T>(
            ReadOnlySpan<T> data,
            int elementStride = 0,
            SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(0, data, elementStride, options);
        }

        public void SetData<T>(
            int byteOffset,
            Span<T> data,
            int elementStride = 0,
            SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(byteOffset, (ReadOnlySpan<T>)data, elementStride, options);
        }

        public void SetData<T>(
            Span<T> data,
            int elementStride = 0,
            SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(0, data, elementStride, options);
        }

        #endregion
    }
}