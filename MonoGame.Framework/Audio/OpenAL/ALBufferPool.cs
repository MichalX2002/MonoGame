using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Audio
{
    internal static class ALBufferPool
    {
        private static Queue<ALBuffer> _pool = new Queue<ALBuffer>();

        public static ALBuffer Rent()
        {
            lock (_pool)
            {
                if (_pool.Count > 0)
                    return _pool.Dequeue();
            }
            return new ALBuffer();
        }

        public static void Return(ALBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.IsDisposed)
                throw new ObjectDisposedException(nameof(ALBuffer));

            lock (_pool)
            {
                if (_pool.Count < 32 && !_pool.Contains(buffer))
                {
                    buffer.ClearBuffer();
                    _pool.Enqueue(buffer);
                }
                else
                    buffer.Dispose();
            }
        }

        public static void Clear()
        {
            lock (_pool)
            {
                while (_pool.Count > 0)
                    _pool.Dequeue().Dispose();
            }
        }
    }
}
