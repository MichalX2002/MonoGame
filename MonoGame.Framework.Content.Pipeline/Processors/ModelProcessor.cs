﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using MonoGame.Framework.Content.Pipeline.Graphics;
using MonoGame.Framework.Graphics;
using MonoGame.Framework.Vectors;

namespace MonoGame.Framework.Content.Pipeline.Processors
{
    [ContentProcessor(DisplayName = "Model - MonoGame")]
    [SkipLocalsInit]
    public class ModelProcessor : ContentProcessor<NodeContent, ModelContent>
    {
        private const bool DefaultColorKeyEnabled = true;
        private const bool DefaultGenerateMipmaps = true;
        private const bool DefaultPremultiplyTextureAlpha = true;
        private const bool DefaultPremultiplyVertexColors = true;
        private const float DefaultScale = 1.0f;
        private const TextureProcessorOutputFormat DefaultTextureFormat = TextureProcessorOutputFormat.Compressed;

        private ContentIdentity _identity;

        public ModelProcessor()
        {
        }

        #region Properties

        public virtual Color ColorKeyColor { get; set; }

        [DefaultValue(DefaultColorKeyEnabled)]
        public virtual bool ColorKeyEnabled { get; set; } = DefaultColorKeyEnabled;

        public virtual MaterialProcessorDefaultEffect DefaultEffect { get; set; }

        [DefaultValue(DefaultGenerateMipmaps)]
        public virtual bool GenerateMipmaps { get; set; } = DefaultGenerateMipmaps;

        public virtual bool GenerateTangentFrames { get; set; }

        [DefaultValue(DefaultPremultiplyTextureAlpha)]
        public virtual bool PremultiplyTextureAlpha { get; set; } = DefaultPremultiplyTextureAlpha;

        [DefaultValue(DefaultPremultiplyVertexColors)]
        public virtual bool PremultiplyVertexColors { get; set; } = DefaultPremultiplyVertexColors;

        public virtual bool ResizeTexturesToPowerOfTwo { get; set; }

        public virtual float RotationX { get; set; }
        public virtual float RotationY { get; set; }
        public virtual float RotationZ { get; set; }

        [DefaultValue(DefaultScale)]
        public virtual float Scale { get; set; } = DefaultScale;

        public virtual bool SwapWindingOrder { get; set; }

        [DefaultValue(DefaultTextureFormat)]
        public virtual TextureProcessorOutputFormat TextureFormat { get; set; } = DefaultTextureFormat;

        #endregion

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            _identity = input.Identity;

            // Perform the processor transforms.
            if (RotationX != 0.0f || RotationY != 0.0f || RotationZ != 0.0f || Scale != 1.0f)
            {
                var rotX = Matrix4x4.CreateRotationX(MathHelper.ToRadians(RotationX));
                var rotY = Matrix4x4.CreateRotationY(MathHelper.ToRadians(RotationY));
                var rotZ = Matrix4x4.CreateRotationZ(MathHelper.ToRadians(RotationZ));
                var scale = Matrix4x4.CreateScale(Scale);
                MeshHelper.TransformScene(input, rotZ * rotX * rotY * scale);
            }

            // Gather all the nodes in tree traversal order.
            var nodes = input.AsEnumerable().SelectDeep(n => n.Children);

            var meshes = nodes.Where(n => n is MeshContent).Cast<MeshContent>();
            var geometries = meshes.SelectMany(m => m.Geometry);
            var distinctMaterials = geometries.Select(g => g.Material).Distinct();

            // Loop through all distinct materials, passing them through the conversion method
            // only once, and then processing all geometries using that material.
            foreach (var inputMaterial in distinctMaterials)
            {
                var geomsWithMaterial = geometries.Where(g => g.Material == inputMaterial);
                var material = ConvertMaterial(inputMaterial, context);

                ProcessGeometryUsingMaterial(material, geomsWithMaterial, context);
            }

            var boneList = new List<ModelBoneContent>();
            var meshList = new List<ModelMeshContent>();
            var rootNode = ProcessNode(input, null, boneList, meshList, context);

            return new ModelContent(rootNode, boneList, meshList);
        }

