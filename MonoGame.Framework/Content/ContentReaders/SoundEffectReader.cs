// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using NVorbis;

namespace Microsoft.Xna.Framework.Content
{
	internal class SoundEffectReader : ContentTypeReader<SoundEffect>
	{
        [ThreadStatic]
        private float[] _buffer;

        protected internal override SoundEffect Read(ContentReader input, SoundEffect existingInstance)
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
            int durationMs = input.ReadInt32();

            byte[] data;
            int soundEffectSize;

            int dataSize = input.ReadInt32();
            short format = BitConverter.ToInt16(header, 0);
            if (format == 1)
            {
                using (var reader = new VorbisReader(input.BaseStream, false))
                {
                    long bytes = reader.TotalSamples * reader.Channels * sizeof(short);
                    if (bytes > int.MaxValue)
                        throw new InvalidDataException("Size of raw audio data exceeded " + int.MaxValue + " bytes.");

                    soundEffectSize = (int)bytes;
                    data = new byte[soundEffectSize];

                    unsafe
                    {
                        fixed (byte* dataPtr = data)
                        {
                            if (_buffer == null)
                                _buffer = new float[1024 * 8];
                            var shortDataPtr = (short*)dataPtr;

                            int totalSamples = 0;
                            int samplesRead;
                            while ((samplesRead = reader.ReadSamples(_buffer, 0, _buffer.Length)) > 0)
                            {
                                for (int i = 0; i < samplesRead; i++)
                                {
                                    int tmp = (int)(32767f * _buffer[i]);
                                    if (tmp > short.MaxValue)
                                        tmp = short.MaxValue;
                                    else if (tmp < short.MinValue)
                                        tmp = short.MinValue;
                                    shortDataPtr[i + totalSamples] = (short)tmp;
                                }
                                totalSamples += samplesRead;
                            }
                        }
                    }
                }
            }
            else
            {
                data = input.ReadBytes(dataSize);
                soundEffectSize = dataSize;
            }

            // Create the effect.
            return new SoundEffect(header, data, soundEffectSize, durationMs, loopStart, loopLength)
            {
                // Store the original asset name for debugging later.
                Name = input.AssetName
            };
        }
    }
}
