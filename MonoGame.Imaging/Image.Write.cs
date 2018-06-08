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
            Save(output, Configuration.Default);
        }

        public void Save(Stream output, Configuration config)
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
            Save(output, format, Configuration.Default);
        }

        public void Save(Stream output, ImageSaveFormat format, Configuration config)
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
                    switch (format)
                    {
                        case ImageSaveFormat.Bmp:
                            {
                                Get(out width, out height, out bpp, out ptr);
                                if (Imaging.CallbackWriteBmp(WriteCallback, _manager, output, width, height, bpp, ptr) == 0)
                                    throw GetException();
                                break;
                            }

                        case ImageSaveFormat.Tga:
                            {
                                Get(out width, out height, out bpp, out ptr);
                                if (Imaging.CallbackWriteTga(WriteCallback, _manager, output, config.UseTgaRLE, width, height, bpp, ptr) == 0)
                                    throw GetException();
                                break;
                            }

                        case ImageSaveFormat.Jpg:
                            {
                                Get(out width, out height, out bpp, out ptr);
                                if (Imaging.CallbackWriteJpg(WriteCallback, _manager, output, config.JpgQuality, width, height, bpp, ptr) == 0)
                                    throw GetException();
                                break;
                            }

                        case ImageSaveFormat.Png:
                            {
                                Get(out width, out height, out bpp, out ptr);
                                if (Imaging.CallbackWritePng(WriteCallback, _manager, output, width, height, bpp, ptr, 0) == 0)
                                    throw GetException();
                                break;
                            }

                        default:
                            throw new ArgumentOutOfRangeException(nameof(format), $"Invalid Format: {format}");
                    }
                }
            }
        }

        private unsafe int WriteCallback(Stream stream, byte* data, int size)
        {
            if (data == null || size <= 0)
                return 0;
            
            int read = 0;
            byte[] buffer = _manager.AllocateByteArray(size);

            using (var input = new UnmanagedMemoryStream(data, size))
            {
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    stream.Write(buffer, 0, read);
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
