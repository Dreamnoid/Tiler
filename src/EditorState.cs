using System.Windows.Forms;
using Dream;
using Dream.Drawing;

namespace Tiler
{
    class EditorState : SceneState
    {
        private Color _currentColor = Color.Black;
        private int _pixelSize = 12;
        private bool _showGrid = true;

        private Bitmap _image = new Bitmap(new Size(16));
        private Bitmap _palette;

        public EditorState()
        {
            _palette = new Bitmap(Program.Platform.LoadBitmap("palette.png"));
            Use(new EditorSystem(OnUpdate));
        }

        private void OnUpdate(Editor editor, float dt)
        {
            var UI = editor;
            var layout = Game.Screen;
            layout = UI.ToolGroup(layout, "Pixel");

            var toolbar = Layout.StackV(ref layout, 25, 2);
            if (UI.Button(Layout.StackH(ref toolbar, 250, 2), "From Clipboard"))
            {
                if (System.Windows.Forms.Clipboard.ContainsImage())
                {
                    _image = Bitmap.FromGDI((System.Drawing.Bitmap)System.Windows.Forms.Clipboard.GetImage());
                }
            }

            if (UI.Button(Layout.StackH(ref toolbar, 250, 2), "To Clipboard"))
            {
                using (var bmp = _image.ToGDI())
                {
                    System.Windows.Forms.Clipboard.SetImage(bmp);
                }
            }

            Layout.StackH(ref toolbar, 20);
            UI.RadioButton(Layout.StackH(ref toolbar, 50), "4", 4, ref _pixelSize);
            UI.RadioButton(Layout.StackH(ref toolbar, 50), "8", 8, ref _pixelSize);
            UI.RadioButton(Layout.StackH(ref toolbar, 50), "12", 12, ref _pixelSize);
            Layout.StackH(ref toolbar, 20);
            UI.ToggleButton(Layout.StackH(ref toolbar, 50), "Grid", ref _showGrid);

            var paletteGroup = UI.ToolGroup(Layout.CarveX(ref layout, _palette.Width + 10), "Palette");
            var currentColorRect = Layout.StackV(ref paletteGroup, 64, 4);
            editor.ColorPicker(ref currentColorRect, ref _currentColor);
            var paletteRect = paletteGroup.Resize(_palette.Size);

            if (editor.Input.LeftClicked() && paletteRect.Contains(editor.Input.MousePosition))
            {
                _currentColor = _palette.Get(editor.Input.MousePosition - paletteRect.Location);
            }

            UI.Register((renderer) =>
            {
                renderer.Draw(Game.Textures.Get("palette.png"), paletteRect.Location, Color.White);
            });

            var previewGroup = UI.ToolGroup(Layout.CarveX(ref layout, 200, Layout.CarvePosition.End), "Preview");
            UI.Register((renderer) =>
            {
                for (int ty = 0; ty < 3; ++ty)
                {
                    for (int tx = 0; tx < 3; ++tx)
                    {
                        DrawImage(renderer,
                            new Point(previewGroup.X + (tx * _image.Width), previewGroup.Y + (ty * _image.Height)));

                        DrawImage(renderer,
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
                if (rect.Contains(editor.Input.MousePosition))
                {
                    var px = ((editor.Input.MousePosition.X - rect.X) / _pixelSize) % _image.Width;
                    var py = ((editor.Input.MousePosition.Y - rect.Y) / _pixelSize) % _image.Height;
                    if (editor.Input.IsAnyPressed(Dream.Keys.MouseButtonMiddle))
                    {
                        _image.FloodFill(new Point(px, py), _currentColor);
                    }
                    else if (editor.Input.LeftDown())
                    {
                        if (editor.Input.IsAnyDown(Dream.Keys.KeyAlt))
                        {
                            _currentColor = _image.Get(new Point(px, py));
                        }
                        else
                        {
                            _image.Set(new Point(px, py), _currentColor);
                        }
                    }
                    else if (editor.Input.IsAnyDown(Dream.Keys.MouseButtonRight))
                    {
                        _image.Set(new Point(px, py), Color.TransparentBlack);
                    }
                }

                UI.Register((renderer) =>
                {
                    for (int ty = 0; ty < 3; ++ty)
                    {
                        for (int tx = 0; tx < 3; ++tx)
                        {
                            DrawImage(
                                renderer,
                                new Point(
                                    rect.X + (tx * _image.Width * _pixelSize),
                                    rect.Y + (ty * _image.Height * _pixelSize)),
                                _pixelSize);
                            if (_showGrid)
                            {
                                renderer.DrawRectangle(new Rectangle(
                                    rect.X + (tx * _image.Width * _pixelSize),
                                    rect.Y + (ty * _image.Height * _pixelSize),
                                    _image.Width * _pixelSize,
                                    _image.Height * _pixelSize), new Color(0, 0, 0, 0.2f));
                            }
                        }
                    }
                    renderer.DrawRectangle(rect, Color.Black);
                });
            }
        }

        private void DrawImage(IRenderer renderer, Point dest, int scale = 1)
        {
            for (int y = 0; y < _image.Height; ++y)
            {
                for (int x = 0; x < _image.Width; ++x)
                {
                    renderer.FillRectangle(
                        new Rectangle(dest.X + (x * scale), dest.Y + (y * scale), scale, scale),
                        _image.Get(new Point(x, y)));
                }
            }
        }
    }
}
