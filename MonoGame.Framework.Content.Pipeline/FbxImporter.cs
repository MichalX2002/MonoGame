// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MonoGame.Framework.Content.Pipeline.Graphics;

namespace MonoGame.Framework.Content.Pipeline
{
    /// <summary>
    /// Provides methods for reading AutoDesk (.fbx) files for use in the Content Pipeline.
    /// </summary>
    [ContentImporter(".fbx", DisplayName = "Fbx Importer - MonoGame", DefaultProcessor = "ModelProcessor")]
    public class FbxImporter : ContentImporter<NodeContent>
    {
        public override NodeContent Import(string filename, ContentImporterContext context)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var importer = new OpenAssetImporter("FbxImporter", true);
            return importer.Import(filename, context);
        }
    }
}
