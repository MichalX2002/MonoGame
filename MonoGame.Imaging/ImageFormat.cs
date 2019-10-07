using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MonoGame.Framework;
using MonoGame.Utilities.Collections;

namespace MonoGame.Imaging
{
    [DebuggerDisplay("{ToString(),nq}")]
    public class ImageFormat
    {
        private static HashSet<ImageFormat> _formats;
        private static HashSet<ImageFormat> _builtinFormats;
        private static Dictionary<string, ImageFormat> _byMimeType;
        private static Dictionary<string, ImageFormat> _byExtension;
        
        #region Built-in Formats

        /// <summary>
        /// Gets the "Portable Network Graphics" format.
        /// </summary>
        public static ImageFormat Png { get; }

        /// <summary> 
        /// Gets the "Joint Photographic Experts Group" (i.e JPEG) format. 
        /// </summary>
        public static ImageFormat Jpeg { get; }

        /// <summary>
        /// Gets the "Graphics Interchange Format".
        /// </summary>
        public static ImageFormat Gif { get; }

        /// <summary>
        /// Gets the "Bitmap" format.
        /// </summary>
        public static ImageFormat Bmp { get; }

        /// <summary>
        /// Gets the "Truevision Graphics Adapter" format.
        /// </summary>
        public static ImageFormat Tga { get; }

        /// <summary>
        /// Gets the "Radiance HDR" (also known as "RGBE") format.
        /// </summary>
        public static ImageFormat Hdr { get; }

        /// <summary>
        /// Gets the "PhotoShop Document" format.
        /// </summary>
        public static ImageFormat Psd { get; }

        #endregion

        // TODO: add coder priority so the user can implement
        // an alternative coder in place of an existing one

        #region Static Constructor

        static ImageFormat()
        {
            _formats = new HashSet<ImageFormat>();
            _builtinFormats = new HashSet<ImageFormat>();
            _byExtension = new Dictionary<string, ImageFormat>(StringComparer.OrdinalIgnoreCase);
            _byMimeType = new Dictionary<string, ImageFormat>(StringComparer.OrdinalIgnoreCase);

            Png = AddBuiltIn("Portable Network Graphics", new[] { "image/png" }, new[] { ".png" });
            Jpeg = AddBuiltIn("Joint Photographic Experts Group", new[] { "image/jpeg" }, new[] { ".jpg", ".jpeg", ".jpe", ".jfif", ".jif" });
            Gif = AddBuiltIn("Graphics Interchange Format", new[] { "image/gif" }, new[] { ".gif" }, supportsAnimation: true);
            Bmp = AddBuiltIn("Bitmap", new[] { "image/bmp", "image/x-bmp" }, new[] { ".bmp", ".bm" });
            Tga = AddBuiltIn("Truevision Graphics Adapter", new[] { "image/x-tga", "image/x-targa" }, new[] { ".tga" });
            Hdr = AddBuiltIn("Radiance HDR", new[] { "image/vnd.radiance", "image/x-hdr" }, new[] { ".hdr" });
            Psd = AddBuiltIn("PhotoShop Document", new[] { "image/vnd.adobe.photoshop", "application/x-photoshop" }, new[] { ".psd" });
        }

