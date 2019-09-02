// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MonoGame.Framework.Content.Pipeline.Processors
{
    public sealed class ModelContent
    {
        private ModelBoneContent _root;

        internal ModelContent() { }

        internal ModelContent(ModelBoneContent root, IList<ModelBoneContent> bones, IList<ModelMeshContent> meshes)
        {
            _root = root;
            Bones = new ModelBoneContentCollection(bones);
            Meshes = new ModelMeshContentCollection(meshes);
        }

        public ModelBoneContentCollection Bones { get; }

        public ModelMeshContentCollection Meshes { get; }

        public ModelBoneContent Root => _root;

        public object Tag { get; set; }
    }
}
