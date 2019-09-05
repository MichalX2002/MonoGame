using System;
using System.Collections;
using System.Collections.Generic;
using MonoGame.Framework;

namespace MonoGame.Utilities
{
    public static class CommonArgumentGuard
    {
        public static void CheckSrcDstSpans<T>(ReadOnlySpan<T> source, Span<T> destination)
        {
            if (source.IsEmpty) throw new ArgumentEmptyException(nameof(source));
            if (destination.IsEmpty) throw new ArgumentEmptyException(nameof(destination));

            if (destination.Length < source.Length)
                throw new ArgumentException(
                    $"The destination is smaller than the source.", nameof(destination));
        }

        /// <summary>
        /// Throws if the <see cref="collection"/> is empty.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <param name="inlineParamName"></param>
        public static void AssertNonEmpty(int? count, string paramName, bool inlineParamName = true)
        {
            if (!count.HasValue)
                throw new ArgumentNullException(nameof(paramName));

            if (count.Value <= 0)
            {
                string name = inlineParamName ? paramName : "The collection";
                throw new ArgumentEmptyException(paramName, $"{name} may not be empty.");
            }
        }

        /// <summary>
        /// Throws if the <see cref="value"/> is less or equal to zero.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <param name="inlineParamName"></param>
        public static void AssertAboveZero(int value, string paramName, bool inlineParamName = true)
        {
            if (value <= 0)
            {
                string name = inlineParamName ? paramName : "value";
                throw new ArgumentOutOfRangeException(
                    $"The {name} must be greater than zero.", paramName);
            }
        }

        /// <summary>
        /// Throws if the <see cref="value"/> is less than zero.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <param name="inlineParamName"></param>
        public static void AssertAtleastZero(int value, string paramName, bool inlineParamName = true)
        {
            if (value < 0)
            {
                string name = inlineParamName ? paramName : "value";
                throw new ArgumentOutOfRangeException(
                    $"The {name} must be equal to or greater than zero.", paramName);
            }
        }
    }
}
