#region License
/*
Microsoft Public License (Ms-PL)
MonoGame - Copyright © 2012 The MonoGame Team

All rights reserved.

This license governs use of the accompanying software. If you use the software, you accept this license. If you do not
accept the license, do not use the software.

1. Definitions
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under 
U.S. copyright law.

A "contribution" is the original software, or any additions or changes to the software.
A "contributor" is any person that distributes its contribution under this license.
"Licensed patents" are a contributor's patent claims that read directly on its contribution.

2. Grant of Rights
(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

3. Conditions and Limitations
(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
your patent license from such contributor to the software ends automatically.
(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution 
notices that are present in the software.
(D) If you distribute any portion of the software in source code form, you may do so only under this license by including 
a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object 
code form, you may only do so under a license that complies with this license.
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees
or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent
permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular
purpose and non-infringement.
*/
#endregion License

using System;
using MonoGame.Framework.Content;
using MonoGame.Framework.Graphics;

namespace MonoGame.Framework
{
    /// <summary>
    /// A default implementation of IResumeManager. 
    /// Loads a user specified image and draws it the middle of the screen.
    /// 
    /// Example usage in Game.Initialize():
    /// 
    /// #if ANDROID
    ///    this.Window.SetResumer(new ResumeManager(
    ///        this.Services, 
    ///        spriteBatch, 
    ///        "UI/ResumingTexture",
    ///        1.0f, 0.01f));
    /// #endif                                         
    /// </summary>
    public class ResumeManager : IResumeManager
    {
        private ContentManager _content;
        private GraphicsDevice _device;
        private SpriteBatch _spriteBatch;
        private string _resumeTextureName;
        private Texture2D _resumeTexture;
        private float _rotation;
        private float _scale;
        private float _rotateSpeed;

        public ResumeManager(
            IServiceProvider services,
            SpriteBatch spriteBatch,
            string resumeTextureName,
            float scale,
            float rotateSpeed)
        {
            _content = new ContentManager(services, "Content");
            _device = services.GetService<IGraphicsDeviceService>().GraphicsDevice;
            _spriteBatch = spriteBatch;
            _resumeTextureName = resumeTextureName;
            _scale = scale;
            _rotateSpeed = rotateSpeed;
        }

        public virtual void LoadContent()
        {
            _content.Unload();
            _resumeTexture = _content.Load<Texture2D>(_resumeTextureName);
        }

        public virtual void Draw()
        {
            _rotation += _rotateSpeed;

            int sw = _device.PresentationParameters.BackBufferWidth;
            int sh = _device.PresentationParameters.BackBufferHeight;
            int tw = _resumeTexture.Width;
            int th = _resumeTexture.Height;

            // Draw the resume texture in the middle of the screen and make it spin
            _spriteBatch.Begin();

            _spriteBatch.Draw(
                _resumeTexture,
                new Vector2(sw, sh) / 2,
                null, Color.White, _rotation,
                new Vector2(tw, th) / 2,
                _scale, SpriteEffects.None, 0f);

            _spriteBatch.End();
        }
    }
}