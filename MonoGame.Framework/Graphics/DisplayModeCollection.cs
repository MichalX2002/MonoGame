// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Framework.Graphics
{
    public class DisplayModeCollection : IReadOnlyCollection<DisplayMode>
    {
        private List<DisplayMode> _modes;

        public int Count => _modes.Count;

        public IEnumerable<DisplayMode> this[SurfaceFormat format]
        {
            get => _modes.Where(m => m.Format == format);
        }

        public DisplayModeCollection(IEnumerable<DisplayMode> modes)
        {
            // Sort the modes in a consistent way that happens
            // to match XNA behavior on some graphics devices.

            if (modes == null)
                throw new ArgumentNullException(nameof(modes));

            _modes = new List<DisplayMode>(modes);
            _modes.Sort(delegate (DisplayMode a, DisplayMode b)
            {
                if (a == b)
                    return 0;
                if (a.Format <= b.Format && a.Width <= b.Width && a.Height <= b.Height)
                    return -1;
                return 1;
            });
        }

        public List<DisplayMode>.Enumerator GetEnumerator()
        {
            return _modes.GetEnumerator();
        }

        IEnumerator<DisplayMode> IEnumerable<DisplayMode>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}