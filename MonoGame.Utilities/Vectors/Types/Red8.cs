// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing an 8-bit X component.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 0, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Red8 : IPackedPixel<Red8, byte>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.UInt8, VectorComponentChannel.Red));

        [CLSCompliant(false)]
        public byte R;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        /// <param name="red">The X component.</param>
        public Red8(byte red)
        {
            R = red;
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public byte PackedValue
        {
            readonly get => UnsafeR.As<Red8, byte>(this);
            set => Unsafe.As<Red8, byte>(ref this) = value;
        }

        public void FromScaledVector(Vector3 scaledVector)
        {
            R = ScalingHelper.ToUInt8(scaledVector.X);
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            R = ScalingHelper.ToUInt8(scaledVector.X);
        }

        public readonly Vector3 ToScaledVector3()
        {
            return new Vector3(ScalingHelper.ToFloat32(R), 0, 0);
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(ToScaledVector3(), 1);
        }

        #endregion

        #region IPixel

        #region From

        public void FromAlpha(Alpha8 source)
        {
            R = byte.MaxValue;
        }

        public void FromAlpha(Alpha16 source)
        {
            R = byte.MaxValue;
        }

        public void FromAlpha(AlphaF source)
        {
            R = byte.MaxValue;
        }

        public void FromGray(Gray8 source)
        {
            R = source.L;
        }

        public void FromGray(Gray16 source)
        {
            R = ScalingHelper.ToUInt8(source.L);
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            R = source.L;
        }

        public void FromGray(GrayF source)
        {
            R = ScalingHelper.ToUInt8(source.L);
        }

        public void FromRgb(Rgb24 source)
        {
            R = source.R;
        }

        public void FromRgba(Color source)
        {
            R = source.R;
        }

        public void FromRgb(Rgb48 source)
        {
            R = ScalingHelper.ToUInt8(source.R);
        }

        public void FromRgba(Rgba64 source)
        {
            R = ScalingHelper.ToUInt8(source.R);
        }

        #endregion

        #region To

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

        public readonly Gray8 ToGray8()
        {
            return new Gray8(R);
        }

        public readonly Gray16 ToGray16()
        {
            return new Gray16(ScalingHelper.ToUInt16(R));
        }

        public readonly GrayF ToGrayF()
        {
            return new GrayF(ScalingHelper.ToFloat32(R));
        }

        public readonly GrayAlpha16 ToGrayAlpha16()
        {
            return new GrayAlpha16(R, byte.MaxValue);
        }

        public readonly Rgb24 ToRgb24()
        {
            return new Rgb24(R, 0, 0);
        }

        public readonly Color ToRgba32()
        {
            return new Color(R, byte.MinValue, byte.MinValue);
        }

        public readonly Rgb48 ToRgb48()
        {
            return new Rgb48(ScalingHelper.ToUInt16(R), 0, 0);
        }

        public readonly Rgba64 ToRgba64()
        {
            return new Rgba64(ScalingHelper.ToUInt16(R), ushort.MinValue, ushort.MinValue);
        }

        #endregion

        #endregion

        #region Equals

        public readonly bool Equals(Red8 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Red8 other && Equals(other);
        }

        public static bool operator ==(Red8 a, Red8 b)
        {
            return a.R == b.R;
        }

        public static bool operator !=(Red8 a, Red8 b)
        {
            return a.R != b.R;
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Red8) + $"({R})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => R.GetHashCode();

        #endregion
    }
}
