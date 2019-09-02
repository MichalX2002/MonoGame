// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
    internal partial class EffectResource
    {
        private const string PathRoot =
            nameof(MonoGame) + "." + nameof(Framework) + ".Platform.Graphics.Effect.Resources.";

        const string AlphaTestEffectName = PathRoot + "AlphaTestEffect.dx11.mgfxo";
        const string BasicEffectName = PathRoot + "BasicEffect.dx11.mgfxo";
        const string DualTextureEffectName = PathRoot + "DualTextureEffect.dx11.mgfxo";
        const string EnvironmentMapEffectName = PathRoot + "EnvironmentMapEffect.dx11.mgfxo";
        const string SkinnedEffectName = PathRoot + "SkinnedEffect.dx11.mgfxo";
        const string SpriteEffectName = PathRoot + "SpriteEffect.dx11.mgfxo";
    }
}
