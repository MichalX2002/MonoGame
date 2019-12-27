// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MonoGame.Framework.Utilities;
using SdlGamePad = Sdl.GameController;

namespace MonoGame.Framework.Input
{
    public static partial class GamePad
    {
        private class GamePadInfo
        {
            public IntPtr Device { get; }
            public IntPtr HapticDevice { get; }
            public int HapticType { get; }

            public int PacketNumber { get; set; }

            public GamePadInfo(IntPtr device, IntPtr hapticDevice, int hapticType)
            {
                Device = device;
                HapticDevice = hapticDevice;
                HapticType = hapticType;
            }
        }

        private static readonly Dictionary<int, GamePadInfo> GamePads = new Dictionary<int, GamePadInfo>();
        private static readonly Dictionary<int, int> _translationTable = new Dictionary<int, int>();

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

        /// <summary>
        /// Loads embedded game controller mappings.
        /// </summary>
        internal static void InitDatabase()
        {
            var assembly = ReflectionHelpers.GetAssembly(typeof(GamePad));
            using var stream = assembly.GetManifestResourceStream("gamecontrollerdb.txt");
            if (stream == null)
                return;

            using var reader = new BinaryReader(stream);
            try
            {
                IntPtr src = Sdl.RwFromMem(reader.ReadBytes((int)stream.Length), (int)stream.Length);
                SdlGamePad.AddMappingFromRw(src, 1);
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Failed to read game controller mappings: " + exc);
            }
        }

        internal static void AddDevice(int deviceId)
        {
            IntPtr device = SdlGamePad.Open(deviceId);
            IntPtr hapticDevice = Sdl.Haptic.OpenFromJoystick(SdlGamePad.GetJoystick(device));
            int hapticType = 0;

            var id = 0;
            while (GamePads.ContainsKey(id))
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
                        Sdl.Haptic.Close(hapticDevice);
                }
                catch
                {
                    Sdl.Haptic.Close(hapticDevice);
                    hapticDevice = IntPtr.Zero;
                    Sdl.ClearError();
                }
            }

