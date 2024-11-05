using HarmonyLib;
using System.Reflection;
using System;
using Microsoft.Xna.Framework;
using MirrorMode.Models;

namespace MirrorMode.Patching
{
    public class OldManEntity
    {
        public OldManEntity (Harmony harmony)
        {
            Type type = Type.GetType("JumpKing.MiscEntities.OldManEntity, JumpKing");
            MethodInfo Draw = type.GetMethod("Draw", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            harmony.Patch(
                Draw,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(OldManEntity), nameof(preDraw))),
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
                // postfix: new HarmonyMethod(AccessTools.Method(typeof(OldManEntity), nameof(postDrawText)))
            );
        }

        private static void preDraw() 
        {
            if (SpriteBatchManager.isMirror && !SpriteBatchManager.isMirroring)
            {
                SpriteBatchManager.Switch2MirrorBatch();
            }
        }

        private static void preDrawText() 
        {
            if (SpriteBatchManager.isMirror && !SpriteBatchManager.isMirroring)
            {
                SpriteBatchManager.Switch2NormalBatch();
            }
        }

        // private static void postDrawText() 
        // {
        //     if (SpriteBatchManager.isMirror && !SpriteBatchManager.isMirroring)
        //     {
        //         SpriteBatchManager.Switch2MirrorBatch();
        //     }
        // }

        private static void postDraw() 
        {
            if (SpriteBatchManager.isMirror && !SpriteBatchManager.isMirroring)
            {
                SpriteBatchManager.Switch2NormalBatch();
                SpriteBatchManager.FlushMirror(mirrorFirst: true);
            }
        }
    }
}