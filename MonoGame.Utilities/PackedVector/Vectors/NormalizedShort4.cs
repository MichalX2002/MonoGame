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
    public struct NormalizedShort4 : IPackedPixel<NormalizedShort4, ulong>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Green),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Blue),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Alpha));

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
        public NormalizedShort4(ulong packed) : this()
        {
            PackedValue = packed;
        }

        public NormalizedShort4(in Vector4 vector) : this()
        {
            FromVector4(vector);
        }

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
            var v = Vector4.Clamp(vector, Vector4.NegativeOne, Vector4.One);
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
            var v = scaledVector * 2;
            v -= Vector4.One;
            FromVector4(v);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            ToVector4(out scaledVector);
            scaledVector += Vector4.One;
            scaledVector /= 2;
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

        public readonly void ToColor(out Color destination)
        {
            destination = default; // TODO: Unsafe.SkipInit

            ToScaledVector4(out var vector);
            destination.FromScaledVector4(vector);
        }

        #endregion

        #region Equals

        public static bool operator ==(NormalizedShort4 a, NormalizedShort4 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(NormalizedShort4 a, NormalizedShort4 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        public bool Equals(NormalizedShort4 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is NormalizedShort4 other && Equals(other);
        }

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(NormalizedShort4) + $"({X}, {Y}, {Z}, {W})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
