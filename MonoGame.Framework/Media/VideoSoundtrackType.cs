// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Media
{
    /// <summary>
    /// Type of sounds in a video.
    /// </summary>
    [Flags]
    public enum VideoSoundtrackType
    {
        /// <summary>
        /// This video does not contain any soundtrack.
        /// </summary>
        None = 0,

        /// <summary>
        /// This video contains only music.
        /// </summary>
        Music = 1 << 0,

        /// <summary>
        /// This video contains only dialog.
        /// </summary>
        Dialog = 1 << 1,

        /// <summary>
        /// This video contains both music and dialog.
        /// </summary>
        MusicAndDialog = Music | Dialog,
    }
}
