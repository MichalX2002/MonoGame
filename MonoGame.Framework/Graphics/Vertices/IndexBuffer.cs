// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Graphics
{
    public partial class IndexBuffer : BufferBase
    {
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
            if (elementSize == IndexElementSize.Int)
            {
                if (graphicsDevice.GraphicsProfile == GraphicsProfile.Reach)
                    throw new NotSupportedException(
                        $"The current graphics profile does not support an element size of " +
                        $"{elementSize}; use {IndexElementSize.Short} instead.");
            }
            else if (elementSize != IndexElementSize.Short)
            {
                throw new ArgumentOutOfRangeException(nameof(elementSize));
            }
            ElementSize = elementSize;

            if (IsValidThreadContext)
                PlatformConstruct();
            else
                Threading.BlockOnMainThread(PlatformConstruct);
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

        public unsafe void GetData<T>(int offsetInBytes, Span<T> destination) 
            where T : unmanaged
        {
            if (BufferUsage == BufferUsage.WriteOnly)
                throw new InvalidOperationException(FrameworkResources.WriteOnlyResource);

            AssertMainThread(true);

            int bufferSize = Capacity * (int)ElementSize;
            int requestedBytes = destination.Length * sizeof(T);

            if (requestedBytes > bufferSize)
                throw new ArgumentOutOfRangeException(
                    nameof(destination), "The amount of data requested exceeds the buffer capacity.");

            if (offsetInBytes + requestedBytes > bufferSize)
                throw new ArgumentOutOfRangeException(
                    nameof(offsetInBytes), "The requested range reaches beyond the buffer.");

            PlatformGetData(offsetInBytes, destination);
        }

        public void GetData<T>(Span<T> destination) 
            where T : unmanaged
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

            AssertMainThread(true);

            int bufferBytes = Capacity * (int)ElementSize;
            int requestedBytes = data.Length * sizeof(T);

            if (requestedBytes > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(data), "The buffer doesn't have enough capacity.");

            if (byteOffset + requestedBytes > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(byteOffset), "The range reaches beyond the buffer.");

            PlatformSetData(byteOffset, data, options);
            Count = data.Length * sizeof(T) / (int)ElementSize;
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