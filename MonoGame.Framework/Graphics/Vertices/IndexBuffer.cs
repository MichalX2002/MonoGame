// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

        public void GetData<T>(int byteOffset, Span<T> destination)
            where T : unmanaged
        {
            if (destination.IsEmpty)
                return;

            if (BufferUsage == BufferUsage.WriteOnly)
                throw new InvalidOperationException(FrameworkResources.WriteOnlyResource);

            AssertMainThread(true);

            int bufferSize = Capacity * (int)ElementSize;
            int dstSize = destination.Length * Unsafe.SizeOf<T>();

            if (dstSize > bufferSize)
                throw new ArgumentOutOfRangeException(
                    nameof(destination), "The amount of data requested exceeds the buffer capacity.");

            if (byteOffset + dstSize > bufferSize)
                throw new ArgumentOutOfRangeException(
                    nameof(byteOffset), "The requested range reaches beyond the buffer.");

            PlatformGetData(byteOffset, MemoryMarshal.AsBytes(destination));
        }

        public void GetData<T>(Span<T> destination)
            where T : unmanaged
        {
            GetData(0, destination);
        }

        #endregion

        #region SetData

        public void SetData<T>(
            int byteOffset,
            ReadOnlySpan<T> source,
            SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            if (source.IsEmpty)
                return;

            AssertMainThread(true);

            int bufferBytes = Capacity * (int)ElementSize;
            int srcSize = source.Length * Unsafe.SizeOf<T>();

            if (srcSize > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(source), "The buffer doesn't have enough capacity.");

            if (byteOffset + srcSize > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(byteOffset), "The range reaches beyond the buffer.");

            PlatformSetData(byteOffset, MemoryMarshal.AsBytes(source), options);
            Count = srcSize / (int)ElementSize;
        }

        public void SetData<T>(
            int byteOffset,
            Span<T> source,
            SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(byteOffset, (ReadOnlySpan<T>)source, options);
        }

        public void SetData<T>(
            ReadOnlySpan<T> source,
            SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData(0, source, options);
        }

        public void SetData<T>(
            Span<T> source,
            SetDataOptions options = SetDataOptions.None)
            where T : unmanaged
        {
            SetData((ReadOnlySpan<T>)source, options);
        }

        #endregion

        public static IndexElementSize ToIndexElementSize(Type type)
        {
            return (type == typeof(short) || type == typeof(ushort))
                ? IndexElementSize.Short
                : (type == typeof(int) || type == typeof(uint))
                ? IndexElementSize.Int
                : throw new ArgumentException($"Could not determine index element size from {type}.");
        }
    }
}