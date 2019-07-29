using System;
using System.IO;
using MonoGame.Utilities.Memory;
using StbSharp;

namespace MonoGame.Imaging
{
    public enum ImageWriterFormat
    {
        Bmp,
        Tga,
        Png,
        Jpg,
    }

    public unsafe class ImageWriter
    {
        private int WriteCallback(Stream stream, byte[] buffer, Span<byte> data)
        {
            if (data.IsEmpty)
                return 0;

            int toWrite = data.Length;
            while (toWrite > 0)
            {
                int sliceLength = Math.Min(toWrite, buffer.Length);
                int written = data.Length - toWrite;

                data.Slice(written, sliceLength).CopyTo(buffer);
                stream.Write(buffer, 0, sliceLength);

                toWrite -= sliceLength;
            }
            return data.Length;
        }

        public void Write(void* data, int x, int y, int comp, ImageWriterFormat format, Stream stream)
        {
            byte[] buffer = RecyclableMemoryManager.Instance.GetBlock();
            try
            {
                switch (format)
                {
                    case ImageWriterFormat.Bmp:
                        StbImageWrite.stbi_write_bmp_to_func(WriteCallback, stream, buffer, x, y, comp, data);
                        break;

                    case ImageWriterFormat.Tga:
                        StbImageWrite.stbi_write_tga_to_func(WriteCallback, stream, buffer, x, y, comp, data);
                        break;

                    case ImageWriterFormat.Jpg:
                        StbImageWrite.stbi_write_jpg_to_func(WriteCallback, stream, buffer, x, y, comp, data, 90);
                        break;

                    case ImageWriterFormat.Png:
                        StbImageWrite.stbi_write_png_to_func(WriteCallback, stream, buffer, x, y, comp, 
                            PngCompressionLevel.Default, data, x * comp);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(format), format, "Unsupported image writer format.");
                }
            }
            finally
            {
                RecyclableMemoryManager.Instance.ReturnBlock(buffer);
            }
        }
    }
}