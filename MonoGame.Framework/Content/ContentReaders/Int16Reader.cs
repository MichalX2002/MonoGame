// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

using MonoGame.Framework.Content;

namespace MonoGame.Framework.Content
{
	internal class Int16Reader : ContentTypeReader<short>
	{
		public Int16Reader ()
		{
		}

		protected internal override short Read (ContentReader input, short existingInstance)
		{
			return input.ReadInt16 ();
		}
	}
}
