using System;

namespace MonoGame.Framework
{
    internal static class ExceptionHelper
    {
        public static bool CheckIndexedSrcDstArrays<T>(
            T[] src, int srcIndex, int length,
            T[] dst, int dstIndex, out Exception exception)
        {
            if (src == null)
            {
                exception = new ArgumentNullException(nameof(src));
                return true;
            }

            if (dst == null)
            {
                exception = new ArgumentNullException(nameof(dst));
                return true;
            }

            if (src.Length < srcIndex + length)
            {
                exception = new ArgumentException(
                    $"Source array is smaller than {nameof(srcIndex)} + {nameof(length)} " +
                    $"({src.Length} < {srcIndex + length}).",
                    nameof(src));
                return true;
            }

            if (dst.Length < dstIndex + length)
            {
                exception = new ArgumentException(
                    $"Destination array is smaller than {nameof(dstIndex)} + {nameof(length)} " +
                    $"({dst.Length} < {dstIndex + length}).",
                    nameof(dst));
                return true;
            }

            exception = null;
            return false;
        }

        public static bool CheckSrcDstArrays<T>(T[] src, T[] dst, out Exception exception)
        {
            if (src == null)
            {
                exception = new ArgumentNullException(nameof(src));
                return true;
            }

            if (dst == null)
            {
                exception = new ArgumentNullException(nameof(dst));
                return true;
            }

            if (dst.Length < src.Length)
            {
                exception = new ArgumentException(
                    $"Destination array is smaller than source array. " +
                    $"({dst.Length} < {src.Length})",
                    nameof(dst));
                return true;
            }

            exception = null;
            return false;
        }
    }
}
