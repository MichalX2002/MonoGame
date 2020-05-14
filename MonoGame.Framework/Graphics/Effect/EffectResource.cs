// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using MonoGame.Framework.Content;
using MonoGame.Framework.Memory;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Internal helper for accessing the bytecode for stock effects.
    /// </summary>
    internal partial class EffectResource
    {
        public static readonly EffectResource AlphaTestEffect = new EffectResource(AlphaTestEffectName);
        public static readonly EffectResource BasicEffect = new EffectResource(BasicEffectName);
        public static readonly EffectResource DualTextureEffect = new EffectResource(DualTextureEffectName);
        public static readonly EffectResource EnvironmentMapEffect = new EffectResource(EnvironmentMapEffectName);
        public static readonly EffectResource SkinnedEffect = new EffectResource(SkinnedEffectName);
        public static readonly EffectResource SpriteEffect = new EffectResource(SpriteEffectName);

        private readonly object _readMutex = new object();
        private readonly string _name;
        private byte[] _byteCode;

        private EffectResource(string name)
        {
            _name = name;
        }

        public byte[] ByteCode
        {
            get
            {
                if (_byteCode == null)
                {
                    lock (_readMutex)
                    {
                        if (_byteCode != null)
                            return _byteCode;

                        var assembly = typeof(EffectResource).Assembly;
                        var stream = assembly.GetManifestResourceStream(_name);
                        if (stream == null)
                            throw new ContentLoadException($"Missing effect resource named \"{_name}\".");

                        _byteCode = new byte[stream.Length];
                        var byteCode = _byteCode.AsMemory();

                        int read;
                        while ((read = stream.Read(byteCode.Span)) != 0)
                            byteCode = byteCode.Slice(read);

                        if (!byteCode.IsEmpty)
                            throw new EndOfStreamException();
                    }
                }
                return _byteCode;
            }
        }
    }
}
