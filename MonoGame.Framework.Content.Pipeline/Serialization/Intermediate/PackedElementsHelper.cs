using System;
using System.Collections.Generic;
using System.Xml;

namespace MonoGame.Framework.Content.Pipeline.Serialization.Intermediate
{
    internal static class PackedElementsHelper
    {
        private static readonly char[] _separators = { ' ', '\t', '\n' };

        private const string _writeSeperator = " ";

        internal static string[] ReadElements(IntermediateReader input)
        {
            if (input.Xml.IsEmptyElement)
                return Array.Empty<string>();

            string str = string.Empty;
            while (input.Xml.NodeType != XmlNodeType.EndElement)
            {
                if (input.Xml.NodeType == XmlNodeType.Comment)
                    input.Xml.Read();
                else
                    str += input.Xml.ReadString();
            }

            // Special case for char ' '
            if (str.Length > 0 && string.IsNullOrWhiteSpace(str))
                return new string[] { str };

            var elements = str.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
            if (elements.Length == 1 && string.IsNullOrEmpty(elements[0]))
                return Array.Empty<string>();

            return elements;
        }

        public static string JoinElements(IEnumerable<string> elements)
        {
            return string.Join(_writeSeperator, elements);
        }
    }
}