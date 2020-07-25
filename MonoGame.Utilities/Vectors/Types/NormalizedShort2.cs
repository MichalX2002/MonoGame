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
    /// Packed vector type containing signed 16-bit XY components.
    /// <para>
    /// Ranges from [-1, -1, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NormalizedShort2 : IPackedPixel<NormalizedShort2, uint>
    {
        internal static Vector2 Offset => new Vector2(-short.MinValue);

        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int16, VectorComponentChannel.Green));

        public short X;
        public short Y;

        #region Constructors

        public NormalizedShort2(short x, short y)
        {
            X = x;
            Y = y;
        }

        [CLSCompliant(false)]
        public NormalizedShort2(uint packed) : this()
        {
            PackedValue = packed;
        }

        #endregion

        public readonly Vector2 ToVector2()
        {
            var vector = new Vector2(X, Y);
            vector += Offset;
            vector *= 2f / ushort.MaxValue;
            vector -= Vector2.One;

            return vector;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<NormalizedShort2, uint>(this);
            set => Unsafe.As<NormalizedShort2, uint>(ref this) = value;
        }

        public readonly Vector4 ToScaledVector4()
        {
            var scaled = new Vector2(X, Y);
            scaled += Offset;
            scaled /= ushort.MaxValue;

            return new Vector4(scaled, 0, 1);
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            var raw = scaledVector.ToVector2();
            raw = VectorHelper.ZeroMax(raw, Vector2.One);
            raw *= ushort.MaxValue;
            raw -= Offset;

            X = (short)raw.X;
            Y = (short)raw.Y;
        }

        public readonly Vector4 ToVector4()
        {
            return new Vector4(ToVector2(), 0, 1);
        }

        public void FromVector(Vector4 vector)
        {
            var raw = vector.ToVector2();
            raw = Vector2.Clamp(raw, -Vector2.One, Vector2.One);
            raw *= ushort.MaxValue / 2f;
            raw -= new Vector2(0.5f);

            X = (short)raw.X;
            Y = (short)raw.Y;
        }

        #endregion

        #region Equals

        public override readonly bool Equals(object? obj)
        {
            return obj is NormalizedShort2 other && Equals(other);
        }

        public readonly bool Equals(NormalizedShort2 other)
        {
            return this == other;
        }

        public static bool operator ==(in NormalizedShort2 a, in NormalizedShort2 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in NormalizedShort2 a, in NormalizedShort2 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object overrides

        public override readonly string ToString() => nameof(NormalizedShort2) + $"({X}, {Y})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