        private static ImageFormat AddBuiltIn(
            string name, string[] mimeTypes, string[] extensions, bool supportsAnimation = false)
        {
            var mimeSet = new ReadOnlySet<string>(mimeTypes, StringComparer.OrdinalIgnoreCase);
            var extensionSet = new ReadOnlySet<string>(extensions, StringComparer.OrdinalIgnoreCase);
            var format = new ImageFormat(name, mimeTypes[0], extensions[0], supportsAnimation, mimeSet, extensionSet);

            _builtinFormats.Add(format);
            AddFormat(format);
            return format;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the format.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the primary MIME type for the format.
        /// </summary>
        public string MimeType { get; }

        /// <summary>
        /// Gets the primary file extension for the format.
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// Gets whether this format supports animation.
        /// </summary>
        public bool SupportsAnimation { get; }

        /// <summary>
        /// Gets associated MIME types for the format.
        /// </summary>
        [DebuggerDisplay("{DebuggerMimeTypesDisplay,nq}")]
        public ReadOnlySet<string> MimeTypes { get; }

        /// <summary>
        /// Gets associated file extensions for the format.
        /// </summary>
        [DebuggerDisplay("{DebuggerExtensionsDisplay,nq}")]
        public ReadOnlySet<string> Extensions { get; }

        #endregion

        #region Constructor

        public ImageFormat(
            string name, string primaryMimeType, string primaryExtension, bool supportsAnimation,
            ISet<string> mimeTypes, ISet<string> extensions)
        {
            if (mimeTypes == null) throw new ArgumentNullException(nameof(mimeTypes));
            if (mimeTypes.Count == 0) throw new ArgumentEmptyException(nameof(mimeTypes));
            if (!mimeTypes.Contains(primaryMimeType))
                throw new ArgumentException("The set doesn't contain the primary value.", nameof(mimeTypes));

            if (extensions == null) throw new ArgumentNullException(nameof(extensions));
            if (extensions.Count == 0) throw new ArgumentEmptyException(nameof(extensions));
            if (!extensions.Contains(primaryExtension))
                throw new ArgumentException("The set doesn't contain the primary value.", nameof(mimeTypes));

            Name = name ?? throw new ArgumentNullException(nameof(name));
            MimeType = primaryMimeType ?? throw new ArgumentNullException(nameof(primaryMimeType));
            Extension = ExtendExtension(primaryExtension) ?? throw new ArgumentNullException(nameof(primaryExtension));
            SupportsAnimation = supportsAnimation;

            MimeTypes = new ReadOnlySet<string>(mimeTypes, StringComparer.OrdinalIgnoreCase);
            Extensions = new ReadOnlySet<string>(extensions, StringComparer.OrdinalIgnoreCase);
        }

        public ImageFormat(string name, string mimeType, string extension, bool supportsAnimation) : this(
            name, mimeType, extension, supportsAnimation,
            new ReadOnlySet<string>(new[] { mimeType }, null),
            new ReadOnlySet<string>(new[] { extension }, null))
        {
        }

        private static string ExtendExtension(string extension)
        {
            if (extension == null)
                return null;

            if (extension.StartsWith("."))
                return extension;
            return "." + extension;
        }

        #endregion

        public override string ToString()
        {
            return $"{nameof(ImageFormat)}: \"{Name} ({Extension})\"";
        }

        #region Custom Format Methods

        public static void AddFormat(ImageFormat format)
        {
            if (_formats.Contains(format))
                throw new ArgumentException("The format has already been added.", nameof(format));

            _formats.Add(format);
            foreach (var mime in format.MimeTypes)
                _byMimeType.Add(mime.ToLower(), format);
            foreach (var ext in format.Extensions)
                _byExtension.Add(ext.ToLower(), format);
        }

        #endregion

        #region Format Getters

        /// <summary>
        /// Returns whether the format already comes with the imaging library.
        /// </summary>
        public static bool IsBuiltIn(ImageFormat format)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));
            return _builtinFormats.Contains(format);
        }

        public static bool TryGetByMimeType(string mimeType, out ImageFormat format)
        {
            return _byMimeType.TryGetValue(mimeType, out format);
        }

        public static bool TryGetByExtension(string extension, out ImageFormat format)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            if (extension.Length > 0 && !extension.StartsWith("."))
                extension = "." + extension;

            return _byExtension.TryGetValue(extension, out format);
        }

        public static ImageFormat GetByExtension(string extension)
        {
            if (TryGetByExtension(extension, out var format))
                return format;
            throw new KeyNotFoundException(
                $"Image format for extension '{extension}' is not defined.");
        }

        public static bool TryGetByPath(string path, out ImageFormat format)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string extension = Path.GetExtension(path);
            return TryGetByExtension(extension, out format);
        }

        public static ImageFormat GetByPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string extension = Path.GetExtension(path);
            return GetByExtension(extension);
        }

        #endregion
    }
}
