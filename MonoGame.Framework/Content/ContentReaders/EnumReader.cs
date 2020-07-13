// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Content
{
    public class EnumReader<T> : ContentTypeReader<T>
    {
        private ContentTypeReader _elementReader;

        public EnumReader()
        {
        }

        protected internal override void Initialize(ContentTypeReaderManager manager)
        {			
			Type readerType = Enum.GetUnderlyingType(typeof(T));
			_elementReader = manager.GetTypeReader(readerType);
        }
		
        protected internal override T Read(ContentReader input, T existingInstance)
        {
			return input.ReadRawObject<T>(_elementReader);
		}
    }
}

