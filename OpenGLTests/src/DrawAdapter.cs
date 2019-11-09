using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Terrain;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;


namespace OpenGLTests.src
{
    public class DrawAdapter
    {
        /// <summary>
        /// Draws an image on the on the current Scene.
        /// </summary>
        /// <param name="image">The image to be drawn</param>
        /// <param name="location">The location to draw the image</param>
        /// <param name="rotation">The rotation of the image in degrees</param>

        public void DrawWeapon(Weapon w)
        {
            Sprite sprite = w.Animation.GetSprite();
            if (sprite == null) return;

            GLCoordinate location = w.Location.ToGLCoordinate();

            float left, right, top, bottom;

            bottom = location.Y - w.Size.Y / 2;
            top = location.Y + w.Size.Y / 2;
            right = location.X + w.Size.X / 2;
            left = location.X - w.Size.X / 2;

            GL.PushMatrix();
            GL.Translate(location.X, location.Y, 0);
            GL.Rotate((double)w.Rotation, 0, 0, 1);
            GL.Translate(-location.X, -location.Y, 0);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);

            GL.Color4(w.Color);

            GL.BindTexture(TextureTarget.Texture2D, sprite.GLID);
            GL.Begin(BeginMode.Quads);
            if (w.GetFacing == Facing.Right) fillSprite(left, right, top, bottom, w);
            else if (w.GetFacing == Facing.Left) fillSprite(right, left, top, bottom, w);

            GL.End();
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);

