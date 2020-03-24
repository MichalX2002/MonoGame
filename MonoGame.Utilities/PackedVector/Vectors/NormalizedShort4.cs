// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing signed 16-bit XYZW components.
    /// <para>
    /// Ranges from [-1, -1, -1, -1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NormalizedShort4 : IPackedVector<ulong>, IEquatable<NormalizedShort4>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(short) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(short) * 8),
            new VectorComponent(VectorComponentType.Blue, sizeof(short) * 8),
            new VectorComponent(VectorComponentType.Alpha, sizeof(short) * 8));

        public short X;
        public short Y;
        public short Z;
        public short W;

        #region Constructors

        public NormalizedShort4(short x, short y, short z, short w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        [CLSCompliant(false)]
        public NormalizedShort4(ulong packed) : this() => PackedValue = packed;

        public NormalizedShort4(in Vector4 vector) : this() => FromVector4(vector);

        public NormalizedShort4(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ulong PackedValue
        {
            readonly get => UnsafeUtils.As<NormalizedShort4, ulong>(this);
            set => Unsafe.As<NormalizedShort4, ulong>(ref this) = value;
        }

        public void FromVector4(in Vector4 vector)
        {
            Vector4.Clamp(vector, -1, 1, out var v);
            v *= short.MaxValue;

            X = (short)v.X;
            Y = (short)v.Y;
            Z = (short)v.Z;
            W = (short)v.W;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = X;
            vector.Base.Y = Y;
            vector.Base.Z = Z;
            vector.Base.W = W;
            vector /= short.MaxValue;
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            Vector4.Multiply(scaledVector, 2, out var v);
            Vector4.Subtract(v, Vector4.One, out v);
            FromVector4(v);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
            Vector4.Add(scaledVector, Vector4.One, out scaledVector);
            Vector4.Divide(scaledVector, 2, out scaledVector);
        }

        #endregion

        #region IPixel

        public void FromGray8(Gray8 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromGray16(Gray16 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromGrayAlpha16(GrayAlpha16 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromRgb24(Rgb24 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromRgb48(Rgb48 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromRgba64(Rgba64 source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public void FromColor(Color source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

        public readonly void ToColor(ref Color destination)
        {
            ToScaledVector4(out var vector);
            destination.FromScaledVector4(vector);
        }

        #endregion

        #region Equals

        public static bool operator ==(NormalizedShort4 a, NormalizedShort4 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(NormalizedShort4 a, NormalizedShort4 b) => a.PackedValue == b.PackedValue;

        public bool Equals(NormalizedShort4 other) => this == other;
        public override bool Equals(object obj) => obj is NormalizedShort4 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(NormalizedShort4) + $"({X}, {Y}, {Z}, {W})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
