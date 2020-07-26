// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using SharpDX.Direct3D11;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.Direct3D11.Resource;

namespace MonoGame.Framework.Graphics
{
    public partial class Texture3D : Texture
    {
        private bool renderTarget;
        private bool mipMap;

        private void PlatformConstruct(
            GraphicsDevice graphicsDevice,
            int width,
            int height,
            int depth,
            bool mipMap,
            SurfaceFormat format,
            bool renderTarget)
        {
            this.renderTarget = renderTarget;
            this.mipMap = mipMap;

            if (mipMap)
                LevelCount = CalculateMipLevels(width, height, depth);

            // Create texture
            GetTexture();
        }

        internal override Resource CreateTexture()
        {
            var description = new Texture3DDescription
            {
                Width = Width,
                Height = Height,
                Depth = Depth,
                MipLevels = LevelCount,
                Format = SharpDXHelper.ToFormat(Format),
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.None,
            };

            if (renderTarget)
            {
                description.BindFlags |= BindFlags.RenderTarget;

                if (mipMap)
                {
                    // Note: XNA 4 does not have a method Texture.GenerateMipMaps() 
                    // because generation of mipmaps is not supported on the Xbox 360.
                    // TODO: New method Texture.GenerateMipMaps() required.
                    description.OptionFlags |= ResourceOptionFlags.GenerateMipMaps;
                }
            }

            return new SharpDX.Direct3D11.Texture3D(GraphicsDevice._d3dDevice, description);
        }

        private void PlatformSetData(
            int level, int left, int top, int right, int bottom, int front, int back,
            int width, int height, int depth, ReadOnlySpan<byte> data)
        {
            int rowPitch = GetPitch(width);
            int slicePitch = rowPitch * height; // For 3D texture: Size of 2D image.
            int subresourceIndex = level;
            var region = new ResourceRegion(left, top, front, right, bottom, back);

            var d3dContext = GraphicsDevice._d3dContext;
            lock (d3dContext)
            {
                var texture = GetTexture();
                ref var mutableData = ref MemoryMarshal.GetReference(data);
                d3dContext.UpdateSubresource(ref mutableData, texture, subresourceIndex, rowPitch, slicePitch, region);
            }
        }

        private void PlatformGetData(
            int level, int left, int top, int right, int bottom, int front, int back, Span<byte> destination)
        {
            // Create a temp staging resource for copying the data.
            // 
            // TODO: Like in Texture2D, we should probably be pooling these staging resources
            // and not creating a new one each time.

            var desc = new Texture3DDescription
            {
                Width = Width,
                Height = Height,
                Depth = Depth,
                MipLevels = 1,
                Format = SharpDXHelper.ToFormat(Format),
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                Usage = ResourceUsage.Staging,
                OptionFlags = ResourceOptionFlags.None,
            };

            int columns = right - left;
            int rows = bottom - top;
            var region = new ResourceRegion(left, top, front, right, bottom, back);

            using (var stagingTex = new SharpDX.Direct3D11.Texture3D(GraphicsDevice._d3dDevice, desc))
            {
                var d3dContext = GraphicsDevice._d3dContext;
                lock (d3dContext)
                {
                    // Copy the data from the GPU to the staging texture.
                    d3dContext.CopySubresourceRegion(GetTexture(), level, region, stagingTex, 0);

                    // Copy the data to the array.
                    var box = d3dContext.MapSubresource(stagingTex, 0, MapMode.Read, MapFlags.None);
                    GraphicsDevice.CopyResourceTo(Format, box, columns, rows, destination);
                }
            }
        }
    }
}