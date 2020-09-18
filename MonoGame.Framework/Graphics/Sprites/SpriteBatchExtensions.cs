// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Runtime.CompilerServices;

namespace MonoGame.Framework.Graphics
{
    public static class SpriteBatchExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Draw(
            ISpriteBatch spriteBatch,
            Texture2D texture,
            in VertexPositionColorTexture vertexTL,
            in VertexPositionColorTexture vertexTR,
            in VertexPositionColorTexture vertexBL,
            in VertexPositionColorTexture vertexBR)
        {
            spriteBatch.Draw(texture, vertexTL, vertexTR, vertexBL, vertexBR, vertexTL.Position.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Draw(ISpriteBatch spriteBatch, Texture2D texture, in SpriteQuad quad)
        {
            spriteBatch.Draw(texture, quad, quad.VertexTL.Position.Z);
        }
    }
}