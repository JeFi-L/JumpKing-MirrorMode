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
    // Mirror all things drawed between LevelManager.Currentcreen.Draw() and LevelManager.CurrentScreen.DrawForeground()
    // (including m_entity_manager.Draw()), and set some flags.
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

        private static IEnumerable<CodeInstruction> transpileDraw(IEnumerable<CodeInstruction> instructions /*, ILGenerator generator*/) {
            CodeMatcher matcher = new CodeMatcher(instructions /*, ILGenerator generator*/);

            try {
                // Find LevelManager.Currentcreen.Draw() Sthen insert preDrawBackground() before and move all labels from it
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

                // Find LevelManager.CurrentScreen.DrawForeground() then insert preDrawForeground(), postDrawForeground() on both side
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
#if DEBUG
            Debug.WriteLine("======");
            foreach (CodeInstruction i in matcher.Instructions()) {
                Debug.WriteLine(i.ToString());
            }
            Debug.WriteLine("======");
            Debugger.Break();
#endif
            return matcher.Instructions();
        }

        private static void preDrawBackground() {
            if (MirrorMode.Preferences.IsEnabled) {
                SpriteBatchManager.isMirroring = true;
                SpriteBatchManager.needMirror = true;
                SpriteBatchManager.StartMirrorBatch();
            }
        }

        private static void preDrawForeground() {
            if (SpriteBatchManager.isMirroring) {
                SpriteBatchManager.Flush();
            }
        }
        private static void postDrawForeground() {
            if (SpriteBatchManager.isMirroring) {
                SpriteBatchManager.needMirror = false;
                SpriteBatchManager.MirrorScreen();
            }
        }
        private static void postRemoveFlag() {
            SpriteBatchManager.isMirroring = false;
            SpriteBatchManager.EndMirrorBatch();
            SpriteBatchManager.Switch2NormalBatch();
        }
    }
}