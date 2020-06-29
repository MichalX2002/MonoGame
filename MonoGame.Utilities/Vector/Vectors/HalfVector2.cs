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
    /// Packed vector type containing 16-bit floating-point XY components.
    /// <para>
    /// Ranges from [-1, -1, 0, 1] to [1, 1, 0, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HalfVector2 : IPackedPixel<HalfVector2, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.Float16, VectorComponentChannel.Red),
            new VectorComponent(VectorComponentType.Float16, VectorComponentChannel.Green));

        public HalfSingle X;
        public HalfSingle Y;

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with raw values.
        /// </summary>
        public HalfVector2(HalfSingle x, HalfSingle y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public HalfVector2(uint packed) : this()
        {
            // TODO: Unsafe.SkipInit(out this)
            PackedValue = packed;
        }

        #endregion

        public readonly Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue
        {
            readonly get => UnsafeR.As<HalfVector2, uint>(this);
            set => Unsafe.As<HalfVector2, uint>(ref this) = value;
        }

        public void FromScaledVector(Vector4 scaledVector)
        {
            var vector = scaledVector.ToVector2();
            vector *= 2;
            vector -= Vector2.One;

            X = new HalfSingle(vector.X);
            Y = new HalfSingle(vector.Y);
        }

        public readonly Vector4 ToScaledVector4()
        {
            var vector = ToVector2();
            vector += Vector2.One;
            vector /= 2f;
            return new Vector4(vector, 0, 1);
        }

        public void FromVector(Vector4 vector)
        {
            X = new HalfSingle(vector.X);
            Y = new HalfSingle(vector.Y);
        }

        public readonly Vector4 ToVector4()
        {
            return new Vector4(X, Y, 0, 1);
        }

        #endregion

        #region Equals

        public readonly bool Equals(HalfVector2 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is HalfVector2 other && Equals(other);
        }

        public static bool operator ==(in HalfVector2 a, in HalfVector2 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in HalfVector2 a, in HalfVector2 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object overrides

        public override readonly string ToString() => nameof(HalfVector2) + $"({ToVector2()})";

        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
