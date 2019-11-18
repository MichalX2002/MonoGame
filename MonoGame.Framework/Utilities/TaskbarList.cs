
namespace MonoGame.Framework.Utilities
{
    public partial class TaskbarList
    {
        private object SyncRoot { get; } = new object();

        private TaskbarProgressState _progressState;
        private TaskbarProgressValue _progressValue;

        public TaskbarProgressState ProgressState
        {
            get => _progressState;
            set
            {
                if (_progressState != value)
                {
                    _progressState = value;

                    if (IsSupported)
                        PlatformSetProgressState(value);
                }
            }
        }

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
        /// Gets whether taskbar functionality is available on the current platform.
        /// </summary>
        public bool IsSupported { get; private set; }

        internal TaskbarList(GameWindow window)
        {
            PlatformConstruct(window);
        }

        internal void Initialize()
        {
            lock (SyncRoot)
            {
                PlatformInitialize();
                IsSupported = PlatformGetIsSupported();

                if (IsSupported)
                {
                    PlatformSetProgressState(_progressState);
                    PlatformSetProgressValue(_progressValue);
                }
            }
        }
    }
}