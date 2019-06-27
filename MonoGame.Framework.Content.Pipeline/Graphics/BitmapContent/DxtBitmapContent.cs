// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Graphics;
using Nvidia.TextureTools;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.PixelFormats;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    public abstract class DxtBitmapContent : BitmapContent
    {
        private byte[] _bitmapData;
        private int _blockSize;
        private SurfaceFormat _format;

        private int _nvttWriteOffset;

        protected DxtBitmapContent(int blockSize)
        {
            if (!((blockSize == 8) || (blockSize == 16)))
                throw new ArgumentException("Invalid block size");
            _blockSize = blockSize;
            TryGetFormat(out _format);
        }

        protected DxtBitmapContent(int blockSize, int width, int height)
            : this(blockSize)
        {
            Width = width;
            Height = height;
        }

        public override byte[] GetPixelData()
        {
            return _bitmapData;
        }

        public override void SetPixelData(byte[] sourceData)
        {
            _bitmapData = sourceData;
        }

        private void NvttBeginImage(int size, int width, int height, int depth, int face, int miplevel)
        {
            _bitmapData = new byte[size];
            _nvttWriteOffset = 0;
        }

        private bool NvttWriteImage(IntPtr data, int length)
        {
            Marshal.Copy(data, _bitmapData, _nvttWriteOffset, length);
            _nvttWriteOffset += length;
            return true;
        }

        private void NvttEndImage()
        {
        }
        
        //private static void PrepareNVTT(byte[] data)
        //{
        //    for (var x = 0; x < data.Length; x += 4)
        //    {
        //        // NVTT wants BGRA where our source is RGBA so
        //        // we swap the red and blue channels.
        //        data[x] ^= data[x + 2];
        //        data[x + 2] ^= data[x];
        //        data[x] ^= data[x + 2];
        //    }
        //}

        private static void PrepareNVTT_DXT1(byte[] data, out bool hasTransparency)
        {
            hasTransparency = false;

            for (var x = 0; x < data.Length; x += 4)
            {
                // NVTT wants BGRA where our source is RGBA so
                // we swap the red and blue channels.
                //data[x] ^= data[x + 2];
                //data[x + 2] ^= data[x];
                //data[x] ^= data[x + 2];

                // Look for non-opaque pixels.
                var alpha = data[x + 3];
                if (alpha < 255)
                {
                    hasTransparency = true;
                    break;
                }
            }
        }

        protected override bool TryCopyFrom(BitmapContent sourceBitmap, Rectangle sourceRegion, Rectangle destinationRegion)
        {
            if (!sourceBitmap.TryGetFormat(out SurfaceFormat sourceFormat))
                return false;

            TryGetFormat(out SurfaceFormat format);

            // A shortcut for copying the entire bitmap to another bitmap of the same type and format
            if (format == sourceFormat && 
                sourceRegion == new Rectangle(0, 0, Width, Height) &&
                sourceRegion == destinationRegion)
            {
                SetPixelData(sourceBitmap.GetPixelData());
                return true;
            }

            // TODO: Add a XNA unit test to see what it does
            // my guess is that this is invalid for DXT.
            //
            // Destination region copy is not yet supported
            if (destinationRegion != new Rectangle(0, 0, Width, Height))
                return false;

            if (sourceBitmap is PixelBitmapContent<RgbaVector> && 
                sourceRegion.Width == destinationRegion.Width &&
                sourceRegion.Height == destinationRegion.Height)
            {
                // NVTT wants 8bit data in BGRA format.
                var colorBitmap = new PixelBitmapContent<Bgra32>(sourceBitmap.Width, sourceBitmap.Height);
                Copy(sourceBitmap, colorBitmap);
                var sourceData = colorBitmap.GetPixelData();

                AlphaMode alphaMode;
                Format outputFormat;
                bool alphaDither = false;
                switch (format)
                {
                    case SurfaceFormat.Dxt1:
                    case SurfaceFormat.Dxt1SRgb:
                    {
                        PrepareNVTT_DXT1(sourceData, out bool hasTransparency);
                        outputFormat = hasTransparency ? Format.DXT1a : Format.DXT1;
                        alphaMode = hasTransparency ? AlphaMode.Transparency : AlphaMode.None;
                        alphaDither = true;
                        break;
                    }

                    case SurfaceFormat.Dxt3:
                    case SurfaceFormat.Dxt3SRgb:
                    {
                        //PrepareNVTT(sourceData);
                        outputFormat = Format.DXT3;
                        alphaMode = AlphaMode.Transparency;
                        break;
                    }

                    case SurfaceFormat.Dxt5:
                    case SurfaceFormat.Dxt5SRgb:
                    {
                        //PrepareNVTT(sourceData);
                        outputFormat = Format.DXT5;
                        alphaMode = AlphaMode.Transparency;
                        break;
                    }

                    default:
                        throw new InvalidOperationException("Invalid DXT surface format!");
                }

                // Do all the calls to the NVTT wrapper within this handler
                // so we properly clean up if things blow up.
                var dataHandle = GCHandle.Alloc(sourceData, GCHandleType.Pinned);
                try
                {
                    var dataPtr = dataHandle.AddrOfPinnedObject();

                    var inputOptions = new InputOptions();
                    inputOptions.SetTextureLayout(TextureType.Texture2D, colorBitmap.Width, colorBitmap.Height, 1);
                    inputOptions.SetMipmapData(dataPtr, colorBitmap.Width, colorBitmap.Height, 1, 0, 0);
                    inputOptions.SetMipmapGeneration(false);
                    inputOptions.SetGamma(1.0f, 1.0f);
                    inputOptions.SetAlphaMode(alphaMode);

                    var compressionOptions = new CompressionOptions();
                    compressionOptions.SetFormat(outputFormat);
                    compressionOptions.SetQuality(Quality.Normal);

                    // TODO: This isn't working which keeps us from getting the
                    // same alpha dither behavior on DXT1 as XNA.
                    //
                    // See https://github.com/MonoGame/MonoGame/issues/6259
                    //
                    //if (alphaDither)
                    //compressionOptions.SetQuantization(false, false, true);

                    var outputOptions = new OutputOptions();
                    outputOptions.SetOutputHeader(false);
                    outputOptions.SetOutputOptionsOutputHandler(NvttBeginImage, NvttWriteImage, NvttEndImage);

                    var dxtCompressor = new Compressor();
                    dxtCompressor.Compress(inputOptions, compressionOptions, outputOptions);
                }
                finally
                {
                    dataHandle.Free();
                }
                return true;
            }

            try
            {
                Copy(sourceBitmap, sourceRegion, this, destinationRegion);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        protected override bool TryCopyTo(BitmapContent destinationBitmap, Rectangle sourceRegion, Rectangle destinationRegion)
        {
            if (!destinationBitmap.TryGetFormat(out SurfaceFormat destinationFormat))
                return false;

            TryGetFormat(out SurfaceFormat format);

            // A shortcut for copying the entire bitmap to another bitmap of the same type and format
            var fullRegion = new Rectangle(0, 0, Width, Height);
            if (format == destinationFormat &&
                sourceRegion == fullRegion &&
                sourceRegion == destinationRegion)
            {
                destinationBitmap.SetPixelData(GetPixelData());
                return true;
            }

            // No other support for copying from a DXT texture yet
            return false;
        }
    }
}