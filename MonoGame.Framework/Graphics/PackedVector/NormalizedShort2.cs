// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.


using System;

namespace Microsoft.Xna.Framework.Graphics.PackedVector
{
	public struct NormalizedShort2 : IPackedVector<uint>, IEquatable<NormalizedShort2>
	{
        public NormalizedShort2(Vector2 vector)
		{
            PackedValue = PackInTwo(vector.X, vector.Y);
		}

        public NormalizedShort2(float x, float y)
		{
            PackedValue = PackInTwo(x, y);
		}

        public static bool operator !=(NormalizedShort2 a, NormalizedShort2 b)
		{
			return !a.Equals (b);
		}

        public static bool operator ==(NormalizedShort2 a, NormalizedShort2 b)
		{
			return a.Equals (b);
		}

        [CLSCompliant(false)]
        public uint PackedValue { get; set; }

        public override bool Equals (object obj)
		{
            return (obj is NormalizedShort2) && Equals((NormalizedShort2)obj);
		}

        public bool Equals(NormalizedShort2 other)
		{
            return PackedValue.Equals(other.PackedValue);
		}

		public override int GetHashCode ()
		{
			return PackedValue.GetHashCode();
		}

		public override string ToString ()
		{
            return PackedValue.ToString("X");
		}

		public Vector2 ToVector2 ()
		{
            const float maxVal = 0x7FFF;

            var v2 = new Vector2
            {
                X = ((short)(PackedValue & 0xFFFF)) / maxVal,
                Y = (short)(PackedValue >> 0x10) / maxVal
            };
            return v2;
		}

		private static uint PackInTwo (float vectorX, float vectorY)
		{
			const float maxPos = 0x7FFF;
            const float minNeg = -maxPos;

			// clamp the value between min and max values
            // Round rather than truncate.
            var word2 = (uint)((int)MathHelper.Clamp((float)Math.Round(vectorX * maxPos), minNeg, maxPos) & 0xFFFF);
            var word1 = (uint)(((int)MathHelper.Clamp((float)Math.Round(vectorY * maxPos), minNeg, maxPos) & 0xFFFF) << 0x10);

			return (word2 | word1);
		}

		void IPackedVector.PackFromVector4 (Vector4 vector)
		{
            PackedValue = PackInTwo(vector.X, vector.Y);
		}

		Vector4 IPackedVector.ToVector4 ()
		{
            const float maxVal = 0x7FFF;

            var v4 = new Vector4(0, 0, 0, 1)
            {
                X = ((short)((PackedValue >> 0x00) & 0xFFFF)) / maxVal,
                Y = ((short)((PackedValue >> 0x10) & 0xFFFF)) / maxVal
            };
            return v4;
		}
	}
}
