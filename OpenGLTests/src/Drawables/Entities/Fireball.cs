using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class Fireball : Effect
    {
        private GameCoordinate baseSpeed = new GameCoordinate(0.02f, 0.02f);
        private Unit source;
        private int collisionDamage;
        private bool diesOnImpact = true;
        public bool Finished = false;
        public Fireball(GameCoordinate origin, GameCoordinate direction, int collisionDamage, Unit source, bool diesOnImpact = true)
        {
            this.diesOnImpact = diesOnImpact;
            this.collisionDamage = collisionDamage;
            this.source = source;
            this.Size = new GLCoordinate(0.15f, 0.12f);
            this.Location = origin;
            this.Animation = new Animation(new SpriteSheet_Fireball());
            this.Speed = new GameCoordinate(direction.X*baseSpeed.X, direction.Y*baseSpeed.Y);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
        }

        public override void LogicStep()
        {
            this.Location += Speed;

            foreach (var unit in GameState.Drawables.GetAllUnits)
            {
                if(unit == source) continue;
                
                if (this.Location.CloseEnough(unit.Location, this.Size.X / 2))
                {
                    unit.Damage(collisionDamage);
                    if (diesOnImpact)
                    {
                        Finished = true;
                        break;
                    }
                }
            }

        }
    }
}
