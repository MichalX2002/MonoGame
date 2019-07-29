// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using MonoGame.Framework.Audio;

namespace MonoGame.Framework.Media
{
    public sealed partial class Song : IDisposable
    {
        internal OggStream _stream;
        private float _volume;
        private TimeSpan _duration;
        private bool _isAtEnd;

        private void PlatformInitialize(Stream stream, bool leaveOpen)
        {
            // init OpenAL if need be
            ALController.EnsureInitialized();

            _stream = new OggStream(this, stream, leaveOpen, OnFinishedPlaying);
            _stream.Prepare(immediate: true);
            _duration = _stream.GetLength();
        }

        private void OnFinishedPlaying()
        {
            OnFinish?.Invoke(this);
            _isAtEnd = true;
        }

        private static void PlatformMasterVolumeChanged()
        {
            var streamer = OggStreamer.Instance;
            lock (streamer._iterationMutex)
            {
                foreach (var stream in streamer._streams)
                    stream.Volume = stream.Parent._volume * _masterVolume;
            }
        }

        private void PlatformPlay(bool immediate, TimeSpan? startPosition)
        {
            if (_stream != null)
            {
                if (_isAtEnd && !startPosition.HasValue)
                    startPosition = TimeSpan.Zero;
                _isAtEnd = false;

                if (startPosition.HasValue)
                    PlatformSetPosition(startPosition.Value);

                _stream.Play(immediate);
            }
        }

        private bool PlatformGetLooped()
        {
            return _stream != null ? _stream.IsLooped : false;
        }

        private void PlatformSetLooped(bool value)
        {
            if (_stream != null)
                _stream.IsLooped = value;
        }

        private void PlatformResume()
        {
            if (_stream != null)
                _stream.Resume();
        }

        private void PlatformPause()
        {
            if (_stream != null)
                _stream.Pause();
        }

        private void PlatformStop()
        {
            if (_stream != null)
                _stream.Stop();
        }

        private TimeSpan PlatformGetPosition()
        {
            return _stream != null ? _stream.GetPosition() : TimeSpan.Zero;
        }

        private MediaState PlatformGetState()
        {
            switch (_stream.GetState())
            {
                case ALSourceState.Paused:
                    return MediaState.Paused;

                case ALSourceState.Playing:
                    return MediaState.Playing;

                default:
                    return MediaState.Stopped;
            }
        }

        private void PlatformSetPosition(TimeSpan time)
        {
            if (_stream != null)
            {
                var initialState = _stream.GetState();

                _stream.SeekToPosition(time);

                if (initialState == ALSourceState.Playing)
                    _stream.Play(immediate: true);
            }
        }

        private float PlatformGetVolume()
        {
            return _stream != null ? _volume : 0;
        }

        private void PlatformSetVolume(float value)
        {
            _volume = value;
            if (_stream != null)
                _stream.Volume = _volume * _masterVolume;
        }

        private float PlatformGetPitch()
        {
            if (_stream != null)
                return _stream.Pitch;
            return 0;
        }

        private void PlatformSetPitch(float value)
        {
            if (_stream != null)
                _stream.Pitch = value;
        }

        private TimeSpan PlatformGetDuration()
        {
            return _duration;
        }

        private void PlatformDispose(bool disposing)
        {
            if (disposing)
            {
                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }
            }
        }
    }
}

