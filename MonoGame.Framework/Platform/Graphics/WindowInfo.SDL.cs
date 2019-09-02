using System;

namespace MonoGame.OpenGL
{
    internal readonly struct WindowInfo : IWindowInfo
    {
        public IntPtr Handle { get; }

        public WindowInfo(IntPtr handle)
        {
            Handle = handle;
        }
    }
}
