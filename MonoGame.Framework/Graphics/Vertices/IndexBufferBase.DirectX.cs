
using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public abstract partial class IndexBufferBase : BufferBase
    {
        internal SharpDX.Direct3D11.Buffer _buffer;
        
        protected void PlatformConstruct()
        {
            GenerateIfRequired();
        }

        private void PlatformGraphicsDeviceResetting()
        {
            SharpDX.Utilities.Dispose(ref _buffer);
        }
        
        protected void GenerateIfRequired()
        {
            if (_buffer != null)
                return;

            // TODO: To use true Immutable resources we would need to delay creation of 
            // the Buffer until SetData() and recreate them if set more than once.

            var sizeInBytes = IndexCount * _indexElementSize;

            var accessflags = SharpDX.Direct3D11.CpuAccessFlags.None;
            var resUsage = SharpDX.Direct3D11.ResourceUsage.Default;

            if (_isDynamic)
            {
                accessflags |= SharpDX.Direct3D11.CpuAccessFlags.Write;
                resUsage = SharpDX.Direct3D11.ResourceUsage.Dynamic;
            }
            _buffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice._d3dDevice,
                                                        sizeInBytes,
                                                        resUsage,
                                                        SharpDX.Direct3D11.BindFlags.IndexBuffer,
                                                        accessflags,
                                                        SharpDX.Direct3D11.ResourceOptionFlags.None,
                                                        0  // StructureSizeInBytes
                                                        );
        }

        protected void PlatformGetData(int offsetInBytes, IntPtr data, int startIndex, int elementCount)
        {
            GenerateIfRequired();

            if (_isDynamic)
            {
                throw new NotImplementedException();
            }
            else
            {
                var deviceContext = GraphicsDevice._d3dContext;

                // Copy the texture to a staging resource
                var stagingDesc = _buffer.Description;
                stagingDesc.BindFlags = SharpDX.Direct3D11.BindFlags.None;
                stagingDesc.CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.Read | SharpDX.Direct3D11.CpuAccessFlags.Write;
                stagingDesc.Usage = SharpDX.Direct3D11.ResourceUsage.Staging;
                stagingDesc.OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None;
                using (var stagingBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice._d3dDevice, stagingDesc))
                {
                    lock (GraphicsDevice._d3dContext)
                        deviceContext.CopyResource(_buffer, stagingBuffer);
                    
                    var startBytes = startIndex * _indexElementSize;
                    var dataPtr = data + startBytes;

                    lock (GraphicsDevice._d3dContext)
                    {
                        // Map the staging resource to CPU accessible memory
                        try
                        {
                            var box = deviceContext.MapSubresource(
                                stagingBuffer, 0, SharpDX.Direct3D11.MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

                            SharpDX.Utilities.CopyMemory(dataPtr, box.DataPointer + offsetInBytes, elementCount * _indexElementSize);
                        }
                        finally
                        {
                            // Make sure that we unmap the resource in case of an exception
                            deviceContext.UnmapSubresource(stagingBuffer, 0);
                        }
                    }
                }
            }
        }

        protected void PlatformSetData(
            int offsetInBytes, IntPtr data, int startIndex, int elementCount, SetDataOptions options)
        {
            GenerateIfRequired();

            if (_isDynamic)
            {
                // We assume discard by default.
                var mode = SharpDX.Direct3D11.MapMode.WriteDiscard;
                if ((options & SetDataOptions.NoOverwrite) == SetDataOptions.NoOverwrite)
                    mode = SharpDX.Direct3D11.MapMode.WriteNoOverwrite;

                var d3dContext = GraphicsDevice._d3dContext;
                lock (d3dContext)
                {
                    var dataBox = d3dContext.MapSubresource(_buffer, 0, mode, SharpDX.Direct3D11.MapFlags.None);
                    
                    int bytes = elementCount * _indexElementSize;

                    int startOffset = startIndex * _indexElementSize;
                    IntPtr inputPtr = data + startOffset;
                    IntPtr outputPtr = dataBox.DataPointer + offsetInBytes;

                    int byteCount = elementCount * _indexElementSize - startOffset;
                    SharpDX.Utilities.CopyMemory(inputPtr, outputPtr, byteCount);

                    d3dContext.UnmapSubresource(_buffer, 0);
                }
            }
            else
            {
                int startBytes = startIndex * _indexElementSize;
                IntPtr dataPtr = data + startBytes;

                var box = new SharpDX.DataBox(dataPtr, elementCount * _indexElementSize, 0);
                var region = new SharpDX.Direct3D11.ResourceRegion
                {
                    Top = 0,
                    Front = 0,
                    Back = 1,
                    Bottom = 1,
                    Left = offsetInBytes,
                    Right = offsetInBytes + (elementCount * _indexElementSize)
                };

                // TODO: We need to deal with threaded contexts here!
                var d3dContext = GraphicsDevice._d3dContext;
                lock (d3dContext)
                    d3dContext.UpdateSubresource(box, _buffer, 0, region);
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
