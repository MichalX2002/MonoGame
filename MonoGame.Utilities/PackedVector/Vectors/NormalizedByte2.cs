// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing signed 8-bit XY components.
    /// <para>
    /// Ranges from [-1, -1, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NormalizedByte2 : IPackedPixel<NormalizedByte2, ushort>
    {
        internal static Vector2 Offset => new Vector2(-sbyte.MinValue);

        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Int8, VectorComponentChannel.Green));

        [CLSCompliant(false)]
        public sbyte X;

        [CLSCompliant(false)]
        public sbyte Y;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte2(sbyte x, sbyte y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public NormalizedByte2(ushort packed) : this()
        {
            // TODO: Unsafe.SkipInit(out this)
            PackedValue = packed;
        }

        #endregion

        public readonly Vector2 ToVector2()
        {
            var vector = new Vector2(X, Y);
            vector += Offset;
            vector *= 2f / byte.MaxValue;
            vector -= Vector2.One;

            return vector;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue
        {
            readonly get => UnsafeR.As<NormalizedByte2, ushort>(this);
            set => Unsafe.As<NormalizedByte2, ushort>(ref this) = value;
        }

        public readonly Vector4 ToScaledVector4()
        {
            var scaled = new Vector2(X, Y);
            scaled += Offset;
            scaled /= byte.MaxValue;

            return new Vector4(scaled, 0, 1);
        }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            var raw = scaledVector.ToVector2();
            raw.Clamp(Vector2.Zero, Vector2.One);
            raw *= byte.MaxValue;
            raw -= Offset;

            X = (sbyte)raw.X;
            Y = (sbyte)raw.Y;
        }

        public readonly Vector4 ToVector4()
        {
            return new Vector4(ToVector2(), 0, 1);
        }

        public void FromVector4(Vector4 vector)
        {
            var raw = vector.ToVector2();
            raw.Clamp(Vector2.NegativeOne, Vector2.One);
            raw *= byte.MaxValue / 2f;
            raw -= Vector2.Half;
            
            X = (sbyte)raw.X;
            Y = (sbyte)raw.Y;
        }

        #endregion

        #region Equals

        public override readonly bool Equals(object obj)
        {
            return obj is NormalizedByte2 other && Equals(other);
        }

        public readonly bool Equals(NormalizedByte2 other)
        {
            return this == other;
        }

        public static bool operator ==(in NormalizedByte2 a, in NormalizedByte2 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in NormalizedByte2 a, in NormalizedByte2 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object Overrides

        public override readonly string ToString() => nameof(NormalizedByte2) + $"({X}, {Y})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
