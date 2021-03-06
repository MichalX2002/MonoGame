﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.


namespace MonoGame.Framework.Graphics
{
    internal partial class ConstantBuffer : GraphicsResource
    {
        private SharpDX.Direct3D11.Buffer _cbuffer;

        private void PlatformInitialize()
        {
            // Allocate the hardware constant buffer.
            var desc = new SharpDX.Direct3D11.BufferDescription
            {
                SizeInBytes = _buffer.Length,
                Usage = SharpDX.Direct3D11.ResourceUsage.Default,
                BindFlags = SharpDX.Direct3D11.BindFlags.ConstantBuffer,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None
            };
            lock (GraphicsDevice._d3dContext)
                _cbuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice._d3dDevice, desc);
        }

        private void PlatformClear()
        {
            SharpDX.Utilities.Dispose(ref _cbuffer);
            IsDirty = true;
        }

        internal void PlatformApply(GraphicsDevice device, ShaderStage stage, int slot)
        {
            if (_cbuffer == null)
                PlatformInitialize();

            // NOTE: We make the assumption here that the caller has
            // locked the d3dContext for us to use.
            var d3dContext = GraphicsDevice._d3dContext;

            // Update the hardware buffer.
            if (IsDirty)
            {
                d3dContext.UpdateSubresource(_buffer, _cbuffer);
                IsDirty = false;
            }
            
            // Set the buffer to the right stage.
            if (stage == ShaderStage.Vertex)
                d3dContext.VertexShader.SetConstantBuffer(slot, _cbuffer);
            else
                d3dContext.PixelShader.SetConstantBuffer(slot, _cbuffer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                SharpDX.Utilities.Dispose(ref _cbuffer);
            base.Dispose(disposing);
        }
    }
}
