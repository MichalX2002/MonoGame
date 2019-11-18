// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Input;
using MonoGame.Framework.Input.Touch;
using System;
using System.ComponentModel;

#if DIRECTX || DESKTOPGL
using MonoGame.Framework.Utilities;
#endif

namespace MonoGame.Framework
{
	public abstract class GameWindow
	{
		public delegate void StateChangedDelegate();

		private string _title;
		internal bool _allowAltF4 = true;

		internal MouseState MouseState;
		internal TouchPanelState TouchPanelState;

		#region Properties

		[DefaultValue(false)]
		public abstract bool AllowUserResizing { get; set; }
		public abstract Rectangle ClientBounds { get; }

		public abstract bool HasClipboardText { get; }
		public abstract string ClipboardText { get; set; }

        public TaskbarList TaskbarList { get; }

        /// <summary>
        /// Gets or sets whether the usage of Alt+F4 closes the window on desktop platforms. 
        /// Set to <see langword="true"/> by default.
        /// </summary>
        public virtual bool AllowAltF4 { get => _allowAltF4; set => _allowAltF4 = value; }

#if (WINDOWS && !WINDOWS_UAP) || DESKTOPGL
		/// <summary>
		/// The location of this window on the desktop, 
        /// i.e. global coordinate space which stretches across all screens.
		/// </summary>
		public abstract Point Position { get; set; }
#endif

		public abstract DisplayOrientation CurrentOrientation { get; }
		public abstract IntPtr Handle { get; }
		public abstract string ScreenDeviceName { get; }

		/// <summary>
		/// Gets or sets the title of the game window.
		/// </summary>
		/// <remarks>
		/// For Windows 8 and Windows 10 UWP this has no effect. For these platforms the title should be
		/// set by using the DisplayName property found in the app manifest file.
		/// </remarks>
		public string Title
		{
			get => _title;
			set
			{
				if (_title != value)
				{
					SetTitle(value);
					_title = value;
				}
			}
		}

        /// <summary>
        /// <para>
        /// Determines whether the border of the window is visible.
        /// </para>
        /// Currently only supported on the WinDX and WinGL/Linux platforms.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Set on a platform other than the WinDX and WinGL/Linux platforms.
        /// </exception>
        public virtual bool IsBorderless
		{
			get => false;
			set => throw new NotImplementedException();
		}

		#endregion

		protected GameWindow()
		{
			TouchPanelState = new TouchPanelState(this);
            TaskbarList = new TaskbarList(this);
        }

		#region Events

		public event SimpleEventHandler<GameWindow> ClientSizeChanged;
		public event SimpleEventHandler<GameWindow> OrientationChanged;
		public event SimpleEventHandler<GameWindow> ScreenDeviceNameChanged;

#if WINDOWS || WINDOWS_UAP || DESKTOPGL || ANGLE

		/// <summary>
		/// Use this event to retrieve text for objects like textboxes.
		/// This event is not raised by non-character keys and supports key repeat.
		/// For more information this event is based off:
		/// http://msdn.microsoft.com/en-AU/library/system.windows.forms.control.keypress.aspx
		/// </summary>
		/// <remarks>
		/// This event is only supported on the Windows DirectX, Windows OpenGL and Linux platforms.
		/// </remarks>
		public event DataEventHandler<GameWindow, TextInputEvent> TextInput;

        internal bool IsTextInputHandled => TextInput != null;

        /// <summary>
        /// Buffered keyboard KeyDown event.
        /// </summary>
		public event DataEventHandler<GameWindow, KeyInputEvent> KeyDown;

        /// <summary>
        /// Buffered keyboard KeyUp event.
        /// </summary>
        public event DataEventHandler<GameWindow, KeyInputEvent> KeyUp;

#endif

        #endregion Events

        public abstract void BeginScreenDeviceChange(bool willBeFullScreen);

		public abstract void EndScreenDeviceChange(
			string screenDeviceName, int clientWidth, int clientHeight);

		public void EndScreenDeviceChange(string screenDeviceName)
		{
			EndScreenDeviceChange(screenDeviceName, ClientBounds.Width, ClientBounds.Height);
		}

		protected void OnActivated()
		{
		}

		internal void OnClientSizeChanged()
		{
			ClientSizeChanged?.Invoke(this);
		}

		protected void OnDeactivated()
		{
		}

		protected void OnOrientationChanged()
		{
			OrientationChanged?.Invoke(this);
		}

		protected void OnPaint()
		{
		}

		protected void OnScreenDeviceNameChanged()
		{
			ScreenDeviceNameChanged?.Invoke(this);
		}

#if WINDOWS || WINDOWS_UAP || DESKTOPGL || ANGLE

        internal void OnTextInput(in TextInputEvent ev) => TextInput?.Invoke(this, ev);

        internal void OnKeyDown(in KeyInputEvent e) => KeyDown?.Invoke(this, e);

        internal void OnKeyUp(in KeyInputEvent e) => KeyUp?.Invoke(this, e);

#endif

        protected internal abstract void SetSupportedOrientations(DisplayOrientation orientations);
		protected abstract void SetTitle(string title);

#if DIRECTX && WINDOWS
		public static GameWindow Create(Game game, int width, int height)
		{
			var window = new WinFormsGameWindow((WinFormsGamePlatform)game.Platform);
			window.Initialize(width, height);

			return window;
		}
#endif
	}
}