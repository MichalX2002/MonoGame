using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MonoGame.Framework.Content.Pipeline;
using MonoGame.Framework.Graphics;

namespace MonoGame.Content.Builder
{
    [XmlRoot(ElementName = "SourceFileCollection")]
    public sealed class SourceFileCollection
    {
        public GraphicsProfile Profile { get; set; }

        public TargetPlatform Platform { get; set; }

        public string Config { get; set; }

        [XmlArrayItem("File")]
        public List<string> SourceFiles { get; }

        [XmlArrayItem("File")]
        public List<string> DestFiles { get; }

        public SourceFileCollection()
        {
            SourceFiles = new List<string>();
            DestFiles = new List<string>();
            Config = string.Empty;
        }

        public static SourceFileCollection Read(string filePath)
        {
            var deserializer = new XmlSerializer(typeof(SourceFileCollection));
            try
            {
                using (var textReader = new XmlTextReader(filePath))
                    return (SourceFileCollection)deserializer.Deserialize(textReader);
            }
            catch (Exception)
            {
            }

            return new SourceFileCollection();
        }

        public void Write(string filePath)
        {
            var serializer = new XmlSerializer(typeof(SourceFileCollection));
            using (var textWriter = new StreamWriter(filePath, false, new UTF8Encoding(false)))
                serializer.Serialize(textWriter, this);
        }

        public void Merge(SourceFileCollection other)
        {
            foreach (var sourceFile in other.SourceFiles)
            {
                bool inContent = SourceFiles.Any(
                    e => string.Equals(e, sourceFile, StringComparison.InvariantCultureIgnoreCase));
               
                if (!inContent)
                    SourceFiles.Add(sourceFile);
            }

            foreach (var destFile in other.DestFiles)
            {
                var inContent = DestFiles.Any(e => string.Equals(e, destFile, StringComparison.InvariantCultureIgnoreCase));
                if (!inContent)
                    DestFiles.Add(destFile);
            }
        }
    }
}