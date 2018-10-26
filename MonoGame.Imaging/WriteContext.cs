using System.IO;

namespace MonoGame.Imaging
{
    public unsafe delegate void WriteCallback(Stream stream, byte* data, byte[] buffer, int size);

    public class WriteContext
    {
        public WriteCallback Write;

        public Stream Stream;
        public byte[] Buffer;
        public bool WriteTgaWithRLE;

        public WriteContext(
            WriteCallback callback, Stream stream, byte[] buffer, bool writeTgaWithRLE)
        {
            Write = callback;

            Stream = stream;
            Buffer = buffer;
            WriteTgaWithRLE = writeTgaWithRLE;
        }
    }
}
