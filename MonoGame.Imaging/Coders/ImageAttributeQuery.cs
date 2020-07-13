using System;
using MonoGame.Imaging.Attributes;

namespace MonoGame.Imaging.Coders
{
    /// <summary>
    /// Helper for identifying attributes on image formats and coders.
    /// </summary>
    public readonly struct ImageAttributeQuery
    {
        /// <summary>
        /// Gets the object which was queried.
        /// </summary>
        public IImageCoderAttribute Source { get; }

        /// <summary>
        /// Gets the image format 
        /// which could be the queried object, <see cref="IImageCoder.Format"/>, or <see langword="null"/>.
        /// </summary>
        public ImageFormat? Format { get; }

        /// <summary>
        /// Gets whether <see cref="Source"/> is <see langword="null"/>.
        /// </summary>
        public bool IsEmpty => Source == null;

        /// <summary>
        /// Gets whether <see cref="Source"/> is an <see cref="IImageFormatAttribute"/>.
        /// </summary>
        public bool IsFormatAttribute => Source is IImageFormatAttribute;

        /// <summary>
        /// Gets whether <see cref="Source"/> is an <see cref="IImageCoder"/>.
        /// </summary>
        public bool IsCoder => Source is IImageCoder;

        /// <summary>
        /// Gets whether <see cref="Source"/> is an <see cref="ImageFormat"/>.
        /// </summary>
        public bool IsFormat => Source is ImageFormat;

        public ImageAttributeQuery(IImageCoderAttribute source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Format =
                source is ImageFormat format ? format :
                source is IImageCoder coder ? coder.Format :
                null;
        }
    }
}
