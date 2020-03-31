// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace MonoGame.Framework.Utilities
{
    internal static class TimerHelper
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtQueryTimerResolution(
            out uint MinimumResolution, out uint MaximumResolution, out uint CurrentResolution);

        public static TimeSpan MaxResolution { get; private set; }
        public static TimeSpan Resolution { get; private set; }

        /// <summary>
        /// Returns the current timer resolution in milliseconds.
        /// </summary>
        public static void UpdateResolution()
        {
            NtQueryTimerResolution(out _, out uint max, out uint current);
            MaxResolution = TimeSpan.FromTicks( 10000 + max);
            Resolution = TimeSpan.FromTicks(current);
        }

        /// <summary>
        /// Sleeps as long as possible without exceeding the specified period.
        /// </summary>
        public static void SleepForNoMoreThan(TimeSpan time)
        {
            // Assumption is that Thread.Sleep(t) will sleep for at least (t), and at most (t + timerResolution)
            if (time < MaxResolution)
                return;

            var sleepTime = time - Resolution;
            if (sleepTime.TotalMilliseconds > 0)
                Thread.Sleep(sleepTime);
        }
    }
}
