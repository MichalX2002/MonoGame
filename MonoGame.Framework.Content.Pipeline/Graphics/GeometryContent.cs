// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides properties that define various aspects of a geometry batch.
    /// </summary>
    public class GeometryContent : ContentItem
    {
        /// <summary>
        /// Gets the list of triangle indices for this geometry batch. 
        /// <para>
        /// Geometry is stored as an indexed triangle list, where each group of three indices defines a single triangle.
        /// </para>
        /// </summary>
        public IndexCollection Indices { get; }

        /// <summary>
        /// Gets or sets the material of the parent mesh.
        /// </summary>
        public MaterialContent Material { get; set; }

        /// <summary>
        /// Gets or sets the parent mesh for this object.
        /// </summary>
        public MeshContent Parent { get; set; }

        /// <summary>
        /// Gets the set of vertex batches for the geometry batch.
        /// </summary>
        public VertexContent Vertices { get; }

        /// <summary>
        /// Creates an instance of <see cref="GeometryContent"/>.
        /// </summary>
        public GeometryContent()
        {
            Indices = new IndexCollection();
            Vertices = new VertexContent(this);
        }
    }
}
