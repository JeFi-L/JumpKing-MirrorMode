using HarmonyLib;
using System;
using System.Reflection;

using JK = JumpKing.PauseMenu;

using MirrorMode.Models;

namespace MirrorMode.Patching
{
    // If mirror mode enabled, PauseManager will draw a copy with entity draw() so disable it.
    public class PauseManager
    {
        public PauseManager (Harmony harmony)
        {
            Type type = AccessTools.TypeByName("JumpKing.PauseMenu.PauseManager");
            MethodInfo Draw = type.GetMethod("Draw", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            harmony.Patch(
                Draw,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PauseManager), nameof(DisableBGDraw)))
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