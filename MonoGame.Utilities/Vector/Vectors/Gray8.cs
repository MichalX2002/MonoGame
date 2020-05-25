// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing a 8-bit XYZ luminance.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Gray8 : IPackedPixel<Gray8, byte>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Luminance));

        public byte L;

        public Gray8(byte luminance)
        {
            L = luminance;
        }

        #region IPackedVector

        public byte PackedValue { readonly get => L; set => L = value; }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            scaledVector *= byte.MaxValue;
            scaledVector += Vector4.Half;
            scaledVector.Clamp(Vector4.Zero, Vector4.MaxValueByte);

            L = (byte)(PackedVectorHelper.GetBT709Luminance(scaledVector.ToVector3()) + 0.5f);
        }

        public readonly Vector4 ToScaledVector4()
        {
            float l = L / (float)byte.MaxValue;
            return new Vector4(l, l, l, 1);
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            L = source.L;
        }

        public void FromGray16(Gray16 source)
        {
            L = PackedVectorHelper.DownScale16To8Bit(source.L);
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            L = source.L;
        }

        public void FromRgb24(Rgb24 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);
        }

        public void FromRgba32(Color source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);
        }

        public void FromRgb48(Rgb48 source)
        {
            L = PackedVectorHelper.DownScale16To8Bit(
                PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B));
        }

        public void FromRgba64(Rgba64 source)
        {
            L = PackedVectorHelper.DownScale16To8Bit(
                PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B));
        }

        public readonly Color ToColor()
        {
            return new Color(L, byte.MaxValue);
        }

        #endregion

        public void FromArgb32(Argb32 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);
        }

        public void FromBgr24(Bgr24 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);
        }

        public void FromBgra32(Bgra32 source)
        {
            L = PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B);
        }

        #region Equals

        public readonly bool Equals(Gray8 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Gray8 other && Equals(other);
        }

        public static bool operator ==(Gray8 a, Gray8 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(Gray8 a, Gray8 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object Overrides

        public override readonly string ToString() => nameof(Gray8) + $"({L})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion

        public static implicit operator Gray8(byte luminance) => new Gray8(luminance);

        public static implicit operator byte(Gray8 value) => value.L;
    }
}
