﻿
using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Graphics
{
    public partial class VertexBuffer : BufferBase
    {
        internal SharpDX.Direct3D11.Buffer _buffer;
        private SharpDX.Direct3D11.Buffer _cachedStagingBuffer;

        protected void PlatformConstruct()
        {
        }

        private void PlatformGraphicsDeviceResetting()
        {
            SharpDX.Utilities.Dispose(ref _buffer);
        }

        void GenerateIfRequired()
        {
            if (_buffer != null)
                return;

            // TODO: To use Immutable resources we would need to delay creation of 
            // the Buffer until SetData() and recreate them if set more than once.

            var accessflags = SharpDX.Direct3D11.CpuAccessFlags.None;
            var usage = SharpDX.Direct3D11.ResourceUsage.Default;

            if (_isDynamic)
            {
                accessflags |= SharpDX.Direct3D11.CpuAccessFlags.Write;
                usage = SharpDX.Direct3D11.ResourceUsage.Dynamic;
            }

            _buffer = new SharpDX.Direct3D11.Buffer(
                GraphicsDevice._d3dDevice,
                Capacity * VertexDeclaration.VertexStride,
                usage,
                SharpDX.Direct3D11.BindFlags.VertexBuffer,
                accessflags,
                SharpDX.Direct3D11.ResourceOptionFlags.None,
                0);  // StructureSizeInBytes
        }

        void CreatedCachedStagingBuffer()
        {
            if (_cachedStagingBuffer != null)
                return;

            var stagingDesc = _buffer.Description;
            stagingDesc.BindFlags = SharpDX.Direct3D11.BindFlags.None;
            stagingDesc.CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.Read | SharpDX.Direct3D11.CpuAccessFlags.Write;
            stagingDesc.Usage = SharpDX.Direct3D11.ResourceUsage.Staging;
            stagingDesc.OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None;

            _cachedStagingBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice._d3dDevice, stagingDesc);
        }

        protected unsafe void PlatformGetData<T>(
            int offsetInBytes, Span<T> destination, int destinationStride)
            where T : unmanaged
        {
            if (_buffer == null)
                return;

            if (_isDynamic)
            {
                throw new NotImplementedException();
            }
            else
            {
                var deviceContext = GraphicsDevice._d3dContext;

                if (_cachedStagingBuffer == null)
                    CreatedCachedStagingBuffer();

                lock (deviceContext)
                    deviceContext.CopyResource(_buffer, _cachedStagingBuffer);

                lock (deviceContext)
                {
                    // Map the staging resource to a CPU accessible memory
                    var box = deviceContext.MapSubresource(
                        _cachedStagingBuffer, 0, SharpDX.Direct3D11.MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

                    int srcBytes = Capacity * VertexDeclaration.VertexStride;
                    var src = new ReadOnlySpan<T>((void*)(box.DataPointer + offsetInBytes), srcBytes);

                    if (destinationStride == sizeof(T))
                    {
                        // the source and destination use tightly packed data,
                        // we can skip the interleaved copy
                        src.Slice(0, destination.Length).CopyTo(destination);
                    }
                    else
                    {
                        var byteSrc = MemoryMarshal.AsBytes(src);
                        var byteDst = MemoryMarshal.AsBytes(destination);

                        // interleaved copy from buffer to destination
                        for (int i = 0; i < destination.Length; i++)
                        {
                            var srcElement = byteSrc.Slice(i * VertexDeclaration.VertexStride);
                            var dstElement = byteDst.Slice(i * destinationStride, destinationStride);
                            srcElement.Slice(0, destinationStride).CopyTo(dstElement);
                        }
                    }

                    // Make sure that we unmap the resource in case of an exception
                    deviceContext.UnmapSubresource(_cachedStagingBuffer, 0);
                }
            }
        }

        protected unsafe void PlatformSetData<T>(
            int offsetInBytes, ReadOnlySpan<T> data, int dataStride, SetDataOptions options)
            where T : unmanaged
        {
            int bytes = dataStride * data.Length;
            GenerateIfRequired();

            var deviceContext = GraphicsDevice._d3dContext;
            if (_isDynamic)
            {
                // We assume discard by default.
                var mode = SharpDX.Direct3D11.MapMode.WriteDiscard;
                if ((options & SetDataOptions.NoOverwrite) == SetDataOptions.NoOverwrite)
                    mode = SharpDX.Direct3D11.MapMode.WriteNoOverwrite;

                lock (deviceContext)
                {
                    var box = deviceContext.MapSubresource(_buffer, 0, mode, SharpDX.Direct3D11.MapFlags.None);

                    var byteSrc = MemoryMarshal.AsBytes(data);
                    int dstBytes = Capacity * VertexDeclaration.VertexStride;
                    var byteDst = new Span<byte>((void*)(box.DataPointer + offsetInBytes), dstBytes);

                    if (sizeof(T) % dataStride == 0)
                    {
                        byteSrc.CopyTo(byteDst);
                    }
                    else
                    {
                        for (int i = 0; i < data.Length; i++)
                        {
                            var srcElement = byteSrc.Slice(i * sizeof(T), sizeof(T));
                            var dstElement = byteDst.Slice(i * dataStride, sizeof(T));
                            srcElement.CopyTo(dstElement);
                        }
                    }

                    deviceContext.UnmapSubresource(_buffer, 0);
                }
            }
            else
            {
                if (sizeof(T) % dataStride == 0)
                {
                    var region = new SharpDX.Direct3D11.ResourceRegion
                    {
                        Top = 0,
                        Front = 0,
                        Back = 1,
                        Bottom = 1,
                        Left = offsetInBytes,
                        Right = offsetInBytes + bytes
                    };

                    lock (deviceContext)
                    {
                        fixed (T* ptr = &MemoryMarshal.GetReference(data))
                        {
                            var box = new SharpDX.DataBox((IntPtr)ptr, bytes, 0);
                            deviceContext.UpdateSubresource(box, _buffer, 0, region);
                        }
                    }
                }
                else
                {
                    if (_cachedStagingBuffer == null)
                        CreatedCachedStagingBuffer();

                    lock (deviceContext)
                    {
                        deviceContext.CopyResource(_buffer, _cachedStagingBuffer);

                        // Map the staging resource to a CPU accessible memory
                        var box = deviceContext.MapSubresource(
                            _cachedStagingBuffer, 0, SharpDX.Direct3D11.MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

                        int dstBytes = Capacity * VertexDeclaration.VertexStride;
                        var byteSrc = MemoryMarshal.AsBytes(data);
                        var byteDst = new Span<byte>((void*)(box.DataPointer + offsetInBytes), dstBytes);

                        for (int i = 0; i < data.Length; i++)
                        {
                            var srcElement = byteSrc.Slice(i * sizeof(T), sizeof(T));
                            var dstElement = byteDst.Slice(i * dataStride, sizeof(T));
                            srcElement.CopyTo(dstElement);
                        }

                        // Make sure that we unmap the resource in case of an exception
                        deviceContext.UnmapSubresource(_cachedStagingBuffer, 0);

                        // Copy back from staging resource to real buffer.
                        deviceContext.CopyResource(_cachedStagingBuffer, _buffer);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                SharpDX.Utilities.Dispose(ref _buffer);
            base.Dispose(disposing);
        }
    }
}
