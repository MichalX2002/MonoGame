// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Audio;
using MonoGame.Framework.Content.Pipeline.Audio;

namespace MonoGame.Framework.Content.Pipeline.Processors
{
    /// <summary>
    /// A sound effect processor that processes an intermediate <see cref="AudioContent"/> type. 
    /// This type encapsulates the source audio content, 
    /// producing a <see cref="SoundEffect"/> type that can be used in the game.
    /// </summary>
    [ContentProcessor(DisplayName = "Sound Effect - MonoGame")]
    public class SoundEffectProcessor : ContentProcessor<AudioContent, SoundEffectContent>
    {
        /// <summary>
        /// Gets or sets the target format quality of the audio content.
        /// </summary>
        /// <value>The ConversionQuality of this audio data.</value>
        public ConversionQuality Quality { get; set; } = ConversionQuality.Best;

        /// <summary>
        /// Initializes a new instance of SoundEffectProcessor.
        /// </summary>
        public SoundEffectProcessor()
        {
        }

        /// <summary>
        /// Builds the content for the source audio.
        /// </summary>
        /// <param name="input">The audio content to build.</param>
        /// <param name="context">Context for the specified processor.</param>
        /// <returns>The built audio.</returns>
        public override SoundEffectContent Process(AudioContent input, ContentProcessorContext context)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var profile = AudioProfile.ForPlatform(context.TargetPlatform);
            var finalQuality = profile.ConvertAudio(
                context.TargetPlatform, Quality, input, context.Logger);

            if (Quality != finalQuality)
                context.Logger.LogMessage(
                    "Failed to convert using \"{0}\" quality, used \"{1}\" quality",
                    Quality,
                    finalQuality);

            return new SoundEffectContent(
                input.Format.NativeWaveFormat,
                input.Data,
                input.DataLength,
                input.LoopStart,
                input.LoopLength,
                input.Duration.TotalMilliseconds);
        }
    }
}
