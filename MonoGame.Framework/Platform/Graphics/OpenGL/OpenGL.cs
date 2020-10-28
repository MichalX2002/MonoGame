// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace MonoGame.OpenGL
{
    public partial struct ColorFormat
    {
        public int R { get; }
        public int G { get; }
        public int B { get; }
        public int A { get; }

        public ColorFormat(int r, int g, int b, int a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }

    // TODO: NET5 function pointers

    public static partial class GL
    {
        public enum RenderAPI
        {
            ES = 12448,
            GL = 12450,
        }

        public static RenderAPI BoundAPI { get; private set; } = RenderAPI.GL;

        public static bool IsES => BoundAPI == RenderAPI.ES;

        // TODO: FastCall on NET5
        private const CallingConvention callingConvention = CallingConvention.Winapi;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void EnableVertexAttribArrayDelegate(int attrib);
        public static EnableVertexAttribArrayDelegate EnableVertexAttribArray;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DisableVertexAttribArrayDelegate(int attrib);
        public static DisableVertexAttribArrayDelegate DisableVertexAttribArray;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void MakeCurrentDelegate(IntPtr window);
        public static MakeCurrentDelegate? MakeCurrent;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GetIntegerDelegate(int param, [Out] out int data);
        public static GetIntegerDelegate GetIntegerv;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate IntPtr GetStringDelegate(StringName param);
        public static GetStringDelegate GetStringInternal;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void ClearDepthDelegate(double depth);
        public static ClearDepthDelegate ClearDepth;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void ClearDepthFDelegate(float depth);
        public static ClearDepthFDelegate? ClearDepthF;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DepthRangedDelegate(double min, double max);
        public static DepthRangedDelegate DepthRanged;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DepthRangefDelegate(float min, float max);
        public static DepthRangefDelegate DepthRangef;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void ClearDelegate(ClearBufferMask mask);
        public static ClearDelegate Clear;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void ClearColorDelegate(float red, float green, float blue, float alpha);
        public static ClearColorDelegate ClearColor;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void ClearStencilDelegate(int stencil);
        public static ClearStencilDelegate ClearStencil;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void ViewportDelegate(int x, int y, int w, int h);
        public static ViewportDelegate? Viewport;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate ErrorCode GetErrorDelegate();
        public static GetErrorDelegate GetError;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void FlushDelegate();
        public static FlushDelegate Flush;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GenTexturesDelegte(int count, [Out] out int id);
        public static GenTexturesDelegte GenTextures;

        public static GLHandle GenTexture()
        {
            GenTextures(1, out int id);
            return GLHandle.Texture(id);
        }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void BindTextureDelegate(TextureTarget target, int id);
        public static BindTextureDelegate BindTexture;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate int EnableDelegate(EnableCap cap);
        public static EnableDelegate Enable;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate int DisableDelegate(EnableCap cap);
        public static DisableDelegate Disable;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void CullFaceDelegate(CullFaceMode mode);
        public static CullFaceDelegate CullFace;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void FrontFaceDelegate(FrontFaceDirection direction);
        public static FrontFaceDelegate FrontFace;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void PolygonModeDelegate(MaterialFace face, PolygonMode mode);
        public static PolygonModeDelegate PolygonMode;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void PolygonOffsetDelegate(float slopeScaleDepthBias, float depthbias);
        public static PolygonOffsetDelegate PolygonOffset;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DrawBuffersDelegate(int count, DrawBuffersElementType[] buffers);
        public static DrawBuffersDelegate DrawBuffers;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void UseProgramDelegate(int program);
        public static UseProgramDelegate UseProgram;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void Uniform4fvDelegate(int location, int size, [In] in float values);
        public static Uniform4fvDelegate Uniform4fv;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void Uniform1iDelegate(int location, int value);
        public static Uniform1iDelegate Uniform1i;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void Uniform1fDelegate(int location, float value);
        public static Uniform1fDelegate Uniform1f;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void ScissorDelegate(int x, int y, int width, int height);
        public static ScissorDelegate? Scissor;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void ReadPixelsDelegate(
            int x, int y, int width, int height, PixelFormat format, PixelType type, IntPtr data);
        public static ReadPixelsDelegate ReadPixelsInternal;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void BindBufferDelegate(BufferTarget target, int buffer);
        public static BindBufferDelegate BindBuffer;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DrawElementsDelegate(
            GLPrimitiveType primitiveType, int count, IndexElementType elementType, IntPtr offset);
        public static DrawElementsDelegate DrawElements;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DrawArraysDelegate(GLPrimitiveType primitiveType, int offset, int count);
        public static DrawArraysDelegate DrawArrays;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GenRenderbuffersDelegate(int count, [Out] out int buffer);
        public static GenRenderbuffersDelegate? GenRenderbuffers;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void BindRenderbufferDelegate(RenderbufferTarget target, int buffer);
        public static BindRenderbufferDelegate? BindRenderbuffer;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DeleteRenderbuffersDelegate(int count, [In] in int buffer);
        public static DeleteRenderbuffersDelegate? DeleteRenderbuffers;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void RenderbufferStorageMultisampleDelegate(RenderbufferTarget target, int sampleCount,
            RenderbufferStorage storage, int width, int height);
        public static RenderbufferStorageMultisampleDelegate? RenderbufferStorageMultisample;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GenFramebuffersDelegate(int count, out int buffer);
        public static GenFramebuffersDelegate? GenFramebuffers;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void BindFramebufferDelegate(FramebufferTarget target, int buffer);
        public static BindFramebufferDelegate? BindFramebuffer;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DeleteFramebuffersDelegate(int count, [In] in int buffer);
        public static DeleteFramebuffersDelegate? DeleteFramebuffers;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void InvalidateFramebufferDelegate(
            FramebufferTarget target, int numAttachments, FramebufferAttachment[] attachments);
        public static InvalidateFramebufferDelegate? InvalidateFramebuffer;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void FramebufferTexture2DDelegate(
            FramebufferTarget target, FramebufferAttachment attachement, TextureTarget textureTarget, int texture, int level);
        public static FramebufferTexture2DDelegate? FramebufferTexture2D;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void FramebufferTexture2DMultiSampleDelegate(
            FramebufferTarget target, FramebufferAttachment attachement,
            TextureTarget textureTarget, int texture, int level, int samples);
        public static FramebufferTexture2DMultiSampleDelegate? FramebufferTexture2DMultiSample;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void FramebufferRenderbufferDelegate(FramebufferTarget target, FramebufferAttachment attachement,
            RenderbufferTarget renderBufferTarget, int buffer);
        public static FramebufferRenderbufferDelegate? FramebufferRenderbuffer;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void RenderbufferStorageDelegate(
            RenderbufferTarget target, RenderbufferStorage storage, int width, int hegiht);
        public static RenderbufferStorageDelegate? RenderbufferStorage;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GenerateMipmapDelegate(GenerateMipmapTarget target);
        public static GenerateMipmapDelegate? GenerateMipmap;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void ReadBufferDelegate(ReadBufferMode buffer);
        public static ReadBufferDelegate? ReadBuffer;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DrawBufferDelegate(DrawBufferMode buffer);
        public static DrawBufferDelegate? DrawBuffer;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void BlitFramebufferDelegate(int srcX0,
            int srcY0,
            int srcX1,
            int srcY1,
            int dstX0,
            int dstY0,
            int dstX1,
            int dstY1,
            ClearBufferMask mask,
            BlitFramebufferFilter filter);
        public static BlitFramebufferDelegate BlitFramebuffer;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate FramebufferErrorCode CheckFramebufferStatusDelegate(FramebufferTarget target);
        public static CheckFramebufferStatusDelegate CheckFramebufferStatus;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void TexParameterFloatDelegate(TextureTarget target, TextureParameterName name, float value);
        public static TexParameterFloatDelegate TexParameterf;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void TexParameterFloatArrayDelegate(TextureTarget target, TextureParameterName name, [In] in float values);
        public static TexParameterFloatArrayDelegate TexParameterfv;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void TexParameterIntDelegate(TextureTarget target, TextureParameterName name, int value);
        public static TexParameterIntDelegate TexParameteri;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GenQueriesDelegate(int count, [Out] out int queryId);
        public static GenQueriesDelegate GenQueries;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void BeginQueryDelegate(QueryTarget target, int queryId);
        public static BeginQueryDelegate BeginQuery;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void EndQueryDelegate(QueryTarget target);
        public static EndQueryDelegate EndQuery;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GetQueryObjectDelegate(int queryId, GetQueryObjectParam getparam, [Out] out int ready);
        public static GetQueryObjectDelegate? GetQueryObject;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DeleteQueriesDelegate(int count, [In] in int queryId);
        public static DeleteQueriesDelegate DeleteQueries;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void ActiveTextureDelegate(TextureUnit textureUnit);
        public static ActiveTextureDelegate ActiveTexture;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate int CreateShaderDelegate(ShaderType type);
        public static CreateShaderDelegate CreateShader;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void ShaderSourceDelegate(int shaderId, int count, IntPtr code, IntPtr length);
        public static ShaderSourceDelegate ShaderSourceInternal;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void CompileShaderDelegate(int shaderId);
        public static CompileShaderDelegate CompileShader;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GetShaderDelegate(int shaderId, int parameter, out int value);
        public static GetShaderDelegate GetShaderiv;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GetShaderInfoLogDelegate(int shader, int bufSize, IntPtr length, StringBuilder infoLog);
        public static GetShaderInfoLogDelegate GetShaderInfoLogInternal;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate bool IsShaderDelegate(int shaderId);
        public static IsShaderDelegate IsShader;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DeleteShaderDelegate(int shaderId);
        public static DeleteShaderDelegate DeleteShader;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate int GetAttribLocationDelegate(int programId, string name);
        public static GetAttribLocationDelegate GetAttribLocation;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate int GetUniformLocationDelegate(int programId, string name);
        public static GetUniformLocationDelegate GetUniformLocation;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate bool IsProgramDelegate(int programId);
        public static IsProgramDelegate IsProgram;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DeleteProgramDelegate(int programId);
        public static DeleteProgramDelegate DeleteProgram;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate int CreateProgramDelegate();
        public static CreateProgramDelegate CreateProgram;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void AttachShaderDelegate(int programId, int shaderId);
        public static AttachShaderDelegate AttachShader;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void LinkProgramDelegate(int programId);
        public static LinkProgramDelegate LinkProgram;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GetProgramDelegate(int programId, int name, out int linked);
        public static GetProgramDelegate GetProgramiv;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GetProgramInfoLogDelegate(int program, int bufSize, IntPtr length, StringBuilder infoLog);
        public static GetProgramInfoLogDelegate GetProgramInfoLogInternal;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DetachShaderDelegate(int programId, int shaderId);
        public static DetachShaderDelegate DetachShader;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void BlendColorDelegate(float r, float g, float b, float a);
        public static BlendColorDelegate BlendColor;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void BlendEquationSeparateDelegate(BlendEquationMode colorMode, BlendEquationMode alphaMode);
        public static BlendEquationSeparateDelegate BlendEquationSeparate;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void BlendEquationSeparateiDelegate(int buffer, BlendEquationMode colorMode, BlendEquationMode alphaMode);
        public static BlendEquationSeparateiDelegate BlendEquationSeparatei;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void BlendFuncSeparateDelegate(BlendingFactorSrc colorSrc, BlendingFactorDest colorDst,
            BlendingFactorSrc alphaSrc, BlendingFactorDest alphaDst);
        public static BlendFuncSeparateDelegate BlendFuncSeparate;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void BlendFuncSeparateiDelegate(int buffer, BlendingFactorSrc colorSrc, BlendingFactorDest colorDst,
            BlendingFactorSrc alphaSrc, BlendingFactorDest alphaDst);
        public static BlendFuncSeparateiDelegate BlendFuncSeparatei;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void ColorMaskDelegate(bool r, bool g, bool b, bool a);
        public static ColorMaskDelegate ColorMask;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DepthFuncDelegate(DepthFunction function);
        public static DepthFuncDelegate DepthFunc;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DepthMaskDelegate(bool enabled);
        public static DepthMaskDelegate DepthMask;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void StencilFuncSeparateDelegate(StencilFace face, GLStencilFunction function, int referenceStencil, int mask);
        public static StencilFuncSeparateDelegate StencilFuncSeparate;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void StencilOpSeparateDelegate(StencilFace face, StencilOp stencilfail, StencilOp depthFail, StencilOp pass);
        public static StencilOpSeparateDelegate StencilOpSeparate;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void StencilFuncDelegate(GLStencilFunction function, int referenceStencil, int mask);
        public static StencilFuncDelegate StencilFunc;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void StencilOpDelegate(StencilOp stencilfail, StencilOp depthFail, StencilOp pass);
        public static StencilOpDelegate StencilOp;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void StencilMaskDelegate(int mask);
        public static StencilMaskDelegate StencilMask;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void CompressedTexImage2DDelegate(TextureTarget target, int level, PixelInternalFormat internalFormat,
            int width, int height, int border, int size, IntPtr data);
        public static CompressedTexImage2DDelegate CompressedTexImage2D;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void TexImage2DDelegate(TextureTarget target, int level, PixelInternalFormat internalFormat,
            int width, int height, int border, PixelFormat format, PixelType pixelType, IntPtr data);
        public static TexImage2DDelegate TexImage2D;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void CompressedTexSubImage2DDelegate(TextureTarget target, int level,
            int x, int y, int width, int height, PixelInternalFormat format, int size, IntPtr data);
        public static CompressedTexSubImage2DDelegate CompressedTexSubImage2D;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void TexSubImage2DDelegate(TextureTarget target, int level,
            int x, int y, int width, int height, PixelFormat format, PixelType pixelType, IntPtr data);
        public static TexSubImage2DDelegate TexSubImage2D;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void PixelStoreDelegate(PixelStoreParameter parameter, int size);
        public static PixelStoreDelegate PixelStore;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void FinishDelegate();
        public static FinishDelegate Finish;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GetTexImageDelegate(
            TextureTarget target, int level, PixelFormat format, PixelType type, IntPtr pixels);
        public static GetTexImageDelegate GetTexImageInternal;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GetCompressedTexImageDelegate(TextureTarget target, int level, IntPtr pixels);
        public static GetCompressedTexImageDelegate GetCompressedTexImageInternal;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void TexImage3DDelegate(TextureTarget target, int level, PixelInternalFormat internalFormat,
            int width, int height, int depth, int border, PixelFormat format, PixelType pixelType, IntPtr data);
        public static TexImage3DDelegate TexImage3D;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void TexSubImage3DDelegate(TextureTarget target, int level,
            int x, int y, int z, int width, int height, int depth, PixelFormat format, PixelType pixelType, IntPtr data);
        public static TexSubImage3DDelegate TexSubImage3D;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DeleteTexturesDelegate(int count, [In] in int id);
        public static DeleteTexturesDelegate DeleteTextures;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void GenBuffersDelegate(int count, out int buffer);
        public static GenBuffersDelegate GenBuffers;

        public static GLHandle GenBuffer()
        {
            GenBuffers(1, out int buffer);
            return GLHandle.Buffer(buffer);
        }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void BufferDataDelegate(BufferTarget target, IntPtr size, IntPtr n, BufferUsageHint usage);
        public static BufferDataDelegate BufferData;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate IntPtr MapBufferDelegate(BufferTarget target, BufferAccess access);
        public static MapBufferDelegate MapBuffer;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void UnmapBufferDelegate(BufferTarget target);
        public static UnmapBufferDelegate UnmapBuffer;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void BufferSubDataDelegate(BufferTarget target, IntPtr offset, IntPtr size, IntPtr data);
        public static BufferSubDataDelegate BufferSubData;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DeleteBuffersDelegate(int count, [In] in int buffer);
        public static DeleteBuffersDelegate DeleteBuffers;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void VertexAttribPointerDelegate(int location, int elementCount, VertexAttribPointerType type, bool normalize,
            int stride, IntPtr data);
        public static VertexAttribPointerDelegate VertexAttribPointer;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DrawElementsInstancedDelegate(GLPrimitiveType primitiveType, int count, IndexElementType elementType,
            IntPtr offset, int instanceCount);
        public static DrawElementsInstancedDelegate DrawElementsInstanced;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void DrawElementsInstancedBaseInstanceDelegate(GLPrimitiveType primitiveType, int count, IndexElementType elementType,
            IntPtr offset, int instanceCount, int baseInstance);
        public static DrawElementsInstancedBaseInstanceDelegate DrawElementsInstancedBaseInstance;

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(callingConvention)]
        [MonoNativeFunctionWrapper]
        public delegate void VertexAttribDivisorDelegate(int location, int frequency);
        public static VertexAttribDivisorDelegate VertexAttribDivisor;

        public delegate void ErrorDelegate(string? message);
        public static event ErrorDelegate? OnError;

#if DEBUG
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void DebugMessageCallbackProc(
            int source, int type, int id, int severity, int length, IntPtr message, IntPtr userParam);
        private static DebugMessageCallbackProc DebugProc;

        [SuppressUnmanagedCodeSecurity]
        [MonoNativeFunctionWrapper]
        private delegate void DebugMessageCallbackDelegate(DebugMessageCallbackProc callback, IntPtr userParam);
        private static DebugMessageCallbackDelegate DebugMessageCallback;

        private static void DebugMessageCallbackHandler(
            int source, int type, int id, int severity, int length, IntPtr message, IntPtr userParam)
        {
            var errorMessage = Marshal.PtrToStringAnsi(message);
            System.Diagnostics.Debug.WriteLine(errorMessage);
            OnError?.Invoke(errorMessage);
        }
#endif

        public static void LoadEntryPoints()
        {
            LoadPlatformEntryPoints();

            Viewport ??= TryLoadFunction<ViewportDelegate>("glViewport");
            Scissor ??= TryLoadFunction<ScissorDelegate>("glScissor");
            MakeCurrent ??= TryLoadFunction<MakeCurrentDelegate>("glMakeCurrent");

            GetError = LoadFunction<GetErrorDelegate>("glGetError");

            TexParameterf = LoadFunction<TexParameterFloatDelegate>("glTexParameterf");
            TexParameterfv = LoadFunction<TexParameterFloatArrayDelegate>("glTexParameterfv");
            TexParameteri = LoadFunction<TexParameterIntDelegate>("glTexParameteri");

            EnableVertexAttribArray = LoadFunction<EnableVertexAttribArrayDelegate>("glEnableVertexAttribArray");
            DisableVertexAttribArray = LoadFunction<DisableVertexAttribArrayDelegate>("glDisableVertexAttribArray");
            GetIntegerv = LoadFunction<GetIntegerDelegate>("glGetIntegerv");
            GetStringInternal = LoadFunction<GetStringDelegate>("glGetString");
            ClearDepth = LoadFunction<ClearDepthDelegate>("glClearDepth");
            ClearDepthF ??= TryLoadFunction<ClearDepthFDelegate>("glClearDepthf");
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
            GetQueryObject ??= TryLoadFunction<GetQueryObjectDelegate>("glGetQueryObjectuiv");
            GetQueryObject ??= TryLoadFunction<GetQueryObjectDelegate>("glGetQueryObjectivARB");
            GetQueryObject ??= TryLoadFunction<GetQueryObjectDelegate>("glGetQueryObjectiv");
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
            if (IsES)
            {
                InvalidateFramebuffer = LoadFunction<InvalidateFramebufferDelegate>("glDiscardFramebufferEXT");
            }

            LoadExtensions();
        }

        private static HashSet<string> Extensions { get; } = new HashSet<string>();

        //[Conditional("DEBUG")]
        private static void LogExtensions()
        {
#if __ANDROID__
            Android.Util.Log.Verbose("GL", "Supported Extensions");
            foreach (var ext in Extensions)
                Android.Util.Log.Verbose("GL", "   " + ext);
#endif
        }

        public static void LoadExtensions()
        {
            string? extstring = GetString(StringName.Extensions);
            var error = GetError();
            if (!string.IsNullOrEmpty(extstring) && error == ErrorCode.NoError)
            {
                foreach (string ext in extstring.Split(' '))
                    Extensions.Add(ext);
            }

            LogExtensions();
            // now load Extensions :)
            if (GenRenderbuffers == null && HasExtension("GL_EXT_framebuffer_object"))
            {
                LoadFrameBufferObjectEXTEntryPoints();
            }
            if (RenderbufferStorageMultisample == null)
            {
                if (HasExtension("GL_APPLE_framebuffer_multisample"))
                {
                    RenderbufferStorageMultisample = LoadFunction<RenderbufferStorageMultisampleDelegate>("glRenderbufferStorageMultisampleAPPLE");
                    BlitFramebuffer = LoadFunction<BlitFramebufferDelegate>("glResolveMultisampleFramebufferAPPLE");
                }
                else if (HasExtension("GL_EXT_multisampled_render_to_texture"))
                {
                    RenderbufferStorageMultisample = LoadFunction<RenderbufferStorageMultisampleDelegate>("glRenderbufferStorageMultisampleEXT");
                    FramebufferTexture2DMultiSample = LoadFunction<FramebufferTexture2DMultiSampleDelegate>("glFramebufferTexture2DMultisampleEXT");
                }
                else if (HasExtension("GL_IMG_multisampled_render_to_texture"))
                {
                    RenderbufferStorageMultisample = LoadFunction<RenderbufferStorageMultisampleDelegate>("glRenderbufferStorageMultisampleIMG");
                    FramebufferTexture2DMultiSample = LoadFunction<FramebufferTexture2DMultiSampleDelegate>("glFramebufferTexture2DMultisampleIMG");
                }
                else if (HasExtension("GL_NV_framebuffer_multisample"))
                {
                    RenderbufferStorageMultisample = LoadFunction<RenderbufferStorageMultisampleDelegate>("glRenderbufferStorageMultisampleNV");
                    BlitFramebuffer = LoadFunction<BlitFramebufferDelegate>("glBlitFramebufferNV");
                }
            }

            if (BlendFuncSeparatei == null && HasExtension("GL_ARB_draw_buffers_blend"))
                BlendFuncSeparatei = LoadFunction<BlendFuncSeparateiDelegate>("BlendFuncSeparateiARB");

            if (BlendEquationSeparatei == null && HasExtension("GL_ARB_draw_buffers_blend"))
                BlendEquationSeparatei = LoadFunction<BlendEquationSeparateiDelegate>("BlendEquationSeparateiARB");
        }

        public static bool HasExtension(string extension)
        {
            return Extensions.Contains(extension);
        }

        public static void LoadFrameBufferObjectEXTEntryPoints()
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

        public static IGraphicsContext CreateContext(IWindowHandle window)
        {
            return PlatformCreateContext(window);
        }

        #region Helper Functions

        public static void DepthRange(float min, float max)
        {
            if (BoundAPI == RenderAPI.ES)
                DepthRangef(min, max);
            else
                DepthRanged(min, max);
        }

        public static void Uniform1(int location, int value)
        {
            Uniform1i(location, value);
        }

        public static void Uniform1(int location, float value)
        {
            Uniform1f(location, value);
        }

        public static void Uniform4(int location, int size, ReadOnlySpan<float> values)
        {
            Uniform4fv(location, size, values[0]);
        }

        public static string? GetString(StringName name)
        {
            return Marshal.PtrToStringAnsi(GetStringInternal(name));
        }

        public static StringBuilder GetProgramInfoLog(int programId)
        {
            GetProgram(programId, GetProgramParameterName.LogLength, out int length);
            var sb = new StringBuilder();
            GetProgramInfoLogInternal(programId, length, IntPtr.Zero, sb);
            return sb;
        }

        public static StringBuilder GetShaderInfoLog(int shaderId)
        {
            GetShader(shaderId, ShaderParameter.LogLength, out int length);
            var sb = new StringBuilder();
            GetShaderInfoLogInternal(shaderId, length, IntPtr.Zero, sb);
            return sb;
        }

        public static void ShaderSource(int shaderId, string code)
        {
            IntPtr strArray = MarshalStringArrayToPtr(new string[] { code });
            ShaderSourceInternal(shaderId, 1, strArray, IntPtr.Zero);
            FreeStringArrayPtr(strArray, 1);
        }

        public static void GetShader(int shaderId, ShaderParameter name, out int result)
        {
            GetShaderiv(shaderId, (int)name, out result);
        }

        public static void GetProgram(int programId, GetProgramParameterName name, out int result)
        {
            GetProgramiv(programId, (int)name, out result);
        }

        public static void GetInteger(GetPName name, out int result)
        {
            GetIntegerv((int)name, out result);
        }

        public static void GetInteger(int name, out int result)
        {
            GetIntegerv(name, out result);
        }

        public static void TexParameter(TextureTarget target, TextureParameterName name, float value)
        {
            TexParameterf(target, name, value);
        }

        public static void TexParameter(
            TextureTarget target, TextureParameterName name, ReadOnlySpan<float> values)
        {
            TexParameterfv(target, name, values[0]);
        }

        public static void TexParameter(TextureTarget target, TextureParameterName name, int value)
        {
            TexParameteri(target, name, value);
        }

        public static void GetTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, IntPtr output)
        {
            GetTexImageInternal(target, level, format, type, output);
        }

        public static void GetCompressedTexImage(TextureTarget target, int level, IntPtr output)
        {
            GetCompressedTexImageInternal(target, level, output);
        }

        public static void ReadPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, IntPtr output)
        {
            ReadPixelsInternal(x, y, width, height, format, type, output);
        }

        public static IntPtr MarshalStringArrayToPtr(string[] strings)
        {
            var array = IntPtr.Zero;
            if (strings != null && strings.Length != 0)
            {
                array = Marshal.AllocHGlobal(strings.Length * IntPtr.Size);

                int i = 0;
                try
                {
                    for (i = 0; i < strings.Length; i++)
                    {
                        IntPtr val = MarshalStringToPtr(strings[i]);
                        Marshal.WriteIntPtr(array, i * IntPtr.Size, val);
                    }
                }
                catch (OutOfMemoryException)
                {
                    for (i--; i >= 0; i--)
                        Marshal.FreeHGlobal(Marshal.ReadIntPtr(array, i * IntPtr.Size));
                    Marshal.FreeHGlobal(array);
                    throw;
                }
            }
            return array;
        }

        public static unsafe IntPtr MarshalStringToPtr(string str)
        {
            if (string.IsNullOrEmpty(str))
                return IntPtr.Zero;

            int num = Encoding.ASCII.GetMaxByteCount(str.Length) + 1;
            IntPtr intPtr = Marshal.AllocHGlobal(num);

            fixed (char* chars = str)
            {
                int bytes = Encoding.ASCII.GetBytes(chars, str.Length, (byte*)intPtr, num);
                Marshal.WriteByte(intPtr, bytes, 0);
                return intPtr;
            }
        }

        public static void FreeStringArrayPtr(IntPtr ptr, int length)
        {
            for (int i = 0; i < length; i++)
                Marshal.FreeHGlobal(Marshal.ReadIntPtr(ptr, i * IntPtr.Size));
            Marshal.FreeHGlobal(ptr);
        }


        #endregion
    }
}