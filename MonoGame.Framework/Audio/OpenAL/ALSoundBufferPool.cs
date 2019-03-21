using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Audio
{
    internal static class ALSoundBufferPool
    {
        private static Queue<ALSoundBuffer> _pool = new Queue<ALSoundBuffer>();

        public static ALSoundBuffer Rent()
        {
            lock (_pool)
            {
                if (_pool.Count > 0)
                    return _pool.Dequeue();
            }
            return new ALSoundBuffer();
        }

        public static void Return(ALSoundBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.IsDisposed)
                throw new ObjectDisposedException(nameof(ALSoundBuffer));

            lock (_pool)
            {
                if (_pool.Count < 32 && !_pool.Contains(buffer))
                {
                    _pool.Enqueue(buffer);
                    buffer.ClearBuffer();
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
