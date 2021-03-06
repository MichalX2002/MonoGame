// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MonoGame.Framework.Input;
using MonoGame.Framework.Input.Touch;
using MonoGame.Framework.Memory;
using MonoGame.Framework.Utilities;

namespace MonoGame.Framework
{
    public abstract class GameWindow : IDisposable
    {
        public delegate void TextEditingEvent(GameWindow sender, TextEditingEventArgs eventArgs);
        public delegate void TextInputEvent(GameWindow window, TextInputEventArgs textInput);

        private string _title;
        private bool _allowAltF4 = true;

        #region Events

        public event Event<GameWindow>? Disposing;
        public event Event<GameWindow>? Disposed;

        /// <summary>
        /// Occurs when the size of the window changes.
        /// </summary>
        public event Event<GameWindow>? SizeChanged;

        public event Event<GameWindow>? OrientationChanged;
        public event Event<GameWindow>? ScreenDeviceNameChanged;
        public event Event<GameWindow>? WindowHandleChanged;

        /// <summary>
        /// Occurs when the user is dragging an object over the window.
        /// </summary>
        /// <remarks>
        /// This event is only supported on Windows.
        /// </remarks>
        public event DataEvent<GameWindow, DragOverEventArgs>? DragOver;

        /// <summary>
        ///  Occurs when the user drops files on the window.
        /// </summary>
        /// <remarks>
        /// This event is only supported on Windows, Linux, and MacOS.
        /// </remarks>
        public event DataEvent<GameWindow, FilesDroppedEventArgs>? FilesDropped;

        /// <summary>
        ///  Occurs when the user drops files on the window.
        /// </summary>
        /// <remarks>
        /// This event is only supported on Windows, Linux, and MacOS.
        /// </remarks>
        public event DataEvent<GameWindow, TextDroppedEventArgs>? TextDropped;

        /// <summary>
        /// Use this event to draw the current text input.
        /// This event is not raised by non-character.
        /// </summary>
        /// <remarks>
        /// This event is only supported on Windows, Linux, and MacOS.
        /// </remarks>
        public event TextEditingEvent? TextEditing;

        /// <summary>
        /// Use this event to retrieve text for objects like textboxes.
        /// This event is not raised by non-character keys and supports key repeat.
        /// For more information this event is based off:
        /// http://msdn.microsoft.com/en-AU/library/system.windows.forms.control.keypress.aspx
        /// </summary>
        /// <remarks>
        /// This event is only supported on Windows, Linux, and MacOS.
        /// </remarks>
        public event TextInputEvent? TextInput;

        /// <summary>
        /// Buffered keyboard KeyDown event.
        /// </summary>
        /// <remarks>
        /// This event is only supported on Windows, Linux, and MacOS.
        /// </remarks>
        public event TextInputEvent? KeyDown;

        /// <summary>
        /// Buffered keyboard KeyUp event.
        /// </summary>
        /// <remarks>
        /// This event is only supported on Windows, Linux, and MacOS.
        /// </remarks>
        public event TextInputEvent? KeyUp;

        #endregion

        #region Properties

        public Mouse Mouse { get; }
        public TouchPanel TouchPanel { get; }
        public TaskbarList TaskbarList { get; }

        public bool IsDisposed { get; private set; }

        public abstract bool AllowUserResizing { get; set; }
        public abstract Rectangle Bounds { get; }

        public abstract bool HasClipboardText { get; }
        public abstract string ClipboardText { get; set; }

        internal bool IsFilesDroppedHandled => FilesDropped != null;
        internal bool IsTextDroppedHandled => TextDropped != null;

        /// <summary>
        /// The location of this window on the desktop, 
        /// i.e. global coordinate space which stretches across all screens.
        /// </summary>
        public abstract Point Position { get; set; }

        /// <summary>
        /// Gets the orientation of the display.
        /// </summary>
        public abstract DisplayOrientation CurrentOrientation { get; }

        /// <summary>
        /// Gets the name of the display device.
        /// </summary>
        public abstract string ScreenDeviceName { get; }

        /// <summary>
        /// Gets or sets whether the usage of Alt+F4 closes the window on desktop platforms. 
        /// Set to <see langword="true"/> by default.
        /// </summary>
        public virtual bool AllowAltF4 { get => _allowAltF4; set => _allowAltF4 = value; }

