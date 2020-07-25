// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Numerics;

namespace MonoGame.Framework.Graphics
{
	public interface IEffectMatrices
	{
		Matrix4x4 Projection { get; set; }
		Matrix4x4 View { get; set; }
		Matrix4x4 World { get; set; }
	}
}

