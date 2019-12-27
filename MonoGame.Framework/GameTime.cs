// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
using System;

namespace MonoGame.Framework
{
    public class GameTime
    {
        public bool IsRunningSlowly { get; set; }
        public TimeSpan TotalGameTime { get; set; }
        public TimeSpan ElapsedGameTime { get; set; }

        public float ElapsedTotalSeconds => (float)ElapsedGameTime.TotalSeconds;

        public GameTime() : this(TimeSpan.Zero, TimeSpan.Zero)
        {
            IsRunningSlowly = false;
        }

        public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime) :
            this(totalGameTime, elapsedGameTime, false)
        {
        }

		public GameTime (TimeSpan totalRealTime, TimeSpan elapsedRealTime, bool isRunningSlowly)
		{
            TotalGameTime = totalRealTime;
            ElapsedGameTime = elapsedRealTime;
		    IsRunningSlowly = isRunningSlowly;
		}
    }
}

