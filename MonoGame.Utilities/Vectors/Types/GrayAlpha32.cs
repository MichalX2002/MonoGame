// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing an 16-bit XYZ luminance and 16-bit W component.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GrayAlpha32 : IPackedPixel<GrayAlpha32, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Luminance),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Alpha));

        [CLSCompliant(false)]
        public ushort L;

        [CLSCompliant(false)]
        public ushort A;

        [CLSCompliant(false)]
        public GrayAlpha32(ushort luminance, ushort alpha)
        {
            L = luminance;
            A = alpha;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<GrayAlpha32, uint>(this);
            set => Unsafe.As<GrayAlpha32, uint>(ref this) = value;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            scaledVector *= ushort.MaxValue;
            scaledVector += new Vector4(0.5f);
            scaledVector = VectorHelper.ZeroMax(scaledVector, byte.MaxValue);
            
            L = (ushort)(LuminanceHelper.BT709.ToGrayF(scaledVector.ToVector3()) + 0.5f);
            A = (ushort)scaledVector.W;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(L, L, L, A) / ushort.MaxValue;
        }

        #endregion

        #region IPixel

        public void FromGray(Gray8 source)
        {
            L = ScalingHelper.ToUInt16(source.L);
            A = ushort.MaxValue;
        }

        public void FromGray(Gray16 source)
        {
            L = source.L;
            A = ushort.MaxValue;
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            L = ScalingHelper.ToUInt16(source.L);
            A = ScalingHelper.ToUInt16(source.A);
        }

        public void FromRgb(Rgb24 source)
        {
            L = ScalingHelper.ToUInt16(
                LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B));
            A = ushort.MaxValue;
        }

        public void FromRgba(Color source)
        {
            L = ScalingHelper.ToUInt16(
                LuminanceHelper.BT709.ToGray8(source.R, source.G, source.B));
            A = ushort.MaxValue;
        }

        public void FromRgb(Rgb48 source)
        {
            L = LuminanceHelper.BT709.ToGray16(source.R, source.G, source.B);
            A = ushort.MaxValue;
        }

        public void FromRgba(Rgba64 source)
        {
            L = LuminanceHelper.BT709.ToGray16(source.R, source.G, source.B);
            A = ushort.MaxValue;
        }

        public readonly Color ToColor()
        {
            return new Color(
                ScalingHelper.ToUInt8(L),
                ScalingHelper.ToUInt8(A));
        }

        #endregion

        #region Equals

        public readonly bool Equals(GrayAlpha32 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is GrayAlpha32 other && Equals(other);
        }

        public static bool operator ==(in GrayAlpha32 a, in GrayAlpha32 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in GrayAlpha32 a, in GrayAlpha32 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object overrides

        public override readonly string ToString() => nameof(GrayAlpha32) + $"(L:{L}, A:{A})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
