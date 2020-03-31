// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework;

namespace Microsoft.Devices.Sensors
{
    public abstract class SensorBase<TReading> : IDisposable
        where TReading : ISensorReading
    {
#if IOS
        [CLSCompliant(false)]
        protected static readonly CoreMotion.CMMotionManager motionManager = new CoreMotion.CMMotionManager();
#endif
        private TimeSpan _timeBetweenUpdates;
        private TReading _currentValue;

        public TReading CurrentValue
        {
            get => _currentValue;
            protected set
            {
                _currentValue = value;
                CurrentValueChanged?.Invoke(this, _currentValue);
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

        public event DataEvent<SensorBase<TReading>, TReading> CurrentValueChanged;
        protected event DatalessEvent<SensorBase<TReading>> TimeBetweenUpdatesChanged;

        protected bool IsDisposed { get; private set; }

        public SensorBase()
        {
            TimeBetweenUpdates = TimeSpan.FromMilliseconds(2);
        }

        public abstract void Start();

        public abstract void Stop();

        /// <summary>
        /// Derived classes override this method to dispose of managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if unmanaged resources are to be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            IsDisposed = true;
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

