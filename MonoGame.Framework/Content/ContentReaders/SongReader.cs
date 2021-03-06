// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using MonoGame.Framework.Media;
using MonoGame.Framework.Utilities;

namespace MonoGame.Framework.Content
{
    internal class SongReader : ContentTypeReader<Song>
    {
        protected internal override Song Read(ContentReader input, Song existingInstance)
        {
            string path = input.ReadString();
            
            if (!string.IsNullOrEmpty(path))
            {
                // Add the ContentManager's RootDirectory
                var dirPath = Path.Combine(input.ContentManager.RootDirectoryFullPath, input.AssetName);

                // Resolve the relative path
                path = FileHelpers.ResolveRelativePath(dirPath, path);
            }

            var duration = input.ReadObject<TimeSpan>();

            return new Song(
                File.OpenRead(path), 
                leaveOpen: false,
                input.AssetName,
                duration);
        }
    }
}
