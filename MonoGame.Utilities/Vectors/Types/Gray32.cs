// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
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
            new VectorComponent(VectorComponentType.UInt32, VectorComponentChannel.Luminance));

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

        public void FromScaledVector(Vector3 scaledVector)
        {
            scaledVector *= uint.MaxValue;
            scaledVector += new Vector3(0.5f);
            scaledVector = VectorHelper.ZeroMax(scaledVector, uint.MaxValue);

            L = (uint)(LuminanceHelper.BT709.ToGrayF(scaledVector) + 0.5f);
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            FromScaledVector(scaledVector.ToVector3());
        }

        public readonly Vector3 ToScaledVector3()
        {
            return new Vector3((float)(L / (double)uint.MaxValue));
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(ToScaledVector3(), 1);
        }

        #endregion

        #region IPixel

        public void FromAlpha(Alpha8 source)
        {
            L = uint.MaxValue;
        }

        public void FromAlpha(Alpha16 source)
        {
            L = uint.MaxValue;
        }

        public void FromAlpha(AlphaF source)
        {
            L = uint.MaxValue;
        }

        public void FromGray(Gray8 source)
        {
            L = ScalingHelper.ToUInt32(source.L);
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            L = ScalingHelper.ToUInt32(source.L);
        }

        public void FromGray(Gray16 source)
        {
            L = ScalingHelper.ToUInt32(source.L);
        }

        public void FromRgb(Rgb24 source)
        {
            L = ScalingHelper.ToUInt32(
                LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B));
        }

        public void FromRgba(Color source)
        {
            L = ScalingHelper.ToUInt32(
                LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B));
        }

        public void FromRgb(Rgb48 source)
        {
            L = LuminanceHelper.BT709.ToGray32(source.R, source.G, source.B);
        }

        public void FromRgba(Rgba64 source)
        {
            L = LuminanceHelper.BT709.ToGray32(source.R, source.G, source.B);
        }

        public readonly Alpha8 ToAlpha8()
        {
            return Alpha8.Opaque;
        }
        
        public readonly Alpha16 ToAlpha16()
        {
            return Alpha16.Opaque;
        }
        
        public readonly AlphaF ToAlphaF()
        {
            return AlphaF.Opaque;
        }

        public readonly Color ToColor()
        {
            return new Color(ScalingHelper.ToUInt8(L), byte.MaxValue);
        }

        #endregion

        #region Equals

        public readonly bool Equals(Gray32 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object? obj)
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

        [CLSCompliant(false)]
        public static implicit operator Gray32(uint luminance) => new Gray32(luminance);

        [CLSCompliant(false)]
        public static implicit operator uint(Gray32 value) => value.L;
    }
}