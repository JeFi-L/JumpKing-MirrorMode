using HarmonyLib;
using System;
using System.Reflection;
// using System.Reflection.Emit;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using JK = JumpKing;
// using Microsoft.Xna.Framework;

using MirrorMode.Models;
using JumpKing;

namespace MirrorMode.Patching
{
    // Oldman entities with text_layer or oldman_layer setted to Foreground (e.g. cats in House of Nine Lives) will draw a copy at the end of
    // JumpGame.Draw() (IForeground), so we need to handle those image mirroring manually.
    public class OldManEntity
    {
        private static FieldInfo m_settings;
        private static FieldInfo home_screen;
        
        public OldManEntity (Harmony harmony)
        {
            Type type = AccessTools.TypeByName("JumpKing.MiscEntities.OldManEntity");
            MethodInfo Draw = type.GetMethod("Draw", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            harmony.Patch(
                Draw,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(OldManEntity), nameof(preDraw))),
                // transpiler: new HarmonyMethod(AccessTools.Method(typeof(OldManEntity), nameof(transpilerDraw)))
                postfix: new HarmonyMethod(AccessTools.Method(typeof(OldManEntity), nameof(postDraw)))
            );
            
            MethodInfo DrawText = type.GetMethod("DrawText", 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public,
                null,
                new Type [] {},
                null);
            harmony.Patch(
                DrawText,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(OldManEntity), nameof(preDrawText)))
            );

            m_settings = type.GetField("m_settings", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            home_screen = typeof(JumpKing.MiscEntities.OldMan.OldManSettings).GetField("home_screen");
        }

//         private static IEnumerable<CodeInstruction> transpilerDraw(IEnumerable<CodeInstruction> instructions) {
//             CodeMatcher matcher = new CodeMatcher(instructions /*, ILGenerator generator*/);

//             try {
//                 matcher.MatchStartForward(
//                         new CodeMatch(OpCodes.Beq_S),
//                         new CodeMatch(OpCodes.Ret)
//                     )
//                     .ThrowIfInvalid("123");
//                 Label label = (Label)matcher.Operand;
//                 matcher.Advance(2)
//                     .Insert(
//                         CodeInstruction.Call(() => preDraw())
//                     );
//                 matcher.Instruction.labels.Add(label);
//                 matcher.Advance(1).Instruction.labels.Clear();
                
//                 matcher.MatchStartForward(new CodeMatch(OpCodes.Ret))
//                     .ThrowIfInvalid("456");
//                 List<Label> labels = new List<Label>(matcher.Instruction.labels);
//                 if (labels.Count != 0) {
//                     matcher.Instruction.labels.Clear();
//                     matcher.Insert(
//                         CodeInstruction.Call(() => postDraw())
//                     );
//                     matcher.Instruction.labels = labels;
//                 }
//                 else {
//                     matcher.Insert(
//                         CodeInstruction.Call(() => postDraw())
//                     );
//                 }
//             } catch (Exception e) {
//                 Debug.WriteLine($"[ERROR] {e.Message}");
//                 return instructions;
//             }
// #if DEBUG
//             Debug.WriteLine("======");
//             foreach (CodeInstruction i in matcher.Instructions()) {
//                 Debug.WriteLine(i.ToString());
//             }
//             Debug.WriteLine("======");
//             Debugger.Break();
// #endif
//             return matcher.Instructions();
//         }

        private static void preDraw(object __instance) 
        {
            if ((int)home_screen.GetValue(m_settings.GetValue(__instance)) == Camera.CurrentScreenIndex1 && SpriteBatchManager.isMirroring && !SpriteBatchManager.needMirror)
            {
                SpriteBatchManager.Switch2MirrorBatch();
            }
        }

        private static void preDrawText() 
        {
            if (SpriteBatchManager.isMirroring && !SpriteBatchManager.needMirror)
            {
                SpriteBatchManager.Switch2NormalBatch();
            }
        }

        private static void postDraw(object __instance) 
        {
            if ((int)home_screen.GetValue(m_settings.GetValue(__instance)) == Camera.CurrentScreenIndex1 && SpriteBatchManager.isMirroring && !SpriteBatchManager.needMirror)
            {
                SpriteBatchManager.Switch2NormalBatch();
                SpriteBatchManager.Flush(isMirrorFirst: true);
            }
        }
    }
}