// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.OpenGL;

namespace MonoGame.Framework.Graphics
{
    public sealed partial class TextureCollection
    {
        private TextureTarget[] _targets;

        private void PlatformInit()
        {
            _targets = new TextureTarget[_textures.Length];
        }

        private void PlatformClear()
        {
            for (int i = 0; i < _targets.Length; i++)
                _targets[i] = 0;
        }

        private void PlatformSetTextures(GraphicsDevice device)
        {
            // Skip out if nothing has changed.
            if (_dirty == 0)
                return;

            for (int i = 0; i < _textures.Length; i++)
            {
                int mask = 1 << i;
                if ((_dirty & mask) == 0)
                    continue;

                GL.ActiveTexture(TextureUnit.Texture0 + i);
                GL.CheckError();

                // Clear the previous binding if the 
                // target is different from the new one.
                var tex = _textures[i];
                if (_targets[i] != 0 && (tex == null || _targets[i] != tex._glTarget))
                {
                    GL.BindTexture(_targets[i], 0);
                    _targets[i] = 0;
                    GL.CheckError();
                }

                if (tex != null)
                {
                    _targets[i] = tex._glTarget;
                    GL.BindTexture(tex._glTarget, tex._glTexture);
                    GL.CheckError();

                    unchecked
                    {
                        _graphicsDevice._graphicsMetrics._textureCount++;
                    }
                }

                _dirty &= ~mask;
                if (_dirty == 0)
                    break;
            }

            _dirty = 0;
        }
    }
}
