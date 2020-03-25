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
            new VectorComponent(VectorComponentType.Luminance, sizeof(byte) * 8));

        public byte L;

        public Gray8(byte luminance) => L = luminance;

        public byte PackedValue { readonly get => L; set => L = value; }

        public void FromVector4(in Vector4 vector) => FromScaledVector4(vector);

        public readonly void ToVector4(out Vector4 vector) => ToScaledVector4(out vector);

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            var v = scaledVector * byte.MaxValue;
            v += Vector4.Half;
            v.Clamp(0, byte.MaxValue);

            L = PackedVectorHelper.Get8BitBT709Luminance((byte)v.X, (byte)v.Y, (byte)v.Z);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            scaledVector.Base.X = scaledVector.Base.Y = scaledVector.Base.Z = L / (float)byte.MaxValue;
            scaledVector.Base.W = 1;
        }

        #region IPixel

        public void FromGray8(Gray8 source) => L = source.L;

        public void FromGray16(Gray16 source) => L = PackedVectorHelper.DownScale16To8Bit(source.L);

        public void FromGrayAlpha16(GrayAlpha16 source) => L = source.L;

        public void FromRgb24(Rgb24 source) => L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        public void FromColor(Color source) => L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

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

        public void FromArgb32(Argb32 source) => L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        public void FromBgr24(Bgr24 source) => L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        public void FromBgra32(Bgra32 source) => L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);

        #region Equals

        public static bool operator ==(Gray8 left, Gray8 right) => left.Equals(right);
        public static bool operator !=(Gray8 a, Gray8 b) => !a.Equals(b);

        public bool Equals(Gray8 other) => this == other;
        public override bool Equals(object obj) => obj is Gray8 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(Gray8) + $"({L})";

        public override int GetHashCode() => L;

        #endregion
    }
}
