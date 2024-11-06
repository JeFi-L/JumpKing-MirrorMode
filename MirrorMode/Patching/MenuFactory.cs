using HarmonyLib;
using System;
using System.Reflection;

using JK = JumpKing.PauseMenu;

using MirrorMode.Models;

namespace MirrorMode.Patching
{
    // If mirror mode enabled, PauseManager will draw a copy with entity draw() so disable it.
    public class MenuFactory
    {
        public MenuFactory (Harmony harmony)
        {
            Type type = Type.GetType("JumpKing.PauseMenu.PauseManager, JumpKing");
            MethodInfo Draw = type.GetMethod("Draw", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            harmony.Patch(
                Draw,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(MenuFactory), nameof(DisableBGDraw)))
            );
        }

        private static bool DisableBGDraw() 
        {
            if (SpriteBatchManager.isMirroring && SpriteBatchManager.needMirror)
            {
                return false;
            }
            return true;
        }
    }
}