// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Security;
using MonoGame.Framework.Graphics;

#if __IOS__ || __TVOS__ || MONOMAC
using ObjCRuntime;
#endif

namespace MonoGame.OpenGL
{
    internal partial class ColorFormat
    {
        internal ColorFormat(int r, int g, int b, int a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        internal int R { get; private set; }
        internal int G { get; private set; }
        internal int B { get; private set; }
        internal int A { get; private set; }
    }

    internal partial class GL
    {
        internal enum RenderApi
        {
            ES = 12448,
            GL = 12450,
        }

        internal static RenderApi BoundApi = RenderApi.GL;

        private const CallingConvention callingConvention = CallingConvention.Winapi;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void EnableVertexAttribArrayDelegate(int attrib);
        internal static EnableVertexAttribArrayDelegate EnableVertexAttribArray;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DisableVertexAttribArrayDelegate(int attrib);
        internal static DisableVertexAttribArrayDelegate DisableVertexAttribArray;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void MakeCurrentDelegate(IntPtr window);
        internal static MakeCurrentDelegate MakeCurrent;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal unsafe delegate void GetIntegerDelegate(int param, [Out] out int data);
        internal static GetIntegerDelegate GetIntegerv;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate IntPtr GetStringDelegate(StringName param);
        internal static GetStringDelegate GetStringInternal;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void ClearDepthDelegate(float depth);
        internal static ClearDepthDelegate ClearDepth;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DepthRangedDelegate(double min, double max);
        internal static DepthRangedDelegate DepthRanged;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DepthRangefDelegate(float min, float max);
        internal static DepthRangefDelegate DepthRangef;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void ClearDelegate(ClearBufferMask mask);
        internal static ClearDelegate Clear;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void ClearColorDelegate(float red, float green, float blue, float alpha);
        internal static ClearColorDelegate ClearColor;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void ClearStencilDelegate(int stencil);
        internal static ClearStencilDelegate ClearStencil;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void ViewportDelegate(int x, int y, int w, int h);
        internal static ViewportDelegate Viewport;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate ErrorCode GetErrorDelegate();
        internal static GetErrorDelegate GetError;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void FlushDelegate();
        internal static FlushDelegate Flush;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void GenTexturesDelegte(int count, [Out] out int id);
        internal static GenTexturesDelegte GenTextures;

