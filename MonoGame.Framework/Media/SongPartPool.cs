using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Media
{
    internal static class SongPartPool
    {
        private static object _mutex = new object();
        private static Stack<SongPart> _pool = new Stack<SongPart>();

        public static int MAX_PARTS = 64;

        public static SongPart Rent(int bufferSize)
        {
            lock (_mutex)
            {
                TryPop:
                if (_pool.Count > 0)
                {
                    var result = _pool.Pop();
                    if (result.Data.Length < bufferSize)
                        goto TryPop;
                    return result;
                }
                return new SongPart(bufferSize);
            }
        }

        public static void Return(SongPart part)
        {
            if (part == null)
                throw new ArgumentNullException();

            lock (_mutex)
            {
                if (_pool.Count < MAX_PARTS)
                    _pool.Push(part);
            }
        }
    }
}