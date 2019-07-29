// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Xna.Framework.Media
{
    public sealed partial class Song : IDisposable
    {
        private static HashSet<Song> _playingSongs = new HashSet<Song>();

        private readonly object _playMutex = new object();
        private Android.Media.MediaPlayer _androidPlayer;
        private string _fileName;
        private bool _looping;
        private bool _paused;
        private float _volume = 1f;
        private TimeSpan _duration;

        private void PlatformInitialize(string fileName)
        {
            _fileName = fileName;
            _androidPlayer = new Android.Media.MediaPlayer();
            _androidPlayer.Completion += OnPlayerCompletion;
        }

        private void OnPlayerCompletion(object sender, EventArgs e)
        {
            OnFinish?.Invoke(this);
        }

        private static void PlatformMasterVolumeChanged()
        {
            lock (_playingSongs)
            {
                foreach (var song in _playingSongs)
                    song.SetPlayerVolume(song._volume);
            }
        }

        private void PlatformPlay(TimeSpan? startPosition)
        {
            lock (_playMutex)
            {
                if (_paused)
                {
                    PlayInternal(startPosition);
                    return;
                }
            }

            var fd = Game.Activity.Assets.OpenFd(_fileName);
            try
            {
                if (fd == null)
                    throw new FileNotFoundException("Could not open file.", _fileName);
                _androidPlayer.Reset();
                _androidPlayer.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
                _duration = TimeSpan.FromMilliseconds(_androidPlayer.Duration);
            }
            finally
            {
                if (fd != null)
                    fd.Dispose();
            }

            lock (_playMutex)
                PlayInternal(startPosition);
        }

        private void PlayInternal(TimeSpan? position)
        {
            if (_androidPlayer == null)
                throw new ObjectDisposedException(nameof(Song));

            _androidPlayer.Looping = _looping;
            SetPlayerVolume(_volume);

            if (position.HasValue)
                PlatformSetPosition(position.Value);

            _androidPlayer.Prepare();

            lock (_playMutex)
            {
                _androidPlayer.Start();
                _paused = false;

                lock (_playingSongs)
                    _playingSongs.Add(this);
            }
        }

        private void PlatformResume()
        {
            lock (_playMutex)
            {
                if (_androidPlayer == null)
                    throw new ObjectDisposedException(nameof(Song));

                if (_paused)
                {
                    SetPlayerVolume(_volume);
                    _androidPlayer.Start();
                    _paused = false;

                    lock (_playingSongs)
                        _playingSongs.Add(this);
                }
            }
        }

        private void PlatformPause()
        {
            lock (_playMutex)
            {
                if (_androidPlayer == null)
                    throw new ObjectDisposedException(nameof(Song));

                if (_androidPlayer.IsPlaying)
                {
                    _androidPlayer.Pause();
                    _paused = true;

                    lock (_playingSongs)
                        _playingSongs.Remove(this);
                }
            }
        }

        private void PlatformStop()
        {
            lock (_playMutex)
            {
                if (_androidPlayer == null)
                    throw new ObjectDisposedException(nameof(Song));

                _androidPlayer.Stop();
                _paused = false;

                lock (_playingSongs)
                    _playingSongs.Remove(this);
            }
        }

        private float PlatformGetVolume()
        {
            return _volume;
        }

        private void PlatformSetVolume(float value)
        {
            SetPlayerVolume(value);
            _volume = value;
        }

        private void SetPlayerVolume(float value)
        {
            var player = _androidPlayer;
            if (player != null)
                player.SetVolume(value * _masterVolume, value * _masterVolume);
        }

        private float PlatformGetPitch()
        {
            throw new NotSupportedException();
        }

        private void PlatformSetPitch(float value)
        {
            throw new NotSupportedException();
        }

        private bool PlatformGetLooping()
        {
            return _looping;
        }

        private void PlatformSetLooping(bool value)
        {
            if (_androidPlayer == null)
                throw new ObjectDisposedException(nameof(Song));

            _looping = value;
            _androidPlayer.Looping = value;
        }

        private MediaState PlatformGetState()
        {
            if (_androidPlayer != null)
            {
                if (_androidPlayer.IsPlaying)
                    return MediaState.Playing;
                if (_paused)
                    return MediaState.Paused;
            }
            return MediaState.Stopped;
        }

        public TimeSpan PlatformGetPosition()
        {
            return TimeSpan.FromMilliseconds(_androidPlayer.CurrentPosition);
        }

        public void PlatformSetPosition(TimeSpan value)
        {
            _androidPlayer.SeekTo((int)value.TotalMilliseconds);
        }

        private TimeSpan PlatformGetDuration()
        {
            return _duration;
        }

        private void PlatformDispose(bool disposing)
        {
            lock (_playMutex)
            {
                lock(_playingSongs)
                    _playingSongs.Remove(this);

                if (_androidPlayer != null)
                {
                    _androidPlayer.Dispose();
                    _androidPlayer = null;
                }
            }
        }
    }
}

