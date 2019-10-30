using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;

/// <summary>
/// these are for action bars
/// </summary>
namespace OpenGLTests.src
{
    public abstract class Ability
    {
        public GameAction Action;
        public SpriteID Icon;
    }

    public class Yell : Ability
    {
        public Yell(Unit owner)
        {
            this.Action = new LambdaAction((o) =>
            {
                Console.WriteLine("waaa");
                return true;
            }, owner);
            this.Icon = SpriteID.action_charge;
        }
    }

    public class Teleport : Ability
    {
        public Teleport(Unit owner)
        {
            this.Action = new TeleportAction(new GLCoordinate(0.4f, 0.4f), owner);
            this.Icon = SpriteID.floor_1;
        }
    }

    //todo: consider making all Abilities to hero abilities.
    public class Move : Ability
    {
        public Move(Unit owner)
        {
            this.Action = new HeroMoveAction(new GLCoordinate(0.3f, 0.3f), owner as Hero);
            this.Icon = SpriteID.action_move;
        }
    }

    public class TossBomb : Ability
    {
        public TossBomb(Unit owner)
        {
            
            this.Action = new AOEEffectAction(new GLCoordinate(0.6f, 0.6f), new RangeShape(new Circle(new GLCoordinate(0.2f, 0.2f)), owner), owner); 
            this.Icon = SpriteID.action_attack;
        }
    }
}
