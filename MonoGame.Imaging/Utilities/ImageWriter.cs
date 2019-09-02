using System;
using System.IO;
using MonoGame.Utilities.Memory;
using MonoGame.Utilities.PackedVector;
using StbSharp;

namespace MonoGame.Imaging
{
    [Obsolete]
    public static unsafe class ImageWriter
    {
        public static void Write<TPixel>(
            ImagePixelProvider<TPixel> data, ImageFormat format, Stream output)
            where TPixel : unmanaged, IPixel
        {
            var pngLevel = System.IO.Compression.CompressionLevel.Optimal;
            var mem = RecyclableMemoryManager.Default;
            byte[] writeBuffer = mem.GetBlock();
            byte[] scratchBuffer = mem.GetBlock();
            try
            {
                //var context = new StbImageWrite.WriteContext(data, output, writeBuffer, scratchBuffer);
                //
                //if (format == ImageFormat.Bmp)
                //    StbImageWrite.stbi_write_bmp_core(context);
                //else if (format == ImageFormat.Tga)
                //    StbImageWrite.stbi_write_tga_core(context, true);
                //else if (format == ImageFormat.Jpg)
                //    StbImageWrite.stbi_write_jpg_core(context, 90);
                //else if (format == ImageFormat.Png)
                //    StbImageWrite.stbi_write_png_core(context, pngLevel);
                //else if (format == ImageFormat.Hdr)
                //        StbImageWrite.stbi_write_hdr_to_func(
                //            WriteCallback, output, buffer, x, y, comp, (float*)data);
                //else
                    throw new ArgumentOutOfRangeException(
                            nameof(format), format, "Unsupported image writer format.");
            }
            finally
            {
                mem.ReturnBlock(scratchBuffer);
                mem.ReturnBlock(writeBuffer);
            }
        }
    }
}