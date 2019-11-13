using GameLib;
using SPFSharp;
using System;
using GameLib.Graphics;
using Vector4 = System.Numerics.Vector4;

namespace Tiler
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Game.Run();
        }
    }

    static class Game
    {
        public static int Width = 1280;
        public static int Height = 720;
        public static Rectangle Screen => new Rectangle(0, 0, Width, Height);

        public static SPF.Texture FontTexture;
        public static PixelFont Font;

        private static void DrawFont(Rectangle dest, Rectangle source, Color color)
        {
            SPF.Renderer.DrawTexture(FontTexture,
                dest.X, dest.Y, dest.Width, dest.Height,
                source.X, source.Y, source.Width, source.Height,
                false, false, color, Vector4.Zero);
        }

        public static void Run()
        {
            Platform.ContentFolder = "";
            using (SPF.Open("Pixel Editor", Width, Height))
            {
                FontTexture = new SPF.Texture(Platform.ReadFile("font.png"));
                Font = new PixelFont(FontTexture.Height, FontTexture.Width / FontTexture.Height, DrawFont) { Scale = 2 };

                var process = new Process();
                process.Run(new Editor());

                while (SPF.BeginLoop(out float dt))
                {
                    dt = Math.Min(dt, 1 / 30f);

                    process.Update(dt);

                    SPF.Renderer.Begin();
                    process.Draw();

                    SPF.EndLoop();
                }
            }
        }

    }
}
