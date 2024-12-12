using HarmonyLib;
using JK = JumpKing.GameManager;
using System.Reflection;
using MirrorMode.Models;
using System;

namespace MirrorMode.Patching
{
    // Some faded texts (ending credit, LocationNotification etc.) draw drawed between background and foreground,
    // so we need to mirror it again to make it looks correctly.
    public class FadeTextEntity
    {
        public FadeTextEntity (Harmony harmony)
        {
            Type type = typeof(JK.FadeTextEntity);
            MethodInfo Draw = type.GetMethod(nameof(JK.FadeTextEntity.Draw));
            harmony.Patch(
                Draw,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FadeTextEntity), nameof(preDraw))),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(FadeTextEntity), nameof(postDraw)))
            );
        }

        private static void preDraw() 
        {
            if (SpriteBatchManager.isMirroring && SpriteBatchManager.needMirror) {
                SpriteBatchManager.Switch2MirrorBatch();
            }
        }
        private static void postDraw() 
        {
            if (SpriteBatchManager.isMirroring && SpriteBatchManager.needMirror) {
                SpriteBatchManager.Switch2NormalBatch();
            }
        }
    }
}