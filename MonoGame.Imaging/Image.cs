using MonoGame.Utilities.IO;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    public sealed partial class Image : IDisposable
    {
        public delegate void ErrorDelegate(ErrorContext errors);
        private static RecyclableMemoryManager _memoryManager = SaveConfiguration.DefaultMemoryManager;

        private Stream _sourceStream;
        private MultiStream _combinedStream;
        private readonly bool _leaveStreamOpen;
        private bool _leavePointerOpen;

        private MarshalPointer _pointer;
        private ImageInfo _cachedInfo;
        private MemoryStream _infoBuffer;

        private ReadCallbacks _callbacks;

        public bool IsDisposed { get; private set; }
        public object SyncRoot { get; } = new object();

        public IntPtr Pointer
        {
            get
            {
                unsafe
                {
                    return (IntPtr)GetDataPointer().Ptr;
                }
            }
        }

        public int PointerLength { get; private set; }
        public ImageInfo Info => GetImageInfo();

        public event ErrorDelegate ErrorOccurred;
        public ErrorContext LastError { get; private set; }

        public bool LastInfoFailed { get; private set; }
        public bool LastContextFailed { get; private set; }
        public bool LastPointerFailed { get; private set; }

        public Image(Stream stream, bool leaveOpen)
        {
            _sourceStream = stream ?? throw new ArgumentNullException(nameof(stream));
            _leaveStreamOpen = leaveOpen;

            LastError = new ErrorContext();
            _infoBuffer = _memoryManager.GetMemoryStream();
            _leavePointerOpen = false;

            unsafe
            {
                _callbacks = new ReadCallbacks(ReadCallback, EoFCallback);
            }
        }

        public Image(Stream stream) : this(stream, false)
        {
        }

        private Image(IntPtr data, bool leavePointerOpen, int width, int height, ImagePixelFormat pixelFormat)
        {
            _pointer = new MarshalPointer(data, leavePointerOpen, width * height * (int)pixelFormat);
            _leavePointerOpen = leavePointerOpen;
            _cachedInfo = new ImageInfo(width, height, pixelFormat, ImageFormat.RawData);
        }

        public Image(IntPtr data, int width, int height, ImagePixelFormat pixelFormat) :
            this(data, true, width, height, pixelFormat)
        {
            if (data == IntPtr.Zero)
                throw new ArgumentException("Pointer is zero.", nameof(data));

            CheckPixelFormat(pixelFormat);
        }

        public Image(int width, int height, ImagePixelFormat pixelFormat) :
            this(GetNewPtr(width, height, pixelFormat), false, width, height, pixelFormat)
        {
        }

        private static IntPtr GetNewPtr(int width, int height, ImagePixelFormat pixelFormat)
        {
            CheckPixelFormat(pixelFormat);
            return Marshal.AllocHGlobal(width * height * (int)pixelFormat);
        }

        private static void CheckPixelFormat(ImagePixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case ImagePixelFormat.Grey:
                case ImagePixelFormat.GreyWithAlpha:
                case ImagePixelFormat.Rgb:
                case ImagePixelFormat.RgbWithAlpha:
                    break;

                default:
                    throw new ArgumentException("Unknown pixel format: " + pixelFormat, nameof(pixelFormat));
            }
        }

        private void TriggerError()
        {
            ErrorOccurred?.Invoke(LastError);
        }

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

                            _combinedStream = new MultiStream(_infoBuffer, _sourceStream, _sourceStream.Length);
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
                        LastError.AddError(ImagingError.NoImageInfo);
                    else
                    {
                        using (var bufferStream = _memoryManager.GetReadBufferedStream(_combinedStream, true))
                        {
                            var buffer = _memoryManager.GetBlock();
                            try
                            {
                                ReadContext rc = GetReadContext(bufferStream, buffer);
                                if (rc != null)
                                {
                                    int bpp = (int)info.PixelFormat;
                                    IntPtr data = Imaging.LoadFromInfo8(rc, info, bpp);
                                    if (data != IntPtr.Zero)
                                    {
                                        PointerLength = info.Width * info.Height * bpp;
                                        _pointer = new MarshalPointer(data, PointerLength);
                                    }
                                    else
                                        LastPointerFailed = true;
                                }
                                else
                                    LastPointerFailed = true;
                            }
                            finally
                            {
                                _memoryManager.ReturnBlock(buffer, null);
                            }
                        }
                    }
                    CloseStream();
                }
                return _pointer;
            }
        }

        private void CheckDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(Image));
        }

        private void CloseStream()
        {
            if (_sourceStream != null)
            {
                if (_leaveStreamOpen == false)
                    _sourceStream.Dispose();
                _sourceStream = null;
            }
        }

        private void Dispose(bool disposing)
        {
            lock (SyncRoot)
            {
                if (!IsDisposed)
                {
                    CloseStream();

                    if (_leavePointerOpen == false)
                        _pointer.Dispose();
                    _pointer = default;

                    IsDisposed = true;
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
