// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
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

        public void FromScaledVector(Vector3 scaledVector)
        {
            scaledVector *= byte.MaxValue;
            scaledVector += new Vector3(0.5f);
            scaledVector.Clamp(byte.MinValue, byte.MaxValue);

            L = (byte)(LuminanceHelper.BT709.ToGrayF(scaledVector) + 0.5f);
        }

        public readonly Vector3 ToScaledVector3()
        {
            float l = L / (float)byte.MaxValue;
            return new Vector3(l, l, l);
        }

        #endregion

        #region IPixel

        public void FromGray(Gray8 source)
        {
            L = source.L;
        }

        public void FromGray(Gray16 source)
        {
            L = ScalingHelper.ToUInt8(source.L);
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            L = source.L;
        }

        public void FromRgb(Rgb24 source)
        {
            L = LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B);
        }

        public void FromRgba(Color source)
        {
            L = LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B);
        }

        public void FromRgb(Rgb48 source)
        {
            L = ScalingHelper.ToUInt8(
                LuminanceHelper.BT709.ToGray16(source.R, source.G, source.B));
        }

        public void FromRgba(Rgba64 source)
        {
            L = ScalingHelper.ToUInt8(
                LuminanceHelper.BT709.ToGray16(source.R, source.G, source.B));
        }

        public readonly Color ToColor()
        {
            return new Color(L, byte.MaxValue);
        }

        #endregion

        public void FromArgb(Argb32 source)
        {
            L = LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B);
        }

        public void FromBgr(Bgr24 source)
        {
            L = LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B);
        }

        public void FromBgra(Bgra32 source)
        {
            L = LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B);
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

        #region Object overrides

        public override readonly string ToString() => nameof(Gray8) + $"({L})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion

        public static implicit operator Gray8(byte luminance) => new Gray8(luminance);

        public static implicit operator byte(Gray8 value) => value.L;
    }
}
