// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Numerics;

namespace MonoGame.Framework.Graphics
{
    public sealed class DirectionalLight
    {
        internal EffectParameter? _diffuseColorParameter;
        internal EffectParameter? _directionParameter;
        internal EffectParameter? _specularColorParameter;

        private Vector3 _diffuseColor;
        private Vector3 _direction;
        private Vector3 _specularColor;
        private bool _enabled;

        public DirectionalLight(
            EffectParameter? directionParameter,
            EffectParameter? diffuseColorParameter,
            EffectParameter? specularColorParameter,
            DirectionalLight? cloneSource)
        {
            _diffuseColorParameter = diffuseColorParameter;
            _directionParameter = directionParameter;
            _specularColorParameter = specularColorParameter;

            if (cloneSource != null)
            {
                _diffuseColor = cloneSource._diffuseColor;
                _direction = cloneSource._direction;
                _specularColor = cloneSource._specularColor;
                _enabled = cloneSource._enabled;
            }
            else
            {
                _diffuseColorParameter = diffuseColorParameter;
                _directionParameter = directionParameter;
                _specularColorParameter = specularColorParameter;
            }
        }

        public Vector3 DiffuseColor
        {
            get => _diffuseColor;
            set
            {
                _diffuseColor = value;
                if (_enabled)
                    _diffuseColorParameter?.SetValue(_diffuseColor);
            }
        }

        public Vector3 Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                _directionParameter?.SetValue(_direction);
            }
        }

        public Vector3 SpecularColor
        {
            get => _specularColor;
            set
            {
                _specularColor = value;
                if (_enabled)
                    _specularColorParameter?.SetValue(_specularColor);
            }
        }
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    if (_enabled)
                    {
                        _diffuseColorParameter?.SetValue(_diffuseColor);
                        _specularColorParameter?.SetValue(_specularColor);
                    }
                    else
                    {
                        _diffuseColorParameter?.SetValue(Vector3.Zero);
                        _specularColorParameter?.SetValue(Vector3.Zero);
                    }
                }

            }
        }
    }
}