        private ModelBoneContent ProcessNode(
            NodeContent node,
            ModelBoneContent parent,
            List<ModelBoneContent> boneList,
            List<ModelMeshContent> meshList,
            ContentProcessorContext context)
        {
            var result = new ModelBoneContent(node.Name, boneList.Count, node.Transform, parent);
            boneList.Add(result);

            if (node is MeshContent)
                meshList.Add(ProcessMesh(node as MeshContent, result, context));

            var children = new List<ModelBoneContent>();
            foreach (var child in node.Children)
                children.Add(ProcessNode(child, result, boneList, meshList, context));
            result.Children = new ModelBoneContentCollection(children);

            return result;
        }

        private ModelMeshContent ProcessMesh(
            MeshContent mesh, ModelBoneContent parent, ContentProcessorContext context)
        {
            var parts = new List<ModelMeshPartContent>();
            var vertexBuffer = new VertexBufferContent();
            var indexBuffer = new IndexCollection();

            if (GenerateTangentFrames)
            {
                context.Logger.LogMessage("Generating tangent frames.");
                foreach (GeometryContent geom in mesh.Geometry)
                {
                    if (!geom.Vertices.Channels.Contains(VertexChannelNames.Normal(0)))
                    {
                        MeshHelper.CalculateNormals(geom, true);
                    }

                    if (!geom.Vertices.Channels.Contains(VertexChannelNames.Tangent(0)) ||
                        !geom.Vertices.Channels.Contains(VertexChannelNames.Binormal(0)))
                    {
                        MeshHelper.CalculateTangentFrames(
                            geom,
                            VertexChannelNames.TextureCoordinate(0),
                            VertexChannelNames.Tangent(0),
                            VertexChannelNames.Binormal(0));
                    }
                }
            }

            var startVertex = 0;
            foreach (var geometry in mesh.Geometry)
            {
                var vertices = geometry.Vertices;
                var vertexCount = vertices.VertexCount;
                ModelMeshPartContent partContent;
                if (vertexCount == 0)
                    partContent = new ModelMeshPartContent();
                else
                {
                    var geomBuffer = geometry.Vertices.CreateVertexBuffer();
                    vertexBuffer.Write(vertexBuffer.VertexData.Length, 1, geomBuffer.VertexData);

                    var startIndex = indexBuffer.Count;
                    indexBuffer.AddRange(geometry.Indices);

                    partContent = new ModelMeshPartContent(
                        vertexBuffer, indexBuffer,
                        startVertex, vertexCount, startIndex, geometry.Indices.Count / 3);

                    // Geoms are supposed to all have the same decl, so just steal one of these
                    vertexBuffer.VertexDeclaration = geomBuffer.VertexDeclaration;

                    startVertex += vertexCount;
                }

                partContent.Material = geometry.Material;
                parts.Add(partContent);
            }

            var bounds = new BoundingSphere();
            if (mesh.Positions.Count > 0)
                bounds = BoundingSphere.CreateFromPoints(mesh.Positions);

            return new ModelMeshContent(mesh.Name, mesh, parent, bounds, parts);
        }

        protected virtual MaterialContent ConvertMaterial(
            MaterialContent material, ContentProcessorContext context)
        {
            var parameters = new OpaqueDataDictionary();
            parameters.Add("ColorKeyColor", ColorKeyColor);
            parameters.Add("ColorKeyEnabled", ColorKeyEnabled);
            parameters.Add("GenerateMipmaps", GenerateMipmaps);
            parameters.Add("PremultiplyTextureAlpha", PremultiplyTextureAlpha);
            parameters.Add("ResizeTexturesToPowerOfTwo", ResizeTexturesToPowerOfTwo);
            parameters.Add("TextureFormat", TextureFormat);
            parameters.Add("DefaultEffect", DefaultEffect);

            return context.Convert<MaterialContent, MaterialContent>(
                material, "MaterialProcessor", parameters);
        }

