using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Elements;
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Drawables.Terrain;

using OpenGLTests.src.Util;
using OpenTK.Graphics.OpenGL;

namespace OpenGLTests.src
{
    public class GameState
    {
        public static List<Player> Players { get; set; }
        public Hero Hero { get; set; }  //todo, move this to Player
        public static DrawableRepository Drawables = new DrawableRepository();
        public static List<StatusEffect> Effects = new List<StatusEffect>();
        public static RainGenerator RainGenerator = new RainGenerator(RainGenerator.RainType.Clear); //todo: move to drawable

        public GameState()
        {
            Players = new List<Player>();
            Hero = new Hero();
            Hero.Location = new GameCoordinate(0, 0);
            Players.Add(new Player(Hero));
            Drawables.Add(Hero);
            Drawables.Add(new RoomLoadRegion(new GameCoordinate(0.8f, 0.8f), RoomLoader.Room.TestSpace));
            Drawables.Add(new FanBoy(new GameCoordinate(0.7f, -0.5f)));
            Drawables.Add(new Campfire(new GameCoordinate(0, 0.3f)));
            Drawables.Add(new BearTrap(new GameCoordinate(0, 0.2f)));
            new Wizard(new GameCoordinate(-0.5f, 0));
            RoomLoader.LoadRoom(RoomLoader.Room.TestEditorOutPut);
        }


        private void LoadTestRoom()
        {
            RoomLoader rl = new RoomLoader();
        }

        private Fight fight = new Fight();
        private int initSteps = 25;
        private int initStepsCount = 0;

        public void Step()
        {
            foreach(var playa in Players) playa.ActiveCamera.Step();

            if (initStepsCount < initSteps)
            {
                initStepsCount++;
                return;
            }

            foreach (var effect in Effects)
            {
                effect.TryApplyEffect();
            }

            for (int i = Effects.Count - 1; i >= 0; i--)
            {
                if(Effects.ElementAt(i).LiveTime == 0) Effects.RemoveAt(i);
            }


            //:: Optimization point
            //this shuold only be movable units
            foreach (var reg in Drawables.GetAllRegions)
            {
                foreach (Unit e in Drawables.GetAllUnits)
                {
                    if (reg.Shape.Contains(e.Location))
                    {
                        reg.OnEntered(e);
                    }
                }
            }

            #region Pause for Fight

            //if we have units in action do fight steps
            //else we do out of combat actions. 
            //this means units not in combat when there is a fight going on will be paused.
            /*if (!fight.LastManStanding()) doFight();
            else
            {
                foreach (var unit in Drawables.GetAllUnits)
                {

                    unit.OutOfCombatStep(outOfCombatIndex);

                }
            }
            */

            #endregion

            if (!fight.LastManStanding())
            {
                var currentFighter = fight.GetCurrentTurn();

                foreach (var h in fight.Heroes())
                {
                    if(h == currentFighter) h.Player.SetCameraToDefault();
                    else h.Player.SetFightCamera(fight);
                }

                if (currentFighter != null) //redundant?
                {
                    currentFighter.CombatStep(fight);
                }
            }

            foreach (var unit in Drawables.GetAllUnits)
            {
                if (unit.InCombat == false)
                {
                    unit.OutOfCombatStep();
                }
            }


            foreach (Unit aggro in Drawables.GetAllUnits
                .Where(c => !(c is Hero) && c.AggroShape != null && c.InCombat == false).ToList())
            {
                if (aggro.AggroShape.Contains(Hero.Location))
                {
                    aggro.OnAggro(Hero);
                    Hero.OnAggro(aggro);
                    fight.AddFighter(Hero);
                    fight.AddFighter(aggro);
                }
            }
        }
    }
}
