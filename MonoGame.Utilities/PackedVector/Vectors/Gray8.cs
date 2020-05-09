// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
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

        public void FromVector4(in Vector4 vector)
        {
            FromScaledVector4(vector);
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            ToScaledVector4(out vector);
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            var v = scaledVector * byte.MaxValue;
            v += Vector4.Half;
            v.Clamp(Vector4.Zero, Vector4.MaxValueByte);

            L = (byte)(PackedVectorHelper.GetBT709Luminance(v.X, v.Y, v.Z) + 0.5f);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            scaledVector.Base.X = scaledVector.Base.Y = scaledVector.Base.Z = L / (float)byte.MaxValue;
            scaledVector.Base.W = 1;
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

        public void FromColor(Color source)
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

        public readonly void ToColor(out Color destination)
        {
            destination.R = destination.G = destination.B = L;
            destination.A = byte.MaxValue;
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

        public static bool operator ==(Gray8 a, Gray8 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(Gray8 a, Gray8 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        public bool Equals(Gray8 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Gray8 other && Equals(other);
        }

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(Gray8) + $"({L})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
