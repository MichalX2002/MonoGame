using System;

namespace MonoGame.Imaging.Processing
{
    public static partial class PixelProcessor
    {
        /// <summary>
        /// Creates a resized version of the source image using the default resampler.
        /// </summary>
        public static Image Resize(
            this ReadOnlyPixelRowsContext context, int width, int height)
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
