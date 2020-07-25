// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace MonoGame.Framework.Content
{
    public sealed class ContentTypeReaderManager
    {
        private static object SyncRoot { get; } = new object();

        private static Dictionary<Type, ContentTypeReader> ContentReaderCache { get; }
            = new Dictionary<Type, ContentTypeReader>(255);

        /// <summary>
        /// Static map of type names to creation functions.
        /// Required as iOS requires all types at compile time.
        /// </summary>
        private static Dictionary<string, Func<ContentTypeReader>> ReaderFactories { get; } =
            new Dictionary<string, Func<ContentTypeReader>>();

        private Dictionary<Type, ContentTypeReader>? _contentReaders;

        public ContentTypeReader? GetTypeReader(Type targetType)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            if (targetType.IsArray && targetType.GetArrayRank() > 1)
                targetType = typeof(Array);

            if (_contentReaders!.TryGetValue(targetType, out var reader))
                return reader;

            return null;
        }

        public ContentTypeReader? GetTypeReader<T>()
        {
            return GetTypeReader(typeof(T));
        }

        // Trick to prevent the linker removing the code, but not actually execute the code
        static readonly bool falseflag = false;

        internal ContentTypeReader[] LoadAssetReaders(ContentReader reader)
        {
            // Trick to prevent the linker removing the code, but not actually execute the code
            if (falseflag)
            {
                // Dummy variables required for it to work on iDevices ** DO NOT DELETE ** 
                // This forces the classes not to be optimized out when deploying to iDevices
                var hByteReader = new ByteReader();
                var hSByteReader = new SByteReader();
                var hDateTimeReader = new DateTimeReader();
                var hDecimalReader = new DecimalReader();
                var hBoundingSphereReader = new BoundingSphereReader();
                var hBoundingFrustumReader = new BoundingFrustumReader();
                var hRayReader = new RayReader();
                var hCharListReader = new ListReader<char>();
                var hRuneListReader = new ListReader<Rune>();
                var hNullableRectReader = new NullableReader<Rectangle>();
                var hRectangleArrayReader = new ArrayReader<Rectangle>();
                var hRectangleListReader = new ListReader<Rectangle>();
                var hVector3ArrayReader = new ArrayReader<Vector3>();
                var hVector3ListReader = new ListReader<Vector3>();
                var hVector2ArrayReader = new ArrayReader<Vector2>();
                var hVector2ListReader = new ListReader<Vector2>();
                var hStringListReader = new ListReader<StringReader>();
                var hIntListReader = new ListReader<int>();
                var hSpriteFontReader = new SpriteFontReader();
                var hTexture2DReader = new Texture2DReader();
                var hCharReader = new CharReader();
                var hRuneReader = new RuneReader();
                var hRectangleReader = new RectangleReader();
                var hStringReader = new StringReader();
                var hVector2Reader = new Vector2Reader();
                var hVector3Reader = new Vector3Reader();
                var hVector4Reader = new Vector4Reader();
                var hCurveReader = new CurveReader();
                var hIndexBufferReader = new IndexBufferReader();
                var hBoundingBoxReader = new BoundingBoxReader();
                var hMatrixReader = new MatrixReader();
                var hBasicEffectReader = new BasicEffectReader();
                var hVertexBufferReader = new VertexBufferReader();
                var hAlphaTestEffectReader = new AlphaTestEffectReader();
                var hEnumSpriteEffectsReader = new EnumReader<Graphics.SpriteFlip>();
                var hArrayFloatReader = new ArrayReader<float>();
                var hArrayMatrixReader = new ArrayReader<Matrix4x4>();
                var hEnumBlendReader = new EnumReader<Graphics.Blend>();
                var hEffectMaterialReader = new EffectMaterialReader();
                var hExternalReferenceReader = new ExternalReferenceReader();
                var hSoundEffectReader = new SoundEffectReader();
                var hSongReader = new SongReader();
                var hModelReader = new ModelReader();
                var hInt32Reader = new Int32Reader();
                var hEffectReader = new EffectReader();
                var hSingleReader = new SingleReader();

                // At the moment the Video class doesn't exist
                // on all platforms... Allow it to compile anyway.
#if (IOS && !TVOS) || MONOMAC || (WINDOWS && !OPENGL) || WINDOWS_UAP
                var hVideoReader = new VideoReader();
#endif
            }

            // The first content byte i read tells me the number of content readers in this XNB file
            var numberOfReaders = reader.Read7BitEncodedInt();
            var contentReaders = new ContentTypeReader[numberOfReaders];
            var needsInitialize = new BitArray(numberOfReaders);
            _contentReaders = new Dictionary<Type, ContentTypeReader>(numberOfReaders);

            // Lock until we're done allocating and initializing any new
            // content type readers...  this ensures we can load content
            // from multiple threads and still cache the readers.
            lock (SyncRoot)
            {
                // For each reader in the file, 
                // we read out the length of the string which contains the type of the reader,
                // then we read out the string. 
                // Finally we instantiate an instance of that reader using reflection
                for (var i = 0; i < numberOfReaders; i++)
                {
                    // This string tells us what reader we need to decode the following data
                    // string readerTypeString = reader.ReadString();
                    string originalReaderTypeString = reader.ReadString();

                    if (ReaderFactories.TryGetValue(originalReaderTypeString, out var readerFactory))
                    {
                        contentReaders[i] = readerFactory.Invoke();
                        needsInitialize[i] = true;
                    }
                    else
                    {
                        //System.Diagnostics.Debug.WriteLine(originalReaderTypeString);

                        // Need to resolve namespace differences
                        string readerTypeString = originalReaderTypeString;
                        readerTypeString = PrepareType(readerTypeString);

                        var l_readerType = Type.GetType(readerTypeString);
                        if (l_readerType != null)
                        {
                            if (!ContentReaderCache.TryGetValue(l_readerType, out var typeReader))
                            {
                                try
                                {
                                    typeReader = (ContentTypeReader)l_readerType.GetDefaultConstructor().Invoke(null);
                                }
                                catch (TargetInvocationException ex)
                                {
                                    // If you are getting here, the Mono runtime is most likely not able to JIT the type.
                                    // In particular, MonoTouch needs help instantiating types
                                    // that are only defined in strings in XNB files. 
                                    throw new InvalidOperationException(
                                        $"Failed to get default constructor for {nameof(ContentTypeReader)}. " +
                                        $"To work around, add a factory function " +
                                        $"(with {nameof(ContentTypeReaderManager)}.{nameof(AddReaderFactory)}) " +
                                        "with the following type string: " + originalReaderTypeString, ex);
                                }

                                needsInitialize[i] = true;

                                ContentReaderCache.Add(l_readerType, typeReader);
                            }

                            contentReaders[i] = typeReader;
                        }
                        else
                        {
                            throw new ContentLoadException(
                                $"Could not find {nameof(ContentTypeReader)} Type. " +
                                "Please ensure the name of the assembly " +
                                "that contains the type matches the assembly in the full type name: " +
                                originalReaderTypeString + " (" + readerTypeString + ")");
                        }
                    }

                    var targetType = contentReaders[i].TargetType;
                    if (targetType != null)
                    {
                        if (!_contentReaders.ContainsKey(targetType))
                            _contentReaders.Add(targetType, contentReaders[i]);
                    }

                    // I think the next 4 bytes refer to the "Version" of the type reader,
                    // although it always seems to be zero
                    reader.ReadInt32();
                }

                // Initialize any new readers.
                for (int i = 0; i < contentReaders.Length; i++)
                {
                    if (needsInitialize.Get(i))
                        contentReaders[i].Initialize(this);
                }
            }

            return contentReaders;
        }

        /// <summary>
        /// Removes Version, Culture and PublicKeyToken from a type string.
        /// </summary>
        /// <remarks>
        /// Supports multiple generic types 
        /// (e.g. <see cref="Dictionary{TKey, TValue}"/>) and nested generic types (e.g. List&lt;List&lt;int&gt;&gt;).
        /// </remarks> 
        /// <param name="type">The string contaning a full type description.</param>
        /// <returns>A type description string without external identifiers.</returns>
        public static string PrepareType(string type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // Needed to support nested types
            int count = type.Split("[[", StringSplitOptions.None).Length - 1;

            string preparedType = type;

            for (int i = 0; i < count; i++)
            {
                preparedType = Regex.Replace(
                    preparedType, @"\[(.+?), Version=.+?\]", "[$1]", RegexOptions.Compiled);
            }

            // Handle non generic types
            if (preparedType.Contains("PublicKeyToken", StringComparison.OrdinalIgnoreCase))
            {
                preparedType = Regex.Replace(
                    preparedType, @"(.+?), Version=.+?$", "$1", RegexOptions.Compiled);
            }

            return preparedType;
        }

        /// <summary>
        /// Adds a reader factory.
        /// </summary>
        /// <param name='typeString'>Type string.</param>
        /// <param name='factory'>Create function.</param>
        public static void AddReaderFactory(string typeString, Func<ContentTypeReader> factory)
        {
            if (!ReaderFactories.ContainsKey(typeString))
                ReaderFactories.Add(typeString, factory);
        }

        public static void ClearReaderFactories()
        {
            ReaderFactories.Clear();
        }

    }
}
