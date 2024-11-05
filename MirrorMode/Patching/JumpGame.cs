using HarmonyLib;
using JK = JumpKing;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

using MirrorMode.Models;
using System;

namespace MirrorMode.Patching
{
    public class JumpGame
    {
        public JumpGame (Harmony harmony)
        {
            MethodInfo Draw = typeof(JK.JumpGame).GetMethod(nameof(JK.JumpGame.Draw));
            harmony.Patch(
                Draw,
                // transpiler: new HarmonyMethod(AccessTools.Method(typeof(JumpGame), nameof(Transpiler)))
                postfix: new HarmonyMethod(AccessTools.Method(typeof(JumpGame), nameof(RemoveFlag)))
            );
        }

        private static void RemoveFlag() {
            SpriteBatchManager.isMirror = false;
            SpriteBatchManager.EndMirrorBatch();
            SpriteBatchManager.Switch2NormalBatch();
        }

        // private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        // {
        //     CodeMatcher matcher = new CodeMatcher(instructions);

        //     matcher.MatchEndForward(
        //         CodeMatch.Call(() => )
        //     )
        // }

    }
}