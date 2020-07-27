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

        internal static void AddDevice(int deviceId)
        {
            IntPtr jdevice = SDL.Joystick.Open(deviceId);

            int id = 0;
            while (Joysticks.ContainsKey(id))
                id++;

            if (id > PlatformLastConnectedIndex)
                PlatformLastConnectedIndex = id;

            var instance = new Instance(id, jdevice);
            Joysticks.Add(id, instance);

            if (SDL.GameController.IsGameController(deviceId) == 1)
                GamePad.AddDevice(deviceId);
        }

        internal static void RemoveDevice(int instanceid)
        {
            foreach (var entry in Joysticks)
            {
                if (SDL.Joystick.InstanceID(entry.Value.Device) == instanceid)
                {
                    Joysticks[entry.Key].Close();
                    Joysticks.Remove(entry.Key);

                    if (entry.Key == PlatformLastConnectedIndex)
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
            PlatformLastConnectedIndex = -1;
            foreach (var entry in Joysticks)
            {
                if (entry.Key > PlatformLastConnectedIndex)
                    PlatformLastConnectedIndex = entry.Key;
            }
        }

        private static int PlatformLastConnectedIndex { get; set; } = -1;

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
