using System;
using System.Collections.Generic;
using MonoGame.Imaging.Coders.Decoding;
using MonoGame.Imaging.Coders.Encoding;
using MonoGame.Imaging.Coders.Formats.Bmp;
using MonoGame.Imaging.Coders.Formats.Jpeg;
using MonoGame.Imaging.Coders.Formats.Png;
using MonoGame.Imaging.Coders.Formats.Tga;
using MonoGame.Imaging.Config;

namespace MonoGame.Imaging
{
    public class ImagingConfig : IImagingConfig
    {
        private Dictionary<Type, object> _modules;

        /// <summary>
        /// Gets the default <see cref="ImagingConfig"/>,
        /// often used for methods that don't take it as an argument.
        /// </summary>
        public static ImagingConfig Default { get; } = new ImagingConfig();

        public ImagingConfig()
        {
            _modules = new Dictionary<Type, object>();

            var decoders = new ImageCoderProvider<IImageDecoder>();
            decoders.TryAdd(ImageFormat.Bmp, new BmpImageDecoder());
            decoders.TryAdd(ImageFormat.Png, new PngImageDecoder());
            decoders.TryAdd(ImageFormat.Jpeg, new JpegImageDecoder());

            var encoders = new ImageCoderProvider<IImageEncoder>();
            encoders.TryAdd(ImageFormat.Bmp, new BmpImageEncoder());
            encoders.TryAdd(ImageFormat.Png, new PngImageEncoder());
            encoders.TryAdd(ImageFormat.Jpeg, new JpegImageEncoder());
            encoders.TryAdd(ImageFormat.Tga, new TgaImageEncoder());

            var formatDetectors = new ImageFormatDetectorProvider();
            formatDetectors.TryAdd(ImageFormat.Bmp, new BmpImageFormatDetector());
            formatDetectors.TryAdd(ImageFormat.Png, new PngImageFormatDetector());
            formatDetectors.TryAdd(ImageFormat.Jpeg, new JpegImageFormatDetector());

            _modules.Add(decoders.GetType(), decoders);
            _modules.Add(encoders.GetType(), encoders);
            _modules.Add(formatDetectors.GetType(), formatDetectors);
        }

        public T? GetModule<T>() where T : class
        {
            if (_modules.TryGetValue(typeof(T), out var value))
                return (T)value;
            return default;
        }
    }
}
