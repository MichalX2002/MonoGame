using System.IO;

namespace MonoGame.Imaging
{
    internal unsafe readonly struct ReadCallbacks
    {
        public delegate int ReadCallback(Stream stream, byte* data, byte[] buffer, int size);
        public delegate int EoFCallback(Stream stream);

        public readonly ReadCallback Read;
        public readonly EoFCallback EoF;

        public ReadCallbacks(ReadCallback read, EoFCallback eof)
        {
            Read = read;
            EoF = eof;
        }
    }
}
