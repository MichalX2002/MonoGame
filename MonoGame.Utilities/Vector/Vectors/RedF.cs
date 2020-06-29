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
    /// Packed vector type containing an 32-bit X component.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 0, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RedF : IPackedPixel<RedF, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Red));

        [CLSCompliant(false)]
        public float R;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        /// <param name="red">The X component.</param>
        public RedF(float red)
        {
            R = red;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public RedF(uint value) : this()
        {
            // TODO: Unsafe.SkipInit(out this)
            PackedValue = value;
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<RedF, uint>(this);
            set => Unsafe.As<RedF, uint>(ref this) = value;
        }

        public void FromScaledVector(Vector3 scaledVector)
        {
            R = scaledVector.X;
        }

        public readonly Vector3 ToScaledVector3()
        {
            return new Vector3(R, 0, 0);
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            R = scaledVector.X;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(R, 0, 0, 1);
        }

        #endregion

        #region IPixel

        #region From

        public void FromAlpha(Alpha8 source)
        {
            R = 0;
        }

        public void FromAlpha(Alpha16 source)
        {
            R = 0;
        }

        public void FromAlpha(AlphaF source)
        {
            R = 0;
        }

        public void FromGray(Gray8 source)
        {
            R = ScalingHelper.ToFloat32(source.L);
        }

        public void FromGray(Gray16 source)
        {
            R = ScalingHelper.ToFloat32(source.L);
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            R = ScalingHelper.ToFloat32(source.L);
        }

        public void FromRgb(Rgb24 source)
        {
            R = ScalingHelper.ToFloat32(source.R);
        }

        public void FromRgba(Color source)
        {
            R = ScalingHelper.ToFloat32(source.R);
        }

        public void FromRgb(Rgb48 source)
        {
            R = ScalingHelper.ToFloat32(source.R);
        }

        public void FromRgba(Rgba64 source)
        {
            R = ScalingHelper.ToFloat32(source.R);
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
            return new Gray8(ScalingHelper.ToUInt8(R));
        }

        public readonly Gray16 ToGray16()
        {
            return new Gray16(ScalingHelper.ToUInt16(R));
        }

        public readonly GrayF ToGrayF()
        {
            return new GrayF(R);
        }

        public readonly GrayAlpha16 ToGrayAlpha16()
        {
            return new GrayAlpha16(ScalingHelper.ToUInt8(R), byte.MaxValue);
        }

        public readonly Rgb24 ToRgb24()
        {
            return new Rgb24(ScalingHelper.ToUInt8(R), 0, 0);
        }

        public readonly Color ToRgba32()
        {
            return new Color(ScalingHelper.ToUInt8(R), byte.MinValue, byte.MinValue);
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

        public readonly bool Equals(RedF other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is RedF other && Equals(other);
        }

        public static bool operator ==(RedF a, RedF b)
        {
            return a.R == b.R;
        }

        public static bool operator !=(RedF a, RedF b)
        {
            return a.R != b.R;
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(RedF) + $"({R})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => R.GetHashCode();

        #endregion
    }
}
