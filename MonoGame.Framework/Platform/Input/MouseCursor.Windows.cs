// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MonoGame.Framework.PackedVector;

namespace MonoGame.Framework.Input
{
    public partial class MouseCursor
    {
        bool _needsDisposing;

        internal Cursor Cursor { get; private set; }

        private MouseCursor(Cursor cursor, bool needsDisposing = false)
        {
            Cursor = cursor;
            _needsDisposing = needsDisposing;
        }

        private static void PlatformInitalize()
        {
            Arrow = new MouseCursor(Cursors.Arrow);
            IBeam = new MouseCursor(Cursors.IBeam);
            Wait = new MouseCursor(Cursors.WaitCursor);
            Crosshair = new MouseCursor(Cursors.Cross);
            WaitArrow = new MouseCursor(Cursors.AppStarting);
            SizeNWSE = new MouseCursor(Cursors.SizeNWSE);
            SizeNESW = new MouseCursor(Cursors.SizeNESW);
            SizeWE = new MouseCursor(Cursors.SizeWE);
            SizeNS = new MouseCursor(Cursors.SizeNS);
            SizeAll = new MouseCursor(Cursors.SizeAll);
            No = new MouseCursor(Cursors.No);
            Hand = new MouseCursor(Cursors.Hand);
        }

        private static unsafe MouseCursor PlatformFromPixels(
            ReadOnlySpan<Color> data, int width, int height, Point origin)
        {
            // bitmaps can not be constructed from Rgba directly
            using (var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                var rect = new System.Drawing.Rectangle(0, 0, width, height);
                var bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                try
                {
                    for (int y = 0; y < bmpData.Height; y++)
                    {
                        var srcRow = data.Slice(y * width);
                        var dstRow = new Span<Bgra32>((byte*)bmpData.Scan0 + y * bmpData.Stride, bmpData.Width);
                        
                        for (int x = 0; x < width; x++)
                            dstRow[x].FromColor(srcRow[x]);
                    }
                }
                finally
                {
                    bitmap.UnlockBits(bmpData);
                }

                var iconInfo = new IconInfo();
                GetIconInfo(bitmap.GetHicon(), ref iconInfo);

                iconInfo.xHotspot = origin.X;
                iconInfo.yHotspot = origin.Y;
                iconInfo.fIcon = false;

                var cursor = new Cursor(CreateIconIndirect(ref iconInfo));
                return new MouseCursor(cursor, needsDisposing: true);
            }
        }

        private void PlatformDispose()
        {
            if (_needsDisposing && Cursor != null)
            {
                Cursor.Dispose();
                Cursor = null;
                _needsDisposing = false;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr MaskBitmap;
            public IntPtr ColorBitmap;
        };

        [DllImport("user32.dll")]
        static extern IntPtr CreateIconIndirect([In] ref IconInfo iconInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
    }
}
