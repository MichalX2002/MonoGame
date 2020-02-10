// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing 16-bit floating-point XYZW components.
    /// <para>Ranges from [-1, -1, -1, 0] to [1, 1, 1, 1] in vector form.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HalfVector4 : IPackedVector<ulong>, IEquatable<HalfVector4>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, Unsafe.SizeOf<HalfSingle>() * 8),
            new VectorComponent(VectorComponentType.Green, Unsafe.SizeOf<HalfSingle>() * 8),
            new VectorComponent(VectorComponentType.Blue, Unsafe.SizeOf<HalfSingle>() * 8),
            new VectorComponent(VectorComponentType.Alpha, Unsafe.SizeOf<HalfSingle>() * 8));

        public HalfSingle X;
        public HalfSingle Y;
        public HalfSingle Z;
        public HalfSingle W;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public HalfVector4(HalfSingle x, HalfSingle y, HalfSingle z, HalfSingle w)
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
        public HalfVector4(ulong packed) : this() => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public HalfVector4(Vector4 vector) => this = Pack(vector);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public HalfVector4(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        private static HalfVector4 Pack(in Vector4 vector) => new HalfVector4(
            new HalfSingle(vector.X),
            new HalfSingle(vector.Y),
            new HalfSingle(vector.Z),
            new HalfSingle(vector.W));

        #region IPackedVector

        [CLSCompliant(false)]
        public ulong PackedValue
        {
            readonly get => UnsafeUtils.As<HalfVector4, ulong>(this);
            set => Unsafe.As<HalfVector4, ulong>(ref this) = value;
        }

        public void FromVector4(Vector4 vector) => this = Pack(vector);

        public readonly Vector4 ToVector4() => new Vector4(X, Y, Z, W);
        
        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector)
        {
            vector *= 2;
            vector -= Vector4.One;
            FromVector4(vector);
        }

        public readonly Vector4 ToScaledVector4()
        {
            var scaled = ToVector4();
            scaled += Vector4.One;
            scaled /= 2;
            return scaled;
        }

        public readonly void ToColor(ref Color destination) => destination.FromScaledVector4(ToScaledVector4());

        public void FromGray8(Gray8 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromGray16(Gray16 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromGrayAlpha16(GrayAlpha16 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgb24(Rgb24 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromColor(Color source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgb48(Rgb48 source) => FromScaledVector4(source.ToScaledVector4());

        public void FromRgba64(Rgba64 source) => FromScaledVector4(source.ToScaledVector4());

        #endregion

        #region Equals

        /// <summary>
        /// Compares the current instance to another to determine whether they are the same.
        /// </summary>
        public static bool operator ==(in HalfVector4 a, in HalfVector4 b) =>
            a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

        /// <summary>
        /// Compares the current instance to another to determine whether they are different.
        /// </summary>
        public static bool operator !=(in HalfVector4 a, in HalfVector4 b) => !(a == b);

        /// <summary>
        /// Returns a value that indicates whether the current instance is equal to a specified object.
        /// </summary>
        public override bool Equals(object obj) => obj is HalfVector4 other && Equals(other);

        /// <summary>
        /// Returns a value that indicates whether the current instance is equal to a specified object.
        /// </summary>
        public bool Equals(HalfVector4 other) => this == other;

        #endregion

        #region Object Overrides

        /// <summary>
        /// Returns a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override string ToString() => nameof(HalfVector4) + $"({ToVector4().ToString()})";

        /// <summary>
        /// Gets the hash code for the current instance.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}