// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
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
    public struct GrayF : IPackedPixel<GrayF, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Luminance));

        [CLSCompliant(false)]
        public float L;

        [CLSCompliant(false)]
        public GrayF(float luminance)
        {
            L = luminance;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<GrayF, uint>(this);
            set => Unsafe.As<GrayF, uint>(ref this) = value;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            scaledVector.Clamp(0, 1);

            L = LuminanceHelper.BT709.ToGrayF(scaledVector.ToVector3());
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(L, L, L, 1);
        }

        #endregion

        #region IPixel

        public void FromGray(Gray8 source)
        {
            L = ScalingHelper.ToFloat32(source.L);
        }

        public void FromGray(Gray16 source)
        {
            L = ScalingHelper.ToFloat32(source.L);
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            L = ScalingHelper.ToFloat32(source.L);
        }

        public void FromRgb(Rgb24 source)
        {
            L = ScalingHelper.ToFloat32(LuminanceHelper.BT709.ToGray8(source));
        }

        public void FromRgba(Color source)
        {
            L = ScalingHelper.ToFloat32(LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B));
        }

        public void FromRgb(Rgb48 source)
        {
            L = ScalingHelper.ToFloat32(LuminanceHelper.BT709.ToGray16(source)); 
        }

        public void FromRgba(Rgba64 source)
        {
            L = ScalingHelper.ToFloat32(LuminanceHelper.BT709.ToGray16(source.R, source.G, source.B));
        }

        public readonly Color ToColor()
        {
            return new Color(ScalingHelper.ToUInt8(L), byte.MaxValue);
        }

        #endregion

        #region Equals

        public readonly bool Equals(GrayF other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is GrayF other && Equals(other);
        }

        public static bool operator ==(GrayF a, GrayF b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(GrayF a, GrayF b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object overrides

        public override readonly string ToString() => nameof(GrayF) + $"({L})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion

        public static implicit operator GrayF(float luminance) => new GrayF(luminance);

        public static implicit operator float(GrayF value) => value.L;
    }
}
