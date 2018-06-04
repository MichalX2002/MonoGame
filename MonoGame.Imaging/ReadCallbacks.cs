using System;

namespace MonoGame.Imaging
{
    internal unsafe struct ReadCallbacks
    {
        public delegate int ReadCallback(void* user, sbyte* data, int size);
        public delegate int SkipCallback(void* user, int n);
        public delegate int EoFCallback(void* user);

        public ReadCallback Read;
        public SkipCallback Skip;
        public EoFCallback EoF;

        public ReadCallbacks(ReadCallback read, SkipCallback skip, EoFCallback eof)
        {
            Read = read;
            Skip = skip;
            EoF = eof;
        }
    }
}
