using HarmonyLib;
using JK = JumpKing.GameManager;
using System.Reflection;
using MirrorMode.Models;

namespace MirrorMode.Patching
{
    public class FadeTextEntity
    {
        public FadeTextEntity (Harmony harmony)
        {
            MethodInfo Draw = typeof(JK.FadeTextEntity).GetMethod(nameof(JK.FadeTextEntity.Draw));
            harmony.Patch(
                Draw,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FadeTextEntity), nameof(preDraw))),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(FadeTextEntity), nameof(postDraw)))
            );
        }

        private static void preDraw() 
        {
            if (SpriteBatchManager.isMirroring) {
                SpriteBatchManager.Switch2MirrorBatch();
            }
        }
        private static void postDraw() 
        {
            if (SpriteBatchManager.isMirroring) {
                SpriteBatchManager.Switch2NormalBatch();
            }
        }
    }
}