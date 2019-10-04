// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoGame.Framework;

namespace MonoGame.Utilities.PackedVector
{
    /// <summary>
    /// Packed vector type containing unsigned 8-bit XYZW components.
    /// <para>Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Byte4 : IPackedVector<uint>, IEquatable<Byte4>, IPixel
    {
        public byte X;
        public byte Y;
        public byte Z;
        public byte W;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Byte4(uint value) : this() => PackedValue = value;

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
        /// Constructs the packed vector with raw values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Byte4(Vector4 vector) : this() => PackedValue = Pack(ref vector);

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public Byte4(float x, float y, float z, float w) : this(new Vector4(x, y, z, w))
        {
        }

        #endregion

        private static uint Pack(ref Vector4 vector)
        {
            vector *= PackedVectorHelper.MaxBytes;
            vector = Vector4.Clamp(vector, Vector4.Zero, PackedVectorHelper.MaxBytes);
            
            uint byte4 = (uint)Math.Round(vector.X) & 0xFF;
            uint byte3 = ((uint)Math.Round(vector.Y) & 0xFF) << 0x8;
            uint byte2 = ((uint)Math.Round(vector.Z) & 0xFF) << 0x10;
            uint byte1 = ((uint)Math.Round(vector.W) & 0xFF) << 0x18;
            return byte4 | byte3 | byte2 | byte1;
        }

        #region IPixel



        #endregion

        #region IPackedVector

        /// <inheritdoc />
        [CLSCompliant(false)]
        public uint PackedValue 
        {
            get => Unsafe.As<Byte4, uint>(ref this);
            set => Unsafe.As<Byte4, uint>(ref this) = value;
        }

        /// <inheritdoc />
        public void FromVector4(Vector4 vector) => PackedValue = Pack(ref vector);

        /// <inheritdoc />
        public Vector4 ToVector4()
        {
            return new Vector4(
                PackedValue & 0xFF,
                (PackedValue >> 0x8) & 0xFF,
                (PackedValue >> 0x10) & 0xFF,
                (PackedValue >> 0x18) & 0xFF);
        }

        #endregion

        #region Equals

        public static bool operator ==(Byte4 a, Byte4 b) => a.PackedValue == b.PackedValue;
        public static bool operator !=(Byte4 a, Byte4 b) => a.PackedValue != b.PackedValue;

        public bool Equals(Byte4 other) => this == other;
        public override bool Equals(object obj) => obj is Byte4 value && Equals(value);

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets the hash code for the current instance.
        /// </summary>
        public override int GetHashCode() => PackedValue.GetHashCode();

        /// <summary>
        /// Returns a string representation of the current instance.
        /// </summary>
        public override string ToString() => PackedValue.ToString("x8");

        #endregion
    }
}

