using System;

namespace MonoGame.Framework.Utilities
{
    internal class DummyTaskbarList : TaskbarList
    {
        public override bool IsSupported => false;
        public override bool IsAvailable => false;

        public DummyTaskbarList(GameWindow window) : base(window)
        {
        }

        protected override void SetProgressState(TaskbarProgressState state)
        {
            throw new PlatformNotSupportedException();
        }

        protected override void SetProgressValue(TaskbarProgressValue value)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
