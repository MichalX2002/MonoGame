// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing a single 16 bit floating point value.
    /// <para>
    /// Ranges from [-1, 0, 0, 1] to [1, 0, 0, 1] in vector form.
    /// </para>
    /// </summary>
    public struct HalfSingle : IPixel, IPackedVector<ushort>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HalfSingle"/> struct.
        /// </summary>
        /// <param name="single">The single component.</param>
        public HalfSingle(float single) => PackedValue = HalfTypeHelper.Pack(single);

        /// <inheritdoc/>
        public ushort PackedValue { get; set; }

        /// <summary>
        /// Compares two <see cref="HalfSingle"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="HalfSingle"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="HalfSingle"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(HalfSingle left, HalfSingle right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="HalfSingle"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="HalfSingle"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="HalfSingle"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(HalfSingle left, HalfSingle right) => !left.Equals(right);

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector)
        {
            float scaled = vector.X;
            scaled *= 2F;
            scaled--;
            PackedValue = HalfTypeHelper.Pack(scaled);
        }

        /// <inheritdoc/>
        public Vector4 ToScaledVector4()
        {
            float single = ToSingle() + 1F;
            single /= 2F;
            return new Vector4(single, 0, 0, 1F);
        }

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => PackedValue = HalfTypeHelper.Pack(vector.X);

        /// <inheritdoc />
        public Vector4 ToVector4() => new Vector4(ToSingle(), 0, 0, 1F);

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
        /// Expands the packed representation into a <see cref="float"/>.
        /// </summary>
        /// <returns>The <see cref="float"/>.</returns>
        public float ToSingle() => HalfTypeHelper.Unpack(PackedValue);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is HalfSingle other && Equals(other);

        /// <inheritdoc />
        public bool Equals(HalfSingle other) => PackedValue.Equals(other.PackedValue);

        /// <inheritdoc />
        public override string ToString() => FormattableString.Invariant($"HalfSingle({ToSingle():#0.##})");

        /// <inheritdoc />
        public override int GetHashCode() => PackedValue.GetHashCode();
    }
}