            GamePads.Add(id, new GamePadInfo(device, hapticDevice, hapticType));
            RefreshTranslationTable();
        }

        internal static void RemoveDevice(int instanceid)
        {
            foreach (KeyValuePair<int, GamePadInfo> entry in GamePads)
            {
                if (Sdl.Joystick.InstanceId(SdlGamePad.GetJoystick(entry.Value.Device)) == instanceid)
                {
                    GamePads.Remove(entry.Key);
                    DisposeDevice(entry.Value);
                    break;
                }
            }

            RefreshTranslationTable();
        }

        internal static void RefreshTranslationTable()
        {
            _translationTable.Clear();
            foreach (var pair in GamePads)
            {
                IntPtr joystick = SdlGamePad.GetJoystick(pair.Value.Device);
                _translationTable[Sdl.Joystick.InstanceId(joystick)] = pair.Key;
            }
        }

        internal static void UpdatePacketInfo(int instanceid, uint packetNumber)
        {
            if (_translationTable.TryGetValue(instanceid, out int index))
            {
                if (GamePads.TryGetValue(index, out GamePadInfo info))
                    info.PacketNumber = (int)(packetNumber < int.MaxValue
                        ? packetNumber
                        : packetNumber - int.MaxValue);
            }
        }

        private static void DisposeDevice(GamePadInfo info)
        {
            if (info.HapticType > 0)
                Sdl.Haptic.Close(info.HapticDevice);
            SdlGamePad.Close(info.Device);
        }

        internal static void CloseDevices()
        {
            foreach (var entry in GamePads)
                DisposeDevice(entry.Value);

            GamePads.Clear();
        }

        private static int PlatformGetMaxNumberOfGamePads()
        {
            return 16;
        }

        private static GamePadCapabilities PlatformGetCapabilities(int index)
        {
            if (!GamePads.TryGetValue(index, out var gamepad))
                return default;

            var gamecontroller = gamepad.Device;
            var mapping = SdlGamePad.GetMapping(gamecontroller).Split(',');
            var set = new HashSet<string>();
            foreach (var map in mapping)
            {
                var split = map.Split(':');
                if (split.Length == 2)
                    set.Add(split[0]);
            }

            bool hasVibrationMotor = GamePads[index].HapticType != 0;
            return new GamePadCapabilities(
                isConnected: true,
                displayName: SdlGamePad.GetName(gamecontroller),
                identifier: Sdl.Joystick.GetGUID(SdlGamePad.GetJoystick(gamecontroller)).ToString(),

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
            // SDL Axis ranges from -32768 to 32767, 
            // so we need to divide with different numbers depending on if it's positive
            if (axis < 0)
                return axis / 32768f;
            return axis / 32767f;
        }

        private static GamePadState PlatformGetState(
            int index, GamePadDeadZone leftDeadZoneMode, GamePadDeadZone rightDeadZoneMode)
        {
            if (!GamePads.TryGetValue(index, out var gamepad))
                return default;

            var device = gamepad.Device;

            // Y gamepad axis is inverted between SDL and XNA
            var thumbSticks = new GamePadThumbSticks(
                new Vector2(
                    GetFromSdlAxis(SdlGamePad.GetAxis(device, SdlGamePad.Axis.LeftX)),
                    GetFromSdlAxis(SdlGamePad.GetAxis(device, SdlGamePad.Axis.LeftY)) * -1f),
                new Vector2(
                    GetFromSdlAxis(SdlGamePad.GetAxis(device, SdlGamePad.Axis.RightX)),
                    GetFromSdlAxis(SdlGamePad.GetAxis(device, SdlGamePad.Axis.RightY)) * -1f),
                leftDeadZoneMode,
                rightDeadZoneMode);

            var triggers = new GamePadTriggers(
                GetFromSdlAxis(SdlGamePad.GetAxis(device, SdlGamePad.Axis.TriggerLeft)),
                GetFromSdlAxis(SdlGamePad.GetAxis(device, SdlGamePad.Axis.TriggerRight)));

            var buttons = new GamePadButtons(
                ((SdlGamePad.GetButton(device, SdlGamePad.Button.A) == 1) ? Buttons.A : 0) |
                ((SdlGamePad.GetButton(device, SdlGamePad.Button.B) == 1) ? Buttons.B : 0) |
                ((SdlGamePad.GetButton(device, SdlGamePad.Button.Back) == 1) ? Buttons.Back : 0) |
                ((SdlGamePad.GetButton(device, SdlGamePad.Button.Guide) == 1) ? Buttons.BigButton : 0) |
                ((SdlGamePad.GetButton(device, SdlGamePad.Button.LeftShoulder) == 1) ? Buttons.LeftShoulder : 0) |
                ((SdlGamePad.GetButton(device, SdlGamePad.Button.RightShoulder) == 1) ? Buttons.RightShoulder : 0) |
                ((SdlGamePad.GetButton(device, SdlGamePad.Button.LeftStick) == 1) ? Buttons.LeftStick : 0) |
                ((SdlGamePad.GetButton(device, SdlGamePad.Button.RightStick) == 1) ? Buttons.RightStick : 0) |
                ((SdlGamePad.GetButton(device, SdlGamePad.Button.Start) == 1) ? Buttons.Start : 0) |
                ((SdlGamePad.GetButton(device, SdlGamePad.Button.X) == 1) ? Buttons.X : 0) |
                ((SdlGamePad.GetButton(device, SdlGamePad.Button.Y) == 1) ? Buttons.Y : 0) |
                ((triggers.Left > 0f) ? Buttons.LeftTrigger : 0) |
                ((triggers.Right > 0f) ? Buttons.RightTrigger : 0));

            var dPad = new GamePadDPad(
                (SdlGamePad.GetButton(device, SdlGamePad.Button.DpadUp) == 1) ? ButtonState.Pressed : ButtonState.Released,
                (SdlGamePad.GetButton(device, SdlGamePad.Button.DpadDown) == 1) ? ButtonState.Pressed : ButtonState.Released,
                (SdlGamePad.GetButton(device, SdlGamePad.Button.DpadLeft) == 1) ? ButtonState.Pressed : ButtonState.Released,
                (SdlGamePad.GetButton(device, SdlGamePad.Button.DpadRight) == 1) ? ButtonState.Pressed : ButtonState.Released);

            return new GamePadState(thumbSticks, triggers, buttons, dPad, gamepad.PacketNumber);
        }

        private static bool PlatformSetVibration(
            int index, float leftMotor, float rightMotor, float leftTrigger, float rightTrigger)
        {
            if (!GamePads.TryGetValue(index, out var gamepad))
                return false;

            if (gamepad.HapticType == 0)
                return false;

            if (leftMotor <= 0f && rightMotor <= 0f)
            {
                Sdl.Haptic.StopAll(gamepad.HapticDevice);
            }
            else if (gamepad.HapticType == 1)
            {
                _hapticLeftRightEffect.leftright.LargeMagnitude = (ushort)(65535f * leftMotor);
                _hapticLeftRightEffect.leftright.SmallMagnitude = (ushort)(65535f * rightMotor);

                Sdl.Haptic.UpdateEffect(gamepad.HapticDevice, 0, ref _hapticLeftRightEffect);
                Sdl.Haptic.RunEffect(gamepad.HapticDevice, 0, 1);
            }
            else if (gamepad.HapticType == 2)
            {
                Sdl.Haptic.RumblePlay(
                    gamepad.HapticDevice,
                    Math.Max(leftMotor, rightMotor),
                    Sdl.Haptic.Infinity);
            }
            return true;
        }
    }
}
