// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.

using System.Numerics;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// The default effect used by SpriteBatch.
    /// </summary>
    public class SpriteEffect : Effect
    {
        private EffectParameter _matrixParam;
        private Viewport _lastViewport;
        private Matrix4x4 _projection;

        // Current behavior:
        // Normal 3D cameras look into the -z direction (z = 1 is in front of z = 0).

        // Old behavior:
        // The sprite batch layer depth is the opposite (z = 0 is in front of z = 1).
        // --> We get the correct matrix with near plane 0 and far plane -1.

        /// <summary>
        /// The minimum Z-value of the view volume.
        /// </summary>
        public float ZNearPlane { get; set; } = -1000;

        /// <summary>
        /// The maximum Z-value of the view volume.
        /// </summary>
        public float ZFarPlane { get; set; } = 1000;

        /// <summary>
        /// Creates a new SpriteEffect.
        /// </summary>
        public SpriteEffect(GraphicsDevice device)
            : base(device, EffectResource.SpriteEffect.ByteCode)
        {
            _matrixParam = Parameters["MatrixTransform"];
        }

        /// <summary>
        /// An optional matrix used to transform the sprite geometry.
        /// Uses <see cref="Matrix4x4.Identity"/> if null.
        /// </summary>
        public Matrix4x4? TransformMatrix { get; set; }

        /// <summary>
        /// Creates a new <see cref="SpriteEffect"/> by 
        /// cloning parameter settings from an existing instance.
        /// </summary>
        protected SpriteEffect(SpriteEffect cloneSource)
            : base(cloneSource)
        {
            _matrixParam = Parameters["MatrixTransform"];
        }

        /// <summary>
        /// Creates a clone of the current SpriteEffect instance.
        /// </summary>
        public override Effect Clone()
        {
            return new SpriteEffect(this);
        }

        /// <summary>
        /// Computes derived parameter values immediately before applying the effect.
        /// </summary>
        protected internal override void OnApply()
        {
            var vp = GraphicsDevice.Viewport;
            if ((vp.Width != _lastViewport.Width) ||
                (vp.Height != _lastViewport.Height))
            {
                _projection = Matrix4x4.CreateOrthographicOffCenter(
                    0, vp.Width, vp.Height, 0, ZNearPlane, ZFarPlane);

                _lastViewport = vp;
            }

            if (TransformMatrix.HasValue)
                _matrixParam.SetValue(TransformMatrix.GetValueOrDefault() * _projection);
            else
                _matrixParam.SetValue(_projection);
        }
    }
}
