// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing four 16-bit signed integer values.
    /// <para>
    /// Ranges from [-37267, -37267, -37267, -37267] to [37267, 37267, 37267, 37267] in vector form.
    /// </para>
    /// </summary>
    public struct Short4 : IPixel, IPackedVector<ulong>
    {
        // Largest two byte positive number 0xFFFF >> 1;
        private const float MaxPos = 0x7FFF;

        // Two's complement
        private const float MinNeg = ~(int)MaxPos;

        private static readonly Vector4 Max = new Vector4(MaxPos);
        private static readonly Vector4 Min = new Vector4(MinNeg);

        /// <summary>
        /// Initializes a new instance of the <see cref="Short4"/> struct.
        /// </summary>
        /// <param name="x">The x-component.</param>
        /// <param name="y">The y-component.</param>
        /// <param name="z">The z-component.</param>
        /// <param name="w">The w-component.</param>
        public Short4(float x, float y, float z, float w)
            : this(new Vector4(x, y, z, w))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Short4"/> struct.
        /// </summary>
        /// <param name="vector">A vector containing the initial values for the components.</param>
        public Short4(Vector4 vector) => PackedValue = Pack(ref vector);

        /// <inheritdoc/>
        public ulong PackedValue { get; set; }

        /// <summary>
        /// Compares two <see cref="Short4"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Short4"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Short4"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Short4 left, Short4 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Short4"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Short4"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Short4"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Short4 left, Short4 right) => !left.Equals(right);

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector)
        {
            vector *= 65534F;
            vector -= new Vector4(32767F);
            FromVector4(vector);
        }

        /// <inheritdoc/>
        public Vector4 ToScaledVector4()
        {
            var scaled = ToVector4();
            scaled += new Vector4(32767F);
            scaled /= 65534F;
            return scaled;
        }

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => PackedValue = Pack(ref vector);

        /// <inheritdoc />
        public Vector4 ToVector4()
        {
            return new Vector4(
                (short)(PackedValue & 0xFFFF),
                (short)((PackedValue >> 0x10) & 0xFFFF),
                (short)((PackedValue >> 0x20) & 0xFFFF),
                (short)((PackedValue >> 0x30) & 0xFFFF));
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

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Short4 other && Equals(other);

        /// <inheritdoc />
        public bool Equals(Short4 other) => PackedValue.Equals(other);

        /// <summary>
        /// Gets the hash code for the current instance.
        /// </summary>
        /// <returns>Hash code for the instance.</returns>
        public override int GetHashCode() => PackedValue.GetHashCode();

        /// <inheritdoc />
        public override string ToString()
        {
            var vector = ToVector4();
            return FormattableString.Invariant($"Short4({vector.X:#0.##}, {vector.Y:#0.##}, {vector.Z:#0.##}, {vector.W:#0.##})");
        }

        private static ulong Pack(ref Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Min, Max);

            // Clamp the value between min and max values
            ulong word4 = ((ulong)Math.Round(vector.X) & 0xFFFF) << 0x00;
            ulong word3 = ((ulong)Math.Round(vector.Y) & 0xFFFF) << 0x10;
            ulong word2 = ((ulong)Math.Round(vector.Z) & 0xFFFF) << 0x20;
            ulong word1 = ((ulong)Math.Round(vector.W) & 0xFFFF) << 0x30;

            return word4 | word3 | word2 | word1;
        }
    }
}
