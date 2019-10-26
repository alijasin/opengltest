using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables.Terrain;

namespace OpenGLTests.src.Drawables
{
    static class Cursor
    {

        public static void SetCursor(Image i)
        {


        }

        private static byte[] converter(Image x)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
            return xByte;
        }

        public static void Draw(GameCoordinate loc)
        {
            if (loc == null) return;

            DrawAdapter d = new DrawAdapter();
            Entity e = new Floor(loc);
            GameState.Drawables.Add(e);
            d.DrawSprite(e, DrawAdapter.DrawMode.Centered);
        }
    }
}
