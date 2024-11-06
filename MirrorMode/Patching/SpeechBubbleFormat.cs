using HarmonyLib;
using JK = JumpKing.MiscEntities.OldMan;
using System.Reflection;
using MirrorMode.Models;
using Microsoft.Xna.Framework;
using JumpKing;

namespace MirrorMode.Patching
{
    // All dialogue of npc will use this class to draw the text, and it will be draw in entites.draw().
    // So we mirror it again and put it on mirrored position to let those looks correctly.
    public class SpeechBubbleFormat
    {
        public SpeechBubbleFormat (Harmony harmony)
        {
            MethodInfo DrawText = typeof(JK.SpeechBubbleFormat).GetMethod(nameof(JK.SpeechBubbleFormat.DrawText));
            harmony.Patch(
                DrawText,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(SpeechBubbleFormat), nameof(preDraw))),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(SpeechBubbleFormat), nameof(postDraw)))
            );
            
        }

        private static void preDraw(ref Vector2 position, ref JK.SpeechBubbleFormat p_format) 
        {
            if (SpriteBatchManager.isMirroring) {
                position.X = 480 - position.X;
                p_format.direction = (p_format.direction == JK.SpeechBubbleFormat.DirectionX.Right) ? JK.SpeechBubbleFormat.DirectionX.Left : JK.SpeechBubbleFormat.DirectionX.Right;
                p_format.anchor.X = (p_format.anchor.X == JK.SpeechBubbleFormat.DirectionX.Right) ? JK.SpeechBubbleFormat.DirectionX.Left : JK.SpeechBubbleFormat.DirectionX.Right;
                if (SpriteBatchManager.needMirror)
                {
                    SpriteBatchManager.Switch2MirrorBatch();
                }
            }
        }
        private static void postDraw() 
        {
            if (SpriteBatchManager.isMirroring && SpriteBatchManager.needMirror)
            {
                SpriteBatchManager.Switch2NormalBatch();
                SpriteBatchManager.Flush();
            }
        }
    }
}