﻿using OpenTK.Graphics.ES20;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenGLTests.src.Drawables.Elements;
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Drawables.Entities.Equipment;
using OpenGLTests.src.Drawables.Terrain;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    public enum Facing
    {
        Right,
        UpsideDownRight,
        Left,
        UpsideDownLeft, 
    }

    public abstract class Drawable : ICloneable
    {
        public bool Flipped = false;
        public Facing Facing { get; private set; } = Facing.Right;
        public virtual bool Visible { get; set; }
        public Color Color { get; set; } = Color.White;
        public GLCoordinate Size { get; set; } = new GLCoordinate(0.1f, 0.1f);
        [JsonIgnore]
        public Animation Animation { get; set; }
        public int Depth { get; set; } = 10;
        [JsonIgnore]
        public int ID { get; set; } = IDGenerator.GetID();
        public void SetFacing(Facing f)
        {
            Facing = f;
        }

        public void NextFacing()
        {
            switch (Facing)
            {
                case Facing.UpsideDownRight:
                    Facing = Facing.UpsideDownLeft;
                    break;
                case Facing.UpsideDownLeft:
                    Facing = Facing.Right;
                    break;
                case Facing.Left:
                    Facing = Facing.UpsideDownRight;
                    break;
                case Facing.Right:
                    Facing = Facing.Left;
                    break;
            }
            Console.WriteLine("current facing: " +Facing);
        }

        public void Flip()
        {
            //invert x and y
            this.Size = new GLCoordinate(this.Size.Y, this.Size.X);

            //update bounding box
            //todo: only works for rectangles, but maybe this is fine.
            if (this is ICollidable c)
            {
                c.BoundingBox.Dispose();
                c.BoundingBox = new RangeShape(new Rectangle(Size), c as Entity);
                c.BoundingBox.Visible = true;
            }

            this.Flipped = !this.Flipped;
        }

        //hack? for serialization
        [OnDeserialized]
        private void Flip(System.Runtime.Serialization.StreamingContext xd)
        {
            //update bounding box
            //todo: only works for rectangles, but maybe this is fine.
            if (this is ICollidable c)
            {
                c.BoundingBox.Dispose();
                c.BoundingBox = new RangeShape(new Rectangle(Size), c as Entity);
                c.BoundingBox.Visible = true;
            }
        }

        public virtual void DrawStep(DrawAdapter drawer)
        {
            
        }

        public virtual void Dispose()
        {
            if (this is ICollidable c)
            {
                c.BoundingBox.Dispose();
            }
            GameState.Drawables.Remove(this);
        }

        //create shallow copy
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public interface IDamagable
    {
        int HitPoints { get; set; }
        void Damage(int dmg);
    }

    public interface ICollidable
    {
        bool Phased { get; set; }
        [JsonIgnore]
        RangeShape BoundingBox { get; set; }
        GameCoordinate Location { get; set; }

        BlockedSides BlockedSides { get; set; }
    }

    public class BlockedSides
    {
        public bool BlockedLeft { get; set; } = false;
        public bool BlockedRight { get; set; } = false;
        public bool BlockedTop { get; set; } = false;
        public bool BlockedBottom { get; set; } = false;

        public int BlockedCount()
        {
            int c = 0;
            if (BlockedLeft) c++;
            if (BlockedBottom) c++;
            if (BlockedTop) c++;
            if (BlockedRight) c++;
            return c;
        }
    }

    public abstract class Element : Drawable
    {
        public GLCoordinate Location { get; set; } = new GLCoordinate(0, 0);

        public Element()
        {
            this.Visible = true;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (Visible)
            {
                if (Animation == null)
                {
                    drawer.FillRectangle(Color, Location.X, Location.Y, Size.X, Size.Y);
                }
                else
                {
                    //drawer.FillRectangle(Color, Location.X, Location.Y, Size.X, Size.Y);
                    drawer.DrawElement(this);
                }
            }
        }
    }

    public abstract class Entity : Drawable
    {
        [JsonProperty]
        public GameCoordinate Speed { get; set; } = new GameCoordinate(0, 0);
        [JsonIgnore]
        public virtual GameCoordinate MovingTowardsPoint { get; set; }

        private GameCoordinate location;
        [JsonProperty]
        public virtual GameCoordinate Location
        {
            get
            {
                if (this.location == null) return new GameCoordinate(0, 0);
                return this.location;
            }
            set
            {
                location = value;
            }
        }

        public virtual int FacingAngle { get; set; }
        public Entity()
        {
            this.Visible = true;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            GLCoordinate location = Location.ToGLCoordinate();
            if (Visible)
            {
                if (Animation == null)
                {
                    drawer.FillRectangle(Color, location.X, location.Y, Size.X, Size.Y);
                }
                else
                {
                    drawer.DrawEntity(this);
                }
            }
        }

        public GameCoordinate Center => new GameCoordinate(this.Location.X + this.Size.X/2, this.Location.Y + this.Size.Y/2);
    }

    public abstract class Stuff : Entity, ICollidable
    {
        protected Stuff()
        {
            this.Color = Color.White;
            this.BoundingBox = new RangeShape(new Rectangle(new GLCoordinate(0.1f, 0.1f)), this);
            GameState.Drawables.Add(this);
        }
        
        public bool Phased { get; set; } = false;
        
        public RangeShape BoundingBox { get; set; }
        public BlockedSides BlockedSides { get; set; }
    }

    public abstract class Unit : Entity, IActionCapable, IDamagable, ICollidable
    {
        public bool Phased { get; set; }
        public RangeShape BoundingBox { get; set; }
        public BlockedSides BlockedSides { get; set; }
        [JsonIgnore]
        public virtual int AvailableActionPoints { get; set; } = 3;
        [JsonIgnore]
        public RangeShape AggroShape { get; set; }
        [JsonIgnore]
        public ActionHandler ActionHandler { get; set; }
        [JsonIgnore]
        public GameAction DefaultAction { get; set; } 
    
        [JsonIgnore]
        public int OutOfCombatIndex = 0;
        [JsonIgnore]
        public int CombatIndex = 0;
        [JsonIgnore]
        public ActionStatus ActionStatus { get; set; }
        [JsonIgnore]
        public GameCoordinate InitialSpeed = new GameCoordinate(0.015f, 0.015f);
        [JsonIgnore]
        public bool EndedTurn = false;
        [JsonIgnore]
        public virtual Weapon Weapon { get; set; }
        [JsonIgnore]
        public bool InCombat { get; set; }
        [JsonIgnore]
        public int Initiative { get; set; }
        [JsonIgnore]
        public int HitPoints { get; set; }
        [JsonIgnore]
        public LootTable LootTable { get; set; }

        public bool HasWeapon => Weapon != null;
        public bool DoingWeaponAction => ActionHandler.SelectedAction is WeaponAction && ActionStatus == ActionStatus.Ongoing;
        public virtual GameCoordinate LeftHandLocation { get; set; } //todo: refactor this into animation


        public Unit()
        {
            LootTable = new LootTable();
            BlockedSides = new BlockedSides();
            DefaultAction = new NothingAction(this);
        }


        public void Damage(int dmg)
        {
            Console.WriteLine("Damaged " + this.GetType() + " for " + dmg + " damage.");
            this.HitPoints -= dmg;
            if (HitPoints <= 0) OnDeath();
        }

        public virtual void OnDeath()
        {
            Fight.RemoveFighter(this);
            LootTable.DropLoot(this.Location);
        }

        public abstract void OnAggro(Unit aggroed);

        public virtual void OnPostTurn()
        {

        }

        public virtual void OnPreTurn()
        {

        }

        public override void Dispose()
        {
            ActionHandler.Dispose();
            base.Dispose();
        }


        public abstract void CombatStep(Fight fight);

        public abstract void OutOfCombatStep();

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            if (Weapon != null)
            {
                if(!(Weapon is EmptyWeapon)) Weapon.DrawStep(drawer);
            }
        }
    }

    public abstract class Effect : Entity
    {
        protected GameCoordinate Origin { get; set; }
        public int Rotation { get; set; } = 0;
        public virtual void LogicStep()
        {

        }

        public override void DrawStep(DrawAdapter drawer)
        {
            drawer.DrawEffect(this);
        }
    }

    public abstract class Structure : Entity, ICollidable
    {
        public bool Phased { get; set; } = false;
        [JsonIgnore]
        public virtual RangeShape BoundingBox { get; set; }

        public BlockedSides BlockedSides { get; set; }

        public Structure()
        {
            this.BoundingBox = new RangeShape(new Rectangle(new GLCoordinate(0.1f, 0.1f)), this);
        }
    }

    public abstract class Indicator : Entity
    {
    }
}
