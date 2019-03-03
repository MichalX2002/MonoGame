using System;
using System.Diagnostics;
using System.IO;

namespace MonoGame.Imaging
{
    public sealed partial class Image
    {
        private void CheckImageInfo(ImageInfo info)
        {
            if (info == null || !info.IsValid())
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
                    $"Source format '{info.SourceFormat}' is not supported.");

            Save(output, saveFormat, config);
        }

        public void Save(Stream output, ImageSaveFormat format)
        {
            Save(output, format, SaveConfiguration.Default);
        }

        private string AppendErrors(string prefix)
        {
            string value = prefix;
            if (Errors.Count > 0)
                value += "\n " + Errors;
            return value;
        }

        [DebuggerHidden]
        private void ThrowSaveException(ImageSaveFormat format)
        {
            throw new InvalidOperationException(AppendErrors($"Could not save image to format '{format}'."));
        }
        
        private unsafe void GetPtrForSave(out int w, out int h, out int p, out byte* d)
        {
            ImageInfo info = GetImageInfo();
            CheckImageInfo(info);

            IntPtr data = GetPointer();
            if (!IsLoaded)
                throw new InvalidDataException(AppendErrors("Could not decode source image stream."));

            if (data == IntPtr.Zero)
                throw new InvalidOperationException(
                    $"No image data is present in this {nameof(Image)}.");

            w = info.Width;
            h = info.Height;
            p = (int)info.PixelFormat;
            d = (byte*)data;
        }
        
        public unsafe void Save(Stream output, ImageSaveFormat format, SaveConfiguration config)
        {
            lock (SyncRoot)
            {
                AssertNotDisposed();

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
                                GetPtrForSave(out width, out height, out bpp, out ptr);
                                if (Imaging.CallbackWriteBmp(writeCtx, width, height, bpp, ptr) == 0)
                                    ThrowSaveException(format);
                                break;
                            }

                        case ImageSaveFormat.Tga:
                            {
                                GetPtrForSave(out width, out height, out bpp, out ptr);
                                if (Imaging.CallbackWriteTga(writeCtx, width, height, bpp, ptr) == 0)
                                    ThrowSaveException(format);
                                break;
                            }

                        case ImageSaveFormat.Jpg:
                            {
                                GetPtrForSave(out width, out height, out bpp, out ptr);
                                if (Imaging.CallbackWriteJpg(writeCtx, config.JpgQuality, width, height, bpp, ptr) == 0)
                                    ThrowSaveException(format);
                                break;
                            }

                        case ImageSaveFormat.Png:
                            {
                                GetPtrForSave(out width, out height, out bpp, out ptr);
                                if (Imaging.CallbackWritePng(writeCtx, width, height, bpp, ptr, 0) == 0)
                                    ThrowSaveException(format);
                                break;
                            }

                        default:
                            throw new ArgumentOutOfRangeException(nameof(format), $"Invalid Format: {format}");
                    }
                }
                catch(Exception exc)
                {
                    TriggerError(ImagingError.SaveException, exc);
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
