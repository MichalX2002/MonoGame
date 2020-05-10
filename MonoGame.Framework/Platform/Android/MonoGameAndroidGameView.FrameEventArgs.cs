// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework
{
    public partial class MonoGameAndroidGameView
    {
        public class FrameEventArgs
        {
            private double _elapsed;

            /// <summary>
            /// Gets a <see cref="double"/> that indicates how many seconds of time elapsed since the previous event.
            /// </summary>
            public double Elapsed
            {
                get => _elapsed;
                set
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException();
                    _elapsed = value;
                }
            }

            /// <summary>
            /// Constructs a new <see cref="FrameEventArgs"/> instance.
            /// </summary>
            public FrameEventArgs()
            {
            }

            /// <summary>
            /// Constructs a new <see cref="FrameEventArgs"/> instance.
            /// </summary>
            /// <param name="elapsed">The amount of time that has elapsed since the previous event, in seconds.</param>
            public FrameEventArgs(double elapsed)
            {
                Elapsed = elapsed;
            }
        }
    }
}