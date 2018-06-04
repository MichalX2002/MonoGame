// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Utilities;
using MonoGame.Imaging;
using MonoGame.Utilities.Png;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.Direct3D11.Resource;

#if WINDOWS_UAP
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System.Threading.Tasks;
#endif

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class Texture2D : Texture
    {
        public int MaxSize => GraphicsDevice.MaxTexture2DSize;

        protected bool Shared { get; private set; }
        protected bool Mipmap { get; private set; }
        protected SampleDescription SampleDescription { get; private set; }

        private SharpDX.Direct3D11.Texture2D _cachedStagingTexture;

        private void PlatformConstruct(int width, int height, bool mipmap, SurfaceFormat format, SurfaceType type, bool shared)
        {
            Shared = shared;
            Mipmap = mipmap;
        }

        private void PlatformSetData<T>(int level, T[] data, int startIndex, int elementCount)
        {
            var elementSizeInByte = ReflectionHelpers.SizeOf<T>.Get();
            var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            // Use try..finally to make sure dataHandle is freed in case of an error
            try
            {
                PlatformSetData(level, dataHandle.AddrOfPinnedObject(), startIndex, elementCount, elementSizeInByte);
            }
            finally
            {
                dataHandle.Free();
            }
        }

        private void PlatformSetData(int level, IntPtr data, int startIndex, int elementCount, int elementSize)
        {
            GetSizeForLevel(Width, Height, level, out int w, out int h);

            // For DXT compressed formats the width and height must be
            // a multiple of 4 for the complete mip level to be set.
            if (_format.IsCompressedFormat())
            {
                w = (w + 3) & ~3;
                h = (h + 3) & ~3;
            }

            int startBytes = startIndex * elementSize;
            IntPtr dataPtr = (IntPtr)(data.ToInt64() + startBytes);
            var region = new ResourceRegion
            {
                Top = 0,
                Front = 0,
                Back = 1,
                Bottom = h,
                Left = 0,
                Right = w
            };

            // TODO: We need to deal with threaded contexts here!
            var subresourceIndex = CalculateSubresourceIndex(0, level);
            var d3dContext = GraphicsDevice._d3dContext;
            lock (d3dContext)
                d3dContext.UpdateSubresource(GetTexture(), subresourceIndex, region, dataPtr, GetPitch(w), 0);
        }

        private void PlatformSetData<T>(int level, int arraySlice, Rectangle rect, T[] data, int startIndex, int elementCount) where T : struct
        {
            var elementSizeInByte = ReflectionHelpers.SizeOf<T>.Get();
            var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            // Use try..finally to make sure dataHandle is freed in case of an error
            try
            {
                var startBytes = startIndex * elementSizeInByte;
                var dataPtr = (IntPtr)(dataHandle.AddrOfPinnedObject().ToInt64() + startBytes);
                var region = new ResourceRegion
                {
                    Top = rect.Top,
                    Front = 0,
                    Back = 1,
                    Bottom = rect.Bottom,
                    Left = rect.Left,
                    Right = rect.Right
                };


                // TODO: We need to deal with threaded contexts here!
                var subresourceIndex = CalculateSubresourceIndex(arraySlice, level);
                var d3dContext = GraphicsDevice._d3dContext;
                lock (d3dContext)
                    d3dContext.UpdateSubresource(GetTexture(), subresourceIndex, region, dataPtr, GetPitch(rect.Width), 0);
            }
            finally
            {
                dataHandle.Free();
            }
        }

        private void PlatformGetData<T>(int level, int arraySlice, Rectangle rect, T[] data, int startIndex, int elementCount) where T : struct
        {
            // Create a temp staging resource for copying the data.
            // 
            // TODO: We should probably be pooling these staging resources
            // and not creating a new one each time.
            //
            var min = _format.IsCompressedFormat() ? 4 : 1;
            var levelWidth = Math.Max(Width >> level, min);
            var levelHeight = Math.Max(Height >> level, min);

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
                var subresourceIndex = CalculateSubresourceIndex(arraySlice, level);

                // Copy the data from the GPU to the staging texture.
                var elementsInRow = rect.Width;
                var rows = rect.Height;
                var region = new ResourceRegion(rect.Left, rect.Top, 0, rect.Right, rect.Bottom, 1);
                d3dContext.CopySubresourceRegion(GetTexture(), subresourceIndex, region, _cachedStagingTexture, 0);

                // Copy the data to the array.
                DataStream stream = null;
                try
                {
                    var databox = d3dContext.MapSubresource(_cachedStagingTexture, 0, MapMode.Read, MapFlags.None, out stream);

                    var elementSize = _format.GetSize();
                    if (_format.IsCompressedFormat())
                    {
                        // for 4x4 block compression formats an element is one block, so elementsInRow
                        // and number of rows are 1/4 of number of pixels in width and height of the rectangle
                        elementsInRow /= 4;
                        rows /= 4;
                    }
                    var rowSize = elementSize * elementsInRow;
                    if (rowSize == databox.RowPitch)
                        stream.ReadRange(data, startIndex, elementCount);
                    else
                    {
                        // Some drivers may add pitch to rows.
                        // We need to copy each row separatly and skip trailing zeros.
                        stream.Seek(0, SeekOrigin.Begin);

                        var elementSizeInByte = ReflectionHelpers.SizeOf<T>.Get();
                        for (var row = 0; row < rows; row++)
                        {
                            int i;
                            for (i = row * rowSize / elementSizeInByte; i < (row + 1) * rowSize / elementSizeInByte; i++)
                                data[i + startIndex] = stream.Read<T>();

                            if (i >= elementCount)
                                break;

                            stream.Seek(databox.RowPitch - rowSize, SeekOrigin.Current);
                        }
                    }
                }
                finally
                {
                    SharpDX.Utilities.Dispose(ref stream);

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
            return arraySlice * _levelCount + level;
        }

        private static Texture2D PlatformFromStream(GraphicsDevice graphicsDevice, Stream stream)
        {
            using (var reader = new ImageReader(stream, true))
            {
                int length = reader.Read(out IntPtr data, out int width, out int height,
                    out int channels, ImagePixelFormat.RgbWithAlpha);

                // XNA blacks out any pixels with an alpha of zero.
                unsafe
                {
                    if (channels == 4)
                    {
                        byte* b = (byte*)data;
                        for (var i = 0; i < length; i += 4)
                        {
                            if (b[i + 3] == 0)
                            {
                                b[i + 0] = 0;
                                b[i + 1] = 0;
                                b[i + 2] = 0;
                            }
                        }
                    }
                }
                
                Texture2D texture = new Texture2D(graphicsDevice, width, height);
                texture.SetData(data, 0, channels, length / channels);
                return texture;
            }
        }

        private void PlatformSaveAsJpeg(Stream stream, int width, int height)
        {
#if WINDOWS_UAP
            SaveAsImage(Windows.Graphics.Imaging.BitmapEncoder.JpegEncoderId, stream, width, height);
#else
            throw new NotImplementedException();
#endif
        }

        //Converts Pixel Data from BGRA to RGBA
        private static void ConvertToRGBA(int pixelHeight, int pixelWidth, byte[] pixels)
        {
            int offset = 0;

            for (int row = 0; row < (uint)pixelHeight; row++)
            {
                for (int col = 0; col < (uint)pixelWidth; col++)
                {
                    offset = (row * pixelWidth * 4) + (col * 4);

                    byte B = pixels[offset];
                    byte R = pixels[offset + 2];

                    pixels[offset] = R;
                    pixels[offset + 2] = B;
                }
            }
        }

        private void PlatformSaveAsPng(Stream stream, int width, int height)
        {
            var pngWriter = new PngWriter();
            pngWriter.Write(this, stream);
        }

#if WINDOWS_UAP
        private void SaveAsImage(Guid encoderId, Stream stream, int width, int height)
        {
            var pixelData = new byte[Width * Height * GraphicsExtensions.GetSize(Format)];
            GetData(pixelData);

            // TODO: We need to convert from Format to R8G8B8A8!

            // TODO: We should implement async SaveAsPng() for WinRT.
            Task.Run(async () =>
            {
                // Create a temporary memory stream for writing the png.
                var memstream = new InMemoryRandomAccessStream();

                // Write the png.
                var encoder = await Windows.Graphics.Imaging.BitmapEncoder.CreateAsync(encoderId, memstream);
                encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Ignore, (uint)width, (uint)height, 96, 96, pixelData);
                await encoder.FlushAsync();

                // Copy the memory stream into the real output stream.
                memstream.Seek(0);
                memstream.AsStreamForRead().CopyTo(stream);

            }).Wait();
        }
#endif

        static unsafe SharpDX.Direct3D11.Texture2D CreateTex2DFromBitmap(BitmapSource bsource, GraphicsDevice device)
        {
            Texture2DDescription desc;
            desc.Width = bsource.Size.Width;
            desc.Height = bsource.Size.Height;
            desc.ArraySize = 1;
            desc.BindFlags = BindFlags.ShaderResource;
            desc.Usage = ResourceUsage.Default;
            desc.CpuAccessFlags = CpuAccessFlags.None;
            desc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            desc.MipLevels = 1;
            desc.OptionFlags = ResourceOptionFlags.None;
            desc.SampleDescription.Count = 1;
            desc.SampleDescription.Quality = 0;

            using (DataStream s = new DataStream(bsource.Size.Height * bsource.Size.Width * 4, true, true))
            {
                bsource.CopyPixels(bsource.Size.Width * 4, s);

                // XNA blacks out any pixels with an alpha of zero.
                var data = (byte*)s.DataPointer;
                for (var i = 0; i < s.Length; i += 4)
                {
                    if (data[i + 3] == 0)
                    {
                        data[i + 0] = 0;
                        data[i + 1] = 0;
                        data[i + 2] = 0;
                    }
                }

                DataRectangle rect = new DataRectangle(s.DataPointer, bsource.Size.Width * 4);

                return new SharpDX.Direct3D11.Texture2D(device._d3dDevice, desc, rect);
            }
        }

        static ImagingFactory imgfactory;

        private static BitmapSource LoadBitmap(Stream stream, out SharpDX.WIC.BitmapDecoder decoder)
        {
            if (imgfactory == null)
            {
                imgfactory = new ImagingFactory();
            }

            decoder = new SharpDX.WIC.BitmapDecoder(
                imgfactory,
                stream,
                DecodeOptions.CacheOnDemand
                );

            var fconv = new FormatConverter(imgfactory);

            using (var frame = decoder.GetFrame(0))
            {
                fconv.Initialize(
                    frame,
                    PixelFormat.Format32bppRGBA,
                    BitmapDitherType.None,
                    null,
                    0.0,
                    BitmapPaletteType.Custom);
            }
            return fconv;
        }

        protected internal virtual Texture2DDescription GetTexture2DDescription()
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

        protected internal virtual SampleDescription CreateSampleDescription()
        {
            return new SampleDescription(1, 0);
        }

        internal SampleDescription GetTextureSampleDescription()
        {
            return SampleDescription;
        }

        private void PlatformReload(Stream textureStream)
        {
        }
    }
}

