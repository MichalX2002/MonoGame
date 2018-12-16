
using System;
using System.IO;
using System.Runtime.InteropServices;

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
                            _cachedInfo = Imaging.GetImageInfo(_desiredFormat, rc);
                            if (_cachedInfo.IsValid() == false || _cachedInfo == null)
                            {
                                LastInfoFailed = true;
                                TriggerError();
                                return null;
                            }

                            TryRemovePngSigError();
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

        private void TryRemovePngSigError()
        {
            if (_cachedInfo.SourceFormat != ImageFormat.Png)
                Errors.RemoveError(ImagingError.BadPngSignature);
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
                        Errors.AddError(ImagingError.NoImageInfo);
                        LastPointerFailed = true;
                        return default;
                    }
                    
                    var dataStream = _combinedStream; //_memoryManager.GetReadBufferedStream(_combinedStream, true);
                    var buffer = _memoryManager.GetBlock();
                    try
                    {
                        ReadContext rc = GetReadContext(dataStream, buffer);
                        LastPointerFailed = rc == null;
                        if (LastPointerFailed == false)
                        {
                            int bpp = info.SourceFormat == ImageFormat.Jpg ?
                                3 : (info.DesiredPixelFormat == ImagePixelFormat.Source ?
                                (int)info.SourcePixelFormat : (int)info.DesiredPixelFormat);

                            IntPtr result = Imaging.LoadFromInfo8(rc, info, bpp);
                            if (result == IntPtr.Zero)
                            {
                                LastPointerFailed = true;
                                return default;
                            }

                            if(info.PixelFormat == ImagePixelFormat.RgbWithAlpha && bpp == 3)
                            {
                                int pixels = info.Width * info.Height;
                                PointerLength = pixels * 4;

                                IntPtr oldResult = result;
                                result = Marshal.AllocHGlobal(PointerLength);

                                byte* srcPtr = (byte*)oldResult;
                                byte* dstPtr = (byte*)result;
                                for (int i = 0; i < pixels; i++)
                                {
                                    dstPtr[i * 4] = srcPtr[i * 3];
                                    dstPtr[i * 4 + 1] = srcPtr[i * 3 + 1];
                                    dstPtr[i * 4 + 2] = srcPtr[i * 3 + 2];
                                    dstPtr[i * 4 + 3] = 255;
                                }
                                Imaging.Free(oldResult);
                            }
                            else
                                PointerLength = info.Width * info.Height * bpp;

                            _pointer = new MarshalPointer(result, PointerLength);
                            LastPointerFailed = false;
                            TryRemovePngSigError();
                        }
                    }
                    catch
                    {
                        LastPointerFailed = true;
                    }
                    finally
                    {
                        _memoryManager.ReturnBlock(buffer, null);
                        dataStream.Dispose();
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

                    var callbacks = new ReadCallbacks(ReadCallback, EoFCallback);
                    var context = Imaging.GetReadContext(stream, Errors, callbacks, buffer);
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
