using System;
using System.Collections.Generic;
using MonoGame.Framework.Graphics;

namespace MonoGame.Effect
{
    internal partial class EffectObject
    {
        private EffectObject()
        {
        }

        public enum D3DRENDERSTATETYPE
        {
            ZENABLE = 7,
            FILLMODE = 8,
            SHADEMODE = 9,
            ZWRITEENABLE = 14,
            ALPHATESTENABLE = 15,
            LASTPIXEL = 16,
            SRCBLEND = 19,
            DESTBLEND = 20,
            CULLMODE = 22,
            ZFUNC = 23,
            ALPHAREF = 24,
            ALPHAFUNC = 25,
            DITHERENABLE = 26,
            ALPHABLENDENABLE = 27,
            FOGENABLE = 28,
            SPECULARENABLE = 29,
            FOGCOLOR = 34,
            FOGTABLEMODE = 35,
            FOGSTART = 36,
            FOGEND = 37,
            FOGDENSITY = 38,
            RANGEFOGENABLE = 48,
            STENCILENABLE = 52,
            STENCILFAIL = 53,
            STENCILZFAIL = 54,
            STENCILPASS = 55,
            STENCILFUNC = 56,
            STENCILREF = 57,
            STENCILMASK = 58,
            STENCILWRITEMASK = 59,
            TEXTUREFACTOR = 60,
            WRAP0 = 128,
            WRAP1 = 129,
            WRAP2 = 130,
            WRAP3 = 131,
            WRAP4 = 132,
            WRAP5 = 133,
            WRAP6 = 134,
            WRAP7 = 135,
            CLIPPING = 136,
            LIGHTING = 137,
            AMBIENT = 139,
            FOGVERTEXMODE = 140,
            COLORVERTEX = 141,
            LOCALVIEWER = 142,
            NORMALIZENORMALS = 143,
            DIFFUSEMATERIALSOURCE = 145,
            SPECULARMATERIALSOURCE = 146,
            AMBIENTMATERIALSOURCE = 147,
            EMISSIVEMATERIALSOURCE = 148,
            VERTEXBLEND = 151,
            CLIPPLANEENABLE = 152,
            POINTSIZE = 154,
            POINTSIZE_MIN = 155,
            POINTSPRITEENABLE = 156,
            POINTSCALEENABLE = 157,
            POINTSCALE_A = 158,
            POINTSCALE_B = 159,
            POINTSCALE_C = 160,
            MULTISAMPLEANTIALIAS = 161,
            MULTISAMPLEMASK = 162,
            PATCHEDGESTYLE = 163,
            DEBUGMONITORTOKEN = 165,
            POINTSIZE_MAX = 166,
            INDEXEDVERTEXBLENDENABLE = 167,
            COLORWRITEENABLE = 168,
            TWEENFACTOR = 170,
            BLENDOP = 171,
            POSITIONDEGREE = 172,
            NORMALDEGREE = 173,
            SCISSORTESTENABLE = 174,
            SLOPESCALEDEPTHBIAS = 175,
            ANTIALIASEDLINEENABLE = 176,
            MINTESSELLATIONLEVEL = 178,
            MAXTESSELLATIONLEVEL = 179,
            ADAPTIVETESS_X = 180,
            ADAPTIVETESS_Y = 181,
            ADAPTIVETESS_Z = 182,
            ADAPTIVETESS_W = 183,
            ENABLEADAPTIVETESSELLATION = 184,
            TWOSIDEDSTENCILMODE = 185,
            CCW_STENCILFAIL = 186,
            CCW_STENCILZFAIL = 187,
            CCW_STENCILPASS = 188,
            CCW_STENCILFUNC = 189,
            COLORWRITEENABLE1 = 190,
            COLORWRITEENABLE2 = 191,
            COLORWRITEENABLE3 = 192,
            BLENDFACTOR = 193,
            SRGBWRITEENABLE = 194,
            DEPTHBIAS = 195,
            WRAP8 = 198,
            WRAP9 = 199,
            WRAP10 = 200,
            WRAP11 = 201,
            WRAP12 = 202,
            WRAP13 = 203,
            WRAP14 = 204,
            WRAP15 = 205,
            SEPARATEALPHABLENDENABLE = 206,
            SRCBLENDALPHA = 207,
            DESTBLENDALPHA = 208,
            BLENDOPALPHA = 209,

