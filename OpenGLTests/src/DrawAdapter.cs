﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenTK.Graphics.OpenGL;


namespace OpenGLTests.src
{
    public class DrawAdapter
    {
        public enum DrawMode
        {
            Centered,
            TopLeft,
        }

        /// <summary>
        /// Draws an image on the on the current Scene.
        /// </summary>
        /// <param name="image">The image to be drawn</param>
        /// <param name="location">The location to draw the image</param>
        /// <param name="rotation">The rotation of the image in degrees</param>
        /*public void DrawImage(ImageX image, GLCoordinate location, float rotation, DrawMode drawMode)
        {

            float left, right, top, bottom;
            switch (drawMode)
            {
                case DrawMode.Centered:
                    {
                        left = -image.Size.Width / 2;
                        right = image.Size.Width / 2;
                        top = -image.Size.Height / 2;
                        bottom = image.Size.Height / 2;
                    }
                    break;

                case DrawMode.TopLeft:
                    {
                        left = 0;
                        right = image.Size.Width;
                        top = 0;
                        bottom = image.Size.Height;
                    }
                    break;

                default:
                    throw new GameBrokenException("Fallthroughx");
            }

            PushMatrix();
            Translate(location.X, location.Y);
            Rotate(rotation);

            GL.Enable(EnableCap.Texture2D);
            GL.Color4(image.BrushColor);
            GL.BindTexture(TextureTarget.Texture2D, image.GLTextureId);
            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0, 0);
            GL.Vertex2(left, top);

            GL.TexCoord2(0, 1);
            GL.Vertex2(left, bottom);

            GL.TexCoord2(1, 1);
            GL.Vertex2(right, bottom);

            GL.TexCoord2(1, 0);
            GL.Vertex2(right, top);

            GL.End();
            GL.Disable(EnableCap.Texture2D);

            PopMatrix();
        }*/

        /*public void TraceRectangle(Color color, float x, float y, float width, float height)
        {
            GL.Color4(color);
            GL.Begin(PrimitiveType.LineLoop);

            GL.Vertex2(x, -y);
            GL.Vertex2(x + width, -y);
            GL.Vertex2(x + width, -(y + height));
            GL.Vertex2(x, -(y + height));

            GL.End();
        }*/

        public void FillRectangle(Color c, float x, float y, float width, float height)
        {
            y = -y;
            x = x - width / 2;
            y = y - height / 2;


            GL.Color4(c);
            
            GL.Begin(PrimitiveType.Quads);


            GL.Vertex2(x, -y);
            GL.Vertex2(x + width, -y);
            GL.Vertex2(x + width, -(y + height));
            GL.Vertex2(x, -(y + height));

            GL.End();
        }


        private const int CIRCLE_VERTICES = 32;
        public void FillCircle(float x, float y, GLCoordinate radius, Color color, float width = 1)
        {
            GL.LineWidth(width);
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


        public void DrawLine(GLCoordinate origin, GLCoordinate terminus, Color color, LineType type, float width = 3)
        {
            GL.PushAttrib(AttribMask.EnableBit);
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

        /*
        public void FillRectangle(Color cb, GLCoordinate topLeft, GLCoordinate bottomRight)
        {
            FillRectangle(cb, topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, topLeft.Y - bottomRight.Y);
        }
        */
        /// <summary>
        /// Pushes a translation matrix
        /// Rubber on before translating or scaling
        /// </summary>
        public void PushMatrix()
        {
            GL.PushMatrix();
        }

        /// <summary>
        /// Pops a translation matrix
        /// Rubber off after translating or scaling
        /// </summary>
        public void PopMatrix()
        {
            GL.PopMatrix();
        }
        /*
        public void Translate(GLCoordinate translation)
        {
            Translate(translation.X, translation.Y);
        }*/

        public void Translate(float x, float y)
        {
            GL.Translate(x, y, 0);
        }

        public void Scale(float xScale, float yScale)
        {
            GL.Scale(xScale, yScale, 1);
        }

        public void Rotate(float radians)
        {
            GL.Rotate(radians, 0, 0, 1);
        }
    
    }
}
