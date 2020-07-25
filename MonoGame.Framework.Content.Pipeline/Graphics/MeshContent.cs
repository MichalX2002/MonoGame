// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Numerics;

namespace MonoGame.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides properties and methods that define various aspects of a mesh.
    /// </summary>
    public class MeshContent : NodeContent
    {
        PositionCollection positions;

        /// <summary>
        /// Gets the list of geometry batches for the mesh.
        /// </summary>
        public GeometryContentCollection Geometry { get; }

        /// <summary>
        /// Gets the list of vertex position values.
        /// </summary>
        public PositionCollection Positions => positions;

        /// <summary>
        /// Initializes a new instance of MeshContent.
        /// </summary>
        public MeshContent()
        {
            Geometry = new GeometryContentCollection(this);
            positions = new PositionCollection();
        }

        /// <summary>
        /// Applies a transform directly to position and normal channels. Node transforms are unaffected.
        /// </summary>
        internal void TransformContents(in Matrix4x4 xform)
        {
            // Transform positions
            for (int i = 0; i < positions.Count; i++)
                positions[i] = Vector3.Transform(positions[i], xform);

            // Transform all vectors too:
            // Normals are "tangent covectors", which need to be transformed using the
            // transpose of the inverse matrix!
            if (!Matrix4x4.Invert(xform, out var ixform))
                throw new ArgumentException("Failed to invert matrix.", nameof(xform));

            var inverseTranspose = Matrix4x4.Transpose(ixform);
            foreach (var geom in Geometry)
            {
                foreach (var channel in geom.Vertices.Channels)
                {
                    if (!(channel is VertexChannel<Vector3> vector3Channel))
                        continue;

                    if (channel.Name.StartsWith("Normal", StringComparison.Ordinal) ||
                        channel.Name.StartsWith("Binormal", StringComparison.Ordinal) ||
                        channel.Name.StartsWith("Tangent", StringComparison.Ordinal))
                    {
                        for (int i = 0; i < vector3Channel.Count; i++)
                        {
                            var normal = Vector3.TransformNormal(vector3Channel[i], inverseTranspose);
                            normal = Vector3.Normalize(normal);
                            vector3Channel[i] = normal;
                        }
                    }
                }
            }

            // Swap winding order when faces are mirrored.
            if (MeshHelper.IsLeftHanded(xform))
                MeshHelper.SwapWindingOrder(this);
        }
    }
}
