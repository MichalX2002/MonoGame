// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct SpriteQuad
    {
        public readonly VertexPositionColorTexture VertexTL;
        public readonly VertexPositionColorTexture VertexTR;
		public readonly VertexPositionColorTexture VertexBL;
		public readonly VertexPositionColorTexture VertexBR;

        public SpriteQuad(
            VertexPositionColorTexture vertexTL, 
            VertexPositionColorTexture vertexTR,
            VertexPositionColorTexture vertexBL, 
            VertexPositionColorTexture vertexBR)
        {
            VertexTL = vertexTL;
            VertexTR = vertexTR;
            VertexBL = vertexBL;
            VertexBR = vertexBR;
        }

        public static SpriteQuad Create(
            Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br,
            Color color, Vector4 texCoord)
        {
            var tlv = new VertexPositionColorTexture(tl, color, texCoord.XY);
            var trv = new VertexPositionColorTexture(tr, color, texCoord.ZY);
            var blv = new VertexPositionColorTexture(bl, color, texCoord.XW);
            var brv = new VertexPositionColorTexture(br, color, texCoord.ZW);
            return new SpriteQuad(tlv, trv, blv, brv);
        }

        public static SpriteQuad Create(
            float x, float y, float dx, float dy,
            float w, float h, float sin, float cos,
            Color color, Vector4 texCoord, float depth)
        {
            // TODO: Should we be just assigning the Depth Value to Z?
            // We do according to 
            // http://blogs.msdn.com/b/shawnhar/archive/2011/01/12/spritebatch-billboards-in-a-3d-world.aspx

            var tl = new Vector3(
                x + dx * cos - dy * sin,
                y + dx * sin + dy * cos, 
                depth);
            
            var tr = new Vector3(
                x + (dx + w) * cos - dy * sin,
                y + (dx + w) * sin + dy * cos,
                depth);
            
            var bl = new Vector3(
                x + dx * cos - (dy + h) * sin,
                y + dx * sin + (dy + h) * cos,
                depth);
            
            var br = new Vector3(
                x + (dx + w) * cos - (dy + h) * sin,
                y + (dx + w) * sin + (dy + h) * cos,
                depth);

            return Create(tl, tr, bl, br, color, texCoord);
        }

        public static SpriteQuad Create(
            float x, float y, float w, float h,
            Color color, Vector4 texCoord, float depth)
        {
            var tl = new Vector3(x, y, depth);
            var tr = new Vector3(x + w, y, depth);
            var bl = new Vector3(x, y + h, depth);
            var br = new Vector3(x + w, y + h, depth);
            return Create(tl, tr, bl, br, color, texCoord);
        }
    }
}

