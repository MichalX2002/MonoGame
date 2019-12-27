// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using MonoGame.OpenAL;

namespace MonoGame.Framework.Audio
{
    public sealed partial class DynamicSoundEffectInstance : SoundEffectInstance
    {
        private Queue<ALBuffer> _queuedBuffers;

        private void PlatformCreate()
        {
            InitializeSound();

            SourceId = Controller.ReserveSource();

            // Ensure that the source is not looped (due to source recycling)
            AL.Source(SourceId.Value, ALSourceb.Looping, false);
            ALHelper.CheckError("Failed to set source loop state.");

            _queuedBuffers = new Queue<ALBuffer>();
        }

        private int PlatformGetBufferedSamples()
        {
            if (!SourceId.HasValue)
                return 0;

            int total = 0;
            lock (_queuedBuffers)
            {
                if (_queuedBuffers.Count == 0)
                    return 0;

                foreach (var buff in _queuedBuffers)
                    total += buff.SampleCount;
            }

            AL.GetSource(SourceId.Value, ALGetSourcei.SampleOffset, out int offset);
            ALHelper.CheckError("Failed to get sample offset in source.");
            total -= offset;

            return total;
        }

        private int PlatformGetPendingBufferCount() => _queuedBuffers.Count;

        private void PlatformPlay()
        {
            AL.SourcePlay(SourceId.Value);
            ALHelper.CheckError("Failed to play the source.");
        }

        private void PlatformPause()
        {
            AL.SourcePause(SourceId.Value);
            ALHelper.CheckError("Failed to pause the source.");
        }

        private void PlatformResume()
        {
            AL.SourcePlay(SourceId.Value);
            ALHelper.CheckError("Failed to play the source.");
        }

        private void PlatformStop()
        {
            AL.SourceStop(SourceId.Value);
            ALHelper.CheckError("Failed to stop the source.");

            // Remove all queued buffers
            AL.Source(SourceId.Value, ALSourcei.Buffer, 0);
            ALHelper.CheckError("Failed to unbind the buffer.");

            lock (_queuedBuffers)
            {
                while (_queuedBuffers.Count > 0)
                {
                    var buffer = _queuedBuffers.Dequeue();
                    ALBufferPool.Return(buffer);
                }
            }
        }

        private void PlatformSubmitBuffer<T>(ReadOnlySpan<T> data, AudioDepth depth)
            where T : unmanaged
        {
            // Bind the data
            ALFormat alFormat = ALHelper.GetALFormat(_channels, depth);
            var buffer = ALBufferPool.Rent();
            buffer.BufferData(data, alFormat, _sampleRate);
            
            // Queue the buffer
            AL.SourceQueueBuffer(SourceId.Value, buffer.BufferId);
            ALHelper.CheckError("Failed to queue buffer.");

            lock (_queuedBuffers)
                _queuedBuffers.Enqueue(buffer);

            // If the source has run out of buffers, restart it
            var sourceState = AL.GetSourceState(SourceId.Value);
            if (_state == SoundState.Playing && sourceState == ALSourceState.Stopped)
            {
                AL.SourcePlay(SourceId.Value);
                ALHelper.CheckError("Failed to resume source playback.");
            }
        }

        private void PlatformDispose(bool disposing)
        {
            // Stop the source and bind null buffer so that it can be recycled
            if (SourceId.HasValue && AL.IsSource(SourceId.Value))
            {
                AL.SourceStop(SourceId.Value);
                ALHelper.CheckError("Failed to stop the source.");

                AL.Source(SourceId.Value, ALSourcei.Buffer, 0);
                ALHelper.CheckError("Failed to unbind the buffer.");

                FreeSource();
            }

            if (disposing)
            {
                lock (_queuedBuffers)
                {
                    while (_queuedBuffers.Count > 0)
                    {
                        var buffer = _queuedBuffers.Dequeue();
                        ALBufferPool.Return(buffer);
                    }
                }
            }
            DynamicSoundEffectInstanceManager.RemoveInstance(this);
        }

        private void PlatformUpdateQueue()
        {
            // Get the completed buffers
            AL.GetSource(SourceId.Value, ALGetSourcei.BuffersProcessed, out int numBuffers);
            ALHelper.CheckError("Failed to get processed buffer count.");

            // Unqueue them
            if (numBuffers > 0)
            {
                AL.SourceUnqueueBuffers(SourceId.Value, numBuffers);
                ALHelper.CheckError("Failed to unqueue buffers.");

                lock (_queuedBuffers)
                {
                    for (int i = 0; i < numBuffers; i++)
                    {
                        var buffer = _queuedBuffers.Dequeue();
                        ALBufferPool.Return(buffer);
                    }
                }
            }

            // Raise the event for each removed buffer, if needed
            for (int i = 0; i < numBuffers; i++)
                CheckBufferCount();
        }
    }
}
