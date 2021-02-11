using System;

namespace MonoGame.Imaging
{
    public class MissingImagingModuleException : ImagingException
    {
        public Type ModuleType { get; }

        public MissingImagingModuleException(Type type) : this(null, null, type)
        {
        }

        public MissingImagingModuleException(string? message, Type type) : this(message, null, type)
        {
        }

        public MissingImagingModuleException(
            string? message, Exception? inner, Type type) : base(message ?? $"Missing module of type {type}.", inner)
        {
            ModuleType = type ?? throw new ArgumentNullException(nameof(type));
        }
    }
}
