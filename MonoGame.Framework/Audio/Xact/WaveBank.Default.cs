// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Audio
{
    partial class WaveBank
    {
        // TODO: use some kind of DynamicSoundEffectInstance helper?

        private SoundEffectInstance PlatformCreateStream(StreamInfo stream)
        {
            throw new NotImplementedException("XACT streaming is not implemented on this platform.");
        }
    }
}

