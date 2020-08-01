﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing signed 8-bit XY components.
    /// <para>
    /// Ranges from [-1, -1, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NormalizedByte2 : IPixel<NormalizedByte2>, IPackedVector<ushort>
    {
        internal static Vector2 Offset => new Vector2(-sbyte.MinValue);

        public static NormalizedByte2 MaxValue => new NormalizedByte2(sbyte.MaxValue, sbyte.MaxValue);
        
        readonly VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Green));

        [CLSCompliant(false)]
        public sbyte X;

        [CLSCompliant(false)]
        public sbyte Y;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte2(sbyte x, sbyte y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte2(ushort packed) : this()
        {
            // TODO: Unsafe.SkipInit(out this);
            PackedValue = packed;
        }

        #endregion

        public readonly Vector2 ToVector2()
        {
            var vector = new Vector2(X, Y);
            vector += Offset;
            vector /= 2f * byte.MaxValue;
            vector -= Vector2.One;

            return vector;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue
        {
            readonly get => UnsafeR.As<NormalizedByte2, ushort>(this);
            set => Unsafe.As<NormalizedByte2, ushort>(ref this) = value;
        }

        public void FromScaledVector(Vector3 scaledVector)
        {
            var raw = scaledVector.ToVector2();
            raw = VectorHelper.ScaledClamp(raw);
            raw *= byte.MaxValue;
            raw -= Offset;

            X = (sbyte)raw.X;
            Y = (sbyte)raw.Y;
        }

        public void FromScaledVector(Vector4 scaledVector) => FromScaledVector(scaledVector.ToVector3());
        
        public void FromVector(Vector3 vector)
        {
            var raw = vector.ToVector2();
            raw = Vector2.Clamp(raw, -Vector2.One, Vector2.One);
            raw *= byte.MaxValue / 2f;
            raw = VectorHelper.Round(raw);

            X = (sbyte)raw.X;
            Y = (sbyte)raw.Y;
        }

        public void FromVector(Vector4 vector) => FromVector(vector.ToVector3());

        public readonly Vector3 ToScaledVector3()
        {
            var scaled = new Vector2(X, Y);
            scaled += Offset;
            scaled /= byte.MaxValue;

            return new Vector3(scaled, 0);
        }

        public readonly Vector4 ToScaledVector4() => new Vector4(ToScaledVector3(), 1f);

        public readonly Vector3 ToVector3() => new Vector3(ToVector2(), 0f);
        public readonly Vector4 ToVector4() => new Vector4(ToVector3(), 1f);

        #endregion

        #region IPixel.From

        public void FromAlpha(Alpha8 source) => this = MaxValue;
        public void FromAlpha(Alpha16 source) => this = MaxValue;
        public void FromAlpha(Alpha32 source) => this = MaxValue;
        public void FromAlpha(AlphaF source) => this = MaxValue;

        #endregion

        #region IPixel.To

        public readonly Alpha8 ToAlpha8() => Alpha8.Opaque;
        public readonly Alpha16 ToAlpha16() => Alpha16.Opaque;
        public readonly AlphaF ToAlphaF() => AlphaF.Opaque;

        public readonly Gray8 ToGray8() => PixelHelper.ToGray8(this);
        public readonly Gray16 ToGray16() => PixelHelper.ToGray16(this);
        public readonly GrayF ToGrayF() => PixelHelper.ToGrayF(this);
        public readonly GrayAlpha16 ToGrayAlpha16() => PixelHelper.ToGrayAlpha16(this);

        public readonly Rgb24 ToRgb24() => ScaledVectorHelper.ToRgb24(this);
        public readonly Rgb48 ToRgb48() => ScaledVectorHelper.ToRgb48(this);

        public readonly Color ToRgba32() => ScaledVectorHelper.ToRgba32(this);
        public readonly Rgba64 ToRgba64() => ScaledVectorHelper.ToRgba64(this);

        #endregion

        #region Equals

        [CLSCompliant(false)]
        public readonly bool Equals(ushort other) => PackedValue == other;

        public readonly bool Equals(NormalizedByte2 other) => this == other;

        public static bool operator ==(NormalizedByte2 a, NormalizedByte2 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(NormalizedByte2 a, NormalizedByte2 b) => a.PackedValue != b.PackedValue;

        #endregion

        #region Object overrides

        public override readonly bool Equals(object? obj) => obj is NormalizedByte2 other && Equals(other);

        public override readonly int GetHashCode() => HashCode.Combine(PackedValue);

        public override readonly string ToString() => nameof(NormalizedByte2) + $"({X}, {Y})";

        #endregion
    }
}
