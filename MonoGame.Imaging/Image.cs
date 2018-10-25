using MonoGame.Utilities.IO;
using System;
using System.IO;

namespace MonoGame.Imaging
{
    public sealed partial class Image : IDisposable
    {
        public delegate void ErrorDelegate(ErrorContext errors);

        private Stream _stream;
        private readonly bool _leaveStreamOpen;
        private bool _fromPtr;

        private byte[] _tempBuffer;

        private MarshalPointer _pointer;
        private ImageInfo _cachedInfo;
        private MemoryStream _infoBuffer;

        private ReadCallbacks _callbacks;

        public bool Disposed { get; private set; }
        public object SyncRoot { get; } = new object();

        public IntPtr Pointer => GetDataPointer().SourcePtr;
        public int PointerLength { get; private set; }
        public ImageInfo Info => GetImageInfo();

        public event ErrorDelegate ErrorOccurred;
        public ErrorContext LastError { get; private set; }

        public bool LastGetInfoFailed { get; private set; }
        public bool LastGetContextFailed { get; private set; }
        public bool LastGetPointerFailed { get; private set; }

        public Image(Stream stream, bool leaveOpen)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _leaveStreamOpen = leaveOpen;

            LastError = new ErrorContext();
            _infoBuffer = new MemoryStream(512);
            _fromPtr = false;

            unsafe
            {
                _callbacks = new ReadCallbacks(ReadCallback, EoFCallback);
            }
        }

        public Image(Stream stream) : this(stream, false)
        {
        }

        public Image(IntPtr data, int width, int height, ImagePixelFormat pixelFormat)
        {
            if (data == IntPtr.Zero)
                throw new ArgumentException("Pointer is zero.", nameof(data));

            switch(pixelFormat)
            {
                case ImagePixelFormat.Grey:
                case ImagePixelFormat.GreyWithAlpha:
                case ImagePixelFormat.Rgb:
                case ImagePixelFormat.RgbWithAlpha:
                    break;

                default:
                    throw new ArgumentException("Unknown pixel format: " + pixelFormat, nameof(pixelFormat));
            }

            _pointer = new MarshalPointer(data, width * height * (int)pixelFormat);
            _cachedInfo = new ImageInfo(width, height, pixelFormat, ImageFormat.RawData);
            _fromPtr = true;
        }

        private void TriggerError()
        {
            ErrorOccurred?.Invoke(LastError);
        }

        private ImageInfo GetImageInfo()
        {
            lock (SyncRoot)
            {
                if (Disposed == false)
                {
                    if (LastGetInfoFailed || LastGetContextFailed)
                        return null;

                    if (_cachedInfo == null)
                    {
                        //_tempBuffer = _manager.();
                        
                        ReadContext rc = GetReadContext();
                        LastGetInfoFailed = CheckInvalidReadCtx(rc);
                        if (LastGetInfoFailed == false)
                        {
                            _cachedInfo = Imaging.GetImageInfo(rc);
                            if (_cachedInfo.IsValid() == false || _cachedInfo == null)
                            {
                                LastGetInfoFailed = true;
                                TriggerError();
                                return null;
                            }

                            _stream = new MultiStream(_infoBuffer, _stream);
                            _infoBuffer.Position = 0;
                            _infoBuffer = null;
                        }

                        //_manager.Return(_tempBuffer);
                    }
                }

                return _cachedInfo;
            }
        }

        private MarshalPointer GetDataPointer()
        {
            lock (SyncRoot)
            {
                CheckDisposed();

                if (LastGetContextFailed || LastGetPointerFailed || LastGetInfoFailed)
                    return default;

                if (_pointer.SourcePtr == IntPtr.Zero)
                {
                    ImageInfo info = Info;
                    if (info == null)
                        LastError.AddError(ImagingError.NoImageInfo);
                    else
                    {
                        try
                        {
                            //_tempBuffer = _manager.Rent();

                            int bpp = (int)info.PixelFormat;
                            ReadContext rc = GetReadContext();
                            if (rc != null)
                            {
                                IntPtr data = Imaging.LoadFromInfo8(rc, info, bpp);
                                if (data == IntPtr.Zero)
                                    LastGetPointerFailed = true;
                                else
                                {
                                    PointerLength = info.Width * info.Height * bpp;
                                    _pointer = new MarshalPointer(data, PointerLength);
                                }
                            }
                        }
                        finally
                        {
                            //_manager.Return(_tempBuffer);
                        }
                    }
                    CloseStream();
                }

                return _pointer;
            }
        }

        private void CheckDisposed()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Image));
        }

        private void CloseStream()
        {
            if (_stream != null)
            {
                if (_leaveStreamOpen == false)
                    _stream.Dispose();
                _stream = null;
            }
        }

        private void Dispose(bool disposing)
        {
            lock (SyncRoot)
            {
                if (!Disposed)
                {
                    if (_fromPtr == false)
                    {
                        _pointer.Dispose();
                        CloseStream();
                    }

                    _pointer = default;
                    Disposed = true;
                }
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Image()
        {
            Dispose(false);
        }
    }
}
