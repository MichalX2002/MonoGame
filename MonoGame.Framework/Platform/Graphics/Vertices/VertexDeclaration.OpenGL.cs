// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using MonoGame.OpenGL;

namespace MonoGame.Framework.Graphics
{
    public partial class VertexDeclaration
    {
        private readonly Dictionary<int, AttributeInfo> _shaderAttributeInfo = 
            new Dictionary<int, AttributeInfo>();

        internal AttributeInfo GetAttributeInfo(Shader shader, int programHash)
        {
            if (_shaderAttributeInfo.TryGetValue(programHash, out AttributeInfo attrInfo))
                return attrInfo;

            // Get the vertex attribute info and cache it
            attrInfo = new AttributeInfo(GraphicsDevice.MaxVertexAttributes);

            var elementList = new List<AttributeInfo.Element>();

            foreach (var ve in VertexElements.Span)
            {
                int attributeLocation = shader.GetAttribLocation(ve.VertexElementUsage, ve.UsageIndex);
                // XNA appears to ignore usages it can't find a match for, so we will do the same
                if (attributeLocation < 0)
                    continue;

                elementList.Add(new AttributeInfo.Element(
                    offset: ve.Offset,
                    attributeLocation: attributeLocation,
                    numberOfElements: ve.VertexElementFormat.OpenGLNumberOfElements(),
                    vertexAttribPointerType: ve.VertexElementFormat.OpenGLVertexAttribPointerType(),
                    normalized: ve.OpenGLVertexAttribNormalized()));

                attrInfo.EnabledAttributes[attributeLocation] = true;
            }

            attrInfo.Elements = elementList.ToArray();

            _shaderAttributeInfo.Add(programHash, attrInfo);
            return attrInfo;
        }

        internal void Apply(Shader shader, IntPtr offset, int programHash)
        {
            var attrInfo = GetAttributeInfo(shader, programHash);
            
            // Apply the vertex attribute info
            foreach(var element in attrInfo.Elements.Span)
            { 
                GL.VertexAttribPointer(
                    element.AttributeLocation,
                    element.NumberOfElements,
                    element.VertexAttribPointerType,
                    element.Normalized,
                    VertexStride,
                    offset + element.Offset);
                GraphicsExtensions.CheckGLError();

#if !(GLES || MONOMAC)
                if (GraphicsDevice.Capabilities.SupportsInstancing)
                {
                    GL.VertexAttribDivisor(element.AttributeLocation, 0);
                    GraphicsExtensions.CheckGLError();
                }
#endif
            }

            GraphicsDevice.SetVertexAttributeArray(attrInfo.EnabledAttributes);
            GraphicsDevice._attribsDirty = true;
        }

        /// <summary>
        /// Vertex attribute information for a particular shader/vertex declaration combination.
        /// </summary>
        internal class AttributeInfo
        {
            internal bool[] EnabledAttributes;
            internal ReadOnlyMemory<Element> Elements;

            internal AttributeInfo(int maxVertexAttributes)
            {
                EnabledAttributes = new bool[maxVertexAttributes];
            }

            public readonly struct Element
            {
                public int Offset { get; }
                public int AttributeLocation { get; }
                public int NumberOfElements { get; }
                public VertexAttribPointerType VertexAttribPointerType { get; }
                public bool Normalized { get; }

                public Element(
                    int offset, 
                    int attributeLocation, 
                    int numberOfElements, 
                    VertexAttribPointerType vertexAttribPointerType,
                    bool normalized)
                {
                    Offset = offset;
                    AttributeLocation = attributeLocation;
                    NumberOfElements = numberOfElements;
                    VertexAttribPointerType = vertexAttribPointerType;
                    Normalized = normalized;
                }
            }
        }
    }
}
