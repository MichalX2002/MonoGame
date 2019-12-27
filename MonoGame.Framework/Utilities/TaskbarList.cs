using System;

namespace MonoGame.Framework.Utilities
{
    /// <summary>
    /// Exposes taskbar utilities on desktop platforms.
    /// </summary>
    public partial class TaskbarList
    {
        private object SyncRoot { get; } = new object();

        private IntPtr _windowHandle;
        private TaskbarProgressState _progressState;
        private TaskbarProgressValue _progressValue;

        /// <summary>
        /// Gets whether taskbar functionality is available on the current platform.
        /// </summary>
        public bool IsSupported => PlatformGetIsSupported();

        /// <summary>
        /// Gets or sets the OS window handle assigned to the taskbar.
        /// </summary>
        public IntPtr WindowHandle
        {
            get => _windowHandle;
            set
            {
                if (_windowHandle != value)
                {
                    _windowHandle = value;
                    Update();
                }
            }
        }

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
                        if (IsSupported)
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
                        if (IsSupported)
                            PlatformSetProgressValue(value);
                    }
                }
            }
        }

        /// <summary>
        /// Constructs the <see cref="TaskbarList"/>.
        /// </summary>
        public TaskbarList()
        {
            PlatformConstruct();
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
            if (IsSupported)
            {
                lock (SyncRoot)
                {
                    PlatformSetProgressState(_progressState);
                    PlatformSetProgressValue(_progressValue);
                }
            }
        }
    }
}