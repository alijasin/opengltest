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
        private bool diesOnImpact;
        public bool Finished = false;

        private RangeShape rs;

        public Fireball(GameCoordinate origin, int dirAngle, int collisionDamage, Unit source, bool diesOnImpact = true)
        {
            this.diesOnImpact = diesOnImpact;
            this.collisionDamage = collisionDamage;
            this.source = source;
            this.Size = new GLCoordinate(0.15f, 0.12f);
            this.Location = origin;
            this.Animation = new Animation(new SpriteSheet_Fireball());
            var direction = new GameCoordinate((float) Math.Cos(dirAngle* Math.PI/ 180), -(float)Math.Sin(dirAngle * Math.PI / 180));
            this.Speed = new GameCoordinate(direction.X*baseSpeed.X, direction.Y*baseSpeed.Y);
            this.Rotation = dirAngle;
            this.rs = new RangeShape(new Rectangle(new GLCoordinate(Size.X/2, Size.Y/2)), this);
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

                if(entity.BoundingBox.RectangleIntersects(this.rs))
                {
                    if (diesOnImpact) Finished = true;
                    if (entity is IDamagable d)
                    {
                        d.Damage(collisionDamage);
                    }
                    break;
                }
            }

        }
    }
}
