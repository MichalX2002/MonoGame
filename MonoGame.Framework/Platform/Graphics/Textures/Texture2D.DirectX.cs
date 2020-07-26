// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.Direct3D11.Resource;

namespace MonoGame.Framework.Graphics
{
    public partial class Texture2D : Texture
    {
        protected bool Shared { get; private set; }
        protected bool Mipmap { get; private set; }

        [CLSCompliant(false)]
        private SampleDescription SampleDescription { get; set; }

        private SharpDX.Direct3D11.Texture2D _cachedStagingTexture;

        private void PlatformConstruct(
            int width, int height, bool mipmap, SurfaceFormat format, SurfaceType type, bool shared)
        {
            Shared = shared;
            Mipmap = mipmap;
            SampleDescription = new SampleDescription(1, 0);
        }

        private unsafe void PlatformSetData<T>(
            int level, int arraySlice, Rectangle? rect, ReadOnlySpan<T> data)
            where T : unmanaged
        {
            int pitch;
            ResourceRegion region;

            if (rect.HasValue)
            {
                var r = rect.Value;
                pitch = GetPitch(r.Width);
                region = new ResourceRegion
                {
                    Top = r.Top,
                    Front = 0,
                    Back = 1,
                    Bottom = r.Bottom,
                    Left = r.Left,
                    Right = r.Right
                };
            }
            else
            {
                GetSizeForLevel(Width, Height, level, out int w, out int h);

                // For DXT compressed formats the width and height must be
                // a multiple of 4 for the complete mip level to be set.
                if (Format.IsCompressedFormat())
                {
                    w = (w + 3) & ~3;
                    h = (h + 3) & ~3;
                }

                pitch = GetPitch(w);
                region = new ResourceRegion
                {
                    Top = 0,
                    Front = 0,
                    Back = 1,
                    Bottom = h,
                    Left = 0,
                    Right = w
                };
            }

            var subresourceIndex = CalculateSubresourceIndex(arraySlice, level);

            // TODO: We need to deal with threaded contexts here!
            var d3dContext = GraphicsDevice._d3dContext;
            lock (d3dContext)
            {
                var texture = GetTexture();
                ref var mutableData = ref MemoryMarshal.GetReference(data);
                d3dContext.UpdateSubresource(ref mutableData, texture, subresourceIndex, pitch, 0, region);
            }
        }

        private void PlatformGetData(
            int level, int arraySlice, Rectangle rect, Span<byte> destination)
        {
            // Create a temp staging resource for copying the data.
            // 
            // TODO: We should probably be pooling these staging resources
            // and not creating a new one each time.
            //
            var min = Format.IsCompressedFormat() ? 4 : 1;
            var levelWidth = Math.Max(Width >> level, min);
            var levelHeight = Math.Max(Height >> level, min);

            if (_cachedStagingTexture == null)
            {
                var desc = new Texture2DDescription();
                desc.Width = levelWidth;
                desc.Height = levelHeight;
                desc.MipLevels = 1;
                desc.ArraySize = 1;
                desc.Format = SharpDXHelper.ToFormat(Format);
                desc.BindFlags = BindFlags.None;
                desc.CpuAccessFlags = CpuAccessFlags.Read;
                desc.SampleDescription = SampleDescription;
                desc.Usage = ResourceUsage.Staging;
                desc.OptionFlags = ResourceOptionFlags.None;

                _cachedStagingTexture = new SharpDX.Direct3D11.Texture2D(GraphicsDevice._d3dDevice, desc);
            }

            var d3dContext = GraphicsDevice._d3dContext;
            lock (d3dContext)
            {
                var subresourceIndex = CalculateSubresourceIndex(arraySlice, level);

                // Copy the data from the GPU to the staging texture.
                var columns = rect.Width;
                var rows = rect.Height;
                var region = new ResourceRegion(rect.Left, rect.Top, 0, rect.Right, rect.Bottom, 1);
                d3dContext.CopySubresourceRegion(GetTexture(), subresourceIndex, region, _cachedStagingTexture, 0);

                try
                {
                    if (Format.IsCompressedFormat())
                    {
                        // for 4x4 block compression formats an element is one block, so elementsInRow
                        // and number of rows are 1/4 of number of pixels in width and height of the rectangle
                        columns /= 4;
                        rows /= 4;
                    }

                    var box = d3dContext.MapSubresource(_cachedStagingTexture, 0, MapMode.Read, MapFlags.None);
                    GraphicsDevice.CopyResourceTo(Format, box, columns, rows, destination);
                }
                finally
                {
                    d3dContext.UnmapSubresource(_cachedStagingTexture, 0);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SharpDX.Utilities.Dispose(ref _cachedStagingTexture);
            }

            base.Dispose(disposing);
        }

        private int CalculateSubresourceIndex(int arraySlice, int level)
        {
            return arraySlice * LevelCount + level;
        }

        [CLSCompliant(false)]
        protected internal virtual Texture2DDescription GetTexture2DDescription()
        {
            var desc = new Texture2DDescription
            {
                Width = Width,
                Height = Height,
                MipLevels = LevelCount,
                ArraySize = ArraySize,
                Format = SharpDXHelper.ToFormat(Format),
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                SampleDescription = SampleDescription,
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.None
            };

            if (Shared)
                desc.OptionFlags |= ResourceOptionFlags.Shared;

            return desc;
        }

        internal override Resource CreateTexture()
        {
            // TODO: Move this to SetData() if we want to make Immutable textures!
            var desc = GetTexture2DDescription();
            return new SharpDX.Direct3D11.Texture2D(GraphicsDevice._d3dDevice, desc);
        }

        private void PlatformReload(Stream stream)
        {
            // TODO
        }
    }
}