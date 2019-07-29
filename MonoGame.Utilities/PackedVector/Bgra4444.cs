// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Microsoft.Xna.Framework.Graphics.PackedVector
{
    /// <summary>
    /// Packed pixel type containing unsigned normalized values, ranging from 0 to 1, using 4 bits each for x, y, z, and w.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Bgra4444 : IPixel, IPackedVector<ushort>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bgra4444"/> struct.
        /// </summary>
        /// <param name="x">The x-component</param>
        /// <param name="y">The y-component</param>
        /// <param name="z">The z-component</param>
        /// <param name="w">The w-component</param>
        public Bgra4444(float x, float y, float z, float w)
            : this(new Vector4(x, y, z, w))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bgra4444"/> struct.
        /// </summary>
        /// <param name="vector">The vector containing the components for the packed vector.</param>
        public Bgra4444(Vector4 vector) => PackedValue = Pack(ref vector);

        /// <inheritdoc/>
        public ushort PackedValue { get; set; }

        /// <summary>
        /// Compares two <see cref="Bgra4444"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Bgra4444"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Bgra4444"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Bgra4444 left, Bgra4444 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Bgra4444"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Bgra4444"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Bgra4444"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Bgra4444 left, Bgra4444 right) => !left.Equals(right);

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => PackedValue = Pack(ref vector);

        /// <inheritdoc />
        public Vector4 ToVector4()
        {
            const float Max = 1 / 15F;

            return new Vector4(
                (PackedValue >> 8) & 0x0F,
                (PackedValue >> 4) & 0x0F,
                PackedValue & 0x0F,
                (PackedValue >> 12) & 0x0F) * Max;
        }

        /// <inheritdoc />
        public void FromArgb32(Argb32 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromBgr24(Bgr24 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromBgra32(Bgra32 source) => FromScaledVector4(source.ToScaledVector4());

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
        public override bool Equals(object obj) => obj is Bgra4444 other && Equals(other);

        /// <inheritdoc />
        public bool Equals(Bgra4444 other) => PackedValue.Equals(other.PackedValue);

        /// <inheritdoc />
        public override string ToString()
        {
            var vector = ToVector4();
            return FormattableString.Invariant($"Bgra4444({vector.Z:#0.##}, {vector.Y:#0.##}, {vector.X:#0.##}, {vector.W:#0.##})");
        }

        /// <inheritdoc />
        public override int GetHashCode() => PackedValue.GetHashCode();

        private static ushort Pack(ref Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);
            return (ushort)((((int)Math.Round(vector.W * 15F) & 0x0F) << 12)
                          | (((int)Math.Round(vector.X * 15F) & 0x0F) << 8)
                          | (((int)Math.Round(vector.Y * 15F) & 0x0F) << 4)
                          | ((int)Math.Round(vector.Z * 15F) & 0x0F));
        }
    }
}
