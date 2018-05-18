// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework
{
    public class GameTime
    {
        private float _delta;
        private TimeSpan _elapsedGameTime;

        public bool IsRunningSlowly { get; set; }
        public float Delta => _delta;
        public TimeSpan TotalGameTime { get; set; }
        public TimeSpan ElapsedGameTime
        {
            get => _elapsedGameTime;
            set { _elapsedGameTime = value; _delta = (float)value.TotalSeconds; }
        }

        public GameTime()
        {
            TotalGameTime = TimeSpan.Zero;
            _elapsedGameTime = TimeSpan.Zero;
            IsRunningSlowly = false;
        }

        public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime)
        {
            TotalGameTime = totalGameTime;
            _elapsedGameTime = elapsedGameTime;
            IsRunningSlowly = false;
        }

		public GameTime (TimeSpan totalRealTime, TimeSpan elapsedRealTime, bool isRunningSlowly)
		{
            TotalGameTime = totalRealTime;
            _elapsedGameTime = elapsedRealTime;
		    IsRunningSlowly = isRunningSlowly;
		}
    }
}

