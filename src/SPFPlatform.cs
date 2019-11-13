using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLib;
using GameLib.Editor;
using SPFSharp;

namespace Tiler
{
    struct SPFTexture : IMGUI.ITexture
    {
        private readonly SPF.Texture _texture;
        public SPFTexture(SPF.Texture texture) : this()
        {
            _texture = texture;
        }
        public int Width => _texture.Width;
        public int Height => _texture.Height;
        public void Draw(Rectangle src, Rectangle dest, Color color) => Platform.Draw(_texture, src, dest, color);
    }

    class SPFPlatform : IMGUI.IPlatform
    {
        public Point MousePosition => Input.MousePosition;

        public void FillHorizontalGradient(Rectangle dest, Color a, Color b) => SPF.Renderer.FillHorizontalGradient(dest.X, dest.Y, dest.Width, dest.Height, a, b);
        public void FillVerticalGradient(Rectangle dest, Color a, Color b) => SPF.Renderer.FillVerticalGradient(dest.X, dest.Y, dest.Width, dest.Height, a, b);

        public void FillRectangle(Rectangle rect, Color color) => Platform.FillRectangle(rect, color);

        public bool IsAltKeyDown() => SPF.Input.IsKeyDown(SPF.Key.Alt);
        public bool IsControlKeyDown() => SPF.Input.IsKeyDown(SPF.Key.Control);
        public bool IsMouseLeftButtonDown() => SPF.Input.IsMouseButtonDown(SPF.MouseButton.Left);
        public bool IsMouseLeftButtonPressed() => SPF.Input.IsMouseButtonPressed(SPF.MouseButton.Left);
        public bool IsMouseRightButtonPressed() => SPF.Input.IsMouseButtonPressed(SPF.MouseButton.Right);
        public bool IsMouseLeftButtonReleased() => SPF.Input.IsMouseButtonReleased(SPF.MouseButton.Left);
        public bool IsMouseRightButtonReleased() => SPF.Input.IsMouseButtonReleased(SPF.MouseButton.Right);
        public bool IsMouseRightButtonDown() => SPF.Input.IsMouseButtonDown(SPF.MouseButton.Right);
    }
}
