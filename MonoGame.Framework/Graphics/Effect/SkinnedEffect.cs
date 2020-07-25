// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.

using System;
using System.Numerics;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Built-in effect for rendering skinned character models.
    /// </summary>
    public class SkinnedEffect : Effect, IEffectMatrices, IEffectLights, IEffectFog
    {
        public const int MaxBones = 72;

        #region Fields

        private EffectParameter _textureParam;
        private EffectParameter _diffuseColorParam;
        private EffectParameter _emissiveColorParam;
        private EffectParameter _specularColorParam;
        private EffectParameter _specularPowerParam;
        private EffectParameter _eyePositionParam;
        private EffectParameter _fogColorParam;
        private EffectParameter _fogVectorParam;
        private EffectParameter _worldParam;
        private EffectParameter _worldInverseTransposeParam;
        private EffectParameter _worldViewProjParam;
        private EffectParameter _bonesParam;

        private bool _preferPerPixelLighting;
        private bool _oneLight;
        private bool _fogEnabled;
        private Matrix4x4 _world = Matrix4x4.Identity;
        private Matrix4x4 _view = Matrix4x4.Identity;
        private Matrix4x4 _projection = Matrix4x4.Identity;
        private Matrix4x4 _worldView;
        private Vector3 _diffuseColor = Vector3.One;
        private Vector3 _emissiveColor = Vector3.Zero;
        private Vector3 _ambientLightColor = Vector3.Zero;
        private float _alpha = 1;
        private float _fogStart;
        private float _fogEnd = 1;
        private int _weightsPerVertex = 4;
        private EffectDirtyFlags _dirtyFlags = EffectDirtyFlags.All;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the world matrix.
        /// </summary>
        public Matrix4x4 World
        {
            get => _world;
            set
            {
                _world = value;
                _dirtyFlags |= EffectDirtyFlags.World | EffectDirtyFlags.WorldViewProj | EffectDirtyFlags.Fog;
            }
        }

        /// <summary>
        /// Gets or sets the view matrix.
        /// </summary>
        public Matrix4x4 View
        {
            get => _view;
            set
            {
                _view = value;
                _dirtyFlags |= EffectDirtyFlags.WorldViewProj | EffectDirtyFlags.EyePosition | EffectDirtyFlags.Fog;
            }
        }
        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        public Matrix4x4 Projection
        {
            get => _projection;
            set
            {
                _projection = value;
                _dirtyFlags |= EffectDirtyFlags.WorldViewProj;
            }
        }

        /// <summary>
        /// Gets or sets the material diffuse color (range 0 to 1).
        /// </summary>
        public Vector3 DiffuseColor
        {
            get => _diffuseColor;
            set
            {
                _diffuseColor = value;
                _dirtyFlags |= EffectDirtyFlags.MaterialColor;
            }
        }

        /// <summary>
        /// Gets or sets the material emissive color (range 0 to 1).
        /// </summary>
        public Vector3 EmissiveColor
        {
            get => _emissiveColor;
            set
            {
                _emissiveColor = value;
                _dirtyFlags |= EffectDirtyFlags.MaterialColor;
            }
        }

        /// <summary>
        /// Gets or sets the material specular color (range 0 to 1).
        /// </summary>
        public Vector3 SpecularColor
        {
            get => _specularColorParam.GetValueVector3();
            set => _specularColorParam.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the material specular power.
        /// </summary>
        public float SpecularPower
        {
            get => _specularPowerParam.GetValueSingle();
            set => _specularPowerParam.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the material alpha.
        /// </summary>
        public float Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value;
                _dirtyFlags |= EffectDirtyFlags.MaterialColor;
            }
        }

        /// <summary>
        /// Gets or sets the per-pixel lighting prefer flag.
        /// </summary>
        public bool PreferPerPixelLighting
        {
            get => _preferPerPixelLighting;
            set
            {
                if (_preferPerPixelLighting != value)
                {
                    _preferPerPixelLighting = value;
                    _dirtyFlags |= EffectDirtyFlags.ShaderIndex;
                }
            }
        }

        /// <summary>
        /// Gets or sets the ambient light color (range 0 to 1).
        /// </summary>
        public Vector3 AmbientLightColor
        {
            get => _ambientLightColor;
            set
            {
                _ambientLightColor = value;
                _dirtyFlags |= EffectDirtyFlags.MaterialColor;
            }
        }

        /// <summary>
        /// Gets the first directional light.
        /// </summary>
        public DirectionalLight DirectionalLight0 { get; private set; }

        /// <summary>
        /// Gets the second directional light.
        /// </summary>
        public DirectionalLight DirectionalLight1 { get; private set; }

        /// <summary>
        /// Gets the third directional light.
        /// </summary>
        public DirectionalLight DirectionalLight2 { get; private set; }

        /// <summary>
        /// Gets or sets the fog enable flag.
        /// </summary>
        public bool FogEnabled
        {
            get => _fogEnabled;
            set
            {
                if (_fogEnabled != value)
                {
                    _fogEnabled = value;
                    _dirtyFlags |= EffectDirtyFlags.ShaderIndex | EffectDirtyFlags.FogEnable;
                }
            }
        }

        /// <summary>
        /// Gets or sets the fog start distance.
        /// </summary>
        public float FogStart
        {
            get => _fogStart;
            set
            {
                _fogStart = value;
                _dirtyFlags |= EffectDirtyFlags.Fog;
            }
        }

        /// <summary>
        /// Gets or sets the fog end distance.
        /// </summary>
        public float FogEnd
        {
            get => _fogEnd;
            set
            {
                _fogEnd = value;
                _dirtyFlags |= EffectDirtyFlags.Fog;
            }
        }

        /// <summary>
        /// Gets or sets the fog color.
        /// </summary>
        public Vector3 FogColor
        {
            get => _fogColorParam.GetValueVector3();
            set => _fogColorParam.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the current texture.
        /// </summary>
        public Texture2D Texture
        {
            get => _textureParam.GetValueTexture2D();
            set => _textureParam.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the number of skinning weights to evaluate for each vertex (1, 2, or 4).
        /// </summary>
        public int WeightsPerVertex
        {
            get => _weightsPerVertex;
            set
            {
                if ((value != 1) &&
                    (value != 2) &&
                    (value != 4))
                    throw new ArgumentOutOfRangeException(nameof(value));

                _weightsPerVertex = value;
                _dirtyFlags |= EffectDirtyFlags.ShaderIndex;
            }
        }

        /// <summary>
        /// Sets an array of skinning bone transform matrices.
        /// </summary>
        public void SetBoneTransforms(Matrix4x4[] boneTransforms)
        {
            if ((boneTransforms == null) || (boneTransforms.Length == 0))
                throw new ArgumentNullException(nameof(boneTransforms));

            if (boneTransforms.Length > MaxBones)
                throw new ArgumentException();

            _bonesParam.SetValue(boneTransforms);
        }

        /// <summary>
        /// Gets a copy of the current skinning bone transform matrices.
        /// </summary>
        public Matrix4x4[] GetBoneTransforms(int count)
        {
            if (count <= 0 || count > MaxBones)
                throw new ArgumentOutOfRangeException(nameof(count));

            Matrix4x4[] bones = _bonesParam.GetValueMatrixArray(count);

            // Convert matrices from 43 to 44 format.
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].M44 = 1;
            }

            return bones;
        }

        /// <summary>
        /// This effect requires lighting, so we explicitly implement
        /// IEffectLights.LightingEnabled, and do not allow turning it off.
        /// </summary>
        bool IEffectLights.LightingEnabled
        {
            get => true;
            set
            {
                if (!value)
                    throw new NotSupportedException(
                        "SkinnedEffect does not support setting LightingEnabled to false.");
            }
        }

        #endregion

        #region Methods


        /// <summary>
        /// Creates a new SkinnedEffect with default parameter settings.
        /// </summary>
        public SkinnedEffect(GraphicsDevice device)
            : base(device, EffectResource.SkinnedEffect.ByteCode)
        {
            CacheEffectParameters(null);

            DirectionalLight0.Enabled = true;

            SpecularColor = Vector3.One;
            SpecularPower = 16;

            var identityBones = new Matrix4x4[MaxBones];

            for (int i = 0; i < MaxBones; i++)
            {
                identityBones[i] = Matrix4x4.Identity;
            }

            SetBoneTransforms(identityBones);
        }

        /// <summary>
        /// Creates a new SkinnedEffect by cloning parameter settings from an existing instance.
        /// </summary>
        protected SkinnedEffect(SkinnedEffect cloneSource)
            : base(cloneSource)
        {
            CacheEffectParameters(cloneSource);

            _preferPerPixelLighting = cloneSource._preferPerPixelLighting;
            _fogEnabled = cloneSource._fogEnabled;

            _world = cloneSource._world;
            _view = cloneSource._view;
            _projection = cloneSource._projection;

            _diffuseColor = cloneSource._diffuseColor;
            _emissiveColor = cloneSource._emissiveColor;
            _ambientLightColor = cloneSource._ambientLightColor;

            _alpha = cloneSource._alpha;

            _fogStart = cloneSource._fogStart;
            _fogEnd = cloneSource._fogEnd;

            _weightsPerVertex = cloneSource._weightsPerVertex;
        }

        /// <summary>
        /// Creates a clone of the current SkinnedEffect instance.
        /// </summary>
        public override Effect Clone()
        {
            return new SkinnedEffect(this);
        }

        /// <summary>
        /// Sets up the standard key/fill/back lighting rig.
        /// </summary>
        public void EnableDefaultLighting()
        {
            AmbientLightColor = EffectHelpers.EnableDefaultLighting(
                DirectionalLight0, DirectionalLight1, DirectionalLight2);
        }

        /// <summary>
        /// Looks up shortcut references to our effect parameters.
        /// </summary>
        private void CacheEffectParameters(SkinnedEffect cloneSource)
        {
            _textureParam = Parameters["Texture"];
            _diffuseColorParam = Parameters["DiffuseColor"];
            _emissiveColorParam = Parameters["EmissiveColor"];
            _specularColorParam = Parameters["SpecularColor"];
            _specularPowerParam = Parameters["SpecularPower"];
            _eyePositionParam = Parameters["EyePosition"];
            _fogColorParam = Parameters["FogColor"];
            _fogVectorParam = Parameters["FogVector"];
            _worldParam = Parameters["World"];
            _worldInverseTransposeParam = Parameters["WorldInverseTranspose"];
            _worldViewProjParam = Parameters["WorldViewProj"];
            _bonesParam = Parameters["Bones"];

            DirectionalLight0 = new DirectionalLight(
                Parameters["DirLight0Direction"],
                Parameters["DirLight0DiffuseColor"],
                Parameters["DirLight0SpecularColor"],
                cloneSource?.DirectionalLight0);

            DirectionalLight1 = new DirectionalLight(
                Parameters["DirLight1Direction"],
                Parameters["DirLight1DiffuseColor"],
                Parameters["DirLight1SpecularColor"],
                cloneSource?.DirectionalLight1);

            DirectionalLight2 = new DirectionalLight(
                Parameters["DirLight2Direction"],
                Parameters["DirLight2DiffuseColor"],
                Parameters["DirLight2SpecularColor"],
                cloneSource?.DirectionalLight2);
        }


        /// <summary>
        /// Lazily computes derived parameter values immediately before applying the effect.
        /// </summary>
        protected internal override void OnApply()
        {
            // Recompute the world+view+projection matrix or fog vector?
            _dirtyFlags = EffectHelpers.SetWorldViewProjAndFog(
                _dirtyFlags, _world, _view, _projection, _worldView,
                _fogEnabled, _fogStart, _fogEnd, _worldViewProjParam, _fogVectorParam);

            // Recompute the world inverse transpose and eye position?
            _dirtyFlags = EffectHelpers.SetLightingMatrices(
                _dirtyFlags, _world, _view, 
                _worldParam, _worldInverseTransposeParam, _eyePositionParam);

            // Recompute the diffuse/emissive/alpha material color parameters?
            if ((_dirtyFlags & EffectDirtyFlags.MaterialColor) != 0)
            {
                EffectHelpers.SetMaterialColor(
                    true, _alpha, _diffuseColor, _emissiveColor, _ambientLightColor, 
                    _diffuseColorParam, _emissiveColorParam);

                _dirtyFlags &= ~EffectDirtyFlags.MaterialColor;
            }

            // Check if we can use the only-bother-with-the-first-light shader optimization.
            bool newOneLight = !DirectionalLight1.Enabled && !DirectionalLight2.Enabled;

            if (_oneLight != newOneLight)
            {
                _oneLight = newOneLight;
                _dirtyFlags |= EffectDirtyFlags.ShaderIndex;
            }

            // Recompute the shader index?
            if ((_dirtyFlags & EffectDirtyFlags.ShaderIndex) != 0)
            {
                int shaderIndex = 0;

                if (!_fogEnabled)
                    shaderIndex += 1;

                if (_weightsPerVertex == 2)
                    shaderIndex += 2;
                else if (_weightsPerVertex == 4)
                    shaderIndex += 4;

                if (_preferPerPixelLighting)
                    shaderIndex += 12;
                else if (_oneLight)
                    shaderIndex += 6;

                _dirtyFlags &= ~EffectDirtyFlags.ShaderIndex;

                CurrentTechnique = Techniques[shaderIndex];
            }
        }


        #endregion
    }
}
