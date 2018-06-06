
namespace MonoGame.Imaging
{
    internal unsafe struct ReadCallbacks
    {
        public delegate int ReadCallback(void* user, byte* data, int size);
        public delegate int EoFCallback(void* user);

        public ReadCallback Read;
        public EoFCallback EoF;

        public ReadCallbacks(ReadCallback read, EoFCallback eof)
        {
            Read = read;
            EoF = eof;
        }
    }
}
