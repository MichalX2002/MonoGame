using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MonoGame.Content.Builder.Tasks
{
    public class CollectContentReferences : Task
    {
        [Required]
        public ITaskItem[] ContentReferences { get; set; }

        [Required]
        public string MonoGamePlatform { get; set; }

        [Output]
        public ITaskItem[] Output { get; set; }

        public override bool Execute()
        {
            var output = new List<ITaskItem>();
            foreach (var content in ContentReferences)
            {
                var relativeDir = content.GetMetadata("RelativeDir").NormalizePath();
                var fullPath = content.GetMetadata("FullPath").NormalizePath();
                var link = content.GetMetadata("Link").NormalizePath();
                
                var contentFolder = string.Empty;
                if (!string.IsNullOrEmpty(link))
                    contentFolder = Path.GetFileName(Path.GetDirectoryName(link));
                else if (!string.IsNullOrEmpty(relativeDir))
                    contentFolder = Path.GetFileName(Path.GetDirectoryName(relativeDir));
                
                var outputPath = Path.GetFileNameWithoutExtension(fullPath);
                if (!outputPath.EndsWith(contentFolder, StringComparison.OrdinalIgnoreCase))
                    outputPath = Path.Combine(outputPath, contentFolder);

                foreach (var met in content.MetadataNames)
                    Log.LogError(met + ": " + content.GetMetadata(met.ToString()));

                Log.LogError("PROPS:");

                foreach (var p in BuildEngine6.GetGlobalProperties())
                    Log.LogError(p.Key + ": " + p.Value);

                var metaData = new Dictionary<string, string>();
                metaData.Add("ContentDirectory", !string.IsNullOrEmpty(contentFolder) ? contentFolder + Path.DirectorySeparatorChar : "");
                metaData.Add("RelativeFullPath", !string.IsNullOrEmpty(relativeDir) ? Path.GetFullPath(relativeDir) : "");
                metaData.Add("ContentOutputDir", Path.Combine("bin", MonoGamePlatform, outputPath));
                metaData.Add("ContentIntermediateOutputDir", Path.Combine("obj", MonoGamePlatform, outputPath));
                output.Add(new TaskItem(fullPath, metaData));
            }
            Output = output.ToArray();
            return !Log.HasLoggedErrors;
        }
    }
}
