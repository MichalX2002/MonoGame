// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;

namespace Microsoft.Xna.Framework.Media
{
    public sealed partial class Song : IEquatable<Song>, IDisposable
    {
        static Android.Media.MediaPlayer _androidPlayer;
        static Song _playingSong;

        private Album album;
        private Artist artist;
        private Genre genre;
        private string name;
        private TimeSpan duration;
        private TimeSpan position;

        [CLSCompliant(false)]
        public Android.Net.Uri AssetUri { get; }

        static Song()
        {
            _androidPlayer = new Android.Media.MediaPlayer();
            _androidPlayer.Completion += AndroidPlayer_Completion;
        }

        internal Song(Album album, Artist artist, Genre genre, string name, TimeSpan duration, Android.Net.Uri assetUri)
        {
            this.album = album;
            this.artist = artist;
            this.genre = genre;
            this.name = name;
            this.duration = duration;
            AssetUri = assetUri;
        }

        private void PlatformInitialize(string fileName)
        {
            // Nothing to do here
        }

        static void AndroidPlayer_Completion(object sender, EventArgs e)
        {
            var playingSong = _playingSong;
            _playingSong = null;

            if (playingSong != null && playingSong.DonePlaying != null)
                playingSong.DonePlaying(sender, e);
        }

        /// <summary>
        /// Set the event handler for "Finished Playing". Done this way to prevent multiple bindings.
        /// </summary>
        internal void SetEventHandler(FinishedPlayingHandler handler)
        {
            if (DonePlaying != null)
                return;
            DonePlaying += handler;
        }

        private void PlatformDispose(bool disposing)
        {
            // Appears to be a noOp on Android
        }

        internal void Play(TimeSpan? startPosition)
        {
            // Prepare the player
            _androidPlayer.Reset();

            if (AssetUri != null)
            {
                _androidPlayer.SetDataSource(MediaLibrary.Context, AssetUri);
            }
            else
            {
                var afd = Game.Activity.Assets.OpenFd(Name);
                if (afd == null)
                    return;

                _androidPlayer.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
            }


            _androidPlayer.Prepare();
            _androidPlayer.Looping = MediaPlayer.IsRepeating;
            _playingSong = this;

            if (startPosition.HasValue)
                Position = startPosition.Value;
            _androidPlayer.Start();
            _playCount++;
        }

        internal void Resume()
        {
            _androidPlayer.Start();
        }

        internal void Pause()
        {
            _androidPlayer.Pause();
        }

        internal void Stop()
        {
            _androidPlayer.Stop();
            _playingSong = null;
            _playCount = 0;
            position = TimeSpan.Zero;
        }

        internal float Volume
        {
            get
            {
                return 0.0f;
            }

            set
            {
                _androidPlayer.SetVolume(value, value);
            }
        }

        internal float Pitch
        {
            get
            {
                return _androidPlayer.PlaybackParams.Pitch;
            }
            set
            {
                float p = SoundEffectInstance.XnaPitchToAlPitch(value);
                _androidPlayer.PlaybackParams.SetPitch(p);
            }
        }

        public TimeSpan Position
        {
            get
            {
                if (_playingSong == this && _androidPlayer.IsPlaying)
                    position = TimeSpan.FromMilliseconds(_androidPlayer.CurrentPosition);

                return position;
            }
            set
            {
                _androidPlayer.SeekTo((int)value.TotalMilliseconds);   
            }
        }


        private Album PlatformGetAlbum()
        {
            return album;
        }

        private Artist PlatformGetArtist()
        {
            return artist;
        }

        private Genre PlatformGetGenre()
        {
            return genre;
        }

        private TimeSpan PlatformGetDuration()
        {
            return AssetUri != null ? duration : _duration;
        }

        private bool PlatformIsProtected()
        {
            return false;
        }

        private bool PlatformIsRated()
        {
            return false;
        }

        private string PlatformGetName()
        {
            return name ?? Path.GetFileNameWithoutExtension(AssetUri.Path);
        }

        private int PlatformGetPlayCount()
        {
            return _playCount;
        }

        private int PlatformGetRating()
        {
            return 0;
        }

        private int PlatformGetTrackNumber()
        {
            return 0;
        }
    }
}

