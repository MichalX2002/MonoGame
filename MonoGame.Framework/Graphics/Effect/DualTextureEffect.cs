// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.

using System.Numerics;
using MonoGame.Framework;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Built-in effect that supports two-layer multitexturing.
    /// </summary>
    public class DualTextureEffect : Effect, IEffectMatrices, IEffectFog
    {
        #region Effect Parameters

        EffectParameter textureParam;
        EffectParameter texture2Param;
        EffectParameter diffuseColorParam;
        EffectParameter fogColorParam;
        EffectParameter fogVectorParam;
        EffectParameter worldViewProjParam;

        #endregion

        #region Fields

        Matrix4x4 _world = Matrix4x4.Identity;
        Matrix4x4 _view = Matrix4x4.Identity;
        Matrix4x4 _projection = Matrix4x4.Identity;
        Matrix4x4 _worldView;

        bool _vertexColorEnabled;
        Vector3 _diffuseColor = Vector3.One;
        float _alpha = 1;

        bool _fogEnabled;
        float _fogStart;
        float _fogEnd = 1;

        EffectDirtyFlags _dirtyFlags = EffectDirtyFlags.All;

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
                _dirtyFlags |= EffectDirtyFlags.WorldViewProj | EffectDirtyFlags.Fog;
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
                _dirtyFlags |= EffectDirtyFlags.WorldViewProj | EffectDirtyFlags.Fog;
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
            get => fogColorParam.GetValueVector3();
            set => fogColorParam.SetValue(value);
        }


        /// <summary>
        /// Gets or sets the current base texture.
        /// </summary>
        public Texture2D Texture
        {
            get => textureParam.GetValueTexture2D();
            set => textureParam.SetValue(value);
        }


        /// <summary>
        /// Gets or sets the current overlay texture.
        /// </summary>
        public Texture2D Texture2
        {
            get => texture2Param.GetValueTexture2D();
            set => texture2Param.SetValue(value);
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
                    _dirtyFlags |= EffectDirtyFlags.ShaderIndex;
                }
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Creates a new DualTextureEffect with default parameter settings.
        /// </summary>
        public DualTextureEffect(GraphicsDevice device)
            : base(device, EffectResource.DualTextureEffect.ByteCode)
        {
            CacheEffectParameters();
        }

        /// <summary>
        /// Creates a new DualTextureEffect by cloning parameter settings from an existing instance.
        /// </summary>
        protected DualTextureEffect(DualTextureEffect cloneSource)
            : base(cloneSource)
        {
            CacheEffectParameters();

            _fogEnabled = cloneSource._fogEnabled;
            _vertexColorEnabled = cloneSource._vertexColorEnabled;

            _world = cloneSource._world;
            _view = cloneSource._view;
            _projection = cloneSource._projection;

            _diffuseColor = cloneSource._diffuseColor;

            _alpha = cloneSource._alpha;

            _fogStart = cloneSource._fogStart;
            _fogEnd = cloneSource._fogEnd;
        }


        /// <summary>
        /// Creates a clone of the current DualTextureEffect instance.
        /// </summary>
        public override Effect Clone()
        {
            return new DualTextureEffect(this);
        }


        /// <summary>
        /// Looks up shortcut references to our effect parameters.
        /// </summary>
        void CacheEffectParameters()
        {
            textureParam = Parameters["Texture"];
            texture2Param = Parameters["Texture2"];
            diffuseColorParam = Parameters["DiffuseColor"];
            fogColorParam = Parameters["FogColor"];
            fogVectorParam = Parameters["FogVector"];
            worldViewProjParam = Parameters["WorldViewProj"];
        }


        /// <summary>
        /// Lazily computes derived parameter values immediately before applying the effect.
        /// </summary>
        protected internal override void OnApply()
        {
            // Recompute the world+view+projection matrix or fog vector?
            _dirtyFlags = EffectHelpers.SetWorldViewProjAndFog(
                _dirtyFlags, _world, _view, _projection, _worldView,
                _fogEnabled, _fogStart, _fogEnd, worldViewProjParam, fogVectorParam);

            // Recompute the diffuse/alpha material color parameter?
            if ((_dirtyFlags & EffectDirtyFlags.MaterialColor) != 0)
            {
                diffuseColorParam.SetValue(new Vector4(_diffuseColor * _alpha, _alpha));

                _dirtyFlags &= ~EffectDirtyFlags.MaterialColor;
            }

            // Recompute the shader index?
            if ((_dirtyFlags & EffectDirtyFlags.ShaderIndex) != 0)
            {
                int shaderIndex = 0;

                if (!_fogEnabled)
                    shaderIndex += 1;

                if (_vertexColorEnabled)
                    shaderIndex += 2;

                _dirtyFlags &= ~EffectDirtyFlags.ShaderIndex;

                CurrentTechnique = Techniques[shaderIndex];
            }
        }


        #endregion
    }
}
