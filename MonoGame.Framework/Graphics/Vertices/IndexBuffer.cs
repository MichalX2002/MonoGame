// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Utilities;

namespace MonoGame.Framework.Graphics
{
    public partial class IndexBuffer : BufferBase
    {
        internal int _indexElementSize;

        public BufferUsage BufferUsage { get; }
        public IndexElementSize IndexElementSize { get; }

        #region Constructors

        protected IndexBuffer(
            GraphicsDevice graphicsDevice, IndexElementSize indexElementSize, int capacity, BufferUsage usage, bool dynamic) :
            base(capacity)
        {
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(
                nameof(graphicsDevice), FrameworkResources.ResourceCreationWhenDeviceIsNull);

            BufferUsage = usage;
            IndexElementSize = indexElementSize;
            _indexElementSize = (IndexElementSize == IndexElementSize.SixteenBits ? 2 : 4);

            _isDynamic = dynamic;
            PlatformConstruct();
        }

        protected IndexBuffer(GraphicsDevice graphicsDevice, Type indexType, int indexCount, BufferUsage usage, bool dynamic)
           : this(graphicsDevice, SizeForType(graphicsDevice, indexType), indexCount, usage, dynamic)
        {
        }


        public IndexBuffer(GraphicsDevice graphicsDevice, IndexElementSize indexElementSize, int indexCount, BufferUsage bufferUsage) :
            this(graphicsDevice, indexElementSize, indexCount, bufferUsage, false)
        {
        }

        public IndexBuffer(GraphicsDevice graphicsDevice, Type indexType, int indexCount, BufferUsage usage) :
            this(graphicsDevice, SizeForType(graphicsDevice, indexType), indexCount, usage, false)
        {
        }

        #endregion

        #region GetData

        public unsafe void GetData<T>(int offsetInBytes, Span<T> destination) where T : unmanaged
        {
            if (BufferUsage == BufferUsage.WriteOnly)
                throw new NotSupportedException(
                    $"Calling {nameof(GetData)} on a resource that was created with {BufferUsage.WriteOnly} is not supported.");

            int bufferBytes = Capacity * _indexElementSize;
            int requestedBytes = destination.Length * sizeof(T);

            if (requestedBytes > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(destination), "More bytes than the buffer contains were requested.");

            if (offsetInBytes + requestedBytes > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(offsetInBytes), "The requested range reaches beyond the buffer.");

            PlatformGetData(offsetInBytes, destination);
        }

        public void GetData<T>(Span<T> destination) where T : unmanaged
        {
            GetData(0, destination);
        }

        #endregion

        #region SetData

        public unsafe void SetData<T>(
            int offsetInBytes, ReadOnlySpan<T> data, SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            if (data.IsEmpty)
                throw new ArgumentNullException(nameof(data));

            int bufferBytes = Capacity * _indexElementSize;
            int requestedBytes = data.Length * sizeof(T);

            if (requestedBytes > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(data), "The buffer doesn't have enough capacity.");

            if (offsetInBytes + requestedBytes > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(offsetInBytes), "The range reaches beyond the buffer.");

            PlatformSetData(offsetInBytes, data, options);
            Count = data.Length * sizeof(T) / _indexElementSize;
        }

        public void SetData<T>(ReadOnlySpan<T> data, SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(0, data, options);
        }

        #region Span<T> Overloads

        public void SetData<T>(
            int offsetInBytes, Span<T> data, SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(offsetInBytes, (ReadOnlySpan<T>)data, options);
        }

        public void SetData<T>(Span<T> data, SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(0, data, options);
        }

        #endregion

        #endregion

        /// <summary>
        /// Gets the relevant <see cref="Graphics.IndexElementSize"/> value for the given type.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="type">The type to use for the index buffer</param>
        /// <returns>The <see cref="Graphics.IndexElementSize"/> value that matches the type.</returns>
        private static IndexElementSize SizeForType(GraphicsDevice graphicsDevice, Type type)
        {
            switch (ReflectionHelpers.ManagedSizeOf(type))
            {
                case 2:
                    return IndexElementSize.SixteenBits;

                case 4:
                    if (graphicsDevice.GraphicsProfile == GraphicsProfile.Reach)
                        throw new NotSupportedException(
                            $"The profile does not support an {nameof(IndexElementSize)} of {IndexElementSize.ThirtyTwoBits}; " +
                            $"use {IndexElementSize.SixteenBits} (or a type that has a size of two bytes).");
                    return IndexElementSize.ThirtyTwoBits;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(type), "Index buffers can only be created for types that are sixteen or thirty two bits in length.");
            }
        }
    }
}