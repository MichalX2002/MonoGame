// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using MonoGame.OpenAL;

namespace MonoGame.Framework.Audio
{
    internal class ALBuffer : IDisposable
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

        public ALBuffer()
        {
        }

        public unsafe void BufferData<T>(
            ReadOnlySpan<T> data, ALFormat format, int sampleRate, int sampleAlignment = 0)
            where T : unmanaged
        {
            AssertNotDisposed();

            if ((format == ALFormat.MonoFloat32 || format == ALFormat.StereoFloat32) && !ALController.Instance.SupportsFloat32)
                throw new InvalidOperationException("Float data is not supported by this OpenAL driver.");

            if ((format == ALFormat.MonoMSAdpcm || format == ALFormat.StereoMSAdpcm) && !ALController.Instance.SupportsAdpcm)
                throw new InvalidOperationException("MS-ADPCM is not supported by this OpenAL driver.");

            if ((format == ALFormat.MonoIma4 || format == ALFormat.StereoIma4) && !ALController.Instance.SupportsIma4)
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

            fixed (T* ptr = &MemoryMarshal.GetReference(data))
            {
                AL.BufferData(BufferID, format, (IntPtr)ptr, sizeof(T) * data.Length, sampleRate);
                ALHelper.CheckError("Failed to fill buffer.");
            }
        }

        [System.Diagnostics.DebuggerHidden]
        private void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ALBuffer));
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