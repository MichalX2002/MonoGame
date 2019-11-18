using System.IO;
using System.Threading;

namespace MonoGame.Utilities.IO
{
    /// <summary>
    /// Determines how a <see cref="Stream"/> should be disposed during or after an operation.
    /// <para>
    /// Commonly used to dispose streams with a <see cref="CancellationTokenRegistration"/> callback.
    /// </para>
    /// </summary>
    public enum StreamDisposalMethod
    {
        /// <summary>
        /// Close the inner stream after disposal.
        /// <para>
        /// <see cref="CancellableClose"/> is recommended for long-running operations (e.g. network resources).
        /// </para>
        /// </summary>
        Close,

        /// <summary>
        /// Leave the inner stream open after disposal.
        /// <para>
        /// <see cref="CancellableLeaveOpen"/> is recommended for long-running operations (e.g. network resources).
        /// </para>
        /// </summary>
        LeaveOpen,

        /// <summary>
        /// Close the inner stream after disposal or when the
        /// <see cref="CancellationToken"/> requests cancellation.
        /// </summary>
        CancellableClose,

        /// <summary>
        /// Leave the inner stream open after disposal but close it when the
        /// <see cref="CancellationToken"/> requests cancellation.
        /// </summary>
        CancellableLeaveOpen,
    }
}
