// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Globalization;
using MonoGame.Framework.Media;

namespace MonoGame.Framework.Content.Pipeline
{
    /// <summary>
    /// Provides a base class for all video objects.
    /// </summary>
    public class VideoContent : ContentItem, IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Gets the bit rate for this video.
        /// </summary>
        public int BitsPerSecond { get; }

        /// <summary>
        /// Gets the duration of this video.
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// Gets or sets the file name for this video.
        /// </summary>
        [ContentSerializer]
        public string Filename { get; set; }

        /// <summary>
        /// Gets the frame rate for this video.
        /// </summary>
        public float FramesPerSecond { get; }

        /// <summary>
        /// Gets or sets the type of soundtrack accompanying the video.
        /// </summary>
        [ContentSerializer]
        public VideoSoundtrackType VideoSoundtrackType { get; set; }

        /// <summary>
        /// Gets the width of this video.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height of this video.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Initializes a new copy of the VideoContent class for the specified video file.
        /// </summary>
        /// <param name="filename">The file name of the video to import.</param>
        public VideoContent(string filename)
        {
            Filename = filename;
            
            ExternalTool.Run("ffprobe",
                string.Format("-i \"{0}\" -show_format -select_streams v -show_streams -print_format ini", Filename), 
                out var stdout, out _);

            var lines = stdout.ToString().Split(
                Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (!line.Contains('='))
                    continue;

                var key = line.Substring(0, line.IndexOf('='));
                var value = line[(line.IndexOf('=') + 1)..];
                switch (key)
                {
                    case "duration":
                        Duration = TimeSpan.FromSeconds(double.Parse(value, CultureInfo.InvariantCulture));
                        break;

                    case "bit_rate":
                        BitsPerSecond = int.Parse(value, CultureInfo.InvariantCulture);
                        break;

                    case "width":
                        Width = int.Parse(value, CultureInfo.InvariantCulture);
                        break;

                    case "height":
                        Height = int.Parse(value, CultureInfo.InvariantCulture);
                        break;

                    case "r_frame_rate":
                        var frac = value.Split('/');
                        FramesPerSecond = float.Parse(
                            frac[0], CultureInfo.InvariantCulture) / float.Parse(frac[1], CultureInfo.InvariantCulture);
                        break;
                }
            }
        }

        ~VideoContent()
        {
            Dispose(false);
        }

        /// <summary>
        /// Immediately releases the unmanaged resources used by this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: Free managed resources here
                    // ...
                }
                // TODO: Free unmanaged resources here
                // ...
                _disposed = true;
            }
        }
    }
}
