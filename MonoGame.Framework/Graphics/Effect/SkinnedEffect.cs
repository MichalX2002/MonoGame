// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.

using System;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Built-in effect for rendering skinned character models.
    /// </summary>
    public class SkinnedEffect : Effect, IEffectMatrices, IEffectLights, IEffectFog
    {
        public const int MaxBones = 72;

        #region Fields

        private EffectParameter textureParam;
        private EffectParameter diffuseColorParam;
        private EffectParameter emissiveColorParam;
        private EffectParameter specularColorParam;
        private EffectParameter specularPowerParam;
        private EffectParameter eyePositionParam;
        private EffectParameter fogColorParam;
        private EffectParameter fogVectorParam;
        private EffectParameter worldParam;
        private EffectParameter worldInverseTransposeParam;
        private EffectParameter worldViewProjParam;
        private EffectParameter bonesParam;

        private bool preferPerPixelLighting;
        private bool oneLight;
        private bool fogEnabled;
        private Matrix world = Matrix.Identity;
        private Matrix view = Matrix.Identity;
        private Matrix projection = Matrix.Identity;
        private Matrix worldView;
        private Vector3 diffuseColor = Vector3.One;
        private Vector3 emissiveColor = Vector3.Zero;
        private Vector3 ambientLightColor = Vector3.Zero;
        private float alpha = 1;
        private float fogStart = 0;
        private float fogEnd = 1;
        private int weightsPerVertex = 4;
        private EffectDirtyFlags dirtyFlags = EffectDirtyFlags.All;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the world matrix.
        /// </summary>
        public Matrix World
        {
            get => world;
            set
            {
                world = value;
                dirtyFlags |= EffectDirtyFlags.World | EffectDirtyFlags.WorldViewProj | EffectDirtyFlags.Fog;
            }
        }

        /// <summary>
        /// Gets or sets the view matrix.
        /// </summary>
        public Matrix View
        {
            get => view;
            set
            {
                view = value;
                dirtyFlags |= EffectDirtyFlags.WorldViewProj | EffectDirtyFlags.EyePosition | EffectDirtyFlags.Fog;
            }
        }
        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        public Matrix Projection
        {
            get => projection;
            set
            {
                projection = value;
                dirtyFlags |= EffectDirtyFlags.WorldViewProj;
            }
        }

        /// <summary>
        /// Gets or sets the material diffuse color (range 0 to 1).
        /// </summary>
        public Vector3 DiffuseColor
        {
            get => diffuseColor;
            set
            {
                diffuseColor = value;
                dirtyFlags |= EffectDirtyFlags.MaterialColor;
            }
        }

        /// <summary>
        /// Gets or sets the material emissive color (range 0 to 1).
        /// </summary>
        public Vector3 EmissiveColor
        {
            get => emissiveColor;
            set
            {
                emissiveColor = value;
                dirtyFlags |= EffectDirtyFlags.MaterialColor;
            }
        }

        /// <summary>
        /// Gets or sets the material specular color (range 0 to 1).
        /// </summary>
        public Vector3 SpecularColor
        {
            get => specularColorParam.GetValueVector3();
            set => specularColorParam.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the material specular power.
        /// </summary>
        public float SpecularPower
        {
            get => specularPowerParam.GetValueSingle();
            set => specularPowerParam.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the material alpha.
        /// </summary>
        public float Alpha
        {
            get => alpha;
            set
            {
                alpha = value;
                dirtyFlags |= EffectDirtyFlags.MaterialColor;
            }
        }

        /// <summary>
        /// Gets or sets the per-pixel lighting prefer flag.
        /// </summary>
        public bool PreferPerPixelLighting
        {
            get => preferPerPixelLighting;
            set
            {
                if (preferPerPixelLighting != value)
                {
                    preferPerPixelLighting = value;
                    dirtyFlags |= EffectDirtyFlags.ShaderIndex;
                }
            }
        }

        /// <summary>
        /// Gets or sets the ambient light color (range 0 to 1).
        /// </summary>
        public Vector3 AmbientLightColor
        {
            get => ambientLightColor;
            set
            {
                ambientLightColor = value;
                dirtyFlags |= EffectDirtyFlags.MaterialColor;
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
            get => fogEnabled;
            set
            {
                if (fogEnabled != value)
                {
                    fogEnabled = value;
                    dirtyFlags |= EffectDirtyFlags.ShaderIndex | EffectDirtyFlags.FogEnable;
                }
            }
        }

        /// <summary>
        /// Gets or sets the fog start distance.
        /// </summary>
        public float FogStart
        {
            get => fogStart;
            set
            {
                fogStart = value;
                dirtyFlags |= EffectDirtyFlags.Fog;
            }
        }

        /// <summary>
        /// Gets or sets the fog end distance.
        /// </summary>
        public float FogEnd
        {
            get => fogEnd;
            set
            {
                fogEnd = value;
                dirtyFlags |= EffectDirtyFlags.Fog;
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
        /// Gets or sets the current texture.
        /// </summary>
        public Texture2D Texture
        {
            get => textureParam.GetValueTexture2D();
            set => textureParam.SetValue(value);
        }

        /// <summary>
        /// Gets or sets the number of skinning weights to evaluate for each vertex (1, 2, or 4).
        /// </summary>
        public int WeightsPerVertex
        {
            get => weightsPerVertex;
            set
            {
                if ((value != 1) &&
                    (value != 2) &&
                    (value != 4))
                    throw new ArgumentOutOfRangeException(nameof(value));

                weightsPerVertex = value;
                dirtyFlags |= EffectDirtyFlags.ShaderIndex;
            }
        }

        /// <summary>
        /// Sets an array of skinning bone transform matrices.
        /// </summary>
        public void SetBoneTransforms(Matrix[] boneTransforms)
        {
            if ((boneTransforms == null) || (boneTransforms.Length == 0))
                throw new ArgumentNullException(nameof(boneTransforms));

            if (boneTransforms.Length > MaxBones)
                throw new ArgumentException();

            bonesParam.SetValue(boneTransforms);
        }

        /// <summary>
        /// Gets a copy of the current skinning bone transform matrices.
        /// </summary>
        public Matrix[] GetBoneTransforms(int count)
        {
            if (count <= 0 || count > MaxBones)
                throw new ArgumentOutOfRangeException(nameof(count));

            Matrix[] bones = bonesParam.GetValueMatrixArray(count);

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

            Matrix[] identityBones = new Matrix[MaxBones];

            for (int i = 0; i < MaxBones; i++)
            {
                identityBones[i] = Matrix.Identity;
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

            preferPerPixelLighting = cloneSource.preferPerPixelLighting;
            fogEnabled = cloneSource.fogEnabled;

            world = cloneSource.world;
            view = cloneSource.view;
            projection = cloneSource.projection;

            diffuseColor = cloneSource.diffuseColor;
            emissiveColor = cloneSource.emissiveColor;
            ambientLightColor = cloneSource.ambientLightColor;

            alpha = cloneSource.alpha;

            fogStart = cloneSource.fogStart;
            fogEnd = cloneSource.fogEnd;

            weightsPerVertex = cloneSource.weightsPerVertex;
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
            textureParam = Parameters["Texture"];
            diffuseColorParam = Parameters["DiffuseColor"];
            emissiveColorParam = Parameters["EmissiveColor"];
            specularColorParam = Parameters["SpecularColor"];
            specularPowerParam = Parameters["SpecularPower"];
            eyePositionParam = Parameters["EyePosition"];
            fogColorParam = Parameters["FogColor"];
            fogVectorParam = Parameters["FogVector"];
            worldParam = Parameters["World"];
            worldInverseTransposeParam = Parameters["WorldInverseTranspose"];
            worldViewProjParam = Parameters["WorldViewProj"];
            bonesParam = Parameters["Bones"];

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
                dirtyFlags, world, view, projection, worldView,
                fogEnabled, fogStart, fogEnd, worldViewProjParam, fogVectorParam);

            // Recompute the world inverse transpose and eye position?
            dirtyFlags = EffectHelpers.SetLightingMatrices(
                dirtyFlags, world, view, 
                worldParam, worldInverseTransposeParam, eyePositionParam);

            // Recompute the diffuse/emissive/alpha material color parameters?
            if ((dirtyFlags & EffectDirtyFlags.MaterialColor) != 0)
            {
                EffectHelpers.SetMaterialColor(
                    true, alpha, diffuseColor, emissiveColor, ambientLightColor, 
                    diffuseColorParam, emissiveColorParam);

                dirtyFlags &= ~EffectDirtyFlags.MaterialColor;
            }

            // Check if we can use the only-bother-with-the-first-light shader optimization.
            bool newOneLight = !DirectionalLight1.Enabled && !DirectionalLight2.Enabled;

            if (oneLight != newOneLight)
            {
                oneLight = newOneLight;
                dirtyFlags |= EffectDirtyFlags.ShaderIndex;
            }

            // Recompute the shader index?
            if ((dirtyFlags & EffectDirtyFlags.ShaderIndex) != 0)
            {
                int shaderIndex = 0;

                if (!fogEnabled)
                    shaderIndex += 1;

                if (weightsPerVertex == 2)
                    shaderIndex += 2;
                else if (weightsPerVertex == 4)
                    shaderIndex += 4;

                if (preferPerPixelLighting)
                    shaderIndex += 12;
                else if (oneLight)
                    shaderIndex += 6;

                dirtyFlags &= ~EffectDirtyFlags.ShaderIndex;

                CurrentTechnique = Techniques[shaderIndex];
            }
        }


        #endregion
    }
}
