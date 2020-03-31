// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Graphics
{
    public partial class IndexBuffer : BufferBase
    {
        internal int _elementSize;

        public IndexElementSize ElementSize { get; }

        #region Constructors

        protected IndexBuffer(
            GraphicsDevice graphicsDevice,
            IndexElementSize elementSize,
            int capacity,
            BufferUsage bufferUsage,
            bool isDynamic) :
            base(graphicsDevice, capacity, bufferUsage, isDynamic)
        {
            if (elementSize == IndexElementSize.Int32)
            {
                if (graphicsDevice.GraphicsProfile == GraphicsProfile.Reach)
                    throw new NotSupportedException(
                        $"The current graphics profile does not support an element size of " +
                        $"{IndexElementSize.Int32}; use {IndexElementSize.Short16} instead.");
            }
            else if (elementSize != IndexElementSize.Short16)
            {
                throw new ArgumentOutOfRangeException(nameof(elementSize));
            }
            ElementSize = elementSize;
            _elementSize = ElementSize == IndexElementSize.Int32 ? 4 : 2;

            PlatformConstruct();
        }

        public IndexBuffer(
            GraphicsDevice graphicsDevice, 
            IndexElementSize elementSize, 
            int capacity,
            BufferUsage bufferUsage) :
            this(graphicsDevice, elementSize, capacity, bufferUsage, false)
        {
        }

        #endregion

        #region GetData

        public unsafe void GetData<T>(int offsetInBytes, Span<T> destination) where T : unmanaged
        {
            if (BufferUsage == BufferUsage.WriteOnly)
                throw new NotSupportedException(
                    $"Calling {nameof(GetData)} on a resource that was created with {BufferUsage.WriteOnly} is not supported.");

            int bufferSize = Capacity * _elementSize;
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
            int byteOffset,
            ReadOnlySpan<T> data,
            SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            if (data.IsEmpty)
                throw new ArgumentEmptyException(nameof(data));

            int bufferBytes = Capacity * _elementSize;
            int requestedBytes = data.Length * sizeof(T);

            if (requestedBytes > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(data), "The buffer doesn't have enough capacity.");

            if (byteOffset + requestedBytes > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(byteOffset), "The range reaches beyond the buffer.");

            PlatformSetData(byteOffset, data, options);
            Count = data.Length * sizeof(T) / _elementSize;
        }

        public void SetData<T>(
            ReadOnlySpan<T> data,
            SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(0, data, options);
        }

        public void SetData<T>(
            int byteOffset,
            Span<T> data,
            SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(byteOffset, (ReadOnlySpan<T>)data, options);
        }

        public void SetData<T>(
            Span<T> data,
            SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(0, data, options);
        }

        #endregion
    }
}