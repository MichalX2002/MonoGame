using System;
using MonoGame.Framework;
using MonoGame.Framework.Vectors;
using StbSharp;

namespace MonoGame.Imaging.Processing
{
    public static partial class PixelProcessor
    {
        /// <summary>
        /// Creates a resized version of the source image using the default resampler.
        /// </summary>
        public static Image Resize(
            this ReadOnlyPixelRowsContext context, Size size)
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
        public static Image<TPixel> Resize<TPixel>(
            this ReadOnlyPixelRowsContext<TPixel> context, Size size)
            where TPixel : unmanaged, IPixel
        {
            //var inputImg = context.Image;
            //var outputImg = new Image<TPixel>(width, height);
            //
            //StbImageResize.stbir_resize(
            //    inputImg, inputImg.Width, inputImg.Height, inputImg.Stride, outputImg, 
            //    outputImg.Width, outputImg.Height, outputImg.Stride, )

            throw new NotImplementedException();
        }
    }
}
