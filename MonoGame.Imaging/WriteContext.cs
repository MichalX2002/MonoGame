using System.IO;

namespace MonoGame.Imaging
{
    public unsafe delegate int WriteCallback(Stream stream, byte* data, int size);

    public struct WriteContext
    {
        public WriteCallback Write;
        public MemoryManager Manager;
        public Stream Stream;

        public bool WriteTgaWithRle;

        public WriteContext(
            WriteCallback callback, MemoryManager manager, Stream stream, bool writeTgaWithRle)
        {
            Write = callback;
            Manager = manager;
            Stream = stream;

            WriteTgaWithRle = writeTgaWithRle;
        }
    }
}
