using System;
using System.Collections.Generic;

namespace MonoGame.Framework.Audio
{
    internal static class ALBufferPool
    {
        private static Stack<ALBuffer> _pool = new Stack<ALBuffer>();

        public static ALBuffer Rent()
        {
            lock (_pool)
            {
                if (_pool.Count > 0)
                    return _pool.Pop();
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
                if (_pool.Count < 256 && !_pool.Contains(buffer))
                {
                    buffer.ClearBuffer();
                    _pool.Push(buffer);
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
                    _pool.Pop().Dispose();
            }
        }
    }
}
