using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Utilities
{
    internal class TaskbarList
    {
        private ITaskbarList _comObject;
        private IntPtr _windowHandle;

        public TaskbarList(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(windowHandle));

            _windowHandle = windowHandle;

            _comObject = (ITaskbarList)new COMTaskbarList();
            _comObject.HrInit();
        }

        public void SetProgressState(TaskbarProgressState state)
        {
            _comObject.SetProgressState(_windowHandle, state);
        }

        public void SetProgressValue(TaskbarProgressValue value)
        {
            _comObject.SetProgressValue(_windowHandle, (ulong)value.Completed, (ulong)value.Total);
        }

        [ComImport]
        [Guid("c43dc798-95d1-4bea-9030-bb99e2983a1a")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ITaskbarList
        {
            [PreserveSig]
            void HrInit();
            [PreserveSig]
            void AddTab(IntPtr hwnd);
            [PreserveSig]
            void DeleteTab(IntPtr hwnd);
            [PreserveSig]
            void ActivateTab(IntPtr hwnd);
            [PreserveSig]
            void SetActiveAlt(IntPtr hwnd);

            [PreserveSig]
            void MarkFullscreenWindow(
                IntPtr hwnd,
                [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

            [PreserveSig]
            void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
            [PreserveSig]
            void SetProgressState(IntPtr hwnd, TaskbarProgressState tbpFlags);
            [PreserveSig]
            void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
            [PreserveSig]
            void UnregisterTab(IntPtr hwndTab);
            [PreserveSig]
            void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
            [PreserveSig]
            void SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, uint dwReserved);

            [PreserveSig]
            void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);

            [PreserveSig]
            void SetOverlayIcon(
              IntPtr hwnd,
              IntPtr hIcon,
              [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);

            [PreserveSig]
            void SetThumbnailTooltip(
                IntPtr hwnd,
                [MarshalAs(UnmanagedType.LPWStr)] string pszTip);

            [PreserveSig]
            void SetThumbnailClip(IntPtr hwnd, IntPtr prcClip);
        }

        [ComImport]
        [Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
        [ClassInterface(ClassInterfaceType.None)]
        private class COMTaskbarList
        {
        }
    }
}
