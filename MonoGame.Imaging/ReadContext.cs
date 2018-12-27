using System;
using System.IO;

namespace MonoGame.Imaging
{
    internal unsafe class ReadContext : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public uint Width;
        public uint Height;
        public int SourceChannels;
        public int OutChannels;

        public Stream Stream;
        public ErrorContext ErrorCtx;
        public ReadCallbacks Callbacks;
        public byte[] Buffer;
        public int ReadFromCallbacks;
        public int BufLength;
        public MarshalPointer BufferStart = Imaging.MAlloc(128);
        public byte* ImgBuffer;
        public byte* ImgBufferEnd;
        public byte* ImgBufferOrigin;
        public byte* ImgBufferEndOrigin;

        public int Error(ImagingError error)
        {
            return ErrorCtx.Error(error);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                BufferStart.Free();
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ReadContext()
        {
            Dispose(false);
        }
    }
}
