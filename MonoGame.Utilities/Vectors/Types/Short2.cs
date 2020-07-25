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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Pack(Vector2 vector, out Short2 destination)
        {
            vector = VectorHelper.Clamp(vector, short.MinValue, short.MaxValue);
            vector = VectorHelper.Round(vector);

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

        public void FromScaledVector(Vector3 scaledVector)
        {
            var vector = scaledVector.ToVector2();
            vector *= ushort.MaxValue;
            vector -= Offset;

            Pack(vector, out this);
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            FromScaledVector(scaledVector.ToVector3());
        }

        public readonly Vector3 ToScaledVector3()
        {
            var scaledVector = ToVector2();
            scaledVector += Offset;
            scaledVector /= ushort.MaxValue;

            return new Vector3(scaledVector, 0);
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(ToScaledVector3(), 1);
        }

        public void FromVector(Vector3 vector)
        {
            Pack(vector.ToVector2(), out this);
        }

        public void FromVector(Vector4 vector)
        {
            FromVector(vector.ToVector3());
        }

        public readonly Vector3 ToVector3()
        {
            return new Vector3(ToVector2(), 0);
        }

        public readonly Vector4 ToVector4()
        {
            return new Vector4(ToVector3(), 1);
        }

        #endregion

        #region IPixel

        public void FromAlpha(Alpha8 source)
        {
            X = Y = short.MaxValue;
        }

        public void FromAlpha(Alpha16 source)
        {
            X = Y = short.MaxValue;
        }

        public void FromAlpha(AlphaF source)
        {
            X = Y = short.MaxValue;
        }

        public Alpha8 ToAlpha8()
        {
            return Alpha8.Opaque;
        }

        public Alpha16 ToAlpha16()
        {
            return Alpha16.Opaque;
        }

        public AlphaF ToAlphaF()
        {
            return AlphaF.Opaque;
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