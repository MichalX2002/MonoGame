using System;

namespace MonoGame.Framework
{
    public readonly struct FrameTime
    {
        private const float SecondsPerTick = 1f / TimeSpan.TicksPerSecond;

        public TimeSpan Total { get; }
        public TimeSpan Elapsed { get; }
        public bool IsRunningSlowly { get; }

        public float ElapsedTotalSeconds => Elapsed.Ticks * SecondsPerTick;

        public FrameTime(TimeSpan total, TimeSpan elapsed, bool isRunningSlowly)
        {
            Total = total;
            Elapsed = elapsed;
            IsRunningSlowly = isRunningSlowly;
        }

        public FrameTime(TimeSpan total, TimeSpan elapsed) :
            this(total, elapsed, isRunningSlowly: false)
        {
        }
    }
}

