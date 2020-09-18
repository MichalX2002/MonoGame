// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    public interface ISpriteBatch
    {
        void Draw(
            Texture2D texture,
            in VertexPositionColorTexture vertexTL,
            in VertexPositionColorTexture vertexTR,
            in VertexPositionColorTexture vertexBL,
            in VertexPositionColorTexture vertexBR,
            float sortDepth);

        void Draw(Texture2D texture, in SpriteQuad quad, float sortDepth);
    }
}