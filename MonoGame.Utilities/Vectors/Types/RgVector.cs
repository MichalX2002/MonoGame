using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vectors
{
    /// <summary>
    /// Packed vector type containing four 32-bit floating-point XY components.
    /// <para>
    /// Ranges from [0, 0, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RgVector : IPixel<RgVector>, IPackedVector<ulong>
    {
        public static RgVector One => new RgVector(Vector2.One);

        readonly VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Float32, VectorComponentChannel.Green));

        public Vector2 Base;

        public float R { readonly get => Base.X; set => Base.X = value; }
        public float G { readonly get => Base.Y; set => Base.Y = value; }

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a vector.
        /// </summary>
        public RgVector(Vector2 vector)
        {
            Base = vector;
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public RgVector(float r, float g) : this(new Vector2(r, g))
        {
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ulong PackedValue
        {
            readonly get => UnsafeR.As<RgVector, ulong>(this);
            set => Unsafe.As<RgVector, ulong>(ref this) = value;
        }

        public void FromScaledVector(Vector3 scaledVector) => Base = scaledVector.ToVector2();
        public void FromScaledVector(Vector4 scaledVector) => Base = scaledVector.ToVector2();

        public readonly Vector3 ToScaledVector3() => Base.ToVector3();
        public readonly Vector4 ToScaledVector4() => Base.ToVector4();

        public readonly Vector3 ToVector3() => ToScaledVector3();
        public readonly Vector4 ToVector4() => ToScaledVector4();

        #endregion

        #region IPixel.From

        public void FromAlpha(Alpha8 source) => this = One;
        public void FromAlpha(Alpha16 source) => this = One;
        public void FromAlpha(Alpha32 source) => this = One;
        public void FromAlpha(AlphaF source) => this = One;

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
        public readonly bool Equals(ulong other) => PackedValue == other;

        public readonly bool Equals(RgVector other) => Base.Equals(other.Base);

        public static bool operator ==(RgVector a, RgVector b) => a.Base == b.Base;
        public static bool operator !=(RgVector a, RgVector b) => a.Base != b.Base;

        #endregion

        #region Object overrides

        public override readonly bool Equals(object? obj) => obj is RgVector other && Equals(other);

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => Base.GetHashCode();

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(RgVector) + $"({Base})";

        #endregion

        public static implicit operator RgVector(in Vector2 vector) => UnsafeR.As<Vector2, RgVector>(vector);
        public static implicit operator Vector2(in RgVector vector) => UnsafeR.As<RgVector, Vector2>(vector);
    }
}
