using System;
using System.IO;

namespace Microsoft.Xna.Framework.Audio
{
    internal static class AudioUtil
    {
        /// <summary>
        /// Appends a WAV header to PCM data.
        /// </summary>
        internal static MemoryStream FormatWavData(byte[] buffer, int sampleRate, int channels)
        {
            //buffer should contain 16-bit PCM wave data
            short bitsPerSample = 16;

            using (var output = new MemoryStream(44 + buffer.Length))
            using (var writer = new BinaryWriter(output))
            {
                writer.Write("RIFF".ToCharArray()); //chunk id
                writer.Write((int)(36 + buffer.Length)); //chunk size
                writer.Write("WAVE".ToCharArray()); //RIFF type

                writer.Write("fmt ".ToCharArray()); //chunk id
                writer.Write((int)16); //format header size
                writer.Write((short)1); //format (PCM)
                writer.Write((short)channels);
                writer.Write((int)sampleRate);
                short blockAlign = (short)((bitsPerSample / 8) * (int)channels);
                writer.Write((int)(sampleRate * blockAlign)); //byte rate
                writer.Write((short)blockAlign);
                writer.Write((short)bitsPerSample);

                writer.Write("data".ToCharArray()); //chunk id
                writer.Write((int)buffer.Length); //data size

                writer.Write(buffer);

                return output;
            }
        }
    }
}