        internal static GLHandle GenTexture()
        {
            GenTextures(1, out int id);
            return GLHandle.Texture(id);
        }

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void BindTextureDelegate(TextureTarget target, int id);
        internal static BindTextureDelegate BindTexture;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate int EnableDelegate(EnableCap cap);
        internal static EnableDelegate Enable;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate int DisableDelegate(EnableCap cap);
        internal static DisableDelegate Disable;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void CullFaceDelegate(CullFaceMode mode);
        internal static CullFaceDelegate CullFace;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void FrontFaceDelegate(FrontFaceDirection direction);
        internal static FrontFaceDelegate FrontFace;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void PolygonModeDelegate(MaterialFace face, PolygonMode mode);
        internal static PolygonModeDelegate PolygonMode;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void PolygonOffsetDelegate(float slopeScaleDepthBias, float depthbias);
        internal static PolygonOffsetDelegate PolygonOffset;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DrawBuffersDelegate(int count, DrawBuffersEnum[] buffers);
        internal static DrawBuffersDelegate DrawBuffers;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void UseProgramDelegate(int program);
        internal static UseProgramDelegate UseProgram;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal unsafe delegate void Uniform4fvDelegate(int location, int size, [In] in float values);
        internal static Uniform4fvDelegate Uniform4fv;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void Uniform1iDelegate(int location, int value);
        internal static Uniform1iDelegate Uniform1i;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void Uniform1fDelegate(int location, float value);
        internal static Uniform1fDelegate Uniform1f;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void ScissorDelegate(int x, int y, int width, int height);
        internal static ScissorDelegate Scissor;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void ReadPixelsDelegate(
            int x, int y, int width, int height, PixelFormat format, PixelType type, IntPtr data);
        internal static ReadPixelsDelegate ReadPixelsInternal;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void BindBufferDelegate(BufferTarget target, int buffer);
        internal static BindBufferDelegate BindBuffer;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DrawElementsDelegate(
            GLPrimitiveType primitiveType, int count, IndexElementType elementType, IntPtr offset);
        internal static DrawElementsDelegate DrawElements;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DrawArraysDelegate(GLPrimitiveType primitiveType, int offset, int count);
        internal static DrawArraysDelegate DrawArrays;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void GenRenderbuffersDelegate(int count, [Out] out int buffer);
        internal static GenRenderbuffersDelegate? GenRenderbuffers;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void BindRenderbufferDelegate(RenderbufferTarget target, int buffer);
        internal static BindRenderbufferDelegate? BindRenderbuffer;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DeleteRenderbuffersDelegate(int count, [In] in int buffer);
        internal static DeleteRenderbuffersDelegate? DeleteRenderbuffers;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void RenderbufferStorageMultisampleDelegate(RenderbufferTarget target, int sampleCount,
            RenderbufferStorage storage, int width, int height);
        internal static RenderbufferStorageMultisampleDelegate? RenderbufferStorageMultisample;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void GenFramebuffersDelegate(int count, out int buffer);
        internal static GenFramebuffersDelegate? GenFramebuffers;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void BindFramebufferDelegate(FramebufferTarget target, int buffer);
        internal static BindFramebufferDelegate? BindFramebuffer;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DeleteFramebuffersDelegate(int count, [In] in int buffer);
        internal static DeleteFramebuffersDelegate? DeleteFramebuffers;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void InvalidateFramebufferDelegate(
            FramebufferTarget target, int numAttachments, FramebufferAttachment[] attachments);
        public static InvalidateFramebufferDelegate? InvalidateFramebuffer;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void FramebufferTexture2DDelegate(
            FramebufferTarget target, FramebufferAttachment attachement, TextureTarget textureTarget, int texture, int level);
        internal static FramebufferTexture2DDelegate? FramebufferTexture2D;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void FramebufferTexture2DMultiSampleDelegate(
            FramebufferTarget target, FramebufferAttachment attachement,
            TextureTarget textureTarget, int texture, int level, int samples);
        internal static FramebufferTexture2DMultiSampleDelegate? FramebufferTexture2DMultiSample;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void FramebufferRenderbufferDelegate(FramebufferTarget target, FramebufferAttachment attachement,
            RenderbufferTarget renderBufferTarget, int buffer);
        internal static FramebufferRenderbufferDelegate? FramebufferRenderbuffer;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void RenderbufferStorageDelegate(
            RenderbufferTarget target, RenderbufferStorage storage, int width, int hegiht);
        public static RenderbufferStorageDelegate? RenderbufferStorage;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void GenerateMipmapDelegate(GenerateMipmapTarget target);
        internal static GenerateMipmapDelegate? GenerateMipmap;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void ReadBufferDelegate(ReadBufferMode buffer);
        internal static ReadBufferDelegate? ReadBuffer;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DrawBufferDelegate(DrawBufferMode buffer);
        internal static DrawBufferDelegate? DrawBuffer;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void BlitFramebufferDelegate(int srcX0,
            int srcY0,
            int srcX1,
            int srcY1,
            int dstX0,
            int dstY0,
            int dstX1,
            int dstY1,
            ClearBufferMask mask,
            BlitFramebufferFilter filter);
        internal static BlitFramebufferDelegate BlitFramebuffer;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate FramebufferErrorCode CheckFramebufferStatusDelegate(FramebufferTarget target);
        internal static CheckFramebufferStatusDelegate CheckFramebufferStatus;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void TexParameterFloatDelegate(TextureTarget target, TextureParameterName name, float value);
        internal static TexParameterFloatDelegate TexParameterf;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal unsafe delegate void TexParameterFloatArrayDelegate(
            TextureTarget target, TextureParameterName name, [In] in float values);
        internal static TexParameterFloatArrayDelegate TexParameterfv;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void TexParameterIntDelegate(TextureTarget target, TextureParameterName name, int value);
        internal static TexParameterIntDelegate TexParameteri;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void GenQueriesDelegate(int count, [Out] out int queryId);
        internal static GenQueriesDelegate GenQueries;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void BeginQueryDelegate(QueryTarget target, int queryId);
        internal static BeginQueryDelegate BeginQuery;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void EndQueryDelegate(QueryTarget target);
        internal static EndQueryDelegate EndQuery;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void GetQueryObjectDelegate(int queryId, GetQueryObjectParam getparam, [Out] out int ready);
        internal static GetQueryObjectDelegate GetQueryObject;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DeleteQueriesDelegate(int count, [In] in int queryId);
        internal static DeleteQueriesDelegate DeleteQueries;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void ActiveTextureDelegate(TextureUnit textureUnit);
        internal static ActiveTextureDelegate ActiveTexture;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate int CreateShaderDelegate(ShaderType type);
        internal static CreateShaderDelegate CreateShader;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal unsafe delegate void ShaderSourceDelegate(int shaderId, int count, IntPtr code, int length);
        internal static ShaderSourceDelegate ShaderSourceInternal;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void CompileShaderDelegate(int shaderId);
        internal static CompileShaderDelegate CompileShader;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal unsafe delegate void GetShaderDelegate(int shaderId, int parameter, out int value);
        internal static GetShaderDelegate GetShaderiv;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void GetShaderInfoLogDelegate(int shader, int bufSize, IntPtr length, StringBuilder infoLog);
        internal static GetShaderInfoLogDelegate GetShaderInfoLogInternal;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate bool IsShaderDelegate(int shaderId);
        internal static IsShaderDelegate IsShader;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DeleteShaderDelegate(int shaderId);
        internal static DeleteShaderDelegate DeleteShader;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate int GetAttribLocationDelegate(int programId, string name);
        internal static GetAttribLocationDelegate GetAttribLocation;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate int GetUniformLocationDelegate(int programId, string name);
        internal static GetUniformLocationDelegate GetUniformLocation;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate bool IsProgramDelegate(int programId);
        internal static IsProgramDelegate IsProgram;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DeleteProgramDelegate(int programId);
        internal static DeleteProgramDelegate DeleteProgram;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate int CreateProgramDelegate();
        internal static CreateProgramDelegate CreateProgram;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void AttachShaderDelegate(int programId, int shaderId);
        internal static AttachShaderDelegate AttachShader;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void LinkProgramDelegate(int programId);
        internal static LinkProgramDelegate LinkProgram;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal unsafe delegate void GetProgramDelegate(int programId, int name, out int linked);
        internal static GetProgramDelegate GetProgramiv;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void GetProgramInfoLogDelegate(int program, int bufSize, IntPtr length, StringBuilder infoLog);
        internal static GetProgramInfoLogDelegate GetProgramInfoLogInternal;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DetachShaderDelegate(int programId, int shaderId);
        internal static DetachShaderDelegate DetachShader;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void BlendColorDelegate(float r, float g, float b, float a);
        internal static BlendColorDelegate BlendColor;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void BlendEquationSeparateDelegate(BlendEquationMode colorMode, BlendEquationMode alphaMode);
        internal static BlendEquationSeparateDelegate BlendEquationSeparate;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void BlendEquationSeparateiDelegate(int buffer, BlendEquationMode colorMode, BlendEquationMode alphaMode);
        internal static BlendEquationSeparateiDelegate BlendEquationSeparatei;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void BlendFuncSeparateDelegate(BlendingFactorSrc colorSrc, BlendingFactorDest colorDst,
            BlendingFactorSrc alphaSrc, BlendingFactorDest alphaDst);
        internal static BlendFuncSeparateDelegate BlendFuncSeparate;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void BlendFuncSeparateiDelegate(int buffer, BlendingFactorSrc colorSrc, BlendingFactorDest colorDst,
            BlendingFactorSrc alphaSrc, BlendingFactorDest alphaDst);
        internal static BlendFuncSeparateiDelegate BlendFuncSeparatei;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void ColorMaskDelegate(bool r, bool g, bool b, bool a);
        internal static ColorMaskDelegate ColorMask;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DepthFuncDelegate(DepthFunction function);
        internal static DepthFuncDelegate DepthFunc;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DepthMaskDelegate(bool enabled);
        internal static DepthMaskDelegate DepthMask;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void StencilFuncSeparateDelegate(StencilFace face, GLStencilFunction function, int referenceStencil, int mask);
        internal static StencilFuncSeparateDelegate StencilFuncSeparate;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void StencilOpSeparateDelegate(StencilFace face, StencilOp stencilfail, StencilOp depthFail, StencilOp pass);
        internal static StencilOpSeparateDelegate StencilOpSeparate;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void StencilFuncDelegate(GLStencilFunction function, int referenceStencil, int mask);
        internal static StencilFuncDelegate StencilFunc;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void StencilOpDelegate(StencilOp stencilfail, StencilOp depthFail, StencilOp pass);
        internal static StencilOpDelegate StencilOp;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void StencilMaskDelegate(int mask);
        internal static StencilMaskDelegate StencilMask;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void CompressedTexImage2DDelegate(TextureTarget target, int level, PixelInternalFormat internalFormat,
            int width, int height, int border, int size, IntPtr data);
        internal static CompressedTexImage2DDelegate CompressedTexImage2D;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void TexImage2DDelegate(TextureTarget target, int level, PixelInternalFormat internalFormat,
            int width, int height, int border, PixelFormat format, PixelType pixelType, IntPtr data);
        internal static TexImage2DDelegate TexImage2D;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void CompressedTexSubImage2DDelegate(TextureTarget target, int level,
            int x, int y, int width, int height, PixelInternalFormat format, int size, IntPtr data);
        internal static CompressedTexSubImage2DDelegate CompressedTexSubImage2D;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void TexSubImage2DDelegate(TextureTarget target, int level,
            int x, int y, int width, int height, PixelFormat format, PixelType pixelType, IntPtr data);
        internal static TexSubImage2DDelegate TexSubImage2D;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void PixelStoreDelegate(PixelStoreParameter parameter, int size);
        internal static PixelStoreDelegate PixelStore;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void FinishDelegate();
        internal static FinishDelegate Finish;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void GetTexImageDelegate(
            TextureTarget target, int level, PixelFormat format, PixelType type, IntPtr pixels);
        internal static GetTexImageDelegate GetTexImageInternal;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void GetCompressedTexImageDelegate(TextureTarget target, int level, IntPtr pixels);
        internal static GetCompressedTexImageDelegate GetCompressedTexImageInternal;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void TexImage3DDelegate(TextureTarget target, int level, PixelInternalFormat internalFormat,
            int width, int height, int depth, int border, PixelFormat format, PixelType pixelType, IntPtr data);
        internal static TexImage3DDelegate TexImage3D;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void TexSubImage3DDelegate(TextureTarget target, int level,
            int x, int y, int z, int width, int height, int depth, PixelFormat format, PixelType pixelType, IntPtr data);
        internal static TexSubImage3DDelegate TexSubImage3D;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DeleteTexturesDelegate(int count, [In] in int id);
        internal static DeleteTexturesDelegate DeleteTextures;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void GenBuffersDelegate(int count, out int buffer);
        internal static GenBuffersDelegate GenBuffers;

