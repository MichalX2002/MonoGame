// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

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
                nameof(graphicsDevice), FrameworkResources.ResourceCreationWithNullDevice);

            BufferUsage = usage;
            IndexElementSize = indexElementSize;
            _indexElementSize = IndexElementSize == IndexElementSize.SixteenBits ? 2 : 4;

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

            int bufferSize = Capacity * _indexElementSize;
            int requestedBytes = destination.Length * sizeof(T);

            if (requestedBytes > bufferSize)
                throw new ArgumentOutOfRangeException(
                    nameof(destination), "The amount of data requested exceeds the buffer capacity.");

            if (offsetInBytes + requestedBytes > bufferSize)
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
            int byteOffset, ReadOnlySpan<T> data, SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            if (data.IsEmpty)
                throw new ArgumentEmptyException(nameof(data));

            int bufferBytes = Capacity * _indexElementSize;
            int requestedBytes = data.Length * sizeof(T);

            if (requestedBytes > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(data), "The buffer doesn't have enough capacity.");

            if (byteOffset + requestedBytes > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(byteOffset), "The range reaches beyond the buffer.");

            PlatformSetData(byteOffset, data, options);
            Count = data.Length * sizeof(T) / _indexElementSize;
        }

        public void SetData<T>(
            ReadOnlySpan<T> data, SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(0, data, options);
        }

        #region Span<T> Overloads

        public void SetData<T>(
            int byteOffset, Span<T> data, SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(byteOffset, (ReadOnlySpan<T>)data, options);
        }

        public void SetData<T>(
            Span<T> data, SetDataOptions options = SetDataOptions.None)
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
        private static unsafe IndexElementSize SizeForType(GraphicsDevice graphicsDevice, Type type)
        {
            switch (Marshal.SizeOf(type))
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