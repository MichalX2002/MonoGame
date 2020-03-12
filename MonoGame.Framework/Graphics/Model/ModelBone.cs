using System.Collections.Generic;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Represents bone data for a model. 
    /// </summary>
    public sealed class ModelBone
    {
        private List<ModelBone> _children;

        /// <summary>
        /// Gets a collection of bones that are children of this bone.
        /// </summary>
        public ModelBoneCollection Children { get; }

        public List<ModelMesh> Meshes { get; }

        /// <summary>
        /// Gets the index of this bone in <see cref="Model.Bones"/>.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets the name of this bone.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the parent of this bone.
        /// </summary>
        public ModelBone Parent { get; set; }
        
        /// <summary>
        /// Gets or sets the matrix used to transform this bone relative to its parent bone.
        /// </summary>
        public Matrix Transform { get; set; }

        /// <summary>
        /// Transform of this node from the root of the model not from the parent.
        /// </summary>
        public Matrix ModelTransform { get; set; }

        public ModelBone()
        {
            _children = new List<ModelBone>();
            Meshes = new List<ModelMesh>();
            Children = new ModelBoneCollection(_children);
        }

        public void AddMesh(ModelMesh mesh) => Meshes.Add(mesh);

        public void AddChild(ModelBone modelBone) => _children.Add(modelBone);
    }

    //// Summary:
    ////     Represents bone data for a model. Reference page contains links to related
    ////     conceptual articles.
    //public sealed class ModelBone
    //{
    //    // Summary:
    //    //     Gets a collection of bones that are children of this bone.
    //    public ModelBoneCollection Children { get { throw new NotImplementedException(); } }
    //    //
    //    // Summary:
    //    //     Gets the index of this bone in the Bones collection.
    //    public int Index { get { throw new NotImplementedException(); } }
    //    //
    //    // Summary:
    //    //     Gets the name of this bone.
    //    public string Name { get { throw new NotImplementedException(); } }
    //    //
    //    // Summary:
    //    //     Gets the parent of this bone.
    //    public ModelBone Parent { get { throw new NotImplementedException(); } }
    //    //
    //    // Summary:
    //    //     Gets or sets the matrix used to transform this bone relative to its parent
    //    //     bone.
    //    public Matrix Transform { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
    //}
}
