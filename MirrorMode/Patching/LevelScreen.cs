using HarmonyLib;
using JK = JumpKing.Level;
using System.Reflection;
using System.Collections.Generic;

using MirrorMode.Models;

namespace MirrorMode.Patching
{
    public class LevelScreen
    {
        public LevelScreen (Harmony harmony)
        {
            MethodInfo Draw = typeof(JK.LevelScreen).GetMethod(nameof(JK.LevelScreen.Draw));
            harmony.Patch(
                Draw,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(LevelScreen), nameof(preDrawBackground)))
            );

            MethodInfo DrawForeground = typeof(JK.LevelScreen).GetMethod(nameof(JK.LevelScreen.DrawForeground));
            harmony.Patch(
                DrawForeground,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(LevelScreen), nameof(preDrawForeground))),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(LevelScreen), nameof(postDrawForeground)))
            );
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
    }
}