        internal static GLHandle GenBuffer()
        {
            GenBuffers(1, out int buffer);
            return GLHandle.Buffer(buffer);
        }

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void BufferDataDelegate(BufferTarget target, IntPtr size, IntPtr n, BufferUsageHint usage);
        internal static BufferDataDelegate BufferData;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate IntPtr MapBufferDelegate(BufferTarget target, BufferAccess access);
        internal static MapBufferDelegate MapBuffer;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void UnmapBufferDelegate(BufferTarget target);
        internal static UnmapBufferDelegate UnmapBuffer;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void BufferSubDataDelegate(BufferTarget target, IntPtr offset, IntPtr size, IntPtr data);
        internal static BufferSubDataDelegate BufferSubData;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DeleteBuffersDelegate(int count, [In] in int buffer);
        internal static DeleteBuffersDelegate DeleteBuffers;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void VertexAttribPointerDelegate(int location, int elementCount, VertexAttribPointerType type, bool normalize,
            int stride, IntPtr data);
        internal static VertexAttribPointerDelegate VertexAttribPointer;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DrawElementsInstancedDelegate(GLPrimitiveType primitiveType, int count, IndexElementType elementType,
            IntPtr offset, int instanceCount);
        internal static DrawElementsInstancedDelegate DrawElementsInstanced;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void DrawElementsInstancedBaseInstanceDelegate(GLPrimitiveType primitiveType, int count, IndexElementType elementType,
            IntPtr offset, int instanceCount, int baseInstance);
        internal static DrawElementsInstancedBaseInstanceDelegate DrawElementsInstancedBaseInstance;

