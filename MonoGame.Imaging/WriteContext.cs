using System.IO;

namespace MonoGame.Imaging
{
    public unsafe delegate int WriteCallback(
        Stream stream, byte* data, int size, in WriteContext c);

    public struct WriteContext
    {
        public SaveConfiguration.ImageWritingDelegate OnWrite;

        public WriteCallback Write;
        public MemoryManager Manager;
        public Stream Stream;

        public bool WriteTgaWithRle;

        public WriteContext(SaveConfiguration.ImageWritingDelegate onWrite,
            WriteCallback callback, MemoryManager manager, Stream stream, bool writeTgaWithRle)
        {
            OnWrite = onWrite;

            Write = callback;
            Manager = manager;
            Stream = stream;

            WriteTgaWithRle = writeTgaWithRle;
        }
    }
}
