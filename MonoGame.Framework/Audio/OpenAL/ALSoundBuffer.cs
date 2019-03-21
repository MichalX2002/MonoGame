// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using MonoGame.OpenAL;

namespace Microsoft.Xna.Framework.Audio
{
    internal class ALSoundBuffer : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public int BufferID { get; private set; }

        public int Bits
        {
            get
            {
                if (BufferID == 0)
                    return 0;
                AL.GetBuffer(BufferID, ALGetBufferi.Bits, out int bits);
                ALHelper.CheckError("Failed to get buffer bits.");
                return bits;
            }
        }

        public int Size
        {
            get
            {
                if (BufferID == 0)
                    return 0;
                AL.GetBuffer(BufferID, ALGetBufferi.Size, out int unpackedSize);
                ALHelper.CheckError("Failed to get buffer size");
                return unpackedSize;
            }
        }

        public int Channels
        {
            get
            {
                if (BufferID == 0)
                    return 0;
                AL.GetBuffer(BufferID, ALGetBufferi.Channels, out int channels);
                ALHelper.CheckError("Failed to get buffer channels.");
                return channels;
            }
        }

        public int SampleRate
        {
            get
            {
                if (BufferID == 0)
                    return 0;
                AL.GetBuffer(BufferID, ALGetBufferi.Frequency, out int sampleRate);
                ALHelper.CheckError("Failed to get buffer sample rate.");
                return sampleRate;
            }
        }

        public int SampleCount => (int)(Size / (Bits / 8.0 * Channels));
        public double Duration => SampleCount / SampleRate;

        public ALSoundBuffer()
        {
        }

        public void BufferData(
            float[] data, int count, ALFormat format, int sampleRate, int sampleAlignment = 0)
        {
            AssertNotDisposed();
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                int bytes = count * sizeof(float);
                BufferData(ptr, bytes, format, sampleRate, sampleAlignment);
            }
            finally
            {
                handle.Free();
            }
        }

        public void BufferData(
            short[] data, int count, ALFormat format, int sampleRate, int sampleAlignment = 0)
        {
            AssertNotDisposed();
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                int bytes = count * sizeof(short);
                BufferData(ptr, bytes, format, sampleRate, sampleAlignment);
            }
            finally
            {
                handle.Free();
            }
        }

        public void BufferData(
            byte[] data, int bytes, ALFormat format, int sampleRate, int sampleAlignment = 0)
        {
            AssertNotDisposed();
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                BufferData(ptr, bytes, format, sampleRate, sampleAlignment);
            }
            finally
            {
                handle.Free();
            }
        }

        public void BufferData(
            IntPtr data, int bytes, ALFormat format, int sampleRate, int sampleAlignment = 0)
        {
            AssertNotDisposed();
            AL.GetError();

            if ((format == ALFormat.MonoFloat32 || format == ALFormat.StereoFloat32) && !ALSoundController.Instance.SupportsFloat32)
                throw new InvalidOperationException();

            if ((format == ALFormat.MonoMSAdpcm || format == ALFormat.StereoMSAdpcm) && !ALSoundController.Instance.SupportsAdpcm)
                throw new InvalidOperationException("MS-ADPCM is not supported by this OpenAL driver.");

            if ((format == ALFormat.MonoIma4 || format == ALFormat.StereoIma4) && !ALSoundController.Instance.SupportsIma4)
                throw new InvalidOperationException("IMA/ADPCM is not supported by this OpenAL driver.");

            if (BufferID != 0)
                ClearBuffer();

            BufferID = AL.GenBuffer();
            ALHelper.CheckError("Failed to generate OpenAL data buffer.");

            if (sampleAlignment > 0)
            {
                AL.Bufferi(BufferID, ALBufferi.UnpackBlockAlignmentSoft, sampleAlignment);
                ALHelper.CheckError("Failed to set buffer alignment.");
            }

            AL.BufferData(BufferID, format, data, bytes, sampleRate);
            ALHelper.CheckError("Failed to fill buffer.");
        }

        [System.Diagnostics.DebuggerHidden]
        private void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ALSoundBuffer));
        }

        public void ClearBuffer()
        {
            if (IsDisposed || BufferID == 0)
                return;

            bool isBuffer = AL.IsBuffer(BufferID);
            ALHelper.CheckError("Failed to fetch buffer state.");

            if (isBuffer)
            {
                AL.DeleteBuffer(BufferID);
                BufferID = 0;
                ALHelper.CheckError("Failed to delete buffer.");
            }
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                ClearBuffer();
                IsDisposed = true;
            }
        }
    }
}