        [SuppressUnmanagedCodeSecurity()]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        internal delegate void VertexAttribDivisorDelegate(int location, int frequency);
        internal static VertexAttribDivisorDelegate VertexAttribDivisor;

#if DEBUG
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void DebugMessageCallbackProc(int source, int type, int id, int severity, int length, IntPtr message, IntPtr userParam);
        static DebugMessageCallbackProc DebugProc;

        [SuppressUnmanagedCodeSecurity()]
        [MonoNativeFunctionWrapper]
        delegate void DebugMessageCallbackDelegate(DebugMessageCallbackProc callback, IntPtr userParam);
        static DebugMessageCallbackDelegate DebugMessageCallback;

        internal delegate void ErrorDelegate(string message);
        internal static event ErrorDelegate OnError;

        static void DebugMessageCallbackHandler(int source, int type, int id, int severity, int length, IntPtr message, IntPtr userParam)
        {
            var errorMessage = Marshal.PtrToStringAnsi(message);
            Debug.WriteLine(errorMessage);
            OnError?.Invoke(errorMessage);
        }
#endif

        internal static int SwapInterval { get; set; }

        internal static void LoadEntryPoints()
        {
            LoadPlatformEntryPoints();

            if (Viewport == null)
                Viewport = LoadFunction<ViewportDelegate>("glViewport");
            if (Scissor == null)
                Scissor = LoadFunction<ScissorDelegate>("glScissor");
            if (MakeCurrent == null)
                MakeCurrent = LoadFunction<MakeCurrentDelegate>("glMakeCurrent");

            GetError = LoadFunction<GetErrorDelegate>("glGetError");

            TexParameterf = LoadFunction<TexParameterFloatDelegate>("glTexParameterf");
            TexParameterfv = LoadFunction<TexParameterFloatArrayDelegate>("glTexParameterfv");
            TexParameteri = LoadFunction<TexParameterIntDelegate>("glTexParameteri");

            EnableVertexAttribArray = LoadFunction<EnableVertexAttribArrayDelegate>("glEnableVertexAttribArray");
            DisableVertexAttribArray = LoadFunction<DisableVertexAttribArrayDelegate>("glDisableVertexAttribArray");
            GetIntegerv = LoadFunction<GetIntegerDelegate>("glGetIntegerv");
            GetStringInternal = LoadFunction<GetStringDelegate>("glGetString");
            ClearDepth = LoadFunction<ClearDepthDelegate>("glClearDepth");
            if (ClearDepth == null)
                ClearDepth = LoadFunction<ClearDepthDelegate>("glClearDepthf");
            DepthRanged = LoadFunction<DepthRangedDelegate>("glDepthRange");
            DepthRangef = LoadFunction<DepthRangefDelegate>("glDepthRangef");
            Clear = LoadFunction<ClearDelegate>("glClear");
            ClearColor = LoadFunction<ClearColorDelegate>("glClearColor");
            ClearStencil = LoadFunction<ClearStencilDelegate>("glClearStencil");
            Flush = LoadFunction<FlushDelegate>("glFlush");
            GenTextures = LoadFunction<GenTexturesDelegte>("glGenTextures");
            BindTexture = LoadFunction<BindTextureDelegate>("glBindTexture");

            Enable = LoadFunction<EnableDelegate>("glEnable");
            Disable = LoadFunction<DisableDelegate>("glDisable");
            CullFace = LoadFunction<CullFaceDelegate>("glCullFace");
            FrontFace = LoadFunction<FrontFaceDelegate>("glFrontFace");
            PolygonMode = LoadFunction<PolygonModeDelegate>("glPolygonMode");
            PolygonOffset = LoadFunction<PolygonOffsetDelegate>("glPolygonOffset");

            BindBuffer = LoadFunction<BindBufferDelegate>("glBindBuffer");
            DrawBuffers = LoadFunction<DrawBuffersDelegate>("glDrawBuffers");
            DrawElements = LoadFunction<DrawElementsDelegate>("glDrawElements");
            DrawArrays = LoadFunction<DrawArraysDelegate>("glDrawArrays");
            Uniform1i = LoadFunction<Uniform1iDelegate>("glUniform1i");
            Uniform4fv = LoadFunction<Uniform4fvDelegate>("glUniform4fv");
            ReadPixelsInternal = LoadFunction<ReadPixelsDelegate>("glReadPixels");

            ReadBuffer = LoadFunction<ReadBufferDelegate>("glReadBuffer");
            DrawBuffer = LoadFunction<DrawBufferDelegate>("glDrawBuffer");

            // Render Target Support. These might be null if they are not supported
            // see GraphicsDevice.OpenGL.FramebufferHelper.cs for handling other extensions.
            GenRenderbuffers = LoadFunction<GenRenderbuffersDelegate>("glGenRenderbuffers");
            BindRenderbuffer = LoadFunction<BindRenderbufferDelegate>("glBindRenderbuffer");
            DeleteRenderbuffers = LoadFunction<DeleteRenderbuffersDelegate>("glDeleteRenderbuffers");
            GenFramebuffers = LoadFunction<GenFramebuffersDelegate>("glGenFramebuffers");
            BindFramebuffer = LoadFunction<BindFramebufferDelegate>("glBindFramebuffer");
            DeleteFramebuffers = LoadFunction<DeleteFramebuffersDelegate>("glDeleteFramebuffers");
            FramebufferTexture2D = LoadFunction<FramebufferTexture2DDelegate>("glFramebufferTexture2D");
            FramebufferRenderbuffer = LoadFunction<FramebufferRenderbufferDelegate>("glFramebufferRenderbuffer");
            RenderbufferStorage = LoadFunction<RenderbufferStorageDelegate>("glRenderbufferStorage");
            RenderbufferStorageMultisample = LoadFunction<RenderbufferStorageMultisampleDelegate>("glRenderbufferStorageMultisample");
            GenerateMipmap = LoadFunction<GenerateMipmapDelegate>("glGenerateMipmap");
            BlitFramebuffer = LoadFunction<BlitFramebufferDelegate>("glBlitFramebuffer");
            CheckFramebufferStatus = LoadFunction<CheckFramebufferStatusDelegate>("glCheckFramebufferStatus");

            GenQueries = LoadFunction<GenQueriesDelegate>("glGenQueries");
            BeginQuery = LoadFunction<BeginQueryDelegate>("glBeginQuery");
            EndQuery = LoadFunction<EndQueryDelegate>("glEndQuery");
            GetQueryObject = LoadFunction<GetQueryObjectDelegate>("glGetQueryObjectuiv");
            if (GetQueryObject == null)
                GetQueryObject = LoadFunction<GetQueryObjectDelegate>("glGetQueryObjectivARB");
            if (GetQueryObject == null)
                GetQueryObject = LoadFunction<GetQueryObjectDelegate>("glGetQueryObjectiv");
            DeleteQueries = LoadFunction<DeleteQueriesDelegate>("glDeleteQueries");

            ActiveTexture = LoadFunction<ActiveTextureDelegate>("glActiveTexture");
            CreateShader = LoadFunction<CreateShaderDelegate>("glCreateShader");
            ShaderSourceInternal = LoadFunction<ShaderSourceDelegate>("glShaderSource");
            CompileShader = LoadFunction<CompileShaderDelegate>("glCompileShader");
            GetShaderiv = LoadFunction<GetShaderDelegate>("glGetShaderiv");
            GetShaderInfoLogInternal = LoadFunction<GetShaderInfoLogDelegate>("glGetShaderInfoLog");
            IsShader = LoadFunction<IsShaderDelegate>("glIsShader");
            DeleteShader = LoadFunction<DeleteShaderDelegate>("glDeleteShader");
            GetAttribLocation = LoadFunction<GetAttribLocationDelegate>("glGetAttribLocation");
            GetUniformLocation = LoadFunction<GetUniformLocationDelegate>("glGetUniformLocation");

            IsProgram = LoadFunction<IsProgramDelegate>("glIsProgram");
            DeleteProgram = LoadFunction<DeleteProgramDelegate>("glDeleteProgram");
            CreateProgram = LoadFunction<CreateProgramDelegate>("glCreateProgram");
            AttachShader = LoadFunction<AttachShaderDelegate>("glAttachShader");
            UseProgram = LoadFunction<UseProgramDelegate>("glUseProgram");
            LinkProgram = LoadFunction<LinkProgramDelegate>("glLinkProgram");
            GetProgramiv = LoadFunction<GetProgramDelegate>("glGetProgramiv");
            GetProgramInfoLogInternal = LoadFunction<GetProgramInfoLogDelegate>("glGetProgramInfoLog");
            DetachShader = LoadFunction<DetachShaderDelegate>("glDetachShader");

            BlendColor = LoadFunction<BlendColorDelegate>("glBlendColor");
            BlendEquationSeparate = LoadFunction<BlendEquationSeparateDelegate>("glBlendEquationSeparate");
            BlendEquationSeparatei = LoadFunction<BlendEquationSeparateiDelegate>("glBlendEquationSeparatei");
            BlendFuncSeparate = LoadFunction<BlendFuncSeparateDelegate>("glBlendFuncSeparate");
            BlendFuncSeparatei = LoadFunction<BlendFuncSeparateiDelegate>("glBlendFuncSeparatei");
            ColorMask = LoadFunction<ColorMaskDelegate>("glColorMask");
            DepthFunc = LoadFunction<DepthFuncDelegate>("glDepthFunc");
            DepthMask = LoadFunction<DepthMaskDelegate>("glDepthMask");
            StencilFuncSeparate = LoadFunction<StencilFuncSeparateDelegate>("glStencilFuncSeparate");
            StencilOpSeparate = LoadFunction<StencilOpSeparateDelegate>("glStencilOpSeparate");
            StencilFunc = LoadFunction<StencilFuncDelegate>("glStencilFunc");
            StencilOp = LoadFunction<StencilOpDelegate>("glStencilOp");
            StencilMask = LoadFunction<StencilMaskDelegate>("glStencilMask");

            CompressedTexImage2D = LoadFunction<CompressedTexImage2DDelegate>("glCompressedTexImage2D");
            TexImage2D = LoadFunction<TexImage2DDelegate>("glTexImage2D");
            CompressedTexSubImage2D = LoadFunction<CompressedTexSubImage2DDelegate>("glCompressedTexSubImage2D");
            TexSubImage2D = LoadFunction<TexSubImage2DDelegate>("glTexSubImage2D");
            PixelStore = LoadFunction<PixelStoreDelegate>("glPixelStorei");
            Finish = LoadFunction<FinishDelegate>("glFinish");
            GetTexImageInternal = LoadFunction<GetTexImageDelegate>("glGetTexImage");
            GetCompressedTexImageInternal = LoadFunction<GetCompressedTexImageDelegate>("glGetCompressedTexImage");
            TexImage3D = LoadFunction<TexImage3DDelegate>("glTexImage3D");
            TexSubImage3D = LoadFunction<TexSubImage3DDelegate>("glTexSubImage3D");
            DeleteTextures = LoadFunction<DeleteTexturesDelegate>("glDeleteTextures");

            GenBuffers = LoadFunction<GenBuffersDelegate>("glGenBuffers");
            BufferData = LoadFunction<BufferDataDelegate>("glBufferData");
            MapBuffer = LoadFunction<MapBufferDelegate>("glMapBuffer");
            UnmapBuffer = LoadFunction<UnmapBufferDelegate>("glUnmapBuffer");
            BufferSubData = LoadFunction<BufferSubDataDelegate>("glBufferSubData");
            DeleteBuffers = LoadFunction<DeleteBuffersDelegate>("glDeleteBuffers");

            VertexAttribPointer = LoadFunction<VertexAttribPointerDelegate>("glVertexAttribPointer");

            // Instanced drawing requires GL 3.2 or up, if the either of the following entry points can not be loaded
            // this will get flagged by setting SupportsInstancing in GraphicsCapabilities to false.
            try
            {
                DrawElementsInstanced = LoadFunction<DrawElementsInstancedDelegate>("glDrawElementsInstanced");
                VertexAttribDivisor = LoadFunction<VertexAttribDivisorDelegate>("glVertexAttribDivisor");
                DrawElementsInstancedBaseInstance = LoadFunction<DrawElementsInstancedBaseInstanceDelegate>("glDrawElementsInstancedBaseInstance");
            }
            catch (EntryPointNotFoundException)
            {
                // this will be detected in the initialization of GraphicsCapabilities
            }

#if DEBUG
            try
            {
                DebugMessageCallback = LoadFunction<DebugMessageCallbackDelegate>("glDebugMessageCallback");
                if (DebugMessageCallback != null)
                {
                    DebugProc = DebugMessageCallbackHandler;
                    DebugMessageCallback(DebugProc, IntPtr.Zero);
                    Enable(EnableCap.DebugOutput);
                    Enable(EnableCap.DebugOutputSynchronous);
                }
            }
            catch (EntryPointNotFoundException)
            {
                // Ignore the debug message callback if the entry point can not be found
            }
#endif
            if (BoundApi == RenderApi.ES)
            {
                InvalidateFramebuffer = LoadFunction<InvalidateFramebufferDelegate>("glDiscardFramebufferEXT");
            }

            LoadExtensions();
        }

