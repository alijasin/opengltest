using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables.Entities.Indicators;
using OpenGLTests.src.Util;
using Console = System.Console;

namespace OpenGLTests.src.Drawables
{
    public class Cursor : Indicator
    {
        private GameAction action;
        private Marker marker;
        private RangeShape rs;
        private RangeShape markerRs;
        private Player owner;

        public Cursor(Player p)
        {
            this.owner = p;
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

        public override GameCoordinate Location
        {
            get
            {
                var loc = new GameCoordinate(OpenTK.Input.Mouse.GetCursorState().X - GibbWindow.WINX, OpenTK.Input.Mouse.GetCursorState().Y - GibbWindow.WINY);
                var xd=  CoordinateFuckery.ClickToGLRelativeToCamera(loc, new GameCoordinate(0, 0));
                return xd;
            }
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (Visible == false) return;

            if (rs != null)
            {
                if (action.PlacementFilter(Location))
                {
                    this.Color = Color.White;
                }
                else
                {
                    this.Color = Color.Red;
                }
                rs.DrawStep(drawer);
            }

            if (marker != null)
            {
                marker.Location = Location;
            }
            if (markerRs != null)
            {
                markerRs.DrawStep(drawer);
                if (action is CombatAction || action is ItemAction)
                {
                    foreach (var drawable in GameState.Drawables.GetAllUnits)
                    {
                        if (drawable == owner.Hero) continue;

                        if (markerRs.Contains(drawable.Location))
                        {
                            drawer.DrawEffect(new WithinCastAreaIndicator(new GameCoordinate(drawable.Location.X, drawable.Location.Y - drawable.Size.Y)));
                        }
                    }
                }
            }
 
            base.DrawStep(drawer);
        }

        public void SetIcon(SpriteID sid)
        {
            setCursor(sid);
        }

        public void SetAction(GameAction action)
        {
            this.action = action; 

            setCursor(action.Icon);
            setRangeShape(action.RangeShape);
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
            if (rs == null) return;
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
