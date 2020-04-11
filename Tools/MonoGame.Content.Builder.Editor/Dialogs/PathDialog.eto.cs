// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Tools.Pipeline
{
    partial class PathDialog : Dialog<bool>
    {
        DynamicLayout layout1;
        StackLayout stack1, stack2;
        Label label1, label2;
        TextBox textBox1;
        Button buttonBrowse;
        Button buttonOk, buttonCancel;

        private void InitializeComponent()
        {
            Title = "Select Folder";
            DisplayMode = DialogDisplayMode.Attached;
            Size = new Size(400, 200);

            buttonOk = new Button
            {
                Text = "Ok"
            };
            PositiveButtons.Add(buttonOk);
            DefaultButton = buttonOk;

            buttonCancel = new Button
            {
                Text = "Cancel"
            };
            NegativeButtons.Add(buttonCancel);
            AbortButton = buttonCancel;

            layout1 = new DynamicLayout
            {
                DefaultSpacing = new Size(4, 4),
                Padding = new Padding(6)
            };
            layout1.BeginVertical();

            layout1.Add(null, true, true);

            label1 = new Label
            {
                Text = "Path to use:"
            };
            layout1.Add(label1);

            stack1 = new StackLayout
            {
                Spacing = 4,
                Orientation = Orientation.Horizontal
            };

            textBox1 = new TextBox();
            stack1.Items.Add(new StackLayoutItem(textBox1, VerticalAlignment.Center, true));

            buttonBrowse = new Button
            {
                Text = "...",
                MinimumSize = new Size(1, 1)
            };
            stack1.Items.Add(new StackLayoutItem(buttonBrowse, VerticalAlignment.Center, false));

            layout1.Add(stack1);

            label2 = new Label
            {
                Text = "Macros:"
            };
            layout1.Add(label2);

            stack2 = new StackLayout
            {
                Spacing = 4,
                Orientation = Orientation.Horizontal
            };

            foreach (var symbol in symbols)
            {
                var buttonSymbol = new Button
                {
                    Text = symbol
                };
                buttonSymbol.Click += ButtonSymbol_Click;
                stack2.Items.Add(new StackLayoutItem(buttonSymbol, true));
            }

            layout1.Add(stack2);

            layout1.Add(null, true, true);

            Content = layout1;

            textBox1.TextChanged += TextBox1_TextChanged;
            buttonBrowse.Click += ButtonBrowse_Click;
            buttonOk.Click += ButtonOk_Click;
            buttonCancel.Click += ButtonCancel_Click;
        }
    }
}
