// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed pixel type containing a 16-bit XYZ luminance.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Gray16 : IPackedVector<ushort>, IEquatable<Gray16>, IPixel
    {
        public int BitDepth => Unsafe.SizeOf<Gray16>() * 8;

        [CLSCompliant(false)]
        public ushort L;

        [CLSCompliant(false)]
        public Gray16(ushort luminance) => L = luminance;

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { readonly get => L; set => L = value; }

        public void FromVector4(Vector4 vector) => this = Pack(ref vector);

        public readonly Vector4 ToVector4()
        {
            float scaled = PackedValue / ushort.MaxValue;
            return new Vector4(scaled, scaled, scaled, 1);
        }

        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector) => this = Pack(ref vector);

        public readonly Vector4 ToScaledVector4() => ToVector4();

        public void FromGray8(Gray8 source) =>
            PackedValue = PackedVectorHelper.UpScale8To16Bit(source.PackedValue);

        public void FromGrayAlpha16(GrayAlpha88 source) => 
            PackedValue = PackedVectorHelper.UpScale8To16Bit(source.L);

        public void FromGray16(Gray16 source) => PackedValue = source.PackedValue;

        public void FromRgb24(Rgb24 source) =>
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));

        public void FromColor(Color source) =>
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));

        public void FromRgb48(Rgb48 source) =>
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B);

        public void FromRgba64(Rgba64 source) =>
            PackedValue = PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B);

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

        public override string ToString() => nameof(Gray16) + $"({PackedValue})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
