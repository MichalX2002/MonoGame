// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing two 16-bit floating-point values.
    /// <para>
    /// Ranges from [-1, -1, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    public struct HalfVector2 : IPixel, IPackedVector<uint>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HalfVector2"/> struct.
        /// </summary>
        /// <param name="x">The x-component.</param>
        /// <param name="y">The y-component.</param>
        public HalfVector2(float x, float y) => PackedValue = Pack(x, y);

        /// <summary>
        /// Initializes a new instance of the <see cref="HalfVector2"/> struct.
        /// </summary>
        /// <param name="vector">A vector containing the initial values for the components.</param>
        public HalfVector2(Vector2 vector) => PackedValue = Pack(vector.X, vector.Y);

        /// <inheritdoc/>
        public uint PackedValue { get; set; }

        /// <summary>
        /// Compares two <see cref="HalfVector2"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="HalfVector2"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="HalfVector2"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(HalfVector2 left, HalfVector2 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="HalfVector2"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="HalfVector2"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="HalfVector2"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(HalfVector2 left, HalfVector2 right) => !left.Equals(right);

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector)
        {
            Vector2 scaled = new Vector2(vector.X, vector.Y) * 2F;
            scaled -= Vector2.One;
            PackedValue = Pack(scaled.X, scaled.Y);
        }

        /// <inheritdoc/>
        public Vector4 ToScaledVector4()
        {
            var scaled = ToVector2();
            scaled += Vector2.One;
            scaled /= 2F;
            return new Vector4(scaled, 0F, 1F);
        }

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => PackedValue = Pack(vector.X, vector.Y);

        /// <inheritdoc />
        public Vector4 ToVector4()
        {
            var vector = ToVector2();
            return new Vector4(vector.X, vector.Y, 0F, 1F);
        }

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
        public void ToColor(ref Color dest)
        {
            dest.FromScaledVector4(ToScaledVector4());
        }

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source) => FromScaledVector4(source.ToScaledVector4());

        /// <summary>
        /// Expands the packed representation into a <see cref="Vector2"/>.
        /// </summary>
        /// <returns>The <see cref="Vector2"/>.</returns>
        public Vector2 ToVector2()
        {
            Vector2 vector;
            vector.X = HalfTypeHelper.Unpack((ushort)PackedValue);
            vector.Y = HalfTypeHelper.Unpack((ushort)(PackedValue >> 0x10));
            return vector;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is HalfVector2 other && Equals(other);

        /// <inheritdoc />
        public bool Equals(HalfVector2 other) => PackedValue.Equals(other.PackedValue);

        /// <inheritdoc />
        public override string ToString()
        {
            var vector = ToVector2();
            return FormattableString.Invariant($"HalfVector2({vector.X:#0.##}, {vector.Y:#0.##})");
        }

        /// <inheritdoc />
        public override int GetHashCode() => PackedValue.GetHashCode();

        private static uint Pack(float x, float y)
        {
            uint num2 = HalfTypeHelper.Pack(x);
            uint num = (uint)(HalfTypeHelper.Pack(y) << 0x10);
            return num2 | num;
        }
    }
}
