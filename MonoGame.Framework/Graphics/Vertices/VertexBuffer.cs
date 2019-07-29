// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Utilities;
using System;

namespace MonoGame.Framework.Graphics
{
    public partial class VertexBuffer : BufferBase
    {
        public BufferUsage BufferUsage { get; private set; }
        public VertexDeclaration VertexDeclaration { get; protected set; }

        #region Constructors

        protected VertexBuffer(
            GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int capacity, BufferUsage bufferUsage, bool dynamic) :
            base(capacity)
        {
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(
                nameof(graphicsDevice), FrameworkResources.ResourceCreationWhenDeviceIsNull);

            BufferUsage = bufferUsage;
            VertexDeclaration = vertexDeclaration;

            // Make sure the graphics device is assigned in the vertex declaration.
            if (vertexDeclaration.GraphicsDevice != graphicsDevice)
                vertexDeclaration.GraphicsDevice = graphicsDevice;

            _isDynamic = dynamic;
            PlatformConstruct();
        }

        public VertexBuffer(GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage bufferUsage) :
            this(graphicsDevice, vertexDeclaration, vertexCount, bufferUsage, false)
        {
        }

        public VertexBuffer(GraphicsDevice graphicsDevice, Type type, int vertexCount, BufferUsage bufferUsage) :
            this(graphicsDevice, VertexDeclaration.FromType(type), vertexCount, bufferUsage, false)
        {
        }

        #endregion

        #region GetData

        /// <summary>
        /// Get the vertex data from this vertex buffer with an optional buffer offset.
        /// </summary>
        /// <typeparam name="T">The struct you want to fill.</typeparam>
        /// <param name="destination">The span to be filled.</param>
        /// <param name="offsetInBytes">The offset to the first element in the vertex buffer in bytes.</param>
        /// <param name="destinationStride">The size of how a vertex buffer element should be interpreted.</param>
        /// <remarks>
        /// Note that this pulls data from VRAM into main memory and because of that it's an expensive operation.
        /// It is often a better idea to keep a copy of the data in main memory.
        /// </remarks>
        public void GetData<T>(
            int offsetInBytes, Span<T> destination, int destinationStride = 0)
            where T : unmanaged
        {
            if (destinationStride == 0)
                destinationStride = ReflectionHelpers.SizeOf<T>.Get();

            var vertexByteSize = Capacity * VertexDeclaration.VertexStride;
            if (destinationStride > vertexByteSize)
                throw new ArgumentOutOfRangeException(
                    nameof(destinationStride), "Vertex stride can not be larger than the vertex buffer size.");

            if (BufferUsage == BufferUsage.WriteOnly)
                throw new NotSupportedException(
                    $"Calling {nameof(GetData)} on a resource that was created with {BufferUsage.WriteOnly} is not supported.");

            if (destination.Length * destinationStride > vertexByteSize)
                throw new InvalidOperationException(
                    "The span is not the correct size for the amount of data requested.");

            PlatformGetData(offsetInBytes, destination, destinationStride);
        }

        public void GetData<T>(Span<T> destination, int vertexStride = 0)
            where T : unmanaged
        {
            GetData(0, destination, vertexStride);
        }

        #endregion

        #region SetData

        /// <summary>
        /// Sets the vertex buffer data.
        /// </summary>
        /// <typeparam name="T">Type of elements in the data array.</typeparam>
        /// <param name="offsetInBytes">Offset in bytes from the beginning of the vertex buffer to start copying to.</param>
        /// <param name="data">The span of vertex data.</param>
        /// <param name="dataStride">
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
        /// you would call this method as follows (note the use of <paramref name="offsetInBytes"/>:
        /// <code>
        /// var texCoords = new Vector2[vertexCount];
        /// vertexBuffer.SetData(offsetInBytes: 12, texCoords);
        /// </code>
        /// </remarks>
        public unsafe void SetData<T>(
            int offsetInBytes, ReadOnlySpan<T> data, int dataStride = 0, SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            if (data.IsEmpty)
                throw new ArgumentNullException(nameof(data));

            if (dataStride == 0)
                dataStride = sizeof(T);

            var vertexByteSize = data.Length * VertexDeclaration.VertexStride;
            if (dataStride > vertexByteSize)
                throw new ArgumentOutOfRangeException(
                    nameof(dataStride), "Data stride can not be larger than the vertex buffer size.");

            if (data.Length > 1 && data.Length * sizeof(T) > vertexByteSize)
                throw new InvalidOperationException("The vertex stride is larger than the vertex buffer.");

            if (dataStride < sizeof(T))
                throw new ArgumentOutOfRangeException(
                    $"The data stride must be greater than or equal to the size of the specified data ({sizeof(T)}).");

            PlatformSetData(offsetInBytes, data, dataStride, options);
            Count = data.Length * sizeof(T) / VertexDeclaration.VertexStride;
        }

        public unsafe void SetData<T>(
            ReadOnlySpan<T> data, int dataStride = 0, SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(0, data, dataStride, options);
        }

        #region Span<T> Overloads

        public unsafe void SetData<T>(
            int offsetInBytes, Span<T> data, int dataStride = 0, SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(offsetInBytes, (ReadOnlySpan<T>)data, dataStride, options);
        }

        public unsafe void SetData<T>(
            Span<T> data, int vertexStride = 0, SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(0, data, vertexStride, options);
        }

        #endregion

        #endregion
    }
}