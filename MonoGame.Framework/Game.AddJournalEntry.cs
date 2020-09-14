// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework
{
    public partial class Game
    {
        private readonly struct AddJournalEntry<T> : IEquatable<AddJournalEntry<T>>
        {
            public int Order { get; }
            public T Item { get; }

            public AddJournalEntry(int order, T item)
            {
                Order = order;
                Item = item;
            }

            public static AddJournalEntry<T> CreateKey(T item)
            {
                return new AddJournalEntry<T>(-1, item);
            }

            public bool Equals(AddJournalEntry<T> other)
            {
                return Item is IEquatable<T> equatable
                    ? equatable.Equals(other.Item)
                    : Equals(Item, other.Item);
            }

            public override bool Equals(object? obj)
            {
                return obj is AddJournalEntry<T> entry && Equals(entry);
            }

            public override int GetHashCode()
            {
                return Item?.GetHashCode() ?? 0;
            }
        }
    }
}