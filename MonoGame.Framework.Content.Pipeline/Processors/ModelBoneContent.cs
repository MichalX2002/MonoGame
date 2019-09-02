// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Content.Pipeline.Processors
{
    public sealed class ModelBoneContent
    {
        private Matrix _transform;

        internal ModelBoneContent() { }

        internal ModelBoneContent(string name, int index, Matrix transform, ModelBoneContent parent)
        {
            Name = name;
            Index = index;
            _transform = transform;
            Parent = parent;
        }

        public ModelBoneContentCollection Children { get; internal set; }

        public int Index { get; }

        public string Name { get; }

        public ModelBoneContent Parent { get; }

        public Matrix Transform
        {
            get => _transform;
            set => _transform = value;
        }
    }
}
