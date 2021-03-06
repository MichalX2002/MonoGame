﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Numerics;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// A basic 3D model with per mesh parent bones.
    /// </summary>
    public sealed class Model : GraphicsResource
    {
        private static Matrix4x4[]? _sharedDrawBoneMatrices;
        
        /// <summary>
        /// A collection of <see cref="ModelBone"/> objects which describe how each mesh in the
        /// mesh collection for this model relates to its parent mesh.
        /// </summary>
        public ModelBoneCollection Bones { get; private set; }

        /// <summary>
        /// A collection of <see cref="ModelMesh"/> objects which compose the model. Each <see cref="ModelMesh"/>
        /// in a model may be moved independently and may be composed of multiple materials
        /// identified as <see cref="ModelMeshPart"/> objects.
        /// </summary>
        public ModelMeshCollection Meshes { get; private set; }

        /// <summary>
        /// Root bone for this model.
        /// </summary>
        public ModelBone Root { get; set; }

        /// <summary>
        /// Custom attached object.
        /// <remarks>
        /// Skinning data is example of attached object for model.
        /// </remarks>
        /// </summary>
        public object? AttachedData { get; set; }

        /// <summary>
        /// Constructs a model. 
        /// </summary>
        /// <param name="graphicsDevice">A valid reference to <see cref="GraphicsDevice"/>.</param>
        /// <param name="bones">The collection of bones.</param>
        /// <param name="meshes">The collection of meshes.</param>
        /// <exception cref="ArgumentNullException"><paramref name="graphicsDevice"/> is null. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="bones"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="meshes"/> is null.</exception>
        public Model(
            GraphicsDevice graphicsDevice, IList<ModelBone> bones, IList<ModelMesh> meshes) 
            : base(graphicsDevice)
        {
            // TODO: Complete member initialization
            Bones = new ModelBoneCollection(bones);
            Meshes = new ModelMeshCollection(meshes);
        }

        internal void BuildHierarchy()
        {
            var globalScale = Matrix4x4.CreateScale(0.01f);
            
            foreach(var node in Root.Children)
                BuildHierarchy(node, Root.Transform * globalScale, 0);
        }
        
        private void BuildHierarchy(ModelBone node, in Matrix4x4 parentTransform, int level)
        {
            node.ModelTransform = node.Transform * parentTransform;
            
            foreach (var child in node.Children)
                BuildHierarchy(child, node.ModelTransform, level + 1);
            
            //string s = string.Empty;
            //
            //for (int i = 0; i < level; i++) 
            //{
            //	s += "\t";
            //}
            //
            //Debug.WriteLine("{0}:{1}", s, node.Name);
        }

        /// <summary>
        /// Draws the model meshes.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device to draw the model to.</param>
        /// <param name="world">The world transform.</param>
        /// <param name="view">The view transform.</param>
        /// <param name="projection">The projection transform.</param>
        public void Draw(GraphicsDevice graphicsDevice, in Matrix4x4 world, in Matrix4x4 view, in Matrix4x4 projection) 
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));

            if (_sharedDrawBoneMatrices == null ||
                _sharedDrawBoneMatrices.Length < Bones.Count)
                _sharedDrawBoneMatrices = new Matrix4x4[Bones.Count];
            
            // Look up combined bone matrices for the entire model.            
            CopyAbsoluteBoneTransformsTo(_sharedDrawBoneMatrices);

            // Draw the model.
            foreach (ModelMesh mesh in Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    if (!(effect is IEffectMatrices effectMatricies))
                        throw new InvalidOperationException();

                    effectMatricies.World = _sharedDrawBoneMatrices[mesh.ParentBone.Index] * world;
                    effectMatricies.View = view;
                    effectMatricies.Projection = projection;
                }

                mesh.Draw(graphicsDevice);
            }
        }

        /// <summary>
        /// Copies bone transforms relative to all parent bones of the each bone from this model to a given span.
        /// </summary>
        /// <param name="destinationBoneTransforms">The span receiving the transformed bones.</param>
        public void CopyAbsoluteBoneTransformsTo(Span<Matrix4x4> destinationBoneTransforms)
        {
            if (destinationBoneTransforms.IsEmpty)
                throw new ArgumentNullException(nameof(destinationBoneTransforms));
            if (destinationBoneTransforms.Length < Bones.Count)
                throw new ArgumentOutOfRangeException(nameof(destinationBoneTransforms));

            int count = Bones.Count;
            for (int i = 0; i < count; ++i)
            {
                ModelBone modelBone = Bones[i];
                if (modelBone.Parent == null)
                {
                    destinationBoneTransforms[i] = modelBone.Transform;
                }
                else
                {
                    int index2 = modelBone.Parent.Index;
                    destinationBoneTransforms[i] = modelBone.Transform * destinationBoneTransforms[index2];
                }
            }
        }

        /// <summary>
        /// Copies bone transforms relative to <see cref="Root"/> bone from a given span to this model.
        /// </summary>
        /// <param name="sourceBoneTransforms">The span of prepared bone transform data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sourceBoneTransforms"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="sourceBoneTransforms"/> is invalid.
        /// </exception>
        public void CopyBoneTransformsFrom(ReadOnlySpan<Matrix4x4> sourceBoneTransforms)
        {
            if (sourceBoneTransforms.IsEmpty)
                throw new ArgumentEmptyException(nameof(sourceBoneTransforms));
            if (sourceBoneTransforms.Length < Bones.Count)
                throw new ArgumentOutOfRangeException(nameof(sourceBoneTransforms));

            int count = Bones.Count;
            for (int i = 0; i < count; i++)
                Bones[i].Transform = sourceBoneTransforms[i];
        }

        /// <summary>
        /// Copies bone transforms relative to <see cref="Root"/> bone from this model to a given span.
        /// </summary>
        /// <param name="destinationBoneTransforms">The span receiving the transformed bones.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="destinationBoneTransforms"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="destinationBoneTransforms"/> is invalid.
        /// </exception>
        public void CopyBoneTransformsTo(Span<Matrix4x4> destinationBoneTransforms)
        {
            if (destinationBoneTransforms.IsEmpty)
                throw new ArgumentEmptyException(nameof(destinationBoneTransforms));
            if (destinationBoneTransforms.Length < Bones.Count)
                throw new ArgumentOutOfRangeException(nameof(destinationBoneTransforms));

            int count = Bones.Count;
            for (int i = 0; i < count; i++)
                destinationBoneTransforms[i] = Bones[i].Transform;
        }
    }
}
