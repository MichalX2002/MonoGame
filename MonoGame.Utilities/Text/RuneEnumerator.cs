// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Framework
{
    public ref struct RuneEnumerator
    {
        private delegate bool MoveNextDelegate(ref RuneEnumerator enumerator);

        private MoveNextDelegate _moveNext;

        private SpanRuneEnumerator _spanEnumerator;
        private StringRuneEnumerator _stringEnumerator;
        private StringBuilder.ChunkEnumerator _builderEnumerator;
        private IEnumerator<Rune> _interfaceEnumerator;

        public Rune Current { get; private set; }

        public RuneEnumerator(SpanRuneEnumerator spanEnumerator) : this()
        {
            _moveNext = SpanMoveNext;
            _spanEnumerator = spanEnumerator;
        }

        public RuneEnumerator(StringRuneEnumerator stringEnumerator) : this()
        {
            _moveNext = StringMoveNext;
            _stringEnumerator = stringEnumerator;
        }

        public RuneEnumerator(StringBuilder.ChunkEnumerator builderEnumerator) : this()
        {
            _moveNext = BuilderMoveNext;
            _builderEnumerator = builderEnumerator;
        }

        public RuneEnumerator(IEnumerator<Rune> interfaceEnumerator) : this()
        {
            _moveNext = InterfaceMoveNext;
            _interfaceEnumerator = interfaceEnumerator;
        }

        public bool MoveNext()
        {
            return _moveNext.Invoke(ref this);
        }

        public RuneEnumerator GetEnumerator()
        {
            return this;
        }

        private static bool SpanMoveNext(ref RuneEnumerator e)
        {
            if (e._spanEnumerator.MoveNext())
            {
                e.Current = e._spanEnumerator.Current;
                return true;
            }
            return false;
        }

        private static bool StringMoveNext(ref RuneEnumerator e)
        {
            if (e._stringEnumerator.MoveNext())
            {
                e.Current = e._stringEnumerator.Current;
                return true;
            }
            return false;
        }

        private static bool BuilderMoveNext(ref RuneEnumerator e)
        {
            TryReturnFromChunk:
            if (e._spanEnumerator.MoveNext())
            {
                e.Current = e._spanEnumerator.Current;
                return true;
            }

            if (e._builderEnumerator.MoveNext())
            {
                var chunkSpan = e._builderEnumerator.Current.Span;
                e._spanEnumerator = chunkSpan.EnumerateRunes();
                goto TryReturnFromChunk;
            }
            return false;
        }

        private static bool InterfaceMoveNext(ref RuneEnumerator e)
        {
            if (e._interfaceEnumerator.MoveNext())
            {
                e.Current = e._interfaceEnumerator.Current;
                return true;
            }
            return false;
        }

        public static implicit operator RuneEnumerator(ReadOnlySpan<char> text)
        {
            return new RuneEnumerator(text.EnumerateRunes());
        }

        public static implicit operator RuneEnumerator(ReadOnlyMemory<char> text)
        {
            return new RuneEnumerator(text.Span.EnumerateRunes());
        }

        public static implicit operator RuneEnumerator(string text)
        {
            return new RuneEnumerator(text.EnumerateRunes());
        }

        public static implicit operator RuneEnumerator(StringBuilder text)
        {
            return new RuneEnumerator(text.GetChunks());
        }
    }
}
