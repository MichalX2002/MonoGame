// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using MonoGame.Framework.Utilities;

namespace MonoGame.Framework
{
    public static partial class TitleContainer
    {
        static partial void PlatformInit();

        static TitleContainer()
        {
            Location = string.Empty;
            PlatformInit();
        }

        static internal string Location { get; private set; }

        /// <summary>
        /// Returns a stream to an exsiting file in the title storage area.
        /// </summary>
        /// <param name="name">The filepath relative to the title storage area.</param>
        /// <returns>A open stream or null if the file is not found.</returns>
        public static Stream OpenStream(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            // We do not accept absolute paths here.
            if (Path.IsPathRooted(name))
                throw new ArgumentException($"A relative path is required.", nameof(name));

            // Normalize the file path.
            var safeName = NormalizeRelativePath(name);

            // Call the platform code to open the stream.  
            // Any errors at this point should result in a file not found.
            try
            {
                var stream = PlatformOpenStream(safeName);
                if (stream == null)
                    throw CreateFileNotFoundException(name, null);
                return stream;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException(name, ex);
            }
        }

        private static Exception CreateFileNotFoundException(string fileName, Exception inner)
        {
            return new FileNotFoundException(null, fileName, inner);
        }

        internal static string NormalizeRelativePath(string name)
        {
            var uri = new Uri("file:///" + name);
            var path = uri.LocalPath;
            path = path.Substring(1);
            return path.Replace(FileHelpers.NotSeparator, FileHelpers.Separator);
        }
    }
}

