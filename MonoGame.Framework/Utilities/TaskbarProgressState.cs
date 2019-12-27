
namespace MonoGame.Framework.Utilities
{
    /// <summary>
    /// Different states for the taskbar progress indicator.
    /// </summary>
    public enum TaskbarProgressState
    {
        /// <summary>
        /// Stops displaying progress and returns the indicator to its normal state. 
        /// </summary>
        None,

        /// <summary>
        /// The progress indicator does not grow in size, but plays a repeating animation.
        /// </summary>
        Indeterminate,

        /// <summary>
        /// The progress indicator grows in size in proportion to the progress.
        /// </summary>
        Normal,

        /// <summary>
        /// The progress indicator turns red to show that an error has occurred. 
        /// This is a determinate state. 
        /// </summary>
        Error,

        /// <summary>
        /// The progress indicator turns yellow to show that progress is paused.
        /// This is a determinate state.
        /// </summary>
        Paused
    }
}
