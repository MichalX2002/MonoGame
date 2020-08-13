using System;
using System.Runtime.InteropServices;
using MonoGame.Framework;
using MonoGame.Framework.Vectors;
using MonoGame.Imaging.Coders.Decoding;
using MonoGame.Imaging.Pixels;
using StbSharp;

namespace MonoGame.Imaging.Processing
{
    public static partial class PixelProcessor
    {
        // TODO: FIXME:

        /// <summary>
        /// Creates a resized version of the source image using the default resampler.
        /// </summary>
        public static Image Resize(
            this ReadOnlyPixelRowsContext context,
            Size size,
            VectorType? destinationType = null,
            object? state = null,
            ProcessingProgressCallback<object?>? onProgress = null)
        {
            throw new NotImplementedException();

            var outputImage = Image.Create(context.PixelType, size);

            // TODO: FIXME:
            //StbImageResize.stbir_resize(
            //    inputImg, context.Width, context.Height, context.Stride, outputImage, 
            //    outputImage.Width, outputImage.Height, outputImage.ByteStride, StbImageResize.DataType)
            //
            //    return outputImage;
        }

        /// <summary>
        /// Creates a resized version of the source image using the default resampler.
        /// </summary>
        public static Image<TPixel> Resize<TPixel, TState>(
            this ReadOnlyPixelRowsContext context,
            Size size,
            TState state = default,
            ProcessingProgressCallback<TState>? onProgress = null)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            throw new NotImplementedException();

            var outputImage = Image.Create(context.PixelType, size);

            // TODO: FIXME:
            //StbImageResize.stbir_resize(
            //    inputImg, context.Width, context.Height, context.Stride, outputImage, 
            //    outputImage.Width, outputImage.Height, outputImage.ByteStride, StbImageResize.DataType)
            //
            //    return outputImage;
        }

        /// <summary>
        /// Creates a resized version of the source image using the default resampler.
        /// </summary>
        public static Image<TPixel> Resize<TPixel, TState>(
            this ReadOnlyPixelRowsContext<TPixel> context,
            Size size,
            TState state = default,
            ProcessingProgressCallback<TState>? onProgress = null)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            //var inputImg = context.Image;
            //var outputImg = new Image<TPixel>(width, height);
            //
            //StbImageResize.stbir_resize(
            //    inputImg, inputImg.Width, inputImg.Height, inputImg.Stride, outputImg, 
            //    outputImg.Width, outputImg.Height, outputImg.Stride, )

            var source = context.ToImage<Color>();
            var output = Image<Color>.CreateUninitialized(size);

            var progressCallback = onProgress == null ? (StbImageResize.ResizeProgressCallback?)null :
                (p, r) => onProgress!.Invoke(state, p, new Rectangle());

            StbImageResize.Resize(
                MemoryMarshal.AsBytes(source.GetPixelSpan()), source.Width, source.Height, source.ByteStride,
                MemoryMarshal.AsBytes(output.GetPixelSpan()), output.Width, output.Height, output.ByteStride,
                numChannels: 4,
                progressCallback);

            return Image.LoadPixels<TPixel>(output);
        }
    }
}
