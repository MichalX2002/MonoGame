// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Eto.Forms;

namespace MonoGame.Tools.Pipeline
{
    public partial class BuildOutput : Pad
    {
        Panel panel;
        TextArea textArea;
        Scrollable scrollable;
        Drawable drawable;

        private void InitializeComponent()
        {
            Title = "Build Output";

            panel = new Panel();

            textArea = new TextArea
            {
                Wrap = false,
                ReadOnly = true
            };

            scrollable = new Scrollable
            {
                BackgroundColor = DrawInfo.BackColor,
                ExpandContentWidth = true,
                ExpandContentHeight = true
            };
            drawable = new Drawable();
            scrollable.Content = drawable;

            panel.Content = textArea;
            CreateContent(panel);

            drawable.MouseDown += Drawable_MouseDown;
            drawable.MouseMove += Drawable_MouseMove;
            drawable.MouseLeave += Drawable_MouseLeave;
            drawable.SizeChanged += Drawable_SizeChanged;
            drawable.Paint += Drawable_Paint;
            scrollable.SizeChanged += Scrollable1_SizeChanged;
            scrollable.Scroll += Scrollable1_Scroll;
        }
    }
}
