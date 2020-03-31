// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework;

namespace Microsoft.Devices.Sensors
{
    public readonly struct CompassReading : ISensorReading
    {
        public double HeadingAccuracy { get;  }
        public double MagneticHeading { get;}
        public Vector3 MagnetometerReading { get;  }
        public DateTimeOffset Timestamp { get; }
        public double TrueHeading { get; }

        public CompassReading(
            double headingAccuracy,
            double magneticHeading, 
            Vector3 magnetometerReading,
            DateTimeOffset timestamp, 
            double trueHeading)
        {
            HeadingAccuracy = headingAccuracy;
            MagneticHeading = magneticHeading;
            MagnetometerReading = magnetometerReading;
            Timestamp = timestamp;
            TrueHeading = trueHeading;
        }
    }
}
