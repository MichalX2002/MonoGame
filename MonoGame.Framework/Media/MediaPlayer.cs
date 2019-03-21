// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Media
{
    public static partial class MediaPlayer
    {
        // Need to hold onto this to keep track of how many songs
        // have played when in shuffle mode
        private static int _numSongsInQueuePlayed = 0;
        private static MediaState _state = MediaState.Stopped;
        private static float _volume = 1.0f;
        private static float _pitch = 0.0f;
        private static bool _isVisualizationEnabled;
        private static bool _isMuted;
        private static bool _isRepeating;
        private static bool _isShuffled;

        public static event EventHandler<EventArgs> ActiveSongChanged;
        public static event EventHandler<EventArgs> MediaStateChanged;

        static MediaPlayer()
        {
            PlatformInitialize();
        }

        #region Properties

        public static MediaQueue Queue { get; } = new MediaQueue();

        public static bool IsRunningSlowly => PlatformGetIsRunningSlowly();
        public static float StreamingUpdateTime => PlatformGetStreamingUpdateTime();
        public static bool GameHasControl => PlatformGetGameHasControl();
        public static TimeSpan PlayPosition => PlatformGetPlayPosition();

        public static bool IsVisualizationEnabled
        {
            get => PlatformGetIsVisualizationEnabled();
            set => PlatformSetIsVisualizationEnabled(value);
        }

        public static bool IsMuted
        {
            get => PlatformGetIsMuted();
            set => PlatformSetIsMuted(value);
        }

        public static bool IsRepeating
        {
            get => PlatformGetIsRepeating();
            set => PlatformSetIsRepeating(value);
        }

        public static bool IsShuffled
        {
            get => PlatformGetIsShuffled();
            set => PlatformSetIsShuffled(value);
        }

        public static MediaState State
        {
            get => PlatformGetState();
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    EventHelpers.Raise(null, MediaStateChanged, EventArgs.Empty);
                }
            }
        }

        public static float Volume
        {
            get => PlatformGetVolume();
            set
            {
                var volume = MathHelper.Clamp(value, 0, 1);
                PlatformSetVolume(volume);
            }
        }

        public static float Pitch
        {
            get => PlatformGetPitch();
            set
            {
                var pitch = MathHelper.Clamp(value, -1, 1);
                PlatformSetPitch(pitch);
            }
        }


        #endregion

        public static void Pause()
        {
            if (State != MediaState.Playing || Queue.ActiveSong == null)
                return;

            PlatformPause();

            State = MediaState.Paused;
        }

        /// <summary>
        /// Play clears the current playback queue, and then queues up the specified song for playback. 
        /// Playback starts immediately at the beginning of the song.
        /// </summary>
        public static void Play(Song song)
        {
            Play(song, null);
        }

        /// <summary>
        /// Play clears the current playback queue, and then queues up the specified song for playback. 
        /// Playback starts immediately at the given position of the song.
        /// </summary>
        public static void Play(Song song, TimeSpan? startPosition)
        {
            var previousSong = Queue.Count > 0 ? Queue[0] : null;
            Queue.Clear();
            _numSongsInQueuePlayed = 0;

            Queue.Add(song);
            Queue.ActiveSongIndex = 0;
            PlaySong(song, startPosition);

            if (previousSong != song)
                EventHelpers.Raise(null, ActiveSongChanged, EventArgs.Empty);
        }

        public static void Play(SongCollection collection, int index = 0)
        {
            var previousSong = index < Queue.Count ? Queue[index] : null;
            Queue.Clear();
            _numSongsInQueuePlayed = 0;

            foreach (var song in collection)
                Queue.Add(song);

            Queue.ActiveSongIndex = index;
            PlaySong(Queue.ActiveSong, null);

            if (previousSong != Queue.ActiveSong)
                EventHelpers.Raise(null, ActiveSongChanged, EventArgs.Empty);
        }

        private static void PlaySong(Song song, TimeSpan? startPosition)
        {
            if (song != null && song.IsDisposed)
                throw new ObjectDisposedException(nameof(song));

            PlatformPlaySong(song, startPosition);
            State = MediaState.Playing;
        }

        internal static void OnSongFinishedPlaying(object sender, EventArgs args)
        {
            // TODO: Check args to see if song sucessfully played
            _numSongsInQueuePlayed++;

            if (_numSongsInQueuePlayed >= Queue.Count)
            {
                _numSongsInQueuePlayed = 0;
                if (!IsRepeating)
                {
                    Stop();
                    EventHelpers.Raise(null, ActiveSongChanged, EventArgs.Empty);
                    return;
                }
            }

            MoveNext();
        }

        public static void Resume()
        {
            if (State != MediaState.Paused)
                return;

            PlatformResume();
            State = MediaState.Playing;
        }

        public static void Stop()
        {
            if (State == MediaState.Stopped)
                return;

            PlatformStop();
            State = MediaState.Stopped;
        }

        public static void MoveNext()
        {
            NextSong(1);
        }

        public static void MovePrevious()
        {
            NextSong(-1);
        }

        private static void NextSong(int direction)
        {
            Stop();

            if (IsRepeating && Queue.ActiveSongIndex >= Queue.Count - 1)
            {
                Queue.ActiveSongIndex = 0;

                // Setting direction to 0 will force the first song
                // in the queue to be played.
                // if we're on "shuffle", then it'll pick a random one
                // anyway, regardless of the "direction".
                direction = 0;
            }

            var nextSong = Queue.GetNextSong(direction, IsShuffled);

            if (nextSong != null)
                PlaySong(nextSong, null);

            EventHelpers.Raise(null, ActiveSongChanged, EventArgs.Empty);
        }

        public static void GetVisualizationData(VisualizationData data)
        {
            PlatformGetVisualizationData(data);
        }
    }
}

