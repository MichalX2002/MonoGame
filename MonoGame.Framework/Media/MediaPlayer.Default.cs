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
            return false;
        }

        private static void PlatformSetIsVisualizationEnabled(bool value)
        {
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
            for (int i = 0; i < Queue.Count; i++)
                Queue.ActiveSong.Stop();
        }

        private static void PlatformGetVisualizationData(VisualizationData data)
        {
        }

        private static bool PlatformGetIsRunningSlowly()
        { 
            // this threshold may need some tweaking
            return PlatformGetStreamingUpdateTime() > 0.1f;
        }

        private static float PlatformGetStreamingUpdateTime()
        {
#if DESKTOPGL || DIRECTX
            float[] timing = OggStreamer.Instance.ThreadTiming;
            float sum = 0;
            for (int i = 0; i < timing.Length; i++)
                sum += timing[i];

            return sum / timing.Length;
#else
            return 0;
#endif
        }
    }
}

