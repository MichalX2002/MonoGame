using System;

namespace MonoGame.Framework.Graphics
{
    public sealed class ModelMeshPart
    {
        private Effect? _effect;
        internal ModelMesh _parent;

        public object? Tag { get; set; }
        public int PrimitiveCount { get; set; }

        internal int EffectIndex { get; set; }

        internal int IndexBufferIndex { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        public int StartIndex { get; set; }

        internal int VertexBufferIndex { get; set; }
        public VertexBuffer VertexBuffer { get; set; }
        public int VertexCount { get; set; }
        public int VertexOffset { get; set; }

        public Effect? Effect 
        {
            get => _effect;
            set
            {
                if (value == _effect)
                    return;

                if (_effect != null)
                {
                    // First check to see any other parts are also using this effect.
                    var removeEffect = true;
                    foreach (var part in _parent.MeshParts)
                    {
                        if (part != this && part._effect == _effect)
                        {
                            removeEffect = false;
                            break;
                        }
                    }

                    if (removeEffect)
                        _parent.Effects.Remove(_effect);
                }

                // Set the new effect.
                _effect = value;

                if (_effect != null && !_parent.Effects.Contains(_effect))
                    _parent.Effects.Add(_effect);
            }
        }

        /// <summary>
        /// Constructs the model mesh part.
        /// </summary>
        /// <param name="parent"></param>
        public ModelMeshPart(ModelMesh parent) 
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }
    }
}
