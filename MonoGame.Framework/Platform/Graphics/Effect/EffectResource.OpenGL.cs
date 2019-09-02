// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    internal partial class EffectResource
    {
        private const string PathRoot =
            nameof(MonoGame) + "." + nameof(Framework) + ".Platform.Graphics.Effect.Resources.";

        const string AlphaTestEffectName = PathRoot + "AlphaTestEffect.ogl.mgfxo";
        const string BasicEffectName = PathRoot + "BasicEffect.ogl.mgfxo";
        const string DualTextureEffectName = PathRoot + "DualTextureEffect.ogl.mgfxo";
        const string EnvironmentMapEffectName = PathRoot + "EnvironmentMapEffect.ogl.mgfxo";
        const string SkinnedEffectName = PathRoot + "SkinnedEffect.ogl.mgfxo";
        const string SpriteEffectName = PathRoot + "SpriteEffect.ogl.mgfxo";
    }
}