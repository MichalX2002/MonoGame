using System;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Processing
{
    public static partial class PixelProcessor
    {
        /// <summary>
        /// Creates a resized version of the source image using default resampler.
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="context"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Image<TPixel> Resize<TPixel>(
            this ReadOnlyPixelViewContext<TPixel> context, int width, int height)
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
