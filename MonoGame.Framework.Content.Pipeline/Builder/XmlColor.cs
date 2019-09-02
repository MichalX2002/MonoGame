// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Xml.Serialization;
using DrawingColor = System.Drawing.Color;

namespace MonoGame.Framework.Content.Pipeline.Builder
{
    /// <summary>
    /// Helper for serializing color types with the XmlSerializer.
    /// </summary>
    public class XmlColor
    {
        private DrawingColor _color;

        public XmlColor()
        {            
        }

        public XmlColor(DrawingColor c)
        {
            _color = c;
        }

        public static implicit operator DrawingColor(XmlColor x)
        {
            return x._color;
        }

        public static implicit operator XmlColor(DrawingColor c)
        {
            return new XmlColor(c);
        }

        public static string FromColor(DrawingColor color)
        {
            if (color.IsNamedColor)
                return color.Name;
            return string.Format("{0}, {1}, {2}, {3}", color.R, color.G, color.B, color.A);
        }

        public static DrawingColor ToColor(string value)
        {            
            if (!value.Contains(","))
                return DrawingColor.FromName(value);
            var colors = value.Split(',');
            int.TryParse(colors.Length > 0 ? colors[0] : string.Empty, out int r);
            int.TryParse(colors.Length > 1 ? colors[1] : string.Empty, out int g);
            int.TryParse(colors.Length > 2 ? colors[2] : string.Empty, out int b);
            int.TryParse(colors.Length > 3 ? colors[3] : string.Empty, out int a);

            return DrawingColor.FromArgb(a, r, g, b);
        }

        [XmlText]
        public string Default
        {
            get => FromColor(_color);
            set => _color = ToColor(value);
        }
    }
}
