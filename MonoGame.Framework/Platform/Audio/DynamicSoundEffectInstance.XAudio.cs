// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MonoGame.Framework;
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

        private int PlatformGetPendingBufferCount() => _queuedItems.Count;

        private void PlatformPlay() => _voice.Start();

        private void PlatformPause() => _voice.Stop();

        private void PlatformResume() => _voice.Start();

        private void PlatformStop()
        {
            _voice.Stop();

            // Dequeue all the submitted buffers
            _voice.FlushSourceBuffers();

            while (_queuedItems.Count > 0)
                DequeueItem();
        }

        private unsafe void PlatformSubmitBuffer<T>(ReadOnlySpan<T> data, AudioDepth depth)
            where T : unmanaged
        {
            int dataByteCount = data.Length * sizeof(T);
            int sampleCount = dataByteCount / ((int)depth / 8);

            // The XAudio voice is always 16-bit, but we support 16-bit and 32-bit data.
            int bufferByteCount = sampleCount * sizeof(short);
            byte[] pooledBuffer = _bufferPool.Get(bufferByteCount);

            // we need to copy so datastream does not pin the buffer that the user might modify later
            if (depth == AudioDepth.Float)
            {
                // we need to convert to 16-bit
                var srcSpan = MemoryMarshal.Cast<T, float>(data);
                var dstSpan = MemoryMarshal.Cast<byte, short>(pooledBuffer.AsSpan(0, bufferByteCount));
                AudioLoader.ConvertSingleToInt16(srcSpan, dstSpan);
            }
            else
            {
                // the data was 16-bit, so just copy over
                var srcSpan = MemoryMarshal.AsBytes(data);
                srcSpan.CopyTo(pooledBuffer);
            }

            var stream = DataStream.Create(pooledBuffer, true, false, 0, true);
            var audioBuffer = new AudioBuffer(stream)
            {
                AudioBytes = bufferByteCount
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
