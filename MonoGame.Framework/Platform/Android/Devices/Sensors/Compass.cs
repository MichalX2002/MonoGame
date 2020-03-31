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
    /// Provides Android applications access to the device’s compass sensor.
    /// </summary>
    public sealed class Compass : SensorBase<CompassReading>
    {
        private const int MaxSensorCount = 10;
        private static SensorManager _sensorManager;
        private static Sensor _sensorMagneticField;
        private static Sensor _sensorAccelerometer;
        private SensorListener _listener;
        private SensorState _state;
        private bool _started = false;
        private static int _instanceCount;

        /// <summary>
        /// Gets whether the device on which the application is running supports the compass sensor.
        /// </summary>
        public static bool IsSupported
        {
            get
            {
                if (_sensorManager == null)
                    Initialize();
                return _sensorMagneticField != null;
            }
        }

        /// <summary>
        /// Gets the current state of the compass. The value is a member of the SensorState enumeration.
        /// </summary>
        public SensorState State
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (_sensorManager == null)
                    Initialize();
                return _state;
            }
        }

        /// <summary>
        /// Creates a new instance of the Compass object.
        /// </summary>
        public Compass()
        {
            if (_instanceCount >= MaxSensorCount)
                throw new SensorFailedException(
                    "The limit of 10 simultaneous instances of the Compass class per application has been exceeded.");

            ++_instanceCount;

            _state = _sensorMagneticField != null ? SensorState.Initializing : SensorState.NotSupported;
            _listener = new SensorListener();
        }

        /// <summary>
        /// Initializes the platform resources required for the compass sensor.
        /// </summary>
        static void Initialize()
        {
            _sensorManager = (SensorManager)AndroidGameActivity.Instance.GetSystemService(Context.SensorService);
            _sensorMagneticField = _sensorManager.GetDefaultSensor(SensorType.MagneticField);
            _sensorAccelerometer = _sensorManager.GetDefaultSensor(SensorType.Accelerometer);
        }

        void ActivityPaused(object sender, EventArgs eventArgs)
        {
            _sensorManager.UnregisterListener(_listener, _sensorMagneticField);
            _sensorManager.UnregisterListener(_listener, _sensorAccelerometer);
        }

        void ActivityResumed(object sender, EventArgs eventArgs)
        {
            _sensorManager.RegisterListener(_listener, _sensorAccelerometer, SensorDelay.Game);
            _sensorManager.RegisterListener(_listener, _sensorMagneticField, SensorDelay.Game);
        }

        /// <summary>
        /// Starts data acquisition from the compass.
        /// </summary>
        public override void Start()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (_sensorManager == null)
                Initialize();
            if (_started == false)
            {
                if (_sensorManager != null && _sensorMagneticField != null && _sensorAccelerometer != null)
                {
                    _listener.compass = this;
                    _sensorManager.RegisterListener(_listener, _sensorMagneticField, SensorDelay.Game);
                    _sensorManager.RegisterListener(_listener, _sensorAccelerometer, SensorDelay.Game);
                }
                else
                {
                    throw new SensorFailedException(
                        "Failed to start compass data acquisition. No default sensor found.");
                }
                _started = true;
                _state = SensorState.Ready;
                return;
            }
            else
            {
                throw new SensorFailedException(
                    "Failed to start compass data acquisition. Data acquisition already started.");
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
                if (_sensorManager != null && _sensorMagneticField != null && _sensorAccelerometer != null)
                {
                    _sensorManager.UnregisterListener(_listener, _sensorAccelerometer);
                    _sensorManager.UnregisterListener(_listener, _sensorMagneticField);
                    _listener.compass = null;
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
                    --_instanceCount;
                    if (_instanceCount == 0)
                    {
                        _sensorAccelerometer = null;
                        _sensorMagneticField = null;
                        _sensorManager = null;
                    }
                }
            }
            base.Dispose(disposing);
        }

        class SensorListener : Java.Lang.Object, ISensorEventListener
        {
            internal Compass compass;
            private float[] valuesAccelerometer;
            private float[] valuesMagenticField;
            private float[] matrixR;
            private float[] matrixI;
            private float[] matrixValues;

            public SensorListener()
            {
                valuesAccelerometer = new float[3];
                valuesMagenticField = new float[3];
                matrixR = new float[9];
                matrixI = new float[9];
                matrixValues = new float[3];
            }

            public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
            {
                //do nothing
            }

            public void OnSensorChanged(SensorEvent e)
            {
                try
                {
                    switch (e.Sensor.Type)
                    {
                        case SensorType.Accelerometer:
                            valuesAccelerometer[0] = e.Values[0];
                            valuesAccelerometer[1] = e.Values[1];
                            valuesAccelerometer[2] = e.Values[2];
                            break;

                        case SensorType.MagneticField:
                            valuesMagenticField[0] = e.Values[0];
                            valuesMagenticField[1] = e.Values[1];
                            valuesMagenticField[2] = e.Values[2];
                            break;
                    }

                    compass.IsDataValid = SensorManager.GetRotationMatrix(
                        matrixR, matrixI, valuesAccelerometer, valuesMagenticField);

                    if (compass.IsDataValid)
                    {
                        SensorManager.GetOrientation(matrixR, matrixValues);

                        // We need the magnetic declination from true north to calculate the true heading from the magnetic heading.
                        // On Android, this is available through Android.Hardware.GeomagneticField, but this requires your geo position.

                        var magneticHeading = matrixValues[0];
                        var magnetometerReading = new Vector3(
                                valuesMagenticField[0], valuesMagenticField[1], valuesMagenticField[2]);

                        compass.CurrentValue = new CompassReading(
                            headingAccuracy: 0,
                            magneticHeading,
                            magnetometerReading,
                            timestamp: DateTime.UtcNow,
                            trueHeading: magneticHeading);
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

