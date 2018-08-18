
using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public abstract partial class VertexBufferBase : BufferBase
    {
        private int _lastSize;
        internal SharpDX.Direct3D11.Buffer _buffer;
        private SharpDX.Direct3D11.Buffer _cachedStagingBuffer;

        protected void PlatformConstruct()
        {
        }

        private void PlatformGraphicsDeviceResetting()
        {
            SharpDX.Utilities.Dispose(ref _buffer);
        }

        void GenerateIfRequired(int size)
        {
            if (_lastSize >= size)
                return;

            if (_buffer != null)
                SharpDX.Utilities.Dispose(ref _buffer);

            // TODO: To use Immutable resources we would need to delay creation of 
            // the Buffer until SetData() and recreate them if set more than once.

            var accessflags = SharpDX.Direct3D11.CpuAccessFlags.None;
            var usage = SharpDX.Direct3D11.ResourceUsage.Default;

            if (_isDynamic)
            {
                accessflags |= SharpDX.Direct3D11.CpuAccessFlags.Write;
                usage = SharpDX.Direct3D11.ResourceUsage.Dynamic;
            }

            _buffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice._d3dDevice,
                                                        size,
                                                        usage,
                                                        SharpDX.Direct3D11.BindFlags.VertexBuffer,
                                                        accessflags,
                                                        SharpDX.Direct3D11.ResourceOptionFlags.None,
                                                        0  // StructureSizeInBytes
                                                        );
            _lastSize = size;
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

        protected void PlatformGetData(
            int offsetInBytes, IntPtr ptr, int startIndex, int elementCount, int elementSizeInBytes, int vertexStride)
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

                int startByteOffset = startIndex * vertexStride;
                IntPtr dataPtr = ptr + startByteOffset;

                lock (deviceContext)
                {
                    // Map the staging resource to a CPU accessible memory
                    var box = deviceContext.MapSubresource(
                        _cachedStagingBuffer, 0, SharpDX.Direct3D11.MapMode.Read, SharpDX.Direct3D11.MapFlags.None);
                    IntPtr boxPtr = box.DataPointer + offsetInBytes;

                    if (vertexStride == elementSizeInBytes)
                    {
                        SharpDX.Utilities.CopyMemory(dataPtr, boxPtr, vertexStride * elementCount);
                    }
                    else
                    {
                        for (int i = 0; i < elementCount; i++)
                            SharpDX.Utilities.CopyMemory(dataPtr + i * vertexStride, boxPtr + i * vertexStride, vertexStride);
                    }

                    // Make sure that we unmap the resource in case of an exception
                    deviceContext.UnmapSubresource(_cachedStagingBuffer, 0);
                }
            }
        }

        protected void PlatformSetData(
            int offsetInBytes, IntPtr ptr, int startIndex,
            int elementCount, int elementSizeInBytes, int vertexStride, SetDataOptions options)
        {
            int size = vertexStride * elementCount;
            GenerateIfRequired(size);

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
                    IntPtr boxPtr = box.DataPointer + offsetInBytes;

                    if (vertexStride == elementSizeInBytes)
                    {
                        SharpDX.Utilities.CopyMemory(
                            boxPtr, ptr + startIndex * vertexStride, size);
                    }
                    else
                    {
                        for (int i = 0; i < elementCount; i++)
                        {
                            SharpDX.Utilities.CopyMemory(
                                boxPtr + i * vertexStride, ptr + (startIndex + i) * elementSizeInBytes, vertexStride);
                        }
                    }

                    deviceContext.UnmapSubresource(_buffer, 0);
                }
            }
            else
            {
                int startBytes = startIndex * vertexStride;
                IntPtr dataPtr = ptr + startBytes;

                if (vertexStride == elementSizeInBytes)
                {
                    var box = new SharpDX.DataBox(dataPtr, size, 0);
                    var region = new SharpDX.Direct3D11.ResourceRegion
                    {
                        Top = 0,
                        Front = 0,
                        Back = 1,
                        Bottom = 1,
                        Left = offsetInBytes,
                        Right = offsetInBytes + (size)
                    };

                    lock (deviceContext)
                        deviceContext.UpdateSubresource(box, _buffer, 0, region);
                }
                else
                {
                    if (_cachedStagingBuffer == null)
                        CreatedCachedStagingBuffer();

                    lock (deviceContext)
                    {
                        deviceContext.CopyResource(_buffer, _cachedStagingBuffer);

                        // Map the staging resource to a CPU accessible memory
                        var box = deviceContext.MapSubresource(_cachedStagingBuffer, 0, SharpDX.Direct3D11.MapMode.Read,
                            SharpDX.Direct3D11.MapFlags.None);

                        for (int i = 0; i < elementCount; i++)
                            SharpDX.Utilities.CopyMemory(
                                box.DataPointer + i * vertexStride + offsetInBytes,
                                dataPtr + i * elementSizeInBytes, vertexStride);

                        // Make sure that we unmap the resource in case of an exception
                        deviceContext.UnmapSubresource(_cachedStagingBuffer, 0);

                        // Copy back from staging resource to real buffer.
                        deviceContext.CopyResource(_cachedStagingBuffer, _buffer);
                    }
                }
            }
            _lastSize = size;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                SharpDX.Utilities.Dispose(ref _buffer);

            base.Dispose(disposing);
        }
    }
}
