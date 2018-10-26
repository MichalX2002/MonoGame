
using System.IO;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        private ReadContext GetReadContext(Stream stream, byte[] buffer)
        {
            lock (SyncRoot)
            {
                unsafe
                {
                    if (LastContextFailed)
                        return null;

                    var context = Imaging.GetReadContext(stream, LastError, _callbacks, buffer);
                    if (context == null)
                        LastContextFailed = true;

                    return context;
                }
            }
        }

        private unsafe int EoFCallback(Stream stream)
        {
            return stream.CanRead ? 1 : 0;
        }

        private unsafe int ReadCallback(Stream stream, byte* data, byte[] buffer, int size)
        {
            if (data == null || size <= 0)
                return 0;

            int leftToRead = size;
            int read = 0;
            while (leftToRead > 0 && (read = stream.Read(buffer, 0, leftToRead)) > 0)
            {
                for (int i = 0; i < read; i++)
                    data[i + size - leftToRead] = buffer[i];

                _infoBuffer?.Write(buffer, 0, read);
                leftToRead -= read;
            }

            return size - leftToRead;
        }
    }
}
