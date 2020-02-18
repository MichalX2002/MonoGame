// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing a 8-bit XYZ luminance.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Gray8 : IPackedVector<byte>, IEquatable<Gray8>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Gray, sizeof(byte) * 8));

        public byte L;

        public Gray8(byte luminance) => L = luminance;

        private static void Pack(ref Vector4 vector, out byte luminance, out byte alpha)
        {
            vector *= Vector4.MaxByteValue;
            vector += Vector4.Half;
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.MaxByteValue);

            luminance = PackedVectorHelper.Get8BitBT709Luminance(
                (byte)vector.X, (byte)vector.Y, (byte)vector.Z);
            alpha = (byte)vector.W;
        }

        public byte PackedValue { readonly get => L; set => L = value; }

        public void FromVector4(Vector4 vector) => Pack(ref vector, out L, out _);

        public readonly Vector4 ToVector4()
        {
            float l = L / 255f;
            return new Vector4(l, l, l, 1);
        }

        #region IPixel

        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        public readonly Vector4 ToScaledVector4() => ToVector4();

        public void FromGray8(Gray8 source) => L = source.L;

        public void FromGray16(Gray16 source) =>
            L = PackedVectorHelper.DownScale16To8Bit(source.L);

        public void FromGrayAlpha16(GrayAlpha16 source) => L = source.L;

        public void FromRgb24(Rgb24 source) =>
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        public void FromColor(Color source) =>
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        public void FromRgb48(Rgb48 source) =>
            L = PackedVectorHelper.Get8BitBT709Luminance(
                PackedVectorHelper.DownScale16To8Bit(source.R),
                PackedVectorHelper.DownScale16To8Bit(source.G),
                PackedVectorHelper.DownScale16To8Bit(source.B));

        public void FromRgba64(Rgba64 source) =>
            L = PackedVectorHelper.Get8BitBT709Luminance(
                PackedVectorHelper.DownScale16To8Bit(source.R),
                PackedVectorHelper.DownScale16To8Bit(source.G),
                PackedVectorHelper.DownScale16To8Bit(source.B));

        public readonly void ToColor(ref Color destination)
        {
            destination.R = destination.G = destination.B = L;
            destination.A = byte.MaxValue;
        }

        #endregion

        public void FromArgb32(Argb32 source) =>
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        public void FromBgr24(Bgr24 source) =>
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        public void FromBgra32(Bgra32 source) =>
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        #region Equals

        public static bool operator ==(Gray8 left, Gray8 right) => left.Equals(right);
        public static bool operator !=(Gray8 a, Gray8 b) => !a.Equals(b);

        public bool Equals(Gray8 other) => this == other;
        public override bool Equals(object obj) => obj is Gray8 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(Gray8) + $"({L.ToString()})";

        public override int GetHashCode() => L;

        #endregion
    }
}
