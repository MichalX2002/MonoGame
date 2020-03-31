// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework;

namespace Microsoft.Devices.Sensors
{
    public readonly struct AccelerometerReading : ISensorReading
    {
        public Vector3 Acceleration { get; }
        public DateTimeOffset Timestamp { get; }

        public AccelerometerReading(Vector3 acceleration, DateTimeOffset timestamp)
        {
            Acceleration = acceleration;
            Timestamp = timestamp;
        }
    }
}

