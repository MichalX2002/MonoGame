using System;
using System.IO;

namespace MonoGame.Imaging
{
    public sealed partial class Image
    {
        private void CheckImageInfo(ImageInfo info)
        {
            if (info == null || info.IsValid() == false)
                throw new InvalidOperationException(
                    $"No image info is present in this {nameof(Image)} instance.");
        }

        public void Save(Stream output)
        {
            Save(output, SaveConfiguration.Default);
        }

        public void Save(Stream output, SaveConfiguration config)
        {
            ImageInfo info = GetImageInfo();
            CheckImageInfo(info);

            var saveFormat = info.SourceFormat.ToSaveFormat();
            if (saveFormat == ImageSaveFormat.Unknown)
                throw new InvalidOperationException(
                    $"Source format \"{info.SourceFormat}\" is not supported.");

            Save(output, saveFormat, config);
        }

        public void Save(Stream output, ImageSaveFormat format)
        {
            Save(output, format, SaveConfiguration.Default);
        }

        public unsafe void Save(Stream output, ImageSaveFormat format, SaveConfiguration config)
        {
            lock (SyncRoot)
            {
                void Get(out int w, out int h, out int p, out byte* d)
                {
                    ImageInfo info = GetImageInfo();
                    CheckImageInfo(info);

                    IntPtr data = Pointer;
                    if (data == IntPtr.Zero)
                        throw new InvalidOperationException(
                            $"No image data is present in this {nameof(Image)} instance.");

                    w = info.Width;
                    h = info.Height;
                    p = (int)info.PixelFormat;
                    d = (byte*)data;
                }

                InvalidOperationException GetException()
                {
                    return new InvalidOperationException($"Could not save image: {Errors}");
                }

                var buffer = _memoryManager.GetBlock();
                var bufferStream = _memoryManager.GetWriteBufferedStream(output, true);
                try
                {
                    byte* ptr;
                    int width, height, bpp;
                    var writeCtx = Imaging.GetWriteContext(WriteCallback, bufferStream, buffer, config);
                    switch (format)
                    {
                        case ImageSaveFormat.Bmp:
                            {
                                Get(out width, out height, out bpp, out ptr);
                                if (Imaging.CallbackWriteBmp(writeCtx, width, height, bpp, ptr) == 0)
                                    throw GetException();
                                break;
                            }

                        case ImageSaveFormat.Tga:
                            {
                                Get(out width, out height, out bpp, out ptr);
                                if (Imaging.CallbackWriteTga(writeCtx, width, height, bpp, ptr) == 0)
                                    throw GetException();
                                break;
                            }

                        case ImageSaveFormat.Jpg:
                            {
                                Get(out width, out height, out bpp, out ptr);
                                if (Imaging.CallbackWriteJpg(writeCtx, config.JpgQuality, width, height, bpp, ptr) == 0)
                                    throw GetException();
                                break;
                            }

                        case ImageSaveFormat.Png:
                            {
                                Get(out width, out height, out bpp, out ptr);
                                if (Imaging.CallbackWritePng(writeCtx, width, height, bpp, ptr, 0) == 0)
                                    throw GetException();
                                break;
                            }

                        default:
                            throw new ArgumentOutOfRangeException(nameof(format), $"Invalid Format: {format}");
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _memoryManager.ReturnBlock(buffer, null);
                    bufferStream.Dispose();
                }
            }
        }

        private unsafe void WriteCallback(Stream stream, byte* data, byte[] buffer, int size)
        {
            if (data == null || size <= 0)
                return;

            if (size > buffer.Length)
            {
                int leftToWrite = size;
                int read = 0;
                while (leftToWrite > 0)
                {
                    read = buffer.Length < leftToWrite ? buffer.Length : leftToWrite;
                    for (int i = 0; i < read; i++)
                        buffer[i] = data[i + size - leftToWrite];

                    stream.Write(buffer, 0, read);
                    leftToWrite -= read;
                }
            }
            else
            {
                for (int i = 0; i < size; i++)
                    buffer[i] = data[i];
                stream.Write(buffer, 0, size);
            }
        }
    }
}
