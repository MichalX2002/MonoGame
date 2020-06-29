// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing an 8-bit XYZ luminance an 8-bit W component.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GrayAlpha16 : IPackedPixel<GrayAlpha16, ushort>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Luminance),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Alpha));

        public byte L;
        public byte A;

        public GrayAlpha16(byte luminance, byte alpha)
        {
            L = luminance;
            A = alpha;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue
        {
            readonly get => UnsafeR.As<GrayAlpha16, ushort>(this);
            set => Unsafe.As<GrayAlpha16, ushort>(ref this) = value;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            scaledVector *= byte.MaxValue;
            scaledVector += new Vector4(0.5f);
            scaledVector.Clamp(byte.MinValue, byte.MaxValue);

            L = (byte)(LuminanceHelper.BT709.ToGrayF(scaledVector.ToVector3()) + 0.5f);
            A = (byte)scaledVector.W;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(L, L, L, A) / byte.MaxValue;
        }

        #endregion

        #region IPixel

        public void FromGray(Gray8 source)
        {
            L = source.L;
            A = byte.MaxValue;
        }

        public void FromGray(Gray16 source)
        {
            L = ScalingHelper.ToUInt8(source.L);
            A = byte.MaxValue;
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            L = source.L;
            A = source.A;
        }

        public void FromRgb(Rgb24 source)
        {
            L = LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B);
            A = byte.MaxValue;
        }

        public void FromRgba(Color source)
        {
            L = LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B);
            A = byte.MaxValue;
        }

        public void FromRgb(Rgb48 source)
        {
            L = ScalingHelper.ToUInt8(
                LuminanceHelper.BT709.ToGray16(source.R, source.G, source.B));
            A = byte.MaxValue;
        }

        public void FromRgba(Rgba64 source)
        {
            L = ScalingHelper.ToUInt8(
                LuminanceHelper.BT709.ToGray16(source.R, source.G, source.B));
            A = byte.MaxValue;
        }

        public readonly Color ToColor()
        {
            return new Color(L, A);
        }

        #endregion

        public void FromArgb(Argb32 source)
        {
            L = LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B);
            A = source.A;
        }

        public void FromBgr(Bgr24 source)
        {
            L = LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B);
            A = byte.MaxValue;
        }

        public void FromBgra(Bgra32 source)
        {
            L = LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B);
            A = source.A;
        }

        #region Equals

        public readonly bool Equals(GrayAlpha16 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is GrayAlpha16 other && Equals(other);
        }

        public static bool operator ==(GrayAlpha16 a, GrayAlpha16 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(GrayAlpha16 a, GrayAlpha16 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object overrides

        public override readonly string ToString() => nameof(GrayAlpha16) + $"(L:{L}, A:{A})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
