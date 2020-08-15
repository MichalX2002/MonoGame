// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.

using System.Numerics;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Built-in effect that supports optional texturing, vertex coloring, fog, and lighting.
    /// </summary>
    public class BasicEffect : Effect, IEffectMatrices, IEffectLights, IEffectFog
    {
        #region Effect Parameters

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

        #endregion

        #region Fields

        private bool _lightingEnabled;
        private bool _preferPerPixelLighting;
        private bool _oneLight;

        private Matrix4x4 _world = Matrix4x4.Identity;
        private Matrix4x4 _view = Matrix4x4.Identity;
        private Matrix4x4 _projection = Matrix4x4.Identity;
        private Matrix4x4 _worldView;

        private bool _textureEnabled;
        private bool _vertexColorEnabled;
        private Vector3 _diffuseColor = Vector3.One;
        private Vector3 _emissiveColor = Vector3.Zero;
        private Vector3 _ambientLightColor = Vector3.Zero;
        private float _alpha = 1;

        private bool _fogEnabled;
        private float _fogStart;
        private float _fogEnd = 1;

        private EffectDirtyFlags dirtyFlags = EffectDirtyFlags.All;

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
                dirtyFlags |= EffectDirtyFlags.World | EffectDirtyFlags.WorldViewProj | EffectDirtyFlags.Fog;
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
                dirtyFlags |= EffectDirtyFlags.WorldViewProj | EffectDirtyFlags.EyePosition | EffectDirtyFlags.Fog;
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
                dirtyFlags |= EffectDirtyFlags.WorldViewProj;
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
                dirtyFlags |= EffectDirtyFlags.MaterialColor;
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
                dirtyFlags |= EffectDirtyFlags.MaterialColor;
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
                dirtyFlags |= EffectDirtyFlags.MaterialColor;
            }
        }

        /// <inheritdoc/>
        public bool LightingEnabled
        {
            get => _lightingEnabled;
            set
            {
                if (_lightingEnabled != value)
                {
                    _lightingEnabled = value;
                    dirtyFlags |= EffectDirtyFlags.ShaderIndex | EffectDirtyFlags.MaterialColor;
                }
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
                    dirtyFlags |= EffectDirtyFlags.ShaderIndex;
                }
            }
        }

        /// <inheritdoc/>
        public Vector3 AmbientLightColor
        {
            get => _ambientLightColor;

            set
            {
                _ambientLightColor = value;
                dirtyFlags |= EffectDirtyFlags.MaterialColor;
            }
        }

        /// <inheritdoc/>
        public DirectionalLight DirectionalLight0 { get; private set; }

        /// <inheritdoc/>
        public DirectionalLight DirectionalLight1 { get; private set; }

        /// <inheritdoc/>
        public DirectionalLight DirectionalLight2 { get; private set; }

        /// <inheritdoc/>
        public bool FogEnabled
        {
            get => _fogEnabled;
            set
            {
                if (_fogEnabled != value)
                {
                    _fogEnabled = value;
                    dirtyFlags |= EffectDirtyFlags.ShaderIndex | EffectDirtyFlags.FogEnable;
                }
            }
        }

        /// <inheritdoc/>
        public float FogStart
        {
            get => _fogStart;

            set
            {
                _fogStart = value;
                dirtyFlags |= EffectDirtyFlags.Fog;
            }
        }

        /// <inheritdoc/>
        public float FogEnd
        {
            get => _fogEnd;

            set
            {
                _fogEnd = value;
                dirtyFlags |= EffectDirtyFlags.Fog;
            }
        }

        /// <inheritdoc/>
        public Vector3 FogColor
        {
            get => _fogColorParam.GetValueVector3();
            set => _fogColorParam.SetValue(value);
        }

        /// <summary>
        /// Gets or sets whether texturing is enabled.
        /// </summary>
        public bool TextureEnabled
        {
            get => _textureEnabled;
            set
            {
                if (_textureEnabled != value)
                {
                    _textureEnabled = value;
                    dirtyFlags |= EffectDirtyFlags.ShaderIndex;
                }
            }
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
        /// Gets or sets whether vertex color is enabled.
        /// </summary>
        public bool VertexColorEnabled
        {
            get => _vertexColorEnabled;
            set
            {
                if (_vertexColorEnabled != value)
                {
                    _vertexColorEnabled = value;
                    dirtyFlags |= EffectDirtyFlags.ShaderIndex;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new BasicEffect with default parameter settings.
        /// </summary>
        public BasicEffect(GraphicsDevice device)
            : base(device, EffectResource.BasicEffect.ByteCode)
        {
            CacheEffectParameters(null);

            DirectionalLight0.Enabled = true;
            SpecularColor = Vector3.One;
            SpecularPower = 16;
        }

        /// <summary>
        /// Creates a new BasicEffect by cloning parameter settings from an existing instance.
        /// </summary>
        protected BasicEffect(BasicEffect cloneSource)
            : base(cloneSource)
        {
            CacheEffectParameters(cloneSource);

            _lightingEnabled = cloneSource._lightingEnabled;
            _preferPerPixelLighting = cloneSource._preferPerPixelLighting;
            _fogEnabled = cloneSource._fogEnabled;
            _textureEnabled = cloneSource._textureEnabled;
            _vertexColorEnabled = cloneSource._vertexColorEnabled;

            _world = cloneSource._world;
            _view = cloneSource._view;
            _projection = cloneSource._projection;

            _diffuseColor = cloneSource._diffuseColor;
            _emissiveColor = cloneSource._emissiveColor;
            _ambientLightColor = cloneSource._ambientLightColor;

            _alpha = cloneSource._alpha;

            _fogStart = cloneSource._fogStart;
            _fogEnd = cloneSource._fogEnd;
        }


        /// <summary>
        /// Creates a clone of the current BasicEffect instance.
        /// </summary>
        public override Effect Clone()
        {
            return new BasicEffect(this);
        }

        /// <inheritdoc/>
        public void EnableDefaultLighting()
        {
            LightingEnabled = true;

            AmbientLightColor = EffectHelpers.EnableDefaultLighting(
                DirectionalLight0, DirectionalLight1, DirectionalLight2);
        }

        /// <summary>
        /// Looks up shortcut references to our effect parameters.
        /// </summary>
        private void CacheEffectParameters(BasicEffect? cloneSource)
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
            dirtyFlags = EffectHelpers.SetWorldViewProjAndFog(
                dirtyFlags, _world, _view, _projection, ref _worldView,
                _fogEnabled, _fogStart, _fogEnd, _worldViewProjParam, _fogVectorParam);

            // Recompute the diffuse/emissive/alpha material color parameters?
            if ((dirtyFlags & EffectDirtyFlags.MaterialColor) != 0)
            {
                EffectHelpers.SetMaterialColor(
                    _lightingEnabled, _alpha, _diffuseColor, _emissiveColor, _ambientLightColor,
                    _diffuseColorParam, _emissiveColorParam);

                dirtyFlags &= ~EffectDirtyFlags.MaterialColor;
            }

            if (_lightingEnabled)
            {
                // Recompute the world inverse transpose and eye position?
                dirtyFlags = EffectHelpers.SetLightingMatrices(
                    dirtyFlags, _world, _view,
                    _worldParam, _worldInverseTransposeParam, _eyePositionParam);


                // Check if we can use the only-bother-with-the-first-light shader optimization.
                bool newOneLight = !DirectionalLight1.Enabled && !DirectionalLight2.Enabled;

                if (_oneLight != newOneLight)
                {
                    _oneLight = newOneLight;
                    dirtyFlags |= EffectDirtyFlags.ShaderIndex;
                }
            }

            // Recompute the shader index?
            if ((dirtyFlags & EffectDirtyFlags.ShaderIndex) != 0)
            {
                int shaderIndex = 0;

                if (!_fogEnabled)
                    shaderIndex += 1;

                if (_vertexColorEnabled)
                    shaderIndex += 2;

                if (_textureEnabled)
                    shaderIndex += 4;

                if (_lightingEnabled)
                {
                    if (_preferPerPixelLighting)
                        shaderIndex += 24;
                    else if (_oneLight)
                        shaderIndex += 16;
                    else
                        shaderIndex += 8;
                }

                dirtyFlags &= ~EffectDirtyFlags.ShaderIndex;

                CurrentTechnique = Techniques[shaderIndex];
            }
        }


        #endregion
    }
}
