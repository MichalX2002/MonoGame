// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MonoGame.Framework
{
    public class GameServiceContainer : IServiceProvider
    {
        private Dictionary<Type, object> _services;

        public GameServiceContainer()
        {
            _services = new Dictionary<Type, object>();
        }

        public void AddService(Type type, object provider)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (!type.IsAssignableFrom(provider.GetType()))
                throw new ArgumentException("The provider does not match the specified service type.");

            _services.Add(type, provider);
        }

        public void AddService<T>(T provider)
        {
            AddService(typeof(T), provider);
        }

        public object GetService(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (_services.TryGetValue(type, out object service))
                return service;

            return null;
        }

        public bool RemoveService(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return _services.Remove(type);
        }

        public bool RemoveService<T>()
        {
            return RemoveService(typeof(T));
        }
    }
}