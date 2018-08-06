// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SpriteBatchItem
    {
        public float SortKey;

        public VertexPositionColorTexture VertexTL;
        public VertexPositionColorTexture VertexTR;
		public VertexPositionColorTexture VertexBL;
		public VertexPositionColorTexture VertexBR;

        public void Set(
            in float x, in float y, in float dx, in float dy,
            in float w, in float h, in float sin, in float cos,
            in Color color, in Vector2 texCoordTL, in Vector2 texCoordBR, in float depth)
        {
            // TODO, Should we be just assigning the Depth Value to Z?
            // According to http://blogs.msdn.com/b/shawnhar/archive/2011/01/12/spritebatch-billboards-in-a-3d-world.aspx
            // We do.

            VertexTL.Position.X = x + dx * cos - dy * sin;
            VertexTL.Position.Y = y + dx * sin + dy * cos;
            VertexTL.Position.Z = depth;
            VertexTL.Color = color;
            VertexTL.TextureCoordinate = texCoordTL;

            VertexTR.Position.X = x + (dx + w) * cos - dy * sin;
            VertexTR.Position.Y = y + (dx + w) * sin + dy * cos;
            VertexTR.Position.Z = depth;
            VertexTR.Color = color;
            VertexTR.TextureCoordinate.X = texCoordBR.X;
            VertexTR.TextureCoordinate.Y = texCoordTL.Y;

            VertexBL.Position.X = x + dx * cos - (dy + h) * sin;
            VertexBL.Position.Y = y + dx * sin + (dy + h) * cos;
            VertexBL.Position.Z = depth;
            VertexBL.Color = color;
            VertexBL.TextureCoordinate.X = texCoordTL.X;
            VertexBL.TextureCoordinate.Y = texCoordBR.Y;

            VertexBR.Position.X = x + (dx + w) * cos - (dy + h) * sin;
            VertexBR.Position.Y = y + (dx + w) * sin + (dy + h) * cos;
            VertexBR.Position.Z = depth;
            VertexBR.Color = color;
            VertexBR.TextureCoordinate = texCoordBR;
        }

        public void Set(
            in float x, in float y, in float w, in float h,
            in Color color, in Vector2 texCoordTL, in Vector2 texCoordBR, in float depth)
        {
            VertexTL.Position.X = x;
            VertexTL.Position.Y = y;
            VertexTL.Position.Z = depth;
            VertexTL.Color = color;
            VertexTL.TextureCoordinate = texCoordTL;

            VertexTR.Position.X = x + w;
            VertexTR.Position.Y = y;
            VertexTR.Position.Z = depth;
            VertexTR.Color = color;
            VertexTR.TextureCoordinate.X = texCoordBR.X;
            VertexTR.TextureCoordinate.Y = texCoordTL.Y;

            VertexBL.Position.X = x;
            VertexBL.Position.Y = y + h;
            VertexBL.Position.Z = depth;
            VertexBL.Color = color;
            VertexBL.TextureCoordinate.X = texCoordTL.X;
            VertexBL.TextureCoordinate.Y = texCoordBR.Y;

            VertexBR.Position.X = x + w;
            VertexBR.Position.Y = y + h;
            VertexBR.Position.Z = depth;
            VertexBR.Color = color;
            VertexBR.TextureCoordinate = texCoordBR;
        }
        
        public int CompareTo(SpriteBatchItem other)
        {
            return SortKey.CompareTo(other.SortKey);
        }

        public int CompareToRef(in SpriteBatchItem other)
        {
            return SortKey.CompareTo(other.SortKey);
        }
    }
}

