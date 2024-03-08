using SPFSharp;
using System;
using Dream;

namespace Tiler
{
    class Program
    {
        public static SPFPlatform Platform;

        [STAThread]
        static void Main(string[] args)
        {
            ErrorHandler.Handle(() =>
            {
                Platform = new SPFPlatform("Tiler", new Size(1900, 960), "", null, new Settings());
                Game.Run(Platform, () =>
                {
                    return new EditorState();
                });
            });
        }
    }
}
