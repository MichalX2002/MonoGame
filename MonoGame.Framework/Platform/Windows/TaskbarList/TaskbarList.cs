using System;

namespace MonoGame.Framework.Utilities
{
    internal partial class TaskbarList
    {
        private ITaskbarList _comObject;
        private IntPtr _windowHandle;

        public TaskbarList(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(windowHandle));

            _windowHandle = windowHandle;

            _comObject = (ITaskbarList)new COMTaskbarList();
            _comObject.HrInit();
        }

        public void SetProgressState(TaskbarProgressState state)
        {
            _comObject.SetProgressState(_windowHandle, state);
        }

        public void SetProgressValue(TaskbarProgressValue value)
        {
            _comObject.SetProgressValue(_windowHandle, (ulong)value.Completed, (ulong)value.Total);
        }
    }
}
