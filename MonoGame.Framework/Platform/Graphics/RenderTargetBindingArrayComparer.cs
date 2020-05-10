// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MonoGame.Framework.Graphics
{
    public partial class GraphicsDevice
    {
        private class RenderTargetBindingArrayComparer : IEqualityComparer<RenderTargetBinding[]>
        {
            public static RenderTargetBindingArrayComparer Instance { get; } =
                new RenderTargetBindingArrayComparer();

            public bool Equals(RenderTargetBinding[] first, RenderTargetBinding[] second)
            {
                if (ReferenceEquals(first, second))
                    return true;

                if (first == null || second == null)
                    return false;

                if (first.Length != second.Length)
                    return false;

                for (var i = 0; i < first.Length; ++i)
                {
                    if (first[i].RenderTarget != second[i].RenderTarget ||
                        first[i].ArraySlice != second[i].ArraySlice)
                        return false;
                }

                return true;
            }

            public int GetHashCode(RenderTargetBinding[] array)
            {
                if (array == null)
                    return 0;

                var code = new HashCode();
                foreach (var item in array)
                {
                    if (item.RenderTarget != null)
                        code.Add(item.RenderTarget);
                    code.Add(item.ArraySlice);
                }
                return code.ToHashCode();
            }
        }

    }
}