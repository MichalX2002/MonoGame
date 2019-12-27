using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Framework.Graphics
{
    public class EffectAnnotationCollection : IReadOnlyList<EffectAnnotation>
    {
        internal static readonly EffectAnnotationCollection Empty = 
            new EffectAnnotationCollection(Array.Empty<EffectAnnotation>());

        private readonly EffectAnnotation[] _annotations;

        public int Count => _annotations.Length;

        public EffectAnnotation this[int index] => _annotations[index];

        public EffectAnnotation this[string name]
        {
            get
            {
                foreach (var annotation in _annotations)
                {
                    if (annotation.Name == name)
                        return annotation;
                }
                return null;
            }
        }

        internal EffectAnnotationCollection(EffectAnnotation[] annotations)
        {
            _annotations = annotations;
        }

        public IEnumerator<EffectAnnotation> GetEnumerator() => ((IEnumerable<EffectAnnotation>)_annotations).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _annotations.GetEnumerator();
    }
}

