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

            foreach (var entity in GameState.Drawables.GetAllCollidables)
            {
                if(entity == source) continue;
                if (entity.BoundingBox.Contains(new GameCoordinate(this.Location.X + this.Size.X/2, this.Location.Y)))
                //if (this.Location.CloseEnough(entity.Location, this.Size.X / 2))
                {
                    if (entity is IDamagable d)
                    {
                        d.Damage(collisionDamage);
                        //Fire f = new Fire(entity.Location, new GLCoordinate(0.4f, 0.4f));
                        //GameState.Drawables.Add(f);
                    }

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
