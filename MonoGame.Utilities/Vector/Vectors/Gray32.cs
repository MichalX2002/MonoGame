// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing a 32-bit XYZ luminance.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Gray32 : IPackedPixel<Gray32, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int32, VectorComponentChannel.Luminance));

        [CLSCompliant(false)]
        public uint L;

        [CLSCompliant(false)]
        public Gray32(uint luminance)
        {
            L = luminance;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue { readonly get => L; set => L = value; }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            scaledVector.Clamp(Vector4.Zero, Vector4.One);
            scaledVector *= uint.MaxValue;

            L = (uint)(PackedVectorHelper.GetBT709Luminance(scaledVector.ToVector3()) + 0.5f);
        }

        public readonly Vector4 ToScaledVector4()
        {
            float l = L / (float)uint.MaxValue;
            return new Vector4(l, l, l, 1);
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            L = PackedVectorHelper.UpScale8To32Bit(source.L);
        }

        public void FromGray16(Gray16 source)
        {
            L = PackedVectorHelper.UpScale16To32Bit(source.L);
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            L = PackedVectorHelper.UpScale8To32Bit(source.L);
        }

        public void FromRgb24(Rgb24 source)
        {
            L = PackedVectorHelper.UpScale8To32Bit(
                PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B));
        }

        public void FromColor(Color source)
        {
            L = PackedVectorHelper.UpScale8To32Bit(
                PackedVectorHelper.Get8BitBT709Luminance(source.R, source.G, source.B));
        }

        public void FromRgb48(Rgb48 source)
        {
            L = PackedVectorHelper.UpScale16To32Bit(
                PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B));
        }

        public void FromRgba64(Rgba64 source)
        {
            L = PackedVectorHelper.UpScale16To32Bit(
                PackedVectorHelper.Get16BitBT709Luminance(source.R, source.G, source.B));
        }

        public readonly Color ToColor()
        {
            return new Color(PackedVectorHelper.DownScale32To8Bit(L), byte.MaxValue);
        }

        #endregion

        #region Equals

        public readonly bool Equals(Gray32 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Gray32 other && Equals(other);
        }

        public static bool operator ==(Gray32 a, Gray32 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(Gray32 a, Gray32 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object Overrides

        public override readonly string ToString() => nameof(Gray32) + $"({L})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
