// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.InteropServices;
using MonoGame.Framework.Memory;

#if OPENAL
using MonoGame.OpenAL;
#endif

#if IOS
using AudioToolbox;
using AudioUnit;
#endif

namespace MonoGame.Framework.Audio
{
    public sealed partial class SoundEffect : IDisposable
    {
        internal const int MAX_PLAYING_INSTANCES = ALController.MAX_NUMBER_OF_SOURCES;
        internal static uint ReverbSlot = 0;
        internal static uint ReverbEffect = 0;

        internal ALBuffer SoundBuffer;

        #region Public Constructors

        private void PlatformLoadAudioStream(Stream stream, out TimeSpan duration)
        {
            using (var data = AudioLoader.Load(
                stream, out ALFormat format, out int freq, out int channels, out int blockAlignment,
                out int bitsPerSample, out _, out int sampleCount))
            {
                duration = TimeSpan.FromSeconds(sampleCount / (float)freq);

                var span = data.GetBuffer().AsSpan(0, (int)data.Length);
                PlatformInitializeBuffer(span, format, channels, freq, blockAlignment, bitsPerSample, 0, 0);
            }
        }

        private void PlatformInitializePcm(
            ReadOnlySpan<byte> data, int sampleBits, int sampleRate, AudioChannels channels,
            int loopStart, int loopLength)
        {
            byte[] largeBuffer = null;
            string bufferTag = null;
            try
            {
                if (sampleBits == 24)
                {
                    // Convert 24-bit signed PCM to 16-bit signed PCM
                    largeBuffer = AudioLoader.Convert24To16(data, out bufferTag, out int size);
                    data = largeBuffer.AsSpan(0, size);
                    sampleBits = 16;
                }

                var format = AudioLoader.GetSoundFormat(AudioLoader.FormatPcm, (int)channels, sampleBits);
                SoundBuffer = ALBufferPool.Rent();
                SoundBuffer.BufferData(data, format, sampleRate);
            }
            finally
            {
                if (largeBuffer != null)
                    RecyclableMemoryManager.Default.ReturnLargeBuffer(largeBuffer, bufferTag);
            }
        }

        private void PlatformInitializeIeeeFloat(
            ReadOnlySpan<byte> data, int sampleRate, AudioChannels channels, int loopStart, int loopLength)
        {
            if (!ALController.Instance.SupportsFloat32)
            {
                var floatData = MemoryMarshal.Cast<byte, float>(data);
                int byteCount = floatData.Length * sizeof(short);
                bool isRecyclable = byteCount <= RecyclableMemoryManager.Default.MaximumLargeBufferSize;

                byte[] largeBuffer = null;
                string bufferTag = nameof(PlatformInitializeIeeeFloat);
                try
                {
                    largeBuffer = isRecyclable
                        ? RecyclableMemoryManager.Default.GetLargeBuffer(byteCount, bufferTag)
                        : new byte[byteCount];

                    // If 32-bit IEEE float is not supported, convert to 16-bit signed PCM
                    AudioLoader.ConvertSingleToInt16(floatData, MemoryMarshal.Cast<byte, short>(largeBuffer));
                    PlatformInitializePcm(largeBuffer, 16, sampleRate, channels, loopStart, loopLength);
                    return;
                }
                finally
                {
                    if (isRecyclable)
                        RecyclableMemoryManager.Default.ReturnLargeBuffer(largeBuffer, bufferTag);
                }
            }

            var format = AudioLoader.GetSoundFormat(AudioLoader.FormatIeee, (int)channels, 32);
            SoundBuffer = ALBufferPool.Rent();
            SoundBuffer.BufferData(data, format, sampleRate);
        }

        private void PlatformInitializeAdpcm(
            ReadOnlySpan<byte> data, int sampleRate, AudioChannels channels, int blockAlignment,
            int loopStart, int loopLength)
        {
            if (!ALController.Instance.SupportsAdpcm)
            {
                // If MS-ADPCM is not supported, convert to 16-bit signed PCM
                var pcmData = AudioLoader.ConvertMsAdpcmToPcm(data, (int)channels, blockAlignment);
                PlatformInitializePcm(pcmData, 16, sampleRate, channels, loopStart, loopLength);
                return;
            }

            var format = AudioLoader.GetSoundFormat(AudioLoader.FormatMsAdpcm, (int)channels, 0);
            int sampleAlignment = AudioLoader.SampleAlignment(format, blockAlignment);

            // Buffer length must be aligned with the block alignment
            int alignedCount = data.Length - (data.Length % blockAlignment);
            data = data.Slice(0, alignedCount);

            SoundBuffer = ALBufferPool.Rent();
            SoundBuffer.BufferData(data, format, sampleRate, sampleAlignment);
        }

