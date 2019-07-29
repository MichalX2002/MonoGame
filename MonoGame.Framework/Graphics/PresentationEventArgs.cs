// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Graphics
{
    internal readonly struct PresentationEventArgs
    {
        public PresentationParameters PresentationParameters { get; }

        public PresentationEventArgs(PresentationParameters presentationParameters)
        {
            PresentationParameters = presentationParameters ?? throw new ArgumentNullException(nameof(presentationParameters));
        }
    }
}
