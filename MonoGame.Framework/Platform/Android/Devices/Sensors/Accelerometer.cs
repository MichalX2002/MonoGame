// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Android.Content;
using Android.Hardware;
using MonoGame.Framework;

namespace Microsoft.Devices.Sensors
{
    /// <summary>
    /// Provides Android applications access to the device’s accelerometer sensor.
    /// </summary>
    public sealed class Accelerometer : SensorBase<AccelerometerReading>
    {
        private const int MaxSensorCount = 10;

        private static SensorManager _sensorManager;
        private static Sensor _sensor;
        private SensorListener _listener;
        private SensorState _state;
        private bool _started = false;
        private static int _instanceCount;

        /// <summary>
        /// Gets whether the device on which the application is running supports the accelerometer sensor.
        /// </summary>
        public static bool IsSupported
        {
            get
            {
                if (_sensorManager == null)
                    Initialize();
                return _sensor != null;
            }
        }

        /// <summary>
        /// Gets the current state of the accelerometer. 
        /// </summary>
        public SensorState State
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (_sensorManager == null)
                {
                    Initialize();
                    _state = _sensor != null ? SensorState.Initializing : SensorState.NotSupported;
                }
                return _state;
            }
        }

        /// <summary>
        /// Creates a new instance of the Accelerometer object.
        /// </summary>
        public Accelerometer()
        {
            if (_instanceCount >= MaxSensorCount)
                throw new SensorFailedException(
                    "The limit of 10 simultaneous instances of the Accelerometer class per application has been exceeded.");

            _instanceCount++;

            _state = _sensor != null ? SensorState.Initializing : SensorState.NotSupported;
            _listener = new SensorListener();
        }

        /// <summary>
        /// Initializes the platform resources required for the accelerometer sensor.
        /// </summary>
        static void Initialize()
        {
            _sensorManager = (SensorManager)AndroidGameActivity.Instance.GetSystemService(Context.SensorService);
            _sensor = _sensorManager.GetDefaultSensor(SensorType.Accelerometer);
        }

        void ActivityPaused(AndroidGameActivity activity)
        {
            _sensorManager.UnregisterListener(_listener, _sensor);
        }

        void ActivityResumed(AndroidGameActivity activity)
        {
            _sensorManager.RegisterListener(_listener, _sensor, SensorDelay.Game);
        }

        /// <summary>
        /// Starts data acquisition from the accelerometer.
        /// </summary>
        public override void Start()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (_sensorManager == null)
                Initialize();

            if (_started == false)
            {
                if (_sensorManager != null && _sensor != null)
                {
                    _listener.accelerometer = this;
                    _sensorManager.RegisterListener(_listener, _sensor, SensorDelay.Game);

                    // So the system can pause and resume the sensor when the activity is paused
                    AndroidGameActivity.Instance.Paused += ActivityPaused;
                    AndroidGameActivity.Instance.Resumed += ActivityResumed;
                }
                else
                {
                    throw new AccelerometerFailedException(
                        "Failed to start accelerometer data acquisition. No default sensor found.", -1);
                }
                _started = true;
                _state = SensorState.Ready;
                return;
            }
            else
            {
                throw new AccelerometerFailedException(
                    "Failed to start accelerometer data acquisition. Data acquisition already started.", -1);
            }
        }

        /// <summary>
        /// Stops data acquisition from the accelerometer.
        /// </summary>
        public override void Stop()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (_started)
            {
                if (_sensorManager != null && _sensor != null)
                {
                    AndroidGameActivity.Instance.Paused -= ActivityPaused;
                    AndroidGameActivity.Instance.Resumed -= ActivityResumed;
                    _sensorManager.UnregisterListener(_listener, _sensor);
                    _listener.accelerometer = null;
                }
            }
            _started = false;
            _state = SensorState.Disabled;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (_started)
                        Stop();

                    _instanceCount--;
                    if (_instanceCount == 0)
                    {
                        _sensor = null;
                        _sensorManager = null;
                    }
                }
            }
            base.Dispose(disposing);
        }

        class SensorListener : Java.Lang.Object, ISensorEventListener
        {
            internal Accelerometer accelerometer;

            public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
            {
                //do nothing
            }

            public void OnSensorChanged(SensorEvent e)
            {
                try
                {
                    if (e != null && e.Sensor.Type == SensorType.Accelerometer && accelerometer != null)
                    {
                        var values = e.Values;
                        try
                        {
                            var reading = default(AccelerometerReading);
                            accelerometer.IsDataValid = (values != null && values.Count == 3);
                            if (accelerometer.IsDataValid)
                            {
                                const float gravity = SensorManager.GravityEarth;
                                reading = new AccelerometerReading(
                                    acceleration: new Vector3(values[0], values[1], values[2]) / gravity,
                                    timestamp: DateTime.UtcNow);
                            }
                            accelerometer.CurrentValue = reading;
                        }
                        finally
                        {
                            (values as IDisposable)?.Dispose();
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    //Occassionally an NullReferenceException is thrown when accessing e.Values??
                    // mono    : Unhandled Exception: System.NullReferenceException: Object reference not set to an instance of an object
                    // mono    :   at Android.Runtime.JNIEnv.GetObjectField (IntPtr jobject, IntPtr jfieldID) [0x00000] in <filename unknown>:0 
                    // mono    :   at Android.Hardware.SensorEvent.get_Values () [0x00000] in <filename unknown>:0
                }
            }
        }
    }
}

