// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.ComponentModel;
using Eto.Forms;

namespace MonoGame.Tools.Pipeline
{
    [Cell(typeof(short))]
    [Cell(typeof(int))]
    [Cell(typeof(long))]
    [Cell(typeof(ushort))]
    [Cell(typeof(uint))]
    [Cell(typeof(ulong))]
    [Cell(typeof(float))]
    [Cell(typeof(double))]
    [Cell(typeof(decimal))]
    public class CellNumber : CellBase
    {
        private TypeConverter _converter;

        public override void OnCreate()
        {
            _converter = TypeDescriptor.GetConverter(_type);

            if (_type == typeof(float) || _type == typeof(double) || _type == typeof(decimal))
            {
                if (_type == typeof(float))
                    DisplayValue = ((float)Value).ToString("0.00");
                else if (_type == typeof(double))
                    DisplayValue = ((double)Value).ToString("0.00");
                else
                    DisplayValue = ((decimal)Value).ToString("0.00");

                DisplayValue = (DisplayValue.Length > Value.ToString().Length) ? DisplayValue : Value.ToString();
            }
        }

        public override void Edit(PixelLayout control)
        {
            SkipCellDraw = true;

            var editText = new TextBox();
            editText.Tag = this;

            control.Add(editText, _lastRec.X, _lastRec.Y);

            editText.Focus();
            editText.CaretIndex = editText.Text.Length;

            OnKill += delegate
            {
                SkipCellDraw = false;
                OnKill = null;

                if (_eventHandler == null)
                    return;

                try
                {
                    _eventHandler(_converter.ConvertFrom(editText.Text), EventArgs.Empty);
                }
                catch 
                {
                }
            };

            editText.KeyDown += (sender, e) =>
            {
                if (e.Key == Keys.Enter)
                    OnKill.Invoke();
            };
        }
    }
}

