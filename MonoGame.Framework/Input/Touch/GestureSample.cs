// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Input.Touch
{
    /// <summary>
    /// Represents data from a multi-touch gesture over a span of time.
    /// </summary>
    public readonly struct GestureSample
    {
        #region Properties

        /// <summary>
        /// Gets the type of the gesture.
        /// </summary>
        public GestureType GestureType { get; }

        /// <summary>
        /// Gets the starting time for this multi-touch gesture sample.
        /// </summary>
        public TimeSpan Timestamp { get; }

        /// <summary>
        /// Gets the position of the first touch-point in the gesture sample.
        /// </summary>
        public Vector2 Position1 { get; }

        /// <summary>
        /// Gets the position of the second touch-point in the gesture sample.
        /// </summary>
        public Vector2 Position2 { get; }

        /// <summary>
        /// Gets the delta information for the first touch-point in the gesture sample.
        /// </summary>
        public Vector2 Delta1 { get; }

        /// <summary>
        /// Gets the delta information for the second touch-point in the gesture sample.
        /// </summary>
        public Vector2 Delta2 { get; }
        #endregion

        /// <summary>
        /// Initializes a new <see cref="GestureSample"/>.
        /// </summary>
        public GestureSample(
            GestureType gestureType,
            TimeSpan timestamp,
            Vector2 position1, Vector2 position2,
            Vector2 delta1, Vector2 delta2)
        {
            GestureType = gestureType;
            Timestamp = timestamp;
            Position1 = position1;
            Position2 = position2;
            Delta1 = delta1;
            Delta2 = delta2;
        }
    }
}

