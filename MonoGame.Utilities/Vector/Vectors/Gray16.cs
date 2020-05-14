// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing a 16-bit XYZ luminance.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Gray16 : IPackedPixel<Gray16, ushort>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Luminance));

        [CLSCompliant(false)]
        public ushort L;

        [CLSCompliant(false)]
        public Gray16(ushort luminance)
        {
            L = luminance;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { readonly get => L; set => L = value; }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            scaledVector *= ushort.MaxValue;
            scaledVector += Vector4.Half;
            scaledVector.Clamp(Vector4.Zero, Vector4.MaxValueUInt16);

            L = (ushort)(PackedVectorHelper.GetBT709Luminance(scaledVector.ToVector3()) + 0.5f);
        }

        public readonly Vector4 ToScaledVector4()
        {
            float l = L / (float)ushort.MaxValue;
            return new Vector4(l, l, l, 1);
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            L = PackedVectorHelper.UpScale8To16Bit(source.L);
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            L = PackedVectorHelper.UpScale8To16Bit(source.L);
        }

        public void FromGray16(Gray16 source)
        {
            L = source.L;
        }

        public void FromRgb24(Rgb24 source)
        {
            L = PackedVectorHelper.UpScale8To16Bit(
                PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B));
        }

        public void FromColor(Color source)
        {
            L = PackedVectorHelper.UpScale8To16Bit(
                PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B));
        }

        public void FromRgb48(Rgb48 source)
        {
            L = PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B);
        }

        public void FromRgba64(Rgba64 source)
        {
            L = PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B);
        }

        public readonly Color ToColor()
        {
            return new Color(PackedVectorHelper.DownScale16To8Bit(L), byte.MaxValue);
        }

        #endregion

        public void FromArgb32(Argb32 source)
        {
            L = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));
        }

        public void FromBgr24(Bgr24 source)
        {
            L = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));
        }

        public void FromBgra32(Bgra32 source)
        {
            L = PackedVectorHelper.Get16BitBT709Luminance(
                PackedVectorHelper.UpScale8To16Bit(source.R),
                PackedVectorHelper.UpScale8To16Bit(source.G),
                PackedVectorHelper.UpScale8To16Bit(source.B));
        }

        #region Equals

        public readonly bool Equals(Gray16 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Gray16 other && Equals(other);
        }

        public static bool operator ==(Gray16 a, Gray16 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(Gray16 a, Gray16 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object Overrides

        public override readonly string ToString() => nameof(Gray16) + $"({L})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
