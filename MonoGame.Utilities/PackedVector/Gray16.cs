// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed pixel type containing a 16-bit XYZ luminance.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    public struct Gray16 : IPackedVector<ushort>, IEquatable<Gray16>, IPixel
    {
        [CLSCompliant(false)]
        public ushort L;

        [CLSCompliant(false)]
        public Gray16(ushort luminance) => L = luminance;

        #region IPackedVector

        /// <inheritdoc />
        [CLSCompliant(false)]
        public ushort PackedValue { get => L; set => L = value; }

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => this = Pack(ref vector);

        /// <inheritdoc />
        public readonly Vector4 ToVector4()
        {
            float scaled = PackedValue / ushort.MaxValue;
            return new Vector4(scaled, scaled, scaled, 1);
        }

        #endregion

        #region IPixel

        /// <inheritdoc/>
        public void FromScaledVector4(Vector4 vector) => this = Pack(ref vector);

        /// <inheritdoc/>
        public readonly Vector4 ToScaledVector4() => ToVector4();

        /// <inheritdoc />
        public void FromGray8(Gray8 source) =>
            PackedValue = PackedVectorHelper.UpScale8To16Bit(source.PackedValue);

        /// <inheritdoc />
        public void FromGrayAlpha16(GrayAlpha16 source) => 
            PackedValue = PackedVectorHelper.UpScale8To16Bit(source.X);

        /// <inheritdoc />
        public void FromGray16(Gray16 source) => PackedValue = source.PackedValue;

        /// <inheritdoc />
        public void FromRgb24(Rgb24 source) =>
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));

        /// <inheritdoc />
        public void FromColor(Color source) =>
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));

        /// <inheritdoc/>
        public void FromRgb48(Rgb48 source) =>
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B);

        /// <inheritdoc/>
        public void FromRgba64(Rgba64 source) =>
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B);

        /// <inheritdoc />
        public readonly void ToColor(ref Color destination)
        {
            destination.R = destination.G = destination.B =
                PackedVectorHelper.DownScale16To8Bit(PackedValue);
            destination.A = byte.MaxValue;
        }

        #endregion

        public void FromArgb32(Argb32 source) =>
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));

        public void FromBgr24(Bgr24 source) =>
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));

        public void FromBgra32(Bgra32 source) =>
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));

        private static Gray16 Pack(ref Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);
            vector *= ushort.MaxValue;

            return new Gray16(
                PackedVectorHelper.Get16BitBT709Luminance(vector.X, vector.Y, vector.Z));
        }

        #region Equals

        public static bool operator ==(Gray16 a, Gray16 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Gray16 a, Gray16 b) => a.PackedValue != b.PackedValue;

        public bool Equals(Gray16 other) => this == other;
        public override bool Equals(object obj) => obj is Gray16 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => $"Gray16({PackedValue})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
