// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
{
    /// <summary>
    /// Packed vector type containing unsigned 8-bit XYZW integer components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [255, 255, 255, 255] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Byte4 : IPackedVector<uint>, IEquatable<Byte4>, IPixel
    {
        VectorComponentInfo IPackedVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Red, sizeof(byte) * 8),
            new VectorComponent(VectorComponentType.Green, sizeof(byte) * 8),
            new VectorComponent(VectorComponentType.Blue, sizeof(byte) * 8),
            new VectorComponent(VectorComponentType.Alpha, sizeof(byte) * 8));

        public byte X;
        public byte Y;
        public byte Z;
        public byte W;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Byte4(uint packed) : this() => PackedValue = packed;

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public Byte4(byte x, byte y, byte z, byte w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Byte4(Vector4 vector) => this = Pack(ref vector);

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Byte4(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        private static Byte4 Pack(ref Vector4 vector)
        {
            vector = Vector4.Clamp(vector, Vector4.Zero, Vector4.ByteMaxValue);
            
            return new Byte4(
                (byte)vector.X,
                (byte)vector.Y,
                (byte)vector.Z,
                (byte)vector.W);
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue 
        {
            readonly get => UnsafeUtils.As<Byte4, uint>(this);
            set => Unsafe.As<Byte4, uint>(ref this) = value;
        }

        public void FromVector4(Vector4 vector) => this = Pack(ref vector);

        public readonly Vector4 ToVector4() => new Vector4(X, Y, Z, W);

        #endregion

        #region IPixel

        public void FromScaledVector4(Vector4 vector) => FromVector4(vector * byte.MaxValue);

        public readonly Vector4 ToScaledVector4() => ToVector4() / byte.MaxValue;

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

        public static bool operator !=(in Byte4 a, in Byte4 b) => !(a == b);
        public static bool operator ==(in Byte4 a, in Byte4 b) =>
            a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

        public bool Equals(Byte4 other) => this == other;
        public override bool Equals(object obj) => obj is Byte4 other && Equals(other);

        #endregion

        /// <summary>
        /// Gets the hexadecimal <see cref="string"/> representation of this <see cref="Color"/>.
        /// </summary>
        public string ToHex()
        {
            uint hexOrder = (uint)(W << 0 | Z << 8 | Y << 16 | X << 24);
            return hexOrder.ToString("x8");
        }

        #region Object Overrides

        /// <summary>
        /// Returns a <see cref="string"/> representation of this packed vector.
        /// </summary>
        public override string ToString() => nameof(Byte4) + $"({X}, {Y}, {Z}, {W})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}

