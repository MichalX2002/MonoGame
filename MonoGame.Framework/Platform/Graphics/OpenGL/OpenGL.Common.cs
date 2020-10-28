using System;
using System.Diagnostics;
using MonoGame.Framework.Graphics;

namespace MonoGame.OpenGL
{
    // Required to allow platforms other than iOS use the same code.
    // just don't include this on iOS
    [AttributeUsage(AttributeTargets.Delegate)]
    internal sealed class MonoNativeFunctionWrapper : Attribute
    {
    }

    internal static class GLEnumExtensions
    {
        public static IndexElementType ToGLElementType(this Framework.Graphics.IndexElementType elementType)
        {
            switch (elementType)
            {
                case Framework.Graphics.IndexElementType.Int16:
                    return IndexElementType.UnsignedShort;

                case Framework.Graphics.IndexElementType.Int32:
                    return IndexElementType.UnsignedInt;

                default:
                    throw new ArgumentOutOfRangeException(nameof(elementType));
            }
        }
    }

    public static partial class GL
    {
        public static int GetBoundTexture2D()
        {
            GetInteger(GetPName.TextureBinding2D, out int prevTexture);
            LogError("GraphicsExtensions.GetBoundTexture2D() GL.GetInteger");
            return prevTexture;
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        public static void CheckError()
        {
            var error = GetError();
            if (error != ErrorCode.NoError)
            {
                Debug.WriteLine(error);
                throw new PlatformGraphicsException("GL.GetError() returned " + error);
            }
        }

        [Conditional("DEBUG")]
        public static void LogError(string location)
        {
            try
            {
                CheckError();
            }
            catch (PlatformGraphicsException ex)
            {
#if ANDROID
                // Todo: Add generic MonoGame logging interface
                Android.Util.Log.Debug("MonoGame", "MonoGameGLException at " + location + " - " + ex.Message);
#else
                Debug.WriteLine("MonoGameGLException at " + location + " - " + ex.Message);
#endif
            }
        }
    }
}