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

        public void Save(Stream output, ImageSaveFormat format, SaveConfiguration config)
        {
            lock (SyncRoot)
            {
                unsafe
                {
                    void Get(out int w, out int h, out int p, out byte* d)
                    {
                        ImageInfo info = GetImageInfo();
                        CheckImageInfo(info);

                        IntPtr data = GetDataPointer();
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
                        return new InvalidOperationException($"Could not save image: {LastError}");
                    }

                    byte* ptr;
                    int width, height, bpp;
                    WriteContext writeCtx = Imaging.GetWriteContext(WriteCallback, _manager, output, config);
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
            }
        }

        private unsafe int WriteCallback(
            Stream stream, byte* data, int size, in WriteContext c)
        {
            if (data == null || size <= 0)
                return 0;

            int bufferSize = _manager.GetOptimalByteArraySize(size);
            byte[] buffer = _manager.AllocateByteArray(bufferSize);

            int read = 0;

            using (var input = new UnmanagedMemoryStream(data, size))
            {
                while ((read = input.Read(buffer, 0, bufferSize)) > 0)
                {
                    stream.Write(buffer, 0, read);
                    c.OnWrite?.Invoke(read);
                }
            }

            _manager.ReleaseByteArray(buffer);

            return size;
        }

        private struct StreamHolder
        {
            public readonly Stream Stream;

            public StreamHolder(Stream stream)
            {
                Stream = stream;
            }
        }
    }
}