        protected virtual void ProcessGeometryUsingMaterial(
            MaterialContent material,
            IEnumerable<GeometryContent> geometryCollection,
            ContentProcessorContext context)
        {
            // If we don't get a material then assign a default one.
            if (material == null)
                material = MaterialProcessor.CreateDefaultMaterial(DefaultEffect);

            // Test requirements from the assigned material.
            int textureChannels;
            bool vertexWeights = false;
            if (material is DualTextureMaterialContent)
            {
                textureChannels = 2;
            }
            else if (material is SkinnedMaterialContent)
            {
                textureChannels = 1;
                vertexWeights = true;
            }
            else if (material is EnvironmentMapMaterialContent)
            {
                textureChannels = 1;
            }
            else if (material is AlphaTestMaterialContent)
            {
                textureChannels = 1;
            }
            else
            {
                // Just check for a "Texture" which should cover custom Effects
                // and BasicEffect which can have an optional texture.
                textureChannels = material.Textures.ContainsKey("Texture") ? 1 : 0;
            }

            // By default we must set the vertex color property to match XNA behavior.
            material.OpaqueData["VertexColorEnabled"] = false;

            // If we run into a geometry that requires vertex color we need a seperate material for it.
            var colorMaterial = material.Clone();
            colorMaterial.OpaqueData["VertexColorEnabled"] = true;

            foreach (var geometry in geometryCollection)
            {
                // Process the geometry.
                for (var i = 0; i < geometry.Vertices.Channels.Count; i++)
                    ProcessVertexChannel(geometry, i, context);

                // Verify we have the right number of texture coords.
                for (var i = 0; i < textureChannels; i++)
                {
                    if (!geometry.Vertices.Channels.Contains(VertexChannelNames.TextureCoordinate(i)))
                        throw new InvalidContentException(
                            $"The mesh \"{geometry.Parent.Name}\", " +
                            $"using { MaterialProcessor.GetDefaultEffect(material)}, " +
                            $"contains geometry that is missing texture coordinates for channel {i}.",
                            _identity);
                }

                // Do we need to enable vertex color?
                if (geometry.Vertices.Channels.Contains(VertexChannelNames.Color(0)))
                    geometry.Material = colorMaterial;
                else
                    geometry.Material = material;

                // Do we need vertex weights?
                if (vertexWeights)
                {
                    var weightsName = VertexChannelNames.EncodeName(VertexElementUsage.BlendWeight, 0);
                    if (!geometry.Vertices.Channels.Contains(weightsName))
                        throw new InvalidContentException(
                            $"The skinned mesh \"{geometry.Parent.Name}\" contains " +
                            $"geometry without any vertex weights.",
                            _identity);
                }
            }
        }

        protected virtual void ProcessVertexChannel(
            GeometryContent geometry, int vertexChannelIndex, ContentProcessorContext context)
        {
            var channel = geometry.Vertices.Channels[vertexChannelIndex];

            // TODO: According to docs, channels with VertexElementUsage.Color -> Color

            // Channels[VertexChannelNames.Weights] -> { Byte4 boneIndices, Color boneWeights }
            if (channel.Name.StartsWith(VertexChannelNames.Weights(), StringComparison.Ordinal))
                ProcessWeightsChannel(geometry, vertexChannelIndex, _identity);
        }

