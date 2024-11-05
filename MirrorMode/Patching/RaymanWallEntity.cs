// using HarmonyLib;
// using JK = JumpKing.Props.RaymanWall;
// using System.Reflection;
// using MirrorMode.Models;
// using System;

// namespace MirrorMode.Patching
// {
//     public class RaymanWallEntity
//     {
//         public RaymanWallEntity (Harmony harmony)
//         {
//             Type type = Type.GetType("JumpKing.Props.RaymanWall.RaymanWallEntity, JumpKing");
//             MethodInfo Draw = type.GetMethod("Draw", BindingFlags.Instance | BindingFlags.Public);
//             harmony.Patch(
//                 Draw,
//                 prefix: new HarmonyMethod(AccessTools.Method(typeof(RaymanWallEntity), nameof(preDraw)))
//             );
            
//             MethodInfo DrawText = type.GetMethod("DrawText", BindingFlags.Instance | BindingFlags.Public);
//             harmony.Patch(
//                 Draw,
//                 prefix: new HarmonyMethod(AccessTools.Method(typeof(RaymanWallEntity), nameof(preDraw)))
//             );
//         }

//         private static void preDraw() 
//         {
//             if (SpriteBatchManager.isMirroring)
//             {
//                 SpriteBatchManager.FlushMirror(false);
//             }
//         }
//     }
// }