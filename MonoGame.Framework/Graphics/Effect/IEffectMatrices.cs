// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Graphics
{
	public interface IEffectMatrices
	{
		Matrix Projection { get; set; }
		Matrix View { get; set; }
		Matrix World { get; set; }
	}
}

