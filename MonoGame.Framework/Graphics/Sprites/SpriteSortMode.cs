// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Defines sprite sort rendering options.
    /// </summary>
    public enum SpriteSortMode
    {
        /// <summary>
        /// All sprites are drawing when <see cref="SpriteBatch.End"/> invokes, in order of draw call sequence and depth.
        /// </summary>
        Deferred,

        /// <summary>
        /// Each sprite is drawing at individual draw call, instead of <see cref="SpriteBatch.End"/>.
        /// </summary>
        Immediate,

        /// <summary>
        /// Same as <see cref="Deferred"/>, except sprites are sorted by texture prior to drawing.
        /// </summary>
        Texture,

        /// <summary>
        /// Same as <see cref="Deferred"/>, except sprites are sorted by depth in back-to-front order prior to drawing.
        /// </summary>
        BackToFront,

        /// <summary>
        /// Same as <see cref="Deferred"/>, except sprites are sorted by depth in front-to-back order prior to drawing.
        /// </summary>
        FrontToBack
    }
}