// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.IO;
using MonoGame.Framework.Content.Pipeline.Audio;
using MonoGame.Framework.Content.Pipeline.Builder;
using MonoGame.Framework.Media;

namespace MonoGame.Framework.Content.Pipeline.Processors
{
    /// <summary>
    /// Custom song processor that processes an intermediate <see cref="AudioContent"/> type. 
    /// This type encapsulates the source audio content,
    /// producing a <see cref="Song"/> type that can be used in the game.
    /// </summary>
    [ContentProcessor(DisplayName = "Song - MonoGame")]
    public class SongProcessor : ContentProcessor<AudioContent, SongContent>
    {
        /// <summary>
        /// Gets or sets the target format quality of the audio content.
        /// </summary>
        /// <value>The ConversionQuality of this audio data.</value>
        public ConversionQuality Quality { get; set; } = ConversionQuality.Best;

        /// <summary>
        /// Initializes a new instance of SongProcessor.
        /// </summary>
        public SongProcessor()
        {
        }

        /// <summary>
        /// Builds the content for the source audio.
        /// </summary>
        /// <param name="input">The audio content to build.</param>
        /// <param name="context">Context for the specified processor.</param>
        /// <returns>The built audio.</returns>
        public override SongContent Process(AudioContent input, ContentProcessorContext context)
        {
            // The xnb name is the basis for the final song filename.
            var songInputFile = context.OutputFilename;

            // Convert and write out the song media file.
            var profile = AudioProfile.ForPlatform(context.TargetPlatform);
            var finalQuality = profile.ConvertStreamingAudio(
                context.TargetPlatform, Quality, input, songInputFile, out string songOutFile);

            // Let the pipeline know about the song file so it can clean things up.
            context.AddOutputFile(songOutFile);

            if (Quality != finalQuality)
                context.Logger.LogMessage(
                    "Failed to convert using \"{0}\" quality, used \"{1}\" quality", Quality, finalQuality);

            // Return the XNB song content.
            var dirPath = Path.GetDirectoryName(context.OutputFilename) + Path.DirectorySeparatorChar;
            var filePath = PathHelper.GetRelativePath(dirPath, songOutFile);
            return new SongContent(filePath, input.Duration);
        }
    }
}
