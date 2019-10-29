// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing a 8-bit XYZ luminance.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Gray8 : IPackedVector<byte>, IEquatable<Gray8>, IPixel
    {
        public byte L;

        public Gray8(byte luminance) => L = luminance;

        public static bool operator ==(Gray8 left, Gray8 right) => left.Equals(right);
        public static bool operator !=(Gray8 a, Gray8 b) => !a.Equals(b);

        private static void Pack(ref Vector4 vector, out byte luminance, out byte alpha)
        {
            vector *= Vector4.MaxBytes;
            vector += Vector4.Half;
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.MaxBytes);

            luminance = PackedVectorHelper.Get8BitBT709Luminance(
                (byte)vector.X, (byte)vector.Y, (byte)vector.Z);
            alpha = (byte)vector.W;
        }

        public byte PackedValue { get => L; set => L = value; }

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => GrayAlpha16.Pack(ref vector, out L, out _);

        /// <inheritdoc />
        public Vector4 ToVector4()
        {
            float l = L / 255f;
            return new Vector4(l, l, l, 1);
        }

		#region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => Pack(ref vector);

        /// <inheritdoc/>
        public Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc/>
        public void FromGray8(Gray8 source) => L = source.L;

        /// <inheritdoc/>
        public void FromGray16(Gray16 source) =>
            PackedValue = PackedVectorHelper.DownScale16To8Bit(source.L);

        /// <inheritdoc/>
        public void FromGrayAlpha16(GrayAlpha16 source) => L = source.L;

        /// <inheritdoc/>
        public void FromRgb24(Rgb24 source) =>
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        /// <inheritdoc />
        public void FromColor(Color source) =>
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source) =>
            L = PackedVectorHelper.Get8BitBT709Luminance(
                PackedVectorHelper.DownScale16To8Bit(source.R),
                PackedVectorHelper.DownScale16To8Bit(source.G),
                PackedVectorHelper.DownScale16To8Bit(source.B));

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source) =>
            L = PackedVectorHelper.Get8BitBT709Luminance(
                PackedVectorHelper.DownScale16To8Bit(source.R),
                PackedVectorHelper.DownScale16To8Bit(source.G),
                PackedVectorHelper.DownScale16To8Bit(source.B));

        /// <inheritdoc />
        public void ToColor(ref Color destination)
        {
            destination.R = destination.G = destination.B = PackedValue;
            destination.A = byte.MaxValue;
        }

        #endregion

        public void FromArgb32(Argb32 source) =>
            PackedValue = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        public void FromBgr24(Bgr24 source) =>
            PackedValue = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        public void FromBgra32(Bgra32 source) =>
            PackedValue = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        #region Equals

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Gray8 other && Equals(other);

        /// <inheritdoc />
        public bool Equals(Gray8 other) => PackedValue.Equals(other.PackedValue);

        #endregion

        #region Object Overrides

        public override string ToString() => $"Gray8({PackedValue})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
