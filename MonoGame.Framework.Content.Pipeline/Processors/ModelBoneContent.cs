// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Numerics;

namespace MonoGame.Framework.Content.Pipeline.Processors
{
    public sealed class ModelBoneContent
    {
        public ModelBoneContentCollection Children { get; internal set; }

        public int Index { get; }

        public string Name { get; }

        public ModelBoneContent Parent { get; }

        public Matrix4x4 Transform { get; set; }

        internal ModelBoneContent()
        {
        }

        internal ModelBoneContent(string name, int index, Matrix4x4 transform, ModelBoneContent parent)
        {
            Name = name;
            Index = index;
            Transform = transform;
            Parent = parent;
        }
    }
}
