// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using MonoGame.OpenAL;

namespace MonoGame.Framework.Audio
{
    internal class ALBuffer : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public uint BufferId { get; private set; }

        public int Bits
        {
            get
            {
                if (BufferId == 0)
                    return 0;
                AL.GetBuffer(BufferId, ALGetBufferi.Bits, out int bits);
                ALHelper.CheckError("Failed to get buffer bits.");
                return bits;
            }
        }

        public int Size
        {
            get
            {
                if (BufferId == 0)
                    return 0;
                AL.GetBuffer(BufferId, ALGetBufferi.Size, out int unpackedSize);
                ALHelper.CheckError("Failed to get buffer size");
                return unpackedSize;
            }
        }

        public int Channels
        {
            get
            {
                if (BufferId == 0)
                    return 0;
                AL.GetBuffer(BufferId, ALGetBufferi.Channels, out int channels);
                ALHelper.CheckError("Failed to get buffer channels.");
                return channels;
            }
        }

        public int SampleRate
        {
            get
            {
                if (BufferId == 0)
                    return 0;
                AL.GetBuffer(BufferId, ALGetBufferi.Frequency, out int sampleRate);
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

            if (!ALController.Instance.SupportsFloat32 && (format == ALFormat.MonoFloat32 || format == ALFormat.StereoFloat32))
                throw new InvalidOperationException("Float data is not supported by this OpenAL driver.");

            if (!ALController.Instance.SupportsAdpcm && (format == ALFormat.MonoMSAdpcm || format == ALFormat.StereoMSAdpcm))
                throw new InvalidOperationException("MS-ADPCM is not supported by this OpenAL driver.");

            if (!ALController.Instance.SupportsIma4 && (format == ALFormat.MonoIma4 || format == ALFormat.StereoIma4))
                throw new InvalidOperationException("IMA/ADPCM is not supported by this OpenAL driver.");

            if (BufferId != 0)
                ClearBuffer();

            BufferId = AL.GenBuffer();
            ALHelper.CheckError("Failed to generate OpenAL data buffer.");

            if (sampleAlignment > 0)
            {
                AL.Bufferi(BufferId, ALBufferi.UnpackBlockAlignmentSoft, sampleAlignment);
                ALHelper.CheckError("Failed to set buffer alignment.");
            }

            AL.BufferData(BufferId, format, data, sampleRate);
            ALHelper.CheckError("Failed to fill buffer.");
        }

        public unsafe void BufferData<T>(
            Span<T> data, ALFormat format, int sampleRate, int sampleAlignment = 0)
            where T : unmanaged
        {
            BufferData((ReadOnlySpan<T>)data, format, sampleRate, sampleAlignment);
        }

        [DebuggerHidden]
        private void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ALBuffer));
        }

        public void ClearBuffer()
        {
            if (IsDisposed || BufferId == 0)
                return;

            bool isBuffer = AL.IsBuffer(BufferId);
            ALHelper.CheckError("Failed to fetch buffer state.");

            if (isBuffer)
            {
                AL.DeleteBuffer(BufferId);
                BufferId = 0;
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