using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Util
{
    public enum SpriteID
    {
        missing, //needs to be first for it to be default: https://stackoverflow.com/questions/529929/choosing-the-default-value-of-an-enum-type-without-having-to-change-values/529937

        elf_m_idle_anim_f0,
        elf_m_idle_anim_f1,
        elf_m_idle_anim_f2,
        elf_m_idle_anim_f3,

        floor_1,
        floor_2,
        floor_3,
        floor_4,
        floor_5,
        floor_6,
        floor_7,
        floor_8,

        action_move,
        action_attack,
        action_charge,

        item_flask_big_red,
        item_flask_big_green,

    }
    static class ImageLoader
    {
        private static Dictionary<SpriteID, int> GLIDs = new Dictionary<SpriteID, int>();
        private static Dictionary<SpriteID, Image> Images = new Dictionary<SpriteID, Image>();

        public static void Init()
        {
            Images[SpriteID.elf_m_idle_anim_f0] = new Bitmap(Properties.Resources.elf_f_idle_anim_f0);
            Images[SpriteID.elf_m_idle_anim_f1] = new Bitmap(Properties.Resources.elf_f_idle_anim_f1);
            Images[SpriteID.elf_m_idle_anim_f2] = new Bitmap(Properties.Resources.elf_f_idle_anim_f2);
            Images[SpriteID.elf_m_idle_anim_f3] = new Bitmap(Properties.Resources.elf_f_idle_anim_f3);

            Images[SpriteID.floor_1] = new Bitmap(Properties.Resources.floor_1);
            Images[SpriteID.floor_2] = new Bitmap(Properties.Resources.floor_2);
            Images[SpriteID.floor_3] = new Bitmap(Properties.Resources.floor_3);
            Images[SpriteID.floor_4] = new Bitmap(Properties.Resources.floor_4);
            Images[SpriteID.floor_5] = new Bitmap(Properties.Resources.floor_5);
            Images[SpriteID.floor_6] = new Bitmap(Properties.Resources.floor_6);
            Images[SpriteID.floor_7] = new Bitmap(Properties.Resources.floor_7);
            Images[SpriteID.floor_8] = new Bitmap(Properties.Resources.floor_8);

            Images[SpriteID.action_move] = new Bitmap(Properties.Resources.action_move);
            Images[SpriteID.action_attack] = new Bitmap(Properties.Resources.action_attack);
            Images[SpriteID.action_charge] = new Bitmap(Properties.Resources.action_charge);

            Images[SpriteID.item_flask_big_red] = new Bitmap(Properties.Resources.flask_big_red);
            Images[SpriteID.item_flask_big_green] = new Bitmap(Properties.Resources.flask_big_green);

            Images[SpriteID.missing] = new Bitmap(Properties.Resources.empty);
            //foreach (var image in Enum.GetValues(typeof(SpriteID)).Cast<SpriteID>())
            foreach (var img in Images)
            {
                GetBinding(img.Key);
            }
        }

        public static Image GetImage(SpriteID id)
        {
            if (Images.ContainsKey(id)) return Images[id];
            else return null;
        }

        /// <summary>
        /// Gets the int used by OpenGL to identify the texture bound from the given Images
        /// </summary>
        /// <param name="spriteId"></param>
        /// <returns></returns>
        public static int GetBinding(SpriteID spriteId)
        {
            if (GLIDs.ContainsKey(spriteId))
            {
                return GLIDs[spriteId];
            }

            var loaded = DrawAdapter.CreateTexture(Images[spriteId]);
            GLIDs[spriteId] = loaded;
            return loaded;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
