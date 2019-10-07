// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Content
{
    public class ContentLoadException : Exception
    {
        public ContentLoadException() : base()
        {
        }

        public ContentLoadException(string message) : base(message)
        {
        }

        public ContentLoadException(string message, Exception innerException) : base(message,innerException)
        {
        }
    }
}

