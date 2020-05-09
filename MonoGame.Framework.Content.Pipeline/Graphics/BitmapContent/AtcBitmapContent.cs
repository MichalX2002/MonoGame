// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Graphics;
using ATI.TextureConverter;
using MonoGame.Framework.Vector;

namespace MonoGame.Framework.Content.Pipeline.Graphics
{
    public abstract class AtcBitmapContent : BitmapContent
    {
        internal byte[] _bitmapData;

        public AtcBitmapContent() : base()
        {
        }

        public AtcBitmapContent(int width, int height)
            : base(width, height)
        {
        }

        public override byte[] GetPixelData()
        {
            return _bitmapData;
        }

        public override void SetPixelData(byte[] data)
        {
            _bitmapData = data;
        }

		protected override bool TryCopyFrom(
            BitmapContent sourceBitmap, Rectangle sourceRegion, Rectangle destinationRegion)
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

            // Destination region copy is not yet supported
            if (destinationRegion != new Rectangle(0, 0, Width, Height))
                return false;

            if (sourceBitmap is PixelBitmapContent<RgbaVector> &&
                sourceRegion.Width == destinationRegion.Width &&
                sourceRegion.Height == destinationRegion.Height)
            {
                ATICompressor.CompressionFormat targetFormat;
                switch (format)
                {
                    case SurfaceFormat.RgbaAtcExplicitAlpha:
                        targetFormat = ATICompressor.CompressionFormat.AtcRgbaExplicitAlpha;
                        break;

                    case SurfaceFormat.RgbaAtcInterpolatedAlpha:
                        targetFormat = ATICompressor.CompressionFormat.AtcRgbaInterpolatedAlpha;
                        break;

                    default:
                        return false;
                }

                var sourceData = sourceBitmap.GetPixelData();
                var compressedData = ATICompressor.Compress(sourceData, Width, Height, targetFormat);
                SetPixelData(compressedData);
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
            if (format == destinationFormat &&
                sourceRegion == new Rectangle(0, 0, Width, Height) &&
                sourceRegion == destinationRegion)
            {
                destinationBitmap.SetPixelData(GetPixelData());
                return true;
            }

            // No other support for copying from a ATC texture yet
            return false;
        }
    }
}
