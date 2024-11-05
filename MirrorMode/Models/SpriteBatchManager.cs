using HarmonyLib;
using JumpKing;
using JumpKing.BodyCompBehaviours;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MirrorMode.Models
{
    public static class SpriteBatchManager
    {
        public static readonly int HEIGHT = Game1.HEIGHT;
        public static readonly int WIDTH = Game1.WIDTH;
        public static bool isMirroring = false;
        public static bool isMirror = false;
        private static bool isMirrorBatching = false;
        private static readonly Matrix Mirror_X = Matrix.CreateTranslation(-WIDTH/2f, 0f, 0f) 
            * Matrix.CreateScale(-1f, 1f, 1f) 
            * Matrix.CreateTranslation(WIDTH/2f, 0f, 0f);
        private static readonly SpriteBatch OriginalBatch;
        private static readonly SpriteBatch MirrorBatch;
        private static readonly RenderTarget2D OriginalTarget;
        private static readonly RenderTarget2D MirrorTarget;
        private static readonly Rectangle GameSize;
        // private static Color[] Data;
        // private static Color[] MirroredData;

        static SpriteBatchManager() {
            HEIGHT = Game1.HEIGHT;
            WIDTH = Game1.WIDTH;
            OriginalBatch = Game1.spriteBatch;
            MirrorBatch = new SpriteBatch(Game1.instance.GraphicsDevice);
            OriginalTarget = Traverse.Create(Game1.instance).Field("m_render_target").GetValue<RenderTarget2D>();
            GameSize = new Rectangle(0, 0, WIDTH, HEIGHT);
            MirrorTarget = new RenderTarget2D(Game1.instance.GraphicsDevice, WIDTH, HEIGHT);
            // Data = new Color[WIDTH*HEIGHT];
            // MirroredData = new Color[WIDTH*HEIGHT];
        }

        public static void StartMirrorBatch() {
            if (isMirrorBatching) {
                return;
            }
            isMirrorBatching = true;
            MirrorBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, RasterizerState.CullClockwise, null, Mirror_X);
        }

        public static void EndMirrorBatch() {
            if (!isMirrorBatching) {
                return;
            }
            isMirrorBatching = false;
            MirrorBatch.End();
        }

        public static void FlushMirror(bool mirrorFirst = false) {
            if (mirrorFirst) {
                EndMirrorBatch();
                StartMirrorBatch();
                OriginalBatch.End();
                OriginalBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            }
            else {
                OriginalBatch.End();
                OriginalBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                EndMirrorBatch();
                StartMirrorBatch();
            }
        }

        public static void Switch2NormalBatch() {
            Game1.spriteBatch = OriginalBatch;
        }

        public static void Switch2MirrorBatch() {
            Game1.spriteBatch = MirrorBatch;
        }

        public static void MirrorScreen() {
            OriginalBatch.End();
            EndMirrorBatch();

            Game1.instance.GraphicsDevice.SetRenderTarget(MirrorTarget);
            Game1.instance.GraphicsDevice.Clear(Color.Black);
            OriginalBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            OriginalBatch.Draw(OriginalTarget, GameSize, GameSize, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
            OriginalBatch.End();

            Game1.instance.GraphicsDevice.SetRenderTarget(OriginalTarget);
            Game1.instance.GraphicsDevice.Clear(Color.Black);
            OriginalBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            OriginalBatch.Draw(MirrorTarget, GameSize, Color.White);
            StartMirrorBatch();
        }

        // public static void MirrorScreen() {
        //     OriginalBatch.End();
        //     EndMirrorBatch();
        //     OriginalTarget.GetData<Color>(Data);
        //     for (int x=0; x<WIDTH; x++) {
        //         for (int y=0; y<HEIGHT; y++) {
        //             int index = (WIDTH-1-x)+y*WIDTH;
        //             MirroredData[index] = Data[x+y*WIDTH];
        //         }
        //     }
        //     OriginalTarget.SetData<Color>(MirroredData);
        //     OriginalBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        //     StartMirrorBatch();
        // }
    }
}
