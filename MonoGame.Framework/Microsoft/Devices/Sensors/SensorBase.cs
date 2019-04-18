// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework;
using System;

namespace Microsoft.Devices.Sensors
{
	public abstract class SensorBase<TReading> : IDisposable
		where TReading : ISensorReading
	{
#if IOS
        [CLSCompliant(false)]
        protected static readonly CoreMotion.CMMotionManager motionManager = new CoreMotion.CMMotionManager();
#endif
        bool disposed;
		private TimeSpan _timeBetweenUpdates;
	    private TReading currentValue;

		public TReading CurrentValue
        {
            get => currentValue;
            protected set
            {
                currentValue = value;
                CurrentValueChanged?.Invoke(this, currentValue);
            }
        }

        public bool IsDataValid { get; protected set; }
		public TimeSpan TimeBetweenUpdates
        {
            get => _timeBetweenUpdates;
            set
            {
                if (_timeBetweenUpdates != value)
                {
                    _timeBetweenUpdates = value;
                    TimeBetweenUpdatesChanged?.Invoke(this);
                }
            }
        }

        public event EventDelegate<SensorBase<TReading>, TReading> CurrentValueChanged;
		protected event SenderDelegate<SensorBase<TReading>> TimeBetweenUpdatesChanged;
        protected bool IsDisposed { get { return disposed; } }

		public SensorBase()
		{
			this.TimeBetweenUpdates = TimeSpan.FromMilliseconds(2);
        }

        public abstract void Start();

        public abstract void Stop();

        /// <summary>
        /// Derived classes override this method to dispose of managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if unmanaged resources are to be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SensorBase()
        {
            Dispose(false);
        }
    }
}

