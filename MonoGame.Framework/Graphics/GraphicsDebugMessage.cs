// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Graphics
{
    public readonly struct GraphicsDebugMessage
    {
        public int Id { get; }
        public string IdName { get; }
        public string Severity { get; }
        public string Category { get; }
        public string Message { get; }

        public GraphicsDebugMessage(int id, string idName, string severity, string category, string message)
        {
            Id = id;
            IdName = idName ?? throw new ArgumentNullException(nameof(idName));
            Severity = severity ?? throw new ArgumentNullException(nameof(severity));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
