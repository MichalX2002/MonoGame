using System;
using System.Numerics;

namespace MonoGame.Framework.Graphics
{
    public static class TextureRegionExtensions
    {
        public static void Draw(
            this SpriteBatch spriteBatch,
            TextureRegion2D textureRegion,
            Vector2 position,
            Color color,
            RectangleF? clippingRectangle = null)
        {
            Draw(spriteBatch, textureRegion, position, color, 0, Vector2.Zero, Vector2.One, SpriteFlip.None, 0, clippingRectangle);
        }

        public static void Draw(
            this SpriteBatch spriteBatch,
            TextureRegion2D textureRegion,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            SpriteFlip flip,
            float layerDepth,
            RectangleF? clippingRectangle = null)
        {
            if (textureRegion.Bounds.IsVisible(ref position, origin, scale, clippingRectangle, out RectangleF srcRect))
            {
                //System.Console.WriteLine(srcRect);
                spriteBatch.Draw(textureRegion.Texture, position, srcRect, color, rotation, origin, scale, flip, layerDepth);
            }
        }

        public static bool IsVisible(
            this RectangleF sourceRect,
            ref Vector2 position,
            Vector2 origin,
            Vector2 scale,
            RectangleF? clipRect,
            out RectangleF clippedRect)
        {
            if (!clipRect.HasValue)
            {
                clippedRect = sourceRect;
                return true;
            }

            float x = position.X - origin.X * scale.X;
            float y = position.Y - origin.Y * scale.Y;
            float width = sourceRect.Width * scale.X;
            float height = sourceRect.Height * scale.Y;
            var dstRect = new RectangleF(x, y, width, height);

            clippedRect = ClipSourceRectangle(sourceRect, dstRect, clipRect.Value);
            position.X += (clippedRect.X - sourceRect.X) * scale.X;
            position.Y += (clippedRect.Y - sourceRect.Y) * scale.Y;

            return clippedRect.Width > 0f && clippedRect.Height > 0f;
        }

        public static void Draw(
            this SpriteBatch spriteBatch,
            TextureRegion2D textureRegion,
            RectangleF destinationRectangle,
            Color color,
            RectangleF? clippingRectangle = null)
        {
            if (textureRegion == null)
                throw new ArgumentNullException(nameof(textureRegion));

            if (textureRegion is NinePatchRegion2D ninePatchRegion)
                Draw(spriteBatch, ninePatchRegion, destinationRectangle, color, clippingRectangle);
            else
                Draw(spriteBatch, textureRegion.Texture, textureRegion.Bounds, destinationRectangle, color, clippingRectangle);
        }

        public static void Draw(
            this SpriteBatch spriteBatch,
            NinePatchRegion2D ninePatchRegion,
            RectangleF destinationRectangle,
            Color color,
            RectangleF? clippingRectangle = null)
        {
            var destinationPatches = ninePatchRegion.CreatePatches(destinationRectangle);
            var sourcePatches = ninePatchRegion.SourcePatches;

            for (var i = 0; i < sourcePatches.Length; i++)
            {
                var sourcePatch = sourcePatches[i];
                var destinationPatch = destinationPatches[i];

                if (clippingRectangle.HasValue)
                {
                    sourcePatch = ClipSourceRectangle(sourcePatch, destinationPatch, clippingRectangle.Value).ToRectangle();
                    destinationPatch = ClipDestinationRectangle(destinationPatch, clippingRectangle.Value).ToRectangle();
                    Draw(spriteBatch, ninePatchRegion.Texture, sourcePatch, destinationPatch, color, clippingRectangle);
                }
                else
                {
                    if (destinationPatch.Width > 0 && destinationPatch.Height > 0)
                        spriteBatch.Draw(ninePatchRegion.Texture, sourcePatch, destinationPatch, color);
                }
            }
        }

        public static void Draw(
            this SpriteBatch spriteBatch,
            Texture2D texture,
            RectangleF sourceRectangle,
            RectangleF destinationRectangle,
            Color color,
            RectangleF? clippingRectangle)
        {
            if (clippingRectangle.HasValue)
            {
                sourceRectangle = ClipSourceRectangle(sourceRectangle, destinationRectangle, clippingRectangle.Value);
                destinationRectangle = ClipDestinationRectangle(destinationRectangle, clippingRectangle.Value);
            }

            if (destinationRectangle.Width > 0 && destinationRectangle.Height > 0)
                spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
        }

        private static RectangleF ClipSourceRectangle(
            RectangleF sourceRectangle, RectangleF destinationRectangle, RectangleF clippingRectangle)
        {
            float left = clippingRectangle.Left - destinationRectangle.Left;
            float right = destinationRectangle.Right - clippingRectangle.Right;
            float top = clippingRectangle.Top - destinationRectangle.Top;
            float bottom = destinationRectangle.Bottom - clippingRectangle.Bottom;
            float x = left > 0 ? left : 0;
            float y = top > 0 ? top : 0;
            float w = (right > 0 ? right : 0) + x;
            float h = (bottom > 0 ? bottom : 0) + y;

            float scaleX = destinationRectangle.Width / sourceRectangle.Width;
            float scaleY = destinationRectangle.Height / sourceRectangle.Height;
            x /= scaleX;
            y /= scaleY;
            w /= scaleX;
            h /= scaleY;

            return new RectangleF(
                sourceRectangle.X + x,
                sourceRectangle.Y + y,
                sourceRectangle.Width - w,
                sourceRectangle.Height - h);
        }

        private static RectangleF ClipDestinationRectangle(
            RectangleF destinationRectangle, RectangleF clippingRectangle)
        {
            float left = clippingRectangle.Left < destinationRectangle.Left ? destinationRectangle.Left : clippingRectangle.Left;
            float top = clippingRectangle.Top < destinationRectangle.Top ? destinationRectangle.Top : clippingRectangle.Top;
            float bottom = clippingRectangle.Bottom < destinationRectangle.Bottom ? clippingRectangle.Bottom : destinationRectangle.Bottom;
            float right = clippingRectangle.Right < destinationRectangle.Right ? clippingRectangle.Right : destinationRectangle.Right;
            return new RectangleF(left, top, right - left, bottom - top);
        }
    }
}