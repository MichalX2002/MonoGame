using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Imaging
{
    public partial class Image
    {
        #region LoadPixelBuffer<TPixelFrom, TPixelTo>

        public static Image<TPixelTo> LoadPixelBuffer<TPixelFrom, TPixelTo>(
            IReadOnlyPixelBuffer<TPixelFrom> buffer, 
            Rectangle sourceRectangle)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            // TODO: test optimization: replacing Span<>.CopyTo with possibly faster memcpy

            if (buffer == null) throw new ArgumentEmptyException(nameof(buffer));
            ImagingArgumentGuard.AssertNonEmptyRectangle(sourceRectangle, nameof(sourceRectangle));

            var image = new Image<TPixelTo>(sourceRectangle.Width, sourceRectangle.Height);
            if (buffer is IReadOnlyPixelMemory<TPixelTo> typeEqualMemory)
            {
                if (!typeEqualMemory.IsPixelContiguous &&
                    sourceRectangle.Position == Point.Zero &&
                    sourceRectangle.Width == buffer.Width &&
                    sourceRectangle.Height == buffer.Height)
                {
                    typeEqualMemory.GetPixelSpan().CopyTo(image.GetPixelSpan());
                }
                else
                {
                    for (int y = 0; y < sourceRectangle.Height; y++)
                    {
                        var srcRow = typeEqualMemory.GetPixelRowSpan(sourceRectangle.Y + y);
                        srcRow.Slice(sourceRectangle.X, sourceRectangle.Width).CopyTo(image.GetPixelRowSpan(y));
                    }
                }
            }
            else if (buffer is IReadOnlyPixelBuffer<TPixelTo> typeEqualBuffer)
            {
                for (int y = 0; y < sourceRectangle.Height; y++)
                {
                    var srcRow = typeEqualBuffer.GetPixelRowSpan(sourceRectangle.Y + y);
                    srcRow.Slice(sourceRectangle.X, sourceRectangle.Width).CopyTo(image.GetPixelRowSpan(y));
                }
            }
            else
            {
                for (int y = 0; y < sourceRectangle.Height; y++)
                {
                    var srcRow = buffer.GetPixelRowSpan(sourceRectangle.Y + y);
                    var dstRow = image.GetPixelRowSpan(y);
                    for (int x = 0; x < sourceRectangle.Width; x++)
                        dstRow[x].FromScaledVector4(srcRow[x + sourceRectangle.X].ToScaledVector4());
                }
            }
            return image;
        }

        public static Image<TPixelTo> LoadPixelBuffer<TPixelFrom, TPixelTo>(
            IReadOnlyPixelBuffer<TPixelFrom> buffer)
            where TPixelFrom : unmanaged, IPixel
            where TPixelTo : unmanaged, IPixel
        {
            return LoadPixelBuffer<TPixelFrom, TPixelTo>(buffer, buffer.GetBounds());
        }

        #endregion

        #region LoadPixelBuffer<TPixel>

        public static Image<TPixel> LoadPixelBuffer<TPixel>(
            IReadOnlyPixelBuffer<TPixel> buffer, 
            Rectangle sourceRectangle)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixelBuffer<TPixel, TPixel>(buffer, sourceRectangle);
        }

        public static Image<TPixel> LoadPixelBuffer<TPixel>(
            IReadOnlyPixelBuffer<TPixel> buffer)
            where TPixel : unmanaged, IPixel
        {
            return LoadPixelBuffer(buffer, buffer.GetBounds());
        }

        #endregion
    }
}
