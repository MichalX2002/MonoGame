// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing signed 8-bit XYZW components.
    /// <para>
    /// Ranges from [-1, -1, -1, -1] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NormalizedByte4 : IPackedVector<uint>, IEquatable<NormalizedByte4>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(sbyte) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(sbyte) * 8),
            new VectorComponent(VectorComponentType.Blue, sizeof(sbyte) * 8),
            new VectorComponent(VectorComponentType.Alpha, sizeof(sbyte) * 8));

        [CLSCompliant(false)]
        public sbyte X;

        [CLSCompliant(false)]
        public sbyte Y;

        [CLSCompliant(false)]
        public sbyte Z;

        [CLSCompliant(false)]
        public sbyte W;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte4(sbyte x, sbyte y, sbyte z, sbyte w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte4(uint packed) : this() => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public NormalizedByte4(Vector4 vector) : this() => FromVector4(vector);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public NormalizedByte4(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeUtils.As<NormalizedByte4, uint>(this);
            set => Unsafe.As<NormalizedByte4, uint>(ref this) = value;
        }

        public void FromVector4(in Vector4 vector)
        {
            var v = Vector4.Clamp(vector, -1, 1);
            v *= sbyte.MaxValue;

            X = (sbyte)v.X;
            Y = (sbyte)v.Y;
            Z = (sbyte)v.Z;
            W = (sbyte)v.W;
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = X;
            vector.Base.Y = Y;
            vector.Base.Z = Z;
            vector.Base.W = W;
            vector /= sbyte.MaxValue;
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

        public void FromColor(Color source)
        {
            source.ToScaledVector4(out var vector);
            FromScaledVector4(vector);
        }

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

        public readonly void ToColor(ref Color destination)
        {
            ToScaledVector4(out var vector);
            destination.FromScaledVector4(vector);
        }

        #endregion

        #region Equals

        public static bool operator ==(in NormalizedByte4 a, in NormalizedByte4 b) =>
            a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

        public static bool operator !=(in NormalizedByte4 a, in NormalizedByte4 b) => !(a == b);

        public bool Equals(NormalizedByte4 other) => this == other;
        public override bool Equals(object obj) => obj is NormalizedByte4 other && Equals(other);

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(NormalizedByte4) + $"({X}, {Y}, {Z}, {W})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
