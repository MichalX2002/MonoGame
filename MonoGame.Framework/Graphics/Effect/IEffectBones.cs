// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Interface for Effects that support bone transforms.
    /// </summary>
    public interface IEffectBones
    {
        /// <summary>
        /// Sets a span of skinning bone transform matrices.
        /// </summary>
        void SetBoneTransforms(ReadOnlySpan<Matrix4x4> boneTransforms);
    }
}