        internal static List<string> Extensions = new List<string>();

        //[Conditional("DEBUG")]
        static void LogExtensions()
        {
#if __ANDROID__
            Android.Util.Log.Verbose("GL","Supported Extensions");
            foreach (var ext in Extensions)
                Android.Util.Log.Verbose("GL", "   " + ext);
#endif
        }

        internal static void LoadExtensions()
        {
            string? extstring = GetString(StringName.Extensions);
            var error = GetError();
            if (!string.IsNullOrEmpty(extstring) && error == ErrorCode.NoError)
                Extensions.AddRange(extstring.Split(' '));

            LogExtensions();
            // now load Extensions :)
            if (GenRenderbuffers == null && Extensions.Contains("GL_EXT_framebuffer_object"))
            {
                LoadFrameBufferObjectEXTEntryPoints();
            }
            if (RenderbufferStorageMultisample == null)
            {
                if (Extensions.Contains("GL_APPLE_framebuffer_multisample"))
                {
                    RenderbufferStorageMultisample = LoadFunction<RenderbufferStorageMultisampleDelegate>("glRenderbufferStorageMultisampleAPPLE");
                    BlitFramebuffer = LoadFunction<BlitFramebufferDelegate>("glResolveMultisampleFramebufferAPPLE");
                }
                else if (Extensions.Contains("GL_EXT_multisampled_render_to_texture"))
                {
                    RenderbufferStorageMultisample = LoadFunction<RenderbufferStorageMultisampleDelegate>("glRenderbufferStorageMultisampleEXT");
                    FramebufferTexture2DMultiSample = LoadFunction<FramebufferTexture2DMultiSampleDelegate>("glFramebufferTexture2DMultisampleEXT");

                }
                else if (Extensions.Contains("GL_IMG_multisampled_render_to_texture"))
                {
                    RenderbufferStorageMultisample = LoadFunction<RenderbufferStorageMultisampleDelegate>("glRenderbufferStorageMultisampleIMG");
                    FramebufferTexture2DMultiSample = LoadFunction<FramebufferTexture2DMultiSampleDelegate>("glFramebufferTexture2DMultisampleIMG");
                }
                else if (Extensions.Contains("GL_NV_framebuffer_multisample"))
                {
                    RenderbufferStorageMultisample = LoadFunction<RenderbufferStorageMultisampleDelegate>("glRenderbufferStorageMultisampleNV");
                    BlitFramebuffer = LoadFunction<BlitFramebufferDelegate>("glBlitFramebufferNV");
                }
            }
            if (BlendFuncSeparatei == null && Extensions.Contains("GL_ARB_draw_buffers_blend"))
            {
                BlendFuncSeparatei = LoadFunction<BlendFuncSeparateiDelegate>("BlendFuncSeparateiARB");
            }
            if (BlendEquationSeparatei == null && Extensions.Contains("GL_ARB_draw_buffers_blend"))
            {
                BlendEquationSeparatei = LoadFunction<BlendEquationSeparateiDelegate>("BlendEquationSeparateiARB");
            }
        }

