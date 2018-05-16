// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

#if WINDOWS_UAP
using Windows.UI.Xaml.Controls;
#endif

#if IOS
using UIKit;
using Microsoft.Xna.Framework.Input.Touch;
#endif

namespace Microsoft.Xna.Framework.Graphics
{
    public class PresentationParameters
    {
        public const int DefaultPresentRate = 60;
        
        /// <summary>
        /// Create a <see cref="PresentationParameters"/> instance with default values for all properties.
        /// </summary>
        public PresentationParameters()
        {
            Clear();
        }

        #region Properties

        /// <summary>
        /// Get or set the format of the back buffer.
        /// </summary>
        public SurfaceFormat BackBufferFormat { get; set; }

        /// <summary>
        /// Get or set the height of the back buffer.
        /// </summary>
        public int BackBufferHeight { get; set; } = GraphicsDeviceManager.DefaultBackBufferHeight;

        /// <summary>
        /// Get or set the width of the back buffer.
        /// </summary>
        public int BackBufferWidth { get; set; } = GraphicsDeviceManager.DefaultBackBufferWidth;

        /// <summary>
        /// Get the bounds of the back buffer.
        /// </summary>
        public Rectangle Bounds 
        {
            get { return new Rectangle(0, 0, BackBufferWidth, BackBufferHeight); }
        }

        /// <summary>
        /// Get or set the handle of the window that will present the back buffer.
        /// </summary>
        public IntPtr DeviceWindowHandle { get; set; }

#if WINDOWS_UAP
        [CLSCompliant(false)]
        public SwapChainPanel SwapChainPanel { get; set; }
#endif

        /// <summary>
        /// Get or set the depth stencil format for the back buffer.
        /// </summary>
		public DepthFormat DepthStencilFormat { get; set; }

        /// <summary>
        /// Get or set a value indicating if we are in full screen mode.
        /// </summary>
        public bool IsFullScreen { get; set; }

        /// <summary>
        /// If <code>true</code> the <see cref="GraphicsDevice"/> will do a mode switch
        /// when going to full screen mode. If <code>false</code> it will instead do a
        /// soft full screen by maximizing the window and making it borderless.
        /// </summary>
        public bool HardwareModeSwitch { get; set; } = true;

        /// <summary>
        /// Get or set the multisample count for the back buffer.
        /// </summary>
        public int MultiSampleCount { get; set; }

        /// <summary>
        /// Get or set the presentation interval.
        /// </summary>
        public PresentInterval PresentationInterval { get; set; }

        /// <summary>
        /// Get or set the display orientation.
        /// </summary>
		public DisplayOrientation DisplayOrientation 
		{ 
			get; 
			set; 
		}
		
        /// <summary>
        /// Get or set the RenderTargetUsage for the back buffer.
        /// Determines if the back buffer is cleared when it is set as the
        /// render target by the <see cref="GraphicsDevice"/>.
        /// <see cref="GraphicsDevice"/> target.
        /// </summary>
		public RenderTargetUsage RenderTargetUsage { get; set; }

        #endregion Properties
        
        #region Methods

        /// <summary>
        /// Reset all properties to their default values.
        /// </summary>
        public void Clear()
        {
            BackBufferFormat = SurfaceFormat.Color;
#if IOS
			// Mainscreen.Bounds does not account for the device's orientation. it ALWAYS assumes portrait
			var width = (int)(UIScreen.MainScreen.Bounds.Width * UIScreen.MainScreen.Scale);
			var height = (int)(UIScreen.MainScreen.Bounds.Height * UIScreen.MainScreen.Scale);
			
			// Flip the dimensions if we need to.
			if (TouchPanel.DisplayOrientation == DisplayOrientation.LandscapeLeft ||
			    TouchPanel.DisplayOrientation == DisplayOrientation.LandscapeRight)
			{
				width = height;
				height = (int)(UIScreen.MainScreen.Bounds.Width * UIScreen.MainScreen.Scale);
			}
			
			backBufferWidth = width;
            backBufferHeight = height;
#else
            BackBufferWidth = GraphicsDeviceManager.DefaultBackBufferWidth;
            BackBufferHeight = GraphicsDeviceManager.DefaultBackBufferHeight;     
#endif
            DeviceWindowHandle = IntPtr.Zero;
#if IOS && !TVOS
			isFullScreen = UIApplication.SharedApplication.StatusBarHidden;
#else
            // isFullScreen = false;
#endif
            DepthStencilFormat = DepthFormat.None;
            MultiSampleCount = 0;
            PresentationInterval = PresentInterval.Default;
            DisplayOrientation = Microsoft.Xna.Framework.DisplayOrientation.Default;
        }

        /// <summary>
        /// Create a copy of this <see cref="PresentationParameters"/> instance.
        /// </summary>
        /// <returns></returns>
        public PresentationParameters Clone()
        {
            return new PresentationParameters
            {
                BackBufferFormat = this.BackBufferFormat,
                BackBufferHeight = this.BackBufferHeight,
                BackBufferWidth = this.BackBufferWidth,
                DeviceWindowHandle = this.DeviceWindowHandle,
                DepthStencilFormat = this.DepthStencilFormat,
                IsFullScreen = this.IsFullScreen,
                HardwareModeSwitch = this.HardwareModeSwitch,
                MultiSampleCount = this.MultiSampleCount,
                PresentationInterval = this.PresentationInterval,
                DisplayOrientation = this.DisplayOrientation,
                RenderTargetUsage = this.RenderTargetUsage
            };
        }

        #endregion Methods

    }
}
