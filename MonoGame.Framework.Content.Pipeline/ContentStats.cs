// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Content.Pipeline
{
    /// <summary>
    /// Content building statistics for a single source content file.
    /// </summary>
    public readonly struct ContentStats
    {
        /// <summary>
        /// The absolute path to the source content file.
        /// </summary>
        public string SourceFile { get; }

        /// <summary>
        /// The absolute path to the destination content file.
        /// </summary>
        public string DestFile { get; }

        /// <summary>
        /// The content processor type name.
        /// </summary>
        public string ProcessorType { get; }

        /// <summary>
        /// The content type name.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// The source file size in bytes.
        /// </summary>
        public long SourceFileSize { get; }

        /// <summary>
        /// The destination file size in bytes.
        /// </summary>
        public long DestFileSize { get; }

        /// <summary>
        /// The content build time in seconds.
        /// </summary>
        public float BuildSeconds { get; }

        public ContentStats(
            string sourceFile, 
            string destFile, 
            string processorType, 
            string contentType, 
            long sourceFileSize,
            long destFileSize,
            float buildSeconds)
        {
            SourceFile = sourceFile ?? throw new ArgumentNullException(nameof(sourceFile));
            DestFile = destFile ?? throw new ArgumentNullException(nameof(destFile));
            ProcessorType = processorType ?? throw new ArgumentNullException(nameof(processorType));
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            SourceFileSize = sourceFileSize;
            DestFileSize = destFileSize;
            BuildSeconds = buildSeconds;
        }
    }
}