        internal static void LoadFrameBufferObjectEXTEntryPoints()
        {
            GenRenderbuffers = LoadFunction<GenRenderbuffersDelegate>("glGenRenderbuffersEXT");
            BindRenderbuffer = LoadFunction<BindRenderbufferDelegate>("glBindRenderbufferEXT");
            DeleteRenderbuffers = LoadFunction<DeleteRenderbuffersDelegate>("glDeleteRenderbuffersEXT");
            GenFramebuffers = LoadFunction<GenFramebuffersDelegate>("glGenFramebuffersEXT");
            BindFramebuffer = LoadFunction<BindFramebufferDelegate>("glBindFramebufferEXT");
            DeleteFramebuffers = LoadFunction<DeleteFramebuffersDelegate>("glDeleteFramebuffersEXT");
            FramebufferTexture2D = LoadFunction<FramebufferTexture2DDelegate>("glFramebufferTexture2DEXT");
            FramebufferRenderbuffer = LoadFunction<FramebufferRenderbufferDelegate>("glFramebufferRenderbufferEXT");
            RenderbufferStorage = LoadFunction<RenderbufferStorageDelegate>("glRenderbufferStorageEXT");
            RenderbufferStorageMultisample = LoadFunction<RenderbufferStorageMultisampleDelegate>("glRenderbufferStorageMultisampleEXT");
            GenerateMipmap = LoadFunction<GenerateMipmapDelegate>("glGenerateMipmapEXT");
            BlitFramebuffer = LoadFunction<BlitFramebufferDelegate>("glBlitFramebufferEXT");
            CheckFramebufferStatus = LoadFunction<CheckFramebufferStatusDelegate>("glCheckFramebufferStatusEXT");
        }

