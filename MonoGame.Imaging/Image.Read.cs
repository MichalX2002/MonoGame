
namespace MonoGame.Imaging
{
    public partial class Image
    {
        private ReadContext GetReadContext()
        {
            lock (SyncRoot)
            {
                unsafe
                {
                    if (LastGetContextFailed)
                        return null;

                    var context = Imaging.GetReadContext(_manager, LastError, _callbacks);
                    if (context == null)
                        LastGetContextFailed = true;

                    return context;
                }
            }
        }

        private bool CheckInvalidReadCtx(ReadContext context)
        {
            return context == null;
        }

        private unsafe int EoFCallback()
        {
            return _stream.CanRead ? 1 : 0;
        }

        private unsafe int ReadCallback(byte* data, int size)
        {
            if (data == null || size <= 0)
                return 0;

            int leftToRead = size;
            int read = 0;
            while (leftToRead > 0 && (read = _stream.Read(_tempBuffer, 0, leftToRead)) > 0)
            {
                for (int i = 0; i < read; i++)
                    data[i + size - leftToRead] = _tempBuffer[i];

                _infoBuffer?.Write(_tempBuffer, 0, read);
                leftToRead -= read;
            }

            return size - leftToRead;
        }
    }
}
