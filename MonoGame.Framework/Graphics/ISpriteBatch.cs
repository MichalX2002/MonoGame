// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    public interface ISpriteBatch
    {
        /// <summary>
        /// Gets a value used to sort a sprite, 
        /// based on the current sort mode and the given parameters.
        /// </summary>
        float GetSortKey(Texture2D texture, float sortDepth);

        /// <summary>
        /// Get a unique <see cref="SpriteQuad"/> reference from the batch.
        /// The reference shall remain unique as long as the batch is not flushed.
        /// </summary>
        ref SpriteQuad GetBatchQuad(Texture2D texture, float sortKey);

        bool FlushIfNeeded();
    }
}