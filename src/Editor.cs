using System.Windows.Forms;
using GameLib;
using GameLib.Editor;

namespace Tiler
{
    class Editor : IOp
    {

        private static IMGUI UI = new IMGUI(new SPFPlatform()) {Font = Game.Font};

        private Color _currentColor = Color.Black;
        private int _pixelSize = 12;

        private Map<Color> _image;

        public OpState Start()
        {
            _image = new Map<Color>(16, 16);
            return OpState.Running;
        }

        public OpState Update(float dt)
        {
            var layout = Game.Screen;
            layout = UI.ToolGroup(layout, "Pixel");

            var toolbar = Layout.StackV(ref layout, 25, 2);
            if (UI.Button(Layout.StackH(ref toolbar, 250, 2), "From Clipboard"))
            {
                if (Clipboard.ContainsImage())
                {
                    _image = Utils.FromBitmap(Clipboard.GetImage());
                }
            }

            if (UI.Button(Layout.StackH(ref toolbar, 250, 2), "To Clipboard"))
            {
                using (var bmp = Utils.ToBitmap(_image))
                {
                    Clipboard.SetImage(bmp);
                }
            }

            Layout.StackH(ref toolbar, 20);
            UI.RadioButton(Layout.StackH(ref toolbar, 50), "4", 4, ref _pixelSize);
            UI.RadioButton(Layout.StackH(ref toolbar, 50), "8", 8, ref _pixelSize);
            UI.RadioButton(Layout.StackH(ref toolbar, 50), "12", 12, ref _pixelSize);

            var paletteGroup = UI.ToolGroup(Layout.CarveX(ref layout, 200), "Palette");
            UI.Custom(() =>
            {
                Platform.FillRectangle(new Rectangle(paletteGroup.X, paletteGroup.Y, 64, 64), _currentColor);
            });

            var previewGroup = UI.ToolGroup(Layout.CarveX(ref layout, 200, Layout.CarvePosition.End), "Preview");
            UI.Custom(() =>
            {
                for (int ty = 0; ty < 3; ++ty)
                {
                    for (int tx = 0; tx < 3; ++tx)
                    {
                        DrawImage(
                            new Point(previewGroup.X + (tx * _image.Width), previewGroup.Y + (ty * _image.Height)));

                        DrawImage(
                            new Point(previewGroup.X + (3 * _image.Width) + 4 + (tx * _image.Width * 2), previewGroup.Y + (ty * _image.Height * 2)),
                            2);
                    }
                }
            });

            var editorGroup = UI.ToolGroup(layout, "Editor");
            {
                var rect = new Rectangle(editorGroup.X, editorGroup.Y,
                    3 * _image.Width * _pixelSize,
                    3 * _image.Height * _pixelSize);
                if (rect.Contains(UI.Platform.MousePosition))
                {
                    var px = ((UI.Platform.MousePosition.X - rect.X) / _pixelSize) % _image.Width;
                    var py = ((UI.Platform.MousePosition.Y - rect.Y) / _pixelSize) % _image.Height;
                    if (UI.Platform.IsMouseLeftButtonDown())
                    {
                        if (UI.Platform.IsAltKeyDown())
                        {
                            _currentColor = _image.Get(new Point(px, py));
                        }
                        else
                        {
                            _image.Set(new Point(px, py), _currentColor);
                        }
                    }
                    else if (UI.Platform.IsMouseRightButtonDown())
                    {
                        _image.Set(new Point(px, py), Color.TransparentBlack);
                    }
                }

                UI.Custom(() =>
                {
                    for (int ty = 0; ty < 3; ++ty)
                    {
                        for (int tx = 0; tx < 3; ++tx)
                        {
                            DrawImage(
                                new Point(
                                    rect.X + (tx * _image.Width * _pixelSize),
                                    rect.Y + (ty * _image.Height * _pixelSize)),
                                _pixelSize);
                            Platform.DrawRectangle(new Rectangle(
                                rect.X + (tx * _image.Width * _pixelSize),
                                rect.Y + (ty * _image.Height * _pixelSize),
								_image.Width * _pixelSize,
								_image.Height * _pixelSize), new Color(0, 0, 0, 0.2f));
                        }
                    }
                    Platform.DrawRectangle(rect, Color.Black);
                });
            }


            return OpState.Running;
        }

        private void DrawImage(Point dest, int scale = 1)
        {
            for (int y = 0; y < _image.Height; ++y)
            {
                for (int x = 0; x < _image.Width; ++x)
                {
                    Platform.FillRectangle(
                        new Rectangle(dest.X + (x * scale), dest.Y + (y * scale), scale, scale),
                        _image.Get(new Point(x, y)));
                }
            }
        }

        public void Draw()
        {
            UI.Draw();
        }
    }
}
