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
        private GLHandle _handle;

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
            if (!_handle.IsNull)
                return _handle;

            var shaderType = Stage == ShaderStage.Vertex ? ShaderType.VertexShader : ShaderType.FragmentShader;
            var shader = GL.CreateShader(shaderType);
            GL.CheckError();

            GL.ShaderSource(shader, GlslCode);
            GL.CheckError();

            GL.CompileShader(shader);
            GL.CheckError();

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int compiled);
            GL.CheckError();

            _handle = GLHandle.Shader(shader);
            if (compiled == (int)Bool.True)
                return _handle;

            var log = GL.GetShaderInfoLog(shader);
            Debug.WriteLine(log);

            _handle.Free();
            throw new InvalidOperationException("Shader compilation failed.");
        }

        internal void GetVertexAttributeLocations(int program)
        {
            for (int i = 0; i < Attributes.Length; ++i)
            {
                Attributes[i].Location = GL.GetAttribLocation(program, Attributes[i].Name);
                GL.CheckError();
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
                GL.CheckError();
                if (loc != -1)
                {
                    GL.Uniform1(loc, sampler.TextureSlot);
                    GL.CheckError();
                }
            }
        }

        private void PlatformGraphicsDeviceResetting()
        {
            if (!_handle.IsNull)
            {
                GraphicsDevice.DisposeResource(_handle);
                _handle = default;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && _handle.IsNull)
            {
                GraphicsDevice.DisposeResource(_handle);
                _handle = default;
            }

            base.Dispose(disposing);
        }
    }
}
