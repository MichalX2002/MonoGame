// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Utilities;
using System;

namespace Microsoft.Xna.Framework.Graphics
{
	public class DynamicVertexBuffer : VertexBuffer
    {
        /// <summary>
        /// Special offset used internally by GraphicsDevice.DrawUserXXX() methods.
        /// </summary>
        internal int UserOffset;

        public bool IsContentLost => false;

        public DynamicVertexBuffer(
            GraphicsDevice graphicsDevice, VertexDeclaration declaration, int vertexCount, BufferUsage bufferUsage)
            : base(graphicsDevice, declaration, vertexCount, bufferUsage, true)
        {
        }
		
		public DynamicVertexBuffer(
            GraphicsDevice graphicsDevice, Type type, int vertexCount, BufferUsage bufferUsage)
            : base(graphicsDevice, VertexDeclaration.FromType(type), vertexCount, bufferUsage, true)
        {
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount, SetDataOptions options) 
            where T : struct
        {
            var elementSizeInBytes = ReflectionHelpers.SizeOf<T>.Get();
            base.SetData<T>(0, data, startIndex, elementCount, elementSizeInBytes, options);
        }
    }
}

