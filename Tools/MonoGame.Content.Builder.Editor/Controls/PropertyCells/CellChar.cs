// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Eto.Forms;

namespace MonoGame.Tools.Pipeline
{
    [Cell(typeof(char))]
    public class CellChar : CellBase
    {
        public override void OnCreate()
        {
            DisplayValue = ((int)(char)Value) + " (" + Value + ")";
        }

        public override void Edit(PixelLayout control)
        {
            SkipCellDraw = true;
            var editText = new TextBox();
            editText.Tag = this;
            editText.Style = "OverrideSize";
            editText.Width = _lastRec.Width;
            editText.Height = _lastRec.Height;

            char.TryParse(Value.ToString(), out char value);

            editText.Text = ((int)value).ToString();

            control.Add(editText, _lastRec.X, _lastRec.Y);

            editText.Focus();
            editText.CaretIndex = editText.Text.Length;

            OnKill += delegate
            {
                SkipCellDraw = false;
                OnKill = null;

                if (_eventHandler == null)
                    return;

                if (!int.TryParse(editText.Text, out int num))
                    return;

                _eventHandler((char)num, EventArgs.Empty);
            };

            editText.KeyDown += (sender, e) =>
            {
                if (e.Key == Keys.Enter)
                    OnKill.Invoke();
            };
        }
    }
}
