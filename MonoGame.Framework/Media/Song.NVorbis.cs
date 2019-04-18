// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Audio;

namespace Microsoft.Xna.Framework.Media
{
    public sealed partial class Song : IDisposable
    {
        internal OggStream stream;
        private float _volume = 1f;
        private float _pitch = 1f;
        private TimeSpan _duration;

        private void PlatformInitialize(string fileName)
        {
            // init OpenAL if need be
            ALController.EnsureInitialized();

            stream = new OggStream(fileName, OnFinishedPlaying);
            stream.Prepare();
            _duration = stream.GetLength();
        }

        private void OnFinishedPlaying()
        {
            OnFinish?.Invoke();
        }

        private void PlatformDispose(bool disposing)
        {
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }

        private void PlatformPlay(TimeSpan? startPosition)
        {
            if (stream != null)
            {
                if (startPosition.HasValue)
                    PlatformSetPosition(startPosition.Value);

                stream.Play();
                _playCount++;
            }
        }

        private void PlatformResume()
        {
            if (stream != null)
                stream.Resume();
        }

        private void PlatformPause()
        {
            if (stream != null)
                stream.Pause();
        }

        private void PlatformStop()
        {
            if (stream != null)
                stream.Stop();
        }

        private TimeSpan PlatformGetPosition()
        {
            return stream != null ? stream.GetPosition() : TimeSpan.Zero;
        }

        private MediaState PlatformGetState()
        {
            switch (stream.GetState())
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
            if (stream != null)
            {
                var initialState = stream.GetState();
                
                stream.SeekToPosition(time);

                if (initialState == ALSourceState.Playing)
                    stream.Play();
            }
        }

        private float PlatformGetVolume()
        {
            return stream != null ? _volume : 0;
        }

        private void PlatformSetVolume(float value)
        {
            _volume = value;
            if (stream != null)
                stream.Volume = _volume * _masterVolume;
        }

        private float PlatformGetPitch()
        {
            return stream != null ? _pitch : 0;
        }

        private void PlatformSetPitch(float value)
        {
            if (_pitch != value)
            {
                _pitch = value;
                if (stream != null)
                    stream.Pitch = _pitch;
            }
        }

        private TimeSpan PlatformGetDuration()
        {
            return _duration;
        }

        private int PlatformGetPlayCount()
        {
            return _playCount;
        }
    }
}

