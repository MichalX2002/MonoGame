// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.PackedVector
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
        private static readonly Vector2 Offset = new Vector2(32768);

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

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="vector"><see cref="Vector4"/> containing the components.</param>
        public Short2(Vector2 vector)
        {
            Pack(vector, out this);
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Short2(float x, float y) : this(new Vector2(x, y))
        {
        }

        #endregion

        public readonly Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        private static void Pack(in Vector2 vector, out Short2 destination)
        {
            var v = vector + Vector2.Half;
            v.Clamp(short.MinValue, short.MaxValue);

            destination.X = (short)v.X;
            destination.Y = (short)v.Y;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeUtils.As<Short2, uint>(this);
            set => Unsafe.As<Short2, uint>(ref this) = value;
        }

        public void FromScaledVector4(in Vector4 scaledVector)
        {
            var v = scaledVector.ToVector2() * ushort.MaxValue;
            v -= Offset;
            Pack(v, out this);
        }

        public readonly void ToScaledVector4(out Vector4 scaledVector)
        {
            scaledVector.Base.X = X + Offset.X;
            scaledVector.Base.Y = Y + Offset.Y;
            scaledVector.Base.Z = 0;
            scaledVector.Base.W = ushort.MaxValue;
            scaledVector /= ushort.MaxValue;
        }

        public void FromVector4(in Vector4 vector)
        {
            Pack(vector.ToVector2(), out this);
        }

        public readonly void ToVector4(out Vector4 vector)
        {
            vector.Base.X = X;
            vector.Base.Y = Y;
            vector.Base.Z = 0;
            vector.Base.W = 1;
        }

        #endregion

        #region Equals

        public static bool operator ==(in Short2 a, in Short2 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in Short2 a, in Short2 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        public bool Equals(Short2 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Short2 other && Equals(other);
        }

        #endregion

        #region Object Overrides

        public override string ToString() => nameof(Short2) + $"({X}, {Y})";

        public override int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}