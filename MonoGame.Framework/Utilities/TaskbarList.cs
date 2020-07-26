
using System;

namespace MonoGame.Framework.Utilities
{
    /// <summary>
    /// Exposes taskbar utilities on desktop platforms.
    /// </summary>
    public abstract partial class TaskbarList : GameWindowDependency
    {
        private object SyncRoot { get; } = new object();

        private TaskbarProgressState _progressState;
        private TaskbarProgressValue _progressValue;

        /// <summary>
        /// Gets whether taskbar functionality is supported on the current platform.
        /// </summary>
        public abstract bool IsSupported { get; }

        /// <summary>
        /// Gets whether taskbar functionality is currently available.
        /// </summary>
        public abstract bool IsAvailable { get; }

        /// <summary>
        /// Gets or sets the type of the progress indicator displayed on the taskbar.
        /// </summary>
        public TaskbarProgressState ProgressState
        {
            get => _progressState;
            set
            {
                AssertIsSupported();
                lock (SyncRoot)
                {
                    if (_progressState != value)
                    {
                        _progressState = value;
                        if (IsAvailable)
                            SetProgressState(value);
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
                AssertIsSupported();
                lock (SyncRoot)
                {
                    if (!_progressValue.Equals(value))
                    {
                        _progressValue = value;
                        if (IsAvailable)
                            SetProgressValue(value);
                    }
                }
            }
        }

        /// <summary>
        /// Constructs the <see cref="TaskbarList"/>.
        /// </summary>
        public TaskbarList(GameWindow window) : base(window)
        {
        }

        public static TaskbarList Create(GameWindow window)
        {
            return PlatformCreate(window);
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
                SetProgressState(_progressState);
                SetProgressValue(_progressValue);
            }
        }

        protected void AssertIsSupported()
        {
            if (!IsSupported)
                throw new PlatformNotSupportedException();
        }

        protected abstract void SetProgressState(TaskbarProgressState state);

        protected abstract void SetProgressValue(TaskbarProgressValue value);

        /// <inheritdoc/>
        protected override void WindowHandleChanged()
        {
            Update();
        }
    }
}