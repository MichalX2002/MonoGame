// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing two 8-bit unsigned normalized values, ranging from 0 to 255.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public partial struct GrayAlpha16 : IPixel, IPackedVector<ushort>
    {
        /// <summary>
        /// Gets or sets the luminance component.
        /// </summary>
        public byte L;

        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        public byte A;

        /// <inheritdoc />
        public ushort PackedValue
        {
            get => Unsafe.As<GrayAlpha16, ushort>(ref this);
            set => Unsafe.As<GrayAlpha16, ushort>(ref this) = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GrayAlpha16"/> struct.
        /// </summary>
        /// <param name="luminance">The luminance component.</param>
        /// <param name="alpha">The alpha component.</param>
        public GrayAlpha16(byte luminance, byte alpha)
        {
            L = luminance;
            A = alpha;
        }

        /// <summary>
        /// Compares two <see cref="GrayAlpha16"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="GrayAlpha16"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="GrayAlpha16"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(GrayAlpha16 left, GrayAlpha16 right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Gray16"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Gray16"/> on the left side of the operand.</param>
        /// <param name="right">The <see cref="Gray16"/> on the right side of the operand.</param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(GrayAlpha16 left, GrayAlpha16 right) => !left.Equals(right);

        #region IPixel Implementation

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => Pack(vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => Pack(vector);

        /// <inheritdoc />
        public Vector4 ToVector4()
        {
            float scaled = L / (float)byte.MaxValue;
            return new Vector4(scaled, scaled, scaled, A);
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
            this = source.ToGrayAlpha16();
        }

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(
                PackedVectorHelper.DownScale16To8Bit(source.R),
                PackedVectorHelper.DownScale16To8Bit(source.G),
                PackedVectorHelper.DownScale16To8Bit(source.B));
        }

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(
                PackedVectorHelper.DownScale16To8Bit(source.R),
                PackedVectorHelper.DownScale16To8Bit(source.G),
                PackedVectorHelper.DownScale16To8Bit(source.B));
            A = PackedVectorHelper.DownScale16To8Bit(source.A);
        }

        /// <inheritdoc />
        public void ToGray8(ref Gray8 dest) => dest.PackedValue = L;

        /// <inheritdoc />
        public void ToGrayAlpha16(ref GrayAlpha16 dest) => dest = this;

        /// <inheritdoc />
        public void ToRgb24(ref Rgb24 dest)
        {
            dest.R = L;
            dest.G = L;
            dest.B = L;
        }

        /// <inheritdoc />
        public void ToColor(ref Color dest)
        {
            dest.R = L;
            dest.G = L;
            dest.B = L;
            dest.A = A;
        }

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is GrayAlpha16 other && Equals(other);

        /// <inheritdoc />
        public bool Equals(GrayAlpha16 other) => PackedValue.Equals(other.PackedValue);

        /// <inheritdoc />
        public override string ToString() => $"GrayAlpha({PackedValue})";

        /// <inheritdoc />
        public override int GetHashCode() => PackedValue.GetHashCode();

        private static ushort Pack(Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, PackedVectorHelper.MaxBytes);
            byte l = PackedVectorHelper.Get8BitBT709Luminance(
                (byte)vector.X, (byte)vector.Y, (byte)vector.Z);

            return (ushort)((l & 0xFFFF) | (((int)Math.Round(vector.W) & 0xFFFF) << 8));
        }
    }
}