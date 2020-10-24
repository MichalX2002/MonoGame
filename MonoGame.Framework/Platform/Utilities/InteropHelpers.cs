// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Text;

namespace MonoGame.Framework
{
    internal static class InteropHelpers
    {
        /// <summary>
        /// Convert a <see cref="IntPtr"/> pointing to a UTF-8 null-terminated string to a <see cref="string"/>.
        /// </summary>
        public static unsafe string Utf8ToString(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return string.Empty;

            var ptr = (byte*)handle;
            while (*ptr != 0)
                ptr++;

            long len = ptr - (byte*)handle;
            if (len == 0)
                return string.Empty;

            return Encoding.UTF8.GetString((byte*)handle, (int)len);
        }
    }
}
