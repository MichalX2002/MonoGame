﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Globalization;
using System.IO;
using System.Text;
using MonoGame.Framework.IO;

namespace MonoGame.Framework.Content.Pipeline.Audio
{
    internal class DefaultAudioProfile : AudioProfile
    {
        private static readonly char[] _newlineChars = new[] { '\r', '\n', '\0' };

        public const int FfmpegThreads = 0; // 0 = optimal

        public override bool Supports(TargetPlatform platform)
        {
            return platform == TargetPlatform.Android
                || platform == TargetPlatform.DesktopGL
                || platform == TargetPlatform.MacOSX
                || platform == TargetPlatform.NativeClient
                || platform == TargetPlatform.RaspberryPi
                || platform == TargetPlatform.Windows
                || platform == TargetPlatform.WindowsPhone8
                || platform == TargetPlatform.WindowsStoreApp
                || platform == TargetPlatform.iOS
                || platform == TargetPlatform.Web;
        }

        public override ConversionQuality ConvertAudio(
            TargetPlatform platform,
            ConversionQuality quality,
            AudioContent content)
        {
            //// Default to PCM data, or ADPCM if the source is ADPCM.
            //var targetFormat = ConversionFormat.Pcm;
            //
            //if (quality != ConversionQuality.Best || 
            //    content.Format.Format == 2 ||
            //    content.Format.Format == 17)
            //{
            //    if (platform == TargetPlatform.iOS ||
            //        platform == TargetPlatform.MacOSX || 
            //        platform == TargetPlatform.DesktopGL)
            //        targetFormat = ConversionFormat.ImaAdpcm;
            //    else
            //        targetFormat = ConversionFormat.Adpcm;
            //}

            var targetFormat = ConversionFormat.Vorbis;
            return ConvertToFormat(content, targetFormat, quality, null);
        }

        private static ConversionFormat PlatformToFormat(TargetPlatform platform)
        {
            switch (platform)
            {
                case TargetPlatform.Windows:
                case TargetPlatform.WindowsStoreApp:
                case TargetPlatform.WindowsPhone8:
                case TargetPlatform.DesktopGL:
                    return ConversionFormat.Vorbis;

                case TargetPlatform.Web:
                    return ConversionFormat.Mp3;

                default:
                    // Most platforms will use AAC ("mp4") by default
                    return ConversionFormat.Aac;
            }
        }

        public override ConversionQuality ConvertStreamingAudio(
            TargetPlatform platform,
            ConversionQuality quality,
            AudioContent content,
            string inputFile,
            out string destinationFile)
        {
            var targetFormat = PlatformToFormat(platform);

            // Get the song output path with the target format extension.
            destinationFile = Path.ChangeExtension(
                inputFile, AudioHelper.GetExtension(targetFormat));

            // Make sure the output folder for the file exists.
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));

