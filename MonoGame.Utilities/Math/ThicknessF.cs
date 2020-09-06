using System;
using System.Linq;
using System.Runtime.Serialization;

namespace MonoGame.Framework
{
    [DataContract]
    public struct ThicknessF : IEquatable<ThicknessF>
    {
        [DataMember]
        public float Left;

        [DataMember]
        public float Top;

        [DataMember]
        public float Right;

        [DataMember]
        public float Bottom;

        public readonly float Width => Left + Right;
        public readonly float Height => Top + Bottom;
        public readonly SizeF Size => new SizeF(Width, Height);

        public ThicknessF(float all) : this(all, all, all, all)
        {
        }

        public ThicknessF(float leftRight, float topBottom) : this(leftRight, topBottom, leftRight, topBottom)
        {
        }

        public ThicknessF(float left, float top, float right, float bottom) : this()
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static ThicknessF FromValues(ReadOnlySpan<float> values)
        {
            switch (values.Length)
            {
                case 1:
                    return new ThicknessF(values[0]);
                case 2:
                    return new ThicknessF(values[0], values[1]);
                case 4:
                    return new ThicknessF(values[0], values[1], values[2], values[3]);

                default:
                    throw new FormatException("Invalid thickness.");
            }
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is ThicknessF other && Equals(other);
        }

        public readonly bool Equals(ThicknessF other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Left, Right, Top, Bottom);
        }

        public static ThicknessF Parse(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return default;

            float[] values = value
                .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(float.Parse)
                .ToArray();

            return FromValues(values);
        }

        public override string ToString()
        {
            if (Left == Right && Top == Bottom)
                return Left == Top ? $"{Left}" : $"{Left} {Top}";

            return $"{Left}, {Right}, {Top}, {Bottom}";
        }

        public static bool operator ==(ThicknessF left, ThicknessF right)
        {
            return left.Left == right.Left
                && left.Right == right.Right
                && left.Top == right.Top
                && left.Bottom == right.Bottom;
        }

        public static bool operator !=(ThicknessF left, ThicknessF right)
        {
            return !(left == right);
        }

        public static implicit operator ThicknessF(float value)
        {
            return new ThicknessF(value);
        }
    }
}