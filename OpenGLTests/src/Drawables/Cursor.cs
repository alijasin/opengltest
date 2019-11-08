using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    public class Cursor : Indicator
    {
        public Cursor()
        {
            this.Visible = true;
            this.Size = new GLCoordinate(0.05f, 0.05f);
            this.Depth = 100;
            this.Animation = new Animation(new SpriteSheet_Icons());
            this.Animation.IsStatic = true;
        }

        public void SetCursor(SpriteID sid)
        {
            this.Visible = true;
            this.Animation.SetSprite(sid);

        }

        public void Hide()
        {
            this.Visible = false;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            this.Location = new GameCoordinate(OpenTK.Input.Mouse.GetCursorState().X - GibbWindow.WINX, OpenTK.Input.Mouse.GetCursorState().Y - GibbWindow.WINY);
            Location = CoordinateFuckery.ClickToGLRelativeToCamera(Location, new GameCoordinate(0, 0));
            
            base.DrawStep(drawer);
        }
    }
}
