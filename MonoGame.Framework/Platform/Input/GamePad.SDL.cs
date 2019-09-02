// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MonoGame.Utilities;

namespace MonoGame.Framework.Input
{
    static partial class GamePad
    {
        private readonly struct GamePadInfo
        {
            public readonly IntPtr Device;
            public readonly IntPtr HapticDevice;
            public readonly int HapticType;

            public GamePadInfo(IntPtr device, IntPtr hapticDevice, int hapticType)
            {
                Device = device;
                HapticDevice = hapticDevice;
                HapticType = hapticType;
            }
        }

        private static readonly Dictionary<int, GamePadInfo> Gamepads = new Dictionary<int, GamePadInfo>();

        private static Sdl.Haptic.Effect _hapticLeftRightEffect = new Sdl.Haptic.Effect
        {
            type = Sdl.Haptic.EffectId.LeftRight,
            leftright = new Sdl.Haptic.LeftRight
            {
                Type = Sdl.Haptic.EffectId.LeftRight,
                Length = Sdl.Haptic.Infinity,
                LargeMagnitude = ushort.MaxValue,
                SmallMagnitude = ushort.MaxValue
            }
        };

        public static void InitDatabase()
        {
            using (var stream = ReflectionHelpers.GetAssembly(typeof(GamePad)).GetManifestResourceStream("gamecontrollerdb.txt"))
            {
                if (stream == null)
                    return;

                using (var reader = new BinaryReader(stream))
                {
                    try
                    {
                        IntPtr src = Sdl.RwFromMem(reader.ReadBytes((int)stream.Length), (int)stream.Length);
                        Sdl.GameController.AddMappingFromRw(src, 1);
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine("Failed to read game controller mappings: " + exc);
                    }
                }
            }
        }

        internal static void AddDevice(int deviceId)
        {
            IntPtr device = Sdl.GameController.Open(deviceId);
            IntPtr hapticDevice = Sdl.Haptic.OpenFromJoystick(Sdl.GameController.GetJoystick(device));
            int hapticType = 0;

            var id = 0;
            while (Gamepads.ContainsKey(id))
                id++;

            if (hapticDevice != IntPtr.Zero)
            {
                try
                {
                    if (Sdl.Haptic.EffectSupported(hapticDevice, ref _hapticLeftRightEffect) == 1)
                    {
                        Sdl.Haptic.NewEffect(hapticDevice, ref _hapticLeftRightEffect);
                        hapticType = 1;
                    }
                    else if (Sdl.Haptic.RumbleSupported(hapticDevice) == 1)
                    {
                        Sdl.Haptic.RumbleInit(hapticDevice);
                        hapticType = 2;
                    }
                    else
                    {
                        Sdl.Haptic.Close(hapticDevice);
                    }
                }
                catch
                {
                    Sdl.Haptic.Close(hapticDevice);
                    hapticDevice = IntPtr.Zero;
                    Sdl.ClearError();
                }
            }

            Gamepads.Add(id, new GamePadInfo(device, hapticDevice, hapticType));
        }

        internal static void RemoveDevice(int instanceid)
        {
            foreach (KeyValuePair<int, GamePadInfo> entry in Gamepads)
            {
                if (Sdl.Joystick.InstanceID(Sdl.GameController.GetJoystick(entry.Value.Device)) == instanceid)
                {
                    Gamepads.Remove(entry.Key);
                    DisposeDevice(entry.Value);
                    break;
                }
            }
        }

        private static void DisposeDevice(in GamePadInfo info)
        {
            if (info.HapticType > 0)
                Sdl.Haptic.Close(info.HapticDevice);
            Sdl.GameController.Close(info.Device);
        }

        internal static void CloseDevices()
        {
            foreach (var entry in Gamepads)
                DisposeDevice(entry.Value);

            Gamepads.Clear();
        }

        private static int PlatformGetMaxNumberOfGamePads()
        {
            return 16;
        }

        private static GamePadCapabilities PlatformGetCapabilities(int index)
        {
            if (!Gamepads.ContainsKey(index))
                return GamePadCapabilities.None;

            var gamecontroller = Gamepads[index].Device;
            var mapping = Sdl.GameController.GetMapping(gamecontroller).Split(',');
            var set = new HashSet<string>();
            foreach (var map in mapping)
            {
                var split = map.Split(':');
                if (split.Length == 2)
                    set.Add(split[0]);
            }

            bool hasVibrationMotor = Gamepads[index].HapticType != 0;
            return new GamePadCapabilities(
                isConnected: true,
                displayName: Sdl.GameController.GetName(gamecontroller),
                identifier: Sdl.Joystick.GetGUID(Sdl.GameController.GetJoystick(gamecontroller)).ToString(),

                hasBackButton: set.Contains("back"),
                hasBigButton: set.Contains("guide"),
                hasStartButton: set.Contains("start"),

                hasAButton: set.Contains("a"),
                hasBButton: set.Contains("b"),
                hasXButton: set.Contains("x"),
                hasYButton: set.Contains("y"),

                hasLeftXThumbStick: set.Contains("leftx"),
                hasLeftYThumbStick: set.Contains("lefty"),
                hasRightXThumbStick: set.Contains("rightx"),
                hasRightYThumbStick: set.Contains("righty"),

                hasDPadLeftButton: set.Contains("dpleft"),
                hasDPadDownButton: set.Contains("dpdown"),
                hasDPadRightButton: set.Contains("dpright"),
                hasDPadUpButton: set.Contains("dpup"),

                hasLeftShoulderButton: set.Contains("leftshoulder"),
                hasRightShoulderButton: set.Contains("rightshoulder"),
                hasLeftStickButton: set.Contains("leftstick"),
                hasRightStickButton: set.Contains("rightstick"),
                hasLeftTrigger: set.Contains("lefttrigger"),
                hasRightTrigger: set.Contains("righttrigger"),

                hasLeftVibrationMotor: hasVibrationMotor,
                hasRightVibrationMotor: hasVibrationMotor,
                hasVoiceSupport: false,
                gamePadType: GamePadType.GamePad);
        }

