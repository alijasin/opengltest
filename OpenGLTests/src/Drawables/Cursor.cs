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
        private Marker marker;
        private RangeShape rs;
        private RangeShape markerRs;

        public Cursor()
        {
            this.Visible = true;
            this.Size = new GLCoordinate(0.05f, 0.05f);
            this.Depth = 100;
            this.Animation = new Animation(new SpriteSheet_Icons());
            this.Animation.IsStatic = true;
        }

        public void Hide()
        {
            this.Visible = false;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (Visible == false) return;

            this.Location = new GameCoordinate(OpenTK.Input.Mouse.GetCursorState().X - GibbWindow.WINX, OpenTK.Input.Mouse.GetCursorState().Y - GibbWindow.WINY);
            this.Location = CoordinateFuckery.ClickToGLRelativeToCamera(Location, new GameCoordinate(0, 0));

            if (marker != null) marker.Location = Location;
            if (rs != null) rs.DrawStep(drawer);
            if (markerRs != null) markerRs.DrawStep(drawer);

            base.DrawStep(drawer);
        }

        public void SetAction(GameAction action)
        {
            setCursor(action.Icon);
            if(rs != null) setRangeShape(rs);
            if (action.Marker != null)
            {
                if (action.Marker is AOEMarker aoe)
                {
                    setMarkerRangeShape(aoe.AOEShape);
                }
            }

            this.marker = action.Marker;
        }

        private void setCursor(SpriteID sid)
        {
            this.Visible = true;
            this.Animation.SetSprite(sid);
        }

        private void setRangeShape(RangeShape rs)
        {
            this.rs = rs;
            this.rs.Visible = true;
        }

        private void setMarkerRangeShape(RangeShape rs)
        {
            this.markerRs = rs;
            this.markerRs.Visible = true;
        }

    }
}
