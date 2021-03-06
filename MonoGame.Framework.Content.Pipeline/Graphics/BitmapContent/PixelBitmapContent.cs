﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Vectors;

namespace MonoGame.Framework.Content.Pipeline.Graphics
{
    public class PixelBitmapContent<TPixel> : BitmapContent
        where TPixel : unmanaged, IPixel<TPixel>
    {
        private byte[] _pixelData;
        private SurfaceFormat _format;

        public PixelBitmapContent(int width, int height)
        {
            if (!TryGetFormat(out _format))
                throw new InvalidOperationException($"Color format \"{typeof(TPixel)}\" is not supported");

            Height = height;
            Width = width;
            _pixelData = new byte[Width * Height * _format.GetSize()];
        }

        public override byte[] GetPixelData()
        {
            return _pixelData;
        }

        public Span<TPixel> GetPixelSpan()
        {
            return MemoryMarshal.Cast<byte, TPixel>(_pixelData);
        }

        public void SetPixelData(ReadOnlySpan<TPixel> data)
        {
            // TODO: use Image.ConvertPixels

            for (int y = 0; y < Height; y++)
            {
                var srcRow = data.Slice(y * Width, Width);
                var dstRow = GetRowSpan(y);
                for (int x = 0; x < Width; x++)
                {
                    dstRow[x].FromScaledVector(srcRow[x].ToScaledVector4());
                }
            }
        }

        public override void SetPixelData(byte[] data)
        {
            var pixels = MemoryMarshal.Cast<byte, TPixel>(data);
            SetPixelData(pixels);
        }

        public Span<TPixel> GetRowSpan(int y)
        {
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException(nameof(y));
            return MemoryMarshal.Cast<byte, TPixel>(_pixelData).Slice(y * Width, Width);
        }

        /// <summary>
        /// Gets the corresponding GPU texture format for the specified bitmap type.
        /// </summary>
        /// <param name="format">Format being retrieved.</param>
        /// <returns>The GPU texture format of the bitmap type.</returns>
        public override bool TryGetFormat(out SurfaceFormat format)
        {
            if (typeof(TPixel) == typeof(Color) ||
                typeof(TPixel) == typeof(Byte4))
                format = SurfaceFormat.Rgba32;
            else if (typeof(TPixel) == typeof(Bgra4444))
                format = SurfaceFormat.Bgra4444;
            else if (typeof(TPixel) == typeof(Bgra5551))
                format = SurfaceFormat.Bgra5551;
            else if (typeof(TPixel) == typeof(Bgr565))
                format = SurfaceFormat.Bgr565;
            else if (typeof(TPixel) == typeof(Alpha8))
                format = SurfaceFormat.Alpha8;
            else if (typeof(TPixel) == typeof(Rgba64))
                format = SurfaceFormat.Rgba64;
            else if (typeof(TPixel) == typeof(Rgba1010102))
                format = SurfaceFormat.Rgba1010102;
            else if (typeof(TPixel) == typeof(Rg32))
                format = SurfaceFormat.Rg32;
            else if (typeof(TPixel) == typeof(NormalizedByte2))
                format = SurfaceFormat.NormalizedByte2;
            else if (typeof(TPixel) == typeof(NormalizedByte4))
                format = SurfaceFormat.NormalizedByte4;
            else if (typeof(TPixel) == typeof(HalfSingle))
                format = SurfaceFormat.HalfSingle;
            else if (typeof(TPixel) == typeof(HalfVector2))
                format = SurfaceFormat.HalfVector2;
            else if (typeof(TPixel) == typeof(HalfVector4))
                format = SurfaceFormat.HalfVector4;
            else if (typeof(TPixel) == typeof(RgbaVector))
                format = SurfaceFormat.Vector4;
            else if (typeof(TPixel) == typeof(RgVector))
                format = SurfaceFormat.Vector2;
            else if (
                typeof(TPixel) == typeof(RedF) ||
                typeof(TPixel) == typeof(GrayF) ||
                typeof(TPixel) == typeof(AlphaF))
            {
                format = SurfaceFormat.Single;
            }
            else
            {
                format = default;
                return false;
            }
            return true;
        }

        public void ReplaceColor(TPixel originalColor, TPixel newColor)
        {
            var pixels = GetPixelSpan();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].Equals(originalColor))
                    pixels[i] = newColor;
            }
        }

        protected override bool TryCopyFrom(
            BitmapContent srcBitmap, Rectangle srcRegion, Rectangle dstRegion)
        {
            if (srcBitmap == null)
                throw new ArgumentNullException(nameof(srcBitmap));

            if (!srcBitmap.TryGetFormat(out SurfaceFormat sourceFormat))
                return false;

            // A shortcut for copying the entire bitmap to another bitmap of the same type and format
            if (_format == sourceFormat &&
                srcRegion == new Rectangle(0, 0, Width, Height) &&
                srcRegion == dstRegion)
            {
                SetPixelData(srcBitmap.GetPixelData());
                return true;
            }

            // If the source is RgbaVector and doesn't require resizing, just copy
            if (srcBitmap is PixelBitmapContent<RgbaVector> src &&
                srcRegion.Width == dstRegion.Width &&
                srcRegion.Height == dstRegion.Height)
            {
                // TODO: use Image.ConvertPixels

                for (int y = 0; y < srcRegion.Height; y++)
                {
                    var srcRow = src.GetRowSpan(srcRegion.Top + y);
                    var dstRow = GetRowSpan(dstRegion.Top + y);

                    for (int x = 0; x < srcRegion.Width; x++)
                    {
                        dstRow[dstRegion.X + x].FromScaledVector(srcRow[srcRegion.Left + x].ToScaledVector4());
                    }
                }
                return true;
            }

            try
            {
                Copy(srcBitmap, srcRegion, this, dstRegion);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        protected override bool TryCopyTo(
            BitmapContent dstBitmap, Rectangle srcRegion, Rectangle dstRegion)
        {
            if (dstBitmap == null)
                throw new ArgumentNullException(nameof(dstBitmap));

            if (!dstBitmap.TryGetFormat(out SurfaceFormat destinationFormat))
                return false;

            // A shortcut for copying the entire bitmap to another bitmap of the same type and format
            if (_format == destinationFormat &&
                srcRegion == new Rectangle(0, 0, Width, Height) &&
                srcRegion == dstRegion)
            {
                dstBitmap.SetPixelData(GetPixelData());
                return true;
            }

            if (dstBitmap is PixelBitmapContent<RgbaVector> dst &&
                srcRegion.Width == dstRegion.Width &&
                srcRegion.Height == dstRegion.Height)
            {
                // TODO: use Image.ConvertPixels

                // Convert to a RgbaVector format
                for (int y = 0; y < srcRegion.Height; y++)
                {
                    var srcRow = GetRowSpan(srcRegion.Top + y);
                    var dstRow = dst.GetRowSpan(dstRegion.Top + y);
                    for (int x = 0; x < srcRegion.Width; x++)
                    {
                        dstRow[dstRegion.Left + x].FromScaledVector(
                            srcRow[srcRegion.Left + x].ToScaledVector4());
                    }
                }
                return true;
            }

            try
            {
                Copy(this, srcRegion, dstBitmap, dstRegion);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}