        private static float GetFromSdlAxis(int axis)
        {
            // SDL Axis ranges from -32768 to 32767, so we need to divide with different numbers depending on if it's positive
            if (axis < 0)
                return axis / 32768f;
            return axis / 32767f;
        }

        private static GamePadState PlatformGetState(
            int index, GamePadDeadZone leftDeadZoneMode, GamePadDeadZone rightDeadZoneMode)
        {
            if (!Gamepads.ContainsKey(index))
                return GamePadState.Default;

            var device = Gamepads[index].Device;

            // Y gamepad axis is inverted between SDL and XNA
            var thumbSticks = new GamePadThumbSticks(
                new Vector2(
                    GetFromSdlAxis(Sdl.GameController.GetAxis(device, Sdl.GameController.Axis.LeftX)),
                    GetFromSdlAxis(Sdl.GameController.GetAxis(device, Sdl.GameController.Axis.LeftY)) * -1f),
                new Vector2(
                    GetFromSdlAxis(Sdl.GameController.GetAxis(device, Sdl.GameController.Axis.RightX)),
                    GetFromSdlAxis(Sdl.GameController.GetAxis(device, Sdl.GameController.Axis.RightY)) * -1f),
                leftDeadZoneMode,
                rightDeadZoneMode);

            var triggers = new GamePadTriggers(
                GetFromSdlAxis(Sdl.GameController.GetAxis(device, Sdl.GameController.Axis.TriggerLeft)),
                GetFromSdlAxis(Sdl.GameController.GetAxis(device, Sdl.GameController.Axis.TriggerRight)));

            var buttons = new GamePadButtons(
                ((Sdl.GameController.GetButton(device, Sdl.GameController.Button.A) == 1) ? Buttons.A : 0) |
                ((Sdl.GameController.GetButton(device, Sdl.GameController.Button.B) == 1) ? Buttons.B : 0) |
                ((Sdl.GameController.GetButton(device, Sdl.GameController.Button.Back) == 1) ? Buttons.Back : 0) |
                ((Sdl.GameController.GetButton(device, Sdl.GameController.Button.Guide) == 1) ? Buttons.BigButton : 0) |
                ((Sdl.GameController.GetButton(device, Sdl.GameController.Button.LeftShoulder) == 1) ? Buttons.LeftShoulder : 0) |
                ((Sdl.GameController.GetButton(device, Sdl.GameController.Button.RightShoulder) == 1) ? Buttons.RightShoulder : 0) |
                ((Sdl.GameController.GetButton(device, Sdl.GameController.Button.LeftStick) == 1) ? Buttons.LeftStick : 0) |
                ((Sdl.GameController.GetButton(device, Sdl.GameController.Button.RightStick) == 1) ? Buttons.RightStick : 0) |
                ((Sdl.GameController.GetButton(device, Sdl.GameController.Button.Start) == 1) ? Buttons.Start : 0) |
                ((Sdl.GameController.GetButton(device, Sdl.GameController.Button.X) == 1) ? Buttons.X : 0) |
                ((Sdl.GameController.GetButton(device, Sdl.GameController.Button.Y) == 1) ? Buttons.Y : 0) |
                ((triggers.Left > 0f) ? Buttons.LeftTrigger : 0) |
                ((triggers.Right > 0f) ? Buttons.RightTrigger : 0));

            var dPad = new GamePadDPad(
                (Sdl.GameController.GetButton(device, Sdl.GameController.Button.DpadUp) == 1) ? ButtonState.Pressed : ButtonState.Released,
                (Sdl.GameController.GetButton(device, Sdl.GameController.Button.DpadDown) == 1) ? ButtonState.Pressed : ButtonState.Released,
                (Sdl.GameController.GetButton(device, Sdl.GameController.Button.DpadLeft) == 1) ? ButtonState.Pressed : ButtonState.Released,
                (Sdl.GameController.GetButton(device, Sdl.GameController.Button.DpadRight) == 1) ? ButtonState.Pressed : ButtonState.Released);

            return new GamePadState(thumbSticks, triggers, buttons, dPad);
        }

        private static bool PlatformSetVibration(int index, float leftMotor, float rightMotor)
        {
            if (!Gamepads.ContainsKey(index))
                return false;

            var gamepad = Gamepads[index];

            if (gamepad.HapticType == 0)
                return false;

            if (leftMotor <= 0.0f && rightMotor <= 0.0f)
                Sdl.Haptic.StopAll(gamepad.HapticDevice);
            else if (gamepad.HapticType == 1)
            {
                _hapticLeftRightEffect.leftright.LargeMagnitude = (ushort)(65535f * leftMotor);
                _hapticLeftRightEffect.leftright.SmallMagnitude = (ushort)(65535f * rightMotor);

                Sdl.Haptic.UpdateEffect(gamepad.HapticDevice, 0, ref _hapticLeftRightEffect);
                Sdl.Haptic.RunEffect(gamepad.HapticDevice, 0, 1);
            }
            else if (gamepad.HapticType == 2)
                Sdl.Haptic.RumblePlay(gamepad.HapticDevice, Math.Max(leftMotor, rightMotor), Sdl.Haptic.Infinity);

            return true;
        }
    }
}
