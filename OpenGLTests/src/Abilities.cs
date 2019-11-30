using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Drawables.Entities.Equipment;
using OpenGLTests.src.Util;

namespace OpenGLTests.src
{
    public abstract class Ability
    {
        public GameAction Action;
        public SpriteID Icon => Action.Icon;
    }

    public class UnarmedAbility : Ability
    {
        public UnarmedAbility(Unit owner)
        {
            this.Action = new LambdaAction((o) =>
            {
                return true;
            }, owner);
            Action.Icon = SpriteID.missing;
        }
    }

    public class TeleportAbility : Ability
    {
        public TeleportAbility(Unit owner)
        {
            this.Action = new TeleportAction(new GLCoordinate(0.4f, 0.4f), owner);
            Action.Icon = SpriteID.action_charge;
        }
    }

    public class HookAbility : Ability
    {
        public HookAbility(Unit owner)
        {
            this.Action = new HookAction(owner);
            Action.Icon = SpriteID.big_demon_run_anim_f0;
        }
    }

    public class HookShotAbility : Ability
    {
        public HookShotAbility(Unit owner)
        {
            this.Action = new HookShotAction(owner);
            Action.Icon = SpriteID.big_zombie_run_anim_f0;
        }
    }

    public class MoveAbility : Ability
    {
        public MoveAbility(Unit owner)
        {
            this.Action = new HeroMoveAction(new GLCoordinate(0.3f, 0.3f), owner as Hero);
            Action.Icon = SpriteID.action_move;
        }
    }

    public class SpawnBearTrapAbility : Ability
    {
        public SpawnBearTrapAbility(Unit owner)
        {
            this.Action = new SpawnBearTrapAction(owner);
            Action.Icon = SpriteID.bear_trap_open;
        }
    }

    public class SliceAbility : Ability
    {
        public SliceAbility(Unit owner)
        {
            this.Action = new SliceAction(owner);
            Action.Icon = SpriteID.fire_sword_slash_green;
        }
    }

    public class FireBallAbility : Ability
    {
        public FireBallAbility(Unit owner)
        {
            this.Action = new FireballAction(owner);
            Action.Icon = SpriteID.fireball_0;
        }
    }

    //todo: weapon icon is not set to weapon's gameaction icon, since its the ability that has the icon.
    public class WeaponAbility : Ability
    {
        public WeaponAbility(Unit owner, Weapon w)
        {
            var wepon = (Weapon)Activator.CreateInstance(w.GetType(), owner);
            this.Action = wepon.WeaponAction;
            this.Action.Icon = w.Icon;
        }
    }
}
