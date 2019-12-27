using System;
using System.Collections.Generic;
using MonoGame.Framework.Collections;

namespace MonoGame.Imaging
{
    public partial class ImageFormat
    {
        private static HashSet<ImageFormat> _builtInFormats = new HashSet<ImageFormat>();

        #region Property Getters

        /// <summary>
        /// Gets the "Portable Network Graphics" format.
        /// </summary>
        public static ImageFormat Png { get; } = AddBuiltIn(
            "Portable Network Graphics", "PNG",
            new[] { "image/png" },
            new[] { ".png" });

        /// <summary> 
        /// Gets the "Joint Photographic Experts Group" (i.e JPEG) format. 
        /// </summary>
        public static ImageFormat Jpeg { get; } = AddBuiltIn(
            "Joint Photographic Experts Group", "JPEG",
            new[] { "image/jpeg" },
            new[] { ".jpeg", ".jpg", ".jpe", ".jfif", ".jif" });

        /// <summary>
        /// Gets the "Graphics Interchange Format".
        /// </summary>
        public static ImageFormat Gif { get; } = new AnimatedImageFormat(
            "Graphics Interchange Format", "GIF",
            new HashSet<string> { "image/gif" }.AsReadOnly(),
            new HashSet<string> { ".gif" }.AsReadOnly(),
            TimeSpan.FromSeconds(0.01));

        /// <summary>
        /// Gets the "Bitmap" format.
        /// </summary>
        public static ImageFormat Bmp { get; } = AddBuiltIn(
            "Bitmap", "BMP",
            new[] { "image/bmp", "image/x-bmp" },
            new[] { ".bmp", ".bm" });

        /// <summary>
        /// Gets the "Truevision Graphics Adapter" format.
        /// </summary>
        public static ImageFormat Tga { get; } = AddBuiltIn(
            "Truevision Graphics Adapter", "TGA",
            new[] { "image/x-tga", "image/x-targa" },
            new[] { ".tga" });

        /// <summary>
        /// Gets the "RGBE" format (also known as "Radiance HDR").
        /// </summary>
        public static ImageFormat Rgbe { get; } = AddBuiltIn(
            "Radiance HDR", "RGBE",
            new[] { "image/vnd.radiance", "image/x-hdr" },
            new[] { ".hdr", ".rgbe" });

        /// <summary>
        /// Gets the "PhotoShop Document" format.
        /// </summary>
        public static ImageFormat Psd { get; } = new LayeredImageFormat(
            "PhotoShop Document", "PSD",
            new HashSet<string> { "image/vnd.adobe.photoshop", "application/x-photoshop" }.AsReadOnly(),
            new HashSet<string> { ".psd" }.AsReadOnly());

        #endregion

        static ImageFormat()
        {
            _builtInFormats.Add(Gif);
            _builtInFormats.Add(Psd);
        }

        /// <summary>
        /// Gets whether the format comes with the imaging library.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsBuiltIn(ImageFormat format)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            return _builtInFormats.Contains(format);
        }

        private static ImageFormat AddBuiltIn(
            string fullName, string name, string[] mimeTypes, string[] extensions)
        {
            var mimeSet = new ReadOnlySet<string>(mimeTypes, StringComparer.OrdinalIgnoreCase);
            var extensionSet = new ReadOnlySet<string>(extensions, StringComparer.OrdinalIgnoreCase);

            var format = new ImageFormat(
                fullName, name, mimeTypes[0], extensions[0], mimeSet, extensionSet);

            _builtInFormats.Add(format);
            AddFormat(format);
            return format;
        }
    }
}
