// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using MonoGame.OpenAL;

namespace Microsoft.Xna.Framework.Audio
{
    internal class OALSoundBuffer : IDisposable
    {
        int openALDataBuffer;
        ALFormat openALFormat;
        bool _isDisposed;

        public int OpenALDataBuffer => openALDataBuffer;
        public double Duration { get; set; }

        public OALSoundBuffer()
        {
            AL.GenBuffers(1, out openALDataBuffer);
            ALHelper.CheckError("Failed to generate OpenAL data buffer.");
        }

        public void BindDataBuffer(byte[] data, ALFormat format, int size, int sampleRate, int sampleAlignment = 0)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                BindDataBuffer(handle.AddrOfPinnedObject(), format, size, sampleRate, sampleAlignment);
            }
            finally
            {
                handle.Free();
            }
        }

        public void BindDataBuffer(IntPtr data, ALFormat format, int size, int sampleRate, int sampleAlignment = 0)
        {
            if ((format == ALFormat.MonoMSAdpcm || format == ALFormat.StereoMSAdpcm) && !OpenALSoundController.Instance.SupportsAdpcm)
                throw new InvalidOperationException("MS-ADPCM is not supported by this OpenAL driver");
            if ((format == ALFormat.MonoIma4 || format == ALFormat.StereoIma4) && !OpenALSoundController.Instance.SupportsIma4)
                throw new InvalidOperationException("IMA/ADPCM is not supported by this OpenAL driver");

            openALFormat = format;

            if (sampleAlignment > 0)
            {
                AL.Bufferi(openALDataBuffer, ALBufferi.UnpackBlockAlignmentSoft, sampleAlignment);
                ALHelper.CheckError("Failed to fill buffer.");
            }

            AL.BufferData(openALDataBuffer, openALFormat, data, size, sampleRate);
            ALHelper.CheckError("Failed to fill buffer.");
            Duration = -1;

            AL.GetBuffer(openALDataBuffer, ALGetBufferi.Bits, out int bits);
            ALHelper.CheckError("Failed to get buffer bits");

            AL.GetBuffer(openALDataBuffer, ALGetBufferi.Channels, out int channels);
            ALHelper.CheckError("Failed to get buffer channels");

            AL.GetBuffer(openALDataBuffer, ALGetBufferi.Size, out int unpackedSize);
            ALHelper.CheckError("Failed to get buffer size");
            Duration = unpackedSize / (bits / 8 * channels) / (float)sampleRate;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // Clean up managed objects
                }

                // Release unmanaged resources
                if (AL.IsBuffer(openALDataBuffer))
                {
                    ALHelper.CheckError("Failed to fetch buffer state.");
                    AL.DeleteBuffers(1, ref openALDataBuffer);
                    ALHelper.CheckError("Failed to delete buffer.");
                }

                _isDisposed = true;
            }
        }

        ~OALSoundBuffer()
        {
            Dispose(false);
        }
    }
}