            return ConvertToFormat(content, targetFormat, quality, destinationFile);
        }

        public static void ProbeFormat(
            string sourceFile,
            out AudioFileType audioFileType,
            out AudioFormat audioFormat,
            out TimeSpan duration,
            out int loopStart,
            out int loopLength)
        {
            string ffprobeArguments = string.Format(
                "-i \"{0}\" -show_format -show_entries streams -v quiet -of flat", sourceFile);

            int ffprobeExitCode = ExternalTool.Run(
                "ffprobe",
                ffprobeArguments,
                out var ffprobeStdout,
                out var ffprobeStderr);

            if (ffprobeExitCode != 0)
            {
                throw new InvalidOperationException(
                    "ffprobe exited with non-zero exit code.",
                    new Exception(ffprobeStderr.ToString()));
            }

            // Set default values if information is not available.
            int averageBytesPerSecond = 0;
            int bitsPerSample = 0;
            int blockAlign = 0;
            int channelCount = 0;
            int sampleRate = 0;
            int format = 0;
            string sampleFormat = null;
            double durationSeconds = 0;
            var formatName = string.Empty;

            try
            {
                var numberFormat = CultureInfo.InvariantCulture.NumberFormat;
                foreach (var line in ffprobeStdout.ToString()
                    .Split(_newlineChars, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] kv = line.Split(new[] { '=' }, 2);
                    switch (kv[0])
                    {
                        case "streams.stream.0.sample_rate":
                            sampleRate = int.Parse(kv[1].Trim('"'), numberFormat);
                            break;

                        case "streams.stream.0.bits_per_sample":
                            bitsPerSample = int.Parse(kv[1].Trim('"'), numberFormat);
                            break;

                        case "streams.stream.0.start_time":
                            if (double.TryParse(
                                kv[1].Trim('"'), NumberStyles.Any, numberFormat, out double seconds))
                                durationSeconds += seconds;
                            break;

                        case "streams.stream.0.duration":
                            durationSeconds += double.Parse(kv[1].Trim('"'), numberFormat);
                            break;

                        case "streams.stream.0.channels":
                            channelCount = int.Parse(kv[1].Trim('"'), numberFormat);
                            break;

                        case "streams.stream.0.sample_fmt":
                            sampleFormat = kv[1].Trim('"').ToUpperInvariant();
                            break;

                        case "streams.stream.0.bit_rate":
                            if (averageBytesPerSecond != 0)
                                break;

                            if (int.TryParse(
                                kv[1].Trim('"'), NumberStyles.Integer, numberFormat, out int bitsPerSec))
                                averageBytesPerSecond = bitsPerSec / 8;
                            break;

                        case "streams.stream.0.codec_tag":
                            string hex = kv[1][3..^1];
                            format = int.Parse(hex, NumberStyles.HexNumber, numberFormat);
                            break;

                        case "format.bit_rate":
                            if (averageBytesPerSecond == 0)
                                averageBytesPerSecond = int.Parse(kv[1].Trim('"'), numberFormat) / 8;
                            break;

                        case "format.format_name":
                            formatName = kv[1].Trim('"').ToUpperInvariant();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to parse ffprobe output.", ex);
            }

            // XNA seems to use the sample format for the bits per sample
            // in the case of non-PCM formats like MP3 and WMA.
            if (bitsPerSample == 0 && sampleFormat != null)
            {
                switch (sampleFormat)
                {
                    case "U8":
                    case "U8P":
                        bitsPerSample = 8;
                        break;

                    case "S16":
                    case "S16P":
                        bitsPerSample = 16;
                        break;

                    case "S32":
                    case "S32P":
                    case "FLT":
                    case "FLTP":
                        bitsPerSample = 32;
                        break;

                    case "DBL":
                    case "DBLP":
                        bitsPerSample = 64;
                        break;
                }
            }

            // Figure out the file type.
            if (formatName == "WAV")
            {
                audioFileType = AudioFileType.Wav;
            }
            else if (formatName == "MP3")
            {
                audioFileType = AudioFileType.Mp3;
                format = 1;
                bitsPerSample = Math.Min(bitsPerSample, 16);
            }
            else if (formatName == "WMA" || formatName == "ASF")
            {
                audioFileType = AudioFileType.Wma;
                format = 1;
                bitsPerSample = Math.Min(bitsPerSample, 16);
            }
            else if (formatName == "OGG")
            {
                audioFileType = AudioFileType.Ogg;
                format = 1;
                bitsPerSample = Math.Min(bitsPerSample, 16);
            }
            else if (formatName == "OPUS")
            {
                audioFileType = AudioFileType.Opus;
                format = 1;
                bitsPerSample = Math.Min(bitsPerSample, 16);
            }
            else
                audioFileType = (AudioFileType)(-1);

            // XNA seems to calculate the block alignment directly from 
            // the bits per sample and channel count regardless of the 
            // format of the audio data.
            // ffprobe doesn't report blockAlign for ADPCM and we cannot calculate it like this
            if (bitsPerSample > 0 && format != 2 && format != 17)
                blockAlign = bitsPerSample * channelCount / 8;
            
            duration = TimeSpan.FromTicks(
                (long)Math.Ceiling(durationSeconds * TimeSpan.TicksPerSecond));

            // Looks like XNA calculates the average bps from
            // the sample rate and block alignment.
            if (blockAlign > 0)
                averageBytesPerSecond = sampleRate * blockAlign;

            audioFormat = new AudioFormat(
                averageBytesPerSecond,
                bitsPerSample,
                blockAlign,
                channelCount,
                format,
                sampleRate);

            // Loop start and length in number of samples. For some
            // reason XNA doesn't report loop length for non-WAV sources.
            loopStart = 0;
            if (audioFileType != AudioFileType.Wav)
                loopLength = 0;
            else
                loopLength = (int)Math.Floor(sampleRate * durationSeconds);
        }

        internal static void SkipRiffWaveHeader(
            FileStream data, out long dataLength, out AudioFormat? audioFormat)
        {
            audioFormat = null;

            using (var reader = new BinaryReader(data, Encoding.UTF8, true))
            {
                var signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                {
                    data.Position = 0;
                    dataLength = data.Length;
                    return;
                }

                reader.ReadInt32(); // riff_chunck_size

                var wformat = new string(reader.ReadChars(4));
                if (wformat != "WAVE")
                {
                    data.Position = 0;
                    dataLength = data.Length;
                    return;
                }

                // Look for the data chunk.
                while (true)
                {
                    var chunkSignature = new string(reader.ReadChars(4));
                    if (chunkSignature.ToUpperInvariant() == "DATA")
                        break;

                    if (chunkSignature.ToUpperInvariant() == "FMT ")
                    {
                        int fmtLength = reader.ReadInt32();
                        short formatTag = reader.ReadInt16();
                        short channels = reader.ReadInt16();
                        int sampleRate = reader.ReadInt32();
                        int avgBytesPerSec = reader.ReadInt32();
                        short blockAlign = reader.ReadInt16();
                        short bitsPerSample = reader.ReadInt16();
                        audioFormat = new AudioFormat(
                            avgBytesPerSec, bitsPerSample, blockAlign, channels, formatTag, sampleRate);

                        fmtLength -= 2 + 2 + 4 + 4 + 2 + 2;
                        if (fmtLength < 0)
                            throw new InvalidOperationException(
                                "riff wave header has unexpected format");

                        reader.BaseStream.Seek(fmtLength, SeekOrigin.Current);
                    }
                    else
                        reader.BaseStream.Seek(reader.ReadInt32(), SeekOrigin.Current);
                }

                dataLength = reader.ReadInt32();
            }
        }

        public static void WritePcmFile(
            AudioContent content, string saveToFile, int bitRate = 192000, int? sampleRate = null)
        {
            string ffmpegArguments = string.Format(
                "-y -i \"{0}\" -vn -c:a pcm_s16le -b:a {2} {3} -f:a wav " +
                "-strict experimental -threads {4} \"{1}\"",
                content.FileName,
                saveToFile,
                bitRate,
                sampleRate.HasValue ? "-ar " + sampleRate.Value : string.Empty,
                FfmpegThreads);

            var ffmpegExitCode = ExternalTool.Run(
                "ffmpeg",
                ffmpegArguments,
                out var ffmpegStdout,
                out var ffmpegStderr);

            if (ffmpegExitCode != 0)
                throw new PipelineException(
                    "ffmpeg exited with non-zero exit code: \n" + ffmpegStdout + "\n" + ffmpegStderr);
        }

        public static ConversionQuality ConvertToFormat(
            AudioContent content,
            ConversionFormat formatType,
            ConversionQuality quality,
            string? destinationFile)
        {
            if (destinationFile != null)
            {
                destinationFile = Path.GetFullPath(destinationFile);
                if (File.Exists(destinationFile))
                    ExternalTool.DeleteFile(destinationFile);
            }

            string outputFile = destinationFile ?? Path.GetTempFileName();
            FileStream? result = null;
            try
            {
                string ffmpegCodecName, ffmpegMuxerName;
                //int format;
                switch (formatType)
                {
                    case ConversionFormat.Adpcm:
                        // ADPCM Microsoft 
                        ffmpegCodecName = "adpcm_ms";
                        ffmpegMuxerName = "wav";
                        //format = 0x0002; /* WAVE_FORMAT_ADPCM */
                        break;

                    case ConversionFormat.Pcm:
                        // XNA seems to preserve the bit size of the input
                        // format when converting to PCM.
                        if (content.Format.BitsPerSample == 8)
                            ffmpegCodecName = "pcm_u8";
                        else if (content.Format.BitsPerSample == 32 && content.Format.Format == 3)
                            ffmpegCodecName = "pcm_f32le";
                        else
                            ffmpegCodecName = "pcm_s16le";
                        ffmpegMuxerName = "wav";
                        //format = 0x0001; /* WAVE_FORMAT_PCM */
                        break;

                    case ConversionFormat.WindowsMedia:
                        // Windows Media Audio 2
                        ffmpegCodecName = "wmav2";
                        ffmpegMuxerName = "asf";
                        //format = 0x0161; /* WAVE_FORMAT_WMAUDIO2 */
                        break;

                    case ConversionFormat.Xma:
                        throw new NotSupportedException(
                            "XMA is not a supported encoding format. It is specific to the Xbox 360.");

                    case ConversionFormat.ImaAdpcm:
                        // ADPCM IMA WAV
                        ffmpegCodecName = "adpcm_ima_wav";
                        ffmpegMuxerName = "wav";
                        //format = 0x0011; /* WAVE_FORMAT_IMA_ADPCM */
                        break;

                    case ConversionFormat.Aac:
                        // AAC (Advanced Audio Coding), Requires -strict experimental
                        ffmpegCodecName = "aac";
                        ffmpegMuxerName = "ipod";
                        //format = 0x0000; /* WAVE_FORMAT_UNKNOWN */
                        break;

                    case ConversionFormat.Vorbis:
                        ffmpegCodecName = "libvorbis";
                        ffmpegMuxerName = "ogg";
                        //format = 0x0000; /* WAVE_FORMAT_UNKNOWN */
                        break;

                    case ConversionFormat.Mp3:
                        // Vorbis
                        ffmpegCodecName = "libmp3lame";
                        ffmpegMuxerName = "mp3";
                        //format = 0x0000; /* WAVE_FORMAT_UNKNOWN */
                        break;

                    case ConversionFormat.Opus:
                        ffmpegCodecName = "libopus";
                        ffmpegMuxerName = "opus";
                        //format = 0x0000; /* WAVE_FORMAT_UNKNOWN */
                        break;

                    default: // Unknown format
                        throw new NotSupportedException();
                }

                StringBuilder ffmpegStdout, ffmpegStderr;
                int ffmpegExitCode;
                do
                {
                    string ffmpegArguments = string.Format(
                        "-y -i \"{0}\" -vn -c:a {1} -b:a {2} -ar {3} -f:a {4} -strict experimental \"{5}\"",
                        content.FileName,
                        ffmpegCodecName,
                        QualityToBitRate(quality),
                        QualityToSampleRate(formatType, quality, content.Format.SampleRate),
                        ffmpegMuxerName,
                        outputFile);

                    ffmpegExitCode = ExternalTool.Run(
                        "ffmpeg",
                        ffmpegArguments,
                        out ffmpegStdout,
                        out ffmpegStderr,
                        null);

                    if (ffmpegExitCode != 0)
                        quality--;
                } while (quality >= 0 && ffmpegExitCode != 0);

                if (ffmpegExitCode != 0)
                    throw new InvalidOperationException(
                        "ffmpeg exited with non-zero exit code: \n" + ffmpegStdout + "\n" + ffmpegStderr);

                ProbeFormat(
                    outputFile, out AudioFileType audioFileType, out AudioFormat audioFormat,
                    out TimeSpan duration, out int loopStart, out int loopLength);

                result = File.OpenRead(outputFile);
                SkipRiffWaveHeader(result, out long dataLength, out AudioFormat riffAudioFormat);
                if (dataLength > int.MaxValue)
                {
                    result.Dispose();
                    throw new InvalidDataException(
                        "Size of raw audio data exceeded " + int.MaxValue + " bytes.");
                }

                // deal with adpcm
                if (audioFormat.Format == 2 || audioFormat.Format == 17)
                {
                    // riff contains correct blockAlign
                    audioFormat = riffAudioFormat;

                    // fix loopLength -> has to be multiple of sample per block, see:
                    // https://msdn.microsoft.com/de-de/library/windows/desktop/ee415711(v=vs.85).aspx
                    int samplesPerBlock = SampleAlignment(audioFormat);
                    loopLength = (int)(audioFormat.SampleRate * duration.TotalSeconds);
                    int remainder = loopLength % samplesPerBlock;
                    loopLength += samplesPerBlock - remainder;
                }

                var wrap = new DisposeCallbackStream(result);
                wrap.OnDispose += (s, disposing) =>
                {
                    if (destinationFile == null) // we used a tmp path instead
                        ExternalTool.DeleteFile(outputFile); // so delete that tmp file
                };
                content.SetData(wrap, (int)dataLength, audioFormat, duration, loopStart, loopLength);
            }
            catch
            {
                result?.Dispose();

                if (destinationFile == null) // we used a tmp path instead
                    ExternalTool.DeleteFile(outputFile); // so delete that tmp file

                throw;
            }
            return quality;
        }

        /// <summary>
        /// Converts block alignment in bytes to sample alignment, primarily for compressed formats.
        /// Calculation of sample alignment from 
        /// http://kcat.strangesoft.net/openal-extensions/SOFT_block_alignment.txt
        /// </summary>
        static int SampleAlignment(AudioFormat format)
        {
            switch (format.Format)
            {
                // MS-ADPCM
                case 2:
                    return (format.BlockAlign / format.ChannelCount - 7) * 2 + 2;

                // IMA/ADPCM
                case 17:
                    return (format.BlockAlign / format.ChannelCount - 4) / 4 * 8 + 1;

                default:
                    return 0;
            }
        }
    }
}
