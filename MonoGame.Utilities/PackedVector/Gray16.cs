// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing a single 16-bit normalized gray value.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public partial struct Gray16 : IPixel, IPackedVector<ushort>
    {
        private const float Max = ushort.MaxValue;

        [CLSCompliant(false)]
        public ushort PackedValue;

        /// <inheritdoc />
        ushort IPackedVector<ushort>.PackedValue
        {
            get => PackedValue;
            set => PackedValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gray16"/> struct.
        /// </summary>
        /// <param name="luminance">The luminance component</param>
        [CLSCompliant(false)]
        public Gray16(ushort luminance) => PackedValue = luminance;

        /// <summary>
        /// Compares two <see cref="Gray16"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Gray16"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Gray16"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Gray16 left, Gray16 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Gray16"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Gray16"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Gray16"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Gray16 left, Gray16 right) => !left.Equals(right);

        #region IPixel Implementation

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => ConvertFromRgbaScaledVector4(vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => ConvertFromRgbaScaledVector4(vector);

        /// <inheritdoc />
        public Vector4 ToVector4()
        {
            float scaled = PackedValue / Max;
            return new Vector4(scaled, scaled, scaled, 1F);
        }

        /// <inheritdoc/>
        public void FromArgb32(Argb32 source)
        {
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));
        }

        /// <inheritdoc/>
        public void FromBgr24(Bgr24 source)
        {
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));
        }

        /// <inheritdoc/>
        public void FromBgra32(Bgra32 source)
        {
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));
        }

        /// <inheritdoc/>
        public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc />
        public void FromGray8(Gray8 source) => PackedValue = PackedVectorHelper.UpScale8To16Bit(source.PackedValue);

        /// <inheritdoc />
        public void FromGray16(Gray16 source) => PackedValue = source.PackedValue;

        /// <inheritdoc />
        public void FromRgb24(Rgb24 source)
        {
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));
        }

        /// <inheritdoc />
        public void FromColor(Color source)
        {
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));
        }

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source) => PackedValue = PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B);

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source) => PackedValue = PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B);

        /// <inheritdoc />
        public void ToColor(ref Color dest)
        {
            byte rgb = PackedVectorHelper.DownScale16To8Bit(PackedValue);
            dest.R = rgb;
            dest.G = rgb;
            dest.B = rgb;
            dest.A = byte.MaxValue;
        }

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Gray16 other && Equals(other);

        /// <inheritdoc />
        public bool Equals(Gray16 other) => PackedValue.Equals(other.PackedValue);

        /// <inheritdoc />
        public override string ToString() => $"Gray16({PackedValue})";

        /// <inheritdoc />
        public override int GetHashCode() => PackedValue.GetHashCode();

        internal void ConvertFromRgbaScaledVector4(Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One) * Max;
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(
                vector.X,
                vector.Y,
                vector.Z);
        }
    }
}