            FORCE_DWORD = 0x7fffffff
        }

        public enum D3DTEXTURESTAGESTATETYPE
        {
            COLOROP = 1,
            COLORARG1 = 2,
            COLORARG2 = 3,
            ALPHAOP = 4,
            ALPHAARG1 = 5,
            ALPHAARG2 = 6,
            BUMPENVMAT00 = 7,
            BUMPENVMAT01 = 8,
            BUMPENVMAT10 = 9,
            BUMPENVMAT11 = 10,
            TEXCOORDINDEX = 11,
            BUMPENVLSCALE = 22,
            BUMPENVLOFFSET = 23,
            TEXTURETRANSFORMFLAGS = 24,
            COLORARG0 = 26,
            ALPHAARG0 = 27,
            RESULTARG = 28,
            CONSTANT = 32,

            FORCE_DWORD = 0x7fffffff
        }

        public enum D3DTRANSFORMSTATETYPE
        {
            VIEW = 2,
            PROJECTION = 3,
            TEXTURE0 = 16,
            TEXTURE1 = 17,
            TEXTURE2 = 18,
            TEXTURE3 = 19,
            TEXTURE4 = 20,
            TEXTURE5 = 21,
            TEXTURE6 = 22,
            TEXTURE7 = 23,
            WORLD = 256,
            FORCE_DWORD = 0x7fffffff
        }

        public const int D3DX_PARAMETER_SHARED = 1;
        public const int D3DX_PARAMETER_LITERAL = 2;
        public const int D3DX_PARAMETER_ANNOTATION = 4;

        public enum D3DXPARAMETER_CLASS
        {
            SCALAR,
            VECTOR,
            MATRIX_ROWS,
            MATRIX_COLUMNS,
            OBJECT,
            STRUCT,
            FORCE_DWORD = 0x7fffffff,
        }

        public enum D3DXPARAMETER_TYPE
        {
            VOID,
            BOOL,
            INT,
            FLOAT,
            STRING,
            TEXTURE,
            TEXTURE1D,
            TEXTURE2D,
            TEXTURE3D,
            TEXTURECUBE,
            SAMPLER,
            SAMPLER1D,
            SAMPLER2D,
            SAMPLER3D,
            SAMPLERCUBE,
            PIXELSHADER,
            VERTEXSHADER,
            PIXELFRAGMENT,
            VERTEXFRAGMENT,
            UNSUPPORTED,
            FORCE_DWORD = 0x7fffffff,
        }

        enum D3DSAMPLERSTATETYPE
        {
            ADDRESSU = 1,
            ADDRESSV = 2,
            ADDRESSW = 3,
            BORDERCOLOR = 4,
            MAGFILTER = 5,
            MINFILTER = 6,
            MIPFILTER = 7,
            MIPMAPLODBIAS = 8,
            MAXMIPLEVEL = 9,
            MAXANISOTROPY = 10,
            SRGBTEXTURE = 11,
            ELEMENTINDEX = 12,
            DMAPOFFSET = 13,

            FORCE_DWORD = 0x7fffffff,
        };

        public enum STATE_CLASS
        {
            LIGHTENABLE,
            FVF,
            LIGHT,
            MATERIAL,
            NPATCHMODE,
            PIXELSHADER,
            RENDERSTATE,
            SETSAMPLER,
            SAMPLERSTATE,
            TEXTURE,
            TEXTURESTAGE,
            TRANSFORM,
            VERTEXSHADER,
            SHADERCONST,
            UNKNOWN,
        };

        public enum MATERIAL_TYPE
        {
            DIFFUSE,
            AMBIENT,
            SPECULAR,
            EMISSIVE,
            POWER,
        };

        public enum LIGHT_TYPE
        {
            TYPE,
            DIFFUSE,
            SPECULAR,
            AMBIENT,
            POSITION,
            DIRECTION,
            RANGE,
            FALLOFF,
            ATTENUATION0,
            ATTENUATION1,
            ATTENUATION2,
            THETA,
            PHI,
        };

