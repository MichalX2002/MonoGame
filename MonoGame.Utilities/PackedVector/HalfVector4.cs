// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing four 16-bit floating-point values.
    /// <para>
    /// Ranges from [-1, -1, -1, -1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct HalfVector4 : IPixel, IPackedVector<ulong>
    {
        public ulong PackedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="HalfVector4"/> struct.
        /// </summary>
        /// <param name="x">The x-component.</param>
        /// <param name="y">The y-component.</param>
        /// <param name="z">The z-component.</param>
        /// <param name="w">The w-component.</param>
        public HalfVector4(float x, float y, float z, float w)
            : this(new Vector4(x, y, z, w))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HalfVector4"/> struct.
        /// </summary>
        /// <param name="vector">A vector containing the initial values for the components</param>
        public HalfVector4(Vector4 vector) => PackedValue = Pack(ref vector);

        ulong IPackedVector<ulong>.PackedValue
        {
            get => PackedValue;
            set => PackedValue = value;
        }

        /// <summary>
        /// Compares two <see cref="HalfVector4"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="HalfVector4"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="HalfVector4"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(HalfVector4 left, HalfVector4 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="HalfVector4"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="HalfVector4"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="HalfVector4"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(HalfVector4 left, HalfVector4 right) => !left.Equals(right);

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector)
        {
            vector *= 2F;
            vector -= Vector4.One;
            FromVector4(vector);
        }

        /// <inheritdoc/>
        public Vector4 ToScaledVector4()
        {
            var scaled = ToVector4();
            scaled += Vector4.One;
            scaled /= 2F;
            return scaled;
        }

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => PackedValue = Pack(ref vector);

        /// <inheritdoc />
        public Vector4 ToVector4()
        {
            return new Vector4(
                HalfTypeHelper.Unpack((ushort)PackedValue),
                HalfTypeHelper.Unpack((ushort)(PackedValue >> 0x10)),
                HalfTypeHelper.Unpack((ushort)(PackedValue >> 0x20)),
                HalfTypeHelper.Unpack((ushort)(PackedValue >> 0x30)));
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
        public override bool Equals(object obj) => obj is HalfVector4 other && Equals(other);

        /// <inheritdoc />
        public bool Equals(HalfVector4 other) => PackedValue.Equals(other.PackedValue);

        /// <inheritdoc />
        public override string ToString()
        {
            var vector = ToVector4();
            return FormattableString.Invariant($"HalfVector4({vector.X:#0.##}, {vector.Y:#0.##}, {vector.Z:#0.##}, {vector.W:#0.##})");
        }

        /// <inheritdoc />
        public override int GetHashCode() => PackedValue.GetHashCode();

        /// <summary>
        /// Packs a <see cref="Vector4"/> into a <see cref="ulong"/>.
        /// </summary>
        /// <param name="vector">The vector containing the values to pack.</param>
        /// <returns>The <see cref="ulong"/> containing the packed values.</returns>
        private static ulong Pack(ref Vector4 vector)
        {
            ulong num4 = HalfTypeHelper.Pack(vector.X);
            ulong num3 = (ulong)HalfTypeHelper.Pack(vector.Y) << 0x10;
            ulong num2 = (ulong)HalfTypeHelper.Pack(vector.Z) << 0x20;
            ulong num1 = (ulong)HalfTypeHelper.Pack(vector.W) << 0x30;
            return num4 | num3 | num2 | num1;
        }
    }
}
