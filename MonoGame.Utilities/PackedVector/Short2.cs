// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing two 16-bit signed integer values.
    /// <para>
    /// Ranges from [-32767, -32767, 0, 1] to [32767, 32767, 0, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Short2 : IPixel, IPackedVector<uint>
    {
        // Largest two byte positive number 0xFFFF >> 1;
        private const float MaxPos = 0x7FFF;

        // Two's complement
        private const float MinNeg = ~(int)MaxPos;

        private static readonly Vector2 Max = new Vector2(MaxPos);
        private static readonly Vector2 Min = new Vector2(MinNeg);

        /// <summary>
        /// Initializes a new instance of the <see cref="Short2"/> struct.
        /// </summary>
        /// <param name="x">The x-component.</param>
        /// <param name="y">The y-component.</param>
        public Short2(float x, float y)
            : this(new Vector2(x, y))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Short2"/> struct.
        /// </summary>
        /// <param name="vector">The vector containing the component values.</param>
        public Short2(Vector2 vector) => PackedValue = Pack(vector);

        /// <inheritdoc/>
        public uint PackedValue { get; set; }

        /// <summary>
        /// Compares two <see cref="Short2"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Short2"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Short2"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Short2 left, Short2 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Short2"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Short2"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Short2"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Short2 left, Short2 right) => !left.Equals(right);

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector)
        {
            Vector2 scaled = new Vector2(vector.X, vector.Y) * 65534F;
            scaled -= new Vector2(32767F);
            PackedValue = Pack(scaled);
        }

        /// <inheritdoc/>
        public Vector4 ToScaledVector4()
        {
            var scaled = ToVector2();
            scaled += new Vector2(32767F);
            scaled /= 65534F;
            return new Vector4(scaled, 0F, 1F);
        }

        /// <inheritdoc />
        public void FromVector4(Vector4 vector)
        {
            var vector2 = new Vector2(vector.X, vector.Y);
            PackedValue = Pack(vector2);
        }

        /// <inheritdoc />
        public Vector4 ToVector4() => new Vector4((short)(PackedValue & 0xFFFF), (short)(PackedValue >> 0x10), 0, 1);

        /// <inheritdoc />
        public void FromArgb32(Argb32 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromBgr24(Bgr24 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromBgra32(Bgra32 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromGray8(Gray8 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromGray16(Gray16 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromRgb24(Rgb24 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromColor(Color source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void ToColor(ref Color dest) => dest.FromScaledVector4(ToScaledVector4());

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source) => FromScaledVector4(source.ToScaledVector4());

        /// <summary>
        /// Expands the packed representation into a <see cref="Vector2"/>.
        /// The vector components are typically expanded in least to greatest significance order.
        /// </summary>
        /// <returns>The <see cref="Vector2"/>.</returns>
        public Vector2 ToVector2() => new Vector2((short)(PackedValue & 0xFFFF), (short)(PackedValue >> 0x10));

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Short2 other && Equals(other);

        /// <inheritdoc />
        public bool Equals(Short2 other) => PackedValue.Equals(other.PackedValue);

        /// <inheritdoc />
        public override int GetHashCode() => PackedValue.GetHashCode();

        /// <inheritdoc />
        public override string ToString()
        {
            var vector = ToVector2();
            return FormattableString.Invariant($"Short2({vector.X:#0.##}, {vector.Y:#0.##})");
        }

        private static uint Pack(Vector2 vector)
        {
            vector = Vector2.Clamp(vector, Min, Max);
            uint word2 = (uint)Math.Round(vector.X) & 0xFFFF;
            uint word1 = ((uint)Math.Round(vector.Y) & 0xFFFF) << 0x10;

            return word2 | word1;
        }
    }
}
