// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Graphics
{
    public static class SpriteBatchExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Draw(
            this ISpriteBatch spriteBatch,
            Texture2D texture,
            in VertexPositionColorTexture vertexTL,
            in VertexPositionColorTexture vertexTR,
            in VertexPositionColorTexture vertexBL,
            in VertexPositionColorTexture vertexBR,
            float sortDepth)
        {
            float sortKey = spriteBatch.GetSortKey(texture, sortDepth);
            ref var quad = ref spriteBatch.GetBatchQuad(texture, sortKey);
            quad.VertexTL = vertexTL;
            quad.VertexTR = vertexTR;
            quad.VertexBL = vertexBL;
            quad.VertexBR = vertexBR;
            spriteBatch.FlushIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Draw(
            this ISpriteBatch spriteBatch,
            Texture2D texture,
            in VertexPositionColorTexture vertexTL,
            in VertexPositionColorTexture vertexTR,
            in VertexPositionColorTexture vertexBL,
            in VertexPositionColorTexture vertexBR)
        {
            spriteBatch.Draw(texture, vertexTL, vertexTR, vertexBL, vertexBR, vertexTL.Position.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Draw(
            this ISpriteBatch spriteBatch, Texture2D texture, in SpriteQuad quad, float sortDepth)
        {
            float sortKey = spriteBatch.GetSortKey(texture, sortDepth);
            spriteBatch.GetBatchQuad(texture, sortKey) = quad;
            spriteBatch.FlushIfNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Draw(
            this ISpriteBatch spriteBatch, Texture2D texture, in SpriteQuad quad)
        {
            spriteBatch.Draw(texture, quad, quad.VertexTL.Position.Z);
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The batch to draw the sprite to.</param>
        /// <param name="texture">A texture.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>
        /// <param name="sourceRectangle">
        /// An optional region on the texture which will be rendered, drawing full texture otherwise.
        /// </param>
        /// <param name="color">A color mask.</param>
        /// <param name="rotation">A rotation of this sprite.</param>
        /// <param name="origin">Center of the rotation. 0,0 by default.</param>
        /// <param name="flip">Sprite mirroring flags. Can be combined.</param>
        /// <param name="sortDepth">A depth of the layer of this sprite.</param>
        public static void Draw(
            this ISpriteBatch spriteBatch,
            Texture2D texture,
            RectangleF destinationRectangle,
            RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            SpriteFlip flip,
            float sortDepth)
        {
            var texelSize = texture.TexelSize;
            Vector4 texCoord;

            if (sourceRectangle.HasValue)
            {
                RectangleF srcRect = sourceRectangle.GetValueOrDefault();
                texCoord = SpriteQuad.GetTexCoord(texelSize, srcRect);
                origin = SpriteQuad.RemapOrigin(origin, texelSize, destinationRectangle, srcRect);
            }
            else
            {
                texCoord = new Vector4(0, 0, 1, 1);
                origin = SpriteQuad.RemapOrigin(origin, texelSize, destinationRectangle);
            }
            SpriteQuad.FlipTexCoords(ref texCoord, flip);

            float sortKey = spriteBatch.GetSortKey(texture, sortDepth);
            ref var quad = ref spriteBatch.GetBatchQuad(texture, sortKey);
            if (rotation == 0f)
            {
                quad.Set(
                    destinationRectangle.X - origin.X, destinationRectangle.Y - origin.Y,
                    destinationRectangle.Width, destinationRectangle.Height,
                    color, texCoord, sortDepth);
            }
            else
            {
                quad.Set(
                    destinationRectangle.X, destinationRectangle.Y, -origin.X, -origin.Y,
                    destinationRectangle.Width, destinationRectangle.Height,
                    MathF.Sin(rotation), MathF.Cos(rotation),
                    color, texCoord, sortDepth);
            }

            spriteBatch.FlushIfNeeded();
        }
    }
}