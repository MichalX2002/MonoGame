using System;

namespace MonoGame.Framework.Content.Pipeline
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class CompressedContentAttribute : Attribute
    {
    }
}
