// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto;
using Eto.Wpf.Forms;
using Eto.Wpf.Forms.Menu;
using Eto.Wpf.Forms.ToolBar;

namespace MonoGame.Tools.Pipeline
{
    public static class Styles
    {
        public static void Load()
        {
            Style.Add<MenuBarHandler>("MenuBar", h =>
            {
                h.Control.Background = System.Windows.SystemColors.ControlLightLightBrush;
            });

            Style.Add<ToolBarHandler>("ToolBar", h =>
            {
                h.Control.Background = System.Windows.SystemColors.ControlLightLightBrush;

                h.Control.Loaded += delegate
                {

                    if (h.Control.Template.FindName("OverflowGrid", h.Control) is System.Windows.FrameworkElement overflowGrid)
                        overflowGrid.Visibility = System.Windows.Visibility.Collapsed;

                    foreach (var item in h.Control.Items)
                    {

                        if (item is System.Windows.Controls.Button i)
                        {
                            i.Opacity = i.IsEnabled ? 1.0 : 0.2;
                            i.IsEnabledChanged += (sender, e) => i.Opacity = i.IsEnabled ? 1.0 : 0.2;
                        }
                    }
                };
            });
        }
    }
}
