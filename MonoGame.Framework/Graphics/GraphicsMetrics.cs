// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.Serialization;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// A snapshot of rendering statistics from <see cref="GraphicsDevice.Metrics"/>
    /// to be used for runtime debugging and profiling.
    /// </summary>
    [Serializable, DataContract]
    public struct GraphicsMetrics
    {
        [DataMember(Name = "ClearCount")] internal long _clearCount;
        [DataMember(Name = "DrawCount")] internal long _drawCount;
        [DataMember(Name = "PrimitiveCount")] internal long _primitiveCount;
        [DataMember(Name = "SpriteCount")] internal long _spriteCount;
        [DataMember(Name = "TargetCount")] internal long _targetCount;
        [DataMember(Name = "TextureCount")] internal long _textureCount;
        [DataMember(Name = "VertexShaderCount")] internal long _vertexShaderCount;
        [DataMember(Name = "PixelShaderCount")] internal long _pixelShaderCount;

        /// <summary>
        /// Gets the amount Clear calls.
        /// </summary>
        [IgnoreDataMember]
        public long ClearCount => _clearCount;

        /// <summary>
        /// Gets the amount of various Draw calls.
        /// </summary>
        [IgnoreDataMember]
        public long DrawCount => _drawCount;

        /// <summary>
        /// Gets the amount of rendered primitives.
        /// </summary>
        [IgnoreDataMember]
        public long PrimitiveCount => _primitiveCount;

        /// <summary>
        /// Gets the amount of sprites and text characters rendered via <see cref="SpriteBatch"/>.
        /// </summary>
        [IgnoreDataMember]
        public long SpriteCount => _spriteCount;

        /// <summary>
        /// Gets the number of times a target was changed on the GPU.
        /// </summary>
        [IgnoreDataMember]
        public long TargetCount => _targetCount;

        /// <summary>
        /// Gets the number of times a texture was changed on the GPU.
        /// </summary>
        [IgnoreDataMember]
        public long TextureCount => _textureCount;

        /// <summary>
        /// Gets the number of times the vertex shader was changed on the GPU.
        /// </summary>
        [IgnoreDataMember]
        public long VertexShaderCount => _vertexShaderCount;

        /// <summary>
        /// Gets the number of times the pixel shader was changed on the GPU.
        /// </summary>
        [IgnoreDataMember]
        public long PixelShaderCount => _pixelShaderCount;

        /// <summary>
        /// Returns the difference between two sets of metrics.
        /// </summary>
        /// <param name="value1">Source <see cref="GraphicsMetrics"/> on the left of the sub sign.</param>
        /// <param name="value2">Source <see cref="GraphicsMetrics"/> on the right of the sub sign.</param>
        /// <returns>Difference between two sets of metrics.</returns>
        public static GraphicsMetrics operator -(GraphicsMetrics value1, GraphicsMetrics value2)
        {
            return new GraphicsMetrics()
            {
                _clearCount = value1._clearCount - value2._clearCount,
                _drawCount = value1._drawCount - value2._drawCount,
                _primitiveCount = value1._primitiveCount - value2._primitiveCount,
                _spriteCount = value1._spriteCount - value2._spriteCount,
                _targetCount = value1._targetCount - value2._targetCount,
                _textureCount = value1._textureCount - value2._textureCount,
                _pixelShaderCount = value1._pixelShaderCount - value2._pixelShaderCount,
                _vertexShaderCount = value1._vertexShaderCount - value2._vertexShaderCount
            };
        }

        /// <summary>
        /// Returns the combination of two sets of metrics.
        /// </summary>
        /// <param name="value1">Source <see cref="GraphicsMetrics"/> on the left of the add sign.</param>
        /// <param name="value2">Source <see cref="GraphicsMetrics"/> on the right of the add sign.</param>
        /// <returns>Combination of two sets of metrics.</returns>
        public static GraphicsMetrics operator +(GraphicsMetrics value1, GraphicsMetrics value2)
        {
            return new GraphicsMetrics()
            {
                _clearCount = value1._clearCount + value2._clearCount,
                _drawCount = value1._drawCount + value2._drawCount,
                _primitiveCount = value1._primitiveCount + value2._primitiveCount,
                _spriteCount = value1._spriteCount + value2._spriteCount,
                _targetCount = value1._targetCount + value2._targetCount,
                _textureCount = value1._textureCount + value2._textureCount,
                _pixelShaderCount = value1._pixelShaderCount + value2._pixelShaderCount,
                _vertexShaderCount = value1._vertexShaderCount + value2._vertexShaderCount
            };
        }
    }
}
