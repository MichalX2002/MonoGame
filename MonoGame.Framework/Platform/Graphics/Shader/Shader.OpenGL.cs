// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using MonoGame.Framework.Utilities;
using MonoGame.OpenGL;

namespace MonoGame.Framework.Graphics
{
    internal partial class Shader
    {
        private int? _shaderId;

        /// <summary>
        /// Saved for recompiling on context loss and debugging.
        /// </summary>
        private string GlslCode { get; set; }

        private static int PlatformProfile()
        {
            return 0;
        }

        private void PlatformConstruct(ShaderStage stage, byte[] shaderBytecode)
        {
            GlslCode = System.Text.Encoding.ASCII.GetString(shaderBytecode);
            HashKey = HashHelper.ComputeHash(shaderBytecode);
        }

        internal int GetShaderHandle()
        {
            // If the shader has already been created then return it.
            if (_shaderId.HasValue)
                return _shaderId.Value;

            var shaderType = Stage == ShaderStage.Vertex ? ShaderType.VertexShader : ShaderType.FragmentShader;
            _shaderId = GL.CreateShader(shaderType);
            GraphicsExtensions.CheckGLError();

            GL.ShaderSource(_shaderId.Value, GlslCode);
            GraphicsExtensions.CheckGLError();

            GL.CompileShader(_shaderId.Value);
            GraphicsExtensions.CheckGLError();

            GL.GetShader(_shaderId.Value, ShaderParameter.CompileStatus, out int compiled);
            GraphicsExtensions.CheckGLError();

            if (compiled == (int)Bool.True)
                return _shaderId.Value;

            var log = GL.GetShaderInfoLog(_shaderId.Value);
            Debug.WriteLine(log);

            GraphicsDevice.DisposeShader(_shaderId.Value);
            _shaderId = null;

            throw new InvalidOperationException("Shader compilation failed.");
        }

        internal void GetVertexAttributeLocations(int program)
        {
            for (int i = 0; i < Attributes.Length; ++i)
            {
                Attributes[i].Location = GL.GetAttribLocation(program, Attributes[i].Name);
                GraphicsExtensions.CheckGLError();
            }
        }

        internal int GetAttribLocation(VertexElementUsage usage, int index)
        {
            for (int i = 0; i < Attributes.Length; ++i)
            {
                if ((Attributes[i].Usage == usage) && (Attributes[i].Index == index))
                    return Attributes[i].Location;
            }
            return -1;
        }

        internal void ApplySamplerTextureUnits(int program)
        {
            // Assign the texture unit index to the sampler uniforms.
            foreach (var sampler in Samplers)
            {
                int loc = GL.GetUniformLocation(program, sampler.Name);
                GraphicsExtensions.CheckGLError();
                if (loc != -1)
                {
                    GL.Uniform1(loc, sampler.TextureSlot);
                    GraphicsExtensions.CheckGLError();
                }
            }
        }

        private void PlatformGraphicsDeviceResetting()
        {
            if (_shaderId.HasValue)
            {
                GraphicsDevice.DisposeShader(_shaderId.Value);
                _shaderId = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && _shaderId.HasValue)
            {
                GraphicsDevice.DisposeShader(_shaderId.Value);
                _shaderId = null;
            }

            base.Dispose(disposing);
        }
    }
}
