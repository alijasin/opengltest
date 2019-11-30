using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Entities;

namespace OpenGLTests.src
{
    public abstract class ItemAction : GameAction
    {
        public Item Item { get; set; }

        protected ItemAction(Unit source, Item i) : base(source)
        {
            Item = i;
            if (ActionLine != null)
            {
                ActionLine.LineType = LineType.Dashed;
            }

            OnFinished = () =>
            {
                if (source is Hero h)
                {
                    if(Item != null) h.Inventory.Remove(Item);
                }
            };
        }
    }

    class TossItemAction : ItemAction
    {
        public TossItemAction(Unit source, Item i = default(Item)) : base(source, i)
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
                if ((int)o >= 20)
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

    class TurnBlueAction : ItemAction
    {
        public TurnBlueAction(Unit source, Item i) : base(source, i)
        {
            IsInstant = true;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                Source.Color = Color.Blue;
                return true;
            };
        }
    }

    class TurnRedAction : ItemAction
    {
        public TurnRedAction(Unit source, Item i = default(Item)) : base(source, i)
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
        public GrowAction(Unit source, Item i = default(Item), float maxSize = 0.3f) : base(source, i)
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
