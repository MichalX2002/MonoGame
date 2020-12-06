using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging;
using MonoGame.Imaging.Pixels;

namespace MonoGame.Framework
{
    public static class ImageExtensions
    {
        public static unsafe Bitmap ToBitmap(
            this IReadOnlyPixelBuffer pixels,
            Rectangle? sourceRectangle = null)
        {
            var srcRect = sourceRectangle ?? pixels.GetBounds();
            ImagingArgumentGuard.AssertRectangleInSource(pixels, srcRect, nameof(sourceRectangle));

            var srcType = pixels.PixelType;
            var dstType = VectorType.Get<Bgra32>();
            var convertPixels = Imaging.Image.GetConvertPixelsDelegate(srcType, dstType);
            int width = srcRect.Width;
            int height = srcRect.Height;

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var bmpRect = new System.Drawing.Rectangle(0, 0, width, height);
            var bmpData = bitmap.LockBits(bmpRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                for (int y = 0; y < bmpData.Height; y++)
                {
                    int srcX = srcRect.X;
                    int srcY = srcRect.Y + y;
                    int dstX = 0;
                    int dstY = y;
                    var dstPtr = (byte*)bmpData.Scan0 + dstY * bmpData.Stride;

                    var srcRow = pixels.GetPixelByteRowSpan(srcY)[srcX..];
                    var dstRow = new Span<Bgra32>(dstPtr, bmpData.Width)[dstX..];

                    convertPixels.Invoke(srcRow, MemoryMarshal.AsBytes(dstRow));
                }
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }
            return bitmap;
        }
    }
}
