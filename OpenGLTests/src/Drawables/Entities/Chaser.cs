﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class ChasingPerson : Hostile
    {
        public ChasingPerson(GameCoordinate location)
        {
            this.Location = location;
            this.Speed = new GameCoordinate(0.001f, 0.001f);

            var chasingAggroShape = new RangeShape(new Circle(new GLCoordinate(0.2f, 0.2f)), this);
            GameState.Drawables.Add(chasingAggroShape);
            chasingAggroShape.Visible = true;
            chasingAggroShape.Color = Color.Red;
            
            OutOfCombatActionPattern = new FindAndChaseEntity(this, chasingAggroShape);
            OutOfCombatActionPattern.Loop = true;

            Animation = new Animation(new SpriteSheet_BigDemonRun());
        }
    }
}
