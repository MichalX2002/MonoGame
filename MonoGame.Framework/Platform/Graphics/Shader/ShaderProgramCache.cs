using System;
using System.Collections.Generic;
using System.Diagnostics;
using MonoGame.OpenGL;

namespace MonoGame.Framework.Graphics
{
    internal class ShaderProgram
    {
        private readonly Dictionary<string, int> _uniformLocations = new Dictionary<string, int>();

        public GLHandle Program { get; }

        public ShaderProgram(GLHandle program)
        {
            Program = program;
        }

        public int GetUniformLocation(string name)
        {
            if (_uniformLocations.TryGetValue(name, out int value))
                return value;

            var location = GL.GetUniformLocation(Program, name);
            GL.CheckError();

            _uniformLocations[name] = location;
            return location;
        }
    }

    /// <summary>
    /// This class is used to Cache the links between Vertex/Pixel Shaders and Constant Buffers.
    /// It will be responsible for linking the programs under OpenGL if they have not been linked
    /// before. If an existing link exists it will be resused.
    /// </summary>
    internal class ShaderProgramCache : IDisposable
    {
        private Dictionary<int, ShaderProgram> _programCache =
            new Dictionary<int, ShaderProgram>();

        private GraphicsDevice _graphicsDevice;
        private bool disposed;

        public ShaderProgramCache(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        ~ShaderProgramCache()
        {
            Dispose(false);
        }

        /// <summary>
        /// Clear the program cache releasing all shader programs.
        /// </summary>
        public void Clear()
        {
            foreach (var pair in _programCache)
            {
                _graphicsDevice.DisposeResource(pair.Value.Program);
            }
            _programCache.Clear();
        }

        public ShaderProgram GetProgram(Shader vertexShader, Shader pixelShader)
        {
            // TODO: We should be hashing in the mix of constant 
            // buffers here as well.  This would allow us to optimize
            // setting uniforms to only when a constant buffer changes.

            int key = vertexShader.HashKey | pixelShader.HashKey;
            if (!_programCache.ContainsKey(key))
            {
                // the key does not exist so we need to link the programs
                _programCache.Add(key, Link(vertexShader, pixelShader));
            }

            return _programCache[key];
        }

        private ShaderProgram Link(Shader vertexShader, Shader pixelShader)
        {
            // NOTE: No need to worry about background threads here
            // as this is only called at draw time when we're in the
            // main drawing thread.
            var program = GL.CreateProgram();
            GL.CheckError();

            GL.AttachShader(program, vertexShader.GetShaderHandle());
            GL.CheckError();

            GL.AttachShader(program, pixelShader.GetShaderHandle());
            GL.CheckError();

            //vertexShader.BindVertexAttributes(program);

            GL.LinkProgram(program);
            GL.CheckError();

            GL.UseProgram(program);
            GL.CheckError();

            vertexShader.GetVertexAttributeLocations(program);

            pixelShader.ApplySamplerTextureUnits(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int linked);
            GL.LogError("VertexShaderCache.Link(), GL.GetProgram");

            var programHandle = GLHandle.Program(program);

            if (linked == (int)Bool.False)
            {
                var log = GL.GetProgramInfoLog(program);
                Debug.WriteLine(log);

                GL.DetachShader(program, vertexShader.GetShaderHandle());
                GL.DetachShader(program, pixelShader.GetShaderHandle());

                programHandle.Free();
                throw new InvalidOperationException("Unable to link effect program");
            }

            return new ShaderProgram(programHandle);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Clear();
                disposed = true;
            }
        }
    }
}