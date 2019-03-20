using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework.Audio
{
    internal static class OALSoundBufferPool
    {
        private static Queue<OALSoundBuffer> _pool = new Queue<OALSoundBuffer>();

        public static OALSoundBuffer Rent()
        {
            lock (_pool)
            {
                if (_pool.Count > 0)
                    return _pool.Dequeue();
            }
            return new OALSoundBuffer();
        }

        public static void Return(OALSoundBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.IsDisposed)
                throw new ObjectDisposedException(nameof(OALSoundBuffer));

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
