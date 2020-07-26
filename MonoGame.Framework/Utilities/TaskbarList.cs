using System;

namespace MonoGame.Framework.Utilities
{
    /// <summary>
    /// Exposes taskbar utilities on desktop platforms.
    /// </summary>
    public partial class TaskbarList : GameWindowDependency
    {
        private object SyncRoot { get; } = new object();

        private TaskbarProgressState _progressState;
        private TaskbarProgressValue _progressValue;

        /// <summary>
        /// Gets whether taskbar functionality is supported on the current platform.
        /// </summary>
        public bool IsSupported => PlatformGetIsSupported();

        /// <summary>
        /// Gets whether taskbar functionality is currently available.
        /// </summary>
        public bool IsAvailable => PlatformGetIsAvailable();

        /// <summary>
        /// Gets or sets the type of the progress indicator displayed on the taskbar.
        /// </summary>
        public TaskbarProgressState ProgressState
        {
            get => _progressState;
            set
            {
                lock (SyncRoot)
                {
                    if (_progressState != value)
                    {
                        _progressState = value;
                        if (IsAvailable)
                            PlatformSetProgressState(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the progress ratio for the taskbar progress indicator.
        /// </summary>
        public TaskbarProgressValue ProgressValue
        {
            get => _progressValue;
            set
            {
                lock (SyncRoot)
                {
                    if (!_progressValue.Equals(value))
                    {
                        _progressValue = value;
                        if (IsAvailable)
                            PlatformSetProgressValue(value);
                    }
                }
            }
        }

        /// <summary>
        /// Constructs the <see cref="TaskbarList"/>.
        /// </summary>
        public TaskbarList(GameWindow window) : base(window)
        {
            PlatformConstruct();
        }

        /// <inheritdoc/>
        protected override void WindowHandleChanged()
        {
            PlatformWindowHandleChanged();
            Update();
        }

        /// <summary>
        /// Sets the progress and completion total for the taskbar progress indicator.
        /// </summary>
        /// <param name="completed">Indicates the proportion of the operation that has been completed.</param>
        /// <param name="total">Specifies when the operation is complete.</param>
        public void SetProgressValue(long completed, long total)
        {
            ProgressValue = new TaskbarProgressValue(completed, total);
        }

        internal void Update()
        {
            if (!IsAvailable)
                return;

            lock (SyncRoot)
            {
                PlatformSetProgressState(_progressState);
                PlatformSetProgressValue(_progressValue);
            }
        }
    }
}