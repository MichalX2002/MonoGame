// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using MonoGame.Framework.Utilities;

namespace MonoGame.Framework.Content
{
    internal class ListReader<T> : ContentTypeReader<List<T>>
    {
        private ContentTypeReader _elementReader;

        public override bool CanDeserializeIntoExistingObject => true;

        public ListReader()
        {
        }

        protected internal override void Initialize(ContentTypeReaderManager manager)
        {
			_elementReader = manager.GetTypeReader(typeof(T));
        }

        protected internal override List<T> Read(ContentReader input, List<T> existingInstance)
        {
            int count = input.ReadInt32();
            var list = existingInstance ?? new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                if (ReflectionHelpers.IsValueType<T>())
				{
                	list.Add(input.ReadObject<T>(_elementReader));
				}
				else
				{
                    var readerType = input.Read7BitEncodedInt();
                	list.Add(readerType > 0 ? input.ReadObject<T>(input.TypeReaders[readerType - 1]) : default);
				}
            }
            return list;
        }
    }
}
