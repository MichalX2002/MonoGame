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
    /// Packed vector type containing an 32-bit W component.
    /// <para>
    /// Ranges from [1, 1, 1, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AlphaF : IPackedPixel<AlphaF, uint>
    {
        public static AlphaF Transparent => default;
        public static AlphaF Opaque => new AlphaF(1f);

        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Alpha));

        [CLSCompliant(false)]
        public float A;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a raw value.
        /// </summary>
        /// <param name="alpha">The W component.</param>
        public AlphaF(float alpha)
        {
            A = alpha;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public AlphaF(uint value) : this()
        {
            // TODO: Unsafe.SkipInit(out this)
            PackedValue = value;
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<AlphaF, uint>(this);
            set => Unsafe.As<AlphaF, uint>(ref this) = value;
        }

        public void FromScaledVector(Vector3 scaledVector)
        {
            A = 1f;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            A = scaledVector.W;
        }

        public readonly Vector3 ToScaledVector3()
        {
            return Vector3.One;
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(1, 1, 1, A);
        }

        #endregion

        #region IPixel.From

        public void FromAlpha(Alpha8 source)
        {
            A = ScalingHelper.ToFloat32(source.A);
        }

        public void FromAlpha(Alpha16 source)
        {
            A = ScalingHelper.ToFloat32(source.A);
        }

        public void FromAlpha(AlphaF source)
        {
            this = source;
        }

        public void FromGray(Gray8 source)
        {
            A = 1f;
        }

        public void FromGray(Gray16 source)
        {
            A = 1f;
        }

        public void FromGrayAlpha(GrayAlpha16 source)
        {
            A = ScalingHelper.ToFloat32(source.A);
        }

        public void FromRgb(Rgb24 source)
        {
            A = 1f;
        }

        public void FromRgba(Color source)
        {
            A = ScalingHelper.ToFloat32(source.A);
        }

        public void FromRgb(Rgb48 source)
        {
            A = 1f;
        }

        public void FromRgba(Rgba64 source)
        {
            A = ScalingHelper.ToFloat32(source.A);
        }

        #endregion

        #region IPixel.To

        public readonly Alpha8 ToAlpha8()
        {
            return ScalingHelper.ToUInt8(A);
        }

        public readonly Alpha16 ToAlpha16()
        {
            return ScalingHelper.ToUInt16(A);
        }

        public readonly AlphaF ToAlphaF()
        {
            return this;
        }

        public readonly Rgb24 ToRgb24()
        {
            return Rgb24.White;
        }

        public readonly Color ToRgba32()
        {
            return new Color(byte.MaxValue, ScalingHelper.ToUInt8(A));
        }

        public readonly Rgb48 ToRgb48()
        {
            return Rgb48.White;
        }

        public readonly Rgba64 ToRgba64()
        {
            return new Rgba64(ushort.MaxValue, ScalingHelper.ToUInt16(A));
        }

        #endregion

        #region Equals

        public readonly bool Equals(AlphaF other) => this == other;

        public override readonly bool Equals(object obj) => obj is AlphaF other && Equals(other);

        public static bool operator ==(AlphaF a, AlphaF b) => a.A == b.A;

        public static bool operator !=(AlphaF a, AlphaF b) => a.A != b.A;

        #endregion

        #region Object overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(AlphaF) + $"({A})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => A.GetHashCode();

        #endregion

        public static implicit operator AlphaF(float alpha) => new AlphaF(alpha);

        public static implicit operator float(AlphaF value) => value.A;
    }
}
