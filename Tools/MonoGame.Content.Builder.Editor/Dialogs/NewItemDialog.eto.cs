// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Tools.Pipeline
{
    partial class NewItemDialog : Dialog<bool>
    {
        TableLayout table1;
        DynamicLayout layout1, layout2;
        Label labelName, labelType, labelExt, labelError;
        TextBox textBox1;
        ListBox list1;
        Button buttonCreate, buttonCancel;

        private void InitializeComponent()
        {
            Title = "New File";
            DisplayMode = DialogDisplayMode.Attached;
            Size = new Size(370, 285);
            MinimumSize = new Size(370, 200);
            Resizable = true;

            buttonCreate = new Button
            {
                Text = "Create"
            };
            PositiveButtons.Add(buttonCreate);
            DefaultButton = buttonCreate;

            buttonCancel = new Button
            {
                Text = "Cancel"
            };
            NegativeButtons.Add(buttonCancel);
            AbortButton = buttonCancel;

            layout1 = new DynamicLayout
            {
                Padding = new Padding(6)
            };

            table1 = new TableLayout(2, 3)
            {
                Spacing = new Size(4, 4)
            };

            labelName = new Label
            {
                Text = "Name: ",
                VerticalAlignment = VerticalAlignment.Center
            };
            table1.Add(labelName, 0, 0, false, false);

            layout2 = new DynamicLayout
            {
                DefaultSpacing = new Size(4, 4)
            };
            layout2.BeginHorizontal();

            textBox1 = new TextBox();
            layout2.Add(textBox1, true, true);

            labelExt = new Label
            {
                Text = " .spriteFont",
                VerticalAlignment = VerticalAlignment.Center,
                Width = 80
            };
            layout2.Add(labelExt, false, true);

            table1.Add(layout2, 1, 0, true, false);

            labelType = new Label
            {
                Text = "Type: ",
                VerticalAlignment = VerticalAlignment.Top
            };
            table1.Add(labelType, 0, 1, false, true);

            list1 = new ListBox();
            table1.Add(list1, 1, 1, true, true);

            layout1.Add(table1, true, true);

            labelError = new Label
            {
                TextAlignment = TextAlignment.Center
            };
            table1.Add(labelError, 1, 2, true, false);

            layout1.Add(labelError, true, false);

            Content = layout1;

            textBox1.TextChanged += TextBox1_TextChanged;
            list1.SelectedIndexChanged += List1_SelectedIndexChanged;
            buttonCreate.Click += ButtonCreate_Click;
            buttonCancel.Click += ButtonCancel_Click;
        }
    }
}
