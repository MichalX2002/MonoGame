﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;

namespace MonoGame.Framework.Content.Pipeline.Audio
{
    /// <summary>
    /// Encapsulates and provides operations, such as format conversions, on the 
    /// source audio. This type is produced by the audio importers and used by audio
    /// processors to produce compiled audio assets.
    /// </summary>
    /// <remarks>
    /// Note that AudioContent can load and process audio files that are not supported by the importers.
    /// </remarks>
    public class AudioContent : ContentItem, IDisposable
    {
        private bool _disposed;
        private readonly AudioFileType _fileType;
        private TimeSpan _duration;
        private AudioFormat _format;
        private int _loopStart;
        private int _loopLength;

        /// <summary>
        /// The name of the original source audio file.
        /// </summary>
        [ContentSerializer(AllowNull = false)]
        public string FileName { get; }

        /// <summary>
        /// The type of the original source audio file.
        /// </summary>
        public AudioFileType FileType => _fileType;

        /// <summary>
        /// The current raw audio data without header information.
        /// </summary>
        public Stream Data { get; private set; }

        /// <summary>
        /// The amount of audio data bytes in the <see cref="Data"/> stream.
        /// </summary>
        public int DataLength { get; private set; }

        /// <summary>
        /// The duration of the audio data.
        /// </summary>
        public TimeSpan Duration => _duration;

        /// <summary>
        /// The current format of the audio data.
        /// </summary>
        /// <remarks>This changes from the source format to the output format after conversion.</remarks>
        public AudioFormat Format => _format;

        /// <summary>
        /// The current loop length in samples.
        /// </summary>
        /// <remarks>This changes from the source loop length to the output loop length after conversion.</remarks>
        public int LoopLength => _loopLength;

        /// <summary>
        /// The current loop start location in samples.
        /// </summary>
        /// <remarks>This changes from the source loop start to the output loop start after conversion.</remarks>
        public int LoopStart => _loopStart;

        /// <summary>
        /// Initializes a new instance of AudioContent.
        /// </summary>
        /// <param name="audioFileName">Name of the audio source file to be processed.</param>
        /// <param name="audioFileType">Type of the processed audio: WAV, MP3 or WMA.</param>
        /// <remarks>Constructs the object from the specified source file, in the format specified.</remarks>
        public AudioContent(string audioFileName, AudioFileType audioFileType)
        {
            FileName = audioFileName;

            try
            {
                // Get the full path to the file.
                audioFileName = Path.GetFullPath(audioFileName);

                // Use probe to get the details of the file.
                DefaultAudioProfile.ProbeFormat(
                    audioFileName, out _fileType, out _format, out _duration, out _loopStart, out _loopLength);

                // Looks like XNA only cares about type mismatch when
                // the type is WAV... else it is ok.
                if ((audioFileType == AudioFileType.Wav || _fileType == AudioFileType.Wav) &&
                    audioFileType != _fileType)
                    throw new ArgumentException("Incorrect file type!", nameof(audioFileType));

                //// Only provide the data for WAV files.
                //if (audioFileType == AudioFileType.Wav)
                //{
                //    byte[] rawData;
                //
                //    // Must be opened in read mode otherwise it fails to open
                //    // read-only files (found in some source control systems)
                //    using (var fs = new FileStream(audioFileName, FileMode.Open, FileAccess.Read))
                //    {
                //        rawData = new byte[fs.Length];
                //        fs.Read(rawData, 0, rawData.Length);
                //    }
                //
                //    var stripped = DefaultAudioProfile.StripRiffWaveHeader(rawData, out AudioFormat riffAudioFormat);
                //
                //    if (riffAudioFormat != null)
                //    {
                //        if ((_format.Format != 2 && _format.Format != 17) && _format.BlockAlign != riffAudioFormat.BlockAlign)
                //            throw new InvalidOperationException("Calculated block align does not match RIFF " + _format.BlockAlign + " : " + riffAudioFormat.BlockAlign);
                //        if (_format.ChannelCount != riffAudioFormat.ChannelCount)
                //            throw new InvalidOperationException("Probed channel count does not match RIFF: " + _format.ChannelCount + ", " + riffAudioFormat.ChannelCount);
                //        if (_format.Format != riffAudioFormat.Format)
                //            throw new InvalidOperationException("Probed audio format does not match RIFF: " + _format.Format + ", " + riffAudioFormat.Format);
                //        if (_format.SampleRate != riffAudioFormat.SampleRate)
                //            throw new InvalidOperationException("Probed sample rate does not match RIFF: " + _format.SampleRate + ", " + riffAudioFormat.SampleRate);
                //    }
                //
                //    _data = stripped;
                //}
            }
            catch (Exception ex)
            {
                var message = string.Format(
                    "Failed to open file {0}. Ensure the file is a valid audio file and is not DRM protected.",
                    Path.GetFileNameWithoutExtension(audioFileName));
                throw new InvalidContentException(message, ex);
            }
        }

        public void SetData(
            Stream data, int dataLength, AudioFormat format, TimeSpan duration, int loopStart, int loopLength)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            _format = format ?? throw new ArgumentNullException(nameof(format));
            DataLength = dataLength;
            _duration = duration;
            _loopStart = loopStart;
            _loopLength = loopLength;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Data?.Dispose();
                Data = null;

                DataLength = -1;
                _disposed = true;
            }
        }
    }
}
