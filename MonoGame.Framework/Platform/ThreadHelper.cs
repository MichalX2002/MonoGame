using System.Runtime.CompilerServices;
using System.Threading;

namespace MonoGame.Framework.Utilities
{
    public abstract class ThreadHelper
    {
        public static ThreadHelper Instance { get; } = Create();

        public abstract void Sleep(object mutex, int sleepMillis);

        private static ThreadHelper Create()
        {
            switch (PlatformInfo.CurrentOS)
            {
                case PlatformInfo.OS.Windows:
                    if (PlatformInfo.Platform == MonoGamePlatform.WindowsUniversal)
                        return new WindowsUniversalThreadHelper();
                    goto default;

                default:
                    return new DefaultThreadHelper();
            }
        }
    }

    internal sealed class DefaultThreadHelper : ThreadHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Sleep(object mutex, int sleepMillis)
        {
            Thread.Sleep(sleepMillis);
        }
    }

    internal sealed class WindowsUniversalThreadHelper : ThreadHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Sleep(object mutex, int sleepMillis)
        {
            lock (mutex)
                Monitor.Wait(mutex, sleepMillis);
        }
    }
}