            GL.PopMatrix();
        }

        public void DrawEntity(Entity drawable)
        {
            Sprite sprite = drawable.Animation.GetSprite();
            if (sprite == null) return;

            //todo: do this before entering draw
            GLCoordinate location = drawable.Location.ToGLCoordinate();

            float left, right, top, bottom;

            bottom = location.Y - drawable.Size.Y / 2;
            top = location.Y + drawable.Size.Y / 2;
            right = location.X + drawable.Size.X / 2;
            left = location.X - drawable.Size.X / 2;


            if (drawable is Hero || drawable is Hostile) TraceRectangle(Color.Red, location.X - drawable.Size.X / 2, -location.Y + drawable.Size.Y / 2, drawable.Size.X, -drawable.Size.Y);

            GL.PushMatrix();

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);

            GL.Color4(drawable.Color);

            GL.BindTexture(TextureTarget.Texture2D, sprite.GLID);
            GL.Begin(BeginMode.Quads);


            if (drawable.Facing == Facing.Right) fillSprite(left, right, top, bottom, drawable);
            else if (drawable.Facing == Facing.UpsideDownLeft) fillSprite(right, left, bottom, top, drawable);
            else if (drawable.Facing == Facing.Left) fillSprite(right, left, top, bottom, drawable);
            else if (drawable.Facing == Facing.UpsideDownRight) fillSprite(left, right, bottom, top, drawable);


            GL.End();
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);

            GL.PopMatrix();
        }
        
        public void DrawElement(Element drawable)
        {
            Sprite sprite = drawable.Animation.GetSprite();
            if (sprite == null) return;
            //todo: do this before entering draw
            GLCoordinate location = ((Element)drawable).Location;

            float left, right, top, bottom;
            
            bottom = location.Y - drawable.Size.Y / 2;
            top = location.Y + drawable.Size.Y / 2;
            right = location.X + drawable.Size.X / 2;
            left = location.X - drawable.Size.X / 2;


            GL.PushMatrix();


            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);

            GL.Color4(drawable.Color);

            GL.BindTexture(TextureTarget.Texture2D, sprite.GLID);
            GL.Begin(BeginMode.Quads);

            
            if     (drawable.Facing == Facing.Right) fillSprite(left, right, top, bottom, drawable); 
            else if(drawable.Facing == Facing.UpsideDownLeft)    fillSprite(right, left, bottom, top, drawable); 
            else if(drawable.Facing == Facing.Left)  fillSprite(right, left, top, bottom, drawable); 
            else if(drawable.Facing == Facing.UpsideDownRight)  fillSprite(left, right, bottom, top, drawable); 


            GL.End();
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);

            GL.PopMatrix();
        }


        private void fillSprite(float left, float right, float top, float bottom, Drawable d)
        {
            GL.TexCoord2(0, 0);
            if(!d.Flipped) GL.Vertex2(left, top);
            else GL.Vertex2(right, bottom);

            GL.TexCoord2(0, 1);
            GL.Vertex2(left, bottom);

            GL.TexCoord2(1, 1);
            if(!d.Flipped) GL.Vertex2(right, bottom);
            else GL.Vertex2(left, top);

            GL.TexCoord2(1, 0);
            GL.Vertex2(right, top);
        }

        public void TraceRectangle(Color color, float x, float y, float width, float height, float lineWidth = 3)
        {
            GL.PushAttrib(AttribMask.EnableBit);
            GL.Enable(EnableCap.Blend);
            GL.LineWidth(lineWidth);

            GL.PopAttrib();
            GL.Color4(color);
            GL.Begin(PrimitiveType.LineLoop);

            GL.Vertex2(x, -y);
            GL.Vertex2(x + width, -y);
            GL.Vertex2(x + width, -(y + height));
            GL.Vertex2(x, -(y + height));

            GL.End();
            GL.PopAttrib();
        }
        public void FillRectangle(Color c, float x, float y, float width, float height)
        {
            y = -y;
            x = x - width / 2;
            y = y - height / 2;
            GL.Enable(EnableCap.Blend);
            GL.Color4(c);
            
            GL.Begin(PrimitiveType.Quads);


            GL.Vertex2(x, -y);
            GL.Vertex2(x + width, -y);
            GL.Vertex2(x + width, -(y + height));
            GL.Vertex2(x, -(y + height));

            GL.End();
        }


        private const int CIRCLE_VERTICES = 32;
        public void DrawCircle(float x, float y, GLCoordinate radius, Color color, float width = 1)
        {
            GL.LineWidth(width);
            GL.Enable(EnableCap.Blend);
            GL.Begin(PrimitiveType.LineLoop);

            GL.Color4(color);
            for (int i = 0; i < CIRCLE_VERTICES; i++)
            {
                float theta = 2.0f * 3.1415926f * i / CIRCLE_VERTICES;//get the current angle

                float xx = radius.X * (float)Math.Cos(theta);//calculate the x component
                float yy = radius.Y * (float)Math.Sin(theta);//calculate the y component

                // GL.Vertex2(x + cx, y + cy);//output vertex
                GL.Vertex2(x + xx, y + yy);
            }

            GL.End();
        }

        public void FillCircle(float x, float y, GLCoordinate radius, Color color, float width = 1)
        {
            GL.LineWidth(width);
            GL.Begin(PrimitiveType.TriangleFan);

            GL.Color4(color);
            for (int i = 0; i < CIRCLE_VERTICES; i++)
            {
                float theta = 2.0f * 3.1415926f * i / CIRCLE_VERTICES;//get the current angle

                float xx = radius.X * (float)Math.Cos(theta);//calculate the x component
                float yy = radius.Y * (float)Math.Sin(theta);//calculate the y component

                // GL.Vertex2(x + cx, y + cy);//output vertex
                GL.Vertex2(x + xx, y + yy);
            }

            GL.End();
        }


        public void DrawFan(float originX, float originY, int alpha, float l, int degrees)
        {
            GL.Enable(EnableCap.Blend);
            GL.Begin(PrimitiveType.LineLoop);

            int fanDeg1 = alpha + degrees/2;
            int fanDeg2 = alpha - degrees/2;

            GL.Color4(Color.Coral);

            GL.Vertex2(originX, originY);

            for (int i = fanDeg2; i < fanDeg1; i++)
            {
                float theta = 2.0f * 3.1415926f * i / 360;//get the current angle
                float xx = l * (float)Math.Cos(theta);//calculate the x component
                float yy = l * (float)Math.Sin(theta);//calculate the y component
                GL.Vertex2(originX + xx, originY + yy);
            }

            GL.Vertex2(originX, originY);

            GL.End();
        }

        public void DrawLine(GLCoordinate origin, GLCoordinate terminus, Color color, LineType type, float width = 3)
        {
            GL.PushAttrib(AttribMask.EnableBit);
            GL.Enable(EnableCap.Blend);
            GL.LineWidth(width);
            if (type == LineType.Dashed)
            {
                GL.LineStipple(1, 0xAAA);
                GL.Enable(EnableCap.LineStipple);
            }
 
            GL.Begin(PrimitiveType.Lines);

            GL.Color4(color);

            GL.Vertex2(origin.X, origin.Y);
            GL.Vertex2(terminus.X, terminus.Y);

            GL.End();
            GL.PopAttrib();
        }
        /// <summary>
        /// Binds an Image in OpenGL 
        /// </summary>
        /// <param name="image">The image to be bound to a texture</param>
        /// <returns>The integer Used by OpenGL to identify the created texture</returns>
        public static int CreateTexture(Image image)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(image);

            BitmapData data = bmp.LockBits(
                new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);


            return id;
        }
    }
}