        public enum SHADER_CONSTANT_TYPE
        {
            VSFLOAT,
            VSBOOL,
            VSINT,
            PSFLOAT,
            PSBOOL,
            PSINT,
        }

        public enum STATE_TYPE
        {
            CONSTANT,
            PARAMETER,
            EXPRESSION,
            EXPRESSIONINDEX,
        }

        public class D3Dx_parameter
        {
            public string name;
            public string semantic;
            public object data;
            public D3DXPARAMETER_CLASS class_;
            public D3DXPARAMETER_TYPE type;
            public uint rows;
            public uint columns;
            public uint element_count;
            public uint annotation_count = 0;
            public uint member_count;
            public uint flags = 0;
            public uint bytes = 0;

            public int bufferIndex = -1;
            public int bufferOffset = -1;

            public D3Dx_parameter[] annotation_handles = null;
            public D3Dx_parameter[] member_handles;

            public override string ToString()
            {
                if (rows > 0 || columns > 0)
                {
                    return string.Format(
                        "{0} {1}{2}x{3} {4} : cb{5},{6}",
                        class_, type, rows, columns, name, bufferIndex, bufferOffset);
                }
                else
                {
                    return string.Format(
                        "{0} {1} {2}", class_, type, name);
                }
            }
        }

        public class D3Dx_state
        {
            public uint operation;
            public uint index;
            public STATE_TYPE type;
            public D3Dx_parameter parameter;
        }

        public class D3Dx_sampler
        {
            public uint state_count = 0;
            public D3Dx_state[] states = null;
        }

        public class D3Dx_pass
        {
            public string name;
            public uint state_count;
            public uint annotation_count = 0;

            public BlendState blendState;
            public DepthStencilState depthStencilState;
            public RasterizerState rasterizerState;

            public D3Dx_state[] states;
            public D3Dx_parameter[] annotation_handles = null;
        }

        public class D3Dx_technique
        {
            public string name;
            public uint pass_count;
            public uint annotation_count = 0;

            public D3Dx_parameter[] annotation_handles = null;
            public D3Dx_pass[] pass_handles;
        }

        public class StateInfo
        {
            public STATE_CLASS Class { get; }
            public uint Op { get; }
            public string Name { get; }

            public StateInfo(STATE_CLASS class_, uint op, string name)
            {
                Class = class_;
                Op = op;
                Name = name;
            }
        }

