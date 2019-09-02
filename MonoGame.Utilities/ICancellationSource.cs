using System.Threading;

namespace MonoGame.Utilities
{
    /// <summary>
    /// Gives the possibility of cancellation via a <see cref="CancellationTokenSource"/>.
    /// </summary>
    public interface ICancellationSource
    {
        /// <summary>
        /// Gets whether this instance can be canceled and has a <see cref="CancellationTokenSource"/>.
        /// </summary>
        bool CanBeCanceled { get; }

        /// <summary>
        /// Gets the token source associated with this instance.
        /// </summary>
        CancellationTokenSource CancellationSource { get; }
    }
}
