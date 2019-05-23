// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using MonoGame.Utilities;

namespace Microsoft.Xna.Framework.Graphics
{
    public class IndexBuffer : IndexBufferBase
    {
        public BufferUsage BufferUsage { get; private set; }

        protected IndexBuffer(
            GraphicsDevice graphicsDevice, IndexElementSize indexElementSize, int indexCount, BufferUsage usage, bool dynamic)
        {
            this.GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(
                nameof(graphicsDevice), FrameworkResources.ResourceCreationWhenDeviceIsNull);

            this.IndexElementSize = indexElementSize;
            this.IndexCount = indexCount;
            this.BufferUsage = usage;

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

        /// <summary>
        /// Gets the relevant IndexElementSize enum value for the given type.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="type">The type to use for the index buffer</param>
        /// <returns>The IndexElementSize enum value that matches the type</returns>
        static IndexElementSize SizeForType(GraphicsDevice graphicsDevice, Type type)
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

        public void GetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) where T : struct
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length < (startIndex + elementCount))
                throw new InvalidOperationException(
                    $"The array specified in the {nameof(data)} parameter is not the correct size for the amount of data requested.");

            if (BufferUsage == BufferUsage.WriteOnly)
                throw new NotSupportedException(
                    $"This {nameof(IndexBuffer)} was created with a usage type of {BufferUsage.WriteOnly}. " +
                    $"Calling {nameof(GetData)} on a resource that was created with {BufferUsage.WriteOnly} is not supported.");

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                PlatformGetData(offsetInBytes, ptr, startIndex, elementCount);
            }
            finally
            {
                handle.Free();
            }
        }

        public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            this.GetData(0, data, startIndex, elementCount);
        }

        public void GetData<T>(T[] data) where T : struct
        {
            this.GetData(0, data, 0, data.Length);
        }

        protected void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, SetDataOptions options) where T : struct
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length < (startIndex + elementCount))
                throw new InvalidOperationException(
                    $"The array specified in the {nameof(data)} parameter is not the correct size for the amount of data requested.");

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                SetData(offsetInBytes, ptr, startIndex, elementCount, options);
            }
            finally
            {
                handle.Free();
            }
        }

        public void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) where T : struct
        {
            SetData(offsetInBytes, data, startIndex, elementCount, SetDataOptions.None);
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            SetData(0, data, startIndex, elementCount);
        }

        public void SetData<T>(T[] data) where T : struct
        {
            SetData(data, 0, data.Length);
        }

        public void SetData(IntPtr data, int elementCount, SetDataOptions options)
        {
            SetData(0, data, 0, elementCount, options);
        }

        public void SetData(
            int offsetInBytes, IntPtr data, int startIndex, int elementCount, SetDataOptions options)
        {
            if (data == IntPtr.Zero)
                throw new ArgumentNullException(nameof(data));

            PlatformSetData(offsetInBytes, data, startIndex, elementCount, options);
            IndexCount = elementCount;
        }
    }
}