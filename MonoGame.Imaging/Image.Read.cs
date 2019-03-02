
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        public bool Load()
        {
            AssertNotDisposed();
            lock (SyncRoot)
            {
                if (!IsLoaded)
                    IsLoaded = LoadInternal();
                return IsLoaded;
            }
        }

        private ImageInfo GetImageInfo()
        {
            AssertNotDisposed();
            lock (SyncRoot)
            {
                if (_cachedImageInfo == null)
                    _cachedImageInfo = CreateImageInfo();
                return _cachedImageInfo;
            }
        }

        private bool LoadInternal()
        {
            ImageInfo info = GetImageInfo();
            if (info == null)
            {
                Errors.AddError(ImagingError.NoImageInfo);
                return false;
            }

            var stream = _memoryManager.GetReadBufferedStream(_combinedStream, true);
            var buffer = _memoryManager.GetBlock();
            try
            {
                ReadContext rc = GetReadContext(stream, buffer);
                if (rc == null)
                {
                    Errors.AddError(ImagingError.NoReadContext);
                    return false;
                }

                int bpp = info.SourceFormat == ImageFormat.Jpg ?
                    3 : (info.DesiredPixelFormat == ImagePixelFormat.Source ?
                    (int)info.SourcePixelFormat : (int)info.DesiredPixelFormat);

                IntPtr result = Imaging.LoadFromInfo8(rc, info, bpp);
                if (result == IntPtr.Zero)
                {
                    Errors.AddError(ImagingError.NullPointer);
                    return false;
                }

                if (info.PixelFormat == ImagePixelFormat.RgbWithAlpha && bpp == 3)
                {
                    int pixels = info.Width * info.Height;
                    PointerLength = pixels * 4;

                    IntPtr oldResult = result;
                    result = Marshal.AllocHGlobal(PointerLength);

                    unsafe
                    {
                        byte* srcPtr = (byte*)oldResult;
                        byte* dstPtr = (byte*)result;
                        for (int i = 0; i < pixels; i++)
                        {
                            dstPtr[i * 4] = srcPtr[i * 3];
                            dstPtr[i * 4 + 1] = srcPtr[i * 3 + 1];
                            dstPtr[i * 4 + 2] = srcPtr[i * 3 + 2];
                            dstPtr[i * 4 + 3] = 255;
                        }
                    }
                    Imaging.Free(oldResult);
                }
                else
                    PointerLength = info.Width * info.Height * bpp;

                _pointer = new MarshalPointer(result, PointerLength);
                TryRemovePngSigError(info);
                return true;
            }
            catch
            {
                Errors.AddError(ImagingError.Exception);
                return false;
            }
            finally
            {
                stream.Dispose();
                _memoryManager.ReturnBlock(buffer, null);
                CloseStream();
            }
        }

        private ImageInfo CreateImageInfo()
        {
            var buffer = _memoryManager.GetBlock();
            try
            {
                ReadContext rc = GetReadContext(_sourceStream, buffer);
                if (rc == null)
                {
                    Errors.AddError(ImagingError.NoReadContext);
                    return null;
                }

                var info = Imaging.GetImageInfo(_desiredFormat, rc);
                if (info == null || !info.IsValid())
                {
                    Errors.AddError(ImagingError.NoImageInfo);
                    return null;
                }

                TryRemovePngSigError(info);
                _combinedStream = new MultiStream(_infoBuffer, _sourceStream);
                _infoBuffer.Position = 0;
                _infoBuffer = null;
                return info;
            }
            catch
            {
                Errors.AddError(ImagingError.Exception);
                return null;
            }
            finally
            {
                _memoryManager.ReturnBlock(buffer, null);
            }
        }

        private void TryRemovePngSigError(ImageInfo info)
        {
            if (info.SourceFormat != ImageFormat.Png)
                Errors.RemoveError(ImagingError.BadPngSignature);
        }

        private unsafe MarshalPointer GetDataPointer()
        {
            lock (SyncRoot)
            {
                AssertNotDisposed();

                if (_lastReadCtxFailed || IsLoaded)
                    return default;

                Load();
                return _pointer;
            }
        }

        private unsafe ReadContext GetReadContext(Stream stream, byte[] buffer)
        {
            lock (SyncRoot)
            {
                if (_lastReadCtxFailed)
                    return null;

                var callbacks = new ReadCallbacks(ReadCallback, EoFCallback);
                var context = Imaging.GetReadContext(stream, Errors, callbacks, buffer);

                _lastReadCtxFailed = context == null;
                return context;
            }
        }

        private unsafe int EoFCallback(Stream stream)
        {
            return stream.CanRead ? 1 : 0;
        }
        
        private unsafe int ReadCallback(Stream stream, byte* data, byte[] buffer, int size)
        {
            if (data == null || size <= 0)
                return 0;

            // "size - left" gives us how much we've already read

            int left = size;
            int read = 0;

            while (left > 0 && (read = stream.Read(buffer, 0, Math.Min(buffer.Length, left))) > 0)
            {
                int totalRead = size - left;
                for (int i = 0; i < read; i++)
                    data[i + totalRead] = buffer[i];

                _infoBuffer?.Write(buffer, 0, read);
                left -= read;
            }

            return size - left;
        }
    }
}
