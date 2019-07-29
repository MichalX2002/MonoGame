// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Graphics;
namespace MonoGame.Framework.Content
{
    internal class VertexDeclarationReader : ContentTypeReader<VertexDeclaration>
	{
		protected internal override VertexDeclaration Read(ContentReader reader, VertexDeclaration existingInstance)
        {
			int vertexStride = reader.ReadInt32();
			int elementCount = reader.ReadInt32();			
            var elements = new VertexElement[elementCount];
			for (int i = 0; i < elementCount; ++i)
			{
				int offset = reader.ReadInt32();
				var elementFormat = (VertexElementFormat)reader.ReadInt32();
				var elementUsage = (VertexElementUsage)reader.ReadInt32();
				int usageIndex = reader.ReadInt32();
				elements[i] = new VertexElement(offset, elementFormat, elementUsage, usageIndex);
			}
            return VertexDeclaration.GetOrCreate(vertexStride, elements);
		}
	}
}
