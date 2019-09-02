// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using SharpDX.Direct3D11;

namespace MonoGame.Framework.Graphics
{
    internal partial class Shader
    {
        private VertexShader _vertexShader;
        private PixelShader _pixelShader;

        // Caches the DirectX input layouts for this vertex shader.
        private InputLayoutCache _inputLayouts;

        internal byte[] Bytecode { get; private set; }

        internal InputLayoutCache InputLayouts => _inputLayouts;

        internal VertexShader VertexShader
        {
            get
            {
                if (_vertexShader == null)
                    CreateVertexShader();
                return _vertexShader;
            }
        }

        internal PixelShader PixelShader
        {
            get
            {
                if (_pixelShader == null)
                    CreatePixelShader();
                return _pixelShader;
            }
        }

        private static int PlatformProfile()
        {
            return 1;
        }

        private void PlatformConstruct(bool isVertexShader, byte[] shaderBytecode)
        {
            // We need the bytecode later for allocating the
            // input layout from the vertex declaration.
            Bytecode = shaderBytecode;

            HashKey = MonoGame.Utilities.Hash.ComputeHash(Bytecode);

            if (isVertexShader)
                CreateVertexShader();
            else
                CreatePixelShader();
        }

        private void PlatformGraphicsDeviceResetting()
        {
            SharpDX.Utilities.Dispose(ref _vertexShader);
            SharpDX.Utilities.Dispose(ref _pixelShader);
            SharpDX.Utilities.Dispose(ref _inputLayouts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SharpDX.Utilities.Dispose(ref _vertexShader);
                SharpDX.Utilities.Dispose(ref _pixelShader);
                SharpDX.Utilities.Dispose(ref _inputLayouts);
            }

            base.Dispose(disposing);
        }

        private void CreatePixelShader()
        {
            System.Diagnostics.Debug.Assert(Stage == ShaderStage.Pixel);
            _pixelShader = new PixelShader(GraphicsDevice._d3dDevice, Bytecode);
        }

        private void CreateVertexShader()
        {
            System.Diagnostics.Debug.Assert(Stage == ShaderStage.Vertex);
            _vertexShader = new VertexShader(GraphicsDevice._d3dDevice, Bytecode, null);
            _inputLayouts = new InputLayoutCache(GraphicsDevice, Bytecode);
        }
    }
}
