using System.IO;
using System.Threading;

namespace MonoGame.Framework.IO
{
    /// <summary>
    /// Determines how a <see cref="Stream"/> should be disposed during or after an operation.
    /// </para>
    /// </summary>
    public enum StreamDisposeMethod
    {
        /// <summary>
        /// Close the inner stream after disposal or when the
        /// <see cref="CancellationToken"/> requests cancellation.
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
        /// Leave the inner stream open after disposal but close it when the
        /// <see cref="CancellationToken"/> requests cancellation.
        /// </summary>
        CancellableLeaveOpen
    }
}
