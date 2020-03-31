﻿using MonoGame.Framework.Graphics;
using SharpDX;
using SharpDX.MediaFoundation;
using SharpDX.Win32;
using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Media
{
    public sealed partial class VideoPlayer : IDisposable
    {
        private MediaSession _session;
        private AudioStreamVolume _volumeController;
        private PresentationClock _clock;

        // HACK: Need SharpDX to fix this.
        private static Guid AudioStreamVolumeGuid;

        private Callback _callback;

        private Texture2D _videoCache;

        private void PlatformInitialize()
        {
            // The GUID is specified in a GuidAttribute attached to the class
            AudioStreamVolumeGuid = Guid.Parse(
                ((GuidAttribute)typeof(AudioStreamVolume).GetCustomAttributes(typeof(GuidAttribute), false)[0]).Value);

            MediaManagerState.CheckStartup();
            MediaFactory.CreateMediaSession(null, out _session);
        }

        private Texture2D PlatformGetTexture()
        {
            var sampleGrabber = Video.SampleGrabber;
            var texData = sampleGrabber.TextureData;
            if (texData == null)
                return null;

            // NOTE: It's entirely possible that we could lose the d3d context and therefore lose this texture,
            // but it's better than allocating a new texture each call!
            if (_videoCache == null)
                _videoCache = new Texture2D(GraphicsDevice, Video.Width, Video.Height, false, SurfaceFormat.Bgr32);

            _videoCache.SetData(texData.AsSpan());
            return _videoCache;
        }

        private MediaState PlatformGetState()
        {
            if (_clock != null)
            {
                _clock.GetState(0, out ClockState state);

                switch (state)
                {
                    case ClockState.Running:
                        return MediaState.Playing;
                        
                    case ClockState.Paused:
                        return MediaState.Paused;
                }
            }
            return MediaState.Stopped;
        }

        private void PlatformPause()
        {
            _session.Pause();
        }

        private void PlatformPlay()
        {
            // Cleanup the last song first.
            if (State != MediaState.Stopped)
            {
                _session.Stop();
                _session.ClearTopologies();
                _session.Close();
                if (_volumeController != null)
                {
                    _volumeController.Dispose();
                    _volumeController = null;
                }
                _clock.Dispose();
            }

            // create the callback if it hasn't been created yet
            if (_callback == null)
            {
                _callback = new Callback(this);
                _session.BeginGetEvent(_callback, null);
            }

            // Set the new song.
            _session.SetTopology(SessionSetTopologyFlags.Immediate, Video.Topology);

            // Get the clock.
            _clock = _session.Clock.QueryInterface<PresentationClock>();

            // Start playing.
            var varStart = new Variant();
            _session.Start(null, varStart);

            // Create cached texture
            if (_videoCache == null || _videoCache.Width != Video.Width || _videoCache.Height != Video.Height)
            {

                // we need to dispose of the old texture if we have one
                _videoCache?.Dispose();

                _videoCache = new Texture2D(GraphicsDevice, Video.Width, Video.Height, false, SurfaceFormat.Bgr32);
            }
        }

        private void PlatformResume()
        {
            _session.Start(null, null);
        }

        private void PlatformStop()
        {
            _session.ClearTopologies();
            _session.Stop();
            _session.Close();

            _volumeController?.Dispose();
            _volumeController = null;

            _clock?.Dispose();
            _clock = null;
        }

        private void SetChannelVolumes()
        {
            if (_volumeController != null && !_volumeController.IsDisposed)
            {
                float volume = _volume;
                if (IsMuted)
                    volume = 0.0f;

                for (int i = 0; i < _volumeController.ChannelCount; i++)
                    _volumeController.SetChannelVolume(i, volume);
            }
        }

        private void PlatformSetVolume()
        {
            if (_volumeController == null)
                return;

            SetChannelVolumes();
        }

        private void PlatformSetIsLooped()
        {
            throw new NotImplementedException();
        }

        private void PlatformSetIsMuted()
        {
            if (_volumeController == null)
                return;

            SetChannelVolumes();
        }

        private TimeSpan PlatformGetPlayPosition()
        {
            return TimeSpan.FromTicks(_clock.Time);
        }

        private void PlatformDispose(bool disposing)
        {
            if (disposing)
            {
                if (_videoCache != null)
                    _videoCache.Dispose();
            }
        }

        private void OnTopologyReady()
        {
            if (_session.IsDisposed)
                return;

            // Get the volume interface.
            MediaFactory.GetService(_session, MediaServiceKeys.StreamVolume, AudioStreamVolumeGuid, out IntPtr volumeObjectPtr);
            _volumeController = CppObject.FromPointer<AudioStreamVolume>(volumeObjectPtr);

            SetChannelVolumes();
        }

        private class Callback : IAsyncCallback
        {
            private VideoPlayer _player;

            public IDisposable Shadow { get; set; }
            public AsyncCallbackFlags Flags { get; private set; }
            public WorkQueueId WorkQueueId { get; private set; }

            public Callback(VideoPlayer player)
            {
                _player = player;
            }

            public void Invoke(AsyncResult asyncResultRef)
            {
                var ev = _player._session.EndGetEvent(asyncResultRef);

                // Trigger an "on Video Ended" event here if needed

                if (ev.TypeInfo == MediaEventTypes.SessionTopologyStatus && 
                    ev.Get(EventAttributeKeys.TopologyStatus) == TopologyStatus.Ready)
                    _player.OnTopologyReady();

                _player._session.BeginGetEvent(this, null);
            }

            public void Dispose()
            {
            }
        }
    }
}
