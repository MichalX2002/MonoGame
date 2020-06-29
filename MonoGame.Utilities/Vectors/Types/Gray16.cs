// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
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

        public void FromScaledVector(Vector4 scaledVector)
        {
            scaledVector *= ushort.MaxValue;
            scaledVector += new Vector4(0.5f);
            scaledVector.Clamp(ushort.MinValue, ushort.MaxValue);

            L = (ushort)(LuminanceHelper.BT709.ToGrayF(scaledVector.ToVector3()) + 0.5f);
        }

        public readonly Vector4 ToScaledVector4()
        {
            float l = L / (float)ushort.MaxValue;
            return new Vector4(l, l, l, 1);
        }

        #endregion

        #region IPixel

        public void FromGray(Gray8 source)
        {
            L = ScalingHelper.ToUInt16(source.L);
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            L = ScalingHelper.ToUInt16(source.L);
        }

        public void FromGray(Gray16 source)
        {
            L = source.L;
        }

        public void FromRgb(Rgb24 source)
        {
            L = ScalingHelper.ToUInt16(
                LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B));
        }

        public void FromRgba(Color source)
        {
            L = ScalingHelper.ToUInt16(
                LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B));
        }

        public void FromRgb(Rgb48 source)
        {
            L = LuminanceHelper.BT709.ToGray16(source.R, source.G, source.B);
        }

        public void FromRgba(Rgba64 source)
        {
            L = LuminanceHelper.BT709.ToGray16(source.R, source.G, source.B);
        }

        public readonly Color ToColor()
        {
            return new Color(ScalingHelper.ToUInt8(L), byte.MaxValue);
        }

        #endregion

        public void FromArgb(Argb32 source)
        {
            L = LuminanceHelper.BT709.ToGray16(
                ScalingHelper.ToUInt16(source.R),
                ScalingHelper.ToUInt16(source.G),
                ScalingHelper.ToUInt16(source.B));
        }

        public void FromBgr(Bgr24 source)
        {
            L = LuminanceHelper.BT709.ToGray16(
                ScalingHelper.ToUInt16(source.R),
                ScalingHelper.ToUInt16(source.G),
                ScalingHelper.ToUInt16(source.B));
        }

        public void FromBgra(Bgra32 source)
        {
            L = LuminanceHelper.BT709.ToGray16(
                ScalingHelper.ToUInt16(source.R),
                ScalingHelper.ToUInt16(source.G),
                ScalingHelper.ToUInt16(source.B));
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

        #region Object overrides

        public override readonly string ToString() => nameof(Gray16) + $"({L})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion

        [CLSCompliant(false)]
        public static implicit operator Gray16(ushort luminance) => new Gray16(luminance);

        [CLSCompliant(false)]
        public static implicit operator ushort(Gray16 value) => value.L;
    }
}
