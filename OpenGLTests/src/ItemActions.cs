using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src
{
    //refactor so that we dont store source, but instead call the action with source upon use.
    //todo. i5d7
    public abstract class ItemAction : GameAction
    {
        protected ItemAction(Unit source) : base(source)
        {
            if (ActionLine != null)
            {
                ActionLine.LineType = LineType.Dashed;
            }
        }
    }

    class TossItemAction : ItemAction
    {
        public TossItemAction(Unit source, Item i) : base(source)
        {
            this.Source = source;
            RangeShape = new RangeShape(new Circle(new GLCoordinate(0.5f, 0.5f)), source);
            Marker = new AOEMarker(source.Location);
            var aoeShape = new RangeShape(new Circle(new GLCoordinate(0.05f, 0.05f)), Marker);
            (Marker as AOEMarker).SetShape(aoeShape);
            RangeShape.Visible = false;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                if ((int)o >= 10)
                {
                    var aoeShape = (Marker as AOEMarker).AOEShape;
                    foreach (var others in GameState.Drawables.GetAllUnits.Where(d => d != Source))
                    {
                        if (aoeShape.Contains(others.Location))
                        {
                            others.Damage(1);
                        }
                    }

                    return true;
                }
                return false;
            };
        }
    }

    class TurnRedAction : ItemAction
    {
        public TurnRedAction(Unit source) : base(source)
        {
            IsInstant = true;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                Source.Color = Color.Red;
                return true;
            };
        }
    }

    class GrowAction : ItemAction
    {
        private float maxSize = 0.3f;
        public GrowAction(Unit source, float maxSize = 0.3f) : base(source)
        {
            IsInstant = true;
            this.maxSize = maxSize;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                var index = (int)o;
                if (index < 10)
                {
                    if (Source.Size.X >= maxSize) return true;
                    Source.Size.X = Source.Size.X * 1.1f;
                    Source.Size.Y = Source.Size.Y * 1.1f;
                    return false;
                }
                return true;
            };
        }
    }
}
