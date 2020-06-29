// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing two signed 16-bit integers.
    /// <para>
    /// Ranges from [-32768, -32768, 0, 1] to [32767, 32767, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Short2 : IPackedPixel<Short2, uint>
    {
        private static Vector2 Offset => new Vector2(32768);

        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Green));

        public short X;
        public short Y;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        [CLSCompliant(false)]
        public Short2(short x, short y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Short2(uint packed) : this()
        {
            PackedValue = packed;
        }

        #endregion

        public readonly Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        private static void Pack(Vector2 vector, out Short2 destination)
        {
            vector.Clamp(short.MinValue, short.MaxValue);
            vector.Round();

            destination.X = (short)vector.X;
            destination.Y = (short)vector.Y;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<Short2, uint>(this);
            set => Unsafe.As<Short2, uint>(ref this) = value;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            var vector = scaledVector.ToVector2();
            vector *= ushort.MaxValue;
            vector -= Offset;

            Pack(vector, out this);
        }

        public readonly Vector4 ToScaledVector4()
        {
            var scaledVector = ToVector2();
            scaledVector += Offset;
            scaledVector /= ushort.MaxValue;

            return new Vector4(scaledVector, 0, 1);
        }

        public void FromVector(Vector4 vector)
        {
            Pack(vector.ToVector2(), out this);
        }

        public readonly Vector4 ToVector4()
        {
            return new Vector4(ToVector2(), 0, 1);
        }

        #endregion

        #region Equals

        public readonly bool Equals(Short2 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Short2 other && Equals(other);
        }

        public static bool operator ==(in Short2 a, in Short2 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in Short2 a, in Short2 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object overrides

        public override readonly string ToString() => nameof(Short2) + $"({X}, {Y})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}