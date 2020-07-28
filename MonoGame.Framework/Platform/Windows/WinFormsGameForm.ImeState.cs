// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Text;

namespace MonoGame.Framework.Windows
{
    internal partial class WinFormsGameForm
    {
        // Adapted from SDL2:video/windows/SDL_windowskeyboard.c

        public class ImeState
        {
            public const int SCS_SETSTR = (int)(GCS.COMPREADSTR | GCS.COMPSTR);

            public WinFormsGameForm Form { get; }

            public char[] ime_composition = new char[32];
            public int ime_cursor;

            private IntPtr Hwnd => Form.Handle;

            public ImeState(WinFormsGameForm form)
            {
                Form = form ?? throw new ArgumentNullException(nameof(form));
            }

            public static short LOWORD(long l)
            {
                return (short)(l & 0xffff);
            }

            public static int StrLength(ReadOnlySpan<char> text)
            {
                return text.IndexOf('\0');
            }

            public void SetTextInputPosition(Point position)
            {
                const int CFS_FORCE_POSITION = 0x0020;

                IntPtr himc = ImmGetContext(Hwnd);
                if (himc != IntPtr.Zero)
                {
                    var cf = new COMPOSITIONFORM()
                    {
                        ptCurrentPos = position,
                        dwStyle = CFS_FORCE_POSITION,
                    };
                    ImmSetCompositionWindow(himc, cf);
                    ImmReleaseContext(Hwnd, himc);
                }
            }

            public void ClearComposition()
            {
                const int NI_COMPOSITIONSTR = 0x0015;
                const int NI_CLOSECANDIDATE = 0x0011;
                const int CPS_CANCEL = 0x0004;

                IntPtr himc = ImmGetContext(Hwnd);
                if (himc == IntPtr.Zero)
                    return;

                ImmNotifyIME(himc, NI_COMPOSITIONSTR, CPS_CANCEL, 0);

                unsafe
                {
                    int* empty = stackalloc int[1] { 0 };
                    ImmSetCompositionString(himc, SCS_SETSTR, empty, sizeof(int), empty, sizeof(int));
                }

                ImmNotifyIME(himc, NI_CLOSECANDIDATE, 0, 0);
                ImmReleaseContext(Hwnd, himc);
                Form.SendTextEditing(default, 0, 0);
            }

            public unsafe void GetCompositionString(IntPtr himc, int str)
            {
                fixed (char* ime_composition_ptr = ime_composition)
                {
                    long length = ImmGetCompositionStringW(
                        himc, str, ime_composition_ptr, (ime_composition.Length - 1) * sizeof(char));

                    if (length < 0)
                        length = 0;

                    length /= sizeof(char);
                    ime_cursor = LOWORD(ImmGetCompositionStringW(himc, (int)GCS.CURSORPOS, null, 0));
                    if (ime_cursor < ime_composition.Length && ime_composition[ime_cursor] == 0x3000)
                    {
                        int i;
                        for (i = ime_cursor + 1; i < length; ++i)
                            ime_composition[i - 1] = ime_composition[i];

                        length--;
                    }
                    ime_composition[length] = '\0';
                }
            }

            public void SendInputEvent()
            {
                var buffer = ime_composition.AsSpan();
                buffer = buffer.Slice(0, StrLength(buffer));

                for (int i = 0; i < buffer.Length;)
                {
                    var status = Rune.DecodeFromUtf16(buffer, out Rune rune, out int consumed);
                    if (status != System.Buffers.OperationStatus.Done)
                        throw new InvalidDataException("Failed to decode rune from UTF-16.");

                    Form.SendTextInput(rune);
                    i += consumed;
                }

                ime_composition[0] = '\0';
                ime_cursor = 0;
            }

            public void SendEditingEvent()
            {
                var buffer = ime_composition.AsSpan();
                buffer = buffer.Slice(0, StrLength(buffer));
                Form.SendTextEditing(buffer, ime_cursor, 0);
            }
        }
    }
}
