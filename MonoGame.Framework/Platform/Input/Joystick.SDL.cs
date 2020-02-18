// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MonoGame.Framework.Input
{
    public static partial class Joystick
    {
        internal static Dictionary<int, Instance> Joysticks = new Dictionary<int, Instance>();
        private static int _lastConnectedIndex = -1;

        internal static void AddDevice(int deviceId)
        {
            IntPtr jdevice = Sdl.Joystick.Open(deviceId);
            int id = 0;
            while (Joysticks.ContainsKey(id))
                id++;

            if (id > _lastConnectedIndex)
                _lastConnectedIndex = id;

            var instance = new Instance(id, jdevice);
            Joysticks.Add(id, instance);

            if (Sdl.GameController.IsGameController(deviceId) == 1)
                GamePad.AddDevice(deviceId);
        }

        internal static void RemoveDevice(int instanceid)
        {
            foreach (var entry in Joysticks)
            {
                if (Sdl.Joystick.InstanceID(entry.Value.Device) == instanceid)
                {
                    Sdl.Joystick.Close(Joysticks[entry.Key]);
                    Joysticks.Remove(entry.Key);

                    if (entry.Key == _lastConnectedIndex)
                        RecalculateLastConnectedIndex();

                    break;
                }
            }
        }

        internal static void CloseDevices()
        {
            GamePad.CloseDevices();

            foreach (var entry in Joysticks)
                entry.Value.Close();

            Joysticks.Clear();
        }

        private static void RecalculateLastConnectedIndex()
        {
            _lastConnectedIndex = -1;
            foreach (var entry in Joysticks)
            {
                if (entry.Key > _lastConnectedIndex)
                    _lastConnectedIndex = entry.Key;
            }
        }

        private static int PlatformLastConnectedIndex
        {
            get { return _lastConnectedIndex; }
        }

        private const bool PlatformIsSupported = true;

        private static JoystickCapabilities PlatformGetCapabilities(int index)
        {
            if (!Joysticks.TryGetValue(index, out var jinstance))
                return JoystickCapabilities.Default;

            return jinstance.GetCapabilities(index);
        }

        private static JoystickState PlatformGetState(int index)
        {
            if (!Joysticks.TryGetValue(index, out var jinstance))
                return JoystickState.Default;

            var jcap = jinstance.GetCapabilities(index);
            var axes = jinstance.GetAxes(jcap);
            var buttons = jinstance.GetButtons(jcap);
            var hats = jinstance.GetHats(jcap);

            return new JoystickState(isConnected: true, axes, buttons, hats);
        }
    }
}
