﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Content.Pipeline
{
    /// <summary>
    /// Provides properties that define various aspects of content stored using the intermediate file format of the XNA Framework.
    /// </summary>
    public class ContentItem
    {

        /// <summary>
        /// Gets or sets the identity of the content item.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public ContentIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets the name of the content item.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets the opaque data of the content item.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public OpaqueDataDictionary OpaqueData { get; } = new OpaqueDataDictionary();

        /// <summary>
        /// Initializes a new instance of ContentItem.
        /// </summary>
        public ContentItem()
        {
        }
    }
}
