// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Content.Pipeline.Processors;

namespace MonoGame.Framework.Content.Pipeline.Graphics
{
    public class EffectMaterialContent : MaterialContent
    {
        public const string EffectKey = "Effect";
        public const string CompiledEffectKey = "CompiledEffect";

        public ExternalReference<EffectContent> Effect
        {
            get => GetReferenceTypeProperty<ExternalReference<EffectContent>>(EffectKey);
            set => SetProperty(EffectKey, value);
        }

        public ExternalReference<CompiledEffectContent> CompiledEffect
        {
            get => GetReferenceTypeProperty<ExternalReference<CompiledEffectContent>>(CompiledEffectKey);
            set => SetProperty(CompiledEffectKey, value);
        }
    }
}
