using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    //todo I really really want to rename this to rectangularelement but I'm getting conflict and right now i'm jammin to Abba so i'm not really in the mood for conflicts.
    public class RectangleElement : Element
    {
        public bool Contains(GameCoordinate point)
        {
            var x = Math.Abs(point.X - Location.X);
            var y = Math.Abs(point.Y - Location.Y);
            //todo move this to somewhere else and fuck yourself.
            GLCoordinate clicked = new GLCoordinate(x * 2 / GibbWindow.WIDTH - 1, -(y * 2 / GibbWindow.HEIGHT - 1));
            return this.Location.X - this.Size.X / 2 < clicked.X && this.Location.X + this.Size.X/2 > clicked.X &&
                   this.Location.Y - this.Size.Y / 2 < clicked.Y && this.Location.Y + this.Size.Y / 2 > clicked.Y;
        }
    }
}
