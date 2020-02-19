// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Windows.Media.Imaging;
using Eto.Drawing;
using Eto.Wpf.Drawing;
using MonoGame.Framework.Memory;

namespace MonoGame.Tools.Pipeline
{
    static partial class Global
    {
        private static void PlatformInit()
        {

        }

        private static System.Drawing.Bitmap PlatformGetFileIcon(string path)
        {
            return System.Drawing.Icon.ExtractAssociatedIcon(path).ToBitmap();
        }

        private static Bitmap ToEtoImage(System.Drawing.Bitmap bitmap)
        {
            var ret = new BitmapImage();

            using (var stream = RecyclableMemoryManager.Default.GetMemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;

                ret.BeginInit();
                ret.DecodePixelHeight = 16;
                ret.DecodePixelWidth = 16;
                ret.StreamSource = stream;
                ret.CacheOption = BitmapCacheOption.OnLoad;
                ret.EndInit();
            }

            return new Bitmap(new BitmapHandler(ret));
        }
    }
}

