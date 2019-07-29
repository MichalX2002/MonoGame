// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing a single 8 bit normalized gray values.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public partial struct Gray8 : IPixel, IPackedVector<byte>
    {
        private const float Average = 1 / 3F;

        private static readonly Vector4 Accumulator = new Vector4(255f * Average, 255f * Average, 255f * Average, 0.5f);

        /// <summary>
        /// Initializes a new instance of the <see cref="Gray8"/> struct.
        /// </summary>
        /// <param name="luminance">The luminance component.</param>
        public Gray8(byte luminance) => PackedValue = luminance;

        /// <inheritdoc />
        public byte PackedValue { get; set; }

        /// <summary>
        /// Compares two <see cref="Gray8"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Gray8"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Gray8"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Gray8 left, Gray8 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Gray8"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Gray8"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Gray8"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Gray8 left, Gray8 right) => !left.Equals(right);

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
            float rgb = PackedValue / 255F;
            return new Vector4(rgb, rgb, rgb, 1F);
        }

        /// <inheritdoc/>
        public void FromArgb32(Argb32 source) => PackedValue = VectorMaths.Get8BitBT709Luminance(source.R, source.G, source.B);

        /// <inheritdoc/>
        public void FromBgr24(Bgr24 source) => PackedValue = VectorMaths.Get8BitBT709Luminance(source.R, source.G, source.B);

        /// <inheritdoc/>
        public void FromBgra32(Bgra32 source) => PackedValue = VectorMaths.Get8BitBT709Luminance(source.R, source.G, source.B);

        /// <inheritdoc/>
        public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

        /// <inheritdoc/>
        public void FromGray8(Gray8 source) => PackedValue = source.PackedValue;

        /// <inheritdoc/>
        public void FromGray16(Gray16 source) => PackedValue = VectorMaths.DownScale16To8Bit(source.PackedValue);

        /// <inheritdoc/>
        public void FromRgb24(Rgb24 source) => PackedValue = VectorMaths.Get8BitBT709Luminance(source.R, source.G, source.B);

        /// <inheritdoc />
        public void FromColor(Color source) => PackedValue = VectorMaths.Get8BitBT709Luminance(source.R, source.G, source.B);

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source)
            => PackedValue = VectorMaths.Get8BitBT709Luminance(
                VectorMaths.DownScale16To8Bit(source.R),
                VectorMaths.DownScale16To8Bit(source.G),
                VectorMaths.DownScale16To8Bit(source.B));

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source)
            => PackedValue = VectorMaths.Get8BitBT709Luminance(
                VectorMaths.DownScale16To8Bit(source.R),
                VectorMaths.DownScale16To8Bit(source.G),
                VectorMaths.DownScale16To8Bit(source.B));

        /// <inheritdoc />
        public void ToColor(ref Color dest)
        {
            dest.R = PackedValue;
            dest.G = PackedValue;
            dest.B = PackedValue;
            dest.A = byte.MaxValue;
        }
		
		#endregion

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Gray8 other && Equals(other);

        /// <inheritdoc />
        public bool Equals(Gray8 other) => PackedValue.Equals(other.PackedValue);

        /// <inheritdoc />
        public override string ToString() => $"Gray8({PackedValue})";

        /// <inheritdoc />
        public override int GetHashCode() => PackedValue.GetHashCode();

        internal void ConvertFromRgbaScaledVector4(Vector4 vector)
        {
            vector *= VectorMaths.MaxBytes;
            vector += VectorMaths.Half;
            vector = Vector4.Clamp(vector, Vector4.Zero, VectorMaths.MaxBytes);
            PackedValue = VectorMaths.Get8BitBT709Luminance((byte)vector.X, (byte)vector.Y, (byte)vector.Z);
        }
    }
}
