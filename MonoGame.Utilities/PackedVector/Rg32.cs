// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing two 16-bit unsigned normalized values ranging from 0 to 1.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Rg32 : IPixel, IPackedVector<uint>
    {
        private static readonly Vector2 Max = new Vector2(ushort.MaxValue);

        /// <summary>
        /// Initializes a new instance of the <see cref="Rg32"/> struct.
        /// </summary>
        /// <param name="x">The x-component</param>
        /// <param name="y">The y-component</param>
        public Rg32(float x, float y)
            : this(new Vector2(x, y))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rg32"/> struct.
        /// </summary>
        /// <param name="vector">The vector containing the component values.</param>
        public Rg32(Vector2 vector) => PackedValue = Pack(vector);

        /// <inheritdoc/>
        public uint PackedValue { get; set; }

        /// <summary>
        /// Compares two <see cref="Rg32"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Rg32"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Rg32"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Rg32 left, Rg32 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Rg32"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Rg32"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Rg32"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Rg32 left, Rg32 right) => !left.Equals(right);

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc />
        public void FromVector4(Vector4 vector)
        {
            var vector2 = new Vector2(vector.X, vector.Y);
            PackedValue = Pack(vector2);
        }

        /// <inheritdoc />
        public Vector4 ToVector4() => new Vector4(ToVector2(), 0F, 1F);

        /// <inheritdoc />
        public void FromArgb32(Argb32 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromBgr24(Bgr24 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromBgra32(Bgra32 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
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
        /// The vector components are typically expanded in least to greatest significance order.
        /// </summary>
        /// <returns>The <see cref="Vector2"/>.</returns>
        public Vector2 ToVector2() => new Vector2(PackedValue & 0xFFFF, (PackedValue >> 16) & 0xFFFF) / Max;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Rg32 other && Equals(other);

        /// <inheritdoc />
        public bool Equals(Rg32 other) => PackedValue.Equals(other.PackedValue);

        /// <inheritdoc />
        public override string ToString()
        {
            var vector = ToVector2();
            return FormattableString.Invariant($"Rg32({vector.X:#0.##}, {vector.Y:#0.##})");
        }

        /// <inheritdoc />
        public override int GetHashCode() => PackedValue.GetHashCode();

        private static uint Pack(Vector2 vector)
        {
            vector = Vector2.Clamp(vector, Vector2.Zero, Vector2.One) * Max;
            return (uint)(((int)Math.Round(vector.X) & 0xFFFF) | (((int)Math.Round(vector.Y) & 0xFFFF) << 16));
        }
    }
}
