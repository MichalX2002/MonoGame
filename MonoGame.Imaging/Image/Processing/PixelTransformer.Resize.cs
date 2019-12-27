using System;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging.Processing
{
    public static partial class PixelTransformer
    {
        /// <summary>
        /// Creates a resized version of the source image using the default resampler.
        /// </summary>
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
