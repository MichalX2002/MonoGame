// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace MonoGame.Framework.Graphics
{
    public partial class TextureCube
    {
        private bool _renderTarget;
        private bool _mipMap;

        private void PlatformConstruct(GraphicsDevice graphicsDevice, int size, bool mipMap, SurfaceFormat format, bool renderTarget)
        {
            _renderTarget = renderTarget;
            _mipMap = mipMap;

            // Create texture
            GetTexture();
        }

        internal override void CreateTexture()
        {
            var description = new Texture2DDescription
            {
                Width = Size,
                Height = Size,
                MipLevels = LevelCount,
                ArraySize = 6, // A texture cube is a 2D texture array with 6 textures.
                Format = SharpDXHelper.ToFormat(Format),
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.TextureCube
            };

            if (_renderTarget)
            {
                description.BindFlags |= BindFlags.RenderTarget;
                if (_mipMap)
                    description.OptionFlags |= ResourceOptionFlags.GenerateMipMaps;
            }

            _texture = new SharpDX.Direct3D11.Texture2D(GraphicsDevice._d3dDevice, description);
        }

        private unsafe void PlatformGetData<T>(
            CubeMapFace cubeMapFace, int level, Rectangle rect, Span<T> destination)
            where T : unmanaged
        {
            // Create a temp staging resource for copying the data.
            // 
            // TODO: Like in Texture2D, we should probably be pooling these staging resources
            // and not creating a new one each time.

            var min = Format.IsCompressedFormat() ? 4 : 1;
            var levelSize = Math.Max(Size >> level, min);

            var desc = new Texture2DDescription
            {
                Width = levelSize,
                Height = levelSize,
                MipLevels = 1,
                ArraySize = 1,
                Format = SharpDXHelper.ToFormat(Format),
                SampleDescription = new SampleDescription(1, 0),
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                Usage = ResourceUsage.Staging,
                OptionFlags = ResourceOptionFlags.None,
            };

            var subresourceIndex = CalculateSubresourceIndex(cubeMapFace, level);
            var columns = rect.Width;
            var rows = rect.Height;
            var region = new ResourceRegion(rect.Left, rect.Top, 0, rect.Right, rect.Bottom, 1);

            var d3dContext = GraphicsDevice._d3dContext;
            using (var stagingTex = new SharpDX.Direct3D11.Texture2D(GraphicsDevice._d3dDevice, desc))
            {
                lock (d3dContext)
                {
                    // Copy the data from the GPU to the staging texture.
                    d3dContext.CopySubresourceRegion(GetTexture(), subresourceIndex, region, stagingTex, 0);

                    var elementSize = Format.GetSize();
                    if (Format.IsCompressedFormat())
                    {
                        // for 4x4 block compression formats an element is one block, so elementsInRow
                        // and number of rows are 1/4 of number of pixels in width and height of the rectangle
                        columns /= 4;
                        rows /= 4;
                    }

                    var box = d3dContext.MapSubresource(stagingTex, 0, MapMode.Read, MapFlags.None);
                    GraphicsDevice.CopyResourceTo(Format, box, columns, rows, destination);
                }
            }
        }

        private unsafe void PlatformSetData<T>(
            CubeMapFace face, int level, Rectangle rect, ReadOnlySpan<T> data)
            where T : unmanaged
        {
            var subresourceIndex = CalculateSubresourceIndex(face, level);
            int pitch = GetPitch(rect.Width);
            var region = new ResourceRegion
            {
                Top = rect.Top,
                Front = 0,
                Back = 1,
                Bottom = rect.Bottom,
                Left = rect.Left,
                Right = rect.Right
            };

            var d3dContext = GraphicsDevice._d3dContext;
            lock (d3dContext)
            {
                var texture = GetTexture();
                ref var mutableData = ref Unsafe.AsRef(data.GetPinnableReference());
                d3dContext.UpdateSubresource(ref mutableData, texture, subresourceIndex, pitch, 0, region);

            }
        }

        private int CalculateSubresourceIndex(CubeMapFace face, int level)
        {
            return (int)face * LevelCount + level;
        }
    }
}