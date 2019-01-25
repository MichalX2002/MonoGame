﻿using System;

namespace Microsoft.Xna.Framework
{
    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider provider) where T : class
        {
            return provider.GetService(typeof(T)) as T;
        }
    }
}
