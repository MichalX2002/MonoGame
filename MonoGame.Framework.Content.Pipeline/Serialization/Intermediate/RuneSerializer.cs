// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MonoGame.Framework.Content.Pipeline.Serialization.Intermediate
{
    [ContentTypeSerializer]
    class RuneSerializer : ElementSerializer<Rune>
    {
        public RuneSerializer() : base("Rune", 1)
        {
        }

        protected internal override Rune Deserialize(string[] inputs, ref int index)
        {
            var str = inputs[index++];
            if (str.Length == 1)
                return (Rune)XmlConvert.ToChar(str);

            // Try parsing it as a UTF code.
            if (int.TryParse(str, out int val))
                return (Rune)val;

            // Last ditch effort to decode it as XML escape value.
            return (Rune)XmlConvert.ToChar(XmlConvert.DecodeName(str));
        }

        protected internal override void Serialize(Rune value, List<string> results)
        {
            results.Add(XmlConvert.ToString(value.Value));
        }
    }
}