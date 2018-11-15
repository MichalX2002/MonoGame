// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Audio;
using MonoGame.OpenAL;
using System;
using System.Threading;

#if IOS
using AudioToolbox;
using AVFoundation;
#endif

namespace Microsoft.Xna.Framework.Media
{
    public static partial class MediaPlayer
    {
        #region Properties
        
        private static void PlatformInitialize()
        {

        }

        private static bool PlatformGetIsMuted()
        {
            return _isMuted;
        }

        private static void PlatformSetIsMuted(bool muted)
        {
            _isMuted = muted;

            if (Queue.Count == 0)
                return;

            var newVolume = _isMuted ? 0.0f : _volume;
            Queue.SetVolume(newVolume);
        }

        private static bool PlatformGetIsRepeating()
        {
            return _isRepeating;
        }

        private static void PlatformSetIsRepeating(bool repeating)
        {
            _isRepeating = repeating;
        }

        private static bool PlatformGetIsShuffled()
        {
            return _isShuffled;
        }

        private static void PlatformSetIsShuffled(bool shuffled)
        {
            _isShuffled = shuffled;
        }

        private static TimeSpan PlatformGetPlayPosition()
        {
            var activeSong = Queue.ActiveSong;
            if (activeSong == null)
                return TimeSpan.Zero;

            return activeSong.Position;
        }

#if (IOS && !TVOS) || ANDROID
        private static void PlatformSetPlayPosition(TimeSpan playPosition)
        {
            var activeSong = Queue.ActiveSong;
            if (activeSong != null)
                activeSong.Position = playPosition;
        }
#endif

        private static MediaState PlatformGetState()
        {
            return _state;
        }

        private static float PlatformGetVolume()
        {
            return _volume;
        }

        private static void PlatformSetVolume(float volume)
        {
            _volume = volume;

            if (Queue.ActiveSong == null)
                return;

            Queue.SetVolume(_isMuted ? 0.0f : _volume);
        }
        
        private static float PlatformGetPitch()
        {
            return _pitch;
        }

        private static void PlatformSetPitch(float pitch)
        {
            _pitch = pitch;

            if (Queue.ActiveSong == null)
                return;

            Queue.SetPitch(_pitch);
        }

        private static bool PlatformGetGameHasControl()
        {
#if IOS
            return !AVAudioSession.SharedInstance().OtherAudioPlaying;
#else
            // TODO: Fix me!
            return true;
#endif
        }
		#endregion

        private static bool PlatformGetIsVisualizationEnabled()
        {
            return _isVisualizationEnabled;
        }

        private static void PlatformSetIsVisualizationEnabled(bool value)
        {
#if DESKTOPGL
            _isVisualizationEnabled = value;
#else
            _isVisualizationEnabled = false;
#endif
        }

        private static void PlatformPause()
        {
            var activeSong = Queue.ActiveSong;
            if (activeSong == null)
                return;

            activeSong.Pause();
        }

        private static void PlatformPlaySong(Song song, TimeSpan? startPosition)
        {
            if (Queue.ActiveSong == null)
                return;

            song.SetEventHandler(OnSongFinishedPlaying);

            song.Volume = _isMuted ? 0.0f : _volume;
            song.Pitch = _pitch;
            song.Play(startPosition);
        }

        private static void PlatformResume()
        {
            var activeSong = Queue.ActiveSong;
            if (activeSong == null)
                return;

            activeSong.Resume();
        }

        private static void PlatformStop()
        {
            // Loop through so that we reset the PlayCount as well
            foreach (var song in Queue.Songs)
                Queue.ActiveSong.Stop();
        }

        private static void PlatformGetVisualizationData(VisualizationData data)
        {
#if DESKTOPGL
            var activeSong = Queue.ActiveSong;
            if (activeSong == null)
                return;

            var stream = activeSong.stream;
            if (stream == null)
                return;

            int start;
            SongPart part;
            lock (stream.prepareMutex)
            {
                AL.GetSource(stream.alSourceId, ALGetSourcei.SampleOffset, out start);
                start *= stream.Reader.Channels;
                part = stream.parts[0];
            }

            int vi = 0;
            int partIndex = 0;
            for (int di = start; vi < data._samples.Length; vi++, di++)
            {
                if (di < part.Count)
                {
                    data._samples[vi] = part.Data[di];
                    continue;
                }

                if (Monitor.TryEnter(stream.prepareMutex, 5))
                {
                    //look at next part
                    partIndex++;
                    if (partIndex < stream.parts.Count)
                    {
                        part = stream.parts[partIndex];
                        if (part.Count == 0)
                        {
                            Monitor.Exit(stream.prepareMutex);
                            break;
                        }

                        di = 0;
                    }
                    else // no more parts ready
                    {
                        Monitor.Exit(stream.prepareMutex);
                        break;
                    }

                    Monitor.Exit(stream.prepareMutex);
                }
                else
                    return;
            }

            for (int i = vi; i < data._samples.Length; i++)
                data._samples[i] = 0;
#endif
        }

        private static bool PlatformGetIsRunningSlowly()
        {
            return PlatformGetUpdateTime() > 0.125f;
        }

        private static float PlatformGetUpdateTime()
        {
#if DESKTOPGL
            float[] timing = OggStreamer.Instance.ThreadTiming;
            float sum = 0;
            for (int i = 0; i < timing.Length; i++)
                sum += timing[i];

            return sum / timing.Length; // this threshold may need some tweaking
#else
            return 0;
#endif
        }
    }
}