        static partial void LoadPlatformEntryPoints();

        internal static IGraphicsContext CreateContext(IWindowHandle window)
        {
            return PlatformCreateContext(window);
        }

        #region Helper Functions

        internal static void DepthRange(float min, float max)
        {
            if (BoundApi == RenderApi.ES)
                DepthRangef(min, max);
            else
                DepthRanged(min, max);
        }

        internal static void Uniform1(int location, int value)
        {
            Uniform1i(location, value);
        }

        internal static void Uniform1(int location, float value)
        {
            Uniform1f(location, value);
        }

        internal static unsafe void Uniform4(int location, int size, ReadOnlySpan<float> values)
        {
            Uniform4fv(location, size, values[0]);
        }

        internal static unsafe string? GetString(StringName name)
        {
            return Marshal.PtrToStringAnsi(GetStringInternal(name));
        }

        protected static IntPtr MarshalStringArrayToPtr(string[] strings)
        {
            IntPtr intPtr = IntPtr.Zero;
            if (strings != null && strings.Length != 0)
            {
                intPtr = Marshal.AllocHGlobal(strings.Length * IntPtr.Size);
                if (intPtr == IntPtr.Zero)
                    throw new OutOfMemoryException();

                int i = 0;
                try
                {
                    for (i = 0; i < strings.Length; i++)
                    {
                        IntPtr val = MarshalStringToPtr(strings[i]);
                        Marshal.WriteIntPtr(intPtr, i * IntPtr.Size, val);
                    }
                }
                catch (OutOfMemoryException)
                {
                    for (i--; i >= 0; i--)
                        Marshal.FreeHGlobal(Marshal.ReadIntPtr(intPtr, i * IntPtr.Size));
                    Marshal.FreeHGlobal(intPtr);
                    throw;
                }
            }
            return intPtr;
        }

