using HarmonyLib;
using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MirrorMode.Models;
using JumpKing;

namespace MirrorMode.Patching
{
    // Mirror cursor position along the center of the game screen if mirror mode enabled.
    public class DebugTeleport
    {
        public DebugTeleport (Harmony harmony)
        {
            Type type = Type.GetType("JumpKing.Player.DebugTeleport, JumpKing");
            MethodInfo Click = type.GetMethod("Click", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            harmony.Patch(
                Click,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DebugTeleport), nameof(MirrorPosition)))
            );
        }

        private static void MirrorPosition(ref MouseState mouse) 
        {
            if (MirrorMode.Preferences.IsEnabled)
            {
                int X = Game1.graphics.PreferredBackBufferWidth-mouse.X;
                mouse = new MouseState(X, mouse.Y, mouse.ScrollWheelValue, mouse.LeftButton, mouse.MiddleButton, mouse.RightButton, mouse.XButton1, mouse.XButton2);
            }
        }
    }
}