// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Content.Pipeline.Graphics
{
    public class BasicMaterialContent : MaterialContent
    {
        public const string AlphaKey = "Alpha";
        public const string DiffuseColorKey = "DiffuseColor";
        public const string EmissiveColorKey = "EmissiveColor";
        public const string SpecularColorKey = "SpecularColor";
        public const string SpecularPowerKey = "SpecularPower";
        public const string TextureKey = "Texture";
        public const string VertexColorEnabledKey = "VertexColorEnabled";

        public float? Alpha
        {
            get => GetValueTypeProperty<float>(AlphaKey);
            set => SetProperty(AlphaKey, value);
        }

        public Vector3? DiffuseColor
        {
            get => GetValueTypeProperty<Vector3>(DiffuseColorKey);
            set => SetProperty(DiffuseColorKey, value);
        }

        public Vector3? EmissiveColor
        {
            get => GetValueTypeProperty<Vector3>(EmissiveColorKey);
            set => SetProperty(EmissiveColorKey, value);
        }

        public Vector3? SpecularColor
        {
            get => GetValueTypeProperty<Vector3>(SpecularColorKey);
            set => SetProperty(SpecularColorKey, value);
        }

        public float? SpecularPower
        {
            get => GetValueTypeProperty<float>(SpecularPowerKey);
            set => SetProperty(SpecularPowerKey, value);
        }

        public ExternalReference<TextureContent> Texture
        {
            get => GetTexture(TextureKey);
            set => SetTexture(TextureKey, value);
        }

        public bool? VertexColorEnabled
        {
            get => GetValueTypeProperty<bool>(VertexColorEnabledKey);
            set => SetProperty(VertexColorEnabledKey, value);
        }
    }
}
