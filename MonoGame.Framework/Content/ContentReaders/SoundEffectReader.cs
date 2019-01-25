// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Utilities.IO;
using NVorbis;

namespace Microsoft.Xna.Framework.Content
{
	internal class SoundEffectReader : ContentTypeReader<SoundEffect>
	{
		[ThreadStatic]
		private float[] _floatBuffer;
        
        [ThreadStatic]
        private byte[] _primitiveBuffer;

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

            SoundEffect effect;
            int rawSize = input.ReadInt32();
			short format = BitConverter.ToInt16(header, 0);

            SoundEffect CreateEffect(byte[] data, int length)
            {
                return new SoundEffect(header, data, length, durationMs, loopStart, loopLength)
                {
                    // Store the original asset name for debugging later.
                    Name = input.AssetName
                };
            }

            if (format == 1)
            {
                if (_floatBuffer == null)
                    _floatBuffer = new float[1024 * 4];

                if (_primitiveBuffer == null)
                    _primitiveBuffer = new byte[2];

                using (var reader = new VorbisReader(input.BaseStream, false))
                {
                    long bytes = reader.TotalSamples * reader.Channels * sizeof(short);
                    if (bytes > int.MaxValue)
                        throw new InvalidDataException("Size of decoded audio data exceeds " + int.MaxValue + " bytes.");

                    using (var memoryBuffer = RecyclableMemoryManager.Instance.GetMemoryStream(null, (int)bytes))
                    {
                        int totalSamples = 0;
                        int samplesRead;
                        while ((samplesRead = reader.ReadSamples(_floatBuffer, 0, _floatBuffer.Length)) > 0)
                        {
                            for (int i = 0; i < samplesRead; i++)
                            {
                                int tmp = (int)(32767f * _floatBuffer[i]);
                                if (tmp > short.MaxValue)
                                    tmp = short.MaxValue;
                                else if (tmp < short.MinValue)
                                    tmp = short.MinValue;

                                _primitiveBuffer[0] = (byte)tmp;
                                _primitiveBuffer[1] = (byte)(tmp >> 8);
                                memoryBuffer.Write(_primitiveBuffer, 0, 2);
                            }
                            totalSamples += samplesRead;
                        }

                        // this buffer becomes unusable after disposing
                        // the recyclable memory stream
                        byte[] data = memoryBuffer.GetBuffer();

                        // data gets uploaded synchronously
                        effect = CreateEffect(data, (int)memoryBuffer.Length);
                    }
                }
            }
            else
            {
                var memoryBuffer = RecyclableMemoryManager.Instance.GetMemoryStream(null, rawSize);
                var block = RecyclableMemoryManager.Instance.GetBlock();
                try
                {
                    int read = 0;
                    int leftToRead = rawSize;
                    while (leftToRead > 0 && (read = input.Read(block, 0, Math.Min(leftToRead, block.Length))) > 0)
                    {
                        memoryBuffer.Write(block, 0, read);
                        leftToRead -= read;
                    }

                    // same deal here as above
                    byte[] data = memoryBuffer.GetBuffer();
                    effect = CreateEffect(data, rawSize);
                }
                finally
                {
                    memoryBuffer.Dispose();
                    RecyclableMemoryManager.Instance.ReturnBlock(block, null);
                }
            }

            return effect;
		}
	}
}
