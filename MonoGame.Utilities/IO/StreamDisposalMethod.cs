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
        /// Closes the inner stream after disposal.
        /// <para>
        /// For long-running reads (e.g. network resources), use <see cref="CancellableClose"/>.
        /// </para>
        /// </summary>
        Close,

        /// <summary>
        /// Leaves the inner stream open after disposal.
        /// <para>
        /// For long-running reads (e.g. network resources), use <see cref="CancellableLeaveOpen"/>.
        /// </para>
        /// </summary>
        LeaveOpen,

        /// <summary>
        /// Closes the inner stream after disposal or when the
        /// <see cref="CancellationToken"/> requests cancellation.
        /// </summary>
        CancellableClose,

        /// <summary>
        /// Leaves the inner stream open after disposal but closes it when the
        /// <see cref="CancellationToken"/> requests cancellation.
        /// </summary>
        CancellableLeaveOpen,
    }
}
