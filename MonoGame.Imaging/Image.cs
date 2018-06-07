using System;
using System.IO;

namespace MonoGame.Imaging
{
    public sealed partial class Image : IDisposable
    {
        public delegate void ErrorDelegate(ErrorContext errors);

        private Stream _stream;
        private MemoryManager _manager;
        private readonly bool _leaveStreamOpen;
        private readonly bool _leaveManagerOpen;
        private bool _fromPtr;

        private IntPtr _pointer;
        private ImageInfo _cachedInfo;
        private MemoryStream _infoBuffer;

        private ReadCallbacks _callbacks;

        public bool Disposed { get; private set; }
        public object SyncRoot { get; } = new object();

        public IntPtr Pointer => GetDataPointer();
        public int PointerLength { get; private set; }
        public ImageInfo Info => GetImageInfo();

        public event ErrorDelegate ErrorOccurred;
        public ErrorContext LastError { get; private set; }

        public bool LastGetInfoFailed { get; private set; }
        public bool LastGetContextFailed { get; private set; }
        public bool LastGetPointerFailed { get; private set; }
        
        public Image(Stream stream, bool leaveStreamOpen,
            MemoryManager manager, bool leaveManagerOpen)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _leaveStreamOpen = leaveStreamOpen;

            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _leaveManagerOpen = leaveManagerOpen;
            
            LastError = new ErrorContext();
            _infoBuffer = new MemoryStream();

            unsafe
            {
                _callbacks = new ReadCallbacks(ReadCallback, EoFCallback);
            }
        }

        public Image(Stream stream, bool leaveOpen) :
            this(stream, leaveOpen, new MemoryManager(true), false)
        {
        }

        public Image(IntPtr data, int width, int height, ImagePixelFormat pixelFormat)
        {
            _pointer = data;
            _cachedInfo = new ImageInfo(width, height, pixelFormat, ImageFormat.RawData);
            _fromPtr = true;
        }

        private void TriggerError()
        {
            ErrorOccurred?.Invoke(LastError);
        }

        public ImageInfo GetImageInfo()
        {
            lock (SyncRoot)
            {
                if (Disposed == false)
                {
                    if (LastGetInfoFailed || LastGetContextFailed)
                        return null;

                    if (_cachedInfo == null)
                    {
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
                    }
                }

                return _cachedInfo;
            }
        }

        public IntPtr GetDataPointer()
        {
            lock (SyncRoot)
            {
                CheckDisposed();

                if (LastGetContextFailed || LastGetPointerFailed || LastGetInfoFailed)
                    return IntPtr.Zero;

                if (_pointer == IntPtr.Zero)
                {
                    ReadContext rc = GetReadContext();
                    LastGetPointerFailed = CheckInvalidReadCtx(rc);
                    if (LastGetPointerFailed == false)
                    {
                        ImageInfo info = GetImageInfo();
                        if (LastGetInfoFailed == false)
                        {
                            int bpp = (int)info.PixelFormat;
                            _pointer = Imaging.LoadFromInfo8(rc, info, bpp);
                            if (_pointer == IntPtr.Zero)
                                LastGetPointerFailed = true;
                            else
                                PointerLength = info.Width * info.Height * bpp;
                        }
                        else
                            LastError.AddError("no image info");
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
                        MemoryManager.Free(_manager, _pointer);

                        if (_leaveManagerOpen == false)
                            _manager.Dispose();

                        CloseStream();

                        _manager = null;
                    }

                    _pointer = IntPtr.Zero;

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