        /// <summary>
        /// Gets or sets the title of the game window.
        /// </summary>
        /// <remarks>
        /// For UWP this has no effect. The title should be
        /// set by using the DisplayName property found in the app manifest file.
        /// </remarks>
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    SetTitle(_title);
                }
            }
        }

        /// <summary>
        /// Determines whether the border of the window is visible.
        /// </summary>
        /// <remarks>
        /// Currently only supported on Windows and Linux.
        /// </remarks>
        /// <exception cref="PlatformNotSupportedException">
        /// Set on a platform other than the Windows and Linux platforms.
        /// </exception>
        public virtual bool IsBorderless
        {
            get => false;
            set => throw new PlatformNotSupportedException();
        }

        #endregion

        /// <summary>
        /// Constructs the <see cref="GameWindow"/>.
        /// </summary>
        protected GameWindow()
        {
            _title = string.Empty;
            Mouse = new Mouse(this);
            TouchPanel = new TouchPanel(this);
            TaskbarList = TaskbarList.Create(this);
        }

        /// <summary>
        /// Gets the platform-dependent handle of this window.
        /// </summary>
        /// <remarks>
        /// To get the OS-dependent subsystem window handle use <see cref="GetSubsystemWindowHandle"/>.
        /// </remarks>
        public abstract IntPtr GetPlatformWindowHandle();

        /// <summary>
        /// Returns the OS-dependent subsystem handle for this window.
        /// </summary>
        /// <remarks>
        /// To get the platform-dependent handle use <see cref="GetPlatformWindowHandle()"/>.
        /// </remarks>
        /// <returns>The subsystem window handle.</returns>
        public abstract IntPtr GetSubsystemWindowHandle();

        public abstract void BeginScreenDeviceChange(bool willBeFullScreen);

        public abstract void EndScreenDeviceChange(string screenDeviceName, int width, int height);

        public void EndScreenDeviceChange(string screenDeviceName)
        {
            EndScreenDeviceChange(screenDeviceName, Bounds.Width, Bounds.Height);
        }

        protected void OnActivated()
        {
        }

        internal void OnSizeChanged()
        {
            SizeChanged?.Invoke(this);
        }

        protected void OnDeactivated()
        {
        }

        protected void OnOrientationChanged()
        {
            OrientationChanged?.Invoke(this);
        }

        protected void OnScreenDeviceNameChanged()
        {
            ScreenDeviceNameChanged?.Invoke(this);
        }

        internal void OnDragOver(DragOverEventArgs ev)
        {
            DragOver?.Invoke(this, ev);
        }

        internal void OnFilesDropped(FilesDroppedEventArgs ev)
        {
            FilesDropped?.Invoke(this, ev);
        }

        internal void OnTextDropped(TextDroppedEventArgs ev)
        {
            TextDropped?.Invoke(this, ev);
        }

        internal void OnTextEditing(TextEditingEventArgs ev)
        {
            TextEditing?.Invoke(this, ev);
        }

        internal void OnTextInput(TextInputEventArgs ev)
        {
            TextInput?.Invoke(this, ev);
        }

        internal void OnKeyDown(TextInputEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }

        internal void OnKeyUp(TextInputEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        internal void OnWindowHandleChanged()
        {
            WindowHandleChanged?.Invoke(this);
        }

        protected internal abstract void SetSupportedOrientations(DisplayOrientation orientations);

        protected abstract void SetTitle(ReadOnlySpan<char> title);

        public abstract void StartTextInput();

        public abstract void SetTextInputRectangle(Rectangle rectangle);

        public abstract void StopTextInput();

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="PlatformNotSupportedException">The platform does not support multiple windows.</exception>
        public static GameWindow Create(Game game, int width, int height)
        {
#if DIRECTX && WINDOWS
            var window = new WinFormsGameWindow((WinFormsGamePlatform)game.Platform);
            window.Initialize(width, height);

            return window;
#else
            throw new PlatformNotSupportedException();
#endif
        }

        // TODO: expose and add SetIcon method
        internal static RecyclableBuffer? ReadEmbeddedIconBytes(string arrayTag)
        {
            try
            {
                var stream = GetEmbeddedIconStream();
                if (stream != null)
                    return RecyclableBuffer.ReadBytes(stream, (int)stream.Length, arrayTag);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load icon from embedded resources: {0}", ex);
            }
            return null;
        }

        public static Stream? GetEmbeddedIconStream()
        {
            // when running NUnit tests entry assembly can be null
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
                return null;

            var declareSpace = assembly.EntryPoint?.DeclaringType?.Namespace;

            return (declareSpace == null ? null : assembly.GetManifestResourceStream(declareSpace + ".Icon.bmp"))
                ?? assembly.GetManifestResourceStream("Icon.bmp")
                ?? Assembly.GetExecutingAssembly().GetManifestResourceStream("MonoGame.bmp");
        }

        protected void OnDisposing()
        {
            Disposing?.Invoke(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Disposed?.Invoke(this);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the <see cref="GameWindow"/>.
        /// </summary>
        ~GameWindow()
        {
            Dispose(disposing: false);
        }
    }
}