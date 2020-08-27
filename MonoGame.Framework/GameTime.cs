// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
using System;

namespace MonoGame.Framework
{
    public class GameTime
    {
        public bool IsRunningSlowly { get; set; }
        public TimeSpan Total { get; set; }
        public TimeSpan Elapsed { get; set; }

        public float ElapsedTotalSeconds => (float)Elapsed.TotalSeconds;

        public GameTime(TimeSpan totalRealTime, TimeSpan elapsedRealTime, bool isRunningSlowly)
        {
            Total = totalRealTime;
            Elapsed = elapsedRealTime;
            IsRunningSlowly = isRunningSlowly;
        }

        public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime) :
            this(totalGameTime, elapsedGameTime, isRunningSlowly: false)
        {
        }

        public GameTime() : this(TimeSpan.Zero, TimeSpan.Zero)
        {
        }
    }
}

