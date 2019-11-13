using GameLib;

namespace Tiler
{
    static class Utils
    {
        public static Map<Color> FromBitmap(System.Drawing.Image img)
        {
            var bmp = (System.Drawing.Bitmap)img;
            var map = new Map<Color>(bmp.Width, bmp.Height);
            for (int y = 0; y < bmp.Height; ++y)
            {
                for (int x = 0; x < bmp.Width; ++x)
                {
                    var px = bmp.GetPixel(x, y);
                    map.Set(new Point(x, y), new Color(px.R, px.G, px.B, px.A));
                }
            }
            return map;
        }

        public static System.Drawing.Bitmap ToBitmap(Map<Color> map)
        {
            var bmp = new System.Drawing.Bitmap(map.Width, map.Height);
            for (int y = 0; y < bmp.Height; ++y)
            {
                for (int x = 0; x < bmp.Width; ++x)
                {
                    var px = map.Get(new Point(x, y));
                    bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(px.A, px.R, px.G, px.B));
                }
            }
            return bmp;
        }

    }
}
