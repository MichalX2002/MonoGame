// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Utilities;

namespace Microsoft.Xna.Framework.Content
{
    public sealed class ContentReader : BinaryReader
    {
        private Action<IDisposable> recordDisposableObject;
        private ContentTypeReaderManager typeReaderManager;
        private string assetName;
        private List<KeyValuePair<int, Action<object>>> sharedResourceFixups;
        internal int version;
		internal int sharedResourceCount;

        internal ContentTypeReader[] TypeReaders { get; private set; }

        internal GraphicsDevice GraphicsDevice { get; }

        internal ContentReader(ContentManager manager, Stream stream, GraphicsDevice graphicsDevice, string assetName, int version, Action<IDisposable> recordDisposableObject)
            : base(stream)
        {
            this.GraphicsDevice = graphicsDevice;
            this.recordDisposableObject = recordDisposableObject;
            this.ContentManager = manager;
            this.assetName = assetName;
			this.version = version;
        }

        public ContentManager ContentManager { get; }

        public string AssetName
        {
            get
            {
                return assetName;
            }
        }

        internal object ReadAsset<T>()
        {
            InitializeTypeReaders();

            // Read primary object
            object result = ReadObject<T>();

            // Read shared resources
            ReadSharedResources();
            
            return result;
        }

        internal object ReadAsset<T>(T existingInstance)
        {
            InitializeTypeReaders();

            // Read primary object
            object result = ReadObject<T>(existingInstance);

            // Read shared resources
            ReadSharedResources();

            return result;
        }

        internal void InitializeTypeReaders()
        {
            typeReaderManager = new ContentTypeReaderManager();
            TypeReaders = typeReaderManager.LoadAssetReaders(this);
            sharedResourceCount = Read7BitEncodedInt();
            sharedResourceFixups = new List<KeyValuePair<int, Action<object>>>();
        }

        internal void ReadSharedResources()
        {
            if (sharedResourceCount <= 0)
                return;

            var sharedResources = new object[sharedResourceCount];
            for (var i = 0; i < sharedResourceCount; ++i)
                sharedResources[i] = InnerReadObject<object>(null);

            // Fixup shared resources by calling each registered action
            foreach (var fixup in sharedResourceFixups)
                fixup.Value(sharedResources[fixup.Key]);
        }

        public T ReadExternalReference<T>()
        {
            var externalReference = ReadString();

            if (!String.IsNullOrEmpty(externalReference))
            {
                return ContentManager.Load<T>(FileHelpers.ResolveRelativePath(assetName, externalReference));
            }

            return default;
        }

        public Matrix ReadMatrix()
        {
            Matrix result = new Matrix
            {
                M11 = ReadSingle(),
                M12 = ReadSingle(),
                M13 = ReadSingle(),
                M14 = ReadSingle(),
                M21 = ReadSingle(),
                M22 = ReadSingle(),
                M23 = ReadSingle(),
                M24 = ReadSingle(),
                M31 = ReadSingle(),
                M32 = ReadSingle(),
                M33 = ReadSingle(),
                M34 = ReadSingle(),
                M41 = ReadSingle(),
                M42 = ReadSingle(),
                M43 = ReadSingle(),
                M44 = ReadSingle()
            };
            return result;
        }
            
        private void RecordDisposable<T>(T result)
        {
            if (!(result is IDisposable disposable))
                return;

            if (recordDisposableObject != null)
                recordDisposableObject(disposable);
            else
                ContentManager.RecordDisposable(disposable);
        }

        public T ReadObject<T>()
        {
            return InnerReadObject(default(T));
        }

        public T ReadObject<T>(ContentTypeReader typeReader)
        {
            var result = (T)typeReader.Read(this, default(T));            
            RecordDisposable(result);
            return result;
        }

        public T ReadObject<T>(T existingInstance)
        {
            return InnerReadObject(existingInstance);
        }

        private T InnerReadObject<T>(T existingInstance)
        {
            var typeReaderIndex = Read7BitEncodedInt();
            if (typeReaderIndex == 0)
                return existingInstance;

            if (typeReaderIndex > TypeReaders.Length)
                throw new ContentLoadException("Incorrect type reader index found!");

            var typeReader = TypeReaders[typeReaderIndex - 1];
            var result = (T)typeReader.Read(this, existingInstance);

            RecordDisposable(result);

            return result;
        }

        public T ReadObject<T>(ContentTypeReader typeReader, T existingInstance)
        {
            if (!ReflectionHelpers.IsValueType(typeReader.TargetType))
                return ReadObject(existingInstance);

            var result = (T)typeReader.Read(this, existingInstance);

            RecordDisposable(result);

            return result;
        }

        public Quaternion ReadQuaternion()
        {
            Quaternion result = new Quaternion
            {
                X = ReadSingle(),
                Y = ReadSingle(),
                Z = ReadSingle(),
                W = ReadSingle()
            };
            return result;
        }

        public T ReadRawObject<T>()
        {
			return ReadRawObject<T>(default(T));
        }

        public T ReadRawObject<T>(ContentTypeReader typeReader)
        {
            return ReadRawObject<T>(typeReader, default);
        }

        public T ReadRawObject<T>(T existingInstance)
        {
            Type objectType = typeof(T);
            foreach(ContentTypeReader typeReader in TypeReaders)
            {
                if(typeReader.TargetType == objectType)
                    return ReadRawObject<T>(typeReader, existingInstance);
            }
            throw new NotSupportedException();
        }

        public T ReadRawObject<T>(ContentTypeReader typeReader, T existingInstance)
        {
            return (T)typeReader.Read(this, existingInstance);
        }

        public void ReadSharedResource<T>(Action<T> fixup)
        {
            int index = Read7BitEncodedInt();
            if (index > 0)
            {
                sharedResourceFixups.Add(new KeyValuePair<int, Action<object>>(index - 1, delegate(object v)
                    {
                        if (!(v is T))
                        {
                            throw new ContentLoadException(String.Format("Error loading shared resource. Expected type {0}, received type {1}", typeof(T).Name, v.GetType().Name));
                        }
                        fixup((T)v);
                    }));
            }
        }

        public Vector2 ReadVector2()
        {
            Vector2 result = new Vector2
            {
                X = ReadSingle(),
                Y = ReadSingle()
            };
            return result;
        }

        public Vector3 ReadVector3()
        {
            Vector3 result = new Vector3
            {
                X = ReadSingle(),
                Y = ReadSingle(),
                Z = ReadSingle()
            };
            return result;
        }

        public Vector4 ReadVector4()
        {
            Vector4 result = new Vector4
            {
                X = ReadSingle(),
                Y = ReadSingle(),
                Z = ReadSingle(),
                W = ReadSingle()
            };
            return result;
        }

        public Color ReadColor()
        {
            Color result = new Color
            {
                R = ReadByte(),
                G = ReadByte(),
                B = ReadByte(),
                A = ReadByte()
            };
            return result;
        }

        internal new int Read7BitEncodedInt()
        {
            return base.Read7BitEncodedInt();
        }
		
		internal BoundingSphere ReadBoundingSphere()
		{
			var position = ReadVector3();
            var radius = ReadSingle();
            return new BoundingSphere(position, radius);
		}
    }
}