        private void PlatformInitializeIma4(
            ReadOnlySpan<byte> data, int sampleRate, AudioChannels channels, int blockAlignment,
            int loopStart, int loopLength)
        {
            if (!ALController.Instance.SupportsIma4)
            {
                // If IMA/ADPCM is not supported, convert to 16-bit signed PCM
                data = AudioLoader.ConvertIma4ToPcm(data, (int)channels, blockAlignment);
                PlatformInitializePcm(data, 16, sampleRate, channels, loopStart, loopLength);
                return;
            }

            var format = AudioLoader.GetSoundFormat(AudioLoader.FormatIma4, (int)channels, 0);
            int sampleAlignment = AudioLoader.SampleAlignment(format, blockAlignment);
            SoundBuffer = ALBufferPool.Rent();
            SoundBuffer.BufferData(data, format, sampleRate, sampleAlignment);
        }

        private void PlatformInitializeFormat(
            ReadOnlySpan<byte> header, ReadOnlySpan<byte> data, int loopStart, int loopLength)
        {
            short wavFormat = BinaryPrimitives.ReadInt16LittleEndian(header);
            short channels = BinaryPrimitives.ReadInt16LittleEndian(header.Slice(2));
            int sampleRate = BinaryPrimitives.ReadInt32LittleEndian(header.Slice(4));
            short blockAlignment = BinaryPrimitives.ReadInt16LittleEndian(header.Slice(12));
            short bitsPerSample = BinaryPrimitives.ReadInt16LittleEndian(header.Slice(14));

            var format = AudioLoader.GetSoundFormat(wavFormat, channels, bitsPerSample);
            PlatformInitializeBuffer(
                data, format, channels, sampleRate, blockAlignment, bitsPerSample, loopStart, loopLength);
        }

        private void PlatformInitializeBuffer(
            ReadOnlySpan<byte> buffer, ALFormat format, int channels, int sampleRate,
            int blockAlignment, int bitsPerSample, int loopStart, int loopLength)
        {
            switch (format)
            {
                case ALFormat.Mono8:
                case ALFormat.Mono16:
                case ALFormat.Stereo8:
                case ALFormat.Stereo16:
                    PlatformInitializePcm(
                        buffer, bitsPerSample, sampleRate, (AudioChannels)channels, loopStart, loopLength);
                    break;

                case ALFormat.MonoMSAdpcm:
                case ALFormat.StereoMSAdpcm:
                    PlatformInitializeAdpcm(
                        buffer, sampleRate, (AudioChannels)channels, blockAlignment, loopStart, loopLength);
                    break;

                case ALFormat.MonoFloat32:
                case ALFormat.StereoFloat32:
                    PlatformInitializeIeeeFloat(
                        buffer, sampleRate, (AudioChannels)channels, loopStart, loopLength);
                    break;

                case ALFormat.MonoIma4:
                case ALFormat.StereoIma4:
                    PlatformInitializeIma4(
                        buffer, sampleRate, (AudioChannels)channels, blockAlignment, loopStart, loopLength);
                    break;

                default:
                    throw new NotSupportedException("Unsupported wave format.");
            }
        }

        private void PlatformInitializeXact(
            ReadOnlySpan<byte> data, MiniFormatTag codec, int channels, int sampleRate, int blockAlignment,
            int loopStart, int loopLength, out TimeSpan duration)
        {
            if (codec != MiniFormatTag.Adpcm)
                throw new NotSupportedException("Unsupported sound format.");

            PlatformInitializeAdpcm(
                data, sampleRate, (AudioChannels)channels, (blockAlignment + 16) * channels, loopStart, loopLength);
            duration = TimeSpan.FromSeconds(SoundBuffer.Duration);
        }

        #endregion

        private void PlatformSetupInstance(SoundEffectInstance inst)
        {
            inst.InitializeSound();
        }

