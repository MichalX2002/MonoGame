// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MonoGame.Utilities;
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace MonoGame.Framework.Audio
{
    public sealed partial class DynamicSoundEffectInstance : SoundEffectInstance
    {
        private static ByteBufferPool _bufferPool = new ByteBufferPool();

        private Queue<DataItem> _queuedItems;

        private void PlatformCreate()
        {
            _format = new WaveFormat(_sampleRate, (int)_channels);
            _voice = new SourceVoice(SoundEffect.Device, _format, true);
            _voice.BufferEnd += OnBufferEnd;
            _queuedItems = new Queue<DataItem>();
        }

        private int PlatformGetPendingBufferCount()
        {
            return _queuedItems.Count;
        }

        private long PlatformGetBufferedSamples()
        {
            if (_queuedItems.Count == 0)
                return 0;

            long bufferedSamples = 0;
            long divider = _format.BitsPerSample / 8 * _format.Channels;
            foreach (var buff in _queuedItems)
                bufferedSamples += buff.Audio.AudioBytes / divider;

            long offset = _voice.State.SamplesPlayed;
            return bufferedSamples - offset;
        }

        private void PlatformPlay()
        {
            _voice.Start();
        }

        private void PlatformPause()
        {
            _voice.Stop();
        }

        private void PlatformResume()
        {
            _voice.Start();
        }

        private void PlatformStop()
        {
            _voice.Stop();

            // Dequeue all the submitted buffers
            _voice.FlushSourceBuffers();

            while (_queuedItems.Count > 0)
                DequeueItem();
        }

        private unsafe void PlatformSubmitBuffer<T>(ReadOnlySpan<T> data)
            where T : unmanaged
        {
            var byteSrc = MemoryMarshal.AsBytes(data);

            // we need to copy so datastream does not pin the buffer that the user might modify later
            byte[] pooledBuffer = _bufferPool.Get(byteSrc.Length);
            byteSrc.CopyTo(pooledBuffer);

            var stream = DataStream.Create(pooledBuffer, true, false, 0, true);
            var audioBuffer = new AudioBuffer(stream)
            {
                AudioBytes = byteSrc.Length
            };

            _voice.SubmitSourceBuffer(audioBuffer, null);
            _queuedItems.Enqueue(new DataItem(audioBuffer, pooledBuffer));
        }

        private void PlatformUpdateQueue()
        {
            // The XAudio implementation utilizes callbacks, so no work here.
        }

        private void PlatformDispose(bool disposing)
        {
            if (disposing)
            {
                while (_queuedItems.Count > 0)
                    DequeueItem();
            }
            // _voice is disposed by SoundEffectInstance.PlatformDispose
        }

        private void OnBufferEnd(IntPtr obj)
        {
            // Release the buffer
            if (_queuedItems.Count > 0)
                DequeueItem();

            CheckBufferCount();
        }

        private void DequeueItem()
        {
            var item = _queuedItems.Dequeue();
            item.Audio.Stream.Dispose();
            _bufferPool.Return(item.Buffer);
        }

        private readonly struct DataItem
        {
            public AudioBuffer Audio { get; }
            public byte[] Buffer { get; }

            public DataItem(AudioBuffer audio, byte[] byteBuffer)
            {
                Audio = audio ?? throw new ArgumentNullException(nameof(audio));
                Buffer = byteBuffer ?? throw new ArgumentNullException(nameof(byteBuffer));
            }
        }
    }
}
