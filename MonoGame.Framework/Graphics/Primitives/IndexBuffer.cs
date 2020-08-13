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
        public IndexElementType ElementType { get; }

        #region Constructors

        protected IndexBuffer(
            GraphicsDevice graphicsDevice,
            IndexElementType elementType,
            int capacity,
            BufferUsage bufferUsage,
            bool isDynamic) :
            base(graphicsDevice, capacity, bufferUsage, isDynamic)
        {
            if (elementType == IndexElementType.Int32)
            {
                if (graphicsDevice.GraphicsProfile == GraphicsProfile.Reach)
                    throw new NotSupportedException(
                        $"The current graphics profile does not support an element type of " +
                        $"{elementType}; use {IndexElementType.Int16} instead.");
            }
            else if (elementType != IndexElementType.Int16)
            {
                throw new ArgumentOutOfRangeException(nameof(elementType));
            }
            ElementType = elementType;

            if (IsValidThreadContext)
                PlatformConstruct();
            else
                Threading.BlockOnMainThread(PlatformConstruct);
        }

        public IndexBuffer(
            GraphicsDevice graphicsDevice,
            IndexElementType elementType,
            int capacity,
            BufferUsage bufferUsage) :
            this(graphicsDevice, elementType, capacity, bufferUsage, false)
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

            int bufferSize = Capacity * ElementType.TypeSize();
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

            int bufferBytes = Capacity * ElementType.TypeSize();
            int sourceByteCount = source.Length * Unsafe.SizeOf<T>();

            if (sourceByteCount > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(source), "The buffer doesn't have enough capacity.");

            if (byteOffset + sourceByteCount > bufferBytes)
                throw new ArgumentOutOfRangeException(
                    nameof(byteOffset), "The range reaches beyond the buffer.");

            PlatformSetData(byteOffset, MemoryMarshal.AsBytes(source), options);
            Count = sourceByteCount / ElementType.TypeSize();
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

        public static IndexElementType ToIndexElementType(Type type)
        {
            return 
                (type == typeof(short) || type == typeof(ushort)) ? IndexElementType.Int16 :
                (type == typeof(int) || type == typeof(uint)) ? IndexElementType.Int32 : 
                throw new ArgumentException($"Could not determine index element type from {type}.");
        }
    }
}