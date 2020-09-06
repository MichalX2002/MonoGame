using System;
using System.Linq;
using System.Runtime.Serialization;

namespace MonoGame.Framework
{
    [DataContract]
    public struct Thickness : IEquatable<Thickness>
    {
        [DataMember]
        public int Left;

        [DataMember]
        public int Top;

        [DataMember]
        public int Right;

        [DataMember]
        public int Bottom;

        public readonly int Width => Left + Right;
        public readonly int Height => Top + Bottom;
        public readonly Size Size => new Size(Width, Height);

        public Thickness(int all) : this(all, all, all, all)
        {
        }

        public Thickness(int leftRight, int topBottom) : this(leftRight, topBottom, leftRight, topBottom)
        {
        }

        public Thickness(int left, int top, int right, int bottom) : this()
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static Thickness FromValues(ReadOnlySpan<int> values)
        {
            switch (values.Length)
            {
                case 1:
                    return new Thickness(values[0]);
                case 2:
                    return new Thickness(values[0], values[1]);
                case 4:
                    return new Thickness(values[0], values[1], values[2], values[3]);

                default:
                    throw new FormatException("Invalid thickness.");
            }
        }

        public static Thickness Parse(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return default;

            var values = value
                .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            return FromValues(values);
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Thickness other && Equals(other);
        }

        public readonly bool Equals(Thickness other)
        {
            return this == other;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Left, Top, Right, Bottom);
        }

        public override string ToString()
        {
            if (Left == Right && Top == Bottom)
                return Left == Top ? $"{Left}" : $"{Left} {Top}";

            return $"{Left}, {Right}, {Top}, {Bottom}";
        }

        public static bool operator ==(Thickness left, Thickness right)
        {
            return left.Left == right.Left
                && left.Right == right.Right
                && left.Top == right.Top
                && left.Bottom == right.Bottom;
        }

        public static bool operator !=(Thickness left, Thickness right)
        {
            return !(left == right);
        }

        public static implicit operator Thickness(int value)
        {
            return new Thickness(value);
        }
    }
}