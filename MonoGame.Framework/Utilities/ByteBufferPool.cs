using System.Collections.Generic;

namespace MonoGame.Framework
{
    internal class ByteBufferPool
    {
        public int FreeAmount => _freeBuffers.Count;

        private readonly List<byte[]> _freeBuffers;

        public ByteBufferPool()
        {
            _freeBuffers = new List<byte[]>();
        }

        /// <summary>
        /// Get a buffer that is at least as big as size.
        /// </summary>
        public byte[] Get(int size)
        {
            byte[] result;
            lock (_freeBuffers)
            {
                int index = FirstLargerThan(size);
                if (index == -1)
                    result = new byte[size];
                else
                {
                    result = _freeBuffers[index];
                    _freeBuffers.RemoveAt(index);
                }
            }
            return result;
        }

        /// <summary>
        /// Return the given buffer to the pool.
        /// </summary>
        /// <param name="buffer"></param>
        public void Return(byte[] buffer)
        {
            lock (_freeBuffers)
            {
                int index = FirstLargerThan(buffer.Length);
                if (index == -1)
                    _freeBuffers.Add(buffer);
                else
                    _freeBuffers.Insert(index, buffer);
            }
        }

        // Find the smallest buffer that is larger than or equally large as size or -1 if none exist
        private int FirstLargerThan(int size)
        {
            if (_freeBuffers.Count == 0)
                return -1;

            int l = 0;
            int r = _freeBuffers.Count - 1;

            while (l <= r)
            {
                int m = (l + r)/2;
                byte[] buffer = _freeBuffers[m];
                if (buffer.Length < size)
                {
                    l = m + 1;
                }
                else if (buffer.Length > size)
                {
                    r = m;
                    if (l == r)
                        return l;
                }
                else
                    return m;
            }

            return -1;
        }
    }
}
