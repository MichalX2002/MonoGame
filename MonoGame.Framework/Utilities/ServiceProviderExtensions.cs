using System;

namespace MonoGame.Framework
{
    /// <summary>
    /// Provides helpers for an <see cref="IServiceProvider"/>,
    /// like getting specific services.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets the service object of the specified generic type.
        /// </summary>
        /// <typeparam name="T">The type of the service object.</typeparam>
        /// <param name="provider">The provider to retrieve the service object from.</param>
        public static T GetService<T>(this IServiceProvider provider) where T : class
        {
            return provider.GetService(typeof(T)) as T;
        }
    }
}
