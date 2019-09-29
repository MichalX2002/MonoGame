// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
﻿
using System;
using System.IO;
using System.Runtime.InteropServices;
using MonoGame.Utilities.Memory;

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
                PlatformInitializeBuffer<byte>(span, format, channels, freq, blockAlignment, bitsPerSample, 0, 0);
            }
        }

        private void PlatformInitializePcm<T>(
            ReadOnlySpan<T> data, int sampleBits, int sampleRate, AudioChannels channels, int loopStart, int loopLength)
            where T : unmanaged
        {
            byte[] largeBuffer = null;
            string bufferTag = null;
            try
            {
                if (sampleBits == 24)
                {
                    // Convert 24-bit signed PCM to 16-bit signed PCM
                    largeBuffer = AudioLoader.Convert24To16(data, out bufferTag, out int size);
                    data = MemoryMarshal.Cast<byte, T>(largeBuffer.AsSpan(0, size));
                    sampleBits = 16;
                }

                var format = AudioLoader.GetSoundFormat(AudioLoader.FormatPcm, (int)channels, sampleBits);
                SoundBuffer = ALBufferPool.Rent();
                SoundBuffer.BufferData(data, format, sampleRate);
            }
            finally
            {
                if (largeBuffer != null && bufferTag != null)
                    RecyclableMemoryManager.Default.ReturnLargeBuffer(largeBuffer, bufferTag);
            }
        }

        private void PlatformInitializeIeeeFloat<T>(
            ReadOnlySpan<T> data, int sampleRate, AudioChannels channels, int loopStart, int loopLength)
            where T : unmanaged
        {
            if (!ALController.Instance.SupportsFloat32)
            {
                // If 32-bit IEEE float is not supported, convert to 16-bit signed PCM
                data = MemoryMarshal.Cast<byte, T>(AudioLoader.ConvertSingleToInt16(data));
                PlatformInitializePcm(data, 16, sampleRate, channels, loopStart, loopLength);
                return;
            }

            var format = AudioLoader.GetSoundFormat(AudioLoader.FormatIeee, (int)channels, 32);
            SoundBuffer = ALBufferPool.Rent();
            SoundBuffer.BufferData(data, format, sampleRate);
        }

        private void PlatformInitializeAdpcm<T>(
            ReadOnlySpan<T> data, int sampleRate, AudioChannels channels, int blockAlignment, int loopStart, int loopLength)
            where T : unmanaged
        {
            ReadOnlySpan<byte> bytes = MemoryMarshal.AsBytes(data);
            if (!ALController.Instance.SupportsAdpcm)
            {
                // If MS-ADPCM is not supported, convert to 16-bit signed PCM
                bytes = AudioLoader.ConvertMsAdpcmToPcm(bytes, (int)channels, blockAlignment);
                PlatformInitializePcm(bytes, 16, sampleRate, channels, loopStart, loopLength);
                return;
            }

            var format = AudioLoader.GetSoundFormat(AudioLoader.FormatMsAdpcm, (int)channels, 0);
            int sampleAlignment = AudioLoader.SampleAlignment(format, blockAlignment);

            // Buffer length must be aligned with the block alignment
            int alignedCount = bytes.Length - (bytes.Length % blockAlignment);
            bytes = bytes.Slice(0, alignedCount);

            SoundBuffer = ALBufferPool.Rent();
            SoundBuffer.BufferData(bytes, format, sampleRate, sampleAlignment);
        }

        private void PlatformInitializeIma4<T>(
            ReadOnlySpan<T> data, int sampleRate, AudioChannels channels, int blockAlignment, int loopStart, int loopLength)
            where T : unmanaged
        {
            if (!ALController.Instance.SupportsIma4)
            {
                // If IMA/ADPCM is not supported, convert to 16-bit signed PCM
                data = MemoryMarshal.Cast<byte, T>(AudioLoader.ConvertIma4ToPcm(data, (int)channels, blockAlignment));
                PlatformInitializePcm(data, 16, sampleRate, channels, loopStart, loopLength);
                return;
            }

            var format = AudioLoader.GetSoundFormat(AudioLoader.FormatIma4, (int)channels, 0);
            int sampleAlignment = AudioLoader.SampleAlignment(format, blockAlignment);
            SoundBuffer = ALBufferPool.Rent();
            SoundBuffer.BufferData(data, format, sampleRate, sampleAlignment);
        }

        private void PlatformInitializeFormat(ReadOnlySpan<byte> header, ReadOnlySpan<byte> data, int loopStart, int loopLength)
        {
            var wavFormat = header.ToInt16();
            var channels = header.Slice(2).ToInt16();
            var sampleRate = header.Slice(4).ToInt32();
            var blockAlignment = header.Slice(12).ToInt16();
            var bitsPerSample = header.Slice(14).ToInt16();

            var format = AudioLoader.GetSoundFormat(wavFormat, channels, bitsPerSample);
            PlatformInitializeBuffer(data, format, channels, sampleRate, blockAlignment, bitsPerSample, loopStart, loopLength);
        }

        private void PlatformInitializeBuffer<T>(
            ReadOnlySpan<T> buffer, ALFormat format, int channels, int sampleRate,
            int blockAlignment, int bitsPerSample, int loopStart, int loopLength)
            where T : unmanaged
        {
            switch (format)
            {
                case ALFormat.Mono8:
                case ALFormat.Mono16:
                case ALFormat.Stereo8:
                case ALFormat.Stereo16:
                    PlatformInitializePcm(buffer, bitsPerSample, sampleRate, (AudioChannels)channels, loopStart, loopLength);
                    break;

                case ALFormat.MonoMSAdpcm:
                case ALFormat.StereoMSAdpcm:
                    PlatformInitializeAdpcm(buffer, sampleRate, (AudioChannels)channels, blockAlignment, loopStart, loopLength);
                    break;

                case ALFormat.MonoFloat32:
                case ALFormat.StereoFloat32:
                    PlatformInitializeIeeeFloat(buffer, sampleRate, (AudioChannels)channels, loopStart, loopLength);
                    break;

                case ALFormat.MonoIma4:
                case ALFormat.StereoIma4:
                    PlatformInitializeIma4(buffer, sampleRate, (AudioChannels)channels, blockAlignment, loopStart, loopLength);
                    break;

                default:
                    throw new NotSupportedException("Unsupported wave format.");
            }
        }

        private void PlatformInitializeXact<T>(
            MiniFormatTag codec, ReadOnlySpan<T> data, int channels, int sampleRate, int blockAlignment,
            int loopStart, int loopLength, out TimeSpan duration)
            where T : unmanaged
        {
            if (codec != MiniFormatTag.Adpcm)
                throw new NotSupportedException("Unsupported sound format.");

            PlatformInitializeAdpcm(data, sampleRate, (AudioChannels)channels, (blockAlignment + 16) * channels, loopStart, loopLength);
            duration = TimeSpan.FromSeconds(SoundBuffer.Duration);
        }

        #endregion

        private void PlatformSetupInstance(SoundEffectInstance inst)
        {
            inst.InitializeSound();
        }

        internal static void PlatformSetReverbSettings(ReverbSettings reverbSettings)
        {
            if (!ALController.Efx.IsInitialized)
                return;

            if (ReverbEffect != 0)
                return;

            var efx = ALController.Efx;
            efx.GenAuxiliaryEffectSlots(1, out ReverbSlot);
            efx.GenEffect(out ReverbEffect);
            efx.Effect(ReverbEffect, EfxEffecti.EffectType, (int)EfxEffectType.Reverb);
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbReflectionsDelay, reverbSettings.ReflectionsDelayMs / 1000f);
            efx.Effect(ReverbEffect, EfxEffectf.LateReverbDelay, reverbSettings.ReverbDelayMs / 1000f);

            // map these from range 0-15 to 0-1
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbDiffusion, reverbSettings.EarlyDiffusion / 15f);
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbDiffusion, reverbSettings.LateDiffusion / 15f);
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbGainLF, Math.Min(XactHelpers.ParseVolumeFromDecibels(reverbSettings.LowEqGain - 8f), 1f));
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbLFReference, (reverbSettings.LowEqCutoff * 50f) + 50f);
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbGainHF, XactHelpers.ParseVolumeFromDecibels(reverbSettings.HighEqGain - 8f));
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbHFReference, (reverbSettings.HighEqCutoff * 500f) + 1000f);

            // According to Xamarin docs EaxReverbReflectionsGain Unit: Linear gain Range [0f .. 3.16f] Default: 0.05f
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbReflectionsGain, Math.Min(XactHelpers.ParseVolumeFromDecibels(reverbSettings.ReflectionsGainDb), 3.16f));
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbGain, Math.Min(XactHelpers.ParseVolumeFromDecibels(reverbSettings.ReverbGainDb), 1f));

            // map these from 0-100 down to 0-1
            efx.Effect(ReverbEffect, EfxEffectf.EaxReverbDensity, reverbSettings.DensityPct / 100f);
            efx.AuxiliaryEffectSlot(ReverbSlot, EfxEffectSlotf.EffectSlotGain, reverbSettings.WetDryMixPct / 200f);

            // Dont know what to do with these EFX has no mapping for them. 
            // Just ignore for now we can enable them as we go. 
            //efx.SetEffectParam (ReverbEffect, EfxEffectf.PositionLeft, reverbSettings.PositionLeft);
            //efx.SetEffectParam (ReverbEffect, EfxEffectf.PositionRight, reverbSettings.PositionRight);
            //efx.SetEffectParam (ReverbEffect, EfxEffectf.PositionLeftMatrix, reverbSettings.PositionLeftMatrix);
            //efx.SetEffectParam (ReverbEffect, EfxEffectf.PositionRightMatrix, reverbSettings.PositionRightMatrix);
            //efx.SetEffectParam (ReverbEffect, EfxEffectf.LowFrequencyReference, reverbSettings.RearDelayMs);
            //efx.SetEffectParam (ReverbEffect, EfxEffectf.LowFrequencyReference, reverbSettings.RoomFilterFrequencyHz);
            //efx.SetEffectParam (ReverbEffect, EfxEffectf.LowFrequencyReference, reverbSettings.RoomFilterMainDb);
            //efx.SetEffectParam (ReverbEffect, EfxEffectf.LowFrequencyReference, reverbSettings.RoomFilterHighFrequencyDb);
            //efx.SetEffectParam (ReverbEffect, EfxEffectf.LowFrequencyReference, reverbSettings.DecayTimeSec);
            //efx.SetEffectParam (ReverbEffect, EfxEffectf.LowFrequencyReference, reverbSettings.RoomSizeFeet);

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
                ALController.Efx.DeleteAuxiliaryEffectSlot((int)ReverbSlot);
                ALController.Efx.DeleteEffect((int)ReverbEffect);
            }

            ALBufferPool.Clear();
            ALController.DestroyInstance();
        }
    }
}