        private static void ProcessWeightsChannel(
            GeometryContent geometry, int vertexChannelIndex, ContentIdentity identity)
        {
            // NOTE: Portions of this code is from the XNA CPU Skinning 
            // sample under Ms-PL, (c) Microsoft Corporation.

            // create a map of Name->Index of the bones
            var skeleton = MeshHelper.FindSkeleton(geometry.Parent);
            if (skeleton == null)
            {
                throw new InvalidContentException(
                    "Skeleton not found. Meshes that contain a Weights vertex channel cannot " +
                    "be processed without access to the skeleton data.",
                    identity);
            }

            var boneIndices = new Dictionary<string, byte>();
            var flattenedBones = MeshHelper.FlattenSkeleton(skeleton);
            if (flattenedBones.Count > byte.MaxValue)
                throw new NotSupportedException("The flattened skeleton contains more than 255 bones.");

            for (int i = 0; i < flattenedBones.Count; i++)
                boneIndices.Add(flattenedBones[i].Name, (byte)i);

            var vertexChannel = geometry.Vertices.Channels[vertexChannelIndex];
            if (!(vertexChannel is VertexChannel<BoneWeightCollection> inputWeights))
            {
                throw new InvalidContentException(
                    string.Format(
                        "Vertex channel \"{0}\" is the wrong type. It has element type {1}. Type {2} is expected.",
                        vertexChannel.Name, vertexChannel.ElementType.FullName, typeof(BoneWeightCollection).FullName),
                    identity);
            }
            var outputIndices = new Byte4[inputWeights.Count];
            var outputWeights = new Vector4[inputWeights.Count];
            for (var i = 0; i < inputWeights.Count; i++)
                ConvertWeights(inputWeights[i], boneIndices, outputIndices, outputWeights, i);

            // create our new channel names
            var usageIndex = VertexChannelNames.DecodeUsageIndex(inputWeights.Name);
            var indicesName = VertexChannelNames.EncodeName(VertexElementUsage.BlendIndices, usageIndex);
            var weightsName = VertexChannelNames.EncodeName(VertexElementUsage.BlendWeight, usageIndex);

            // add in the index and weight channels
            geometry.Vertices.Channels.Insert(vertexChannelIndex + 1, indicesName, outputIndices);
            geometry.Vertices.Channels.Insert(vertexChannelIndex + 2, weightsName, outputWeights);

            // remove the original weights channel
            geometry.Vertices.Channels.RemoveAt(vertexChannelIndex);
        }

        // From the XNA CPU Skinning Sample under Ms-PL, (c) Microsoft Corporation
        private static void ConvertWeights(
            BoneWeightCollection weights,
            Dictionary<string, byte> boneIndices,
            Byte4[] outIndices,
            Vector4[] outWeights,
            int vertexIndex)
        {
            // we only handle 4 weights per bone
            const int maxWeights = 4;

            // create some tmp spans to hold our values
            Span<byte> tmpIndices = stackalloc byte[maxWeights];
            Span<float> tmpWeights = stackalloc float[maxWeights];

            // cull out any extra bones
            weights.NormalizeWeights(maxWeights);

            // get our indices and weights
            for (var i = 0; i < weights.Count; i++)
            {
                var weight = weights[i];
                if (!boneIndices.ContainsKey(weight.BoneName))
                    throw new Exception(string.Format(
                        "Bone '{0}' was not found in the skeleton! Skeleton bones are: '{1}'.",
                        weight.BoneName, string.Join("', '", boneIndices.Keys)));

                tmpIndices[i] = boneIndices[weight.BoneName];
                tmpWeights[i] = weight.Weight;
            }

            // zero out any remaining spaces
            for (int i = weights.Count; i < maxWeights; i++)
            {
                tmpIndices[i] = 0;
                tmpWeights[i] = 0;
            }

            // output the values
            outIndices[vertexIndex] = new Byte4(tmpIndices[0], tmpIndices[1], tmpIndices[2], tmpIndices[3]);
            outWeights[vertexIndex] = new Vector4(tmpWeights[0], tmpWeights[1], tmpWeights[2], tmpWeights[3]);
        }
    }

    internal static class ModelEnumerableExtensions
    {
        /// <summary>
        /// Returns each element of a tree structure in hierarchical order.
        /// </summary>
        /// <typeparam name="T">The enumerated type.</typeparam>
        /// <param name="source">The enumeration to traverse.</param>
        /// <param name="selector">A function which returns the children of the element.</param>
        /// <returns>An IEnumerable whose elements are in tree structure heriarchical order.</returns>
        public static IEnumerable<T> SelectDeep<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            var stack = new Stack<T>(source.Reverse());
            while (stack.Count > 0)
            {
                // Return the next item on the stack.
                var item = stack.Pop();
                yield return item;

                // Get the children from this item.
                var children = selector(item);

                // If we have no children then skip it.
                if (children == null)
                    continue;

                // We're using a stack, so we need to push the
                // children on in reverse to get the correct order.
                foreach (var child in children.Reverse())
                    stack.Push(child);
            }
        }

        /// <summary>
        /// Returns an enumerable from a single element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }
    }
}