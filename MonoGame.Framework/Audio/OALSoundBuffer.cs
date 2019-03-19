// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MonoGame.OpenAL;

namespace Microsoft.Xna.Framework.Audio
{
    internal class OALSoundBuffer : IDisposable
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

        public double Duration => Size / (Bits / 8f * Channels) / SampleRate;

        public OALSoundBuffer()
        {
        }

        public void BufferData(byte[] data, ALFormat format, int size, int sampleRate, int sampleAlignment = 0)
        {
            AssertNotDisposed();

            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                BufferData(handle.AddrOfPinnedObject(), format, size, sampleRate, sampleAlignment);
            }
            finally
            {
                handle.Free();
            }
        }

        public void BufferData(IntPtr data, ALFormat format, int size, int sampleRate, int sampleAlignment = 0)
        {
            AssertNotDisposed();

            if ((format == ALFormat.MonoMSAdpcm || format == ALFormat.StereoMSAdpcm) && !OpenALSoundController.Instance.SupportsAdpcm)
                throw new InvalidOperationException("MS-ADPCM is not supported by this OpenAL driver");

            if ((format == ALFormat.MonoIma4 || format == ALFormat.StereoIma4) && !OpenALSoundController.Instance.SupportsIma4)
                throw new InvalidOperationException("IMA/ADPCM is not supported by this OpenAL driver");

            if (BufferID != 0)
                ClearBuffer();

            BufferID = AL.GenBuffer();
            ALHelper.CheckError("Failed to generate OpenAL data buffer.");

            if (sampleAlignment > 0)
            {
                AL.Bufferi(BufferID, ALBufferi.UnpackBlockAlignmentSoft, sampleAlignment);
                ALHelper.CheckError("Failed to set buffer alignment.");
            }

            AL.BufferData(BufferID, format, data, size, sampleRate);
            ALHelper.CheckError("Failed to fill buffer.");
        }

        [System.Diagnostics.DebuggerHidden]
        private void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(OALSoundBuffer));
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

        internal static class Pool
        {
            private static Queue<OALSoundBuffer> _pool = new Queue<OALSoundBuffer>();

            public static OALSoundBuffer Rent()
            {
                lock (_pool)
                {
                    if (_pool.Count > 0)
                        return _pool.Dequeue();
                }
                return new OALSoundBuffer();
            }

            public static void Return(OALSoundBuffer buffer)
            {
                if (buffer == null)
                    throw new ArgumentNullException(nameof(buffer));

                if (buffer.IsDisposed)
                    throw new ObjectDisposedException(nameof(OALSoundBuffer));

                lock (_pool)
                {
                    if (_pool.Count < 32 && !_pool.Contains(buffer))
                    {
                        _pool.Enqueue(buffer);
                        buffer.ClearBuffer();
                    }
                    else
                        buffer.Dispose();
                }
            }

            public static void Clear()
            {
                lock (_pool)
                {
                    while (_pool.Count > 0)
                        _pool.Dequeue().Dispose();
                }
            }
        }
    }
}