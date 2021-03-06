// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
//
// Author: Kenneth James Pouncey

using System;
using MonoGame.OpenGL;

namespace MonoGame.Framework.Graphics
{
    public sealed partial class SamplerStateCollection
    {
        private void PlatformSetSamplerState(int index)
        {
        }

        private void PlatformClear()
        {
        }

        private void PlatformDirty()
        {
        }

        internal void PlatformSetSamplers(GraphicsDevice device)
        {
            var samplers = _actualSamplers.AsSpan();
            var textures = device.Textures.GetTexturesSpan().Slice(0, samplers.Length);

            for (int i = 0; i < samplers.Length; i++)
            {
                var sampler = samplers[i];
                if (sampler == null)
                    continue;

                var texture = textures[i];
                if (texture == null)
                    continue;

                if (sampler != texture._glLastSamplerState)
                {
                    // TODO: Avoid doing this redundantly (see TextureCollection.SetTextures())
                    // However, I suspect that rendering from the same texture with different sampling modes
                    // is a relatively rare occurrence...
                    GL.ActiveTexture(TextureUnit.Texture0 + i);
                    GL.CheckError();

                    // NOTE: We don't have to bind the texture here because it is already bound in
                    // TextureCollection.SetTextures(). This, of course, assumes that SetTextures() is called
                    // before this method is called. If that ever changes this code will misbehave.
                    // GL.BindTexture(texture.glTarget, texture.glTexture);
                    // GL.CheckError();

                    sampler.Activate(device, texture._glTarget, texture.LevelCount > 1);
                    texture._glLastSamplerState = sampler;
                }
            }
        }
    }
}