        /// <summary>
        /// The shared state definition table.
        /// </summary>
        public static readonly StateInfo[] state_table =
        {
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ZENABLE, "ZENABLE"), /* 0x0 */
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FILLMODE, "FILLMODE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SHADEMODE, "SHADEMODE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ZWRITEENABLE, "ZWRITEENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ALPHATESTENABLE, "ALPHATESTENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.LASTPIXEL, "LASTPIXEL"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SRCBLEND, "SRCBLEND"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.DESTBLEND, "DESTBLEND"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CULLMODE, "CULLMODE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ZFUNC, "ZFUNC"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ALPHAREF, "ALPHAREF"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ALPHAFUNC, "ALPHAFUNC"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.DITHERENABLE, "DITHERENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ALPHABLENDENABLE, "ALPHABLENDENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGENABLE, "FOGENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SPECULARENABLE, "SPECULARENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGCOLOR, "FOGCOLOR"), /* 0x10 */
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGTABLEMODE, "FOGTABLEMODE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGSTART, "FOGSTART"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGEND, "FOGEND"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGDENSITY, "FOGDENSITY"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.RANGEFOGENABLE, "RANGEFOGENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILENABLE, "STENCILENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILFAIL, "STENCILFAIL"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILZFAIL, "STENCILZFAIL"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILPASS, "STENCILPASS"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILFUNC, "STENCILFUNC"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILREF, "STENCILREF"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILMASK, "STENCILMASK"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.STENCILWRITEMASK, "STENCILWRITEMASK"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.TEXTUREFACTOR, "TEXTUREFACTOR"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP0, "WRAP0"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP1, "WRAP1"), /* 0x20 */
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP2, "WRAP2"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP3, "WRAP3"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP4, "WRAP4"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP5, "WRAP5"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP6, "WRAP6"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP7, "WRAP7"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP8, "WRAP8"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP9, "WRAP9"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP10, "WRAP10"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP11, "WRAP11"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP12, "WRAP12"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP13, "WRAP13"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP14, "WRAP14"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.WRAP15, "WRAP15"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CLIPPING, "CLIPPING"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.LIGHTING, "LIGHTING"), /* 0x30 */
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.AMBIENT, "AMBIENT"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.FOGVERTEXMODE, "FOGVERTEXMODE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.COLORVERTEX, "COLORVERTEX"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.LOCALVIEWER, "LOCALVIEWER"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.NORMALIZENORMALS, "NORMALIZENORMALS"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.DIFFUSEMATERIALSOURCE, "DIFFUSEMATERIALSOURCE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SPECULARMATERIALSOURCE, "SPECULARMATERIALSOURCE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.AMBIENTMATERIALSOURCE, "AMBIENTMATERIALSOURCE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.EMISSIVEMATERIALSOURCE, "EMISSIVEMATERIALSOURCE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.VERTEXBLEND, "VERTEXBLEND"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CLIPPLANEENABLE, "CLIPPLANEENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSIZE, "POINTSIZE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSIZE_MIN, "POINTSIZE_MIN"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSIZE_MAX, "POINTSIZE_MAX"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSPRITEENABLE, "POINTSPRITEENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSCALEENABLE, "POINTSCALEENABLE"), /* 0x40 */
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSCALE_A, "POINTSCALE_A"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSCALE_B, "POINTSCALE_B"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POINTSCALE_C, "POINTSCALE_C"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.MULTISAMPLEANTIALIAS, "MULTISAMPLEANTIALIAS"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.MULTISAMPLEMASK, "MULTISAMPLEMASK"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.PATCHEDGESTYLE, "PATCHEDGESTYLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.DEBUGMONITORTOKEN, "DEBUGMONITORTOKEN"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.INDEXEDVERTEXBLENDENABLE, "INDEXEDVERTEXBLENDENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.COLORWRITEENABLE, "COLORWRITEENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.TWEENFACTOR, "TWEENFACTOR"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.BLENDOP, "BLENDOP"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.POSITIONDEGREE, "POSITIONDEGREE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.NORMALDEGREE, "NORMALDEGREE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SCISSORTESTENABLE, "SCISSORTESTENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SLOPESCALEDEPTHBIAS, "SLOPESCALEDEPTHBIAS"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ANTIALIASEDLINEENABLE, "ANTIALIASEDLINEENABLE"), /* 0x50 */
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.MINTESSELLATIONLEVEL, "MINTESSELLATIONLEVEL"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.MAXTESSELLATIONLEVEL, "MAXTESSELLATIONLEVEL"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ADAPTIVETESS_X, "ADAPTIVETESS_X"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ADAPTIVETESS_Y, "ADAPTIVETESS_Y"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ADAPTIVETESS_Z, "ADAPTIVETESS_Z"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ADAPTIVETESS_W, "ADAPTIVETESS_W"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.ENABLEADAPTIVETESSELLATION, "ENABLEADAPTIVETESSELLATION"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.TWOSIDEDSTENCILMODE, "TWOSIDEDSTENCILMODE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CCW_STENCILFAIL, "CCW_STENCILFAIL"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CCW_STENCILZFAIL, "CCW_STENCILZFAIL"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CCW_STENCILPASS, "CCW_STENCILPASS"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.CCW_STENCILFUNC, "CCW_STENCILFUNC"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.COLORWRITEENABLE1, "COLORWRITEENABLE1"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.COLORWRITEENABLE2, "COLORWRITEENABLE2"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.COLORWRITEENABLE3, "COLORWRITEENABLE3"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.BLENDFACTOR, "BLENDFACTOR"), /* 0x60 */
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SRGBWRITEENABLE, "SRGBWRITEENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.DEPTHBIAS, "DEPTHBIAS"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SEPARATEALPHABLENDENABLE, "SEPARATEALPHABLENDENABLE"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.SRCBLENDALPHA, "SRCBLENDALPHA"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.DESTBLENDALPHA, "DESTBLENDALPHA"),
            new StateInfo(STATE_CLASS.RENDERSTATE, (uint)D3DRENDERSTATETYPE.BLENDOPALPHA, "BLENDOPALPHA"),
            /* Texture stages */
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.COLOROP, "COLOROP"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.COLORARG0, "COLORARG0"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.COLORARG1, "COLORARG1"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.COLORARG2, "COLORARG2"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.ALPHAOP, "ALPHAOP"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.ALPHAARG0, "ALPHAARG0"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.ALPHAARG1, "ALPHAARG1"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.ALPHAARG2, "ALPHAARG2"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.RESULTARG, "RESULTARG"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.BUMPENVMAT00, "BUMPENVMAT00"), /* 0x70 */
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.BUMPENVMAT01, "BUMPENVMAT01"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.BUMPENVMAT10, "BUMPENVMAT10"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.BUMPENVMAT11, "BUMPENVMAT11"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.TEXCOORDINDEX, "TEXCOORDINDEX"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.BUMPENVLSCALE, "BUMPENVLSCALE"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.BUMPENVLOFFSET, "BUMPENVLOFFSET"),
            new StateInfo(STATE_CLASS.TEXTURESTAGE, (uint)D3DTEXTURESTAGESTATETYPE.TEXTURETRANSFORMFLAGS, "TEXTURETRANSFORMFLAGS"),
            /* */
            new StateInfo(STATE_CLASS.UNKNOWN, 0, "UNKNOWN"),
            /* NPatchMode */
            new StateInfo(STATE_CLASS.NPATCHMODE, 0, "NPatchMode"),
            /* */
            new StateInfo(STATE_CLASS.UNKNOWN, 0, "UNKNOWN"),
            /* Transform */
            new StateInfo(STATE_CLASS.TRANSFORM, (uint)D3DTRANSFORMSTATETYPE.PROJECTION, "PROJECTION"),
            new StateInfo(STATE_CLASS.TRANSFORM, (uint)D3DTRANSFORMSTATETYPE.VIEW, "VIEW"),
            new StateInfo(STATE_CLASS.TRANSFORM, (uint)D3DTRANSFORMSTATETYPE.WORLD, "WORLD"),
            new StateInfo(STATE_CLASS.TRANSFORM, (uint)D3DTRANSFORMSTATETYPE.TEXTURE0, "TEXTURE0"),
            /* Material */
            new StateInfo(STATE_CLASS.MATERIAL, (uint)MATERIAL_TYPE.DIFFUSE, "MaterialDiffuse"),
            new StateInfo(STATE_CLASS.MATERIAL, (uint)MATERIAL_TYPE.AMBIENT, "MaterialAmbient"), /* 0x80 */
            new StateInfo(STATE_CLASS.MATERIAL, (uint)MATERIAL_TYPE.SPECULAR, "MaterialSpecular"),
            new StateInfo(STATE_CLASS.MATERIAL, (uint)MATERIAL_TYPE.EMISSIVE, "MaterialEmissive"),
            new StateInfo(STATE_CLASS.MATERIAL, (uint)MATERIAL_TYPE.POWER, "MaterialPower"),
            /* Light */
            new StateInfo(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.TYPE, "LightType"),
            new StateInfo(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.DIFFUSE, "LightDiffuse"),
            new StateInfo(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.SPECULAR, "LightSpecular"),
            new StateInfo(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.AMBIENT, "LightAmbient"),
            new StateInfo(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.POSITION, "LightPosition"),
            new StateInfo(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.DIRECTION, "LightDirection"),
            new StateInfo(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.RANGE, "LightRange"),
            new StateInfo(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.FALLOFF, "LightFallOff"),
            new StateInfo(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.ATTENUATION0, "LightAttenuation0"),
            new StateInfo(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.ATTENUATION1, "LightAttenuation1"),
            new StateInfo(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.ATTENUATION2, "LightAttenuation2"),
            new StateInfo(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.THETA, "LightTheta"),
            new StateInfo(STATE_CLASS.LIGHT, (uint)LIGHT_TYPE.PHI, "LightPhi"), /* 0x90 */
            /* Ligthenable */
            new StateInfo(STATE_CLASS.LIGHTENABLE, 0, "LightEnable"),
            /* Vertexshader */
            new StateInfo(STATE_CLASS.VERTEXSHADER, 0, "Vertexshader"),
            /* Pixelshader */
            new StateInfo(STATE_CLASS.PIXELSHADER, 0, "Pixelshader"),
            /* Shader constants */
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSFLOAT, "VertexShaderConstantF"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSBOOL, "VertexShaderConstantB"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSINT, "VertexShaderConstantI"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSFLOAT, "VertexShaderConstant"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSFLOAT, "VertexShaderConstant1"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSFLOAT, "VertexShaderConstant2"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSFLOAT, "VertexShaderConstant3"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.VSFLOAT, "VertexShaderConstant4"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSFLOAT, "PixelShaderConstantF"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSBOOL, "PixelShaderConstantB"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSINT, "PixelShaderConstantI"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSFLOAT, "PixelShaderConstant"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSFLOAT, "PixelShaderConstant1"), /* 0xa0 */
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSFLOAT, "PixelShaderConstant2"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSFLOAT, "PixelShaderConstant3"),
            new StateInfo(STATE_CLASS.SHADERCONST, (uint)SHADER_CONSTANT_TYPE.PSFLOAT, "PixelShaderConstant4"),
            /* Texture */
            new StateInfo(STATE_CLASS.TEXTURE, 0, "Texture"),
            /* Sampler states */
            new StateInfo(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.ADDRESSU, "AddressU"),
            new StateInfo(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.ADDRESSV, "AddressV"),
            new StateInfo(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.ADDRESSW, "AddressW"),
            new StateInfo(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.BORDERCOLOR, "BorderColor"),
            new StateInfo(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.MAGFILTER, "MagFilter"),
            new StateInfo(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.MINFILTER, "MinFilter"),
            new StateInfo(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.MIPFILTER, "MipFilter"),
            new StateInfo(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.MIPMAPLODBIAS, "MipMapLodBias"),
            new StateInfo(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.MAXMIPLEVEL, "MaxMipLevel"),
            new StateInfo(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.MAXANISOTROPY, "MaxAnisotropy"),
            new StateInfo(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.SRGBTEXTURE, "SRGBTexture"),
            new StateInfo(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.ELEMENTINDEX, "ElementIndex"), /* 0xb0 */
            new StateInfo(STATE_CLASS.SAMPLERSTATE, (uint)D3DSAMPLERSTATETYPE.DMAPOFFSET, "DMAPOffset"),
            /* Set sampler */
            new StateInfo(STATE_CLASS.SETSAMPLER, 0, "Sampler"),
        };

        static public EffectParameterClass ToXNAParameterClass(D3DXPARAMETER_CLASS class_)
        {
            switch (class_)
            {
                case D3DXPARAMETER_CLASS.SCALAR:
                    return EffectParameterClass.Scalar;
                case D3DXPARAMETER_CLASS.VECTOR:
                    return EffectParameterClass.Vector;
                case D3DXPARAMETER_CLASS.MATRIX_ROWS:
                case D3DXPARAMETER_CLASS.MATRIX_COLUMNS:
                    return EffectParameterClass.Matrix;
                case D3DXPARAMETER_CLASS.OBJECT:
                    return EffectParameterClass.Object;
                case D3DXPARAMETER_CLASS.STRUCT:
                    return EffectParameterClass.Struct;
                default:
                    throw new NotImplementedException();
            }
        }

        static public EffectParameterType ToXNAParameterType(D3DXPARAMETER_TYPE type)
        {
            switch (type)
            {
                case D3DXPARAMETER_TYPE.BOOL:
                    return EffectParameterType.Bool;
                case D3DXPARAMETER_TYPE.INT:
                    return EffectParameterType.Int32;
                case D3DXPARAMETER_TYPE.FLOAT:
                    return EffectParameterType.Single;
                case D3DXPARAMETER_TYPE.STRING:
                    return EffectParameterType.String;
                case D3DXPARAMETER_TYPE.TEXTURE:
                    return EffectParameterType.Texture;
                case D3DXPARAMETER_TYPE.TEXTURE1D:
                    return EffectParameterType.Texture1D;
                case D3DXPARAMETER_TYPE.TEXTURE2D:
                    return EffectParameterType.Texture2D;
                case D3DXPARAMETER_TYPE.TEXTURE3D:
                    return EffectParameterType.Texture3D;
                case D3DXPARAMETER_TYPE.TEXTURECUBE:
                    return EffectParameterType.TextureCube;
                default:
                    throw new NotImplementedException();
            }
        }

        static internal VertexElementUsage ToXNAVertexElementUsage(MojoShader.MOJOSHADER_usage usage)
        {
            switch (usage)
            {
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_POSITION:
                    return VertexElementUsage.Position;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_BLENDWEIGHT:
                    return VertexElementUsage.BlendWeight;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_BLENDINDICES:
                    return VertexElementUsage.BlendIndices;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_NORMAL:
                    return VertexElementUsage.Normal;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_POINTSIZE:
                    return VertexElementUsage.PointSize;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_TEXCOORD:
                    return VertexElementUsage.TextureCoordinate;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_TANGENT:
                    return VertexElementUsage.Tangent;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_BINORMAL:
                    return VertexElementUsage.Binormal;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_TESSFACTOR:
                    return VertexElementUsage.TessellateFactor;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_COLOR:
                    return VertexElementUsage.Color;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_FOG:
                    return VertexElementUsage.Fog;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_DEPTH:
                    return VertexElementUsage.Depth;
                case MojoShader.MOJOSHADER_usage.MOJOSHADER_USAGE_SAMPLE:
                    return VertexElementUsage.Sample;

                default:
                    throw new NotImplementedException();
            }
        }


        static public EffectObject CompileEffect(ShaderResult shaderResult, out string errorsAndWarnings)
        {
            var effect = new EffectObject();
            errorsAndWarnings = string.Empty;

            // These are filled out as we process stuff.
            effect.ConstantBuffers = new List<ConstantBufferData>();
            effect.Shaders = new List<ShaderData>();

            // Go thru the techniques and that will find all the 
            // shaders and constant buffers.
            var shaderInfo = shaderResult.ShaderInfo;
            effect.Techniques = new D3Dx_technique[shaderInfo.Techniques.Count];
            for (var t = 0; t < shaderInfo.Techniques.Count; t++)
            {
                var tinfo = shaderInfo.Techniques[t];

                var technique = new D3Dx_technique
                {
                    name = tinfo.name,
                    pass_count = (uint)tinfo.Passes.Count,
                    pass_handles = new D3Dx_pass[tinfo.Passes.Count]
                };

                for (var p = 0; p < tinfo.Passes.Count; p++)
                {
                    var pinfo = tinfo.Passes[p];

                    var pass = new D3Dx_pass
                    {
                        name = pinfo.name ?? string.Empty,

                        blendState = pinfo.blendState,
                        depthStencilState = pinfo.depthStencilState,
                        rasterizerState = pinfo.rasterizerState,

                        state_count = 0
                    };
                    var tempstate = new D3Dx_state[2];

                    shaderResult.Profile.ValidateShaderModels(pinfo);

                    if (!string.IsNullOrEmpty(pinfo.psFunction))
                    {
                        pass.state_count += 1;
                        tempstate[pass.state_count - 1] = effect.CreateShader(
                            shaderResult, pinfo.psFunction, pinfo.psModel, false, ref errorsAndWarnings);
                    }

                    if (!string.IsNullOrEmpty(pinfo.vsFunction))
                    {
                        pass.state_count += 1;
                        tempstate[pass.state_count - 1] = effect.CreateShader(
                            shaderResult, pinfo.vsFunction, pinfo.vsModel, true, ref errorsAndWarnings);
                    }

                    pass.states = new D3Dx_state[pass.state_count];
                    for (var s = 0; s < pass.state_count; s++)
                        pass.states[s] = tempstate[s];

                    technique.pass_handles[p] = pass;
                }

                effect.Techniques[t] = technique;
            }

            // Make the list of parameters by combining all the
            // constant buffers ignoring the buffer offsets.
            var parameters = new List<D3Dx_parameter>();
            for (var c = 0; c < effect.ConstantBuffers.Count; c++)
            {
                var cb = effect.ConstantBuffers[c];

                for (var i = 0; i < cb.Parameters.Count; i++)
                {
                    var param = cb.Parameters[i];

                    var match = parameters.FindIndex(e => e.name == param.name);
                    if (match == -1)
                    {
                        cb.ParameterIndex.Add(parameters.Count);
                        parameters.Add(param);
                    }
                    else
                    {
                        // TODO: Make sure the type and size of 
                        // the parameter match up!
                        cb.ParameterIndex.Add(match);
                    }
                }
            }

            // Add the texture parameters from the samplers.
            foreach (var shader in effect.Shaders)
            {
                for (var s = 0; s < shader._samplers.Length; s++)
                {
                    var sampler = shader._samplers[s];

                    var match = parameters.FindIndex(e => e.name == sampler.parameterName);
                    if (match == -1)
                    {
                        // Store the index for runtime lookup.
                        shader._samplers[s].parameter = parameters.Count;

                        var param = new D3Dx_parameter
                        {
                            class_ = D3DXPARAMETER_CLASS.OBJECT,
                            name = sampler.parameterName,
                            semantic = string.Empty
                        };

                        switch (sampler.type)
                        {
                            case MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_1D:
                                param.type = D3DXPARAMETER_TYPE.TEXTURE1D;
                                break;

                            case MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_2D:
                                param.type = D3DXPARAMETER_TYPE.TEXTURE2D;
                                break;

                            case MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_VOLUME:
                                param.type = D3DXPARAMETER_TYPE.TEXTURE3D;
                                break;

                            case MojoShader.MOJOSHADER_samplerType.MOJOSHADER_SAMPLER_CUBE:
                                param.type = D3DXPARAMETER_TYPE.TEXTURECUBE;
                                break;
                        }

                        parameters.Add(param);
                    }
                    else
                    {
                        // TODO: Make sure the type and size of 
                        // the parameter match up!

                        shader._samplers[s].parameter = match;
                    }
                }
            }

            // TODO: Annotations are part of the .FX format and
            // not a part of shaders... we need to implement them
            // in our mgfx parser if we want them back.

            effect.Parameters = parameters.ToArray();

            return effect;
        }


        private D3Dx_state CreateShader(
            ShaderResult shaderResult, string shaderFunction, string shaderProfile,
            bool isVertexShader, ref string errorsAndWarnings)
        {
            // Compile and create the shader.
            var shaderData = shaderResult.Profile.CreateShader(
                shaderResult, shaderFunction, shaderProfile, isVertexShader, this, ref errorsAndWarnings);

            var state = new D3Dx_state
            {
                index = 0,
                type = STATE_TYPE.CONSTANT,
                operation = isVertexShader ? 146 : (uint)147,

                parameter = new D3Dx_parameter
                {
                    name = string.Empty,
                    semantic = string.Empty,
                    class_ = D3DXPARAMETER_CLASS.OBJECT,
                    type = isVertexShader ? D3DXPARAMETER_TYPE.VERTEXSHADER : D3DXPARAMETER_TYPE.PIXELSHADER,
                    rows = 0,
                    columns = 0,
                    data = shaderData.SharedIndex
                }
            };

            return state;
        }

        internal static int GetShaderIndex(STATE_CLASS type, D3Dx_state[] states)
        {
            foreach (var state in states)
            {
                var operation = state_table[state.operation];
                if (operation.Class != type)
                    continue;

                if (state.type != STATE_TYPE.CONSTANT)
                    throw new NotSupportedException("We do not support shader expressions!");

                return (int)state.parameter.data;
            }

            return -1;
        }

        public D3Dx_parameter[] Objects { get; private set; }

        public D3Dx_parameter[] Parameters { get; private set; }

        public D3Dx_technique[] Techniques { get; private set; }

        public List<ShaderData> Shaders { get; private set; }

        public List<ConstantBufferData> ConstantBuffers { get; private set; }
    }
}

