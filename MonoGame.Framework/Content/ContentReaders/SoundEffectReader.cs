// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Buffers.Binary;
using System.IO;
using MonoGame.Framework.Audio;
using MonoGame.Framework.Memory;
using MonoGame.OpenAL;

namespace MonoGame.Framework.Content
{
    internal class SoundEffectReader : ContentTypeReader<SoundEffect>
    {
        protected internal override SoundEffect Read(
            ContentReader input, SoundEffect existingInstance)
        {
            // XNB format for SoundEffect...
            //            
            // Byte [format size]	Format	WAVEFORMATEX structure
            // UInt32	Data size	
            // Byte [data size]	Data	Audio waveform data
            // Int32	Loop start	In bytes (start must be format block aligned)
            // Int32	Loop length	In bytes (length must be format block aligned)
            // Int32	Duration	In milliseconds

            // The header containss the WAVEFORMATEX header structure
            // defined as the following...
            //
            //  WORD  wFormatTag;       // byte[0]  +2
            //  WORD  nChannels;        // byte[2]  +2
            //  DWORD nSamplesPerSec;   // byte[4]  +4
            //  DWORD nAvgBytesPerSec;  // byte[8]  +4
            //  WORD  nBlockAlign;      // byte[12] +2
            //  WORD  wBitsPerSample;   // byte[14] +2
            //  WORD  cbSize;           // byte[16] +2
            //
            // We let the sound effect deal with parsing this based
            // on what format the audio data actually is.

            int headerSize = input.ReadInt32();
            byte[] header = input.ReadBytes(headerSize);

            int loopStart = input.ReadInt32();
            int loopLength = input.ReadInt32();
            double durationMs = input.ReadDouble();

            SoundEffect effect;
            int rawSize = input.ReadInt32();
            short format = BinaryPrimitives.ReadInt16LittleEndian(header);

            SoundEffect CreateSoundEffect(RecyclableBuffer buffer)
            {
                var data = buffer.AsSpan(0, buffer.BaseLength);
                var duration = TimeSpan.FromMilliseconds(durationMs);

                return new SoundEffect(header, data, duration, loopStart, loopLength)
                {
                    // Store the original asset name for debugging later.
                    Name = input.AssetName
                };
            }

            if (format == 1)
            {
                using (var data = AudioLoader.Load(
                    input.BaseStream, out ALFormat alFormat, out _, out _, out _, out _, out _, out _))
                {
                    if (alFormat == ALFormat.MonoFloat32 || alFormat == ALFormat.StereoFloat32)
                        BinaryPrimitives.WriteInt16LittleEndian(header, 3);

                    // data gets uploaded synchronously
                    effect = CreateSoundEffect(data);
                }
            }
            else
            {
                using (var data = RecyclableBuffer.ReadBytes(
                    input.BaseStream, rawSize, nameof(SoundEffectReader)))
                    effect = CreateSoundEffect(data);
            }

            return effect;
        }
    }
}