        protected static unsafe IntPtr MarshalStringToPtr(string str)
        {
            if (string.IsNullOrEmpty(str))
                return IntPtr.Zero;

            int num = Encoding.ASCII.GetMaxByteCount(str.Length) + 1;
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            if (intPtr == IntPtr.Zero)
                throw new OutOfMemoryException();

            fixed (char* chars = str + RuntimeHelpers.OffsetToStringData / 2)
            {
                int bytes = Encoding.ASCII.GetBytes(chars, str.Length, (byte*)(void*)intPtr, num);
                Marshal.WriteByte(intPtr, bytes, 0);
                return intPtr;
            }
        }

        protected static void FreeStringArrayPtr(IntPtr ptr, int length)
        {
            for (int i = 0; i < length; i++)
                Marshal.FreeHGlobal(Marshal.ReadIntPtr(ptr, i * IntPtr.Size));
            Marshal.FreeHGlobal(ptr);
        }

        internal static StringBuilder GetProgramInfoLog(int programId)
        {
            GetProgram(programId, GetProgramParameterName.LogLength, out int length);
            var sb = new StringBuilder();
            GetProgramInfoLogInternal(programId, length, IntPtr.Zero, sb);
            return sb;
        }

        internal static StringBuilder GetShaderInfoLog(int shaderId)
        {
            GetShader(shaderId, ShaderParameter.LogLength, out int length);
            var sb = new StringBuilder();
            GetShaderInfoLogInternal(shaderId, length, IntPtr.Zero, sb);
            return sb;
        }

        internal static unsafe void ShaderSource(int shaderId, string code)
        {
            IntPtr intPtr = MarshalStringArrayToPtr(new string[] { code });
            ShaderSourceInternal(shaderId, 1, intPtr, code.Length);
            FreeStringArrayPtr(intPtr, 1);
        }

        internal static unsafe void GetShader(int shaderId, ShaderParameter name, out int result)
        {
            GetShaderiv(shaderId, (int)name, out result);
        }

        internal static unsafe void GetProgram(int programId, GetProgramParameterName name, out int result)
        {
            GetProgramiv(programId, (int)name, out result);
        }

        internal static unsafe void GetInteger(GetPName name, out int result)
        {
            GetIntegerv((int)name, out result);
        }

        internal static unsafe void GetInteger(int name, out int result)
        {
            GetIntegerv(name, out result);
        }

        internal static void TexParameter(TextureTarget target, TextureParameterName name, float value)
        {
            TexParameterf(target, name, value);
        }

        internal static unsafe void TexParameter(
            TextureTarget target, TextureParameterName name, ReadOnlySpan<float> values)
        {
            TexParameterfv(target, name, values[0]);
        }

        internal static void TexParameter(TextureTarget target, TextureParameterName name, int value)
        {
            TexParameteri(target, name, value);
        }

        internal static void GetTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, IntPtr output)
        {
            GetTexImageInternal(target, level, format, type, output);
        }

        internal static void GetCompressedTexImage(TextureTarget target, int level, IntPtr output)
        {
            GetCompressedTexImageInternal(target, level, output);
        }

        public static void ReadPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, IntPtr output)
        {
            ReadPixelsInternal(x, y, width, height, format, type, output);
        }

        #endregion
    }
}