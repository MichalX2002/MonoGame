
namespace MonoGame.Imaging
{
    internal unsafe readonly struct ReadCallbacks
    {
        public delegate int ReadCallback(byte* data, int size);
        public delegate int EoFCallback();

        public readonly ReadCallback Read;
        public readonly EoFCallback EoF;

        public ReadCallbacks(ReadCallback read, EoFCallback eof)
        {
            Read = read;
            EoF = eof;
        }
    }
}
