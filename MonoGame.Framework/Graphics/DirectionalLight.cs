// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
	public sealed class DirectionalLight
	{
		internal EffectParameter diffuseColorParameter;
		internal EffectParameter directionParameter;
		internal EffectParameter specularColorParameter;
		
		Vector3 diffuseColor;
		Vector3 direction;
		Vector3 specularColor;
		bool enabled;
		
		public DirectionalLight (EffectParameter directionParameter, EffectParameter diffuseColorParameter, EffectParameter specularColorParameter, DirectionalLight cloneSource)
		{
			this.diffuseColorParameter = diffuseColorParameter;
			this.directionParameter = directionParameter;
			this.specularColorParameter = specularColorParameter;
			if (cloneSource != null) {
				diffuseColor = cloneSource.diffuseColor;
				direction = cloneSource.direction;
				specularColor = cloneSource.specularColor;
				enabled = cloneSource.enabled;
			} else {
				this.diffuseColorParameter = diffuseColorParameter;
				this.directionParameter = directionParameter;
				this.specularColorParameter = specularColorParameter;
			}
		}
		
		public Vector3 DiffuseColor
        {
            get => diffuseColor;
            set
            {
                diffuseColor = value;
                if (enabled && diffuseColorParameter != null)
                    diffuseColorParameter.SetValue(diffuseColor);
            }
        }

        public Vector3 Direction
        {
            get => direction;
            set
            {
                direction = value;
                if (directionParameter != null)
                    directionParameter.SetValue(direction);
            }
        }

        public Vector3 SpecularColor
        {
            get => specularColor;
            set
            {
                specularColor = value;
                if (enabled && specularColorParameter != null)
                    specularColorParameter.SetValue(specularColor);
            }
        }
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    if (enabled)
                    {
                        if (diffuseColorParameter != null)
                        {
                            diffuseColorParameter.SetValue(diffuseColor);
                        }
                        if (specularColorParameter != null)
                        {
                            specularColorParameter.SetValue(specularColor);
                        }
                    }
                    else
                    {
                        if (diffuseColorParameter != null)
                        {
                            diffuseColorParameter.SetValue(Vector3.Zero);
                        }
                        if (specularColorParameter != null)
                        {
                            specularColorParameter.SetValue(Vector3.Zero);
                        }
                    }
                }

            }
        }
    }
}

