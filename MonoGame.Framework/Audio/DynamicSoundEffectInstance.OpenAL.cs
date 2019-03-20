// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MonoGame.OpenAL;

namespace Microsoft.Xna.Framework.Audio
{
    public sealed partial class DynamicSoundEffectInstance : SoundEffectInstance
    {
        private Queue<OALSoundBuffer> _queuedBuffers;

        private void PlatformCreate()
        {
            InitializeSound();

            SourceID = controller.ReserveSource();
            HasSourceId = true;

            _queuedBuffers = new Queue<OALSoundBuffer>();
        }

        private int PlatformGetPendingBufferCount()
        {
            return _queuedBuffers.Count;
        }

        private int PlatformGetBufferedSamples()
        {
            AL.GetError();
            int total = 0;

            lock (_queuedBuffers)
            {
                if (_queuedBuffers.Count == 0)
                    return 0;

                foreach (var buff in _queuedBuffers)
                    total += buff.SampleCount;
            }

            AL.GetSource(SourceID, ALGetSourcei.SampleOffset, out int offset);
            ALHelper.CheckError("Failed to get sample offset in source.");
            total -= offset;

            return total;
        }

        private void PlatformPlay()
        {
            AL.GetError();

            // Ensure that the source is not looped (due to source recycling)
            AL.Source(SourceID, ALSourceb.Looping, false);
            ALHelper.CheckError("Failed to set source loop state.");

            AL.SourcePlay(SourceID);
            ALHelper.CheckError("Failed to play the source.");
        }

        private void PlatformPause()
        {
            AL.GetError();
            AL.SourcePause(SourceID);
            ALHelper.CheckError("Failed to pause the source.");
        }

        private void PlatformResume()
        {
            AL.GetError();
            AL.SourcePlay(SourceID);
            ALHelper.CheckError("Failed to play the source.");
        }

        private void PlatformStop()
        {
            AL.GetError();
            AL.SourceStop(SourceID);
            ALHelper.CheckError("Failed to stop the source.");

            // Remove all queued buffers
            AL.Source(SourceID, ALSourcei.Buffer, 0);

            lock (_queuedBuffers)
            {
                while (_queuedBuffers.Count > 0)
                {
                    var buffer = _queuedBuffers.Dequeue();
                    OALSoundBufferPool.Return(buffer);
                }
            }
        }

        private void PlatformSubmitBuffer(byte[] buffer, int offset, int count)
        {
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject() + offset;
                PlatformSubmitBuffer(ptr, count, false);
            }
            finally
            {
                handle.Free();
            }
        }

        private void PlatformSubmitBuffer(short[] buffer, int offset, int count)
        {
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject() + offset * sizeof(short);
                PlatformSubmitBuffer(ptr, count * sizeof(short), false);
            }
            finally
            {
                handle.Free();
            }
        }

        private void PlatformSubmitBuffer(float[] buffer, int offset, int count)
        {
            if (!OpenALSoundController.Instance.SupportsFloat32)
                throw new NotSupportedException("Float data is not supported.");

            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject() + offset * sizeof(float);
                PlatformSubmitBuffer(ptr, count * sizeof(float), true);
            }
            finally
            {
                handle.Free();
            }
        }

        private void PlatformSubmitBuffer(IntPtr buffer, int bytes, bool useFloat)
        {
            // Get a buffer
            var alBuffer = OALSoundBufferPool.Rent();

            // Bind the data
            ALFormat format = ALHelper.GetALFormat(_channels, useFloat);
            alBuffer.BufferData(buffer, format, bytes, _sampleRate);

            // Queue the buffer
            AL.SourceQueueBuffer(SourceID, alBuffer.BufferID);
            ALHelper.CheckError();

            lock (_queuedBuffers)
                _queuedBuffers.Enqueue(alBuffer);

            // If the source has run out of buffers, restart it
            var sourceState = AL.GetSourceState(SourceID);
            if (_state == SoundState.Playing && sourceState == ALSourceState.Stopped)
            {
                AL.SourcePlay(SourceID);
                ALHelper.CheckError("Failed to resume source playback.");
            }
        }

        private void PlatformDispose(bool disposing)
        {
            // Stop the source and bind null buffer so that it can be recycled
            AL.GetError();
            if (AL.IsSource(SourceID))
            {
                AL.SourceStop(SourceID);
                AL.Source(SourceID, ALSourcei.Buffer, 0);
                ALHelper.CheckError("Failed to stop the source.");
                controller.RecycleSource(SourceID);
            }

            if (disposing)
            {
                lock (_queuedBuffers)
                {
                    while (_queuedBuffers.Count > 0)
                    {
                        var buffer = _queuedBuffers.Dequeue();
                        OALSoundBufferPool.Return(buffer);
                    }
                }
                DynamicSoundEffectInstanceManager.RemoveInstance(this);
            }
        }

        private void PlatformUpdateQueue()
        {
            // Get the completed buffers
            AL.GetError();
            AL.GetSource(SourceID, ALGetSourcei.BuffersProcessed, out int numBuffers);
            ALHelper.CheckError("Failed to get processed buffer count.");

            // Unqueue them
            if (numBuffers > 0)
            {
                AL.SourceUnqueueBuffers(SourceID, numBuffers);
                ALHelper.CheckError("Failed to unqueue buffers.");

                lock (_queuedBuffers)
                {
                    for (int i = 0; i < numBuffers; i++)
                    {
                        var buffer = _queuedBuffers.Dequeue();
                        OALSoundBufferPool.Return(buffer);
                    }
                }
            }

            // Raise the event for each removed buffer, if needed
            for (int i = 0; i < numBuffers; i++)
                CheckBufferCount();
        }
    }
}
