﻿
namespace MonoGame.Framework.Input
{
    public static class ButtonStateExtensions
    {
        public static bool ToBool(this ButtonState state)
        {
            return state == ButtonState.Pressed;
        }
    }
}
