// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Content.Pipeline.Processors
{
    /// <summary>
    /// Represents a processed Song object.
    /// </summary>
    [CompressedContent]
    public sealed class SongContent
    {
        public string FileName { get; }
        public TimeSpan Duration { get; }

        /// <summary>
        /// Creates a new instance of the SongContent class
        /// </summary>
        /// <param name="fileName">Filename of the song</param>
        /// <param name="duration">Duration of the song</param>
        public SongContent(string fileName, TimeSpan duration)
        {
            FileName = fileName;
            Duration = duration;
        }
    }
}
