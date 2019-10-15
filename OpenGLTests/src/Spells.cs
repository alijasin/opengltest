using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;

namespace OpenGLTests.src
{
    public abstract class Spell
    {
        public GameAction Action;
        public SpriteID Icon;
    }

    public class Yell : Spell
    {
        public Yell(Entity owner)
        {
            this.Action = new LambdaAction((o) =>
            {
                Console.WriteLine("waaa");
                return true;
            });
            this.Icon = SpriteID.action_charge;
        }
    }

    public class Teleport : Spell
    {
        public Teleport(Entity owner)
        {
            this.Action = new TeleportAction(new GLCoordinate(0.4f, 0.4f), owner);
            this.Icon = SpriteID.floor_1;
        }
    }

    public class Move : Spell
    {
        public Move(Entity owner)
        {
            this.Action = new MoveAction(new GLCoordinate(0.3f, 0.3f), owner);
            this.Icon = SpriteID.action_move;
        }
    }

    public class TossBomb : Spell
    {
        public TossBomb(Entity owner)
        {
            this.Action = new AOEEffectAction(new GLCoordinate(0.6f, 0.6f), new GLCoordinate(0.2f, 0.2f));
            this.Icon = SpriteID.action_attack;
        }
    }
}
