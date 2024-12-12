using HarmonyLib;
using JK = JumpKing.Player;
using System.Reflection;
using System;

namespace MirrorMode.Patching
{
    // Exchange input left and right when mirror mode is enabled.
    public class InputComponent
    {
        public InputComponent (Harmony harmony)
        {
            Type type = typeof(JK.InputComponent);
            MethodInfo GetState = type.GetMethod(nameof(JK.InputComponent.GetState));
            harmony.Patch(
                GetState,
                postfix: new HarmonyMethod(AccessTools.Method(typeof(InputComponent), nameof(MirrorRL)))
            );
            
            MethodInfo GetPressedState = typeof(JK.InputComponent).GetMethod(nameof(JK.InputComponent.GetPressedState));
            harmony.Patch(
                GetPressedState,
                postfix: new HarmonyMethod(AccessTools.Method(typeof(InputComponent), nameof(MirrorRL)))
            );
        }

        private static void MirrorRL(ref JK.InputComponent.State __result) 
        {
            if (MirrorMode.Preferences.IsEnabled)
            {
                bool tmp = __result.right;
                __result.right = __result.left;
                __result.left = tmp;
            }
        }
    }
}