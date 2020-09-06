using System;

namespace MonoGame.Framework.Graphics
{
    public class NinePatchRegion2D : TextureRegion2D
    {
        public const int TopLeft = 0;
        public const int TopMiddle = 1;
        public const int TopRight = 2;
        public const int MiddleLeft = 3;
        public const int Middle = 4;
        public const int MiddleRight = 5;
        public const int BottomLeft = 6;
        public const int BottomMiddle = 7;
        public const int BottomRight = 8;

        public RectangleF[] SourcePatches { get; } = new RectangleF[9];
        public ThicknessF Padding { get; }
        public float LeftPadding => Padding.Left;
        public float TopPadding => Padding.Top;
        public float RightPadding => Padding.Right;
        public float BottomPadding => Padding.Bottom;

        public NinePatchRegion2D(TextureRegion2D textureRegion, ThicknessF padding)
            : base(
                textureRegion?.Texture, textureRegion?.Name,
                textureRegion?.X ?? 0, textureRegion?.Y ?? 0, textureRegion?.Width ?? 0, textureRegion?.Height ?? 0)
        {
            Padding = padding;
            CreatePatches(textureRegion!.Bounds, SourcePatches);
        }

        public NinePatchRegion2D(
            TextureRegion2D textureRegion, float padding)
            : this(textureRegion, padding, padding, padding, padding)
        {
        }

        public NinePatchRegion2D(
            TextureRegion2D textureRegion, float leftRightPadding, float topBottomPadding)
            : this(textureRegion, leftRightPadding, topBottomPadding, leftRightPadding, topBottomPadding)
        {
        }

        public NinePatchRegion2D(
            TextureRegion2D textureRegion, float leftPadding, float topPadding, float rightPadding, float bottomPadding)
            : this(textureRegion, new ThicknessF(leftPadding, topPadding, rightPadding, bottomPadding))
        {
        }

        public NinePatchRegion2D(Texture2D texture, ThicknessF thickness)
            : this(new TextureRegion2D(texture), thickness)
        {
        }

        public void CreatePatches(RectangleF sourceRectangle, Span<RectangleF> destination)
        {
            float x = sourceRectangle.X;
            float y = sourceRectangle.Y;
            float w = sourceRectangle.Width;
            float h = sourceRectangle.Height;
            float middleWidth = w - LeftPadding - RightPadding;
            float middleHeight = h - TopPadding - BottomPadding;
            float bottomY = y + h - BottomPadding;
            float rightX = x + w - RightPadding;
            float leftX = x + LeftPadding;
            float topY = y + TopPadding;

            destination[TopLeft] = new RectangleF(x, y, LeftPadding, TopPadding);
            destination[TopMiddle] = new RectangleF(leftX, y, middleWidth, TopPadding);
            destination[TopRight] = new RectangleF(rightX, y, RightPadding, TopPadding);
            destination[MiddleLeft] = new RectangleF(x, topY, LeftPadding, middleHeight);
            destination[Middle] = new RectangleF(leftX, topY, middleWidth, middleHeight);
            destination[MiddleRight] = new RectangleF(rightX, topY, RightPadding, middleHeight);
            destination[BottomLeft] = new RectangleF(x, bottomY, LeftPadding, BottomPadding);
            destination[BottomMiddle] = new RectangleF(leftX, bottomY, middleWidth, BottomPadding);
            destination[BottomRight] = new RectangleF(rightX, bottomY, RightPadding, BottomPadding);
        }

        public RectangleF[] CreatePatches(RectangleF sourceRectangle)
        {
            var destinationPatches = new RectangleF[9];
            CreatePatches(sourceRectangle, destinationPatches);
            return destinationPatches;
        }
    }
}