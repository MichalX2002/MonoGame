using System.IO;

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

                    var context = Imaging.GetReadContext(
                            _manager, LastError, _callbacks, null);

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

        private unsafe int EoFCallback(void* user)
        {
            return _stream.CanRead ? 1 : 0;
        }

        private unsafe int ReadCallback(void* user, byte* data, int size)
        {
            if (data == null || size <= 0)
                return 0;

            int bufferSize = _manager.GetOptimalByteArraySize(size);
            byte[] buffer = _manager.AllocateByteArray(bufferSize);

            int leftToRead = size;
            int read = 0;

            using (var stream = new UnmanagedMemoryStream(data, size, size, FileAccess.Write))
            {
                while (leftToRead > 0 && (read = _stream.Read(buffer, 0, leftToRead)) > 0)
                {
                    stream.Write(buffer, 0, read);
                    _infoBuffer?.Write(buffer, 0, read);

                    leftToRead -= read;
                }
            }

            _manager.ReleaseByteArray(buffer);

            return size - leftToRead;
        }
    }
}
