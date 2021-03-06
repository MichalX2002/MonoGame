﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Numerics;

namespace MonoGame.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides a base class for graphics types that define local coordinate systems.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Node '{Name}'")]
    public class NodeContent : ContentItem
    {
        AnimationContentDictionary animations;

        /// <summary>
        /// Gets the value of the local Transform property, multiplied by the AbsoluteTransform of the parent.
        /// </summary>
        public Matrix4x4 AbsoluteTransform
        {
            get
            {
                if (Parent != null)
                    return Transform * Parent.AbsoluteTransform;
                return Transform;
            }
        }

        /// <summary>
        /// Gets the set of animations belonging to this node.
        /// </summary>
        public AnimationContentDictionary Animations => animations;

        /// <summary>
        /// Gets the children of the NodeContent object.
        /// </summary>
        public NodeContentCollection Children { get; }

        /// <summary>
        /// Gets the parent of this NodeContent object.
        /// </summary>
        public NodeContent Parent { get; set; }

        /// <summary>
        /// Gets the transform matrix of the scene.
        /// The transform matrix defines a local coordinate system for the content in addition to any children of this object.
        /// </summary>
        public Matrix4x4 Transform { get; set; } = Matrix4x4.Identity;

        /// <summary>
        /// Creates an instance of NodeContent.
        /// </summary>
        public NodeContent()
        {
            Children = new NodeContentCollection(this);
            animations = new AnimationContentDictionary();
        }
    }
}
