
using System;
using System.IO;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        private ImageInfo GetImageInfo()
        {
            lock (SyncRoot)
            {
                if (IsDisposed == false)
                {
                    if (LastInfoFailed || LastContextFailed)
                        return null;

                    if (_cachedInfo == null)
                    {
                        var buffer = _memoryManager.GetBlock();
                        ReadContext rc = GetReadContext(_sourceStream, buffer);
                        LastInfoFailed = rc == null;
                        if (LastInfoFailed == false)
                        {
                            _cachedInfo = Imaging.GetImageInfo(rc);
                            if (_cachedInfo.IsValid() == false || _cachedInfo == null)
                            {
                                LastInfoFailed = true;
                                TriggerError();
                                return null;
                            }

                            _combinedStream = new MultiStream(_infoBuffer, _sourceStream);
                            _infoBuffer.Position = 0;
                            _infoBuffer = null;
                        }
                        _memoryManager.ReturnBlock(buffer, null);
                    }
                }
                return _cachedInfo;
            }
        }

        private unsafe MarshalPointer GetDataPointer()
        {
            lock (SyncRoot)
            {
                CheckDisposed();

                if (LastContextFailed || LastPointerFailed || LastInfoFailed)
                    return default;

                if (_pointer.Ptr == null)
                {
                    ImageInfo info = Info;
                    if (info == null)
                    {
                        LastError.AddError(ImagingError.NoImageInfo);
                        LastPointerFailed = true;
                        return default;
                    }

                    var bufferStream = _memoryManager.GetReadBufferedStream(_combinedStream, true);
                    var buffer = _memoryManager.GetBlock();
                    try
                    {
                        ReadContext rc = GetReadContext(bufferStream, buffer);
                        LastPointerFailed = rc == null;
                        if (LastPointerFailed == false)
                        {
                            int bpp = (int)info.PixelFormat;
                            IntPtr data = Imaging.LoadFromInfo8(rc, info, bpp);
                            if (data == IntPtr.Zero)
                            {
                                LastPointerFailed = true;
                                return default;
                            }

                            PointerLength = info.Width * info.Height * bpp;
                            _pointer = new MarshalPointer(data, PointerLength);
                            LastPointerFailed = false;
                        }
                    }
                    catch
                    {
                        LastPointerFailed = true;
                    }
                    finally
                    {
                        _memoryManager.ReturnBlock(buffer, null);
                        bufferStream.Dispose();
                        CloseStream();
                    }
                }
                return _pointer;
            }
        }

        private ReadContext GetReadContext(Stream stream, byte[] buffer)
        {
            lock (SyncRoot)
            {
                unsafe
                {
                    if (LastContextFailed)
                        return null;

                    var context = Imaging.GetReadContext(stream, LastError, _callbacks, buffer);
                    LastContextFailed = context == null;
                    return context;
                }
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

            int leftToRead = size;
            int read = 0;
            while (leftToRead > 0 && (read = stream.Read(buffer, 0, leftToRead)) > 0)
            {
                for (int i = 0; i < read; i++)
                    data[i + size - leftToRead] = buffer[i];

                _infoBuffer?.Write(buffer, 0, read);
                leftToRead -= read;
            }

            return size - leftToRead;
        }
    }
}
