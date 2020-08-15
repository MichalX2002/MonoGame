using System;
using System.Collections.Generic;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Represents a mesh that is part of a <see cref="Model"/>.
    /// </summary>
    public sealed class ModelMesh
    {
        private ModelBone _parentBone;

        /// <summary>
        /// Constructs the model mesh out of required components.
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="parentBone"></param>
        public ModelMesh(List<ModelMeshPart> parts, ModelBone parentBone)
        {
            // TODO: Complete member initialization
            _parentBone = parentBone ?? throw new ArgumentNullException(nameof(parentBone));

            MeshParts = new ModelMeshPartCollection(parts);

            for (int i = 0; i < MeshParts.Count; i++)
                MeshParts[i]._parent = this;

            Effects = new ModelEffectCollection();
        }

        /*internal void BuildEffectList()
        {
            List<Effect> effects = new List<Effect>();
            foreach (ModelMeshPart item in parts) 
            {
                if (!effects.Contains(item.Effect))
                {
                    if (item.Effect != null)
                        effects.Add(item.Effect);
                }
            }
            Effects = new ModelEffectCollection(effects);
        }*/

        /// <summary>
        /// Gets the BoundingSphere that contains this mesh.
        /// </summary>
        public BoundingSphere BoundingSphere { get; set; }

        /// <summary>
        /// Gets a collection of effects associated with this mesh.
        /// </summary>
        public ModelEffectCollection Effects { get; internal set; }

        /// <summary>
        /// Gets the <see cref="ModelMeshPart"/> objects that make up this mesh.
        /// Each part is composed of primitives that share the same material.
        /// </summary>
        public ModelMeshPartCollection MeshParts { get; }

        /// <summary>
        /// Gets the parent bone for this mesh. The parent bone contains a
        /// transformation matrix that describes the mesh location relative to
        /// any parent meshes in a model.
        /// </summary>
        public ModelBone ParentBone
        {
            get => _parentBone;
            set => _parentBone = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the name of this mesh.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets an object identifying this mesh.
        /// </summary>
        public object? Tag { get; set; }

        /// <summary>
        /// Draws all the <see cref="ModelMeshPart"/> object in this mesh, 
        /// using their current <see cref="Effect"/> settings.
        /// </summary>
        public void Draw(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));

            for (int i = 0; i < MeshParts.Count; i++)
            {
                var part = MeshParts[i];
                var effect = part.Effect;

                if (part.PrimitiveCount > 0)
                {
                    graphicsDevice.SetVertexBuffer(part.VertexBuffer);
                    graphicsDevice.Indices = part.IndexBuffer;

                    if (effect != null)
                    {
                        foreach (var pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();

                            graphicsDevice.DrawIndexedPrimitives(
                                PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                        }
                    }
                    else
                    {
                        graphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
        }
    }


    //// Summary:
    //// Represents a mesh that is part of a Model.
    //public sealed class ModelMesh
    //{
    //    // Summary:
    //    //     Gets the BoundingSphere that contains this mesh.
    //    public BoundingSphere BoundingSphere { get { throw new NotImplementedException(); } }
    //    //
    //    // Summary:
    //    //     Gets a collection of effects associated with this mesh.
    //    public ModelEffectCollection Effects { get { throw new NotImplementedException(); } }
    //    //
    //    // Summary:
    //    //     Gets the ModelMeshPart objects that make up this mesh. Each part of a mesh
    //    //     is composed of a set of primitives that share the same material.
    //    public ModelMeshPartCollection MeshParts { get { throw new NotImplementedException(); } }
    //    //
    //    // Summary:
    //    //     Gets the name of this mesh.
    //    public string Name { get { throw new NotImplementedException(); } }
    //    //
    //    // Summary:
    //    //     Gets the parent bone for this mesh. The parent bone of a mesh contains a
    //    //     transformation matrix that describes how the mesh is located relative to
    //    //     any parent meshes in a model.
    //    public ModelBone ParentBone { get { throw new NotImplementedException(); } }
    //    //
    //    // Summary:
    //    //     Gets or sets an object identifying this mesh.
    //    public object Tag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

    //    // Summary:
    //    //     Draws all of the ModelMeshPart objects in this mesh, using their current
    //    //     Effect settings.
    //    public void Draw() { throw new NotImplementedException(); }
    //}
}