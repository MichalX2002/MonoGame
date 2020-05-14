// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing unsigned 4-bit XYZW components.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra4444 : IPackedPixel<Bgra4444, ushort>
    {
        private static readonly Vector4 _maxValue = new Vector4(15);

        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Blue, 4),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Green, 4),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Red, 4),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Alpha, 4));

        #region Constructors

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Bgra4444(ushort packed)
        {
            PackedValue = packed;
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        /// <param name="scaledVector"><see cref="Vector4"/> containing the components.</param>
        public Bgra4444(Vector4 scaledVector) : this()
        {
            // TODO: Unsafe.SkipInit(out this)
            FromScaledVector4(scaledVector);
        }

        /// <summary>
        /// Constructs the packed vector with vector form values.
        /// </summary>
        public Bgra4444(float r, float g, float b, float a) : this(new Vector4(r, g, b, a))
        {
        }

        #endregion

        #region IPackedVector

        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            scaledVector *= 15f;
            scaledVector += Vector4.Half;
            scaledVector.Clamp(Vector4.Zero, _maxValue);

            PackedValue = (ushort)(
                (((int)scaledVector.W & 0x0F) << 12) |
                (((int)scaledVector.X & 0x0F) << 8) |
                (((int)scaledVector.Y & 0x0F) << 4) |
                ((int)scaledVector.Z & 0x0F));
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(
                (PackedValue >> 8) & 0x0F,
                (PackedValue >> 4) & 0x0F,
                PackedValue & 0x0F,
                (PackedValue >> 12) & 0x0F) / 15f;
        }

        #endregion

        #region Equals

        public readonly bool Equals(Bgra4444 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Bgra4444 other && Equals(other);
        }

        public static bool operator ==(Bgra4444 a, Bgra4444 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(Bgra4444 a, Bgra4444 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a string representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Bgra4444) + $"({ToScaledVector4()})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
