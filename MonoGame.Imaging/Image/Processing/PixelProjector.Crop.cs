using System;
using MonoGame.Framework;
using MonoGame.Imaging.Pixels;
using MonoGame.Utilities;
using MonoGame.Utilities.PackedVector;

namespace MonoGame.Imaging.Processing
{
    public static partial class PixelProjector
    {
        #region Crop (IReadOnlyPixelView)

        /// <summary>
        /// Creates a cropped view of the source image.
        /// </summary>
        public static IReadOnlyPixelView<TPixel> Crop<TPixel>(
            this ReadOnlyPixelViewContext<TPixel> context, Rectangle sourceRectangle)
            where TPixel : unmanaged, IPixel
        {
            ImagingArgumentGuard.AssertRectangleInSource(context.View, sourceRectangle, nameof(sourceRectangle));
            if (context.View.GetBounds() == sourceRectangle)
                return context.View;
            return new ReadOnlyViewCrop<TPixel>(context.View, sourceRectangle);
        }

        /// <summary>
        /// Creates a cropped view of the source image.
        /// </summary>
        public static IReadOnlyPixelView<TPixel> Crop<TPixel>(
            this ReadOnlyPixelViewContext<TPixel> context, int x, int y, int width, int height)
            where TPixel : unmanaged, IPixel
        {
            return Crop(context, new Rectangle(x, y, width, height));
        }

        #endregion

        #region Crop (IPixelView)

        /// <summary>
        /// Creates a cropped view of the source image.
        /// </summary>
        public static IPixelView<TPixel> Crop<TPixel>(
            this PixelViewContext<TPixel> context, Rectangle sourceRectangle)
            where TPixel : unmanaged, IPixel
        {
            ImagingArgumentGuard.AssertRectangleInSource(context.View, sourceRectangle, nameof(sourceRectangle));
            if (context.View.GetBounds() == sourceRectangle)
                return context.View;
            return new ViewCrop<TPixel>(context.View, sourceRectangle);
        }

        /// <summary>
        /// Creates a cropped view of the source image.
        /// </summary>
        public static IPixelView<TPixel> Crop<TPixel>(
            this PixelViewContext<TPixel> context, int x, int y, int width, int height)
            where TPixel : unmanaged, IPixel
        {
            return Crop(context, new Rectangle(x, y, width, height));
        }

        #endregion

        private class ReadOnlyViewCrop<TPixel> : IReadOnlyPixelRows<TPixel>
            where TPixel : unmanaged, IPixel
        {
            private IReadOnlyPixelView<TPixel> _view;
            protected Rectangle _rectangle;

            public int Width => _rectangle.Width;
            public int Height => _rectangle.Height;

            public ReadOnlyViewCrop(IReadOnlyPixelView<TPixel> view, Rectangle rectangle)
            {
                _view = view;
                _rectangle = rectangle;
            }

            public TPixel GetPixel(int x, int y)
            {
                x += _rectangle.X;
                y += _rectangle.Y;
                AssertValidCoords(x, y);
                return _view.GetPixel(x + _rectangle.X, y + _rectangle.Y);
            }

            public void GetPixelRow(int x, int y, Span<TPixel> destination)
            {
                int len = Math.Min(destination.Length, _rectangle.Width);
                AssertValidRow(x, len);

                if (_view is IReadOnlyPixelBuffer<TPixel> buffer)
                {
                    var pixelRow = buffer.GetPixelRowSpan(y + _rectangle.Y);
                    pixelRow.Slice(_rectangle.X, Width).Slice(x, len).CopyTo(destination);
                }
                else
                {
                    for (int xx = 0; xx < len; xx++)
                        destination[xx] = _view.GetPixel(x + xx + _rectangle.X, y + _rectangle.Y);
                }
            }

            protected void AssertValidRow(int x, int len)
            {
                CommonArgumentGuard.AssertAtleastZero(x, nameof(x));

                if (x + len > _rectangle.X + Width)
                    throw new ArgumentException(
                        "The requested range is not within the cropped view.");
            }

            protected void AssertValidCoords(int x, int y)
            {
                CommonArgumentGuard.AssertAtleastZero(x, nameof(x));
                if (x >= Width) throw new ArgumentOutOfRangeException(nameof(x));

                CommonArgumentGuard.AssertAtleastZero(y, nameof(y));
                if (y >= Height) throw new ArgumentOutOfRangeException(nameof(y));
            }

            public void Dispose()
            {
            }
        }

        private class ViewCrop<TPixel> : ReadOnlyViewCrop<TPixel>, IPixelRows<TPixel>
            where TPixel : unmanaged, IPixel
        {
            private IPixelView<TPixel> _view;

            public ViewCrop(IPixelView<TPixel> view, Rectangle rectangle) :
                base(view, rectangle)
            {
                _view = view;
            }

            public void SetPixel(int x, int y, TPixel value)
            {
                x += _rectangle.X;
                y += _rectangle.Y;
                AssertValidCoords(x, y);
                _view.SetPixel(x, y, value);
            }

            public void SetPixelRow(int x, int y, Span<TPixel> row)
            {
                int len = Math.Min(row.Length, _rectangle.Width);
                AssertValidRow(x, len);

                if (_view is IReadOnlyPixelBuffer<TPixel> buffer)
                {
                    var pixelRow = buffer.GetPixelRowSpan(y + _rectangle.Y).Slice(0, Width);
                    pixelRow.Slice(x + _rectangle.X, len).CopyTo(row);
                }
                else
                {
                    for (int xx = 0; xx < len; xx++)
                        _view.SetPixel(x + xx + _rectangle.X, y + _rectangle.Y, row[xx]);
                }
            }
        }
    }
}