        internal static void PlatformSetReverbSettings(ReverbSettings reverbSettings)
        {
            var efx = ALController.Instance.Efx;
            if (!efx.IsAvailable)
                return;

            if (ReverbEffect != 0)
                return;

            ReverbSlot = efx.GenAuxiliaryEffectSlot();
            ReverbEffect = efx.GenEffect();
            efx.Effect(ReverbEffect, EfxEffecti.EffectType, (int)EfxEffectType.Reverb);
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbReflectionsDelay, reverbSettings.ReflectionsDelayMs / 1000f);
            efx.Effect(ReverbEffect, EfxEffectf.LateReverbDelay, reverbSettings.ReverbDelayMs / 1000f);

            // map these from range 0-15 to 0-1
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbDiffusion, reverbSettings.EarlyDiffusion / 15f);
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbDiffusion, reverbSettings.LateDiffusion / 15f);
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbGainLF, 
                Math.Min(XactHelpers.ParseVolumeFromDecibels(reverbSettings.LowEqGain - 8f), 1f));
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbLFReference, (reverbSettings.LowEqCutoff * 50f) + 50f);
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbGainHF,
                XactHelpers.ParseVolumeFromDecibels(reverbSettings.HighEqGain - 8f));
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbHFReference, (reverbSettings.HighEqCutoff * 500f) + 1000f);

            // According to Xamarin docs EaxReverbReflectionsGain Unit: Linear gain Range [0f .. 3.16f] Default: 0.05f
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbReflectionsGain, 
                Math.Min(XactHelpers.ParseVolumeFromDecibels(reverbSettings.ReflectionsGainDb), 3.16f));
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbGain,
                Math.Min(XactHelpers.ParseVolumeFromDecibels(reverbSettings.ReverbGainDb), 1f));

            // map these from 0-100 down to 0-1
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbDensity, reverbSettings.DensityPct / 100f);
            efx.AuxiliaryEffectSlot(ReverbSlot, EfxEffectSlotf.EffectSlotGain, reverbSettings.WetDryMixPct / 200f);

            // Dont know what to do with these; EFX has no mapping for them. 
            // Just ignore for now we can enable them as we go. 
            //efx.SetEffectParam(ReverbEffect, EfxEffectf.PositionLeft, reverbSettings.PositionLeft);
            //efx.SetEffectParam(ReverbEffect, EfxEffectf.PositionRight, reverbSettings.PositionRight);
            //efx.SetEffectParam(ReverbEffect, EfxEffectf.PositionLeftMatrix, reverbSettings.PositionLeftMatrix);
            //efx.SetEffectParam(ReverbEffect, EfxEffectf.PositionRightMatrix, reverbSettings.PositionRightMatrix);
            //efx.SetEffectParam(ReverbEffect, EfxEffectf.LowFrequencyReference, reverbSettings.RearDelayMs);
            //efx.SetEffectParam(ReverbEffect, EfxEffectf.LowFrequencyReference, reverbSettings.RoomFilterFrequencyHz);
            //efx.SetEffectParam(ReverbEffect, EfxEffectf.LowFrequencyReference, reverbSettings.RoomFilterMainDb);
            //efx.SetEffectParam(ReverbEffect, EfxEffectf.LowFrequencyReference, reverbSettings.RoomFilterHighFrequencyDb);
            //efx.SetEffectParam(ReverbEffect, EfxEffectf.LowFrequencyReference, reverbSettings.DecayTimeSec);
            //efx.SetEffectParam(ReverbEffect, EfxEffectf.LowFrequencyReference, reverbSettings.RoomSizeFeet);

            efx.BindEffectToAuxiliarySlot(ReverbSlot, ReverbEffect);
        }

        private void PlatformDispose(bool disposing)
        {
            if (SoundBuffer != null)
            {
                ALBufferPool.Return(SoundBuffer);
                SoundBuffer = null;
            }
        }

        internal static void PlatformInitialize()
        {
            ALController.EnsureInitialized();
        }

        internal static void PlatformShutdown()
        {
            if (_systemState == SoundSystemState.Initialized && ReverbEffect != 0)
            {
                var efx = ALController.Instance.Efx;
                efx.DeleteAuxiliaryEffectSlot(ReverbSlot);
                efx.DeleteEffect(ReverbEffect);
            }

            ALBufferPool.Clear();
            ALController.DestroyInstance();
        }
    }
}