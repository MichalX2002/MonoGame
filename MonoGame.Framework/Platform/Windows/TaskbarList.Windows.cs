using System;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Utilities
{
    public partial class TaskbarList
    {
        private ITaskbarList? _comObject;
        private IntPtr _windowHandle;

        private static bool PlatformGetIsSupported()
        {
            return PlatformInfo.OS != PlatformInfo.OperatingSystem.Windows;
        }

        private void PlatformConstruct()
        {
            if (!PlatformGetIsSupported())
                return;

            if (_comObject == null)
            {
                _comObject = (ITaskbarList)new COMTaskbarList();
                _comObject.HrInit();
            }
        }

        private void PlatformWindowHandleChanged()
        {
            if (_comObject == null)
                return;

            _windowHandle = Window.GetSubsystemWindowHandle();
        }

        private bool PlatformGetIsAvailable()
        {
            return _windowHandle != IntPtr.Zero;
        }

        private void PlatformSetProgressState(TaskbarProgressState state)
        {
            int flags = state switch
            {
                TaskbarProgressState.None => 0,
                TaskbarProgressState.Indeterminate => 0x1,
                TaskbarProgressState.Normal => 0x2,
                TaskbarProgressState.Error => 0x4,
                TaskbarProgressState.Paused => 0x8,
                _ => throw new ArgumentOutOfRangeException(nameof(state))
            };
            _comObject!.SetProgressState(_windowHandle, flags);
        }

        private void PlatformSetProgressValue(TaskbarProgressValue value)
        {
            _comObject!.SetProgressValue(_windowHandle, (ulong)value.Completed, (ulong)value.Total);
        }

        #region COM Interface

        [ComImport]
        [Guid("c43dc798-95d1-4bea-9030-bb99e2983a1a")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ITaskbarList
        {
            #region ITaskbarList
            [PreserveSig] void HrInit();
            [PreserveSig] void AddTab(IntPtr hwnd);
            [PreserveSig] void DeleteTab(IntPtr hwnd);
            [PreserveSig] void ActivateTab(IntPtr hwnd);
            [PreserveSig] void SetActiveAlt(IntPtr hwnd);
            #endregion

            #region ITaskbarList2
            [PreserveSig]
            void MarkFullscreenWindow(
                IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);
            #endregion

            #region ITaskbarList3
            [PreserveSig] void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);
            [PreserveSig] void SetProgressState(IntPtr hwnd, int tbpFlags);

            [PreserveSig] void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
            [PreserveSig] void UnregisterTab(IntPtr hwndTab);
            [PreserveSig] void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
            [PreserveSig] void SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, uint dwReserved);

            [PreserveSig] void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);

            [PreserveSig]
            void SetOverlayIcon(
              IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);

            [PreserveSig]
            void SetThumbnailTooltip(
                IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);

            [PreserveSig] void SetThumbnailClip(IntPtr hwnd, IntPtr prcClip);
            #endregion
        }

        [ComImport]
        [Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
        [ClassInterface(ClassInterfaceType.None)]
        private class COMTaskbarList
        {
        }

        #endregion
    }
}
