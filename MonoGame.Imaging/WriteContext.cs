using MonoGame.Utilities.IO;
using System.IO;

namespace MonoGame.Imaging
{
    public unsafe delegate int WriteCallback(Stream stream, byte* data, int size);

    public class WriteContext
    {
        public WriteCallback Write;
        public Stream Stream;
        public bool WriteTgaWithRle;

        public WriteContext(
            WriteCallback callback, Stream stream, bool writeTgaWithRle)
        {
            Write = callback;
            Stream = stream;

            WriteTgaWithRle = writeTgaWithRle;
        }
    }
}
