
namespace MonoGame.Imaging
{
    internal unsafe class ReadContext
    {
        public uint Width;
        public uint Height;
        public int SourceChannels;
        public int OutChannels;

        public MemoryManager Manager;
        public ErrorContext ErrorCtx;
        public ReadCallbacks Callbacks;
        public void* IOUserData;
        public int ReadFromCallbacks;
        public int BufLength;
        public MarshalPointer<byte> BufferStart = new MarshalPointer<byte>(128);
        public byte* ImgBuffer;
        public byte* ImgBufferEnd;
        public byte* ImgBufferOrigin;
        public byte* ImgBufferEndOrigin;

        public int Error(string error)
        {
            return ErrorCtx.Error(error);
        }
    }
}
