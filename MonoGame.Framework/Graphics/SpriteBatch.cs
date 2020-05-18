// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace MonoGame.Framework.Graphics
{
    /// <summary>
    /// Helper class for drawing text and sprites in optimized batches.
    /// </summary>
    public class SpriteBatch : GraphicsResource
    {
        #region Private Fields

        private SpriteBatcher _batcher;
        private SpriteEffect _spriteEffect;
        private EffectPass _spritePass;

        private bool _beginCalled;
        private SpriteSortMode _sortMode;
        private BlendState _blendState;
        private SamplerState _samplerState;
        private DepthStencilState _depthStencilState;
        private RasterizerState _rasterizerState;
        private Effect _effect;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs the <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="graphicsDevice">The <see cref="GraphicsDevice"/> which will be used for sprite rendering.</param>
        /// <exception cref="ArgumentNullException"><paramref name="graphicsDevice"/> is null.</exception>
        public SpriteBatch(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            _batcher = new SpriteBatcher(GraphicsDevice);
            _spriteEffect = new SpriteEffect(GraphicsDevice);
            _spritePass = _spriteEffect.CurrentTechnique.Passes[0];

            _beginCalled = false;
        }

        #endregion

        #region Begin

        /// <summary>
        /// Begins a new sprite and text batch with the specified render state.
        /// </summary>
        /// <param name="sortMode">The drawing order for sprite and text drawing. <see cref="SpriteSortMode.Deferred"/> by default.</param>
        /// <param name="blendState">State of the blending. Uses <see cref="BlendState.AlphaBlend"/> if null.</param>
        /// <param name="samplerState">State of the sampler. Uses <see cref="SamplerState.LinearClamp"/> if null.</param>
        /// <param name="depthStencilState">State of the depth-stencil buffer. Uses <see cref="DepthStencilState.None"/> if null.</param>
        /// <param name="rasterizerState">State of the rasterization. Uses <see cref="RasterizerState.CullCounterClockwise"/> if null.</param>
        /// <param name="effect">A custom <see cref="Effect"/> to override the default sprite effect. Uses default sprite effect if null.</param>
        /// <param name="transformMatrix">An optional matrix used to transform the sprite geometry. Uses <see cref="Matrix.Identity"/> if null.</param>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="Begin"/> is called next time without previous <see cref="End"/>.</exception>
        /// <remarks>
        /// <see cref="Begin"/> should be called before draw methods,
        /// and you cannot call it again before subsequent <see cref="End"/>.
        /// </remarks>
        public void Begin(
            SpriteSortMode sortMode = SpriteSortMode.Deferred,
            BlendState blendState = null,
            SamplerState samplerState = null,
            DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null,
            Effect effect = null,
            Matrix? transformMatrix = null)
        {
            if (_beginCalled)
                throw new InvalidOperationException(
                    "Begin cannot be called again until End has been successfully called.");

            // defaults
            _sortMode = sortMode;
            _blendState = blendState ?? BlendState.AlphaBlend;
            _samplerState = samplerState ?? SamplerState.LinearClamp;
            _depthStencilState = depthStencilState ?? DepthStencilState.None;
            _rasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;
            _effect = effect;
            _spriteEffect.TransformMatrix = transformMatrix;

            // Setup things now so a user can change them.
            if (sortMode == SpriteSortMode.Immediate)
                Setup();

            _beginCalled = true;
        }

        #endregion

        #region End

        /// <summary>
        /// Flushes all batched text and sprites to the screen.
        /// </summary>
        /// <remarks>
        /// This should be called after <see cref="Begin"/> and draw methods.
        /// </remarks>
        public void End()
        {
            if (!_beginCalled)
                throw new InvalidOperationException(
                    "Begin must be called before calling End.");

            _beginCalled = false;

            if (_sortMode != SpriteSortMode.Immediate)
                Setup();

            _batcher.DrawBatch(_sortMode, _effect);
        }

        #endregion

        /// <summary>
        /// Reuse a previously allocated <see cref="SpriteBatchItem"/> from the item pool. 
        /// The pool grows and initializes new items if none are available.
        /// </summary>
        public SpriteBatchItem GetBatchItem(Texture2D texture)
        {
            var item = _batcher.GetBatchItem();
            item.Texture = texture;
            return item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FlipTexCoords(ref Vector4 texCoord, SpriteEffects effects)
        {
            if ((effects & SpriteEffects.FlipVertically) != 0)
            {
                var tmp = texCoord.W;
                texCoord.W = texCoord.Y;
                texCoord.Y = tmp;
            }
            if ((effects & SpriteEffects.FlipHorizontally) != 0)
            {
                var tmp = texCoord.Z;
                texCoord.Z = texCoord.X;
                texCoord.X = tmp;
            }
        }

        public float GetSortKey(Texture2D texture, float depth)
        {
            // set SortKey based on SpriteSortMode.
            switch (_sortMode)
            {
                // Comparison of Texture objects.
                case SpriteSortMode.Texture:
                    return texture.SortingKey;

                // Comparison of Depth
                case SpriteSortMode.FrontToBack:
                    return depth;

                // Comparison of Depth in reverse
                case SpriteSortMode.BackToFront:
                    return -depth;

                default:
                    return depth;
            }
        }

        private void Setup()
        {
            var gd = GraphicsDevice;
            gd.BlendState = _blendState;
            gd.DepthStencilState = _depthStencilState;
            gd.RasterizerState = _rasterizerState;
            gd.SamplerStates[0] = _samplerState;

            _spritePass.Apply();
        }

        void AssertBeginCalled(string callerName)
        {
            if (!_beginCalled)
                throw new InvalidOperationException(
                    $"{nameof(Begin)} must be called successfully before you can call {callerName}.");
        }

        void AssertValidArguments(Texture2D texture, string callerName)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            AssertBeginCalled(callerName);
        }

        void AssertValidArguments(SpriteFont spriteFont, string callerName)
        {
            if (spriteFont == null)
                throw new ArgumentNullException(nameof(spriteFont));

            AssertBeginCalled(callerName);
        }

        public void Flush()
        {
            _batcher.DrawBatch(_sortMode, _effect);
        }

        /// <summary>
        /// Called at the end of a draw operation for <see cref="SpriteSortMode.Immediate"/>.
        /// </summary>
        public void FlushIfNeeded()
        {
            if (_sortMode == SpriteSortMode.Immediate)
                Flush();
        }

        public void Draw(
            Texture2D texture,
            in VertexPositionColorTexture vertexTL, in VertexPositionColorTexture vertexTR,
            in VertexPositionColorTexture vertexBL, in VertexPositionColorTexture vertexBR,
            float depth)
        {
            var item = GetBatchItem(texture);
            item.SortKey = GetSortKey(texture, depth);

            item.VertexTL = vertexTL;
            item.VertexTR = vertexTR;
            item.VertexBL = vertexBL;
            item.VertexBR = vertexBR;

            FlushIfNeeded();
        }

        public void Draw(
            Texture2D texture,
            in VertexPositionColorTexture vertexTL, in VertexPositionColorTexture vertexTR,
            in VertexPositionColorTexture vertexBL, in VertexPositionColorTexture vertexBR)
        {
            Draw(texture, vertexTL, vertexTR, vertexBL, vertexBR, vertexTL.Position.Z);
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The drawing location on screen or null if <paramref name="destinationRectangle"> is used.</paramref></param>
        /// <param name="destinationRectangle">The drawing bounds on screen or null if <paramref name="position"> is used.</paramref></param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="origin">An optional center of rotation. Uses <see cref="Vector2.Zero"/> if null.</param>
        /// <param name="rotation">An optional rotation of this sprite. 0 by default.</param>
        /// <param name="scale">An optional scale vector. Uses <see cref="Vector2.One"/> if null.</param>
        /// <param name="color">An optional color mask. Uses <see cref="Color.White"/> if null.</param>
        /// <param name="effects">The optional drawing modificators. <see cref="SpriteEffects.None"/> by default.</param>
        /// <param name="layerDepth">An optional depth of the layer of this sprite. 0 by default.</param>
        /// <exception cref="ArgumentException">
        /// Throwns if both <paramref name="position"/> and <paramref name="destinationRectangle"/> been used.
        /// </exception>
        /// <remarks>
        /// This overload uses optional parameters. 
        /// This overload requires only one of <paramref name="position"/> and 
        /// <paramref name="destinationRectangle"/> been used.
        /// </remarks>
        [Obsolete("In future versions this method can be removed.")]
        public void Draw(
            Texture2D texture,
            Vector2? position = null,
            RectangleF? destinationRectangle = null,
            RectangleF? sourceRectangle = null,
            Vector2? origin = null,
            float rotation = 0f,
            Vector2? scale = null,
            Color? color = null,
            SpriteEffects effects = SpriteEffects.None,
            float layerDepth = 0f)
        {
            // Assign default values to null parameters here, as they are not compile-time constants
            if (!color.HasValue)
                color = Color.White;

            if (!origin.HasValue)
                origin = Vector2.Zero;

            if (!scale.HasValue)
                scale = Vector2.One;

            // If both drawRectangle and position are null,
            // or if both have been assigned a value, raise an error
            if (destinationRectangle.HasValue == position.HasValue)
                throw new ArgumentException(
                    "Expected drawRectangle or position, but received neither or both.");

            if (position.HasValue) // Call Draw() using position
            {
                Draw(
                    texture, position.Value, sourceRectangle, color.Value,
                    rotation, origin.Value, scale.Value, effects, layerDepth);
            }
            else // Call Draw() using drawRectangle
            {
                Draw(
                    texture, destinationRectangle.Value, sourceRectangle, color.Value,
                    rotation, origin.Value, effects, layerDepth);
            }
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="sourceRectangle">
        /// An optional region on the texture which will be rendered.
        /// If null; draws full texture.
        /// </param>
        /// <param name="color">A color mask.</param>
        /// <param name="rotation">A rotation of this sprite.</param>
        /// <param name="origin">Center of the rotation. 0,0 by default.</param>
        /// <param name="scale">A scaling of this sprite.</param>
        /// <param name="effects">Modificators for drawing. Can be combined.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(
            Texture2D texture, Vector2 position, in RectangleF? sourceRectangle, Color color,
            float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            AssertValidArguments(texture, nameof(Draw));

            var item = GetBatchItem(texture);
            item.SortKey = GetSortKey(texture, layerDepth);

            origin *= scale;

            Vector4 texCoord;
            float w, h;
            if (sourceRectangle.HasValue)
            {
                RectangleF srcRect = sourceRectangle.Value;
                w = srcRect.Width * scale.X;
                h = srcRect.Height * scale.Y;
                texCoord.Base.X = srcRect.X * texture.Texel.X;
                texCoord.Base.Y = srcRect.Y * texture.Texel.Y;
                texCoord.Base.Z = (srcRect.X + srcRect.Width) * texture.Texel.X;
                texCoord.Base.W = (srcRect.Y + srcRect.Height) * texture.Texel.Y;
            }
            else
            {
                w = texture.Width * scale.X;
                h = texture.Height * scale.Y;
                texCoord = new Vector4(0, 0, 1, 1);
            }

            FlipTexCoords(ref texCoord, effects);

            if (rotation == 0f)
            {
                item.Set(
                    position.X - origin.X, position.Y - origin.Y, w, h,
                    color, texCoord, layerDepth);
            }
            else
            {
                item.Set(
                    position.X, position.Y, -origin.X, -origin.Y, w, h,
                    MathF.Sin(rotation), MathF.Cos(rotation),
                    color, texCoord, layerDepth);
            }

            FlushIfNeeded();
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="sourceRectangle">
        /// An optional region on the texture which will be rendered, drawing full texture otherwise.
        /// </param>
        /// <param name="color">A color mask.</param>
        /// <param name="rotation">A rotation of this sprite.</param>
        /// <param name="origin">Center of the rotation. 0,0 by default.</param>
        /// <param name="scale">A scaling of this sprite.</param>
        /// <param name="effects">Modificators for drawing. Can be combined.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(
            Texture2D texture,
            Vector2 position,
            in RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            SpriteEffects effects,
            float layerDepth)
        {
            Draw(
                texture, position, sourceRectangle, color,
                rotation, origin, new Vector2(scale, scale), effects, layerDepth);
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>
        /// <param name="sourceRectangle">
        /// An optional region on the texture which will be rendered, drawing full texture otherwise.
        /// </param>
        /// <param name="color">A color mask.</param>
        /// <param name="rotation">A rotation of this sprite.</param>
        /// <param name="origin">Center of the rotation. 0,0 by default.</param>
        /// <param name="effects">Modificators for drawing. Can be combined.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(
            Texture2D texture,
            in RectangleF destinationRectangle,
            in RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            SpriteEffects effects,
            float layerDepth)
        {
            AssertValidArguments(texture, nameof(Draw));

            var item = GetBatchItem(texture);
            item.SortKey = GetSortKey(texture, layerDepth);

            float originX, originY;
            Vector4 texCoord;
            if (sourceRectangle.HasValue)
            {
                RectangleF srcRect = sourceRectangle.Value;
                texCoord = new Vector4(
                    srcRect.X * texture.Texel.X,
                    srcRect.Y * texture.Texel.Y,
                    (srcRect.X + srcRect.Width) * texture.Texel.X,
                    (srcRect.Y + srcRect.Height) * texture.Texel.Y);

                originX = srcRect.Width != 0
                    ? origin.X * destinationRectangle.Width / srcRect.Width
                    : origin.X * destinationRectangle.Width * texture.Texel.X;

                originY = srcRect.Height != 0
                    ? origin.Y * destinationRectangle.Height / srcRect.Height
                    : origin.Y * destinationRectangle.Height * texture.Texel.Y;
            }
            else
            {
                texCoord = new Vector4(0, 0, 1, 1);

                originX = origin.X * destinationRectangle.Width * texture.Texel.X;
                originY = origin.Y * destinationRectangle.Height * texture.Texel.Y;
            }

            FlipTexCoords(ref texCoord, effects);

            if (rotation == 0f)
            {
                item.Set(
                    destinationRectangle.X - originX, destinationRectangle.Y - originY,
                    destinationRectangle.Width, destinationRectangle.Height,
                    color, texCoord, layerDepth);
            }
            else
            {
                item.Set(
                    destinationRectangle.X, destinationRectangle.Y, -originX, -originY,
                    destinationRectangle.Width, destinationRectangle.Height,
                    MathF.Sin(rotation), MathF.Cos(rotation),
                    color, texCoord, layerDepth);
            }

            FlushIfNeeded();
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">A color mask.</param>
        public void Draw(Texture2D texture, Vector2 position, in RectangleF? sourceRectangle, Color color)
        {
            AssertValidArguments(texture, nameof(Draw));

            var item = GetBatchItem(texture);
            item.SortKey = GetSortKey(texture, 0);

            Vector4 texCoord;
            float w;
            float h;
            if (sourceRectangle.HasValue)
            {
                RectangleF srcRect = sourceRectangle.Value;
                w = srcRect.Width;
                h = srcRect.Height;
                texCoord = new Vector4(
                    srcRect.X * texture.Texel.X,
                    srcRect.Y * texture.Texel.Y,
                    (srcRect.X + srcRect.Width) * texture.Texel.X,
                    (srcRect.Y + srcRect.Height) * texture.Texel.Y);
            }
            else
            {
                w = texture.Width;
                h = texture.Height;
                texCoord = new Vector4(0, 0, 1, 1);
            }

            item.Set(position.X, position.Y, w, h, color, texCoord, 0);
            FlushIfNeeded();
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">A color mask.</param>
        public void Draw(Texture2D texture, in RectangleF destinationRectangle, in RectangleF? sourceRectangle, Color color)
        {
            AssertValidArguments(texture, nameof(Draw));

            var item = GetBatchItem(texture);
            item.SortKey = GetSortKey(texture, 0);

            Vector4 texCoord;
            if (sourceRectangle.HasValue)
            {
                RectangleF srcRect = sourceRectangle.Value;
                texCoord = new Vector4(
                    srcRect.X * texture.Texel.X,
                    srcRect.Y * texture.Texel.Y,
                    (srcRect.X + srcRect.Width) * texture.Texel.X,
                    (srcRect.Y + srcRect.Height) * texture.Texel.Y);
            }
            else
            {
                texCoord = new Vector4(0, 0, 1, 1);
            }

            item.Set(
                destinationRectangle.X, destinationRectangle.Y,
                destinationRectangle.Width, destinationRectangle.Height,
                color, texCoord, 0);

            FlushIfNeeded();
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="color">A color mask.</param>
        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            AssertValidArguments(texture, nameof(Draw));

            var item = GetBatchItem(texture);
            item.SortKey = GetSortKey(texture, 0);

            item.Set(
                position.X, position.Y,
                texture.Width, texture.Height,
                color, new Vector4(0, 0, 1, 1), 0);

            FlushIfNeeded();
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>
        /// <param name="color">A color mask.</param>
        public void Draw(Texture2D texture, in RectangleF destinationRectangle, Color color)
        {
            AssertValidArguments(texture, nameof(Draw));

            var item = GetBatchItem(texture);
            item.SortKey = GetSortKey(texture, 0);

            item.Set(
                destinationRectangle.X, destinationRectangle.Y,
                destinationRectangle.Width, destinationRectangle.Height,
                color, new Vector4(0, 0, 1, 1), 0);

            FlushIfNeeded();
        }

        /// <summary>
        /// Submit text for drawing in the current batch.
        /// </summary>
        /// <param name="spriteFont">A font.</param>
        /// <param name="text">The text which will be drawn.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="color">A color mask.</param>
        public unsafe void DrawString(SpriteFont spriteFont, RuneEnumerator text, Vector2 position, Color color)
        {
            AssertValidArguments(spriteFont, nameof(DrawString));
            float sortKey = GetSortKey(spriteFont.Texture, 0);

            var offset = Vector2.Zero;
            bool firstGlyphOfLine = true;
            var glyphs = spriteFont.Glyphs;

            foreach (Rune c in text)
            {
                if (c == (Rune)'\r')
                    continue;

                if (c == (Rune)'\n')
                {
                    offset.X = 0;
                    offset.Y += spriteFont.LineSpacing;
                    firstGlyphOfLine = true;
                    continue;
                }

                int glyphIndex = spriteFont.GetGlyphIndexOrDefault(c);
                ref readonly SpriteFont.Glyph glyph = ref glyphs[glyphIndex];

                // The first character on a line might have a negative left side bearing.
                // In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
                // so that text does not hang off the left side of its rectangle.
                if (firstGlyphOfLine)
                {
                    offset.X = Math.Max(glyph.LeftSideBearing, 0);
                    firstGlyphOfLine = false;
                }
                else
                {
                    offset.X += spriteFont.Spacing + glyph.LeftSideBearing;
                }

                var p = offset;
                p.X += glyph.Cropping.X;
                p.Y += glyph.Cropping.Y;
                p += position;

                var item = GetBatchItem(spriteFont.Texture);
                item.SortKey = sortKey;

                var texCoord = new Vector4(
                    glyph.BoundsInTexture.X * spriteFont.Texture.Texel.X,
                    glyph.BoundsInTexture.Y * spriteFont.Texture.Texel.Y,
                    (glyph.BoundsInTexture.X + glyph.BoundsInTexture.Width) * spriteFont.Texture.Texel.X,
                    (glyph.BoundsInTexture.Y + glyph.BoundsInTexture.Height) * spriteFont.Texture.Texel.Y);

                item.Set(
                    p.X, p.Y, glyph.BoundsInTexture.Width, glyph.BoundsInTexture.Height,
                    color, texCoord, 0);

                offset.X += glyph.Width + glyph.RightSideBearing;
            }

            // We need to flush if we're using Immediate sort mode.
            FlushIfNeeded();
        }

        /// <summary>
        /// Submit text for drawing in the current batch.
        /// </summary>
        /// <param name="spriteFont">A font.</param>
        /// <param name="text">The text which will be drawn.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="color">A color mask.</param>
        /// <param name="rotation">A rotation of this string.</param>
        /// <param name="origin">Center of the rotation. 0,0 by default.</param>
        /// <param name="scale">A scaling of this string.</param>
        /// <param name="effects">Modificators for drawing. Can be combined.</param>
        /// <param name="layerDepth">A depth of the layer of this string.</param>
        public void DrawString(
            SpriteFont spriteFont, string text, Vector2 position, Color color,
            float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            DrawString(spriteFont, text, position, color, rotation, origin, new Vector2(scale), effects, layerDepth);
        }

        /// <summary>
        /// Submit text for drawing in the current batch.
        /// </summary>
        /// <param name="spriteFont">A font.</param>
        /// <param name="text">The text which will be drawn.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="color">A color mask.</param>
        /// <param name="rotation">A rotation of this string.</param>
        /// <param name="origin">Center of the rotation. 0,0 by default.</param>
        /// <param name="scale">A scaling of this string.</param>
        /// <param name="effects">Modificators for drawing. Can be combined.</param>
        /// <param name="layerDepth">A depth of the layer of this string.</param>
        public unsafe void DrawString(
            SpriteFont spriteFont, RuneEnumerator text, Vector2 position, Color color,
            float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            AssertValidArguments(spriteFont, nameof(DrawString));
            float sortKey = GetSortKey(spriteFont.Texture, layerDepth);

            var flipAdjustment = Vector2.Zero;
            var flippedVert = (effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
            var flippedHorz = (effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;

            if (flippedVert || flippedHorz)
            {
                Vector2 size = spriteFont.MeasureString(text);
                if (flippedHorz)
                {
                    origin.X *= -1;
                    flipAdjustment.X = -size.X;
                }
                if (flippedVert)
                {
                    origin.Y *= -1;
                    flipAdjustment.Y = spriteFont.LineSpacing - size.Y;
                }
            }

            var transformation = Matrix.Identity;
            float cos = 0, sin = 0;
            if (rotation == 0)
            {
                transformation.M11 = flippedHorz ? -scale.X : scale.X;
                transformation.M22 = flippedVert ? -scale.Y : scale.Y;
                transformation.M41 = ((flipAdjustment.X - origin.X) * transformation.M11) + position.X;
                transformation.M42 = ((flipAdjustment.Y - origin.Y) * transformation.M22) + position.Y;
            }
            else
            {
                cos = MathF.Cos(rotation);
                sin = MathF.Sin(rotation);
                transformation.M11 = (flippedHorz ? -scale.X : scale.X) * cos;
                transformation.M12 = (flippedHorz ? -scale.X : scale.X) * sin;
                transformation.M21 = (flippedVert ? -scale.Y : scale.Y) * (-sin);
                transformation.M22 = (flippedVert ? -scale.Y : scale.Y) * cos;
                transformation.M41 = ((flipAdjustment.X - origin.X) * transformation.M11) + (flipAdjustment.Y - origin.Y) * transformation.M21 + position.X;
                transformation.M42 = ((flipAdjustment.X - origin.X) * transformation.M12) + (flipAdjustment.Y - origin.Y) * transformation.M22 + position.Y;
            }

            var offset = Vector2.Zero;
            var firstGlyphOfLine = true;
            var glyphs = spriteFont.Glyphs;

            foreach (Rune c in text)
            {
                if (c == (Rune)'\r')
                    continue;

                if (c == (Rune)'\n')
                {
                    offset.X = 0;
                    offset.Y += spriteFont.LineSpacing;
                    firstGlyphOfLine = true;
                    continue;
                }

                int glyphIndex = spriteFont.GetGlyphIndexOrDefault(c);
                ref readonly SpriteFont.Glyph glyph = ref glyphs[glyphIndex];

                // The first character on a line might have a negative left side bearing.
                // In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
                //  so that text does not hang off the left side of its rectangle.
                if (firstGlyphOfLine)
                {
                    offset.X = Math.Max(glyph.LeftSideBearing, 0);
                    firstGlyphOfLine = false;
                }
                else
                {
                    offset.X += spriteFont.Spacing + glyph.LeftSideBearing;
                }

                Vector2 pos = offset;

                if (flippedHorz)
                    pos.X += glyph.BoundsInTexture.Width;
                pos.X += glyph.Cropping.X;

                if (flippedVert)
                    pos.Y += glyph.BoundsInTexture.Height - spriteFont.LineSpacing;
                pos.Y += glyph.Cropping.Y;

                pos = Vector2.Transform(pos, transformation);

                var item = GetBatchItem(spriteFont.Texture);
                item.SortKey = sortKey;

                var texCoord = new Vector4(
                    glyph.BoundsInTexture.X * spriteFont.Texture.Texel.X,
                    glyph.BoundsInTexture.Y * spriteFont.Texture.Texel.Y,
                    (glyph.BoundsInTexture.X + glyph.BoundsInTexture.Width) * spriteFont.Texture.Texel.X,
                    (glyph.BoundsInTexture.Y + glyph.BoundsInTexture.Height) * spriteFont.Texture.Texel.Y);

                FlipTexCoords(ref texCoord, effects);

                if (rotation == 0f)
                {
                    item.Set(
                        pos.X, pos.Y,
                        glyph.BoundsInTexture.Width * scale.X,
                        glyph.BoundsInTexture.Height * scale.Y,
                        color,
                        texCoord,
                        layerDepth);
                }
                else
                {
                    item.Set(
                        pos.X, pos.Y, 0, 0,
                        glyph.BoundsInTexture.Width * scale.X,
                        glyph.BoundsInTexture.Height * scale.Y,
                        sin, cos,
                        color,
                        texCoord,
                        layerDepth);
                }

                offset.X += glyph.Width + glyph.RightSideBearing;
            }

            // We need to flush if we're using Immediate sort mode.
            FlushIfNeeded();
        }

        /// <summary>
        /// Immediately releases the unmanaged resources used by this object.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources;
        /// <see langword="false"/> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                _spritePass = null;

                if (disposing)
                {
                    _spriteEffect?.Dispose();
                    _spriteEffect = null;
                }

                _batcher?.Dispose();
                _batcher = null;
            }
            base.Dispose(disposing);
        }
    }
}