using System.IO;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        private ReadContext GetReadContext()
        {
            lock (SyncRoot)
            {
                if (LastGetContextFailed)
                    return null;

                if (_readContext == null)
                {
                    unsafe
                    {
                        _readContext = Imaging.GetReadContext(
                            _manager, LastError, _callbacks, null);
                    }

                    if (LastError.Count > 0)
                    {
                        LastGetContextFailed = true;
                        TriggerError();
                        return null;
                    }
                }

                _readContext.ErrorCtx.Clear();
                return _readContext;
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

            int leftToRead = size;
            int read = 0;
            byte[] buffer = _manager.AllocateByteArray(size);

            using (var stream = new UnmanagedMemoryStream(data, size, size, FileAccess.Write))
            {
                while (leftToRead > 0 && (read = _stream.Read(buffer, 0, size)) > 0)
                {
                    stream.Write(buffer, 0, read);
                    leftToRead -= read;
                }
            }

            return size - leftToRead;
        }
    }
}
