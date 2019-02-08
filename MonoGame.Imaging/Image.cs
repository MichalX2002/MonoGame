using MonoGame.Utilities.IO;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MonoGame.Imaging
{
    public sealed partial class Image : IDisposable
    {
        public delegate void ErrorDelegate(ErrorContext errors);
        private static RecyclableMemoryManager _memoryManager = RecyclableMemoryManager.Instance;

        private Stream _sourceStream;
        private MultiStream _combinedStream;
        private readonly bool _leaveStreamOpen;
        private bool _leavePointerOpen;

        private MarshalPointer _pointer;
        private ImagePixelFormat _desiredFormat;
        private ImageInfo _cachedInfo;
        private MemoryStream _infoBuffer;
        
        public bool IsDisposed { get; private set; }
        public object SyncRoot { get; } = new object();

        public int PointerLength { get; private set; }
        public ImageInfo Info => GetImageInfo();
        public int Width => Info.Width;
        public int Height => Info.Height;
        public ImageFormat SourceFormat => Info.SourceFormat;
        public ImagePixelFormat PixelFormat => Info.PixelFormat;
        public ImagePixelFormat SourcePixelFormat => Info.SourcePixelFormat;
        public ImagePixelFormat DesiredPixelFormat => Info.DesiredPixelFormat;

        public event ErrorDelegate ErrorOccurred;
        public ErrorContext Errors { get; private set; }

        public bool LastInfoFailed { get; private set; }
        public bool LastContextFailed { get; private set; }
        public bool LastPointerFailed { get; private set; }

        public Image(Stream stream, ImagePixelFormat desiredFormat, bool leaveOpen)
        {
            _sourceStream = stream ?? throw new ArgumentNullException(nameof(stream));
            _leaveStreamOpen = leaveOpen;

            _desiredFormat = desiredFormat;
            switch (_desiredFormat)
            {
                case ImagePixelFormat.Grey:
                case ImagePixelFormat.GreyWithAlpha:
                case ImagePixelFormat.Rgb:
                case ImagePixelFormat.RgbWithAlpha:
                case ImagePixelFormat.Source:
                    break;

                default:
                    throw new ArgumentException(desiredFormat + " is not valid.", nameof(desiredFormat));
            }

            Errors = new ErrorContext();
            _infoBuffer = _memoryManager.GetMemoryStream();
            _leavePointerOpen = false;
            PointerLength = -1;
        }

        public Image(Stream stream, bool leaveOpen) : this(stream, ImagePixelFormat.Source, leaveOpen)
        {
        }

        public Image(Stream stream, ImagePixelFormat desiredFormat) : this(stream, desiredFormat, false)
        {
        }

        public Image(Stream stream) : this(stream, ImagePixelFormat.Source)
        {
        }

        private Image(IntPtr data, bool leavePointerOpen, int width, int height, ImagePixelFormat pixelFormat)
        {
            _pointer = new MarshalPointer(data, leavePointerOpen, width * height * (int)pixelFormat);
            _leavePointerOpen = leavePointerOpen;

            PointerLength = width * height * (int)pixelFormat;
            _cachedInfo = new ImageInfo(width, height, pixelFormat, pixelFormat, ImageFormat.Pointer);
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
                    throw new ArgumentException(pixelFormat + " is not valid.", nameof(pixelFormat));
            }
        }

        public unsafe IntPtr GetPointer()
        {
            return (IntPtr)GetDataPointer().Ptr;
        }

        private void TriggerError()
        {
            ErrorOccurred?.Invoke(Errors);
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
                if (!_leaveStreamOpen)
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
                        _pointer.Free();
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
