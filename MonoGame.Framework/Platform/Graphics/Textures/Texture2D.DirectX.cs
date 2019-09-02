// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
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
        internal SampleDescription SampleDescription { get; private set; }

        private SharpDX.Direct3D11.Texture2D _cachedStagingTexture;

        private void PlatformConstruct(
            int width, int height, bool mipmap, SurfaceFormat format, SurfaceType type, bool shared)
        {
            Shared = shared;
            Mipmap = mipmap;
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
                if (_format.IsCompressedFormat())
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

            // TODO: We need to deal with threaded contexts here!
            var subresourceIndex = CalculateSubresourceIndex(arraySlice, level);
            var d3dContext = GraphicsDevice._d3dContext;
            lock (d3dContext)
            {
                fixed (T* ptr = &MemoryMarshal.GetReference(data))
                {
                    d3dContext.UpdateSubresource(GetTexture(), subresourceIndex, region, (IntPtr)ptr, pitch, 0);
                }
            }
        }

        private unsafe void PlatformGetData<T>(
            int level, int arraySlice, Rectangle rect, Span<T> destination)
            where T : unmanaged
        {
            // Create a tmp staging resource for copying the data.
            
            // TODO: We should probably be pooling these staging resources
            // and not creating a new one each time.
            
            int min = _format.IsCompressedFormat() ? 4 : 1;
            int levelWidth = Math.Max(Width >> level, min);
            int levelHeight = Math.Max(Height >> level, min);

            if (_cachedStagingTexture == null)
            {
                var desc = new Texture2DDescription
                {
                    Width = levelWidth,
                    Height = levelHeight,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = SharpDXHelper.ToFormat(_format),
                    BindFlags = BindFlags.None,
                    CpuAccessFlags = CpuAccessFlags.Read,
                    SampleDescription = CreateSampleDescription(),
                    Usage = ResourceUsage.Staging,
                    OptionFlags = ResourceOptionFlags.None
                };

                // Save sampling description.
                SampleDescription = desc.SampleDescription;

                _cachedStagingTexture = new SharpDX.Direct3D11.Texture2D(GraphicsDevice._d3dDevice, desc);
            }

            var d3dContext = GraphicsDevice._d3dContext;
            lock (d3dContext)
            {
                int subresourceIndex = CalculateSubresourceIndex(arraySlice, level);

                // Copy the data from the GPU to the staging texture.
                int columns = rect.Width;
                int rows = rect.Height;
                var region = new ResourceRegion(rect.Left, rect.Top, 0, rect.Right, rect.Bottom, 1);
                d3dContext.CopySubresourceRegion(GetTexture(), subresourceIndex, region, _cachedStagingTexture, 0);

                // Copy the data to the array.
                try
                {
                    if (_format.IsCompressedFormat())
                    {
                        // for 4x4 block compression formats an element is one block, so elementsInRow
                        // and number of rows are 1/4 of number of pixels in width and height of the rectangle
                        columns /= 4;
                        rows /= 4;
                    }

                    var box = d3dContext.MapSubresource(_cachedStagingTexture, 0, MapMode.Read, MapFlags.None);
                    GraphicsDevice.CopyResourceTo(_format, box, columns, rows, destination);
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
                SharpDX.Utilities.Dispose(ref _cachedStagingTexture);
            base.Dispose(disposing);
        }

        private int CalculateSubresourceIndex(int arraySlice, int level)
        {
            return arraySlice * _levelCount + level;
        }

        internal virtual Texture2DDescription GetTexture2DDescription()
        {
            var desc = new Texture2DDescription
            {
                Width = Width,
                Height = Height,
                MipLevels = _levelCount,
                ArraySize = ArraySize,
                Format = SharpDXHelper.ToFormat(_format),
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                SampleDescription = CreateSampleDescription(),
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

            // Save sampling description.
            SampleDescription = desc.SampleDescription;

            return new SharpDX.Direct3D11.Texture2D(GraphicsDevice._d3dDevice, desc);
        }

        internal virtual SampleDescription CreateSampleDescription()
        {
            return new SampleDescription(1, 0);
        }
    }
}

