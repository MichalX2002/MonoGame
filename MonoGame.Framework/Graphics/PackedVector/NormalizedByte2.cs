// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.


using System;

namespace Microsoft.Xna.Framework.Graphics.PackedVector
{
    public struct NormalizedByte2 : IPackedVector<ushort>, IEquatable<NormalizedByte2>
    {
        public NormalizedByte2(Vector2 vector)
        {
            PackedValue = Pack(vector.X, vector.Y);
        }

        public NormalizedByte2(float x, float y)
        {
            PackedValue = Pack(x, y);
        }

        public static bool operator !=(NormalizedByte2 a, NormalizedByte2 b)
        {
            return a.PackedValue != b.PackedValue;
        }

        public static bool operator ==(NormalizedByte2 a, NormalizedByte2 b)
        {
            return a.PackedValue == b.PackedValue;
        }

        [CLSCompliant(false)]
        public ushort PackedValue { get; set; }

        public override bool Equals(object obj)
        {
            return (obj is NormalizedByte2) &&
                    ((NormalizedByte2)obj).PackedValue == PackedValue;
        }

        public bool Equals(NormalizedByte2 other)
        {
            return PackedValue == other.PackedValue;
        }

        public override int GetHashCode()
        {
            return PackedValue.GetHashCode();
        }

        public override string ToString()
        {
            return PackedValue.ToString("X");
        }

        private static ushort Pack(float x, float y)
        {
            var byte2 = (((ushort) Math.Round(MathHelper.Clamp(x, -1.0f, 1.0f) * 127.0f)) & 0xFF) << 0;
            var byte1 = (((ushort) Math.Round(MathHelper.Clamp(y, -1.0f, 1.0f) * 127.0f)) & 0xFF) << 8;

            return (ushort)(byte2 | byte1);
        }

        void IPackedVector.PackFromVector4(Vector4 vector)
        {
            PackedValue = Pack(vector.X, vector.Y);
        }

        Vector4 IPackedVector.ToVector4()
        {
            return new Vector4(ToVector2(), 0.0f, 1.0f);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(
                ((sbyte) ((PackedValue >> 0) & 0xFF)) / 127.0f,
                ((sbyte) ((PackedValue >> 8) & 0xFF)) / 127.0f);
        }
    }
}
