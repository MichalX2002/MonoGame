// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Vector
{
    /// <summary>
    /// Packed vector type containing unsigned XYZW components.
    /// The XYZ components use 10 bits each, and the W component uses 2 bits.
    /// <para>
    /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rgba1010102 : IPackedPixel<Rgba1010102, uint>
    {
        VectorComponentInfo IVector.ComponentInfo => new VectorComponentInfo(
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Red, 10),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Green, 10),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Blue, 10),
            new VectorComponent(VectorComponentType.BitField, VectorComponentChannel.Alpha, 2));

        /// <summary>
        /// Constructs the packed vector with a packed value.
        /// </summary>
        [CLSCompliant(false)]
        public Rgba1010102(uint packed)
        {
            PackedValue = packed;
        }

        #region IPackedVector

        [CLSCompliant(false)]
        public uint PackedValue { get; set; }

        public void FromScaledVector4(Vector4 scaledVector)
        {
            scaledVector.Clamp(Vector4.Zero, Vector4.One);
            scaledVector *= 1023f;
            scaledVector.W *= 3f / 1023f;
            scaledVector += Vector4.Half;

            PackedValue = (uint)(
                (((int)scaledVector.X & 0x03FF) << 0) |
                (((int)scaledVector.Y & 0x03FF) << 10) |
                (((int)scaledVector.Z & 0x03FF) << 20) |
                (((int)scaledVector.W) & 0x03) << 30);
        }

        public readonly Vector4 ToScaledVector4()
        {
            return new Vector4(
                (PackedValue >> 0) & 0x03FF,
                (PackedValue >> 10) & 0x03FF,
                (PackedValue >> 20) & 0x03FF,
                ((PackedValue >> 30) & 0x03) * 1023f / 3f) / 1023f;
        }

        #endregion

        #region Equals

        public readonly bool Equals(Rgba1010102 other)
        {
            return this == other;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Rgba1010102 other && Equals(other);
        }

        public static bool operator ==(in Rgba1010102 a, in Rgba1010102 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        public static bool operator !=(in Rgba1010102 a, in Rgba1010102 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Gets a <see cref="string"/> representation of the packed vector.
        /// </summary>
        public override readonly string ToString() => nameof(Rgba1010102) + $"({this.ToVector4()})";

        /// <summary>
        /// Gets a hash code of the packed vector.
        /// </summary>
        public override readonly int GetHashCode() => PackedValue.GetHashCode();

        #endregion
    }
}
