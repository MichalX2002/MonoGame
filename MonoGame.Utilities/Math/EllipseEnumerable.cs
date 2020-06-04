using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace MonoGame.Framework
{
    public struct EllipseEnumerable : IEnumerable<Vector2>, IEnumerator<Vector2>
    {
        private readonly Vector2 _size;
        private readonly float _step;
        private readonly int _count;
        private readonly float _start;

        private int _offset;
        private float _theta;

        public Vector2 Current { get; private set; }
        object IEnumerator.Current => Current;

        public EllipseEnumerable(Vector2 size, int sides, int count, float start)
        {
            if (sides < 0)
                throw new ArgumentOutOfRangeException(nameof(sides));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            _size = size;
            _count = count;
            _start = start;
            _step = 2 * MathF.PI / sides;

            _theta = _start;
            _offset = 0;
            Current = default;
        }

        public EllipseEnumerable(Vector2 size, int sides) : this(size, sides, sides, 0)
        {
        }

        public bool MoveNext()
        {
            if (_offset < _count)
            {
                Current = new Vector2(MathF.Cos(_theta), MathF.Sin(_theta)) * _size;
                _theta += _step;
                _offset++;
                return true;
            }

            Current = default;
            return false;
        }

        public void Reset()
        {
            _theta = _start;
            _offset = 0;
        }

        public void Dispose()
        {
        }

        public EllipseEnumerable GetEnumerator()
        {
            return this;
        }

        IEnumerator<Vector2> IEnumerable<Vector2>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}