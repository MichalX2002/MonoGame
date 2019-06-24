// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Graphics
{
	public class DynamicIndexBuffer : IndexBuffer
	{
        /// <summary>
        /// Special offset used internally by GraphicsDevice.DrawUserXXX() methods.
        /// </summary>
        internal int UserOffset;

        public bool IsContentLost => false;

        public DynamicIndexBuffer(
            GraphicsDevice graphicsDevice, IndexElementSize elementSize, int indexCount, BufferUsage usage) :
			base(graphicsDevice, elementSize, indexCount, usage, dynamic: true)
		{
		}

   		public DynamicIndexBuffer(
            GraphicsDevice graphicsDevice, Type indexType, int indexCount, BufferUsage usage) :
            base(graphicsDevice, indexType, indexCount, usage, dynamic: true)
        {
        }
    }
}

