using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src
{
    public class Animation
    {
        private SpriteSheet spriteSheet;
        private int spriteIndex = 0;
        public bool IsStatic = false;

        public Animation(SpriteSheet sheet)
        {
            spriteSheet = sheet;
            
            if (sheet is SpriteSheet_Icons)
            {
                IsStatic = true;
            }
        }

        //todo: assert this actually works.
        public void SetSprite(SpriteID sid)
        {
            spriteIndex = spriteSheet.IndexOfSpriteID(sid);
        }

        public Sprite GetSprite()
        {
            if (!IsStatic) spriteIndex++;
            if (spriteIndex == spriteSheet.NumberOfSprites()) spriteIndex = 0;
            return spriteSheet.GetSprite(spriteIndex);
        }
    }

    class AnimationRange
    {
        private int frameStart;
        private int frameEnd;
        public int StartFrame => frameStart;
        public int EndFrame => frameEnd - 1;

        public AnimationRange(int frameStart, int frameEnd)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd;
        }

        public bool WithinRange(int index)
        {
            return (index >= frameStart && index <= frameEnd);
        }
    }

    public class FrameOrder
    {
        private Dictionary<AnimationRange, Sprite> frameRangeToSprite = new Dictionary<AnimationRange, Sprite>();
        public int TotalFrames = 0;

        public void AddSprite(Sprite sprite, int frameTime)
        {
            frameRangeToSprite.Add(new AnimationRange(TotalFrames, TotalFrames + frameTime), sprite);
            TotalFrames += frameTime;
        }

        public int GetIndex(SpriteID sid)
        {
            foreach (var kvp in frameRangeToSprite)
            {
                if (kvp.Value.sid == sid) return kvp.Key.EndFrame;
            }

            return 0;
        }

        public Sprite GetSprite(int frame)
        {
            foreach (var sw in frameRangeToSprite)
            {
                if (sw.Key.WithinRange(frame)) return sw.Value;
            }
            Console.WriteLine("Trying to draw something with an animatino that hasnt a set sprite index");
            return null;
        }
    }

    public abstract class SpriteSheet
    {
        protected FrameOrder frameOrder = new FrameOrder();

        public SpriteSheet(Dictionary<SpriteID, int> sidsAndFrames)
        {
            foreach (var sidAndFrame in sidsAndFrames)
            {
                frameOrder.AddSprite(new Sprite(sidAndFrame.Key), sidAndFrame.Value);
            }
        }

        public int IndexOfSpriteID(SpriteID sid)
        {
            return frameOrder.GetIndex(sid);
        }

        internal int NumberOfSprites()
        {
            return frameOrder.TotalFrames;
        }

        public Sprite GetSprite(int indexFrame)
        {
            return frameOrder.GetSprite(indexFrame);
        }
    }

    class SpriteSheet_ElfIdle : SpriteSheet
    {
        public SpriteSheet_ElfIdle() : base(new Dictionary<SpriteID, int>()
        {   { SpriteID.elf_m_idle_anim_f0, 12 },
            { SpriteID.elf_m_idle_anim_f1, 12 },
            { SpriteID.elf_m_idle_anim_f2, 12 },
            { SpriteID.elf_m_idle_anim_f3, 12 }
        })
        { }
    }

    class SpriteSheet_Floor : SpriteSheet
    {
        public SpriteSheet_Floor() : base(new Dictionary<SpriteID, int>()
        {   { SpriteID.floor_1, 12 },
            { SpriteID.floor_2, 12 },
            { SpriteID.floor_3, 12 },
            { SpriteID.floor_4, 12 },
            { SpriteID.floor_5, 12 },
            { SpriteID.floor_6, 12 },
            { SpriteID.floor_7, 12 },
            { SpriteID.floor_8, 12 },
        })
        { }
    }

    class SpriteSheet_Actions : SpriteSheet
    {
        public SpriteSheet_Actions() : base(new Dictionary<SpriteID, int>()
        {   { SpriteID.action_move, 5 },
            { SpriteID.action_attack, 5 },
            { SpriteID.action_charge, 2 }
        })
        { }
    }

    class SpriteSheet_Icons : SpriteSheet
    {
        public SpriteSheet_Icons() : base(new Dictionary<SpriteID, int>()
        {   { SpriteID.missing, 5 },
            { SpriteID.item_flask_big_red, 5 },
            { SpriteID.item_flask_big_green, 5 },
            { SpriteID.item_apple, 5 },
            { SpriteID.action_move, 5 },
            { SpriteID.action_attack, 5 },
            { SpriteID.action_charge, 5 },
            { SpriteID.floor_1, 5 }

        })
        { }
    }

    public class Sprite
    {
        public int GLID;
        public SpriteID sid;
        public Image Image() => ImageLoader.GetImage(sid);

        public Sprite(SpriteID sid)
        {
            this.sid = sid;
            GLID = ImageLoader.GetBinding(sid);
        }
    }

}
