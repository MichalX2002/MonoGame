//#region File Description
//-----------------------------------------------------------------------------
// EffectHelpers.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
//#endregion

using System;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Track which effect parameters need to be recomputed during the next OnApply.
    /// </summary>
    [Flags]
    internal enum EffectDirtyFlags
    {
        WorldViewProj = 1,
        World = 2,
        EyePosition = 4,
        MaterialColor = 8,
        Fog = 16,
        FogEnable = 32,
        AlphaTest = 64,
        ShaderIndex = 128,
        All = -1
    }

    /// <summary>
    /// Helper code shared between the various built-in effects.
    /// </summary>
    internal static class EffectHelpers
    {
        /// <summary>
        /// Sets up the standard key/fill/back lighting rig.
        /// </summary>
        internal static Vector3 EnableDefaultLighting(
            DirectionalLight light0, 
            DirectionalLight light1,
            DirectionalLight light2)
        {
            // Key light.
            light0.Direction = new Vector3(-0.5265408f, -0.5735765f, -0.6275069f);
            light0.DiffuseColor = new Vector3(1, 0.9607844f, 0.8078432f);
            light0.SpecularColor = new Vector3(1, 0.9607844f, 0.8078432f);
            light0.Enabled = true;

            // Fill light.
            light1.Direction = new Vector3(0.7198464f, 0.3420201f, 0.6040227f);
            light1.DiffuseColor = new Vector3(0.9647059f, 0.7607844f, 0.4078432f);
            light1.SpecularColor = Vector3.Zero;
            light1.Enabled = true;

            // Back light.
            light2.Direction = new Vector3(0.4545195f, -0.7660444f, 0.4545195f);
            light2.DiffuseColor = new Vector3(0.3231373f, 0.3607844f, 0.3937255f);
            light2.SpecularColor = new Vector3(0.3231373f, 0.3607844f, 0.3937255f);
            light2.Enabled = true;

            // Ambient light.
            return new Vector3(0.05333332f, 0.09882354f, 0.1819608f);
        }


        /// <summary>
        /// Lazily recomputes the world+view+projection matrix and
        /// fog vector based on the current effect parameter settings.
        /// </summary>
        internal static EffectDirtyFlags SetWorldViewProjAndFog(
            EffectDirtyFlags dirtyFlags,
            in Matrix world, 
            in Matrix view, 
            in Matrix projection, 
            Matrix worldView,
            bool fogEnabled, 
            float fogStart,
            float fogEnd,
            EffectParameter worldViewProjParam, 
            EffectParameter fogVectorParam)
        {
            // Recompute the world+view+projection matrix?
            if ((dirtyFlags & EffectDirtyFlags.WorldViewProj) != 0)
            {
                worldView = Matrix.Multiply(world, view);
                
                var worldViewProj = Matrix.Multiply(worldView, projection);
                worldViewProjParam.SetValue(worldViewProj);
                dirtyFlags &= ~EffectDirtyFlags.WorldViewProj;
            }

            if (fogEnabled)
            {
                // Recompute the fog vector?
                if ((dirtyFlags & (EffectDirtyFlags.Fog | EffectDirtyFlags.FogEnable)) != 0)
                {
                    SetFogVector(worldView, fogStart, fogEnd, fogVectorParam);
                    dirtyFlags &= ~(EffectDirtyFlags.Fog | EffectDirtyFlags.FogEnable);
                }
            }
            else
            {
                // When fog is disabled, make sure the fog vector is reset to zero.
                if ((dirtyFlags & EffectDirtyFlags.FogEnable) != 0)
                {
                    fogVectorParam.SetValue(Vector4.Zero);
                    dirtyFlags &= ~EffectDirtyFlags.FogEnable;
                }
            }

            return dirtyFlags;
        }


        /// <summary>
        /// Sets a vector which can be dotted with the object 
        /// space vertex position to compute fog amount.
        /// </summary>
        static void SetFogVector(
            in Matrix worldView, 
            float fogStart, 
            float fogEnd, 
            EffectParameter fogVectorParam)
        {
            if (fogStart == fogEnd)
            {
                // force everything to 100% fogged if start and end are the same
                fogVectorParam.SetValue(new Vector4(0, 0, 0, 1));
            }
            else
            {
                // We want to transform vertex positions into view space, take the resulting
                // Z value, then scale and offset according to the fog start/end distances.
                // Because we only care about the Z component, the shader can do all this
                // with a single dot product, using only the Z row of the world+view matrix.

                var fogVector = new Vector4(
                    worldView.M13,
                    worldView.M23,
                    worldView.M33,
                    worldView.M43 + fogStart);

                fogVector /= fogStart - fogEnd;

                fogVectorParam.SetValue(fogVector);
            }
        }


        /// <summary>
        /// Lazily recomputes the world inverse transpose matrix and
        /// eye position based on the current effect parameter settings.
        /// </summary>
        internal static EffectDirtyFlags SetLightingMatrices(
            EffectDirtyFlags dirtyFlags, 
            in Matrix world, 
            in Matrix view,
            EffectParameter worldParam, 
            EffectParameter worldInverseTransposeParam, 
            EffectParameter eyePositionParam)
        {
            // Set the world and world inverse transpose matrices.
            if ((dirtyFlags & EffectDirtyFlags.World) != 0)
            {
                var worldTranspose = Matrix.Invert(world);
                var worldInverseTranspose = Matrix.Transpose(worldTranspose);

                worldParam.SetValue(world);
                worldInverseTransposeParam.SetValue(worldInverseTranspose);
                dirtyFlags &= ~EffectDirtyFlags.World;
            }

            // Set the eye position.
            if ((dirtyFlags & EffectDirtyFlags.EyePosition) != 0)
            {
                var viewInverse = Matrix.Invert(view);

                eyePositionParam.SetValue(viewInverse.Translation);
                dirtyFlags &= ~EffectDirtyFlags.EyePosition;
            }

            return dirtyFlags;
        }


        /// <summary>
        /// Sets the diffuse/emissive/alpha material color parameters.
        /// </summary>
        internal static void SetMaterialColor(
            bool lightingEnabled,
            float alpha,
            in Vector3 diffuseColor,
            in Vector3 emissiveColor,
            in Vector3 ambientLightColor,
            EffectParameter diffuseColorParam,
            EffectParameter emissiveColorParam)
        {
            // Desired lighting model:
            //   ((AmbientLightColor + sum(diffuse directional light)) * DiffuseColor) + EmissiveColor
            //
            // When lighting is disabled, ambient and directional lights are ignored, leaving:
            //   DiffuseColor + EmissiveColor
            //
            // For the lighting disabled case, we can save one shader instruction by precomputing
            // diffuse+emissive on the CPU, after which the shader can use DiffuseColor directly,
            // ignoring its emissive parameter.
            //
            // When lighting is enabled, we can merge the ambient and emissive settings. If we
            // set our emissive parameter to emissive+(ambient*diffuse), the shader no longer
            // needs to bother adding the ambient contribution, simplifying its computation to:
            //   (sum(diffuse directional light) * DiffuseColor) + EmissiveColor
            //
            // For futher optimization goodness, we merge material alpha with the diffuse
            // color parameter, and premultiply all color values by this alpha.

            if (lightingEnabled)
            {
                var diffuse = new Vector4(diffuseColor, 1) * alpha;
                var emissive = (emissiveColor + ambientLightColor * diffuseColor) * alpha;

                diffuseColorParam.SetValue(diffuse);
                emissiveColorParam.SetValue(emissive);
            }
            else
            {
                var diffuse = new Vector4(diffuseColor + emissiveColor, 1) * alpha;

                diffuseColorParam.SetValue(diffuse);
            }
        }
    }
}
