// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Microsoft.Xna.Framework.Content.Pipeline.Processors
{
    public sealed class ModelMeshContent
    {
        private MeshContent _sourceMesh;

        internal ModelMeshContent() { }

        internal ModelMeshContent(string name, MeshContent sourceMesh, ModelBoneContent parentBone,
                                  BoundingSphere boundingSphere, IList<ModelMeshPartContent> meshParts)
        {
            Name = name;
            _sourceMesh = sourceMesh;
            ParentBone = parentBone;
            BoundingSphere = boundingSphere;
            MeshParts = new ModelMeshPartContentCollection(meshParts);
        }

        public BoundingSphere BoundingSphere { get; }

        public ModelMeshPartContentCollection MeshParts { get; }

        public string Name { get; }

        public ModelBoneContent ParentBone { get; }

        public MeshContent SourceMesh
        {
            get { return _sourceMesh; }
        }

        public object Tag { get; set; }
    }
}
