// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MonoGame.Framework.Input
{
    public static partial class Joystick
    {
        internal class Instance
        {
            private string _identifier;

            public int Id { get; }
            public IntPtr Device { get; private set; }
            public Guid Guid { get; private set; }

            public int[] Axes { get; private set; }
            public ButtonState[] Buttons { get; private set; }
            public JoystickHat[] Hats { get; private set; }

            public Instance(int id, IntPtr device)
            {
                Id = id;
                Device = device;

                Axes = Array.Empty<int>();
                Buttons = Array.Empty<ButtonState>();
                Hats = Array.Empty<JoystickHat>();
            }

            public JoystickCapabilities GetCapabilities(int index)
            {
                var guid = Sdl.Joystick.GetGUID(Device);
                if (Guid != guid)
                {
                    Guid = guid;
                    _identifier = Guid.ToString();
                }

                return new JoystickCapabilities(
                    isConnected: true,
                    displayName: Sdl.Joystick.GetJoystickName(Device),
                    identifier: _identifier,
                    isGamepad: Sdl.GameController.IsGameController(index) == 1,
                    axisCount: Sdl.Joystick.NumAxes(Device),
                    buttonCount: Sdl.Joystick.NumButtons(Device),
                    hatCount: Sdl.Joystick.NumHats(Device));
            }

            public int[] GetAxes(in JoystickCapabilities jcap)
            {
                if (Axes.Length != jcap.AxisCount)
                    Axes = new int[jcap.AxisCount];

                for (var i = 0; i < Axes.Length; i++)
                    Axes[i] = Sdl.Joystick.GetAxis(Device, i);
                return Axes;
            }

            public ButtonState[] GetButtons(in JoystickCapabilities jcap)
            {
                if (Buttons.Length != jcap.ButtonCount)
                    Buttons = new ButtonState[jcap.ButtonCount];

                for (var i = 0; i < Buttons.Length; i++)
                    Buttons[i] = (Sdl.Joystick.GetButton(Device, i) == 0) ? ButtonState.Released : ButtonState.Pressed;
                return Buttons;
            }

            public JoystickHat[] GetHats(in JoystickCapabilities jcap)
            {
                if (Hats.Length != jcap.HatCount)
                    Hats = new JoystickHat[jcap.HatCount];

                for (var i = 0; i < Hats.Length; i++)
                {
                    var hatstate = Sdl.Joystick.GetHat(Device, i);

                    Hats[i] = new JoystickHat(
                        up: hatstate.HasFlags(Sdl.Joystick.Hat.Up) ? ButtonState.Pressed : ButtonState.Released,
                        down: hatstate.HasFlags(Sdl.Joystick.Hat.Down) ? ButtonState.Pressed : ButtonState.Released,
                        left: hatstate.HasFlags(Sdl.Joystick.Hat.Left) ? ButtonState.Pressed : ButtonState.Released,
                        right: hatstate.HasFlags(Sdl.Joystick.Hat.Right) ? ButtonState.Pressed : ButtonState.Released);
                }
                return Hats;
            }

            public void Close()
            {
                Sdl.Joystick.Close(Device);
                Device = IntPtr.Zero;
            }
        }
    }
}
