using HarmonyLib;
using JK = JumpKing;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Diagnostics;

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
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(JumpGame), nameof(transpileDraw))),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(JumpGame), nameof(postRemoveFlag)))
            );
        }

        private static IEnumerable<CodeInstruction> transpileDraw(IEnumerable<CodeInstruction> instructions) {
            CodeMatcher matcher = new CodeMatcher(instructions /*, ILGenerator generator*/);

            try {
                matcher.MatchStartForward(
                        new CodeMatch(OpCodes.Call, AccessTools.Method("JumpKing.Level.LevelManager:get_CurrentScreen")),
                        new CodeMatch(OpCodes.Callvirt, AccessTools.Method("JumpKing.Level.LevelScreen:Draw"))
                    )
                    .ThrowIfInvalid("Cant find LevelManager.CurrentScreen.Draw()");
                var labels = new List<Label>(matcher.Instruction.labels);
                matcher.Instruction.labels.Clear();
                matcher.Insert(
                        CodeInstruction.Call(() => preDrawBackground())
                    );
                matcher.Instruction.labels = labels;

                matcher.MatchStartForward(
                        new CodeMatch(OpCodes.Call, AccessTools.Method("JumpKing.Level.LevelManager:get_CurrentScreen")),
                        new CodeMatch(OpCodes.Callvirt, AccessTools.Method("JumpKing.Level.LevelScreen:DrawForeground"))
                    )
                    .ThrowIfInvalid("Cant find LevelManager.CurrentScreen.DrawForeground()")
                    .Insert(CodeInstruction.Call(() => preDrawForeground()))
                    .Advance(3)
                    .Insert(CodeInstruction.Call(() => postDrawForeground()));
            } catch (Exception e) {
                Debug.WriteLine($"[ERROR] {e.Message}");
                return instructions;
            }

            foreach (CodeInstruction i in matcher.Instructions()) {
                Debug.WriteLine(i.ToString());
            }
            Debugger.Break();
            return matcher.Instructions();
        }

        private static void preDrawBackground() {
            if (MirrorMode.Preferences.IsEnabled) {
                SpriteBatchManager.isMirror = true;
                SpriteBatchManager.isMirroring = true;
                SpriteBatchManager.StartMirrorBatch();
            }
        }

        private static void preDrawForeground() {
            if (SpriteBatchManager.isMirror) {
                SpriteBatchManager.FlushMirror();
            }
        }
        private static void postDrawForeground() {
            if (MirrorMode.Preferences.IsEnabled) {
                SpriteBatchManager.isMirroring = false;
                SpriteBatchManager.MirrorScreen();
            }

        }
        private static void postRemoveFlag() {
            SpriteBatchManager.isMirror = false;
            SpriteBatchManager.EndMirrorBatch();
            SpriteBatchManager.Switch2NormalBatch();
        }
    }
}